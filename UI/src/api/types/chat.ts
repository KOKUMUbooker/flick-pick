import type { ChatMessage } from "../../types";

export interface CreateChatData {
    UserId: string;
    Message: string;
    SentAt: string;
    ConnectionId: string;
}

export interface FetchedChatData {
    messages: ChatMessage[];
    nextCursor: string;
    hasMore: boolean;
}