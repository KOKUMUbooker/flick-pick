<script lang="ts">
	import { Star } from '@lucide/svelte';

	interface Props {
		/** The current rating value (1-5) */
		rating: number;
		/** Whether the component is in edit mode */
		editable?: boolean;
		/** Whether the component is disabled */
		disabled?: boolean;
		/** Size of the stars in pixels */
		size?: number;
		/** Custom class for the container */
		class?: string;
	}

	let {
		rating = $bindable(),
		editable = false,
		disabled = false,
		size = 24,
		class: className = ''
	}: Props = $props();

	// Track hover state for edit mode preview
	let hoverRating = $state(0);

	function handleStarClick(selectedRating: number) {
		if (!editable || disabled) return;
		rating = selectedRating;
	}

	function handleStarHover(starIndex: number) {
		if (!editable || disabled) return;
		hoverRating = starIndex;
	}

	function handleStarLeave() {
		hoverRating = 0;
	}

	// Determine if a star should be filled
	function isStarFilled(starIndex: number): boolean {
		if (editable && !disabled && hoverRating > 0) {
			return starIndex <= hoverRating;
		}
		return starIndex <= rating;
	}
</script>

<div class={`inline-flex items-center gap-1 ${className}`} class:pointer-events-none={disabled}>
	{#each [1, 2, 3, 4, 5] as starIndex (starIndex)}
		<button
			type="button"
			class="rounded-sm transition-all duration-100 focus:outline-none focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2
				{editable && !disabled ? 'cursor-pointer hover:scale-110' : 'cursor-default'}"
			class:pointer-events-none={!editable || disabled}
			onclick={() => handleStarClick(starIndex)}
			onmouseenter={() => handleStarHover(starIndex)}
			onmouseleave={handleStarLeave}
			aria-label="{starIndex} star{starIndex === 1 ? '' : 's'}"
			{disabled}
		>
			<Star
				class={`transition-colors duration-100 ${isStarFilled(starIndex) ? 'fill-yellow-500 text-yellow-500' : 'fill-none text-muted-foreground'}`}
				{size}
			/>
		</button>
	{/each}

	{#if editable && !disabled}
		<span class="ml-2 text-sm text-muted-foreground">
			{hoverRating > 0 ? hoverRating : rating > 0 ? rating : ''}
			{hoverRating > 0 || rating > 0 ? ' star' + ((hoverRating || rating) === 1 ? '' : 's') : ''}
		</span>
	{/if}
</div>
