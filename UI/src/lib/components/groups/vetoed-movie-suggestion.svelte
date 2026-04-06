<script lang="ts">
	import { Eye, Undo2 } from '@lucide/svelte';
	import { createMutation, type CreateQueryResult } from '@tanstack/svelte-query';
	import { toast } from 'svelte-sonner';
	import { apiFetch, QUERY_KEYS, queryClient } from '../../../api';
	import type { CreateVoteData } from '../../../api/types';
	import type { FetchedMovieSuggestion } from '../../../api/types/fetch-movie-suggestions';
	import { API_BASE_URL } from '../../../api/urls';
	import { movieNightHub } from '../../../hubs';
	import { appState, getAppUser, hubIsDisconnected } from '../../../store';
	import { VoteType, type MovieNightEvent } from '../../../types';
	import CustomDialog from '../common/CustomDialog.svelte';
	import Badge from '../ui/badge/badge.svelte';
	import Button from '../ui/button/button.svelte';
	import Spinner from '../ui/spinner/spinner.svelte';
	import MovieDetailsContent from './movie-details-content.svelte';

	interface VetoedMovieSuggestionProps {
		movieSuggestionQuery: CreateQueryResult<
			{ movieNightSuggestions: FetchedMovieSuggestion[] },
			Error
		>;
		event: MovieNightEvent;
		suggestion: FetchedMovieSuggestion;
		selectedGroupId: string | undefined;
	}

	let {
		movieSuggestionQuery = $bindable(),
		event,
		suggestion,
		selectedGroupId
	}: VetoedMovieSuggestionProps = $props();
	let user = getAppUser();
	let showMovieDetailsDialog = $state(false);

	const onShowMovieDialogChange = (show: boolean) => {
		showMovieDetailsDialog = show;
	};
	function canVote(event: MovieNightEvent): boolean {
		return !event.isLocked && new Date(event.scheduledAt) > new Date();
	}

	let voteMutation = createMutation<
		{ message: string }, // response type
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
		onSuccess: async () => {
			await queryClient.invalidateQueries({
				queryKey: [QUERY_KEYS.MOVIE_NIGHT_EVENTS + selectedGroupId + 'upcoming']
			});
		}
	}));

	const handleVoteClick = async (vote: VoteType) => {
		if (!user) return toast.error('Please login to proceed', { richColors: true });
		if (!hubIsDisconnected()) {
			await movieNightHub.join(event.id);
		}
		await voteMutation.mutateAsync({
			UserId: user.id,
			VoteType: vote,
			ConnectionId: appState.hubConnection.connectionId || ''
		});
	};
</script>

<CustomDialog
	header={{ title: 'Movie Details' }}
	bind:open={showMovieDetailsDialog}
	onOpenChange={onShowMovieDialogChange}
>
	<MovieDetailsContent movie={suggestion.movie} />
</CustomDialog>
<div
	class="mb-2 rounded-lg border border-destructive/30 bg-destructive/5 p-3 opacity-80 hover:opacity-100"
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
	<div class="mt-2 flex justify-between gap-2">
		<Button size="sm" variant="outline" onclick={() => (showMovieDetailsDialog = true)}>
			<Eye class="mr-2 h-4 w-4" />
			View Movie
		</Button>
		<div>
			{#if suggestion?.userVote == VoteType.Veto}
				<Button
					size="sm"
					variant="outline"
					disabled={!canVote(event) || voteMutation.isPending}
					onclick={handleVoteClick.bind(null, VoteType.Veto)}
				>
					{#if voteMutation.isPending}
						<Spinner />
					{/if}
					<Undo2 class="mr-2 h-4 w-4" />
					Undo veto
				</Button>
			{/if}
		</div>
	</div>
</div>
