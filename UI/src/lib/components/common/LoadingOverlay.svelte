<!-- src/lib/components/ui/loading-overlay.svelte -->
<script lang="ts">
	// import { Spinner } from '$lib/components/ui/spinner';
	import { Loader2 } from '@lucide/svelte';
	// import { Loader2 } from '@lucide/svelte';

	export let show: boolean = false;
	export let message: string = 'Loading...';
	export let fullScreen: boolean = true;
	export let zIndex: number = 50;
	export let blurBackground: boolean = true;
	export let spinnerSize: 'sm' | 'md' | 'lg' | 'xl' = 'md';
	export let spinnerColor: 'default' | 'primary' | 'white' = 'default';

	// Map spinner sizes
	const spinnerSizeMap = {
		sm: 'h-4 w-4',
		md: 'h-8 w-8',
		lg: 'h-12 w-12',
		xl: 'h-16 w-16'
	};

	// Map spinner colors
	const spinnerColorMap = {
		default: 'text-foreground',
		primary: 'text-primary',
		white: 'text-white'
	};
</script>

{#if show}
	<div
		class="fixed inset-0 flex items-center justify-center transition-all duration-200 {fullScreen
			? 'h-screen w-screen'
			: 'h-full w-full'}"
		style="z-index: {zIndex};"
		role="status"
		aria-live="polite"
		aria-label="Loading"
	>
		<!-- Backdrop -->
		<div
			class="absolute inset-0 {blurBackground
				? 'backdrop-blur-sm'
				: 'bg-background/80'} transition-opacity duration-200"
			aria-hidden="true"
		></div>

		<!-- Loading Content -->
		<div
			class="relative flex animate-in flex-col items-center justify-center gap-4 rounded-xl border border-border bg-background/90 p-6 shadow-lg duration-300 fade-in zoom-in"
			role="presentation"
		>
			<!-- Spinner -->
			<div class="relative">
				<!-- Option 1: Using shadcn Spinner component -->
				<!-- <Spinner size={spinnerSize} class={spinnerColorMap[spinnerColor]} /> -->

				<!-- Option 2: Using lucide icon -->
				
                <Loader2 class={`${spinnerSizeMap[spinnerSize]} ${spinnerColorMap[spinnerColor]} animate-spin`} />
               
			</div>

			<!-- Message -->
			{#if message}
				<div class="space-y-1 text-center">
					<p class="font-medium text-foreground">{message}</p>
					{#if spinnerSize === 'sm' || spinnerSize === 'md'}
						<p class="text-xs text-muted-foreground">Please wait...</p>
					{/if}
				</div>
			{/if}

			<!-- Optional Progress Dots -->
			<div class="flex items-center gap-1">
				<div
					class="h-1.5 w-1.5 animate-pulse rounded-full bg-primary [animation-delay:-0.3s]"
				></div>
				<div
					class="h-1.5 w-1.5 animate-pulse rounded-full bg-primary [animation-delay:-0.15s]"
				></div>
				<div class="h-1.5 w-1.5 animate-pulse rounded-full bg-primary"></div>
			</div>
		</div>
	</div>
{/if}

<style>
	@keyframes fadeIn {
		from {
			opacity: 0;
		}
		to {
			opacity: 1;
		}
	}

	@keyframes zoomIn {
		from {
			transform: scale(0.95);
		}
		to {
			transform: scale(1);
		}
	}

	.animate-in {
		animation-duration: 300ms;
		animation-timing-function: cubic-bezier(0.16, 1, 0.3, 1);
		animation-fill-mode: both;
	}

	.fade-in {
		animation-name: fadeIn;
	}

	.zoom-in {
		animation-name: zoomIn;
	}
</style>
