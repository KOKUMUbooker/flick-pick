<script lang="ts">
	import { Button } from '$lib/components/ui/button/index.js';
	import { Field, FieldDescription, FieldGroup, Label } from '$lib/components/ui/field/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { cn, type WithElementRef } from '$lib/utils.js';
	import { forgotPasswordSchema } from '@/forms';
	import type { HTMLFormAttributes } from 'svelte/elements';
	import HelperText from '../common/HelperText.svelte';
	import { createMutation } from '@tanstack/svelte-query';
	import { apiFetch } from '../../../api';
	import { API_BASE_URL } from '../../../api/urls';
	import { toast } from 'svelte-sonner';
	import Spinner from '../ui/spinner/spinner.svelte';
	let {
		ref = $bindable(null),
		class: className,
		...restProps
	}: WithElementRef<HTMLFormAttributes> = $props();
	const id = $props.id();

	let formData = $state({
		Email: ''
	});
	let errors: Record<string, string> = $state({});
	let touched: Record<string, boolean> = $state({});

	function validateField(field: keyof typeof formData) {
		const partial = forgotPasswordSchema.pick({ [field]: true } as any);
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


	let forgotPasswordMutation = createMutation<
		{message:string}, // response type
		Error, // error type
		{Email:string} // variables type
	>(() => ({
		mutationFn: async (data) => {
			return apiFetch(`${API_BASE_URL}/api/auth/forgot-password`, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(data)
			});
		}
	}));

	async function onSubmit(event: SubmitEvent) {
		event.preventDefault();

		// mark everything touched
		Object.keys(formData).forEach((key) => (touched[key] = true));

		const result = forgotPasswordSchema.safeParse(formData);

		if (!result.success) {
			errors = Object.fromEntries(
				result.error.issues.map((issue) => [issue.path[0] as string, issue.message])
			);
			return;
		}

		const res = await forgotPasswordMutation.mutateAsync(result.data)
		toast.success(res.message,{richColors:true})
	}
</script>

<form
	onsubmit={onSubmit}
	class={cn('flex flex-col gap-6', className)}
	bind:this={ref}
	{...restProps}
>
	<FieldGroup>
		<div class="flex flex-col items-center gap-1 text-center">
			<h1 class="text-2xl font-bold">Forgot password</h1>
			<p class="text-sm text-balance text-muted-foreground">
				Enter the email that will receive the password reset link
			</p>
		</div>
		<Field>
			<Label for="Email">Email</Label>
			<Input
				id="Email"
				type="email"
				bind:value={formData.Email}
				class={`${errors?.['Email'] ? 'border-destructive' : ''}`}
				placeholder="m@example.com"
				onblur={onBlur.bind(null, 'Email')}
				oninput={() => touched.Email && validateField('Email')}
			/>
			<HelperText variant="error" message={errors?.['Email'] || ''} show={!!errors?.['Email']}
			></HelperText>
		</Field>
		<Field>
			<Button type="submit" disabled={forgotPasswordMutation.isPending}>
				{#if forgotPasswordMutation.isPending}
					<Spinner />
				{/if}
				{forgotPasswordMutation.isPending ? 'Submitting...' : 'Submit'} 
			</Button>
		</Field>
		<Field>
			<FieldDescription class="text-center">
				<a href="/login" class="underline underline-offset-4">Back to login</a>
			</FieldDescription>
		</Field>
	</FieldGroup>
</form>
