<script lang="ts">
	import { Button } from '$lib/components/ui/button/index.js';
	import Calendar from '$lib/components/ui/calendar/calendar.svelte';
	import { Field, FieldGroup, Label } from '$lib/components/ui/field/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import * as Popover from '$lib/components/ui/popover/index.js';
	import { cn, type WithElementRef } from '$lib/utils.js';
	import {
		addMovieNightSchema,
		getDefaultMovieEventFormData
	} from '@/forms/add-movie-night-schema';
	import { getLocalTimeZone } from '@internationalized/date';
	import ChevronDownIcon from '@lucide/svelte/icons/chevron-down';
	import { createMutation } from '@tanstack/svelte-query';
	import { toast } from 'svelte-sonner';
	import type { HTMLFormAttributes } from 'svelte/elements';
	import { apiFetch, QUERY_KEYS, queryClient } from '../../../../api';
	import { UpdateMovieNightEventToQueryCache } from '../../../../api/query-cache-crud';
	import type { AddMovieNightEventData, AddMovieNightEventFormData } from '../../../../api/types';
	import { API_BASE_URL } from '../../../../api/urls';
	import { movieNightHub } from '../../../../hubs';
	import { appState, getAppUser, hubIsDisconnected } from '../../../../store';
	import type { DBGroup, MovieNightEvent } from '../../../../types';
	import { combineDateTime } from '../../../../utils/combine-date-time';
	import HelperText from '../../common/HelperText.svelte';
	import Spinner from '../../ui/spinner/spinner.svelte';

	interface AddMovieNightProps extends WithElementRef<HTMLFormAttributes> {
		onOpenChange: (open: boolean) => void;
		selectedGroup: DBGroup | null;
		defaultMovieEvent?: MovieNightEvent;
	}

	let {
		ref = $bindable(null),
		onOpenChange,
		selectedGroup,
		defaultMovieEvent = $bindable(),
		class: className,
		...restProps
	}: AddMovieNightProps = $props();
	let defaultFormValues = getDefaultMovieEventFormData(defaultMovieEvent);
	let formData = $state<AddMovieNightEventFormData>(defaultFormValues);
	let errors: Record<string, string> = $state({});
	let touched: Record<string, boolean> = $state({});
	let open = $state(false);

	let createMovieNightMutation = createMutation<
		{ message: string; movieEvent: MovieNightEvent }, // response type
		Error, // error type
		AddMovieNightEventData // variables type
	>(() => ({
		mutationFn: async (data) => {
			let url = `${API_BASE_URL}/api/groups/${selectedGroup?.id}/movie-night`;
			if (defaultMovieEvent) url += `/edit?userId=${encodeURIComponent(user?.id || '')}`;
			return apiFetch(url, {
				method: defaultMovieEvent ? 'PATCH' : 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(data)
			});
		},
		onSuccess: async (res) => {
			if (defaultMovieEvent) {
				await UpdateMovieNightEventToQueryCache(selectedGroup?.id || '', res.movieEvent);
			} else {
				await queryClient.invalidateQueries({
					queryKey: [QUERY_KEYS.MOVIE_NIGHT_EVENTS + selectedGroup?.id + 'upcoming']
				});
			}
		}
	}));
	const user = getAppUser();

	function validateField(field: keyof typeof formData) {
		const partial = addMovieNightSchema.pick({ [field]: true } as object);
		const result = partial.safeParse({ [field]: formData[field] });

		if (!result.success) {
			errors[field] = result.error.issues[0].message;
		} else {
			delete errors[field];
		}
	}

	function validateSheduleDate() {
		const field: keyof typeof formData = 'ScheduledAt';
		const scheduledAt = combineDateTime(formData.ScheduledDate, formData.ScheduledTime);
		if (scheduledAt) formData.ScheduledAt = scheduledAt;
		const partial = addMovieNightSchema.pick({ [field]: true } as object);
		const result = partial.safeParse({ [field]: scheduledAt });

		if (!result.success) {
			errors[field] = result.error.issues[0].message;
		} else {
			delete errors[field];
		}
	}

	function onBlur(field: keyof typeof formData) {
		touched[field] = true;
		validateField(field);
	}

	async function onSubmit(event: SubmitEvent) {
		event.preventDefault();

		if (!user) {
			return toast.error('Please refresh the page or login', { richColors: true });
		}

		// mark everything touched
		Object.keys(formData).forEach((key) => (touched[key] = true));

		const result = addMovieNightSchema.safeParse(formData);

		if (!result.success) {
			errors = Object.fromEntries(
				result.error.issues.map((issue) => [issue.path[0] as string, issue.message])
			);
			return;
		}

		if (!formData.ScheduledDate) {
			return toast.error('Please provide a scheduled at date', { richColors: true });
		}

		const sheduledAt = combineDateTime(formData.ScheduledDate, formData.ScheduledTime);
		if (!sheduledAt) {
			return toast.error('Please provide a scheduled at time', { richColors: true });
		}
		if (defaultMovieEvent && !hubIsDisconnected()) {
			await movieNightHub.join(defaultMovieEvent.id);
		}

		const res = await createMovieNightMutation.mutateAsync({
			...formData,
			ScheduledAt: sheduledAt.toISOString(),
			CreatedById: user.id,
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
			<h1 class="text-2xl font-bold">{formData.Id ? 'Update' : 'Create'} movie event</h1>
		</div>
		<Field>
			<Label for="Name">Name</Label>
			<Input
				id="Name"
				bind:value={formData.Name}
				class={`${errors?.['Name'] ? 'border-destructive' : ''}`}
				placeholder="Comedy special"
				onblur={onBlur.bind(null, 'Name')}
				oninput={() => touched.Name && validateField('Name')}
			/>
			<HelperText variant="error" message={errors?.['Name'] || ''} show={!!errors?.['Name']}
			></HelperText>
		</Field>
		<Field>
			<Label for="Description">Description <span class="text-xs">(optional)</span></Label>
			<Input
				id="Description"
				bind:value={formData.Description}
				class={`${errors?.['Description'] ? 'border-destructive' : ''}`}
				placeholder=""
				onblur={onBlur.bind(null, 'Description')}
				oninput={() => touched.Description && validateField('Description')}
			/>
			<HelperText
				variant="error"
				message={errors?.['Description'] || ''}
				show={!!errors?.['Description']}
			></HelperText>
		</Field>

		<Field>
			<Label for="Description">Scheduled At</Label>

			<div class="flex gap-4">
				<div class="flex flex-col gap-3">
					<Label for="{id}-date" class="px-1">Date</Label>
					<Popover.Root bind:open>
						<Popover.Trigger id="{id}-date">
							{#snippet child({ props })}
								<Button {...props} variant="outline" class="w-32 justify-between font-normal">
									{formData.ScheduledDate
										? formData.ScheduledDate.toDate(getLocalTimeZone()).toLocaleDateString()
										: 'Select date'}
									<ChevronDownIcon />
								</Button>
							{/snippet}
						</Popover.Trigger>
						<Popover.Content class="w-auto overflow-hidden p-0" align="start">
							<Calendar
								type="single"
								bind:value={formData.ScheduledDate}
								onValueChange={() => {
									open = false;
									validateSheduleDate();
								}}
								captionLayout="dropdown"
							/>
						</Popover.Content>
					</Popover.Root>
				</div>
				<div class="flex flex-col gap-3">
					<Label for="{id}-time" class="px-1">Time</Label>
					<Input
						type="time"
						id="{id}-time"
						step="1"
						onblur={validateSheduleDate}
						bind:value={formData.ScheduledTime}
						oninput={() => validateSheduleDate()}
						class="appearance-none bg-background [&::-webkit-calendar-picker-indicator]:hidden [&::-webkit-calendar-picker-indicator]:appearance-none"
					/>
				</div>
			</div>

			<HelperText
				variant="error"
				message={errors?.['ScheduledAt'] || ''}
				show={!!errors?.['ScheduledAt']}
			></HelperText>
		</Field>

		<Field>
			<Button disabled={createMovieNightMutation.isPending} type="submit">
				{#if createMovieNightMutation.isPending}
					<Spinner />
				{/if}
				{createMovieNightMutation.isPending
					? formData.Id
						? 'Updating movie event...'
						: 'Creating movie event...'
					: formData.Id
						? 'Update movie event'
						: 'Create movie event'}
			</Button>
		</Field>
	</FieldGroup>
</form>
