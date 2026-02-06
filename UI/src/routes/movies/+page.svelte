<script lang="ts">
	import MovieGrid from '$lib/components/home/MovieGrid.svelte';
	import { Badge } from '$lib/components/ui/badge';
	import { Button } from '$lib/components/ui/button';
	import { Card, CardHeader, CardTitle } from '$lib/components/ui/card';
	import { Input } from '$lib/components/ui/input';
	import { Select, SelectContent, SelectItem, SelectTrigger } from '$lib/components/ui/select';
	import {
		Sheet,
		SheetContent,
		SheetDescription,
		SheetHeader,
		SheetTitle,
		SheetTrigger
	} from '$lib/components/ui/sheet';
	import ActiveFilters from '@/components/movies/active-filters.svelte';
	import FiltersContent from '@/components/movies/filters/filters-content.svelte';
	import HeroHeader from '@/components/movies/hero-header.svelte';
	import Grid2x2 from '@lucide/svelte/icons/grid-2x2';
	import List from '@lucide/svelte/icons/list';
	import Search from '@lucide/svelte/icons/search';
	import Filter from '@lucide/svelte/icons/sliders-horizontal';
	import X from '@lucide/svelte/icons/x';
	import { onMount } from 'svelte';
	import { dummyMovies } from '../../data/movies';
	import { type Genre, type Movie } from '../../types';
	import FiltersHeader from '@/components/movies/filters/filters-header.svelte';

	// State
	let movies: Movie[] = dummyMovies;
	let filteredMovies: Movie[] = dummyMovies;
	let isLoading = false;

	// Filter state
	let searchQuery = '';
	let selectedGenres: Genre[] = [];
	let yearRange: [number, number] = [1980, 2025];
	let ratingRange: [number, number] = [0, 10];
	let sortBy: 'title' | 'rating' | 'releaseDate' | 'duration' = 'rating';
	let sortOrder: 'asc' | 'desc' = 'desc';
	let viewMode: 'cards' | 'grid' | 'list' = 'cards';

	// UI state
	let isFilterOpen = false;
	let showMobileFilters = false;

	// Initialize
	onMount(() => {
		movies = dummyMovies;
		filteredMovies = movies;
		updateFilters();
	});

	// Available years for filtering
	const availableYears = Array.from(
		new Set(movies.map((m) => m.ReleaseDate.getFullYear()).sort((a, b) => b - a))
	);

	// Update filters
	const updateFilters = () => {};

	// Handler functions
	const handleSearch = () => updateFilters();

	const toggleGenre = (genre: Genre) => {};

	const clearFilters = () => {
		searchQuery = '';
		selectedGenres = [];
		yearRange = [1980, 2025];
		ratingRange = [0, 10];
		sortBy = 'rating';
		sortOrder = 'desc';
		updateFilters();
	};

	const toggleSortOrder = () => {
		sortOrder = sortOrder === 'asc' ? 'desc' : 'asc';
		updateFilters();
	};

	// Get active filter count
	const activeFilterCount =
		(searchQuery ? 1 : 0) +
		selectedGenres.length +
		(yearRange[0] > 1980 || yearRange[1] < 2025 ? 1 : 0) +
		(ratingRange[0] > 0 || ratingRange[1] < 10 ? 1 : 0);
</script>

<svelte:head>
	<title>Movies - Browse Collection</title>
	<meta
		name="description"
		content="Browse and filter our movie collection. Find your next favorite film with advanced search and filtering options."
	/>
</svelte:head>

