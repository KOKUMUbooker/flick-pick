<script lang="ts">
	import { goto } from '$app/navigation';
	import { page } from '$app/stores';
	import { Button } from '$lib/components/ui/button';
	import { Home } from '@lucide/svelte';

	export let error: unknown;

	$: status = $page.status || 500;
	$: message =
		error && typeof error === 'object' && 'message' in error
			? String(error.message)
			: 'Something went wrong';
</script>

<div class="flex min-h-screen flex-col items-center justify-center gap-6 bg-background p-4">
	<div class="text-center">
		<div class="mb-4 text-7xl font-bold text-destructive">{status}</div>
		<p class="mb-8 max-w-sm text-muted-foreground">{message}</p>
		<Button size="lg" onclick={() => goto('/')}>
			<Home class="mr-2 h-5 w-5" />
			Go Home
		</Button>
	</div>
</div>
