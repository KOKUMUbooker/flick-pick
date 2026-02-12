<script lang="ts">
	import { cn } from '$lib/utils.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import * as Field from '$lib/components/ui/field/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import type { HTMLAttributes } from 'svelte/elements';
	import { resetPasswordBaseSchema, resetPasswordSchema } from '@/forms';
	import HelperText from '../common/HelperText.svelte';
	let { class: className, ...restProps }: HTMLAttributes<HTMLFormElement> = $props();

	let formData = $state({
		OldPassword: '',
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

	function onSubmit(event: SubmitEvent) {
		event.preventDefault();

		// mark everything touched
		Object.keys(formData).forEach((key) => (touched[key] = true));

		const result = resetPasswordSchema.safeParse(formData);

		if (!result.success) {
			errors = Object.fromEntries(
				result.error.issues.map((issue) => [issue.path[0] as string, issue.message])
			);
			return;
		}

		// fully valid data
		console.log('SUBMIT OK', result.data);

		// continue: API call, navigation, etc.
	}
</script>

<form onsubmit={onSubmit} class={cn('flex flex-col gap-6', className)} {...restProps}>
	<Field.Group>
		<div class="flex flex-col items-center gap-1 text-center">
			<h1 class="text-2xl font-bold">Reset password</h1>
		</div>
		<Field.Field>
			<Field.Label for="NewPassword">New</Field.Label>
			<Input
				id="NewPassword"
				bind:value={formData.NewPassword}
				onblur={onBlur.bind(null, 'NewPassword')}
				oninput={() => touched.NewPassword && validateField('NewPassword')}
				class={`${errors?.['NewPassword'] ? 'border-destructive' : ''}`}
				type="NewPassword"
			/>
			<HelperText
				variant={errors?.['NewPassword'] ? 'error' : 'info'}
				message={'Must be at least 8 characters long.'}
			></HelperText>
		</Field.Field>

		<Field.Field>
			<Field.Label for="passwordConfirm">Confirm Password</Field.Label>
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
			<Button type="submit">Reset password</Button>
		</Field.Field>
		<Field.Field>
			<Field.Description class="px-6 text-center">
				<a href="/login">Back to log in</a>
			</Field.Description>
		</Field.Field>
	</Field.Group>
</form>
