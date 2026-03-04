<script lang="ts">
	import { Edit, MessageSquare, Plus, ThumbsDown, ThumbsUp, XCircle } from '@lucide/svelte';
	import type { MovieNightEvent, VoteType } from '../../../types';
	import Badge from '../ui/badge/badge.svelte';
	import Button from '../ui/button/button.svelte';
	import {
		Card,
		CardContent,
		CardDescription,
		CardFooter,
		CardHeader,
		CardTitle
	} from '../ui/card';

	interface EventCardProps {
		userVotes: Map<number, VoteType>;
		openEventChat: (event: MovieNightEvent) => void;
		handleVote: (suggestionId: number, voteType: VoteType) => void;
		event: MovieNightEvent;
	}

	let { event, handleVote, openEventChat, userVotes }: EventCardProps = $props();

	function getEventStatus(event: MovieNightEvent): {
		label: string;
		variant: 'default' | 'outline' | 'destructive' | 'secondary';
	} {
		if (event.isLocked) return { label: 'Locked', variant: 'secondary' };
		if (new Date(event.scheduledAt) < new Date()) return { label: 'Ended', variant: 'destructive' };
		return { label: 'Voting Active', variant: 'default' };
	}

	function canVote(event: MovieNightEvent): boolean {
		return !event.isLocked && new Date(event.scheduledAt) > new Date();
	}
</script>

