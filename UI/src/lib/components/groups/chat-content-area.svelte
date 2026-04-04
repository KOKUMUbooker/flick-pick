<script lang="ts">
	import { ChevronLeft, MessageCircle, RefreshCw } from '@lucide/svelte';
	import { createMutation, createQuery } from '@tanstack/svelte-query';
	import { toast } from 'svelte-sonner';
	import { SvelteDate } from 'svelte/reactivity';
	import { QUERY_KEYS, apiFetch, queryClient } from '../../../api';
	import type { CreateChatData } from '../../../api/types';
	import { API_BASE_URL } from '../../../api/urls';
	import { getAppUser } from '../../../store';
	import type { BadgeVariants, ChatMessage, DBGroup, MovieNightEvent } from '../../../types';
	import { Avatar, AvatarFallback } from '../ui/avatar';
	import Badge from '../ui/badge/badge.svelte';
	import Button from '../ui/button/button.svelte';
	import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card';
	import ChatMsgSkeleton from './chat-msg-skeleton.svelte';
	import { onMount } from 'svelte';

	interface FetchedChatData {
		messages: ChatMessage[];
		nextCursor: string;
		hasMore: boolean;
	}

	interface ChatContentAreaProps {
		closeEventChat: () => void;
		selectedGroup: DBGroup | null;
		selectedEvent: MovieNightEvent | null;
		getEventStatus: (event: MovieNightEvent) => {
			label: string;
			variant: BadgeVariants;
		};
	}

	const { closeEventChat, getEventStatus, selectedEvent, selectedGroup }: ChatContentAreaProps =
		$props();
	let user = getAppUser();
	let chatMsg = $state("");
	let curCursor = $state(new SvelteDate().toISOString());

	let _chatMsgsQuery = createQuery<
		null, // variables type
		Error, // error type
		FetchedChatData // response type
	>(() => ({
		queryKey: [QUERY_KEYS.CHAT_MSG + selectedEvent?.id],
		queryFn: async () => {
			return apiFetch(
				`${API_BASE_URL}/api/movie-night/${selectedEvent?.id}/chat?before=${curCursor || ''}&movieNightId=${selectedEvent?.id}`,
				{
					method: 'GET',
					headers: { 'Content-Type': 'application/json' }
				}
			);
		},
		enabled: selectedGroup != null && selectedEvent != null,
		// staleTime: Infinity
	}));
	let chatMsgsQuery = $state(_chatMsgsQuery);
	
	let chatMsgsMutation = createMutation<
		void, // response type
		Error, // error type
		CreateChatData // variables type
	>(() => ({
 		mutationFn: async (data) => {
			return apiFetch(
				`${API_BASE_URL}/api/movie-night/${selectedEvent?.id}/chat`,
				{
					method: 'POST',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify(data)
				}
			);
		},
		onSuccess: async () => {
			curCursor = new SvelteDate().toISOString()
			await queryClient.invalidateQueries({ queryKey:[QUERY_KEYS.CHAT_MSG + selectedEvent?.id]});
		}
	}));

</script>

{#if selectedEvent}
	<!-- Event-Specific Chat View -->
	<div class="space-y-4">
		<div class="flex flex-row items-center justify-between">
			<Button variant="outline" onclick={closeEventChat}>
				<ChevronLeft class="mr-2 h-4 w-4" />
				Back to {selectedGroup?.name}
			</Button>
			<Button
				variant="secondary"
				disabled={chatMsgsQuery.isFetching || chatMsgsQuery.isPending}
				onclick={chatMsgsQuery.refetch}
			>
				<RefreshCw />
			</Button>
		</div>

		<Card>
			<CardHeader class="border-b border-border bg-muted/30">
				<div class="flex items-center justify-between">
					<div>
						<CardTitle class="flex items-center gap-2">
							<MessageCircle class="h-5 w-5" />
							Event Chat: {selectedEvent?.name}
						</CardTitle>
						<CardDescription>
							{selectedEvent
								? new Date(selectedEvent?.scheduledAt).toLocaleDateString('en-US', {
										weekday: 'long',
										month: 'long',
										day: 'numeric',
										hour: 'numeric',
										minute: 'numeric'
									})
								: ''}
						</CardDescription>
					</div>
					<Badge variant={getEventStatus(selectedEvent).variant as BadgeVariants}>
						{getEventStatus(selectedEvent).label}
					</Badge>
				</div>
			</CardHeader>

			<CardContent class="p-0">
				<!-- Chat Messages -->
				<div class="max-h-125 space-y-4 overflow-y-auto p-6">
					{#if chatMsgsQuery.isPending}
						{#each Array(3) as _, i (i)}
							<ChatMsgSkeleton />
						{/each}
					{:else}
						{#if chatMsgsQuery.data }
							{#if chatMsgsQuery.data.messages.length > 0}
								{#each chatMsgsQuery.data.messages as msg (msg.id)}
									<div class="flex gap-3">
										<Avatar class="h-8 w-8">
											<AvatarFallback>{msg.user.fullName.charAt(0)}</AvatarFallback>
										</Avatar>
										<div class="flex-1">
											<div class="mb-1 flex items-center gap-2">
												<span class="font-medium">{msg.user.fullName}</span>
												<span class="text-xs text-muted-foreground">{new Date(msg.sentAt).toLocaleString()}</span>
											</div>
											<p class="rounded-lg bg-muted p-3">{msg.message}</p>
										</div>
									</div>
								{/each}
							{:else}	
								<div class="py-12 text-center">
									<MessageCircle class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
									<h3 class="mb-2 text-lg font-semibold">No messages yet</h3>
									<p class="text-sm text-muted-foreground">
										Be the first to start the conversation about this movie night
									</p>
								</div>
							{/if}
						{/if}
					{/if}
				</div>

				<!-- Message Input -->
				<div class="border-t border-border p-4">
					<form
						class="flex gap-2"
						onsubmit={async(e) => {
							e.preventDefault();
							if (!selectedEvent) return toast.error("No active movie event in selection",{richColors:true})
							if (!user) return toast.error("Please log in",{richColors:true})
							if (chatMsg.trim()) {
								const data : CreateChatData= {
									Message: chatMsg,
									SentAt: new Date().toISOString(),
									UserId: user.id
								}
								await chatMsgsMutation.mutateAsync(data);
								chatMsg = '';
							}
						}}
					>
						<input
							type="text"
							bind:value={chatMsg}
							placeholder="Type your message..."
							class="flex-1 rounded-lg border border-border bg-background px-4 py-2"
						/>
						<Button type="submit">Send</Button>
					</form>
				</div>
			</CardContent>
		</Card>
	</div>
{/if}
