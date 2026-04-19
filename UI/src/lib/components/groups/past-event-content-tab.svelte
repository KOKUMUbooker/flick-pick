<script lang="ts">
	import { Clock, EditIcon, MessageSquare, RefreshCw, Star, Trash } from '@lucide/svelte';
	import { createQuery } from '@tanstack/svelte-query';
	import { onMount } from 'svelte';
	import { QUERY_KEYS, apiFetch } from '../../../api';
	import { API_BASE_URL } from '../../../api/urls';
	import { getAppUser } from '../../../store';
	import type { DBGroup, MovieNightEvent } from '../../../types';
	import Button from '../ui/button/button.svelte';
	import { Card, CardContent } from '../ui/card';
	import Skeleton from '../ui/skeleton/skeleton.svelte';
	import { TabsContent } from '../ui/tabs';
	import CardHeader from '../ui/card/card-header.svelte';
	import CardTitle from '../ui/card/card-title.svelte';
	import Separator from '../ui/separator/separator.svelte';
	import PastEventCard from './past-event-card.svelte';
	interface PastEventsTabContentProps {
		selectedGroup: DBGroup | null;
		openEventChat: (event: MovieNightEvent) => void;
	}

	let { selectedGroup, openEventChat }: PastEventsTabContentProps = $props();
	let showAddGroupDialog = $state(false);
	let showDeleteWarnDialog = $state(false);
	let user = getAppUser();
	let _movieEventsQuery = createQuery<
		null, // variables type
		Error, // error type
		{ movieEvents: MovieNightEvent[] } // response type
	>(() => ({
		queryKey: [QUERY_KEYS.MOVIE_NIGHT_EVENTS + selectedGroup?.id + 'past'],
		queryFn: async (data) => {
			return apiFetch(
				`${API_BASE_URL}/api/groups/${selectedGroup?.id}/movie-nights?status=past&initiator=${user?.id || ''}`,
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
				{#each movieEventsQuery.data.movieEvents as event, i (event.id)}
					<PastEventCard
						bind:event={movieEventsQuery.data.movieEvents[i]}
						{openEventChat}
						{selectedGroup}
					/>
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
