import type { InfiniteData } from "@tanstack/svelte-query";
import { QUERY_KEYS, queryClient } from "../../api";
import { appState } from "../../store";
import type { ChatMessage } from "../../types";
import type { FetchedChatData } from "../../api/types";
import { SvelteDate } from "svelte/reactivity";

export function chatListeners() {
    appState.hubConnection.off("msg")

    appState.hubConnection.on("msg", async (movieEventId: string, newMessage: ChatMessage) => {
        const queryKey = [QUERY_KEYS.CHAT_MSG + movieEventId];

        // Check if cache exists
        const existingData = queryClient.getQueryData<InfiniteData<FetchedChatData>>(queryKey);

        if (!existingData || !existingData.pages.length) {
            // Cache is empty meaning no prior fetch so just reset the query to trigger a refetch
            await queryClient.resetQueries({
                queryKey: [QUERY_KEYS.CHAT_MSG + movieEventId]
            });
            return; // Don't create incomplete cache
        }

        // Cache exists - safely add the new message
        queryClient.setQueryData<InfiniteData<FetchedChatData>>(queryKey, (oldData) => {
            if (!oldData || !oldData.pages.length) {
                // This shouldn't happen now due to the check above
                return oldData;
            }

            const updatedPages = [...oldData.pages];
            const messageExists = updatedPages[0].messages.some(m => m.id === newMessage.id);

            if (!messageExists) {
                updatedPages[0] = {
                    ...updatedPages[0],
                    messages: [...updatedPages[0].messages, newMessage]
                };

                // Sort to maintain order
                updatedPages[0].messages.sort((a, b) =>
                    new SvelteDate(a.sentAt).getTime() - new SvelteDate(b.sentAt).getTime()
                );
            }

            return {
                ...oldData,
                pages: updatedPages
            };
        });
    });
}