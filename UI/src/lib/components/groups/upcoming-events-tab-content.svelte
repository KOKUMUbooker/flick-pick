<script lang="ts">
	import { Calendar, Plus } from '@lucide/svelte';
	import { createQuery } from '@tanstack/svelte-query';
	import { onMount } from 'svelte';
	import { apiFetch, QUERY_KEYS } from '../../../api';
	import { API_BASE_URL } from '../../../api/urls';
	import type { DBGroup, MovieNightEvent } from '../../../types';
	import Button from '../ui/button/button.svelte';
	import { Card, CardContent } from '../ui/card';
	import { TabsContent } from '../ui/tabs';
	import EventCard from './event-card.svelte';

	interface UpcomingEventsTabContentProps {
		selectedGroup: DBGroup | null;
		createNewEvent: () => void;
		openEventChat: (event: MovieNightEvent) => void;
	}

	let { createNewEvent, selectedGroup, openEventChat }: UpcomingEventsTabContentProps = $props();

	let _movieNightsQuery = createQuery<
		null, // variables type
		Error, // error type
		MovieNightEvent[] // response type
	>(() => ({
		queryKey: [QUERY_KEYS.MOVIE_NIGHT_EVENTS],
		queryFn: async (data) => {
			return apiFetch(
				`${API_BASE_URL}/api/groups/${selectedGroup?.id}/movie-nights?status=upcoming`,
				{
					method: 'GET',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify(data)
				}
			);
		},
		enabled: selectedGroup != null
	}));

	let movieNightsQuery = $state(_movieNightsQuery);

	onMount(async () => {
		await movieNightsQuery.refetch();
	});
</script>

<TabsContent value="upcoming" class="mt-6">
	{#if movieNightsQuery.data != undefined}
		<div class="space-y-6">
			{#each movieNightsQuery.data as event (event.id)}
				<EventCard {event} {openEventChat} />
			{/each}
		</div>
	{:else}
		<Card>
			<CardContent class="py-12 text-center">
				<Calendar class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
				<h3 class="mb-2 text-lg font-semibold">No upcoming movie nights</h3>
				<p class="mb-4 text-sm text-muted-foreground">Plan your first movie night with the group</p>
				<Button onclick={createNewEvent}>
					<Plus class="mr-2 h-4 w-4" />
					Plan Movie Night
				</Button>
			</CardContent>
		</Card>
	{/if}
</TabsContent>
