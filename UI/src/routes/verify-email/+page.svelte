<!-- src/routes/verify-email/+page.svelte -->
<script lang="ts">
	import { goto } from '$app/navigation';
	import mvImg from '$lib/assets/movie.jpg';
	import { Alert, AlertDescription } from '$lib/components/ui/alert';
	import { Button } from '$lib/components/ui/button';
	import { Mail, RefreshCw } from '@lucide/svelte';
	import { onDestroy, onMount } from 'svelte';
	import { apiFetch, type SignUpRes } from '../../api';
	import { createMutation } from '@tanstack/svelte-query';
	import { API_BASE_URL } from '../../api/urls';
	import { toast } from 'svelte-sonner';
	import { page } from '$app/stores';

	let noTokenFoundMessage = false;
 	let isVerifyingEmail = false;
	let resendCooldown = 0;
	let cooldownTimer: number | null = null;
	$: token = $page.url.searchParams.get('tkn');

	onMount(async () => {
		const urlParams = new URLSearchParams(window.location.search);
		token = urlParams.get('tkn') || '';

		if (!token) {
			noTokenFoundMessage = true;
			return;
		}
	});

	onDestroy(() => {
		if (cooldownTimer) {
			clearInterval(cooldownTimer);
		}
	});

	function startCooldown(seconds: number) {
		resendCooldown = seconds;

		if (cooldownTimer) {
			clearInterval(cooldownTimer);
		}

		cooldownTimer = window.setInterval(() => {
			resendCooldown -= 1;

			if (resendCooldown <= 0) {
			resendCooldown = 0;
			clearInterval(cooldownTimer!);
			cooldownTimer = null;
			}
		}, 1000);
	}
	

	let { isPending, mutateAsync } = createMutation<
		SignUpRes, // response type
		Error, // error type
		{Token: string} // variables type
	>(() => ({
		mutationFn: (data) =>
			apiFetch(`${API_BASE_URL}/api/auth/resend-verification`, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(data)
			}),
		onError: (error) => {
			toast.error(error.message, { richColors: true });
		}
	}));

	// async function verifyEmail(token: string) {}

	async function resendVerification() {
		if (!token) {
			toast.error("Verification token missing",{richColors:true});
			return
		}
		const res = await mutateAsync({Token: token})
		toast.success(res.message,{richColors: true})

 		startCooldown(60);

    	goto(`/verify-email?tkn=${res.emailVerificationToken}`, { replaceState: true });
	}

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
						{#if isVerifyingEmail}
							<div
								class="h-7 w-7 animate-spin rounded-full border-2 border-primary border-t-transparent"
							></div>
						{:else}
							<Mail class="h-7 w-7 text-primary" />
						{/if}
					</div>
					<h1 class="text-2xl font-bold">Email verification</h1>
					<p class="mt-2 text-muted-foreground">
						A link was sent to your email, please click it to activate your account or in case it
						has expired request for a new one by pressing resend verification
					</p>
				</div>

				{#if noTokenFoundMessage}
					<Alert variant={'destructive'} class="mb-6">
						<AlertDescription class="text-center">
							Verification link is invalid or missing.
						</AlertDescription>
					</Alert>
				{/if}

				<!-- Progress Bar -->
				{#if isVerifyingEmail}
					<div class="mb-6">
						<div class="h-1 w-full overflow-hidden rounded-full bg-muted">
							<div class="h-full animate-pulse rounded-full bg-primary" style="width: 60%;"></div>
						</div>
						<p class="mt-2 text-center text-xs text-muted-foreground">Processing...</p>
					</div>
				{/if}

				<!-- Actions -->
				<div class="space-y-3">
					<Button
						class="w-full"
						onclick={resendVerification}
						disabled={isPending || resendCooldown > 0 || !token }
					>
						{#if isPending}
							<RefreshCw class="mr-2 h-4 w-4 animate-spin" />
							Sending...
						{:else if resendCooldown > 0}
							Resend ({resendCooldown}s)
						{:else}
							<Mail class="mr-2 h-4 w-4" />
							Resend Verification
						{/if}
					</Button>
					<Button variant="outline" class="w-full" onclick={() => goto('/login')}>
						Back to Login
					</Button>
				</div>
			</div>
		</div>
	</div>

	<!-- Background Image -->
	<div class="relative hidden bg-muted lg:block">
		<img
			src={mvImg}
			alt="Movie background"
			class="absolute inset-0 h-full w-full object-cover dark:brightness-[0.8] dark:grayscale"
		/>
	</div>
</div>
