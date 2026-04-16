import { toast } from "svelte-sonner";
import { QUERY_KEYS, queryClient } from "..";
import { movieNightHub } from "../../hubs";
import { appState } from "../../store";
import type { MovieNightEvent } from "../../types";

/**
 * 
 * @param groupId string
 * @param targetMovieEventId string
 * @returns void
 * 
 * 1. This checks for presence of a movie event first in the upcoming movies query cache first
 * 2. If the query cache had not been initialized, reset it ie trigger a refetch
 * 3. Then recheck for its availability again in the same query cache after a refetch.
 * 4. If no data was found previously but now its found afte the refetch, return
 * 5. If data was present initially, try to check if in the movieEvent to be updated can be found so that it can be deleted
 * 6. If movieEvent to be deleted was found, stop execution 
 * 
 * 7. Otherwise, repeat the above steps but for movie events past movies query cache
 */
export async function DeleteMovieNightEventFromQueryCache(groupId: string, targetMovieEventId: string, initiatorId?: string) {
    await movieNightHub.leave(targetMovieEventId);

    // =============== Check availability in upcoming movie events cache
    const upcomingQueryKey = [QUERY_KEYS.MOVIE_NIGHT_EVENTS + groupId + 'upcoming']
    const existingDataInUpcoming = queryClient.getQueryData<{ movieEvents: MovieNightEvent[] }>(upcomingQueryKey);
    if (!existingDataInUpcoming) {
        // Cache is empty meaning no prior fetch so just reset the query to trigger a refetch
        await queryClient.resetQueries({ queryKey: upcomingQueryKey });
    }
    let inUpComingIdx = queryClient.getQueryData<{ movieEvents: MovieNightEvent[] }>(upcomingQueryKey)
        ?.movieEvents.findIndex(me => me.id === targetMovieEventId);
    // If it didn't exist before, but now it exists stop execution as latest data got fetched
    // console.log("Movie event found : ", movieEventFound)
    if (!existingDataInUpcoming && inUpComingIdx != undefined && inUpComingIdx < 0) {
        return
    }
    else if (existingDataInUpcoming && inUpComingIdx != undefined && inUpComingIdx >= 0) {
        let movieEventFound = DeleteFromQueryClientMovieEventsCache(upcomingQueryKey, targetMovieEventId, initiatorId);
        if (movieEventFound) return // Don't proceed further 
    }


    // =============== Check availability in past movie events cache
    const pastQueryKey = [QUERY_KEYS.MOVIE_NIGHT_EVENTS + groupId + 'past']
    const existingDataInPast = queryClient.getQueryData<{ movieEvents: MovieNightEvent[] }>(pastQueryKey);
    if (!existingDataInPast) {
        await queryClient.resetQueries({ queryKey: pastQueryKey });
    }
    let inPastIdx = queryClient.getQueryData<{ movieEvents: MovieNightEvent[] }>(pastQueryKey)
        ?.movieEvents.findIndex(me => me.id === targetMovieEventId);
    // If it didn't exist before, but now it exists stop execution as latest data got fetched
    if (!existingDataInUpcoming && inPastIdx != undefined && inPastIdx < 0) return
    else if (existingDataInUpcoming && inPastIdx != undefined && inPastIdx >= 0) DeleteFromQueryClientMovieEventsCache(pastQueryKey, targetMovieEventId, initiatorId)
}

function DeleteFromQueryClientMovieEventsCache(queryKey: string[], targetMovieEventId: string, initiatorId?: string): boolean {
    let movieEventFound = false;
    queryClient.setQueryData<{ movieEvents: MovieNightEvent[] }>(queryKey, (oldData) => {
        if (!oldData) {
            // This shouldn't happen now due to the check above
            return oldData;
        }

        const movieEventsToUpdate = [...oldData.movieEvents]
        const targetMovieEventIdx = movieEventsToUpdate.findIndex(s => s.id === targetMovieEventId);
        if (targetMovieEventIdx < 0) {
            return oldData
        }

        if (initiatorId != appState.user?.id) {
            toast.info(`The event "${movieEventsToUpdate[targetMovieEventIdx].name}" was deleted by an admin.`);
        }
        movieEventFound = true;
        movieEventsToUpdate.splice(targetMovieEventIdx, 1);

        return { movieEvents: movieEventsToUpdate }
    });

    return movieEventFound;
}