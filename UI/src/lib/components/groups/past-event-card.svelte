<script lang="ts">
	import { EditIcon, MessageSquare, Star, Trash } from '@lucide/svelte';
	import type { DBGroup, MovieNightEvent } from '../../../types';
	import Button from '../ui/button/button.svelte';
	import { Card, CardContent } from '../ui/card';
	import CardHeader from '../ui/card/card-header.svelte';
	import CardTitle from '../ui/card/card-title.svelte';
	import Separator from '../ui/separator/separator.svelte';
	import CustomDialog from '../common/CustomDialog.svelte';
	import AddMovieNightForm from '../dashboard/forms/add-movie-night-form.svelte';
	import { createMutation } from '@tanstack/svelte-query';
	import { apiFetch } from '../../../api';
	import { DeleteMovieNightEventFromQueryCache } from '../../../api/query-cache-crud';
	import type { DeleteMovieEventData } from '../../../api/types';
	import { API_BASE_URL } from '../../../api/urls';
	import { toast } from 'svelte-sonner';
	import { movieNightHub } from '../../../hubs';
	import { hubIsDisconnected, appState, getAppUser } from '../../../store';
	import DialogPortal from '../ui/dialog/dialog-portal.svelte';

	interface PastEventCardProps {
		selectedGroup: DBGroup | null;
		event: MovieNightEvent;
		openEventChat: (event: MovieNightEvent) => void;
	}

	let { selectedGroup, event = $bindable(), openEventChat }: PastEventCardProps = $props();
	let user = getAppUser();

	let showAddMovieNightDialog = $state(false);
	let showDeleteWarnDialog = $state(false);

	function onShowMovieNightDialogOpenChange(show: boolean) {
		showAddMovieNightDialog = show;
	}
	let movieEventDeleteMutation = createMutation<
		{ message: string; movieEvent: { id: string } }, // response type
		Error, // error type
		DeleteMovieEventData // variables type
	>(() => ({
		mutationFn: async (data) => {
			return apiFetch(
				`${API_BASE_URL}/api/groups/${selectedGroup?.id || ''}/movie-event/${event.id}`,
				{
					method: 'DELETE',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify(data)
				}
			);
		},
		onSuccess: async (res) => {
			await DeleteMovieNightEventFromQueryCache(selectedGroup?.id || '', res.movieEvent.id);
			// await queryClient.invalidateQueries({
			// 	queryKey: [QUERY_KEYS.MOVIE_NIGHT_EVENTS + selectedGroup?.id + 'upcoming']
			// });
		}
	}));
	const onShowDelWarningOnchange = (show: boolean) => {
		showDeleteWarnDialog = show;
	};

	const onProceedMovieEventDelete = async () => {
		if (!user) return toast.error('Please login to proceed', { richColors: true });
		if (!hubIsDisconnected()) {
			await movieNightHub.join(event.id);
		}

		const res = await movieEventDeleteMutation.mutateAsync({
			Initiator: user.id,
			ConnectionId: appState.hubConnection.connectionId || ''
		});
		toast.success(res.message, { richColors: true });
		onShowDelWarningOnchange(false);
	};
</script>

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
<Card class="group transition-all hover:shadow-lg">
	<CardHeader class="my-0 flex flex-wrap items-center justify-between py-0 md:flex-row">
		<CardTitle class="my-0 py-0">{event.name}</CardTitle>
		{#if selectedGroup?.isUserAdmin}
			<div class="flex flex-row items-center gap-2">
				<Button variant="outline" onclick={() => (showAddMovieNightDialog = true)}>
					<EditIcon />
				</Button>
				<Button variant="destructive" onclick={() => (showDeleteWarnDialog = true)}>
					<Trash />
				</Button>
			</div>
		{/if}
	</CardHeader>
	<Separator />
	<CardContent class="p-6">
		<div class="mb-4 flex items-start justify-between">
			<div class="flex items-center gap-3">
				<img
					src={`https://image.tmdb.org/t/p/w92${event.selectedMovie?.posterPath}`}
					alt={event.selectedMovie?.title}
					class="h-16 w-12 rounded object-cover"
				/>
				<div>
					<h3 class="text-lg font-semibold">{event.selectedMovie?.title}</h3>
					<p class="text-sm text-muted-foreground">
						{new Date(event.scheduledAt).toLocaleDateString('en-US', {
							month: 'short',
							day: 'numeric',
							year: 'numeric'
						})}
					</p>
				</div>
			</div>
		</div>

		<div class="space-y-3">
			<div class="flex items-center justify-between">
				<span class="text-sm text-muted-foreground">Group Rating</span>

				<div class="flex items-center gap-1">
					{#if event.totalRatings > 0}
						{#each [1, 2, 3, 4, 5] as i (i)}
							<Star
								class={`h-4 w-4 ${
									i < event.averageRating ? 'fill-primary text-primary' : 'text-muted-foreground'
								}`}
							/>
						{/each}

						<span class="ml-1 text-sm font-medium">
							{event.averageRating.toFixed(1)} ({event.totalRatings})
						</span>
					{:else}
						{#each [1, 2, 3, 4, 5] as i (i)}
							<Star class="h-4 w-4 text-muted-foreground" />
						{/each}
						<span class="ml-1 text-sm text-muted-foreground"> No ratings </span>
					{/if}
				</div>
			</div>

			<div class="flex items-center gap-2 pt-2">
				<Button variant="outline" size="sm" class="flex-1" onclick={() => openEventChat(event)}>
					<MessageSquare class="mr-2 h-4 w-4" />
					Chat
				</Button>
				<Button variant="outline" size="sm" class="flex-1">
					<Star class="mr-2 h-4 w-4" />
					Rate
				</Button>
			</div>
		</div>
	</CardContent>
</Card>
