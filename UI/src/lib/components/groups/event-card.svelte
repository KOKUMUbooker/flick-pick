<script lang="ts">
	import {
		Edit,
		Eye,
		MessageSquare,
		Plus,
		ThumbsDown,
		ThumbsUp,
		Trash,
		XCircle
	} from '@lucide/svelte';
	import { createQuery } from '@tanstack/svelte-query';
	import { QUERY_KEYS, apiFetch } from '../../../api';
	import type { FetchedMovieSuggestion } from '../../../api/types/fetch-movie-suggestions';
	import { API_BASE_URL } from '../../../api/urls';
	import { getAppUser } from '../../../store';
	import { VoteType, type DBGroup, type MovieNightEvent } from '../../../types';
	import SuggestionFlow from '../dashboard/suggestion-flow.svelte';
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
	import CustomDialog from '../common/CustomDialog.svelte';
	import AddMovieNightForm from '../dashboard/forms/add-movie-night-form.svelte';

	interface EventCardProps {
		selectedGroup: DBGroup | null;
		event: MovieNightEvent;
		openEventChat: (event: MovieNightEvent) => void;
	}

	let { selectedGroup, event = $bindable(), openEventChat }: EventCardProps = $props();
	let user = getAppUser();
	
	let showSuggestionFlow = $state(false);
	let showAddMovieNightDialog = $state(false);
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

	let _movieSuggestionQuery = createQuery<
		null, // variables type
		Error, // error type
		{ movieNightSuggestions: FetchedMovieSuggestion[] } // response type
	>(() => ({
		queryKey: [QUERY_KEYS.MOVIE_SUGGESTIONS + event.id],
		queryFn: async () => {
			return apiFetch(`${API_BASE_URL}/api/movie-nights/${event.id}/suggestions`, {
				method: 'GET',
				headers: { 'Content-Type': 'application/json' }
			});
		}
	}));

	let movieSuggestionQuery = $state(_movieSuggestionQuery);
	let hasGivenSuggestion = $derived(
		movieSuggestionQuery?.data
			? movieSuggestionQuery.data.movieNightSuggestions.find(
					(ms) => ms.suggestedBy.email == user?.email
				)
			: true
	);

	function onShowMovieNightDialogOpenChange(show:boolean){
		showAddMovieNightDialog = show
	}
</script>

<CustomDialog bind:open={showAddMovieNightDialog} onOpenChange={onShowMovieNightDialogOpenChange}>
	<AddMovieNightForm bind:defaultMovieEvent={event} selectedGroup={selectedGroup} onOpenChange={onShowMovieNightDialogOpenChange} />
