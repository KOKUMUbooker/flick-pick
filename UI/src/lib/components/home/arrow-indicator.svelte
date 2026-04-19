<script>
	import { ArrowDown } from '@lucide/svelte';
</script>

<!-- Arrow Indicator -->
<div class="mx-auto flex justify-center">
	<button
		class="group animate-bounce rounded-full border border-border/50 bg-background/50 p-3 backdrop-blur-sm transition-all hover:bg-background/80 hover:shadow-lg"
		on:click={() => {
			document.getElementById('quickOverview')?.scrollIntoView({
				behavior: 'smooth'
			});
		}}
		aria-label="Scroll to features"
	>
		<ArrowDown class="h-5 w-5 text-muted-foreground transition-colors group-hover:text-primary" />
	</button>
</div>
