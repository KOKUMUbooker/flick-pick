import { QUERY_KEYS, queryClient } from "../../api";
import type { FetchedMovieSuggestion } from "../../api/types/fetch-movie-suggestions";
import { appState } from "../../store";

export type SuggestionEventActions = "vote" | "create" | "delete" | "edit"

export function suggestionListeners() {
    appState.hubConnection.off("suggestion")

    appState.hubConnection.on("suggestion", async (movieEventId: string, movieSuggestion: FetchedMovieSuggestion, action: SuggestionEventActions) => {
        const queryKey = [QUERY_KEYS.MOVIE_SUGGESTIONS + movieEventId];

        // console.log(`suggestion event recieved by ${appState.user?.email} with data : ${movieSuggestion}`)
        // Check if cache exists
        const existingData = queryClient.getQueryData<{ movieNightSuggestions: FetchedMovieSuggestion[] }>(queryKey);
        if (!existingData) {
            // Cache is empty meaning no prior fetch so just reset the query to trigger a refetch
            await queryClient.resetQueries({
                queryKey: [QUERY_KEYS.CHAT_MSG + movieEventId]
            });
            return; // Don't create incomplete cache
        }


        // loop through the results searching for the updated suggestion & update it once a match is found
        // Cache exists 
        queryClient.setQueryData<{ movieNightSuggestions: FetchedMovieSuggestion[] }>(queryKey, (oldData) => {
            if (!oldData) {
                // This shouldn't happen now due to the check above
                return oldData;
            }
            const suggestionsToUpdate = [...oldData.movieNightSuggestions]
            if (action === "create") {
                suggestionsToUpdate.push(movieSuggestion)
            }
            else if (action === "delete") {
                const targetSuggestionIdx = suggestionsToUpdate.findIndex(s => s.id === movieSuggestion.id);
                if (targetSuggestionIdx < 0) {
                    return oldData
                }
                delete suggestionsToUpdate[targetSuggestionIdx]
            }
            else if (action === "vote" || action === "edit") {
                const targetSuggestionIdx = suggestionsToUpdate.findIndex(s => s.id === movieSuggestion.id);
                if (targetSuggestionIdx < 0) {
                    return oldData
                }

                suggestionsToUpdate[targetSuggestionIdx] = movieSuggestion
            }

            return { movieNightSuggestions: suggestionsToUpdate }
        });
    });
}