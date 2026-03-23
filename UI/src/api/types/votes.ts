import type { VoteType } from "../../types";

export interface CreateVoteData {
    UserId: string;
    VoteType: VoteType
}