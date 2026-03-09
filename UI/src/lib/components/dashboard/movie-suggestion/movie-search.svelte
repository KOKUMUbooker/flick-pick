<script lang="ts">
	import { Search } from '@lucide/svelte';
	import { debounce } from '../../../../utils/debounce';

	let {
		query = $bindable(),
		isSearching,
		onSearch
	}: {
		query: string;
		isSearching: boolean;
		onSearch: (query: string) => void;
	} = $props();

	// Debounce search to avoid too many API calls
	const debouncedSearch = debounce((value: string) => {
		onSearch(value);
	}, 500);

	function handleInput(e: Event) {
		const target = e.target as HTMLInputElement;
		query = target.value;
		debouncedSearch(query);
	}
</script>

<div class="relative">
	<div class="pointer-events-none absolute inset-y-0 left-3 flex items-center">
		<Search class="h-4 w-4 text-muted-foreground" />
	</div>
	<input
		type="text"
		placeholder="Search for a movie..."
		value={query}
		oninput={handleInput}
		disabled={isSearching}
		class="w-full rounded-lg border bg-background py-3 pr-4 pl-10 focus:ring-2 focus:ring-ring focus:outline-none disabled:opacity-50"
	/>
</div>
