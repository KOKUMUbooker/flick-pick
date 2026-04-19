<script lang="ts">
	// No script needed, but keeping the tag for consistency
	import tmdbLogo from '$lib/assets/TMDB-logo.svg';
</script>

<!-- Footer -->
<footer class="relative z-20 border-t border-border bg-background px-4 py-6">
	<div class="container mx-auto max-w-6xl">
		<div class="flex flex-col items-center gap-3 text-center">
			<p class="text-sm text-muted-foreground">
				© {new Date().getFullYear()} WatchHive. All rights reserved.
			</p>

			<a
				href="https://www.themoviedb.org/"
				target="_blank"
				rel="noopener noreferrer"
				class="flex items-center gap-2 text-sm text-muted-foreground transition-opacity hover:opacity-80"
			>
				<img src={tmdbLogo} alt="TMDB Logo" class="h-4" />
				<span>This product uses the TMDB API but is not endorsed or certified by TMDB.</span>
			</a>
		</div>
	</div>
</footer>
