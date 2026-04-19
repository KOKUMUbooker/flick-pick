<script lang="ts">
	import { Button } from '$lib/components/ui/button/index.js';
	import { Field, FieldGroup, Label } from '$lib/components/ui/field/index.js';
	import { cn, type WithElementRef } from '$lib/utils.js';
	import StarRating from '@/components/common/star-rating.svelte';
	import Textarea from '@/components/ui/textarea/textarea.svelte';
	import { createMutation } from '@tanstack/svelte-query';
	import { toast } from 'svelte-sonner';
	import type { HTMLFormAttributes } from 'svelte/elements';
	import { apiFetch } from '../../../../api';
	import { UpdateMovieNightEventRatingDataToQueryCache } from '../../../../api/query-cache-crud';
	import type { CreateRatingData, CreateRatingFormData } from '../../../../api/types';
	import { API_BASE_URL } from '../../../../api/urls';
	import { movieNightHub } from '../../../../hubs';
	import { appState, getAppUser, hubIsDisconnected } from '../../../../store';
	import type { DBGroup, MovieNightEvent } from '../../../../types';
	import Spinner from '../../ui/spinner/spinner.svelte';

	interface AddMovieNightRatingProps extends WithElementRef<HTMLFormAttributes> {
		onOpenChange: (open: boolean) => void;
		movieEvent: MovieNightEvent;
		selectedGroup: DBGroup | null;
	}

	let {
		ref = $bindable(null),
		onOpenChange,
		selectedGroup,
		movieEvent,
		class: className,
		...restProps
	}: AddMovieNightRatingProps = $props();
	let formData = $state<CreateRatingFormData>({ Rating: 1, Comment: '' });
	let user = getAppUser();

	let createRatingMutation = createMutation<
		{ message: string; movieEvent: MovieNightEvent }, // response type
		Error, // error type
		CreateRatingData // variables type
	>(() => ({
		mutationFn: async (data) => {
			let url = `${API_BASE_URL}/api/movie-event/${movieEvent.id}?userId=${encodeURIComponent(user?.id || '')}`;
			return apiFetch(url, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(data)
			});
		},
		onSuccess: async (res) => {
			await UpdateMovieNightEventRatingDataToQueryCache(selectedGroup?.id || '', res.movieEvent);
		}
	}));

	async function onSubmit(event: SubmitEvent) {
		event.preventDefault();

		if (!user) {
			return toast.error('Please refresh the page or login', { richColors: true });
		}

		if (!hubIsDisconnected()) {
			await movieNightHub.join(movieEvent.id);
		}

		const res = await createRatingMutation.mutateAsync({
			...formData,
			ConnectionId: appState.hubConnection.connectionId || ''
		});

		toast.success(res.message, { richColors: true });
		onOpenChange(false);
	}

	const id = $props.id();
</script>

<form
	onsubmit={onSubmit}
	class={cn('flex flex-col gap-6', className)}
	bind:this={ref}
	{...restProps}
>
	<FieldGroup>
		<div class="flex flex-col items-center gap-1 text-center">
			<h1 class="text-2xl font-bold">Rate {movieEvent.name}</h1>
		</div>
		<Field>
			<Label>Rating</Label>
			<StarRating bind:rating={formData.Rating} editable size={36} />
		</Field>
		<Field>
			<Label>Comment</Label>
			<Textarea id="Comment" bind:value={formData.Comment} placeholder="" />
		</Field>
		<Field>
			<Button disabled={createRatingMutation.isPending} type="submit">
				{#if createRatingMutation.isPending}
					<Spinner />
				{/if}
				{createRatingMutation.isPending ? 'Rating movie event...' : 'Rate movie event'}
			</Button>
		</Field>
	</FieldGroup>
</form>
