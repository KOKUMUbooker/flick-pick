<!-- src/lib/components/movies/MovieGrid.svelte -->
<script lang="ts">
	import Grid2x2 from '@lucide/svelte/icons/grid-2x2';
	import List from '@lucide/svelte/icons/list';
	import type { Movie } from '../../../types';
	import { Button } from '../ui/button';
	import { Skeleton } from '../ui/skeleton';
	import MovieCard from './MovieCard.svelte';
	import MovieCardCompact from './MovieCardCompact.svelte';

	interface MovieGridInterface {
		movies: Movie[];
		isLoading: boolean;
		viewMode: 'grid' | 'list' | 'cards';
		columns: number;
		emptyMessage: string;
	}
	let {
		movies = [],
		isLoading = false,
		viewMode = 'cards',
		columns = viewMode == 'cards' ? 3 : 4,
		emptyMessage = 'No movies found'
	}: MovieGridInterface = $props();
	// console.log('Movie grid movies : ', movies);

	const gridColumns: { [key: number]: string } = {
		1: 'grid-cols-1',
		2: 'grid-cols-1 sm:grid-cols-2',
		3: 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3',
		4: 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4',
		5: 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-5',
		6: 'grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-6'
	};
</script>

<div class="space-y-6">
	<!-- View Controls -->
	<div class="flex items-center justify-between">
		<div class="text-md text-muted-foreground">
			Showing <span class="font-semibold text-foreground">{movies.length}</span> movies
		</div>
		<div class="flex items-center gap-2">
			<span class="mr-2 text-sm text-muted-foreground">View:</span>
			<Button
				variant={viewMode === 'cards' ? 'default' : 'outline'}
				size="sm"
				onclick={() => (viewMode = 'cards')}
				class="px-3"
			>
				<Grid2x2 size={16} class="mr-2" />
				Cards
			</Button>

			<Button
				variant={viewMode === 'list' ? 'default' : 'outline'}
				size="sm"
				onclick={() => (viewMode = 'list')}
				class="px-3"
			>
				<List size={16} class="mr-2" />
				List
			</Button>
		</div>
	</div>

	<!-- Loading State -->
	{#if isLoading}
		<div class="grid gap-6 {gridColumns[columns]}">
			{#each Array(8) as _}
				<div class="space-y-3">
					<Skeleton class="aspect-2/3 w-full rounded-lg" />
					<Skeleton class="h-4 w-full" />
					<Skeleton class="h-4 w-3/4" />
				</div>
			{/each}
		</div>
	{:else if movies.length === 0}
		<!-- Empty State -->
		<div class="py-12 text-center">
			<div class="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-muted">
				<svg
					class="h-8 w-8 text-muted-foreground"
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
			</div>
			<h3 class="mb-2 text-lg font-semibold text-foreground">{emptyMessage}</h3>
			<p class="mx-auto max-w-md text-muted-foreground">
				Try adjusting your search or filter to find what you're looking for.
			</p>
			<Button variant="outline" class="mt-4">Clear Filters</Button>
		</div>
	{:else}
		<!-- Movie Grid -->
		{#if viewMode === 'cards'}
			<div class="grid gap-6 {gridColumns[columns]}">
				{#each movies as movie (movie.Id)}
					<MovieCard {movie} />
				{/each}
			</div>
		{:else if viewMode === 'grid'}
			<div class="grid gap-4 {gridColumns[columns]}">
				{#each movies as movie (movie.Id)}
					<MovieCardCompact {movie} variant="grid" />
				{/each}
			</div>
		{:else}
			<div class="space-y-2">
				{#each movies as movie (movie.Id)}
					<MovieCardCompact {movie} variant="list" />
				{/each}
			</div>
		{/if}
	{/if}
</div>
