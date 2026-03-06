<script lang="ts">
	import { goto } from '$app/navigation';
	import { resolve } from '$app/paths';
	import { Badge } from '$lib/components/ui/badge';
	import { Button } from '$lib/components/ui/button';
	import {
		Card,
		CardContent,
		CardDescription,
		CardFooter,
		CardHeader,
		CardTitle
	} from '$lib/components/ui/card';
	import { Progress } from '$lib/components/ui/progress';
	import { Check, ChevronRight, Film, Loader2, Sparkles, X } from '@lucide/svelte';
	import { onMount } from 'svelte';
	import { genreSampleTMDBRes } from '../../../data';

	// Types
	interface Genre {
		id: number;
		name: string;
	}

	interface GenreSelection {
		id: number;
		name: string;
		selected: boolean;
	}

	interface PreferencesState {
		selectedGenres: GenreSelection[];
		isSubmitting: boolean;
		error: string | null;
	}

	// State
	let preferences: PreferencesState = {
		selectedGenres: [],
		isSubmitting: false,
		error: null
	};

	let searchQuery: string = '';
	let isLoading: boolean = true;

	// Initialize genres on mount
	onMount(() => {
		// Simulate loading delay
		setTimeout(() => {
			preferences.selectedGenres = genreSampleTMDBRes.genres.map((genre: Genre) => ({
				...genre,
				selected: false
			}));
			isLoading = false;
		}, 800);
	});

	// Computed values
	$: filteredGenres = preferences.selectedGenres.filter((genre: GenreSelection) =>
		genre.name.toLowerCase().includes(searchQuery.toLowerCase())
	);

	$: selectedCount = preferences.selectedGenres.filter((g: GenreSelection) => g.selected).length;

	$: completionPercentage = Math.min((selectedCount / 5) * 100, 100);

	$: canProceed = selectedCount >= 3 && selectedCount <= 10 && !preferences.isSubmitting;

	// Handlers with proper typing
	const toggleGenre = (genreId: number): void => {
		preferences.selectedGenres = preferences.selectedGenres.map((genre: GenreSelection) => {
			if (genre.id === genreId) {
				// Prevent selecting more than 10 genres
				if (!genre.selected && selectedCount >= 10) {
					return genre;
				}
				return { ...genre, selected: !genre.selected };
			}
			return genre;
		});
	};

	const selectAll = (): void => {
		const remainingSlots = 10 - selectedCount;
		if (remainingSlots <= 0) return;

		preferences.selectedGenres = preferences.selectedGenres.map((genre: GenreSelection) => {
			if (!genre.selected && remainingSlots > 0) {
				return { ...genre, selected: true };
			}
			return genre;
		});
	};

	const clearAll = (): void => {
		preferences.selectedGenres = preferences.selectedGenres.map((genre: GenreSelection) => ({
			...genre,
			selected: false
		}));
	};

	const savePreferences = async (): Promise<void> => {
		if (!canProceed) return;

		preferences.isSubmitting = true;
		preferences.error = null;

		try {
			const selectedGenreIds = preferences.selectedGenres
				.filter((g: GenreSelection) => g.selected)
				.map((g: GenreSelection) => g.id);

			// Simulate API call
			await new Promise((resolve) => setTimeout(resolve, 1500));

			console.log('Saving preferences:', selectedGenreIds);

			// Store in localStorage or send to backend
			localStorage.setItem('userGenrePreferences', JSON.stringify(selectedGenreIds));

			// Redirect to dashboard
			goto(resolve('/dashboard'));
		} catch (err) {
			preferences.error = err instanceof Error ? err.message : 'Failed to save preferences';
		} finally {
			preferences.isSubmitting = false;
		}
	};

	const skipForNow = () => {
		goto(resolve('/dashboard'));
	};
</script>

