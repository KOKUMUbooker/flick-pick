<script lang="ts">
	import { Eye, ThumbsDown, ThumbsUp, Trash, XCircle } from '@lucide/svelte';
	import { createMutation, createQuery } from '@tanstack/svelte-query';
	import { toast } from 'svelte-sonner';
	import { apiFetch, QUERY_KEYS, queryClient } from '../../../api';
	import type { CreateVoteData } from '../../../api/types';
	import type {
		FetchedMovieSuggestion,
		FetchedVoteCountData,

		VoteForSuggestionRes

	} from '../../../api/types/fetch-movie-suggestions';
	import { API_BASE_URL } from '../../../api/urls';
	import { movieNightHub } from '../../../hubs';
	import { appState, getAppUser, hubIsDisconnected } from '../../../store';
	import { VoteType, type MovieNightEvent } from '../../../types';
	import CustomDialog from '../common/CustomDialog.svelte';
	import Button from '../ui/button/button.svelte';
	import Spinner from '../ui/spinner/spinner.svelte';
	import MovieDetailsContent from './movie-details-content.svelte';
	import { DeleteSuggestionFromQueryCache, UpdateSuggestionInQueryCache, UpdateVoteCountInQueryCache } from '../../../api/query-cache-crud';
	import type { VoteCountEventPayload } from '../../../hubs/event-payload-types';

	interface MovieSuggestionItem {
		selectedGroupId: string | undefined;
		event: MovieNightEvent;
		suggestion: FetchedMovieSuggestion;
		movieSuggestionSuccessfullyFetched: boolean;
		hasSuggestionVotesBeenFetched: boolean;
	}

	let {
		suggestion,
		event = $bindable(),
		selectedGroupId,
		movieSuggestionSuccessfullyFetched,
		hasSuggestionVotesBeenFetched = $bindable()
	}: MovieSuggestionItem = $props();
	let user = getAppUser();
	let showDeleteWarnDialog = $state(false);
	let showMovieDetailsDialog = $state(false);

	let selectedVote = $state<VoteType | null>(null);

	const onShowMovieDialogChange = (show: boolean) => {
		showMovieDetailsDialog = show;
	};

	function canVote(event: MovieNightEvent): boolean {
		return !event.isLocked && new Date(event.scheduledAt) > new Date();
	}

	let suggestionDeleteMutation = createMutation<
		{ message: string }, // response type
		Error, // error type
		{ Initiator: string , ConnectionId : string } // variables type
	>(() => ({
		mutationFn: async (data) => {
			return apiFetch(`${API_BASE_URL}/api/movie-events/${event.id}/suggestion/${suggestion.id}`, {
				method: 'DELETE',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(data)
			});
		},
		onSuccess: async () => {
			await DeleteSuggestionFromQueryCache(event.id,suggestion.id)
			// await queryClient.invalidateQueries({
			// 	queryKey: [QUERY_KEYS.MOVIE_NIGHT_EVENTS + selectedGroupId + 'upcoming']
			// });
		}
	}));

	let votesCountQuery = createQuery<
		null, // variables type
		Error, // error type
		FetchedVoteCountData // response type
	>(() => ({
		queryKey: [QUERY_KEYS.VOTECOUNT + suggestion.id],
		queryFn: async () => {
			const url = `${API_BASE_URL}/api/movie-suggestions/${suggestion.id}/votes?initiator=${encodeURIComponent(user?.id || '')}`;
			// console.log('Fetching from:', url);
			return apiFetch(url, {
				method: 'GET',
				headers: {
					Accept: 'application/json'
				}
			});
		},
		enabled: movieSuggestionSuccessfullyFetched && !!suggestion.id && !!user
	}));

	// let votesQuery = createQuery<
	// 	null, // variables type
	// 	Error, // error type
	// 	{ votes: Vote[] } // response type
	// >(() => ({
	// 	queryKey: [QUERY_KEYS.VOTES + suggestion.id],
	// 	queryFn: async () => {
	// 		const url = `${API_BASE_URL}/api/movie-suggestions/${suggestion.id}/votes`;
	// 		console.log('Fetching from:', url);
	// 		return apiFetch(url, {
	// 			method: 'GET',
	// 			headers: {
	// 				Accept: 'application/json'
	// 			}
	// 		});
	// 	},
	// 	enabled: movieSuggestionSuccessfullyFetched && !!suggestion.id
	// }));

	// suggestions/{movieSuggestionId}/vote
	let voteMutation = createMutation<
		VoteForSuggestionRes, // response type
		Error, // error type
		CreateVoteData // variables type
	>(() => ({
		mutationFn: async (data) => {
			return apiFetch(`${API_BASE_URL}/api/movie-suggestions/${suggestion.id}/vote`, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(data)
			});
		},
		onSuccess: async ({data},input) => {
			if (input.VoteType == VoteType.Veto) {
				await UpdateSuggestionInQueryCache(event.id,data.movieSuggestion);
			}
			await UpdateVoteCountInQueryCache(suggestion.id,data.voteCountData,input.VoteType)
		}
	}));

	const handleVoteClick = async (vote: VoteType) => {
		try {
			if (!user) return toast.error('Please login to proceed', { richColors: true });
			if (!hubIsDisconnected()) {
				await movieNightHub.join(event.id);
			}
			selectedVote = vote;
			await voteMutation.mutateAsync({
				UserId: user.id,
				VoteType: vote,
				ConnectionId: appState.hubConnection.connectionId || ''
			});
		} catch (err) {
			console.error(err);
		} finally {
			selectedVote = null;
		}
	};

	const onShowDelWarningOnchange = (show: boolean) => {
		showDeleteWarnDialog = show;
	};

	const onProceedSuggestionDelete = async () => {
		if (!user) return toast.error('Please login to proceed', { richColors: true });
		if (!hubIsDisconnected()) {
				await movieNightHub.join(event.id);
		}
		
		const res = await suggestionDeleteMutation.mutateAsync({ 
			Initiator: user.id, 
			ConnectionId: appState.hubConnection.connectionId || '' 
		});
		toast.success(res.message, { richColors: true });
		onShowDelWarningOnchange(false);
	};

	$effect(() => {
		hasSuggestionVotesBeenFetched = votesCountQuery.isSuccess;
	});

	const iconFillClasses = 'fill-current text-primary';
