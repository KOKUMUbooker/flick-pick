import { QUERY_KEYS, queryClient } from "..";
import type { MovieNightEvent } from "../../types";

export async function AddMovieNightEventToQueryCache(groupId: string, newMovieNightEvent: MovieNightEvent) {
    const queryKey = [QUERY_KEYS.MOVIE_NIGHT_EVENTS + groupId + 'upcoming']

    const existingData = queryClient.getQueryData<{ movieEvents: MovieNightEvent[] }>(queryKey);
    if (!existingData) {
        // Cache is empty meaning no prior fetch so just reset the query to trigger a refetch
        await queryClient.resetQueries({ queryKey });
        return; // Don't create incomplete cache
    }

    queryClient.setQueryData<{ movieEvents: MovieNightEvent[] }>(queryKey, (oldData) => {
        if (!oldData) {
            // This shouldn't happen now due to the check above
            return oldData;
        }

        const movieNightEventsToUpdate = [...oldData.movieEvents]
        movieNightEventsToUpdate.push(newMovieNightEvent)

        return { movieEvents: movieNightEventsToUpdate }
    });
}