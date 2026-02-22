<!-- src/lib/components/home/CTAButtons.svelte -->
<div class="space-y-6">
	<h2 class="text-3xl font-bold">Ready to organize your movie collection?</h2>
	<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
		Join thousands of film enthusiasts who use Watch Hive to track, rate, and organize their
		collections.
	</p>

	<div class="mt-8 flex flex-col items-center justify-center gap-4 sm:flex-row">
		<a
			href="/movies"
			class="flex w-full transform items-center justify-center gap-3 rounded-xl bg-primary px-8 py-4 text-lg font-semibold text-background shadow-lg transition-all hover:scale-105 hover:bg-primary-foreground hover:shadow-xl sm:w-auto"
		>
			<svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
				<path
					stroke-linecap="round"
					stroke-linejoin="round"
					stroke-width="2"
					d="M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z"
				/>
			</svg>
			Browse Collection
		</a>

		<a
			href="/movies/create"
			class="flex w-full transform items-center justify-center gap-3 rounded-xl border border-border bg-secondary px-8 py-4 text-lg font-semibold text-secondary-foreground shadow-lg transition-all hover:scale-105 hover:bg-accent hover:text-accent-foreground hover:shadow-xl sm:w-auto"
		>
			<svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
				<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
			</svg>
			Add New Movie
		</a>
	</div>

	<div class="mt-8 border-t border-border pt-8">
		<p class="text-sm text-muted-foreground">
			<strong>Pro Tip:</strong> Use the search bar above to quickly find movies. You can search by title,
			director, genre, or even year!
		</p>
	</div>
</div>
