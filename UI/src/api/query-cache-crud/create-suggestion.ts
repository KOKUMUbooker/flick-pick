import { QUERY_KEYS, queryClient } from "..";
import type { FetchedMovieSuggestion } from "../types/fetch-movie-suggestions";

export async function AddSuggestionToQueryCache(movieEventId: string, movieSuggestion: FetchedMovieSuggestion) {
    const queryKey = [QUERY_KEYS.MOVIE_SUGGESTIONS + movieEventId];

    const existingData = queryClient.getQueryData<{ movieNightSuggestions: FetchedMovieSuggestion[] }>(queryKey);
    if (!existingData) {
        // Cache is empty meaning no prior fetch so just reset the query to trigger a refetch
        await queryClient.resetQueries({ queryKey });
        return; // Don't create incomplete cache
    }

    queryClient.setQueryData<{ movieNightSuggestions: FetchedMovieSuggestion[] }>(queryKey, (oldData) => {
        if (!oldData) {
            // This shouldn't happen now due to the check above
            return oldData;
        }

        const suggestionsToUpdate = [...oldData.movieNightSuggestions]
        suggestionsToUpdate.push(movieSuggestion)

        return { movieNightSuggestions: suggestionsToUpdate }
    });

    // Trigger fetching for the vote count
    const voteCountQueryKey = [QUERY_KEYS.VOTECOUNT + movieSuggestion.id]
    await queryClient.resetQueries({ queryKey: voteCountQueryKey });
}