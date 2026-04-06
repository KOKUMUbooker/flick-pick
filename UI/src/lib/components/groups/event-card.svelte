<script lang="ts">
	import { Calculator, Edit, MessageSquare, Plus, Trash } from '@lucide/svelte';
	import { createMutation, createQuery } from '@tanstack/svelte-query';
	import { toast } from 'svelte-sonner';
	import { QUERY_KEYS, apiFetch, queryClient } from '../../../api';
	import type { FetchedMovieSuggestion } from '../../../api/types/fetch-movie-suggestions';
	import { API_BASE_URL } from '../../../api/urls';
	import { movieNightHub } from '../../../hubs';
	import { getAppUser, hubIsConnected } from '../../../store';
	import { type DBGroup, type MovieNightEvent } from '../../../types';
	import CustomDialog from '../common/CustomDialog.svelte';
	import AddMovieNightForm from '../dashboard/forms/add-movie-night-form.svelte';
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
	import ValidMovieSuggestion from './valid-movie-suggestion.svelte';
	import VetoedMovieSection from './vetoed-movie-section.svelte';

	interface EventCardProps {
		selectedGroup: DBGroup | null;
		event: MovieNightEvent;
		openEventChat: (event: MovieNightEvent) => void;
	}

	let { selectedGroup, event = $bindable(), openEventChat }: EventCardProps = $props();
	let user = getAppUser();

	let showSuggestionFlow = $state(false);
	let showAddMovieNightDialog = $state(false);
	let showDeleteWarnDialog = $state(false);
	let showComputeResultsDialog = $state(false);
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
			return apiFetch(
				`${API_BASE_URL}/api/movie-nights/${event.id}/suggestions?initiator=${encodeURIComponent(user?.id || '')}`,
				{
					method: 'GET',
					headers: { 'Content-Type': 'application/json' }
				}
			);
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

	function onShowMovieNightDialogOpenChange(show: boolean) {
		showAddMovieNightDialog = show;
	}
	let movieEventDeleteMutation = createMutation<
		{ message: string }, // response type
		Error, // error type
		void // variables type
	>(() => ({
		mutationFn: async (data) => {
			return apiFetch(
				`${API_BASE_URL}/api/groups/${selectedGroup?.id || ''}/movie-event/${event.id}?userId=${encodeURIComponent(user?.id || '')}`,
				{
					method: 'DELETE',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify(data)
				}
			);
		},
		onSuccess: async () => {
			await queryClient.invalidateQueries({
				queryKey: [QUERY_KEYS.MOVIE_NIGHT_EVENTS + selectedGroup?.id + 'upcoming']
			});
		}
	}));
	let computeEventResultsMutation = createMutation<
		{ message: string }, // response type
		Error, // error type
		{ Initiator: string } // variables type
	>(() => ({
		mutationFn: async (data) => {
			return apiFetch(`${API_BASE_URL}/api/movieEvent/${event.id}/compute-results`, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(data)
			});
		},
		onSuccess: async () => {
			// TODO: Update this to only fetch the updated movie event not all
			await queryClient.invalidateQueries({
				queryKey: [QUERY_KEYS.MOVIE_NIGHT_EVENTS + selectedGroup?.id + 'upcoming']
			});
		}
	}));
	const onShowDelWarningOnchange = (show: boolean) => {
		showDeleteWarnDialog = show;
	};

	const onProceedMovieEventDelete = async () => {
		const res = await movieEventDeleteMutation.mutateAsync();
		toast.success(res.message, { richColors: true });
	};
	const onProceedComputeEventResults = async () => {
		if (!user) return toast.error('Please log in', { richColors: true });
		const res = await computeEventResultsMutation.mutateAsync({ Initiator: user.id });
		toast.success(res.message, { richColors: true });
	};

	$effect(() => {
		(async () => {
			if (hubIsConnected()) {
				// console.log(`${user?.email} is joining event ${event.id} ...`);
				await movieNightHub.join(event.id);
			}
		})();
	});

	// onDestroy(async()=>{
	// 	await movieNightHub.leave(event.id)
	// })

	const onShowComputeResultsOnchange = (show: boolean) => {
		showComputeResultsDialog = show;
	};
</script>

<CustomDialog
	header={{ title: 'Compute Event Results' }}
	bind:open={showComputeResultsDialog}
	onOpenChange={(show) => (showComputeResultsDialog = show)}
	isLoading={computeEventResultsMutation.isPending}
	actions={{ onProceed: onProceedComputeEventResults }}
>
	<p>
		Are you sure you want to compute the movie to be watched for the movie event. This will lock the
		event thus preventing incoming votes on the event.
	</p>
</CustomDialog>
<CustomDialog
	header={{ title: 'Delete Movie Event' }}
	bind:open={showDeleteWarnDialog}
	onOpenChange={onShowDelWarningOnchange}
	isLoading={movieEventDeleteMutation.isPending}
	actions={{ onProceed: onProceedMovieEventDelete }}
>
	<p>
		Are you sure you want to delete this movie event and all of its data? This action is
		irreversible so proceed with caution.
	</p>
</CustomDialog>
<CustomDialog bind:open={showAddMovieNightDialog} onOpenChange={onShowMovieNightDialogOpenChange}>
	<AddMovieNightForm
		bind:defaultMovieEvent={event}
		{selectedGroup}
		onOpenChange={onShowMovieNightDialogOpenChange}
	/>
</CustomDialog>
{#if showSuggestionFlow}
	<SuggestionFlow
		movieEventId={event.id}
		movieNightId={event.id}
		onCancel={() => (showSuggestionFlow = false)}
		onSuggestionAdded={() => (showSuggestionFlow = false)}
	/>
{/if}
<Card class="overflow-hidden pt-0 pb-0">
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

	<CardContent class="px-6">
		{#if !event.isLocked && canVote(event)}
			<!-- Voting Interface -->
			<div class="mb-6">
				<h3 class="mb-4 font-semibold">Cast Your Vote</h3>
				<div class="space-y-3">
					{#each movieSuggestionQuery.data?.movieNightSuggestions.filter((s) => !s.isDisqualified) as suggestion (suggestion.id)}
						<ValidMovieSuggestion {event} {suggestion} selectedGroupId={selectedGroup?.id} />
					{/each}

					{#if movieSuggestionQuery.data?.movieNightSuggestions.some((s) => s.isDisqualified)}
						<VetoedMovieSection
							{event}
							bind:movieSuggestionQuery
							selectedGroupId={selectedGroup?.id}
						/>
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
				<div class="text-lg font-semibold">Selected movie</div>
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

	<CardFooter class="flex justify-between bg-muted/30 py-4">
		<div>
			{#if selectedGroup?.isUserAdmin}
				<div class="flex flex-row items-center gap-2">
					<Button variant="outline" size="sm" onclick={() => (showAddMovieNightDialog = true)}>
						<Edit class="mr-2 h-4 w-4" />
						Edit Event
					</Button>
					<Button variant="destructive" size="sm" onclick={() => (showDeleteWarnDialog = true)}>
						<Trash class="mr-2 h-4 w-4" />
						Delete Event
					</Button>
				</div>
			{/if}
		</div>
		<div>
			<Button size="sm" variant="outline" onclick={() => openEventChat(event)}>
				<MessageSquare class="mr-2 h-4 w-4" />
				Event Chat
			</Button>
			{#if selectedGroup?.isUserAdmin}
				<Button size="sm" onclick={() => (showComputeResultsDialog = true)}>
					<Calculator />
					Compute results
				</Button>
			{/if}
		</div>
	</CardFooter>
</Card>
