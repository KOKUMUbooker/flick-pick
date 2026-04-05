<script lang="ts">
	import { ChevronLeft, MessageCircle, RefreshCw } from '@lucide/svelte';
	import { createInfiniteQuery, createMutation, type InfiniteData } from '@tanstack/svelte-query';
	import { onDestroy } from 'svelte';
	import { toast } from 'svelte-sonner';
	import { SvelteDate } from 'svelte/reactivity';
	import { QUERY_KEYS, apiFetch, queryClient } from '../../../api';
	import type { CreateChatData, FetchedChatData } from '../../../api/types';
	import { API_BASE_URL } from '../../../api/urls';
	import { movieNightHub } from '../../../hubs';
	import { appState, getAppUser, hubIsDisconnected } from '../../../store';
	import type { BadgeVariants, ChatMessage, DBGroup, MovieNightEvent } from '../../../types';
	import { Avatar, AvatarFallback } from '../ui/avatar';
	import Badge from '../ui/badge/badge.svelte';
	import Button from '../ui/button/button.svelte';
	import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card';
	import { Spinner } from '../ui/spinner';
	import ChatMsgSkeleton from './chat-msg-skeleton.svelte';

	interface ChatContentAreaProps {
		closeEventChat: () => void;
		selectedGroup: DBGroup | null;
		selectedEvent: MovieNightEvent | null;
		getEventStatus: (event: MovieNightEvent) => {
			label: string;
			variant: BadgeVariants;
		};
	}

	const { closeEventChat, getEventStatus, selectedEvent, selectedGroup }: ChatContentAreaProps = $props();
	let user = getAppUser();
	let chatMsg = $state("");
	
	// svelte-ignore non_reactive_update
	let messagesContainer: HTMLDivElement;
	let isLoadingMore = false;
	let scrollTimeout: NodeJS.Timeout;
	let isInitialLoad = $state(true);

	// Create infinite query
	let _chatMsgsQuery = createInfiniteQuery<
		FetchedChatData,
		Error,
		InfiniteData<FetchedChatData>
	>(() => ({
		queryKey: [QUERY_KEYS.CHAT_MSG + selectedEvent?.id],
		queryFn: async ({ pageParam }) => {
			const cursor = pageParam || new SvelteDate().toISOString();
			return apiFetch(
				`${API_BASE_URL}/api/movie-night/${selectedEvent?.id}/chat?before=${cursor}&movieNightId=${selectedEvent?.id}`,
				{
					method: 'GET',
					headers: { 'Content-Type': 'application/json' }
				}
			);
		},
		initialPageParam: new SvelteDate().toISOString(),
		getNextPageParam: (lastPage) => {
			if (lastPage.hasMore && lastPage.messages.length > 0) {
				const oldestMessage = lastPage.messages[0];
				return oldestMessage.sentAt;
			}
			return undefined;
		},
		enabled: selectedGroup != null && selectedEvent != null,
		gcTime: 1000 * 60 * 30, // Keep cache for 30 minutes (default is 5 minutes)
		staleTime: 1000 * 60 * 5, // Consider data stale after 5 minutes
	}));

	let chatMsgsQuery = $state(_chatMsgsQuery);

	// Function to load older messages
	async function loadOlderMessages() {
		// Don't load if:
		// 1. No more pages
		// 2. Already fetching next page
		// 3. Currently loading
		// 4. No data yet
		if (!chatMsgsQuery.hasNextPage || 
			chatMsgsQuery.isFetchingNextPage || 
			isLoadingMore || 
			!chatMsgsQuery.data) {
			return;
		}
		
		isLoadingMore = true;
		
		// Save current scroll position relative to the top
		const previousScrollTop = messagesContainer?.scrollTop || 0;
		const previousScrollHeight = messagesContainer?.scrollHeight || 0;
		
		try {
			await chatMsgsQuery.fetchNextPage();
			
			// Only adjust scroll if we actually loaded new messages
			if (messagesContainer && chatMsgsQuery.data) {
				// Use requestAnimationFrame for smoother scroll adjustment
				requestAnimationFrame(() => {
					const newScrollHeight = messagesContainer.scrollHeight;
					const heightDifference = newScrollHeight - previousScrollHeight;
					
					// Only adjust if we added new content
					if (heightDifference > 0) {
						// Maintain the same relative position from the top
						messagesContainer.scrollTop = previousScrollTop + heightDifference;
					}
				});
			}
		} catch (error) {
			console.error('Failed to load older messages:', error);
		} finally {
			isLoadingMore = false;
		}
	}

	// Handle scroll event
	function handleScroll() {
		if (!messagesContainer || !chatMsgsQuery.data) return;
		
		// Don't trigger if no more messages
		if (!chatMsgsQuery.hasNextPage) return;
		
		// Don't trigger if already loading
		if (chatMsgsQuery.isFetchingNextPage || isLoadingMore) return;
		
		// Clear previous timeout
		clearTimeout(scrollTimeout);
		
		// Debounce scroll events
		scrollTimeout = setTimeout(() => {
			// Check if scrolled within 50px of the top
			const isNearTop = messagesContainer.scrollTop <= 50;
			
			if (isNearTop) {
				loadOlderMessages();
			}
		}, 150);
	}

	// Auto-scroll to bottom on initial load and new messages
	$effect(() => {
		if (chatMsgsQuery.data && messagesContainer) {
			if (isInitialLoad) {
				// Scroll to bottom on initial load
				setTimeout(() => {
					messagesContainer.scrollTop = messagesContainer.scrollHeight;
					isInitialLoad = false;
				}, 100);
			}
		}
	});

	// Clean up timeout on destroy
	onDestroy(() => {
		if (scrollTimeout) {
			clearTimeout(scrollTimeout);
		}
	});

	let chatMsgsMutation = createMutation<
		{message:string, msgId : string},
		Error,
		CreateChatData
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
		onSuccess: async (res, variables) => {
			// Get the current query data
			const queryKey = [QUERY_KEYS.CHAT_MSG + selectedEvent?.id];
			const currentData = queryClient.getQueryData<InfiniteData<FetchedChatData>>(queryKey);
			if (!user) return console.error("Please login");
			
			if (currentData && currentData.pages.length > 0) {
				// Create the new message object
				const newMessage: ChatMessage = {
					id: res.msgId,
					userId: user.id,
					user: {
						fullName: user.fullName,
						email: user.email
					},
					message: variables.Message,
					sentAt: variables.SentAt
				};
				
				// Add the new message to the first page (most recent page)
				const updatedPages = [...currentData.pages];
				updatedPages[0] = {
					...updatedPages[0],
					messages: [...updatedPages[0].messages, newMessage]
				};
				
				// Update the cache
				queryClient.setQueryData(queryKey, {
					...currentData,
					pages: updatedPages
				});
			} else {
				// If no data exists, just invalidate
				await queryClient.invalidateQueries({ queryKey });
			}
			
			
			// Auto-scroll to bottom after sending a new message
			setTimeout(() => {
				if (messagesContainer) {
					messagesContainer.scrollTop = messagesContainer.scrollHeight;
				}
			}, 100);
		}
	}));

