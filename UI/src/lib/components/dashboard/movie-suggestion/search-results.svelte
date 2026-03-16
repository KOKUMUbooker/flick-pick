<script lang="ts">
	import { Calendar, Star } from '@lucide/svelte';
	import type { TmdbMovieResult } from '../../../../api/types';

	let {
		results,
		onSelect
	}: {
		results: TmdbMovieResult[];
		onSelect: (movie: TmdbMovieResult) => void;
	} = $props();

	function getPosterUrl(path: string | null): string {
		if (!path) return '/placeholder-movie.png';
		return `https://image.tmdb.org/t/p/w200${path}`;
	}

	function formatYear(dateString: string | null): string {
		if (!dateString) return 'Unknown';
		return new Date(dateString).getFullYear().toString();
	}

	function formatVoteAverage(vote: number): string {
		return vote.toFixed(1);
	}
</script>

<div class="space-y-3">
	{#each results as movie (movie.tmdbId)}
		<button
			onclick={() => onSelect(movie)}
			class="flex w-full gap-4 rounded-lg bg-muted/50 p-3 text-left transition-colors hover:bg-muted"
		>
			<!-- Poster -->
			<div class="h-24 w-16 shrink-0 overflow-hidden rounded-md bg-muted">
				{#if movie.posterPath}
					<img
						src={getPosterUrl(movie.posterPath)}
						alt={movie.title}
						class="h-full w-full object-cover"
					/>
				{:else}
					<div class="flex h-full w-full items-center justify-center text-xs text-muted-foreground">
						No Poster
					</div>
				{/if}
			</div>

			<!-- Movie Info -->
			<div class="min-w-0 flex-1">
				<div class="flex items-start justify-between gap-2">
					<h3 class="truncate font-semibold">{movie.title}</h3>
					<div
						class="flex items-center gap-1 rounded-full bg-primary/10 px-2 py-1 text-sm text-primary"
					>
						<Star class="h-3 w-3 fill-current" />
						<span>{formatVoteAverage(movie?.voteAverage || 0)}</span>
					</div>
				</div>

				<div class="mt-1 flex items-center gap-4 text-sm text-muted-foreground">
					<div class="flex items-center gap-1">
						<Calendar class="h-3 w-3" />
						<span>{movie?.releaseDate ?formatYear(movie?.releaseDate) :""}</span>
					</div>
				</div>

				<p class="mt-2 line-clamp-2 text-sm text-muted-foreground">
					{movie.overview || 'No description available'}
				</p>
			</div>

			<div class="shrink-0">
				<span class="text-sm font-medium text-primary">Suggest</span>
			</div>
		</button>
	{/each}
</div>
