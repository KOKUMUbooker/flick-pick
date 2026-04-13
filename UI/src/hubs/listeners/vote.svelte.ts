import { QUERY_KEYS } from "../../api";
import { UpdateVoteCountInQueryCache } from "../../api/query-cache-crud";
import { appState } from "../../store";
import type { VoteCountEventPayload } from "../event-payload-types";

export function voteListeners() {
    appState.hubConnection.off("vote")

    appState.hubConnection.on("vote", async (suggestionId: string, payload: VoteCountEventPayload) => {
        const queryKey = [QUERY_KEYS.VOTECOUNT + suggestionId];

        UpdateVoteCountInQueryCache(suggestionId, payload)
    });
}