<Card class="overflow-hidden pt-0">
	<CardHeader class="bg-muted/50 py-4">
		<div class="flex items-center justify-between">
			<div>
				<CardTitle>{event.title}</CardTitle>
				<CardDescription>
					{new Date(event.scheduledAt).toLocaleString('en-US', {
						weekday: 'short',
						month: 'short',
						day: 'numeric',
						hour: 'numeric',
						minute: 'numeric'
					})}
				</CardDescription>
			</div>
			<div class="flex items-center gap-2">
				<Button size="sm" variant="ghost" class="relative" onclick={() => openEventChat(event)}>
					<MessageSquare class="h-4 w-4" />
					{#if event.chatMessages.length > 0}
						<span
							class="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center rounded-full bg-primary text-[10px] text-primary-foreground"
						>
							{event.chatMessages.length}
						</span>
					{/if}
				</Button>
				<Badge variant={getEventStatus(event).variant}>
					{getEventStatus(event).label}
				</Badge>
			</div>
		</div>
	</CardHeader>

	<CardContent class="p-6">
		{#if !event.isLocked && canVote(event)}
			<!-- Voting Interface -->
			<div class="mb-6">
				<h3 class="mb-4 font-semibold">Cast Your Vote</h3>
				<div class="space-y-3">
					{#each event.suggestions.filter((s) => !s.isDisqualified) as suggestion}
						<div class="rounded-lg border border-border p-4">
							<div class="mb-3 flex items-center justify-between">
								<div class="flex items-center gap-3">
									<img
										src={`https://image.tmdb.org/t/p/w92${suggestion.movieDetails?.posterPath}`}
										alt={suggestion.movieDetails?.title}
										class="h-12 w-8 rounded object-cover"
									/>
									<div>
										<span class="font-medium">{suggestion.movieDetails?.title}</span>
										<span class="ml-2 text-sm text-muted-foreground">
											Added by {suggestion.suggestedBy.name}
										</span>
									</div>
								</div>
								<div class="flex items-center gap-4">
									<div class="flex items-center gap-1">
										<ThumbsUp class="h-4 w-4 text-green-500" />
										<span class="font-semibold">
											{suggestion.votes.filter((v) => v.voteType === 'Upvote').length}
										</span>
									</div>
									<div class="flex items-center gap-1">
										<ThumbsDown class="h-4 w-4 text-red-500" />
										<span class="font-semibold">
											{suggestion.votes.filter((v) => v.voteType === 'Downvote').length}
										</span>
									</div>
								</div>
							</div>

							<div class="flex gap-2">
								<Button
									size="sm"
									variant={userVotes.get(suggestion.id) === 'Upvote' ? 'default' : 'outline'}
									class="flex-1"
									disabled={!canVote(event)}
									onclick={() => handleVote(suggestion.id, 'Upvote')}
								>
									<ThumbsUp class="mr-2 h-4 w-4" />
									Upvote
								</Button>
								<Button
									size="sm"
									variant={userVotes.get(suggestion.id) === 'Downvote' ? 'default' : 'outline'}
									class="flex-1"
									disabled={!canVote(event)}
									onclick={() => handleVote(suggestion.id, 'Downvote')}
								>
									<ThumbsDown class="mr-2 h-4 w-4" />
									Downvote
								</Button>
								<Button
									size="sm"
									variant={userVotes.get(suggestion.id) === 'Veto' ? 'destructive' : 'outline'}
									class="flex-1"
									disabled={!canVote(event)}
									onclick={() => handleVote(suggestion.id, 'Veto')}
								>
									<XCircle class="mr-2 h-4 w-4" />
									Veto
								</Button>
							</div>
						</div>
					{/each}

					{#if event.suggestions.some((s) => s.isDisqualified)}
						<div class="mt-4">
							<h4 class="mb-2 text-sm font-medium text-muted-foreground">Disqualified (Vetoed)</h4>
							{#each event.suggestions.filter((s) => s.isDisqualified) as suggestion}
								<div
									class="rounded-lg border border-destructive/30 bg-destructive/5 p-3 opacity-60"
								>
									<div class="flex items-center gap-3">
										<img
											src={`https://image.tmdb.org/t/p/w92${suggestion.movieDetails?.posterPath}`}
											alt={suggestion.movieDetails?.title}
											class="h-10 w-7 rounded object-cover"
										/>
										<span class="font-medium line-through">{suggestion.movieDetails?.title}</span>
										<Badge variant="destructive" class="ml-auto">Vetoed</Badge>
									</div>
								</div>
							{/each}
						</div>
					{/if}
				</div>
			</div>

			<div class="flex items-center justify-between rounded-lg bg-muted p-4">
				<div>
					<div class="font-medium">Add more options</div>
					<div class="text-sm text-muted-foreground">Suggest movies for everyone to vote on</div>
				</div>
				<Button size="sm">
					<Plus class="mr-2 h-4 w-4" />
					Add Movie
				</Button>
			</div>
		{:else if event.selectedMovie}
			<!-- Scheduled Event Details -->
			<div class="space-y-4">
				<div class="rounded-lg bg-primary/5 p-4">
					<div class="mb-2 flex items-center gap-2">
						<img
							src={`https://image.tmdb.org/t/p/w92${event.selectedMovie.posterPath}`}
							alt={event.selectedMovie.title}
							class="h-16 w-12 rounded object-cover"
						/>
						<div>
							<div class="text-lg font-semibold">
								{event.selectedMovie.title}
							</div>
							<div class="text-sm text-muted-foreground">
								{event.selectedMovie.runtime} min • {new Date(
									event.selectedMovie.releaseDate
								).getFullYear()}
							</div>
						</div>
					</div>
					<p class="mt-2 text-sm text-muted-foreground">
						{event.selectedMovie.overview}
					</p>
				</div>

				<div class="flex items-center justify-between rounded-lg border border-border p-4">
					<div>
						<div class="font-medium">Event Discussion</div>
						<div class="text-sm text-muted-foreground">
							{event.chatMessages.length} messages
						</div>
					</div>
					<Button size="sm" variant="outline" onclick={() => openEventChat(event)}>
						<MessageSquare class="mr-2 h-4 w-4" />
						Join Chat
					</Button>
				</div>
			</div>
		{/if}
	</CardContent>

	<CardFooter class="flex justify-between bg-muted/30">
		<Button variant="outline" size="sm">
			<Edit class="mr-2 h-4 w-4" />
			Edit Event
		</Button>
		<Button size="sm" onclick={() => openEventChat(event)}>
			<MessageSquare class="mr-2 h-4 w-4" />
			Event Chat ({event.chatMessages.length})
		</Button>
	</CardFooter>
</Card>