</script>

<CustomDialog
	header={{ title: 'Movie Details' }}
	bind:open={showMovieDetailsDialog}
	onOpenChange={onShowMovieDialogChange}
>
	<MovieDetailsContent movie={suggestion.movie} />
</CustomDialog>
<CustomDialog
	header={{ title: 'Delete movie suggestion' }}
	bind:open={showDeleteWarnDialog}
	onOpenChange={onShowDelWarningOnchange}
	isLoading={suggestionDeleteMutation.isPending}
	actions={{ onProceed: onProceedSuggestionDelete }}
>
	<p>
		Are you sure you want to delete this movie suggestion and all of its data? This action is
		irreversible so proceed with caution.
	</p>
</CustomDialog>
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
					{votesCountQuery?.data?.voteData.upvoteCount}
				</span>
			</div>
			<div class="flex items-center gap-1">
				<ThumbsDown class="h-4 w-4 text-red-500" />
				<span class="font-semibold">
					{votesCountQuery?.data?.voteData.downvoteCount}
				</span>
			</div>
		</div>
	</div>

	{#if suggestion.suggestedBy.email !== user?.email}
		<div class="flex items-center justify-between">
			<div class="flex-1">
				<Button size="sm" variant="outline" onclick={() => (showMovieDetailsDialog = true)}>
					<Eye class="mr-2 h-4 w-4" />
					View Movie
				</Button>
			</div>
			<div class="flex flex-3 gap-2">
				<Button
					size="sm"
					variant="outline"
					class="flex-1"
					disabled={!canVote(event) || voteMutation.isPending}
					onclick={handleVoteClick.bind(null, VoteType.Upvote)}
				>
					{#if selectedVote == VoteType.Upvote && voteMutation.isPending}
						<Spinner />
					{/if}
					<ThumbsUp
						class={`mr-2 h-4 w-4 ${votesCountQuery.data?.voteData?.userVote == VoteType.Upvote ? iconFillClasses : ''}`}
					/>
					Upvote
				</Button>
				<Button
					size="sm"
					variant="outline"
					class="flex-1"
					disabled={!canVote(event) || voteMutation.isPending}
					onclick={handleVoteClick.bind(null, VoteType.Downvote)}
				>
					{#if selectedVote == VoteType.Downvote && voteMutation.isPending}
						<Spinner />
					{/if}
					<ThumbsDown
						class={`mr-2 h-4 w-4 ${votesCountQuery.data?.voteData?.userVote == VoteType.Downvote ? iconFillClasses : ''}`}
					/>
					Downvote
				</Button>
				<Button
					size="sm"
					variant="outline"
					class="flex-1"
					disabled={!canVote(event) || voteMutation.isPending}
					onclick={handleVoteClick.bind(null, VoteType.Veto)}
				>
					{#if selectedVote == VoteType.Veto && voteMutation.isPending}
						<Spinner />
					{/if}
					<XCircle
						class={`mr-2 h-4 w-4 ${votesCountQuery.data?.voteData?.userVote == VoteType.Veto ? iconFillClasses : ''}`}
					/>
					Veto
				</Button>
			</div>
		</div>
	{:else}
		<div class="flex justify-between gap-2">
			<Button size="sm" variant="outline" onclick={() => (showMovieDetailsDialog = true)}>
				<Eye class="mr-2 h-4 w-4" />
				View Movie
			</Button>
			<Button
				size="sm"
				variant="destructive"
				disabled={!canVote(event)}
				onclick={() => (showDeleteWarnDialog = true)}
			>
				<Trash class="mr-2 h-4 w-4" />
				Delete suggestion
			</Button>
		</div>
	{/if}
</div>
