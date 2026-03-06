<script lang="ts">
	import type { DBGroup } from '../../../types';
	import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card';
	import { TabsContent } from '../ui/tabs';

	interface StatsTabContentProps {
		selectedGroup: DBGroup | null;
	}

	let { selectedGroup }: StatsTabContentProps = $props();
	// TODO: Use selected group to fetch its stats

	// svelte-ignore state_referenced_locally
	console.log('selectedGroup : ', selectedGroup);

	let stats = $state({
		moviesWatched: 0,
		totalVotes: 0,
		averageRating: 0,
		streak: 0
	});
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
					<div class="text-2xl font-bold">{stats.moviesWatched}</div>
					<div class="text-sm text-muted-foreground">Movies Watched</div>
				</div>
				<div class="rounded-lg border border-border p-4">
					<div class="text-2xl font-bold">{stats.totalVotes}</div>
					<div class="text-sm text-muted-foreground">Total Votes</div>
				</div>
				<div class="rounded-lg border border-border p-4">
					<div class="text-2xl font-bold">{stats.averageRating}/5</div>
					<div class="text-sm text-muted-foreground">Avg Rating</div>
				</div>
				<div class="rounded-lg border border-border p-4">
					<div class="text-2xl font-bold">{stats.streak} weeks</div>
					<div class="text-sm text-muted-foreground">Current Streak</div>
				</div>
			</div>
		</CardContent>
	</Card>
</TabsContent>
