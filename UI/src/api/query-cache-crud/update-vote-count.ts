import { QUERY_KEYS, queryClient } from "..";
import type { VoteCountEventPayload } from "../../hubs/event-payload-types";
import type { VoteType } from "../../types";
import type { FetchedVoteCountData } from "../types/fetch-movie-suggestions";

export async function UpdateVoteCountInQueryCache(movieSuggestionId: string, updatedVoteCount: VoteCountEventPayload, userVote?: VoteType) {
    const queryKey = [QUERY_KEYS.VOTECOUNT + movieSuggestionId]

    const existingData = queryClient.getQueryData<FetchedVoteCountData>(queryKey);
    if (!existingData) {
        // Cache is empty meaning no prior fetch so just reset the query to trigger a refetch
        await queryClient.resetQueries({ queryKey });
        return; // Don't create incomplete cache
    }

    queryClient.setQueryData<FetchedVoteCountData>(queryKey, (oldData) => {
        if (!oldData) {
            // This shouldn't happen now due to the check above
            return oldData;
        }

        const voteCountData: FetchedVoteCountData = { voteData: { ...oldData.voteData } }

        voteCountData.voteData.upvoteCount = updatedVoteCount.upvoteCount
        voteCountData.voteData.downvoteCount = updatedVoteCount.downvoteCount

        // Toggling behavior for setting user vote
        if (userVote != undefined) {
            if (userVote === oldData.voteData.userVote) voteCountData.voteData.userVote = undefined
            else voteCountData.voteData.userVote = userVote;
        }

        return voteCountData
    });
}