<div class="min-h-screen bg-linear-to-b from-background to-muted/30">
	<main class="container mx-auto px-4 py-8 lg:py-12">
		<!-- Header Section -->
		<div class="mx-auto mb-8 max-w-3xl text-center lg:mb-12">
			<div class="mb-4 inline-flex items-center justify-center rounded-full bg-primary/10 p-2">
				<Film class="h-5 w-5 text-primary" />
			</div>
			<h1 class="mb-3 text-3xl font-bold lg:text-4xl">Tell us your movie taste 🎬</h1>
			<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
				Select your favorite genres to help us suggest better movies for your future movie nights
			</p>
			<Button variant="link" class="mt-2 text-muted-foreground" onclick={skipForNow}>
				Skip for now
			</Button>
		</div>

		<!-- Main Card -->
		<Card class="mx-auto max-w-4xl border-primary/10 shadow-xl">
			<CardHeader class="border-b border-border/50 pb-6">
				<div class="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
					<div>
						<CardTitle class="flex items-center gap-2 text-2xl">
							Genre Preferences
							{#if selectedCount > 0}
								<Badge variant="outline" class="ml-2">
									{selectedCount}/10 selected
								</Badge>
							{/if}
						</CardTitle>
						<CardDescription class="mt-1.5 text-base">
							Choose at least 3 genres (max 10) to get personalized suggestions
						</CardDescription>
					</div>

					<!-- Selection Progress -->
					<div class="flex min-w-50 items-center gap-3">
						<Progress value={completionPercentage} class="h-2" />
						<span class="min-w-11.25 text-sm font-medium">
							{completionPercentage}%
						</span>
					</div>
				</div>

				<!-- Selected Genres Preview -->
				{#if selectedCount > 0}
					<div class="mt-4 flex flex-wrap items-center gap-2 rounded-lg bg-muted/30 p-3">
						<span class="mr-1 text-sm font-medium">Selected:</span>
						{#each preferences.selectedGenres.filter((g) => g.selected) as genre (genre.id)}
							<Badge variant="secondary" class="gap-1 py-1 pr-1 pl-2">
								{genre.name}
								<button
									onclick={() => toggleGenre(genre.id)}
									class="ml-1 rounded-full p-0.5 hover:bg-muted-foreground/20"
									aria-label={`Remove ${genre.name}`}
								>
									<X class="h-3 w-3" />
								</button>
							</Badge>
						{/each}
					</div>
				{/if}

				<!-- Search and Actions -->
				<div class="mt-4 flex flex-col gap-3 sm:flex-row">
					<div class="relative flex-1">
						<input
							type="text"
							placeholder="Search genres..."
							bind:value={searchQuery}
							class="w-full rounded-lg border border-border bg-background px-4 py-2 pl-10 focus:ring-2 focus:ring-primary/20 focus:outline-none"
						/>
						<Film class="absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
					</div>
					<div class="flex gap-2">
						<Button variant="outline" size="sm" onclick={selectAll} disabled={selectedCount >= 10}>
							Select Max
						</Button>
						<Button variant="outline" size="sm" onclick={clearAll} disabled={selectedCount === 0}>
							Clear All
						</Button>
					</div>
				</div>
			</CardHeader>

			<CardContent class="pt-6">
				{#if isLoading}
					<!-- Loading State -->
					<div class="flex flex-col items-center justify-center py-16">
						<Loader2 class="mb-4 h-12 w-12 animate-spin text-primary" />
						<p class="text-muted-foreground">Loading genres...</p>
					</div>
				{:else}
					<!-- Genre Grid -->
					<div class="grid grid-cols-2 gap-3 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
						{#each filteredGenres as genre (genre.id)}
							<button
								onclick={() => toggleGenre(genre.id)}
								disabled={!genre.selected && selectedCount >= 10}
								class={`
										group relative rounded-xl border-2 p-4 text-left transition-all
										${
											genre.selected
												? 'border-primary bg-primary/5 shadow-md'
												: 'border-border hover:border-primary/30 hover:bg-muted/50'
										}
										${!genre.selected && selectedCount >= 10 ? 'cursor-not-allowed opacity-50' : 'cursor-pointer'}
									`}
							>
								<div class="flex items-start justify-between">
									<span class="text-sm font-medium">{genre.name}</span>
									{#if genre.selected}
										<div class="flex h-5 w-5 items-center justify-center rounded-full bg-primary">
											<Check class="h-3 w-3 text-white" />
										</div>
									{:else if selectedCount >= 10}
										<div class="flex h-5 w-5 items-center justify-center rounded-full bg-muted">
											<span class="text-[10px] text-muted-foreground">max</span>
										</div>
									{/if}
								</div>

								<!-- Hover effect for unselected -->
								{#if !genre.selected && selectedCount < 10}
									<div
										class="pointer-events-none absolute inset-0 rounded-xl bg-primary/5 opacity-0 transition-opacity group-hover:opacity-100"
									></div>
								{/if}
							</button>
						{/each}
					</div>

					<!-- No results -->
					{#if filteredGenres.length === 0}
						<div class="py-12 text-center">
							<p class="text-muted-foreground">No genres found matching "{searchQuery}"</p>
							<Button variant="link" onclick={() => (searchQuery = '')} class="mt-2">
								Clear search
							</Button>
						</div>
					{/if}
				{/if}
			</CardContent>

			<CardFooter
				class="flex flex-col gap-4 border-t border-border/50 pt-6 sm:flex-row sm:justify-between"
			>
				<div class="text-sm text-muted-foreground">
					{#if selectedCount < 3}
						<span class="text-yellow-600 dark:text-yellow-400">
							Select at least {3 - selectedCount} more genre{3 - selectedCount === 1 ? '' : 's'}
						</span>
					{:else if selectedCount <= 10}
						<span class="flex items-center gap-1 text-green-600 dark:text-green-400">
							<Sparkles class="h-4 w-4" />
							Ready to continue!
						</span>
					{/if}
				</div>

				<div class="flex w-full flex-col gap-3 sm:w-auto sm:flex-row">
					{#if preferences.error}
						<p class="text-sm text-destructive">{preferences.error}</p>
					{/if}

					<Button size="lg" disabled={!canProceed} onclick={savePreferences} class="min-w-50">
						{#if preferences.isSubmitting}
							<div>
								<Loader2 class="mr-2 h-4 w-4 animate-spin" />
								Saving...
							</div>
						{:else}
							<div>
								Continue to Dashboard
								<ChevronRight class="ml-2 h-4 w-4" />
							</div>
						{/if}
					</Button>
				</div>
			</CardFooter>
		</Card>

		<!-- Info Section -->
		<div class="mx-auto mt-8 max-w-3xl text-center">
			<p class="text-sm text-muted-foreground">
				✨ Your preferences help us recommend movies you'll love. You can always update them later
				in your profile settings.
			</p>
		</div>
	</main>
</div>

<style>
	/* Smooth transitions */
	* {
		transition:
			border-color 0.2s,
			background-color 0.2s,
			opacity 0.2s;
	}

	/* Custom scrollbar for genre grid */
	.genre-grid {
		scrollbar-width: thin;
		scrollbar-color: hsl(var(--primary)) hsl(var(--muted));
	}

	.genre-grid::-webkit-scrollbar {
		width: 6px;
	}

	.genre-grid::-webkit-scrollbar-track {
		background: hsl(var(--muted));
		border-radius: 3px;
	}

	.genre-grid::-webkit-scrollbar-thumb {
		background: hsl(var(--primary));
		border-radius: 3px;
	}
</style>