</script>

{#if selectedEvent}
	<div class="space-y-4">
		<div class="flex flex-row items-center justify-between">
			<Button variant="outline" onclick={closeEventChat}>
				<ChevronLeft class="mr-2 h-4 w-4" />
				Back to {selectedGroup?.name}
			</Button>
			<Button
				variant="secondary"
				disabled={chatMsgsQuery.isFetching || chatMsgsQuery.isPending}
				onclick={async() => {
					isInitialLoad = true;
					await queryClient.resetQueries({ 
						queryKey: [QUERY_KEYS.CHAT_MSG + selectedEvent?.id]
					});
				}}
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
				<div 
					bind:this={messagesContainer}
					onscroll={handleScroll}
					class="max-h-125 space-y-4 overflow-y-auto p-6"
				>
					<!-- Show loading indicator when fetching older messages -->
					{#if chatMsgsQuery.isFetchingNextPage}
						<div class="py-2 text-center">
							<div class="text-xs text-muted-foreground">Loading older messages...</div>
						</div>
					{/if}
					
					<!-- Show "No more messages" indicator when reached the end -->
					{#if !chatMsgsQuery.hasNextPage && !chatMsgsQuery.isPending}
						{#if chatMsgsQuery.data?.pages && chatMsgsQuery.data?.pages.length > 0 && chatMsgsQuery.data?.pages?.[0]?.messages.length > 0}
							<div class="py-2 text-center">
								<div class="text-xs text-muted-foreground">You've reached the beginning of the conversation</div>
							</div>
						{/if}
					{/if}
					
					{#if chatMsgsQuery.isPending}
						{#each Array(3) as _, i (i)}
							<ChatMsgSkeleton />
						{/each}
					{:else}
						{#if chatMsgsQuery.data}
							{#if chatMsgsQuery.data.pages[0]?.messages.length > 0}
								<!-- Display messages in chronological order (oldest to newest) -->
								{#each [...chatMsgsQuery.data.pages].reverse() as page (page.nextCursor)}
									{#each page.messages as msg (msg.id)}
										<div class="flex gap-3">
											<Avatar class="h-8 w-8">
												<AvatarFallback>{msg.user.fullName.charAt(0)}</AvatarFallback>
											</Avatar>
											<div class="flex-1">
												<div class="mb-1 flex items-center gap-2">
													<span class="font-medium">{msg.user.fullName}</span>
													<span class="text-xs text-muted-foreground">
														{new Date(msg.sentAt).toLocaleString()}
													</span>
												</div>
												<p class="rounded-lg bg-muted p-3">{msg.message}</p>
											</div>
										</div>
									{/each}
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
							if (!selectedEvent) return toast.error("No active movie event in selection", { richColors: true });
							if (!user) return toast.error("Please log in", { richColors: true });
							if (chatMsg.trim()) {
								// console.log(`Sender ${user.email} , their connection id before : ${appState.hubConnection.connectionId}`)
								if (!hubIsDisconnected()){
									await movieNightHub.join(selectedEvent.id)
								}
								// console.log(`Sender ${user.email} , their connection id after : ${appState.hubConnection.connectionId}`)
								const data: CreateChatData = {
									Message: chatMsg,
									SentAt: new Date().toISOString(),
									UserId: user.id,
									ConnectionId : appState.hubConnection.connectionId || ""
								};
								await chatMsgsMutation.mutateAsync(data);
								chatMsg = '';
							}
						}}
					>
						<input
							type="text"
							disabled={chatMsgsMutation.isPending}
							bind:value={chatMsg}
							placeholder="Type your message..."
							class="flex-1 rounded-lg border border-border bg-background px-4 py-2"
						/>
						<Button type="submit" disabled={chatMsgsMutation.isPending}>
							{#if chatMsgsMutation.isPending}
								<Spinner/>
							{/if}
							Send</Button>
					</form>
				</div>
			</CardContent>
		</Card>
	</div>
{/if}