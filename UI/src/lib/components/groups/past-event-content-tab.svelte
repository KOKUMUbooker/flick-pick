<script lang="ts">
	import { Clock, MessageSquare, Star } from '@lucide/svelte';
	import type { EventItem, MovieNightEvent } from '../../../types';
	import Button from '../ui/button/button.svelte';
	import { Card, CardContent } from '../ui/card';
	import { TabsContent } from '../ui/tabs';

	interface PastEventsTabContentProps {
		events: EventItem;
		openEventChat: (event: MovieNightEvent) => void;
		createNewEvent: () => void;
	}
	let props: PastEventsTabContentProps = $props();
</script>

<TabsContent value="past" class="mt-6">
	{#if props.events.past.length > 0}
		<div class="grid gap-4 md:grid-cols-2">
			{#each props.events.past as event}
				<Card class="group transition-all hover:shadow-lg">
					<CardContent class="p-6">
						<div class="mb-4 flex items-start justify-between">
							<div class="flex items-center gap-3">
								<img
									src={`https://image.tmdb.org/t/p/w92${event.selectedMovie?.posterPath}`}
									alt={event.selectedMovie?.title}
									class="h-16 w-12 rounded object-cover"
								/>
								<div>
									<h3 class="text-lg font-semibold">{event.selectedMovie?.title}</h3>
									<p class="text-sm text-muted-foreground">
										{new Date(event.scheduledAt).toLocaleDateString('en-US', {
											month: 'short',
											day: 'numeric',
											year: 'numeric'
										})}
									</p>
								</div>
							</div>
						</div>

						<div class="space-y-3">
							<div class="flex items-center justify-between">
								<span class="text-sm text-muted-foreground">Group Rating</span>
								<div class="flex items-center gap-1">
									{#each Array(5) as _, i}
										<Star
											class={`h-4 w-4 ${
												i <
												Math.round(
													event.ratings.reduce(
														(acc: any, r: { rating: any }) => acc + r.rating,
														0
													) / event.ratings.length || 0
												)
													? 'fill-primary text-primary'
													: 'text-muted-foreground'
											}`}
										/>
									{/each}
									<span class="ml-1 text-sm font-medium">
										{(
											event.ratings.reduce((acc: any, r: { rating: any }) => acc + r.rating, 0) /
												event.ratings.length || 0
										).toFixed(1)}
									</span>
								</div>
							</div>

							<div class="flex items-center gap-2 pt-2">
								<Button
									variant="outline"
									size="sm"
									class="flex-1"
									onclick={() => props.openEventChat(event)}
								>
									<MessageSquare class="mr-2 h-4 w-4" />
									Chat ({event.chatMessages.length})
								</Button>
								<Button variant="outline" size="sm" class="flex-1">
									<Star class="mr-2 h-4 w-4" />
									Rate
								</Button>
							</div>
						</div>
					</CardContent>
				</Card>
			{/each}
		</div>
	{:else}
		<Card>
			<CardContent class="py-12 text-center">
				<Clock class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
				<h3 class="mb-2 text-lg font-semibold">No past events yet</h3>
				<p class="text-sm text-muted-foreground">Your group's watch history will appear here</p>
			</CardContent>
		</Card>
	{/if}
</TabsContent>
