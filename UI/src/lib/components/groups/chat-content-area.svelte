<script lang="ts">
	import { ChevronLeft, MessageCircle, RefreshCw } from '@lucide/svelte';
	import { createQuery } from '@tanstack/svelte-query';
	import { onMount } from 'svelte';
	import { SvelteDate } from 'svelte/reactivity';
	import { QUERY_KEYS, apiFetch } from '../../../api';
	import { API_BASE_URL } from '../../../api/urls';
	import type { BadgeVariants, ChatMessage, DBGroup, MovieNightEvent } from '../../../types';
	import { Avatar, AvatarFallback } from '../ui/avatar';
	import Badge from '../ui/badge/badge.svelte';
	import Button from '../ui/button/button.svelte';
	import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card';
	import ChatMsgSkeleton from './chat-msg-skeleton.svelte';

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
		enabled: selectedGroup != null && selectedEvent != null
		// staleTime: Infinity
	}));

	let chatMsgsQuery = $state(_chatMsgsQuery);

	onMount(async () => {
		const res = await chatMsgsQuery.refetch();
		if (res.data) curCursor = res.data?.nextCursor;
	});
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
												<span class="text-xs text-muted-foreground">{msg.sentAt}</span>
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
						onsubmit={(e) => {
							e.preventDefault();
							// const form = e.target as HTMLFormElement;
							// const input = form.elements.namedItem('message') as HTMLInputElement;
							// if (input.value.trim()) {
							// 	sendEventChatMessage(selectedEvent.id, input.value);
							// 	input.value = '';
							// }
						}}
					>
						<input
							name="message"
							type="text"
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
