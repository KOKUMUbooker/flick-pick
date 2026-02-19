<script lang="ts">
	import { ChevronLeft, MessageCircle, Lock } from '@lucide/svelte';

	import { AvatarFallback, Avatar } from '../ui/avatar';

	import Button from '../ui/button/button.svelte';
	import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '../ui/card';
	import type { BadgeVariants, MovieNightEvent } from '../../../types';
	import Badge from '../ui/badge/badge.svelte';

	interface MainContentAreaProps {
		showEventChat: boolean;
		closeEventChat: () => void;
		selectedEvent: MovieNightEvent | null;
		getEventStatus: (event: MovieNightEvent) => {
			label: string;
			variant: BadgeVariants;
		};
	}

	const props: MainContentAreaProps = $props();
</script>

{#if props?.selectedEvent}
	<!-- Event-Specific Chat View -->
	<div class="space-y-4">
		<Button variant="ghost" size="sm" onclick={props.closeEventChat} class="mb-2">
			<ChevronLeft class="mr-2 h-4 w-4" />
			Back to {props?.selectedEvent?.title}
		</Button>

		<Card>
			<CardHeader class="border-b border-border bg-muted/30">
				<div class="flex items-center justify-between">
					<div>
						<CardTitle class="flex items-center gap-2">
							<MessageCircle class="h-5 w-5" />
							Event Chat: {props?.selectedEvent?.title}
						</CardTitle>
						<CardDescription>
							{props?.selectedEvent
								? new Date(props?.selectedEvent?.scheduledAt).toLocaleDateString('en-US', {
										weekday: 'long',
										month: 'long',
										day: 'numeric',
										hour: 'numeric',
										minute: 'numeric'
									})
								: ''}
						</CardDescription>
					</div>
					<Badge variant={props.getEventStatus(props.selectedEvent).variant as BadgeVariants}>
						{props.getEventStatus(props.selectedEvent).label}
					</Badge>
				</div>
			</CardHeader>

			<CardContent class="p-0">
				<!-- Chat Messages -->
				<div class="max-h-125 space-y-4 overflow-y-auto p-6">
					{#each props?.selectedEvent?.chatMessages as message}
						<div class="flex gap-3">
							<Avatar class="h-8 w-8">
								<AvatarFallback>{message.user.name.charAt(0)}</AvatarFallback>
							</Avatar>
							<div class="flex-1">
								<div class="mb-1 flex items-center gap-2">
									<span class="font-medium">{message.user.name}</span>
									<span class="text-xs text-muted-foreground">{message.createdAt}</span>
								</div>
								<p class="rounded-lg bg-muted p-3">{message.message}</p>
							</div>
						</div>
					{/each}

					{#if props?.selectedEvent?.chatMessages.length === 0}
						<div class="py-12 text-center">
							<MessageCircle class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
							<h3 class="mb-2 text-lg font-semibold">No messages yet</h3>
							<p class="text-sm text-muted-foreground">
								Be the first to start the conversation about this movie night
							</p>
						</div>
					{/if}
				</div>

				<!-- Message Input -->
				{#if !props?.selectedEvent?.isLocked && new Date(props?.selectedEvent?.scheduledAt) > new Date()}
					<div class="border-t border-border p-4">
						<form
							class="flex gap-2"
							onsubmit={(e) => {
								e.preventDefault();
								const form = e.target as HTMLFormElement;
								const input = form.elements.namedItem('message') as HTMLInputElement;
								// if (input.value.trim()) {
								// 	sendEventChatMessage(props.selectedEvent.id, input.value);
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
				{/if}

				<!-- Read-only for past events -->
				{#if props?.selectedEvent?.isLocked || new Date(props?.selectedEvent?.scheduledAt || new Date()) < new Date()}
					<div class="border-t border-border bg-muted/30 p-4 text-center">
						<p class="text-sm text-muted-foreground">
							<Lock class="mr-2 inline h-4 w-4" />
							This event has ended. Chat is now read-only.
						</p>
					</div>
				{/if}
			</CardContent>
		</Card>
	</div>
{/if}