<div class="min-h-screen bg-background">
	<!-- Hero Header -->
	<HeroHeader movieLength={movies.length} />

	<div class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
		<div class="flex flex-col gap-8 lg:flex-row">
			<!-- Left Sidebar - Filters (Desktop) -->
			<div class="hidden lg:block lg:w-1/4">
				<Card>
					<FiltersHeader {activeFilterCount} {clearFilters} />

					<FiltersContent
						{clearFilters}
						{filteredMovies}
						{handleSearch}
						{isFilterOpen}
						{ratingRange}
						{searchQuery}
						{selectedGenres}
						{sortBy}
						{sortOrder}
						{toggleGenre}
						{toggleSortOrder}
						{updateFilters}
						{yearRange}
					/>
				</Card>
			</div>

			<!-- Main Content -->
			<div class="lg:w-3/4">
				<!-- Mobile Filter Bar -->
				<div class="mb-6 lg:hidden">
					<div class="flex flex-col gap-4 sm:flex-row">
						<!-- Mobile Search -->
						<div class="relative flex-1">
							<Input
								bind:value={searchQuery}
								placeholder="Search movies..."
								oninput={handleSearch}
								class="pl-10"
							/>
							<Search
								class="absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 transform text-muted-foreground"
							/>
						</div>

						<!-- Mobile Filter Button -->
						<Sheet bind:open={showMobileFilters}>
							<SheetTrigger>
								<Button variant="outline" class="gap-2">
									<Filter size={16} />
									Filters
									{#if activeFilterCount > 0}
										<Badge variant="default" class="ml-1 h-5 w-5 p-0">
											{activeFilterCount}
										</Badge>
									{/if}
								</Button>
							</SheetTrigger>
							<SheetContent side="left" class="w-[90vw] max-w-md overflow-y-auto">
								<SheetHeader>
									<SheetTitle>Filters</SheetTitle>
									<SheetDescription>Refine your movie search</SheetDescription>
								</SheetHeader>
								<div class="pt-2 pb-6">
									<FiltersContent
										{clearFilters}
										{filteredMovies}
										{handleSearch}
										{isFilterOpen}
										{ratingRange}
										{searchQuery}
										{selectedGenres}
										{sortBy}
										{sortOrder}
										{toggleGenre}
										{toggleSortOrder}
										{updateFilters}
										{yearRange}
									/>
								</div>
							</SheetContent>
						</Sheet>

						<!-- View Toggle -->
						<div class="flex items-center gap-2">
							<Button
								variant={viewMode === 'grid' ? 'default' : 'outline'}
								size="icon"
								onclick={() => (viewMode = 'grid')}
							>
								<Grid2x2 size={16} />
							</Button>
							<Button
								variant={viewMode === 'list' ? 'default' : 'outline'}
								size="icon"
								onclick={() => (viewMode = 'list')}
							>
								<List size={16} />
							</Button>
						</div>
					</div>
				</div>

				<!-- Active Filters Display -->
				{#if activeFilterCount > 0}
					<ActiveFilters
						{clearFilters}
						{ratingRange}
						{selectedGenres}
						{searchQuery}
						{toggleGenre}
						{updateFilters}
						{yearRange}
					/>
				{/if}

				<!-- Movie Grid -->
				<MovieGrid
					movies={filteredMovies}
					{viewMode}
					columns={viewMode === 'cards' ? 3 : 4}
					{isLoading}
					emptyMessage={searchQuery || selectedGenres.length > 0
						? 'No movies match your search criteria. Try adjusting your filters.'
						: 'No movies available in the collection.'}
				/>

				{#if filteredMovies.length > 0}
					<div class="mt-8 flex justify-center">
						<div class="flex items-center gap-2">
							<Button variant="outline" size="sm">Previous</Button>
							<div class="flex items-center gap-1">
								<Button variant="ghost" size="sm" class="h-8 w-8 p-0">1</Button>
								<Button variant="outline" size="sm" class="h-8 w-8 p-0">2</Button>
								<Button variant="ghost" size="sm" class="h-8 w-8 p-0">3</Button>
								<span class="px-2 text-muted-foreground">...</span>
								<Button variant="ghost" size="sm" class="h-8 w-8 p-0">10</Button>
							</div>
							<Button variant="outline" size="sm">Next</Button>
						</div>
					</div>
				{/if}
			</div>
		</div>
	</div>
</div>

<style>
	/* Custom scrollbar for filter panel */
	::-webkit-scrollbar {
		width: 6px;
	}

	::-webkit-scrollbar-track {
		background: transparent;
	}

	::-webkit-scrollbar-thumb {
		background: hsl(var(--border));
		border-radius: 3px;
	}

	::-webkit-scrollbar-thumb:hover {
		background: hsl(var(--muted-foreground));
	}
</style>
