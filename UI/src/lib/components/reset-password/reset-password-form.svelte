<script lang="ts">
	import { cn } from '$lib/utils.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import * as Field from '$lib/components/ui/field/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import type { HTMLAttributes } from 'svelte/elements';
	import { resetPasswordBaseSchema, resetPasswordSchema } from '@/forms';
	import HelperText from '../common/HelperText.svelte';
	import { page } from '$app/state';
	import { createMutation } from '@tanstack/svelte-query';
	import { API_BASE_URL } from '../../../api/urls';
	import { apiFetch, type PasswordResetData } from '../../../api';
	import { toast } from 'svelte-sonner';
	import { goto } from '$app/navigation';
	import Spinner from '../ui/spinner/spinner.svelte';
	let { class: className, ...restProps }: HTMLAttributes<HTMLFormElement> = $props();
	
	let searchParams = page.url.searchParams;
	let token = $derived(searchParams.get('tkn'));
	let email = $derived(searchParams.get('email'));

	let formData = $state({
		NewPassword: '',
		PasswordConfirm: ''
	});

	let errors: Record<string, string> = $state({});
	let touched: Record<string, boolean> = $state({});

	function validateField(field: keyof typeof formData) {
		const partial = resetPasswordBaseSchema.pick({ [field]: true } as any);
		const result = partial.safeParse({ [field]: formData[field] });

		if (!result.success) {
			errors[field] = result.error.issues[0].message;
		} else {
			delete errors[field];
		}
	}

	function validatePasswords() {
		const result = resetPasswordSchema.safeParse(formData);

		const issue = result.success
			? null
			: result.error.issues.find((i) => i.path[0] === 'PasswordConfirm');

		if (issue) {
			errors.PasswordConfirm = issue.message;
		} else {
			delete errors.PasswordConfirm;
		}
	}

	function onBlur(field: keyof typeof formData) {
		touched[field] = true;
		validateField(field);
	}

	let resetPasswordMutation = createMutation<
		{ message : string }, // response type
		Error, // error type
		PasswordResetData // variables type
	>(() => ({
		mutationFn: async (data) => {
			return apiFetch(`${API_BASE_URL}/api/auth/reset-password`, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(data)
			});
		}
	}));

	async function onSubmit(event: SubmitEvent) {
		event.preventDefault();

		if (!token) {
			toast.error("No token found",{richColors:true})
			return
		}

		// mark everything touched
		Object.keys(formData).forEach((key) => (touched[key] = true));

		const result = resetPasswordSchema.safeParse(formData);

		if (!result.success) {
			errors = Object.fromEntries(
				result.error.issues.map((issue) => [issue.path[0] as string, issue.message])
			);
			return;
		}

		const data : PasswordResetData = {NewPassword : result.data.NewPassword, PasswordVerificationToken :token} 
		const res = await resetPasswordMutation.mutateAsync(data)
		toast.success(res.message, {richColors: true})

		let url = '/login'
		if (email) url += `?email=${email}`
		goto(url)
	}
</script>

<form onsubmit={onSubmit} class={cn('flex flex-col gap-6', className)} {...restProps}>
	<Field.Group>
		<div class="flex flex-col items-center gap-1 text-center">
			<h1 class="text-2xl font-bold">Reset password</h1>
		</div>
		<Field.Field>
			<Field.Label for="NewPassword">New password</Field.Label>
			<Input
				id="NewPassword"
				bind:value={formData.NewPassword}
				onblur={onBlur.bind(null, 'NewPassword')}
				oninput={() => touched.NewPassword && validateField('NewPassword')}
				class={`${errors?.['NewPassword'] ? 'border-destructive' : ''}`}
				type="password"
			/>
			<HelperText
				variant={errors?.['NewPassword'] ? 'error' : 'info'}
				message={'Must be at least 8 characters long.'}
			></HelperText>
		</Field.Field>

		<Field.Field>
			<Field.Label for="passwordConfirm">Confirm password</Field.Label>
			<Input
				id="passwordConfirm"
				class={`${errors?.['PasswordConfirm'] ? 'border-destructive' : ''}`}
				bind:value={formData.PasswordConfirm}
				onblur={validatePasswords}
				oninput={() => touched.PasswordConfirm && validateField('PasswordConfirm')}
				type="password"
			/>
			<HelperText
				variant="error"
				message={errors?.['PasswordConfirm'] || ''}
				show={!!errors?.['PasswordConfirm']}
			></HelperText>
		</Field.Field>

		<Field.Field>
			<Button disabled={resetPasswordMutation.isPending} type="submit">
				{#if resetPasswordMutation.isPending}
					<Spinner />
				{/if}
				{resetPasswordMutation.isPending ? 'Resetting password...' : 'Reset password'} 
				
			</Button>
		</Field.Field>
		<Field.Field>
			<Field.Description class="px-6 text-center">
				<a href="/login">Back to log in</a>
			</Field.Description>
		</Field.Field>
	</Field.Group>
</form>