</CustomDialog>
{#if showSuggestionFlow}
	<SuggestionFlow
		movieEventId={event.id}
		movieNightId={event.id}
		onCancel={() => (showSuggestionFlow = false)}
		onSuggestionAdded={() => (showSuggestionFlow = false)}
	/>
{/if}
<Card class="overflow-hidden pt-0">
	<CardHeader class="bg-muted/50 py-4">
		<div class="flex items-center justify-between">
			<div>
				<CardTitle>{event.name}</CardTitle>
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
					<!-- {#if event.chatMessages.length > 0}
						<span
							class="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center rounded-full bg-primary text-[10px] text-primary-foreground"
						>
							{event.chatMessages.length}
						</span>
					{/if} -->
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
					{#each movieSuggestionQuery.data?.movieNightSuggestions.filter((s) => !s.isDisqualified) as suggestion (suggestion.id)}
						<div
							class={`rounded-lg border border-border ${user?.email == suggestion.suggestedBy.email ? 'bg-primary/15' : ''} p-4`}
						>
							<div class="mb-3 flex items-center justify-between">
								<div class="flex items-center gap-3">
									<img
										src={`https://image.tmdb.org/t/p/w92${suggestion.movie?.posterPath}`}
										alt={suggestion.movie?.title}
										class="h-12 w-8 rounded object-cover"
									/>
									<div>
										<span class="font-medium">{suggestion.movie?.title}</span>
										<span class="ml-2 text-sm text-muted-foreground">
											Added by {user?.email == suggestion.suggestedBy.email
												? 'You'
												: suggestion.suggestedBy.fullName}
										</span>
									</div>
								</div>
								<div class="flex items-center gap-4">
									<div class="flex items-center gap-1">
										<ThumbsUp class="h-4 w-4 text-green-500" />
										<span class="font-semibold">
											{suggestion.votes.filter((v) => v.voteType === VoteType.Upvote).length}
										</span>
									</div>
									<div class="flex items-center gap-1">
										<ThumbsDown class="h-4 w-4 text-red-500" />
										<span class="font-semibold">
											{suggestion.votes.filter((v) => v.voteType === VoteType.Downvote).length}
										</span>
									</div>
								</div>
							</div>

							{#if suggestion.suggestedBy.email !== user?.email}
								<div class="flex gap-2">
									<Button
										size="sm"
										variant="outline"
										class="flex-1"
										disabled={!canVote(event)}
										onclick={() => {}}
									>
										<ThumbsUp class="mr-2 h-4 w-4" />
										Upvote
									</Button>
									<Button
										size="sm"
										variant="outline"
										class="flex-1"
										disabled={!canVote(event)}
										onclick={() => {}}
									>
										<ThumbsDown class="mr-2 h-4 w-4" />
										Downvote
									</Button>
									<Button
										size="sm"
										variant="outline"
										class="flex-1"
										disabled={!canVote(event)}
										onclick={() => {}}
									>
										<XCircle class="mr-2 h-4 w-4" />
										Veto
									</Button>
								</div>
							{:else}
								<div class="flex justify-end gap-2">
									<Button size="sm" variant="outline" disabled={!canVote(event)} onclick={() => {}}>
										<Eye class="mr-2 h-4 w-4" />
										View Movie details
									</Button>
									<Button
										size="sm"
										variant="destructive"
										disabled={!canVote(event)}
										onclick={() => {}}
									>
										<Trash class="mr-2 h-4 w-4" />
										Delete suggestion
									</Button>
								</div>
							{/if}
						</div>
					{/each}

					{#if movieSuggestionQuery.data?.movieNightSuggestions.some((s) => s.isDisqualified)}
						<div class="mt-4">
							<h4 class="mb-2 text-sm font-medium text-muted-foreground">Disqualified (Vetoed)</h4>
							{#each movieSuggestionQuery.data?.movieNightSuggestions.filter((s) => s.isDisqualified) as suggestion (suggestion.id)}
								<div
									class="rounded-lg border border-destructive/30 bg-destructive/5 p-3 opacity-60"
								>
									<div class="flex items-center gap-3">
										<img
											src={`https://image.tmdb.org/t/p/w92${suggestion.movie?.posterPath}`}
											alt={suggestion.movie?.title}
											class="h-10 w-7 rounded object-cover"
										/>
										<span class="font-medium line-through">{suggestion.movie?.title}</span>
										<Badge variant="destructive" class="ml-auto">Vetoed</Badge>
									</div>
								</div>
							{/each}
						</div>
					{/if}
				</div>
			</div>

			{#if !hasGivenSuggestion}
				<div class="flex items-center justify-between rounded-lg bg-muted p-4">
					<div>
						<div class="font-medium">Add more options</div>
						<div class="text-sm text-muted-foreground">Suggest movies for everyone to vote on</div>
					</div>
					<Button size="sm" onclick={() => (showSuggestionFlow = true)}>
						<Plus class="mr-2 h-4 w-4" />
						Add Movie
					</Button>
				</div>
			{/if}
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
					<div class="font-medium">Event Discussion</div>
					<Button size="sm" variant="outline" onclick={() => openEventChat(event)}>
						<MessageSquare class="mr-2 h-4 w-4" />
						Join Chat
					</Button>
				</div>
			</div>
		{/if}
	</CardContent>

	<CardFooter class="flex justify-between bg-muted/30">
		<div>
			{#if selectedGroup?.isUserAdmin}
				<div class="flex flex-row items-center gap-2">
					<Button variant="outline" size="sm" onclick={()=>showAddMovieNightDialog=true}>
						<Edit class="mr-2 h-4 w-4" />
						Edit Event
					</Button>
					<Button variant="destructive" size="sm">
						<Trash class="mr-2 h-4 w-4" />
						Delete Event
					</Button>
				</div>
			{/if}
		</div>
		<Button  size="sm" onclick={() => openEventChat(event)}>
			<MessageSquare class="mr-2 h-4 w-4" />
			Event Chat
		</Button>
	</CardFooter>
</Card>
