<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { Tabs, TabsList, TabsTrigger } from '$lib/components/ui/tabs';
	import CustomDialog from '@/components/common/CustomDialog.svelte';
	import AddGroupForm from '@/components/dashboard/forms/add-group-form.svelte';
	import AddMovieNightForm from '@/components/dashboard/forms/add-movie-night-form.svelte';
	import SearchJoinGroupsFlow from '@/components/dashboard/search-join-groups-flow.svelte';
	import SearchUsersToInviteFlow from '@/components/dashboard/search-users-to-invite-flow.svelte';
	import ChatContentArea from '@/components/groups/chat-content-area.svelte';
	import DesktopHeader from '@/components/groups/desktop-header.svelte';
	import GroupActionsMobile from '@/components/groups/group-actions-mobile.svelte';
	import GroupsMobileNav from '@/components/groups/groups-mobile-nav.svelte';
	import GroupsSideBar from '@/components/groups/groups-side-bar.svelte';
	import InvitesTabContent from '@/components/groups/invites-tab-content.svelte';
	import MembersTabContent from '@/components/groups/members-tab-content.svelte';
	import PastEventContentTab from '@/components/groups/past-event-content-tab.svelte';
	import UpcomingEventsTabContent from '@/components/groups/upcoming-events-tab-content.svelte';
	import TabsContent from '@/components/ui/tabs/tabs-content.svelte';
	import { getIsMobile } from '@/hooks/is-mobile.svelte';
	import { Calendar, Clock, MailPlus, Menu, Plus, UserPlus, Users } from '@lucide/svelte';
	import { createMutation, createQuery } from '@tanstack/svelte-query';
	import { onMount } from 'svelte';
	import { toast } from 'svelte-sonner';
	import { apiFetch, QUERY_KEYS, queryClient } from '../../api';
	import { API_BASE_URL } from '../../api/urls';
	import { appState, getAppUser, hubIsDisconnected } from '../../store';
	import type { DBGroup, MovieNightEvent } from '../../types';

	// State management
	let activeTab = $state('upcoming');
	let sidebarOpen = $state(false); // Controls mobile sidebar visibility
	let searchQuery = $state('');

	let showPendingInvitesDialog = $state(false);
	let showJoinGroupsDialog = $state(false);
	let showSendInviteDialog = $state(false);
	let showDeleteWarnDialog = $state(false);

	// Track selected event for chat view
	let selectedGroup = $state<DBGroup | null>(null);
	let selectedEvent = $state<MovieNightEvent | null>(null);
	let showEventChat = $state(false);

	let user = getAppUser();

	let showAddGroupDialog = $state(false);
	const onShowAddGroupDialogOpenChange = (open: boolean) => {
		showAddGroupDialog = open;
	};

	let showAddMovieNightDialog = $state(false);
	const onShowMovieNightDialogOpenChange = (open: boolean) => {
		showAddMovieNightDialog = open;
	};

	let shouldFetchGroups = $state(false);
	let _groupsQuery = createQuery<
		null, // variables type
		Error, // error type
		{ groups: DBGroup[] } // response type
	>(() => ({
		queryKey: [QUERY_KEYS.GROUPS + user?.id || ''],
		queryFn: async () => {
			return apiFetch(`${API_BASE_URL}/api/groups?userId=${user?.id}`, {
				method: 'GET',
				headers: { 'Content-Type': 'application/json' }
			});
		},
		enabled: shouldFetchGroups
	}));

	let groupsQuery = $state(_groupsQuery);

	// Initialize
	onMount(async () => {
		try {
			if (!user) return;

			// 1. Fetch groups
			shouldFetchGroups = true;
			const groupsRes = await groupsQuery.refetch();
			if (groupsRes.error || !groupsRes.data) return;

			if (groupsRes.data.groups.length > 0) {
				selectedGroup = groupsRes.data.groups[0];
			}

			// 2. Connect to signalR hub
			if (hubIsDisconnected()) await appState.hubConnection.start();
		} catch (err) {
			console.error('error : ', err);
		}
	});

	const filteredGroups = $derived(
		searchQuery && groupsQuery?.data
			? groupsQuery.data.groups.filter(
					(g) =>
						g.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
						g?.description?.toLowerCase().includes(searchQuery.toLowerCase())
				)
			: groupsQuery?.data?.groups || []
	);

	function openEventChat(event: MovieNightEvent) {
		selectedEvent = event;
		showEventChat = true;
	}

	function closeEventChat() {
		showEventChat = false;
		selectedEvent = null;
	}

	function createNewEvent() {
		if (!selectedGroup) {
			return toast.error('No group has been selected, please select one from the sidebar');
		}
		showAddMovieNightDialog = true;
	}

	function createNewGroup() {
		showAddGroupDialog = true;
	}

	function toggleSidebar() {
		sidebarOpen = !sidebarOpen;
	}

	let groupDeleteMutation = createMutation<
		{ message: string }, // response type
		Error, // error type
		void // variables type
	>(() => ({
		mutationFn: async (data) => {
			return apiFetch(
				`${API_BASE_URL}/api/groups/${selectedGroup?.id || ''}/del?userId=${encodeURIComponent(user?.id || '')}`,
				{
					method: 'DELETE',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify(data)
				}
			);
		},
		onSuccess: async () => {
			await queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.GROUPS + user?.id || ''] });
			selectedGroup = null;
		}
	}));

	function onShowDelWarningOnchange(show: boolean) {
		showDeleteWarnDialog = show;
	}

	async function onProceedGroupDelete() {
		const res = await groupDeleteMutation.mutateAsync();
		toast.success(res.message, { richColors: true });
		onShowDelWarningOnchange(false);
	}

	function getEventStatus(event: MovieNightEvent): {
		label: string;
		variant: 'default' | 'outline' | 'destructive' | 'secondary';
	} {
		if (event.isLocked) return { label: 'Locked', variant: 'secondary' };
		if (new Date(event.scheduledAt) < new Date()) return { label: 'Ended', variant: 'destructive' };
		return { label: 'Voting Active', variant: 'default' };
	}
