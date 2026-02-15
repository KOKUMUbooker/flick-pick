<script lang="ts">
	import { goto } from '$app/navigation';
	import { page } from '$app/state';
	import mvImg from '$lib/assets/movie.jpg';
	import Button from '@/components/ui/button/button.svelte';
	import { ArrowRight, MailCheck } from '@lucide/svelte';

	// Extract 'email' from URL query params
	let email = page.url.searchParams.get('email');

	let loginRoute = '/login';
	if (email) loginRoute += `?email=${email}`;
</script>

<div class="grid min-h-svh lg:grid-cols-2">
	<div class="flex flex-col gap-4 p-6 md:p-10">
		<div class="flex flex-1 items-center justify-center">
			<div class="w-full max-w-xs">
				<!-- Header -->
				<div class="mb-8 text-center">
					<div
						class="mb-4 inline-flex h-14 w-14 items-center justify-center rounded-full bg-primary/10"
					>
						<MailCheck class="h-7 w-7 text-primary" />
					</div>
					<h1 class="text-2xl font-bold">Email verified 🎉</h1>
					<p class="mt-2 text-muted-foreground">
						Thanks for verifying your email — your account is now fully active.
					</p>
					<Button class="mt-4 w-full" onclick={() => goto(loginRoute)}>
						Go to Login
						<ArrowRight class="ml-2 h-4 w-4" />
					</Button>
				</div>
			</div>
		</div>
	</div>
	<div class="relative hidden bg-muted lg:block">
		<img
			src={mvImg}
			alt="placeholder"
			class="absolute inset-0 h-full w-full object-cover dark:brightness-[0.8] dark:grayscale"
		/>
	</div>
</div>
