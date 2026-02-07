<script lang="ts">
	import favicon from '$lib/assets/favicon.svg';
	import Header from '@/components/common/Header.svelte';
	import { QueryClientProvider } from '@tanstack/svelte-query';
	import { ModeWatcher } from 'mode-watcher';
	import { queryClient } from '../api';
	import './layout.css';
	import { Toaster } from '@/components/ui/sonner';

	let { children } = $props();
</script>

<svelte:head><link rel="icon" href={favicon} /></svelte:head>
<QueryClientProvider client={queryClient}>
	<div class="mb-16">
		<Header />
	</div>
	<ModeWatcher />
	<Toaster />
	{@render children()}
</QueryClientProvider>
