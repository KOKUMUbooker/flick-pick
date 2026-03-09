<script lang="ts">
	import { ArrowLeft, Check, Loader2, X } from '@lucide/svelte';
	import type {
		MovieSuggestion,
		SearchState,
		SuggestionFlowProps,
		TmdbMovieResult
	} from '../../../api/types';
	import { dummySearchResponse } from '../../../data/mock-tmdb-res';
	import MovieSearch from './movie-suggestion/movie-search.svelte';
	import SearchResults from './movie-suggestion/search-results.svelte';
	import SuggestionConfirm from './movie-suggestion/suggestion-confirm.svelte';

	let { movieNightId, onCancel, onSuggestionAdded }: SuggestionFlowProps = $props();

	let currentState: SearchState = $state('idle');
	let searchQuery = $state('');
	let selectedMovie: TmdbMovieResult | null = $state(null);
	let searchResults: TmdbMovieResult[] = $state([]);
	let isSearching = $state(false);
	let error = $state<string | null>(null);

	// Handle search
	async function handleSearch(query: string) {
		if (!query.trim()) {
			searchResults = [];
			currentState = 'idle';
			return;
		}

		searchQuery = query;
		isSearching = true;
		error = null;
		currentState = 'searching';

		try {
			// const response = await fetch(`/api/movies/search?query=${encodeURIComponent(query)}`);
			// if (!response.ok) throw new Error('Search failed');

			// const data: { results: TmdbMovieResult[] } = await response.json();

			searchResults = dummySearchResponse.results;
			currentState = searchResults.length > 0 ? 'results' : 'idle';
		} catch (err) {
			error = err instanceof Error ? err.message : 'Failed to search movies';
			currentState = 'idle';
		} finally {
			isSearching = false;
		}
	}

	// Handle movie selection
	function handleSelectMovie(movie: TmdbMovieResult) {
		selectedMovie = movie;
		currentState = 'confirming';
	}

	// Handle suggestion confirmation
	async function handleConfirmSuggestion() {
		if (!selectedMovie) return;

		currentState = 'success';
		error = null;

		try {
			// Create suggestion
			const response = await fetch('/api/movie-night/suggestions', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({
					movieNightId,
					movieId: selectedMovie.id,
					title: selectedMovie.title,
					posterPath: selectedMovie.poster_path,
					releaseDate: selectedMovie.release_date,
					overview: selectedMovie.overview,
					voteAverage: selectedMovie.vote_average
				})
			});

			if (!response.ok) throw new Error('Failed to create suggestion');

			const suggestion: MovieSuggestion = await response.json();
			onSuggestionAdded(suggestion);

			// Reset after success
			setTimeout(() => {
				currentState = 'idle';
				selectedMovie = null;
				searchQuery = '';
				searchResults = [];
			}, 2000);
		} catch (err) {
			error = err instanceof Error ? err.message : 'Failed to create suggestion';
			currentState = 'confirming';
		}
	}

	// Handle back button
	function handleBack() {
		if (currentState === 'confirming') {
			currentState = 'results';
			selectedMovie = null;
		} else if (currentState === 'results') {
			currentState = 'idle';
			searchQuery = '';
			searchResults = [];
		} else {
			onCancel();
		}
	}
</script>

<div class="fixed inset-0 z-100 bg-background/80 backdrop-blur-sm">
	<div
		class="fixed inset-4 md:inset-auto md:top-1/2 md:left-1/2 md:h-auto md:w-full md:max-w-3xl md:-translate-x-1/2 md:-translate-y-1/2"
	>
		<div class="overflow-hidden rounded-lg border bg-card shadow-lg">
			<!-- Header -->
			<div class="flex items-center justify-between border-b p-4">
				<div class="flex items-center gap-2">
					{#if currentState !== 'idle'}
						<button
							onclick={handleBack}
							class="rounded-full p-2 transition-colors hover:bg-muted"
							aria-label="Go back"
						>
							<ArrowLeft class="h-4 w-4" />
						</button>
					{/if}
					<h2 class="text-lg font-semibold">
						{#if currentState === 'confirming'}
							Confirm Suggestion
						{:else if currentState === 'success'}
							Suggestion Added!
						{:else}
							Suggest a Movie
						{/if}
					</h2>
				</div>
				<button
					onclick={onCancel}
					class="rounded-full p-2 transition-colors hover:bg-muted"
					aria-label="Close"
				>
					<X class="h-4 w-4" />
				</button>
			</div>

			<!-- Content -->
			<div class="max-h-[calc(100vh-12rem)] overflow-y-auto p-4">
				{#if error}
					<div class="mb-4 rounded-md bg-destructive/10 p-3 text-sm text-destructive">
						{error}
					</div>
				{/if}

				{#if currentState === 'success'}
					<div class="flex flex-col items-center justify-center py-12 text-center">
						<div class="mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-primary/10">
							<Check class="h-8 w-8 text-primary" />
						</div>
						<h3 class="mb-2 text-xl font-semibold">Movie Suggested!</h3>
						<p class="text-muted-foreground">
							"{selectedMovie?.title}" has been added to the suggestions
						</p>
					</div>
				{:else}
					<MovieSearch query={searchQuery} {isSearching} onSearch={handleSearch} />

					{#if currentState === 'searching'}
						<div class="flex justify-center py-12">
							<Loader2 class="h-8 w-8 animate-spin text-primary" />
						</div>
					{/if}

					{#if currentState === 'results'}
						<SearchResults results={searchResults} onSelect={handleSelectMovie} />
					{/if}

					{#if currentState === 'confirming' && selectedMovie}
						<SuggestionConfirm
							movie={selectedMovie}
							onConfirm={handleConfirmSuggestion}
							onCancel={() => handleBack()}
						/>
					{/if}
				{/if}
			</div>
		</div>
	</div>
</div>
