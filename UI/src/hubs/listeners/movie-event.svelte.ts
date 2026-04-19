import { AddMovieNightEventToQueryCache, DeleteMovieNightEventFromQueryCache, UpdateMovieNightEventRatingDataToQueryCache, UpdateMovieNightEventToQueryCache } from "../../api/query-cache-crud";
import { appState } from "../../store";
import type { MovieNightEvent } from "../../types";

export type MovieNightEventActions = "create" | "delete" | "edit" | "rate"

export function movieNightEventListeners() {
    appState.hubConnection.off("movieEvent")

    appState.hubConnection.on("movieEvent", async (movieEventId: string, movieEvent: MovieNightEvent, action: MovieNightEventActions, initiatorId?: string) => {
        if (action === "create") {
            await AddMovieNightEventToQueryCache(movieEventId, movieEvent)
        }
        else if (action === "delete") {
            await DeleteMovieNightEventFromQueryCache(movieEventId, movieEvent.id, initiatorId)
        }
        else if (action === "edit") {
            await UpdateMovieNightEventToQueryCache(movieEventId, movieEvent, initiatorId)
        }
        else if (action === "rate") {
            await UpdateMovieNightEventRatingDataToQueryCache(movieEventId, movieEvent)
        }
    });
}