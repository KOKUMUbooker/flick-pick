<script lang="ts">
	import { Calendar, Plus, RefreshCw } from '@lucide/svelte';
	import { createQuery } from '@tanstack/svelte-query';
	import { apiFetch, QUERY_KEYS } from '../../../api';
	import { API_BASE_URL } from '../../../api/urls';
	import { getAppUser } from '../../../store';
	import type { DBGroup, MovieNightEvent } from '../../../types';
	import Button from '../ui/button/button.svelte';
	import { Card, CardContent } from '../ui/card';
	import Skeleton from '../ui/skeleton/skeleton.svelte';
	import { TabsContent } from '../ui/tabs';
	import EventCard from './event-card.svelte';

	interface UpcomingEventsTabContentProps {
		selectedGroup: DBGroup | null;
		createNewEvent: () => void;
		openEventChat: (event: MovieNightEvent) => void;
	}

	let { createNewEvent, selectedGroup, openEventChat }: UpcomingEventsTabContentProps = $props();
	let user = getAppUser();
	let _movieEventsQuery = createQuery<
		null, // variables type
		Error, // error type
		{ movieEvents: MovieNightEvent[] } // response type
	>(() => ({
		queryKey: [QUERY_KEYS.MOVIE_NIGHT_EVENTS + selectedGroup?.id + 'upcoming'],
		queryFn: async (data) => {
			return apiFetch(
				`${API_BASE_URL}/api/groups/${selectedGroup?.id}/movie-nights?status=upcoming&initiator=${user?.id || ''}`,
				{
					method: 'GET',
					headers: { 'Content-Type': 'application/json' }
				}
			);
		},
		enabled: selectedGroup != null
	}));

	let movieEventsQuery = $state(_movieEventsQuery);
</script>

<TabsContent value="upcoming">
	<div class="grid space-y-2">
		<div class="flex justify-end py-2">
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
		{:else if movieEventsQuery.data}
			{#if movieEventsQuery.data.movieEvents.length > 0}
				<div class="">
					{#each movieEventsQuery.data.movieEvents as event, i (event.id)}
						<div class="mb-6">
							<EventCard
								bind:event={movieEventsQuery.data.movieEvents[i]}
								{openEventChat}
								{selectedGroup}
							/>
						</div>
					{/each}
				</div>
			{:else}
				<Card>
					<CardContent class="py-12 text-center">
						<Calendar class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
						<h3 class="mb-2 text-lg font-semibold">No upcoming movie nights</h3>
						{#if selectedGroup?.isUserAdmin}
							<p class="mb-4 text-sm text-muted-foreground">
								Plan your first movie night with the group
							</p>
							<Button onclick={createNewEvent}>
								<Plus class="mr-2 h-4 w-4" />
								Plan Movie Night
							</Button>
						{/if}
					</CardContent>
				</Card>
			{/if}
		{:else}
			<Card>
				<CardContent class="py-12 text-center">
					<Calendar class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
					<h3 class="mb-2 text-lg font-semibold">No upcoming movie nights</h3>
					<p class="mb-4 text-sm text-muted-foreground">
						Plan your first movie night with the group
					</p>
					<Button onclick={createNewEvent}>
						<Plus class="mr-2 h-4 w-4" />
						Plan Movie Night
					</Button>
				</CardContent>
			</Card>
		{/if}
	</div>
</TabsContent>
