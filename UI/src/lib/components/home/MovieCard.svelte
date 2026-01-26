<!-- src/lib/components/movies/MovieCard.svelte -->
<script lang="ts">
	import Calendar from '@lucide/svelte/icons/calendar-range';
	import Clock from '@lucide/svelte/icons/clock';
	import ShieldCheck from '@lucide/svelte/icons/shield-check';
	import Star from '@lucide/svelte/icons/star';
	import User from '@lucide/svelte/icons/user';
	import { Badge } from '../ui/badge';
	import { Button } from '../ui/button';
	import { Card, CardContent, CardFooter } from '../ui/card';
	import type { Movie } from '../../../types';

	let showActions: boolean = true;
	let compact: boolean = false;

	const { movie }: { movie: Movie } = $props();
	let isHovered = false;

	// Format duration to hours and minutes
	const formatDuration = (minutes: number): string => {
		const hours = Math.floor(minutes / 60);
		const mins = minutes % 60;
		return hours > 0 ? `${hours}h ${mins}m` : `${mins}m`;
	};

	// Format date to readable string
	const formatDate = (date: Date): string => {
		return (
			date?.toLocaleDateString('en-US', {
				year: 'numeric',
				month: 'short',
				day: 'numeric'
			}) || ''
		);
	};

	// Get rating color based on score
	const getRatingColor = (rating: number): string => {
		if (rating >= 8) return 'text-green-500';
		if (rating >= 7) return 'text-yellow-500';
		if (rating >= 6) return 'text-orange-500';
		return 'text-red-500';
	};

	// Get age rating color
	const getAgeRatingColor = (rating: string): string => {
		switch (rating) {
			case 'PG':
				return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-100';
			case 'PG-13':
				return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-100';
			case 'R':
				return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-100';
			default:
				return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-100';
		}
	};
</script>

<Card
	onmouseenter={() => (isHovered = true)}
	onmouseleave={() => (isHovered = false)}
	class="group relative overflow-hidden border-border transition-all duration-300 hover:-translate-y-1 hover:shadow-xl"
>
	<!-- Movie Poster Image -->
	<div class="relative aspect-2/3 overflow-hidden bg-muted">
		<img
			src={movie.ImgUrl}
			alt={`Poster for ${movie.Title}`}
			class="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105"
		/>

		<!-- Overlay with details on hover -->
		<div
			class="absolute inset-0 flex flex-col justify-end bg-linear-to-t from-black/90 via-black/50 to-transparent p-4 opacity-0 transition-opacity duration-300 group-hover:opacity-100"
		>
			<div class="space-y-2">
				<h3 class="line-clamp-2 text-lg font-semibold text-white">{movie.Title}</h3>
				<p class="line-clamp-3 text-sm text-gray-300">{movie.Description}</p>
				<div class="flex items-center gap-2 text-sm text-gray-300">
					<Calendar size={14} />
					<span>{formatDate(movie.ReleaseDate)}</span>
				</div>
			</div>
		</div>

		<!-- Age Rating Badge -->
		<div class="absolute top-3 left-3">
			<Badge variant="secondary" class={getAgeRatingColor(movie.AgeRating) + ' font-bold'}>
				{movie.AgeRating}
			</Badge>
		</div>

		<!-- Verified Badge -->
		{#if movie.verified}
			<div class="absolute top-3 right-3">
				<Badge variant="outline" class="bg-background/90 backdrop-blur-sm">
					<ShieldCheck size={12} class="mr-1" />
					Verified
				</Badge>
			</div>
		{/if}
	</div>

	<!-- Movie Info -->
	<CardContent class="p-4">
		<!-- Title and Year -->
		<div class="mb-3 flex items-start justify-between">
			<div>
				<h3 class="line-clamp-1 text-lg leading-tight font-semibold">{movie.Title}</h3>
				<p class="text-sm text-muted-foreground">
					{movie.ReleaseDate?.getFullYear()} • {movie.Genre}
				</p>
			</div>
		</div>

		<!-- Rating and Duration -->
		<div class="mb-4 flex items-center justify-between text-sm">
			<div class="flex items-center gap-1">
				<div class="flex items-center">
					<Star size={14} class={getRatingColor(movie.Rating) + ' mr-1 fill-current'} />
					<span class="font-medium">{movie.Rating?.toFixed(1)}</span>
					<span class="ml-1 text-muted-foreground">/10</span>
				</div>
			</div>
			<div class="flex items-center gap-1 text-muted-foreground">
				<Clock size={14} />
				<span>{formatDuration(movie.Minutes)}</span>
			</div>
		</div>

		<!-- Added By -->
		{#if compact}
			<div class="flex items-center gap-2 text-sm text-muted-foreground">
				<User size={14} />
				<span>Added by {movie.AddedBy}</span>
				<span class="text-xs">•</span>
				<span class="text-xs">{formatDate(movie.CreatedAt)}</span>
			</div>
		{/if}
	</CardContent>

	<!-- Actions -->
	{#if showActions}
		<CardFooter class="p-4 pt-0">
			<div class="flex w-full gap-2">
				<a class="flex-1" href={`/movies/${movie.Id}`}>
					<Button variant="outline" class="w-full" size="sm">Details</Button>
				</a>
				<Button variant="default" class="flex-1" size="sm">Watch Trailer</Button>
			</div>
		</CardFooter>
	{/if}
</Card>
