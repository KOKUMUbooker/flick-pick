<script lang="ts">
	import { Eye, ThumbsDown, ThumbsUp, Trash, XCircle } from "@lucide/svelte";
	import { createMutation } from "@tanstack/svelte-query";
	import { toast } from "svelte-sonner";
	import { apiFetch, QUERY_KEYS, queryClient } from "../../../api";
	import type { FetchedMovieSuggestion } from "../../../api/types/fetch-movie-suggestions";
	import { API_BASE_URL } from "../../../api/urls";
	import { getAppUser } from "../../../store";
	import { VoteType, type MovieNightEvent } from "../../../types";
	import CustomDialog from "../common/CustomDialog.svelte";
	import Button from "../ui/button/button.svelte";
	import MovieDetailsContent from "./movie-details-content.svelte";

    interface MovieSuggestionItem {
        selectedGroupId : string | undefined;
 		event: MovieNightEvent;
        suggestion: FetchedMovieSuggestion
	}

	let { suggestion, event = $bindable() ,selectedGroupId}: MovieSuggestionItem = $props();
	let user = getAppUser();
    let showDeleteWarnDialog = $state(false);
    let showMovieDetailsDialog = $state(false);

    const onShowMovieDialogChange = (show:boolean) => {
        showMovieDetailsDialog = show
    }

    function canVote(event: MovieNightEvent): boolean {
		return !event.isLocked && new Date(event.scheduledAt) > new Date();
	}
    // "movie-events/{movieEventId}/suggestion/{suggestionId}")
    let suggestionDeleteMutation = createMutation<
		{message : string}, // response type
		Error, // error type
		{Initiator : string} // variables type
	>(() => ({
		mutationFn: async (data) => {
			return apiFetch(`${API_BASE_URL}/api/movie-events/${event.id}/suggestion/${suggestion.id}`, {
				method: 'DELETE',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(data)
			});
		},
		onSuccess: async () => {
			await queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.MOVIE_NIGHT_EVENTS + selectedGroupId + 'upcoming'] });
 		}
	}));

    const onShowDelWarningOnchange = (show:boolean) =>{
        showDeleteWarnDialog = show
    }

    const onProceedSuggestionDelete = async () => {
        if (!user) return toast.error("Please login to proceed",{richColors:true});

        const res = await suggestionDeleteMutation.mutateAsync({Initiator : user.id});
        toast.success(res.message,{richColors:true})
        onShowDelWarningOnchange(false)
    }
    
</script>


<CustomDialog 
	header={{title:"Movie Details"}} 
	bind:open={showMovieDetailsDialog} 
	onOpenChange={onShowMovieDialogChange}
 >
  <MovieDetailsContent movie={suggestion.movie}/>
</CustomDialog>
<CustomDialog 
	header={{title:"Delete movie suggestion"}} 
	bind:open={showDeleteWarnDialog} 
	onOpenChange={onShowDelWarningOnchange}
	isLoading={suggestionDeleteMutation.isPending}
	actions={{onProceed:onProceedSuggestionDelete}}
>
	<p>Are you sure you want to delete this movie suggestion and all of its data? This action is irreversible so proceed with caution.</p>
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
        <div class="flex items-center justify-between">
            <div class="flex-1">
                <Button size="sm" variant="outline" onclick={() => showMovieDetailsDialog = true}>
                    <Eye class="mr-2 h-4 w-4" />
                    View Movie
                </Button>
            </div>
            <div class="flex gap-2 flex-3">
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
        </div>
    {:else}
        <div class="flex justify-between gap-2">
            <Button size="sm" variant="outline" onclick={() => showMovieDetailsDialog= true}>
                <Eye class="mr-2 h-4 w-4" />
                View Movie
            </Button>
            <Button
                size="sm"
                variant="destructive"
                disabled={!canVote(event)}
                onclick={() => showDeleteWarnDialog = true}
            >
                <Trash class="mr-2 h-4 w-4" />
                Delete suggestion
            </Button>
        </div>
    {/if}
</div>