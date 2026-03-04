<script lang="ts">
	import { Calendar, Plus } from '@lucide/svelte';
	import type { EventItem, MovieNightEvent, VoteType } from '../../../types';
	import Button from '../ui/button/button.svelte';
	import { Card, CardContent } from '../ui/card';
	import { TabsContent } from '../ui/tabs';
	import EventCard from './event-card.svelte';

	interface UpcomingEventsTabContentProps {
		userVotes: Map<number, VoteType>;
		events: EventItem;
		openEventChat: (event: MovieNightEvent) => void;
		handleVote: (suggestionId: number, voteType: VoteType) => void;
		createNewEvent: () => void;
	}

	let props: UpcomingEventsTabContentProps = $props();
</script>

<TabsContent value="upcoming" class="mt-6">
	{#if props.events.upcoming.length > 0}
		<div class="space-y-6">
			{#each props.events.upcoming as event}
				<EventCard
					{event}
					handleVote={props.handleVote}
					openEventChat={props.openEventChat}
					userVotes={props.userVotes}
				/>
			{/each}
		</div>
	{:else}
		<Card>
			<CardContent class="py-12 text-center">
				<Calendar class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
				<h3 class="mb-2 text-lg font-semibold">No upcoming movie nights</h3>
				<p class="mb-4 text-sm text-muted-foreground">Plan your first movie night with the group</p>
				<Button onclick={props.createNewEvent}>
					<Plus class="mr-2 h-4 w-4" />
					Plan Movie Night
				</Button>
			</CardContent>
		</Card>
	{/if}
</TabsContent>
