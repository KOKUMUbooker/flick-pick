<script lang="ts">
	import type { CreateQueryResult } from '@tanstack/svelte-query';
	import type { FetchedMovieSuggestion } from '../../../api/types/fetch-movie-suggestions';
	import { type MovieNightEvent } from '../../../types';
	import VetoedMovieSuggestion from './vetoed-movie-suggestion.svelte';

	interface VetoedMovieSuggestionProps {
		movieSuggestionQuery: CreateQueryResult<
			{ movieNightSuggestions: FetchedMovieSuggestion[] },
			Error
		>;
		event: MovieNightEvent;
		selectedGroupId: string | undefined;
		validMvSuggestionCmpHasFetchedVotes: boolean;
		movieSuggestionSuccefullyFetched: boolean;
	}

	let {
		movieSuggestionQuery = $bindable(),
		event,
		selectedGroupId,
		movieSuggestionSuccefullyFetched,
		validMvSuggestionCmpHasFetchedVotes
	}: VetoedMovieSuggestionProps = $props();
</script>

<div class="mt-4">
	<h4 class="mb-2 text-sm font-medium text-muted-foreground">Disqualified (Vetoed)</h4>
	{#each movieSuggestionQuery.data?.movieNightSuggestions.filter((s) => s.isDisqualified) as suggestion (suggestion.id)}
		<VetoedMovieSuggestion
			{movieSuggestionSuccefullyFetched}
			{validMvSuggestionCmpHasFetchedVotes}
			{event}
			{suggestion}
			bind:movieSuggestionQuery
			{selectedGroupId}
		/>
	{/each}
</div>
