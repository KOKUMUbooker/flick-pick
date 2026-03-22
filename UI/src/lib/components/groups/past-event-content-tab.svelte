<script lang="ts">
	import { Clock, MessageSquare, RefreshCw, Star } from '@lucide/svelte';
	import { createQuery } from '@tanstack/svelte-query';
	import { onMount } from 'svelte';
	import { QUERY_KEYS, apiFetch } from '../../../api';
	import { API_BASE_URL } from '../../../api/urls';
	import type { DBGroup, MovieNightEvent } from '../../../types';
	import Button from '../ui/button/button.svelte';
	import { Card, CardContent } from '../ui/card';
	import { TabsContent } from '../ui/tabs';
	import Skeleton from '../ui/skeleton/skeleton.svelte';
	interface PastEventsTabContentProps {
		selectedGroup: DBGroup | null;
		openEventChat: (event: MovieNightEvent) => void;
	}

	let { selectedGroup, openEventChat }: PastEventsTabContentProps = $props();
	let _movieEventsQuery = createQuery<
		null, // variables type
		Error, // error type
		{ movieEvents: MovieNightEvent[] } // response type
	>(() => ({
		queryKey: [QUERY_KEYS.MOVIE_NIGHT_EVENTS + selectedGroup?.id + 'past'],
		queryFn: async (data) => {
			return apiFetch(`${API_BASE_URL}/api/groups/${selectedGroup?.id}/movie-nights?status=past`, {
				method: 'GET',
				headers: { 'Content-Type': 'application/json' }
			});
		},
		enabled: selectedGroup != null
	}));

	let movieEventsQuery = $state(_movieEventsQuery);

	onMount(async () => {
		await movieEventsQuery.refetch();
	});
</script>

<TabsContent value="past">
	<div class="mb-2 flex justify-end py-2">
		<Button
			variant="outline"
			onclick={movieEventsQuery.refetch}
			disabled={movieEventsQuery.isPending || movieEventsQuery.isFetching}
		>
			<RefreshCw />
			Refetch</Button
		>
	</div>
	{#if movieEventsQuery.isFetching || movieEventsQuery.isPending}
		<div class="grid space-y-2">
			{#each [1, 2, 3] as count (count)}
				<Skeleton class="h-64" />
			{/each}
		</div>
	{:else if movieEventsQuery.data != undefined}
		{#if movieEventsQuery.data.movieEvents.length > 0}
			<div class="grid gap-4 md:grid-cols-2">
				{#each movieEventsQuery.data.movieEvents as event (event.id)}
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
										{#if event.totalRatings > 0}
											{#each [1, 2, 3, 4, 5] as i (i)}
												<Star
													class={`h-4 w-4 ${
														i < event.averageRating
															? 'fill-primary text-primary'
															: 'text-muted-foreground'
													}`}
												/>
											{/each}

											<span class="ml-1 text-sm font-medium">
												{event.averageRating.toFixed(1)} ({event.totalRatings})
											</span>
										{:else}
											{#each [1, 2, 3, 4, 5] as i (i)}
												<Star class="h-4 w-4 text-muted-foreground" />
											{/each}
											<span class="ml-1 text-sm text-muted-foreground"> No ratings </span>
										{/if}
									</div>
								</div>

								<div class="flex items-center gap-2 pt-2">
									<Button
										variant="outline"
										size="sm"
										class="flex-1"
										onclick={() => openEventChat(event)}
									>
										<MessageSquare class="mr-2 h-4 w-4" />
										Chat
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
