<script lang="ts">
	import { Calendar, Star } from '@lucide/svelte';
	import type { TmdbMovieResult } from '../../../../api/types';

	let {
		movie,
		onConfirm,
		onCancel
	}: {
		movie: TmdbMovieResult;
		onConfirm: () => void;
		onCancel: () => void;
	} = $props();

	function getPosterUrl(path: string, size?: 'w342' | 'w154' | 'w780') {
		if (!size) size = 'w342';
		return `https://image.tmdb.org/t/p/${size}${path}`;
	}

	function formatYear(dateString: string | null): string {
		if (!dateString) return 'Unknown';
		return new Date(dateString).getFullYear().toString();
	}
</script>

<div class="space-y-6">
	<div class="my-4 flex flex-col gap-6 md:flex-row">
		<!-- Poster -->
		<div class="h-72 w-full shrink-0 overflow-hidden rounded-lg bg-muted md:w-48">
			{#if movie.posterPath}
				<img
					src={getPosterUrl(movie.posterPath, 'w780')}
					alt={movie.title}
					class="h-full w-full object-cover"
				/>
			{:else}
				<div class="flex h-full w-full items-center justify-center text-muted-foreground">
					No Poster Available
				</div>
			{/if}
		</div>

		<!-- Movie Details -->
		<div class="flex-1">
			<h3 class="mb-2 text-2xl font-bold">{movie.title}</h3>

			<div class="mb-4 flex items-center gap-4 text-muted-foreground">
				<div class="flex items-center gap-1">
					<Calendar class="h-4 w-4" />
					<span>{formatYear(movie.releaseDate)}</span>
				</div>
				<div class="flex items-center gap-1">
					<Star class="h-4 w-4 fill-current text-primary" />
					<span class="font-medium">{movie.voteAverage.toFixed(1)}</span>
				</div>
			</div>

			<div class="space-y-4">
				<div>
					<h4 class="mb-2 text-sm font-medium text-muted-foreground">Overview</h4>
					<p class="text-sm leading-relaxed">
						{movie.overview || 'No overview available.'}
					</p>
				</div>
			</div>
		</div>
	</div>

	<!-- Actions -->
	<div class="flex justify-end gap-3 border-t pt-4">
		<button
			onclick={onCancel}
			class="rounded-md px-4 py-2 text-sm font-medium transition-colors hover:bg-muted"
		>
			Cancel
		</button>
		<button
			onclick={onConfirm}
			class="rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary/90"
		>
			Confirm Suggestion
		</button>
	</div>
</div>
