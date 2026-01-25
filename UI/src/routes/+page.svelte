<script lang="ts">
	import CTAButtons from '$lib/components/home/CTAButtons.svelte';
	import FeaturedMovies from '$lib/components/home/FeaturedMovies.svelte';
	import StatsOverview from '$lib/components/home/StatsOverview.svelte';
	import * as Carousel from '$lib/components/ui/carousel/index.js';
	import Autoplay from 'embla-carousel-autoplay';
	import { dummyMovies } from '../data/movies';
	import evilDead from '../lib/assets/evildead.jpg';
	import frankenstein from '../lib/assets/frankenstein.jpg';
	import hereditary from '../lib/assets/hereditary.jpg';
	import it from '../lib/assets/it.jpg';
	import rec from '../lib/assets/rec.jpg';
	import smile from '../lib/assets/smile.jpg';
	import Hero from '../lib/components/home/HeroSection.svelte';
	import type { Movie } from '../types/movie';

	let stats = {
		totalMovies: 0,
		totalDuration: 0,
		avgRating: 0,
		recentAdditions: 0
	};
	let featuredMovies: Movie[] = dummyMovies.slice(1, 4);
	const movieUrls = [hereditary, rec, evilDead, it, smile, frankenstein];
</script>

<svelte:head>
	<title>Movie Manager - A Personal Film Collection</title>
	<meta
		name="description"
		content="Manage the movie collection with ease. Track, rate, and organize your favorite films."
	/>
</svelte:head>

<main class="mt-18 min-h-screen bg-background text-foreground">
	<!-- Hero Section -->
	<div class="mb-4 h-[115vh]">
		<Carousel.Root
			plugins={[
				Autoplay({
					delay: 6000
				})
			]}
			class=" h-full w-full"
		>
			<div
				class="absolute top-0 z-5 flex h-full w-full items-center justify-center backdrop-blur-xs"
			>
				<Carousel.Next class="absolute right-8" />
				<Carousel.Previous class="absolute left-8" />
				<Hero />
			</div>
			<Carousel.Content class="h-full">
				{#each movieUrls as movieUrl (movieUrl)}
					<Carousel.Item
						class="aspect-video h-[115vh] bg-cover bg-center"
						style={`background-image: url(${movieUrl});`}
					></Carousel.Item>
				{/each}
			</Carousel.Content>
		</Carousel.Root>
	</div>

	<!-- Quick Stats -->
	<section class="mx-auto max-w-7xl px-4 py-12">
		<h2 class="mb-8 text-center text-3xl font-bold">Current Collection at a Glance</h2>
		<div class="flex w-full justify-center">
			<StatsOverview {stats} />
		</div>
	</section>

	<!-- Featured Movies -->
	{#if featuredMovies.length > 0}
		<section class="mx-auto max-w-7xl px-4 py-12">
			<div class="mb-8 flex items-center justify-between">
				<h2 class="text-3xl font-bold">
					<!-- {featuredMovies.length >= 6 ? 'Top Rated Films' : 'Your Movies'} -->
					Featured movies
				</h2>
				<a
					href="/movies"
					class="flex items-center gap-2 text-primary transition-colors hover:text-primary-foreground"
				>
					View All
					<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
						<path
							stroke-linecap="round"
							stroke-linejoin="round"
							stroke-width="2"
							d="M9 5l7 7-7 7"
						/>
					</svg>
				</a>
			</div>
			<FeaturedMovies {...featuredMovies} />
		</section>
	{/if}

	<!-- Call to Action -->
	<section class="mx-auto max-w-4xl px-4 py-16 text-center">
		<CTAButtons />
	</section>
</main>
