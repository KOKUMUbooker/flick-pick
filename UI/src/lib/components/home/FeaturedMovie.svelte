<!-- src/lib/components/movies/FeaturedMovie.svelte -->
<script lang="ts">
	import type { Movie } from '../../../types/movie';
	import { Button } from '../ui/button';
	import { Badge } from '../ui/badge';
	import Star from '@lucide/svelte/icons/star';
	import Clock from '@lucide/svelte/icons/clock';
	import Calendar from '@lucide/svelte/icons/calendar-range';
	import Play from '@lucide/svelte/icons/play';
	import Heart from '@lucide/svelte/icons/heart';
	import Share2 from '@lucide/svelte/icons/share-2';

	interface FeaturedMovieProps {
		movie: Movie;
		showActions: boolean;
	}
	const { movie, showActions = true }: FeaturedMovieProps = $props();

	let isLiked = $state(false);

	const formatDuration = (minutes: number): string => {
		const hours = Math.floor(minutes / 60);
		const mins = minutes % 60;
		return `${hours}h ${mins}m`;
	};
</script>

<div
	class="relative overflow-hidden rounded-2xl border border-border bg-linear-to-br from-background to-secondary/30"
>
	<div class="grid gap-8 p-8 md:grid-cols-2">
		<!-- Movie Info -->
		<div class="space-y-6">
			<div>
				<div class="mb-4 flex items-center gap-3">
					<Badge variant="secondary" class="px-4 py-1 text-lg font-bold">
						{movie.AgeRating}
					</Badge>
					<Badge variant="outline" class="text-sm">
						{movie.Genre}
					</Badge>
					{#if movie.Verified}
						<Badge variant="default" class="bg-green-500 hover:bg-green-600">✓ Verified</Badge>
					{/if}
				</div>

				<h1 class="mb-4 text-4xl font-bold tracking-tight md:text-5xl">
					{movie.Title}
					<span class="ml-2 text-2xl text-muted-foreground">
						({movie.ReleaseDate.getFullYear()})
					</span>
				</h1>

				<div class="mb-6 flex items-center gap-6 text-lg">
					<div class="flex items-center gap-2">
						<Star size={20} class="fill-current text-yellow-500" />
						<span class="font-bold">{movie.Rating.toFixed(1)}</span>
						<span class="text-muted-foreground">/10</span>
					</div>
					<div class="flex items-center gap-2">
						<Clock size={20} />
						<span>{formatDuration(movie.Minutes)}</span>
					</div>
					<div class="flex items-center gap-2">
						<Calendar size={20} />
						<span
							>{movie.ReleaseDate.toLocaleDateString('en-US', {
								month: 'long',
								year: 'numeric'
							})}</span
						>
					</div>
				</div>
			</div>

			<p class="text-lg leading-relaxed text-muted-foreground">
				{movie.Description}
			</p>

			{#if showActions}
				<div class="flex flex-wrap gap-4 pt-4">
					<Button size="lg" class="min-w-45">
						<Play size={20} class="mr-2" />
						Watch Trailer
					</Button>
					<Button variant="outline" size="lg" onclick={() => (isLiked = !isLiked)}>
						<Heart size={20} class="mr-2" />
						{isLiked ? 'Liked' : 'Add to Favorites'}
					</Button>
					<Button variant="ghost" size="lg">
						<Share2 size={20} class="mr-2" />
						Share
					</Button>
				</div>
			{/if}

			<div class="border-t border-border pt-6">
				<p class="text-sm text-muted-foreground">
					Added by <span class="font-medium text-foreground">{movie.AddedBy}</span> • Created on {movie.CreatedAt.toLocaleDateString(
						'en-US',
						{ dateStyle: 'medium' }
					)}
				</p>
			</div>
		</div>

		<!-- Movie Poster -->
		<div class="relative">
			<div class="aspect-2/3 overflow-hidden rounded-xl border border-border shadow-2xl">
				<img
					src={movie.ImgUrl}
					alt={`Poster for ${movie.Title}`}
					class="h-full w-full object-cover"
				/>
			</div>
			<div class="absolute -right-4 -bottom-4 h-32 w-32 rounded-full bg-primary/10 blur-3xl"></div>
			<div class="absolute -top-4 -left-4 h-24 w-24 rounded-full bg-purple-500/10 blur-3xl"></div>
		</div>
	</div>
</div>
