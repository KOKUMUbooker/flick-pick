<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { Tabs, TabsList, TabsTrigger } from '$lib/components/ui/tabs';
	import DesktopHeader from '@/components/groups/desktop-header.svelte';
	import GroupsMobileNav from '@/components/groups/groups-mobile-nav.svelte';
	import GroupsSideBar from '@/components/groups/groups-side-bar.svelte';
	import MainContentArea from '@/components/groups/main-content-area.svelte';
	import MembersTabContent from '@/components/groups/members-tab-content.svelte';
	import PastEventContentTab from '@/components/groups/past-event-content-tab.svelte';
	import StatsTabContent from '@/components/groups/stats-tab-content.svelte';
	import UpcomingEventsTabContent from '@/components/groups/upcoming-events-tab-content.svelte';
	import { Calendar, Clock, Menu, Plus, Star, Users } from '@lucide/svelte';
	import { onMount } from 'svelte';
	import { loadGroupData, mockGroups } from '../../../data';
	import type { Group, MovieNightEvent } from '../../../types';

	// State management
	let activeGroup = $state<Group | null>(null);
	let groups = $state<Group[]>([]);
	let activeTab = $state('upcoming');
	let sidebarOpen = $state(false); // Controls mobile sidebar visibility
	let searchQuery = $state('');

	// Track selected event for chat view
	let selectedEvent = $state<MovieNightEvent | null>(null);
	let showEventChat = $state(false);

	let events = $state<{
		upcoming: MovieNightEvent[];
		past: MovieNightEvent[];
	}>({
		upcoming: [],
		past: []
	});

	let stats = $state({
		moviesWatched: 0,
		totalVotes: 0,
		averageRating: 0,
		streak: 0
	});

	// User's vote state
	let userVotes = $state(new Map<number, VoteType>());

	// Filtered groups based on search
	const filteredGroups = $derived(
		searchQuery
			? groups.filter(
					(g) =>
						g.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
						g.description.toLowerCase().includes(searchQuery.toLowerCase())
				)
			: groups
	);

	// Computed property for event chat visibility
	const activeEventChat = $derived(showEventChat && selectedEvent ? selectedEvent : null);

	// Initialize
	onMount(() => {
		groups = mockGroups.map((g) => ({ ...g, isActive: false }));

		if (groups.length > 0) {
			setActiveGroup(groups[0]);
		}
	});

	function setActiveGroup(group: Group) {
		groups = groups.map((g) => ({
			...g,
			isActive: g.id === group.id
		}));

		activeGroup = { ...group, isActive: true };
		showEventChat = false;
		selectedEvent = null;

		// Close sidebar on mobile after selection
		sidebarOpen = false;

		loadGroupData({
			groupId: group.id,
			eventCb: (data) => (events = data),
			statsCb: (data) => (stats = data)
		});
	}

	function openEventChat(event: MovieNightEvent) {
		selectedEvent = event;
		showEventChat = true;
	}

	function closeEventChat() {
		showEventChat = false;
		selectedEvent = null;
	}

	function createNewEvent() {
		if (!activeGroup) return;
		console.log('Create event for group:', activeGroup.id);
	}

	function inviteToGroup() {
		if (!activeGroup) return;
		console.log('Invite to group:', activeGroup.id);
	}

	function createNewGroup() {
		console.log('Create new group');
	}

	function toggleSidebar() {
		sidebarOpen = !sidebarOpen;
	}

	type VoteType = 'Upvote' | 'Downvote' | 'Veto';

	function handleVote(suggestionId: number, voteType: VoteType) {
		userVotes.set(suggestionId, voteType);
		console.log(`Voted ${voteType} on suggestion ${suggestionId}`);
	}

	function sendEventChatMessage(eventId: number, message: string) {
		console.log(`Sending message to event ${eventId}: ${message}`);
	}

	function canVote(event: MovieNightEvent): boolean {
		return !event.isLocked && new Date(event.scheduledAt) > new Date();
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
		{createNewGroup}
		{filteredGroups}
		{searchQuery}
		{setActiveGroup}
		{toggleSidebar}
		{sidebarOpen}
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
					{activeGroup ? activeGroup.name : 'FlickPick'}
				</h1>
			</div>
		</header>

		{#if activeGroup}
			<!-- Desktop Group Header -->
			<DesktopHeader {activeGroup} {createNewEvent} {inviteToGroup} />

			<!-- Main Content Area -->
			<div class="p-4 md:p-6">
				<!-- Conditionally show event chat or group tabs -->
				{#if showEventChat && selectedEvent}
					<!-- Event-Specific Chat View -->
					<MainContentArea {closeEventChat} {showEventChat} {selectedEvent} {getEventStatus} />
				{:else}
					<!-- Tabs -->
					<Tabs value={activeTab} onValueChange={(value) => (activeTab = value)} class="mb-8">
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
							<TabsTrigger value="stats">
								<Star class="mr-2 h-4 w-4" />
								Stats
							</TabsTrigger>
						</TabsList>

						<!-- Upcoming Events Tab -->
						<UpcomingEventsTabContent
							{events}
							{userVotes}
							{handleVote}
							{openEventChat}
							{createNewEvent}
						/>

						<!-- Past Events Tab -->
						<PastEventContentTab {events} {openEventChat} {createNewEvent} />

						<!-- Members Tab -->
						<MembersTabContent {activeGroup} {inviteToGroup} />

						<!-- Stats Tab -->
						<StatsTabContent {events} {stats} />
					</Tabs>
				{/if}
			</div>
		{:else}
			<!-- No Group Selected State -->
			<div class="flex h-full items-center justify-center p-8">
				<div class="max-w-md text-center">
					<Users class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
					<h2 class="mb-2 text-2xl font-semibold">Welcome to FlickPick</h2>
					<p class="mb-6 text-muted-foreground">
						Select a group from the sidebar or create your first group to start planning movie
						nights.
					</p>
					<div class="flex flex-col gap-3 sm:flex-row sm:justify-center">
						<Button onclick={createNewGroup}>
							<Plus class="mr-2 h-4 w-4" />
							Create Your First Group
						</Button>
						<Button variant="outline">
							<Users class="mr-2 h-4 w-4" />
							Join Existing Group
						</Button>
					</div>
				</div>
			</div>
		{/if}

		<!-- Mobile Bottom Navigation -->
		<GroupsMobileNav {activeTab} />
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
