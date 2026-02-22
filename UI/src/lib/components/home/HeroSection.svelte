<!-- src/lib/components/home/HeroSection.svelte -->
<script lang="ts">
	import Star from '@lucide/svelte/icons/star';
	import Search from '@lucide/svelte/icons/search';

	let searchQuery = '';

	const handleSearch = () => {
		if (searchQuery.trim()) {
			window.location.href = `/movies?search=${encodeURIComponent(searchQuery)}`;
		}
	};

	const handleKeyPress = (e: any) => {
		if (e.key === 'Enter') handleSearch();
	};
</script>

<section class="relative overflow-hidden px-4">
	<!-- Background gradient primary color -->
	<div class="absolute inset-0"></div>

	<div class="relative mx-auto max-w-7xl text-center">
		<!-- Logo/Brand -->
		<div class="mb-6 flex justify-center">
			<div class="flex items-center gap-3">
				<Star class="h-10 w-10 text-primary" fill="currentColor" />
				<h1
					class="bg-linear-to-r from-primary to-primary-foreground bg-clip-text text-5xl font-bold text-transparent md:text-6xl"
				>
					Watch Hive
				</h1>
			</div>
		</div>

		<!-- Tagline -->
		<p class="mx-auto mb-8 max-w-3xl text-xl font-bold md:text-2xl">
			Organize, rate, and rediscover your movie collection. Perfect for film enthusiasts and
			collectors.
		</p>

		<!-- Quick Search -->
		<div class="mx-auto mb-10 max-w-2xl">
			<div class="relative">
				<input
					type="text"
					bind:value={searchQuery}
					on:keypress={handleKeyPress}
					placeholder="Search for movies by title or genre..."
					class="w-full rounded-xl border border-border bg-input/90 px-6 py-4 placeholder-muted-foreground backdrop-blur-xl focus:border-transparent focus:ring-2 focus:ring-primary focus:outline-none"
				/>
				<button
					on:click={handleSearch}
					aria-label="search"
					class="absolute top-1/2 right-2 -translate-y-1/2 transform rounded-lg bg-primary p-2 text-background transition-colors hover:bg-primary-foreground"
				>
					<Search class="h-5 w-5" />
				</button>
			</div>
			<p class="mt-2 text-left text-sm font-bold">Try: "Sci-Fi" or "Interstellar"</p>
		</div>

		<!-- Hero Image/Visual -->
	</div>
</section>
