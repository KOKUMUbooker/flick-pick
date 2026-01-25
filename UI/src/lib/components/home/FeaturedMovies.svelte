<!-- src/lib/components/home/FeaturedMovies.svelte -->
<script lang="ts">
	import type { Movie } from '../../../types/movie';
	import MovieCard from './MovieCard.svelte';

	const movies: Movie[] = $props();
</script>

{#if movies.length > 0}
	<div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
		{#each movies as movie (movie.Id)}
			<div class="transform transition-transform hover:-translate-y-2">
				<MovieCard {...movie} />
			</div>
		{/each}
	</div>
{:else}
	<div class="py-12 text-center">
		<svg
			class="mx-auto mb-4 h-16 w-16 text-muted"
			fill="none"
			stroke="currentColor"
			viewBox="0 0 24 24"
		>
			<path
				stroke-linecap="round"
				stroke-linejoin="round"
				stroke-width="1.5"
				d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
			/>
		</svg>
		<h3 class="mb-2 text-xl font-semibold text-muted-foreground">No movies yet</h3>
		<p class="mb-6 text-muted-foreground">Start building your collection!</p>
		<a
			href="/movies/create"
			class="inline-flex items-center gap-2 rounded-lg bg-primary px-6 py-3 text-background transition-colors hover:bg-primary-foreground"
		>
			<svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
				<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
			</svg>
			Add Your First Movie
		</a>
	</div>
{/if}
