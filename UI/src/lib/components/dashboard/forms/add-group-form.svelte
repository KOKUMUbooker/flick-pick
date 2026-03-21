<script lang="ts">
	import { Button } from '$lib/components/ui/button/index.js';
	import { Field, FieldGroup, Label } from '$lib/components/ui/field/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { cn, type WithElementRef } from '$lib/utils.js';
	import { addGroupSchema } from '@/forms/add-group-schema';
	import { createMutation } from '@tanstack/svelte-query';
	import { toast } from 'svelte-sonner';
	import type { HTMLFormAttributes } from 'svelte/elements';
	import { apiFetch, QUERY_KEYS, queryClient } from '../../../../api';
	import type { AddGroupData, AddGroupRes } from '../../../../api/types';
	import { API_BASE_URL } from '../../../../api/urls';
	import { getAppUser } from '../../../../store';
	import HelperText from '../../common/HelperText.svelte';
	import Spinner from '../../ui/spinner/spinner.svelte';

	interface AddGroupFromProps extends WithElementRef<HTMLFormAttributes> {
		defaultValues? : { Id : string ,Name : string, Description : string };
		onOpenChange: (open: boolean) => void;
	}

	let {
		ref = $bindable(null),
		defaultValues,
		onOpenChange,
		class: className,
		...restProps
	}: AddGroupFromProps = $props();
	let formData = $state({
		Id: defaultValues?.Id || "",
		Name: defaultValues?.Name || '',
		Description: defaultValues?.Description || ''
	});
	let errors: Record<string, string> = $state({});
	let touched: Record<string, boolean> = $state({});

	let upsertGroupMutation = createMutation<
		AddGroupRes, // response type
		Error, // error type
		AddGroupData // variables type
	>(() => ({
		mutationFn: async (data) => {
			return apiFetch(`${API_BASE_URL}/api/groups`, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(data)
			});
		},
		onSuccess: async () => {
			await queryClient.invalidateQueries({ queryKey:  [QUERY_KEYS.GROUPS + user?.id || ""] });
		}
	}));
	const user = getAppUser();

	function validateField(field: keyof typeof formData) {
		const partial = addGroupSchema.pick({ [field]: true } as any);
		const result = partial.safeParse({ [field]: formData[field] });

		if (!result.success) {
			errors[field] = result.error.issues[0].message;
		} else {
			delete errors[field];
		}
	}

	function onBlur(field: keyof typeof formData) {
		touched[field] = true;
		validateField(field);
	}

	async function onSubmit(event: SubmitEvent) {
		event.preventDefault();

		if (!user) {
			return toast.error('Please refresh the page or login', { richColors: true });
		}

		// mark everything touched
		Object.keys(formData).forEach((key) => (touched[key] = true));

		const result = addGroupSchema.safeParse(formData);

		if (!result.success) {
			errors = Object.fromEntries(
				result.error.issues.map((issue) => [issue.path[0] as string, issue.message])
			);
			return;
		}

		const res = await upsertGroupMutation.mutateAsync({ ...formData, UserId: user.id });
		toast.success(res.message, { richColors: true });
		onOpenChange(false);
	}

	const id = $props.id();
</script>

<form
	onsubmit={onSubmit}
	class={cn('flex flex-col gap-6', className)}
	bind:this={ref}
	{...restProps}
>
	<FieldGroup>
		<div class="flex flex-col items-center gap-1 text-center">
			<h1 class="text-2xl font-bold">{formData.Id ? "Update" :"Create"} movie group</h1>
		</div>
		<Field>
			<Label for="Name">Name</Label>
			<Input
				id="Name"
				bind:value={formData.Name}
				class={`${errors?.['Name'] ? 'border-destructive' : ''}`}
				placeholder="Horror Nights"
				onblur={onBlur.bind(null, 'Name')}
				oninput={() => touched.Name && validateField('Name')}
			/>
			<HelperText variant="error" message={errors?.['Name'] || ''} show={!!errors?.['Name']}
			></HelperText>
		</Field>
		<Field>
			<Label for="Description">Description</Label>
			<Input
				id="Description"
				bind:value={formData.Description}
				class={`${errors?.['Description'] ? 'border-destructive' : ''}`}
				placeholder="For fans of horror"
				onblur={onBlur.bind(null, 'Description')}
				oninput={() => touched.Description && validateField('Description')}
			/>
			<HelperText
				variant="error"
				message={errors?.['Description'] || ''}
				show={!!errors?.['Description']}
			></HelperText>
		</Field>

		<Field>
			<Button disabled={upsertGroupMutation.isPending} type="submit">
				{#if upsertGroupMutation.isPending}
					<Spinner />
				{/if}
				{upsertGroupMutation.isPending ? 
					formData.Id ? 'Updating group...' :'Creating group...' : 
					formData.Id ? 'Update group' :'Create group'
				}
			</Button
			>
		</Field>
	</FieldGroup>
</form>
