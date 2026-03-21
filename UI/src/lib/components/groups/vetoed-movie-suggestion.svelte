<script lang='ts'>
	import type { CreateQueryResult } from "@tanstack/svelte-query";
	import type { FetchedMovieSuggestion } from "../../../api/types/fetch-movie-suggestions";
	import Badge from "../ui/badge/badge.svelte";

    interface VetoedMovieSuggestionProps {
        movieSuggestionQuery: CreateQueryResult<{ movieNightSuggestions: FetchedMovieSuggestion[];}, Error>
    }
    let {movieSuggestionQuery = $bindable() }: VetoedMovieSuggestionProps = $props()
</script>

<div class="mt-4">
    <h4 class="mb-2 text-sm font-medium text-muted-foreground">Disqualified (Vetoed)</h4>
    {#each movieSuggestionQuery.data?.movieNightSuggestions.filter((s) => s.isDisqualified) as suggestion (suggestion.id)}
        <div
            class="rounded-lg border border-destructive/30 bg-destructive/5 p-3 opacity-60"
        >
            <div class="flex items-center gap-3">
                <img
                    src={`https://image.tmdb.org/t/p/w92${suggestion.movie?.posterPath}`}
                    alt={suggestion.movie?.title}
                    class="h-10 w-7 rounded object-cover"
                />
                <span class="font-medium line-through">{suggestion.movie?.title}</span>
                <Badge variant="destructive" class="ml-auto">Vetoed</Badge>
            </div>
        </div>
    {/each}
</div>