</script>

{#if showSendInviteDialog}
	<SearchUsersToInviteFlow
		{selectedGroup}
		onCancel={() => (showSendInviteDialog = false)}
		onGroupSelectConfirm={() => (showSendInviteDialog = false)}
	/>
{/if}

{#if showJoinGroupsDialog}
	<SearchJoinGroupsFlow
		onCancel={() => (showJoinGroupsDialog = false)}
		onGroupSelectConfirm={() => (showJoinGroupsDialog = false)}
	/>
{/if}

<CustomDialog
	bind:open={showPendingInvitesDialog}
	width="4xl"
	onOpenChange={() => (showPendingInvitesDialog = false)}
>
	<div class="mt-6">
		<InvitesTabContent selectedGroup={null} allowAutomaticFetch={true} />
	</div>
</CustomDialog>

<CustomDialog bind:open={showAddGroupDialog} onOpenChange={onShowAddGroupDialogOpenChange}>
	<AddGroupForm
		onOpenChange={onShowAddGroupDialogOpenChange}
		defaultValues={{
			Id: selectedGroup?.id || '',
			Description: selectedGroup?.description || '',
			Name: selectedGroup?.name || ''
		}}
	/>
</CustomDialog>

<CustomDialog bind:open={showAddMovieNightDialog} onOpenChange={onShowMovieNightDialogOpenChange}>
	<AddMovieNightForm {selectedGroup} onOpenChange={onShowMovieNightDialogOpenChange} />
</CustomDialog>

<CustomDialog
	header={{ title: 'Delete Group' }}
	bind:open={showDeleteWarnDialog}
	onOpenChange={onShowDelWarningOnchange}
	isLoading={groupDeleteMutation.isPending}
	actions={{ onProceed: onProceedGroupDelete }}
>
	<p>
		Are you sure you want to delete this group and all of its data? This action is irreversible so
		proceed with caution.
	</p>
</CustomDialog>

<div class="flex min-h-screen bg-background">
	<!-- Mobile Sidebar Overlay -->
	{#if sidebarOpen}
		<button
			class="fixed inset-0 z-40 bg-black/50"
			aria-label="Sidbar button"
			onclick={toggleSidebar}
		></button>
	{/if}

	<!-- Sidebar -->
	<GroupsSideBar
		{sidebarOpen}
		{toggleSidebar}
		{filteredGroups}
		isFetching={groupsQuery.isFetching || groupsQuery.isPending}
		bind:searchQuery
		bind:selectedGroup
	/>

	<!-- Main Content -->
	<main class="flex-1 md:ml-0">
		<!-- Mobile Header -->
		<header
			class="sticky top-0 z-30 border-b border-border bg-background/95 backdrop-blur supports-backdrop-filter:bg-background/60 md:hidden"
		>
			<div class="flex h-16 items-center px-4">
				<Button size="sm" variant="ghost" onclick={toggleSidebar} class="mr-2">
					<Menu class="h-5 w-5" />
				</Button>
				<h1 class="text-lg font-semibold">
					{selectedGroup ? selectedGroup.name : 'WatchHive'}
				</h1>
			</div>
		</header>

		{#if selectedGroup}
			<!-- Desktop Group Header -->
			<DesktopHeader
				{selectedGroup}
				{createNewEvent}
				bind:showSendInviteDialog
				bind:showJoinGroupsDialog
				bind:showAddGroupDialog
				bind:showDeleteWarnDialog
			/>

			<!-- Main Content Area -->
			<div class="p-4 md:p-6">
				<!-- Conditionally show event chat or group tabs -->
				{#if showEventChat && selectedEvent}
					<ChatContentArea {closeEventChat} {selectedEvent} {getEventStatus} {selectedGroup} />
				{:else}
					<!-- Tabs -->
					<Tabs value={activeTab} onValueChange={(value) => (activeTab = value)} class="mb-8">
						{#if !getIsMobile()}
							<TabsList class="grid w-full grid-cols-4 md:w-auto">
								<TabsTrigger value="upcoming">
									<Calendar class="mr-2 h-4 w-4" />
									Upcoming
								</TabsTrigger>
								<TabsTrigger value="past">
									<Clock class="mr-2 h-4 w-4" />
									Past Events
								</TabsTrigger>
								<TabsTrigger value="members">
									<Users class="mr-2 h-4 w-4" />
									Members
								</TabsTrigger>
								<TabsTrigger value="invites">
									<UserPlus class="mr-2 h-4 w-4" />
									Invites
								</TabsTrigger>
							</TabsList>
						{:else}
							<GroupActionsMobile
								{selectedGroup}
								{createNewEvent}
								bind:showAddGroupDialog
								bind:showSendInviteDialog
								bind:showJoinGroupsDialog
								bind:showDeleteWarnDialog
							/>
						{/if}

						<!-- Upcoming Events Tab -->
						<UpcomingEventsTabContent {selectedGroup} {openEventChat} {createNewEvent} />

						<!-- Past Events Tab -->
						<PastEventContentTab {openEventChat} {selectedGroup} />

						<!-- Members Tab -->
						<MembersTabContent {selectedGroup} />

						<!-- Invites Tab -->
						<TabsContent value="invites" class="mt-6">
							<InvitesTabContent {selectedGroup} />
						</TabsContent>
					</Tabs>
				{/if}
			</div>
		{:else}
			<!-- No Group Selected State -->
			<div class="flex h-full items-center justify-center p-8">
				<div class="max-w-md text-center">
					<Users class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
					<h2 class="mb-2 text-2xl font-semibold">Welcome to WatchHive</h2>
					<p class="mb-6 text-muted-foreground">
						{user
							? 'Select a group from the sidebar or create your first group to start planning movie nights.'
							: "Oops, it seems you're logged out, please proceed to login"}
					</p>
					{#if user}
						<div class="flex flex-col gap-3 sm:flex-row sm:justify-center">
							<Button onclick={createNewGroup}>
								<Plus class="mr-2 h-4 w-4" />
								Create Your First Group
							</Button>

							<Button variant="secondary" onclick={() => (showJoinGroupsDialog = true)}>
								<Users class="mr-2 h-4 w-4" />
								Join Existing Group
							</Button>
							<Button variant="outline" onclick={() => (showPendingInvitesDialog = true)}>
								<MailPlus class="mr-2 h-4 w-4" />
								Check pending invites
							</Button>
						</div>
					{/if}
				</div>
			</div>
		{/if}

		{#if !showEventChat}
			<!-- Mobile Bottom Navigation -->
			<GroupsMobileNav bind:activeTab />
		{/if}
	</main>
</div>

<style>
	/* Adjust padding for mobile bottom nav */
	main > div:last-of-type {
		padding-bottom: 5rem;
	}

	/* Mobile adjustments */
	@media (max-width: 768px) {
		main {
			padding-bottom: 4rem;
		}
	}
</style>
