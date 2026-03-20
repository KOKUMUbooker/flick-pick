<script lang="ts">
	import { Search } from '@lucide/svelte';
	import { onMount } from 'svelte';
	import { debounce } from '../../../utils/debounce';
 
	interface SearchBarProps {
		query: string;
		onSearch: (query: string) => void;
        placeholder? : string,
        debounceDelay?: number
	}

	let { query = $bindable(), onSearch,placeholder ,debounceDelay = 500 }:SearchBarProps= $props();
	let inputElement: HTMLInputElement | null = $state(null);

	// Debounce search to avoid too many API calls
	const debouncedSearch = debounce((value: string) => {
		onSearch(value);
 	}, debounceDelay);

	function handleInput(e: Event) {
		const target = e.target as HTMLInputElement;
		query = target.value;
		debouncedSearch(query);
	}

	onMount(() => {
		inputElement?.focus();
	});
</script>

<div class="relative">
	<div class="pointer-events-none absolute inset-y-0 left-3 flex items-center">
		<Search class="h-4 w-4 text-muted-foreground" />
	</div>
	<input
		bind:this={inputElement}
		type="text"
		placeholder={placeholder || "Search ..."}
		value={query}
		oninput={handleInput}
 		class="w-full rounded-lg border bg-background py-3 pr-4 pl-10 focus:ring-2 focus:ring-ring focus:outline-none disabled:opacity-50"
	/>
</div>
