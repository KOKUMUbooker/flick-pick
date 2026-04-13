import { AddSuggestionToQueryCache, DeleteSuggestionFromQueryCache, UpdateSuggestionInQueryCache } from "../../api/query-cache-crud";
import type { FetchedMovieSuggestion } from "../../api/types/fetch-movie-suggestions";
import { appState } from "../../store";

export type SuggestionEventActions = "vote" | "create" | "delete" | "edit"

export function suggestionListeners() {
    appState.hubConnection.off("suggestion")

    appState.hubConnection.on("suggestion", async (movieEventId: string, movieSuggestion: FetchedMovieSuggestion, action: SuggestionEventActions) => {
        if (action === "create") {
            await AddSuggestionToQueryCache(movieEventId, movieSuggestion)
        }
        else if (action === "delete") {
            await DeleteSuggestionFromQueryCache(movieEventId, movieSuggestion.id)
        }
        else if (action === "edit") {
            await UpdateSuggestionInQueryCache(movieEventId, movieSuggestion)
        }
    });
}