<!-- src/lib/components/movies/MovieCardCompact.svelte -->
<script lang="ts">
	import Clock from '@lucide/svelte/icons/clock';
	import Play from '@lucide/svelte/icons/play';
	import Star from '@lucide/svelte/icons/star';
	import type { Movie } from '../../../types';
	import { Badge } from '../ui/badge';

	interface MovieCompactProps {
		movie: Movie;
		variant?: 'grid' | 'list';
	}
	const { movie, variant = 'grid' }: MovieCompactProps = $props();

	const formatDuration = (minutes: number): string => {
		const hours = Math.floor(minutes / 60);
		const mins = minutes % 60;
		return hours > 0 ? `${hours}h ${mins}m` : `${mins}m`;
	};
</script>

{#if variant === 'grid'}
	<!-- Grid Variant -->
	<div
		class="group bg-card relative overflow-hidden rounded-lg border border-border transition-all hover:-translate-y-0.5 hover:shadow-lg"
	>
		<!-- Image -->
		<div class="relative aspect-2/3 overflow-hidden">
			<img
				src={movie.ImgUrl}
				alt={movie.Title}
				class="h-full w-full object-cover transition-transform group-hover:scale-105"
			/>
			<div
				class="absolute inset-0 flex items-center justify-center bg-linear-to-t from-black/60 to-transparent opacity-0 transition-opacity group-hover:opacity-100"
			>
				<button
					class="rounded-full bg-primary p-3 text-primary-foreground transition-colors hover:bg-primary-foreground hover:text-background"
				>
					<Play size={20} />
				</button>
			</div>
			<Badge variant="secondary" class="absolute top-2 left-2 bg-background/90 backdrop-blur-sm">
				{movie.AgeRating}
			</Badge>
		</div>

		<!-- Content -->
		<div class="p-3">
			<h3 class="mb-1 line-clamp-1 text-sm font-semibold">{movie.Title}</h3>
			<div class="mb-2 flex items-center justify-between text-xs text-muted-foreground">
				<span>{movie.ReleaseDate.getFullYear()}</span>
				<span class="flex items-center gap-1">
					<Clock size={12} />
					{formatDuration(movie.Minutes)}
				</span>
			</div>
			<div class="flex items-center justify-between">
				<div class="flex items-center gap-1">
					<Star size={12} class="fill-current text-yellow-500" />
					<span class="text-sm font-medium">{movie.Rating.toFixed(1)}</span>
				</div>
				<Badge variant="outline" class="text-xs">
					{movie.Genre}
				</Badge>
			</div>
		</div>
	</div>
{:else}
	<!-- List Variant -->
	<div class="group flex gap-4 border-b border-border p-4 transition-colors hover:bg-accent">
		<!-- Image -->
		<div class="relative w-16 shrink-0">
			<img src={movie.ImgUrl} alt={movie.Title} class="h-24 w-16 rounded-md object-cover" />
			<Badge variant="secondary" class="absolute -top-1 -right-1 px-1 text-[10px]">
				{movie.AgeRating}
			</Badge>
		</div>

		<!-- Content -->
		<div class="min-w-0 flex-1">
			<div class="mb-1 flex items-start justify-between">
				<h3 class="line-clamp-1 text-base font-semibold">{movie.Title}</h3>
				<div class="flex items-center gap-1 text-sm">
					<Star size={14} class="fill-current text-yellow-500" />
					<span class="font-medium">{movie.Rating.toFixed(1)}</span>
				</div>
			</div>

			<div class="mb-2 flex items-center gap-3 text-sm text-muted-foreground">
				<span>{movie.ReleaseDate.getFullYear()}</span>
				<span>•</span>
				<span class="flex items-center gap-1">
					<Clock size={12} />
					{formatDuration(movie.Minutes)}
				</span>
				<span>•</span>
				<Badge variant="outline">
					{movie.Genre}
				</Badge>
			</div>

			<p class="mb-3 line-clamp-2 text-sm text-muted-foreground">
				{movie.Description}
			</p>

			<div class="flex items-center justify-between">
				<span class="text-xs text-muted-foreground">
					Added by {movie.AddedBy}
				</span>
				<button class="text-sm font-medium text-primary hover:text-primary-foreground">
					View Details →
				</button>
			</div>
		</div>
	</div>
{/if}
