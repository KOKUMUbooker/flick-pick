import { QUERY_KEYS, queryClient } from "..";
import type { FetchedMovieSuggestion } from "../types/fetch-movie-suggestions";

export async function DeleteSuggestionFromQueryCache(movieEventId: string, suggestionId: string) {
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
        const targetSuggestionIdx = suggestionsToUpdate.findIndex(s => s.id === suggestionId);
        if (targetSuggestionIdx < 0) {
            return oldData
        }
        delete suggestionsToUpdate[targetSuggestionIdx]

        return { movieNightSuggestions: suggestionsToUpdate }
    });
}