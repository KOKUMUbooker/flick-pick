<script lang="ts">
	import { Button } from '$lib/components/ui/button/index.js';
	import {
		Field,
		FieldDescription,
		FieldGroup,
		FieldLabel,
		Label
	} from '$lib/components/ui/field/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { cn, type WithElementRef } from '$lib/utils.js';
	import { loginSchema } from '@/forms';
	import type { HTMLFormAttributes } from 'svelte/elements';
	import HelperText from '../common/HelperText.svelte';

	let {
		ref = $bindable(null),
		class: className,
		...restProps
	}: WithElementRef<HTMLFormAttributes> = $props();
	let formData = $state({
		Email: '',
		Password: ''
	});
	let errors: Record<string, string> = $state({});
	let touched: Record<string, boolean> = $state({});

	function validateField(field: keyof typeof formData) {
		const partial = loginSchema.pick({ [field]: true } as any);
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

	function onSubmit(event: SubmitEvent) {
		event.preventDefault();

		// mark everything touched
		Object.keys(formData).forEach((key) => (touched[key] = true));

		const result = loginSchema.safeParse(formData);

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
			<h1 class="text-2xl font-bold">Login to your account</h1>
			<p class="text-sm text-balance text-muted-foreground">
				Enter your email below to login to your account
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
			<div class="flex items-center">
				<FieldLabel for="password-{id}">Password</FieldLabel>
				<a href="/forgot-password" class="ms-auto text-sm underline-offset-4 hover:underline">
					Forgot your password?
				</a>
			</div>
			<Input
				id="Password"
				bind:value={formData.Password}
				onblur={onBlur.bind(null, 'Password')}
				oninput={() => touched.Password && validateField('Password')}
				class={`${errors?.['Password'] ? 'border-destructive' : ''}`}
				type="password"
			/>
			<HelperText
				variant={errors?.['Password'] ? 'error' : 'info'}
				message={'Must be at least 8 characters long.'}
			></HelperText>
		</Field>
		<Field>
			<Button type="submit">Login</Button>
		</Field>
		<Field>
			<FieldDescription class="text-center">
				Don't have an account?
				<a href="/signup" class="underline underline-offset-4">Sign up</a>
			</FieldDescription>
		</Field>
	</FieldGroup>
</form>
