<script lang="ts">
	import { Film, Star } from '@lucide/svelte';
	import { stats } from '../../../data';
	import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '../ui/card';
	import { TabsContent } from '../ui/tabs';
	import Separator from '../ui/separator/separator.svelte';
	import type { EventItem, StatsItem } from '../../../types';

	interface StatsTabContentProps {
		stats: StatsItem;
		events: EventItem;
	}
	let props: StatsTabContentProps = $props();
</script>

<TabsContent value="stats" class="mt-6">
	<Card>
		<CardHeader>
			<CardTitle>Group Statistics</CardTitle>
			<CardDescription>Your movie night analytics</CardDescription>
		</CardHeader>
		<CardContent>
			<div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
				<div class="rounded-lg border border-border p-4">
					<div class="text-2xl font-bold">{props.stats.moviesWatched}</div>
					<div class="text-sm text-muted-foreground">Movies Watched</div>
				</div>
				<div class="rounded-lg border border-border p-4">
					<div class="text-2xl font-bold">{props.stats.totalVotes}</div>
					<div class="text-sm text-muted-foreground">Total Votes</div>
				</div>
				<div class="rounded-lg border border-border p-4">
					<div class="text-2xl font-bold">{props.stats.averageRating}/5</div>
					<div class="text-sm text-muted-foreground">Avg Rating</div>
				</div>
				<div class="rounded-lg border border-border p-4">
					<div class="text-2xl font-bold">{props.stats.streak} weeks</div>
					<div class="text-sm text-muted-foreground">Current Streak</div>
				</div>
			</div>

			<Separator class="my-6" />

			<h3 class="mb-4 font-semibold">Recent Activity</h3>
			<div class="space-y-3">
				{#each props.events.past.slice(0, 3) as event}
					<div class="flex items-center justify-between">
						<div class="flex items-center gap-2">
							<Film class="h-4 w-4 text-muted-foreground" />
							<span>{event.selectedMovie?.title}</span>
						</div>
						<div class="flex items-center gap-1">
							<Star class="h-3 w-3 fill-primary text-primary" />
							<span class="text-sm">
								{(
									event.ratings.reduce((acc, r) => acc + r.rating, 0) / event.ratings.length || 0
								).toFixed(1)}
							</span>
						</div>
					</div>
				{/each}
			</div>
		</CardContent>
	</Card>
</TabsContent>
