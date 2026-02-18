<script lang="ts">
	import { Avatar, AvatarFallback } from '$lib/components/ui/avatar';
	import { Badge } from '$lib/components/ui/badge';
	import { Button } from '$lib/components/ui/button';
	import {
		Card,
		CardContent,
		CardDescription,
		CardFooter,
		CardHeader,
		CardTitle
	} from '$lib/components/ui/card';
	import {
		DropdownMenu,
		DropdownMenuContent,
		DropdownMenuItem,
		DropdownMenuSeparator,
		DropdownMenuTrigger
	} from '$lib/components/ui/dropdown-menu';
	import { Separator } from '$lib/components/ui/separator';
	import { Tabs, TabsContent, TabsList, TabsTrigger } from '$lib/components/ui/tabs';
	import GroupsSideBar from '@/components/groups/groups-side-bar.svelte';
	import {
		Bell,
		Calendar,
		ChevronLeft,
		Clock,
		Edit,
		Film,
		Link,
		Lock,
		Mail,
		Menu,
		MessageCircle,
		MessageSquare,
		MoreVertical,
		Plus,
		Settings,
		Share2,
		Star,
		ThumbsDown,
		ThumbsUp,
		Trash2,
		Users,
		XCircle
	} from '@lucide/svelte';
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
			<header
				class="sticky top-0 z-30 hidden border-b border-border bg-background/95 backdrop-blur supports-backdrop-filter:bg-background/60 md:block"
			>
				<div class="flex h-16 items-center justify-between px-6">
					<div>
						<h1 class="text-xl font-semibold">{activeGroup.name}</h1>
						<p class="text-sm text-muted-foreground">
							{activeGroup.members.length} members • {activeGroup.description}
						</p>
					</div>

					<div class="flex items-center gap-2">
						<Button size="sm" variant="outline" onclick={inviteToGroup}>
							<Users class="mr-2 h-4 w-4" />
							Invite
						</Button>
						<Button size="sm" onclick={createNewEvent}>
							<Plus class="mr-2 h-4 w-4" />
							New Event
						</Button>
						<DropdownMenu>
							<DropdownMenuTrigger>
								<Button size="sm" variant="ghost">
									<MoreVertical class="h-4 w-4" />
								</Button>
							</DropdownMenuTrigger>
							<DropdownMenuContent align="end">
								<DropdownMenuItem onclick={() => console.log('Settings')}>
									<Settings class="mr-2 h-4 w-4" />
									Group Settings
								</DropdownMenuItem>
								<DropdownMenuItem onclick={inviteToGroup}>
									<Share2 class="mr-2 h-4 w-4" />
									Share Invite Link
								</DropdownMenuItem>
								<DropdownMenuItem>
									<Bell class="mr-2 h-4 w-4" />
									Notification Settings
								</DropdownMenuItem>
								<DropdownMenuSeparator />
								<DropdownMenuItem class="text-destructive">
									<Trash2 class="mr-2 h-4 w-4" />
									Leave Group
								</DropdownMenuItem>
							</DropdownMenuContent>
						</DropdownMenu>
					</div>
				</div>
			</header>

			<!-- Main Content Area -->
			<div class="p-4 md:p-6">
				<!-- Conditionally show event chat or group tabs -->
				{#if showEventChat && selectedEvent}
					<!-- Event-Specific Chat View -->
					<div class="space-y-4">
						<Button variant="ghost" size="sm" onclick={closeEventChat} class="mb-2">
							<ChevronLeft class="mr-2 h-4 w-4" />
							Back to {selectedEvent.title}
						</Button>

						<Card>
							<CardHeader class="border-b border-border bg-muted/30">
								<div class="flex items-center justify-between">
									<div>
										<CardTitle class="flex items-center gap-2">
											<MessageCircle class="h-5 w-5" />
											Event Chat: {selectedEvent.title}
										</CardTitle>
										<CardDescription>
											{new Date(selectedEvent.scheduledAt).toLocaleDateString('en-US', {
												weekday: 'long',
												month: 'long',
												day: 'numeric',
												hour: 'numeric',
												minute: 'numeric'
											})}
										</CardDescription>
									</div>
									<Badge variant={getEventStatus(selectedEvent).variant}>
										{getEventStatus(selectedEvent).label}
									</Badge>
								</div>
							</CardHeader>

							<CardContent class="p-0">
								<!-- Chat Messages -->
								<div class="max-h-[500px] space-y-4 overflow-y-auto p-6">
									{#each selectedEvent.chatMessages as message}
										<div class="flex gap-3">
											<Avatar class="h-8 w-8">
												<AvatarFallback>{message.user.name.charAt(0)}</AvatarFallback>
											</Avatar>
											<div class="flex-1">
												<div class="mb-1 flex items-center gap-2">
													<span class="font-medium">{message.user.name}</span>
													<span class="text-xs text-muted-foreground">{message.createdAt}</span>
												</div>
												<p class="rounded-lg bg-muted p-3">{message.message}</p>
											</div>
										</div>
									{/each}

									{#if selectedEvent.chatMessages.length === 0}
										<div class="py-12 text-center">
											<MessageCircle class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
											<h3 class="mb-2 text-lg font-semibold">No messages yet</h3>
											<p class="text-sm text-muted-foreground">
												Be the first to start the conversation about this movie night
											</p>
										</div>
									{/if}
								</div>

								<!-- Message Input -->
								{#if !selectedEvent.isLocked && new Date(selectedEvent.scheduledAt) > new Date()}
									<div class="border-t border-border p-4">
										<form
											class="flex gap-2"
											onsubmit={(e) => {
												e.preventDefault();
												const form = e.target as HTMLFormElement;
												const input = form.elements.namedItem('message') as HTMLInputElement;
												// if (input.value.trim()) {
												// 	sendEventChatMessage(selectedEvent.id, input.value);
												// 	input.value = '';
												// }
											}}
										>
											<input
												name="message"
												type="text"
												placeholder="Type your message..."
												class="flex-1 rounded-lg border border-border bg-background px-4 py-2"
											/>
											<Button type="submit">Send</Button>
										</form>
									</div>
								{/if}

								<!-- Read-only for past events -->
								{#if selectedEvent.isLocked || new Date(selectedEvent.scheduledAt) < new Date()}
									<div class="border-t border-border bg-muted/30 p-4 text-center">
										<p class="text-sm text-muted-foreground">
											<Lock class="mr-2 inline h-4 w-4" />
											This event has ended. Chat is now read-only.
										</p>
									</div>
								{/if}
							</CardContent>
						</Card>
					</div>
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
						<TabsContent value="upcoming" class="mt-6">
							{#if events.upcoming.length > 0}
								<div class="space-y-6">
									{#each events.upcoming as event}
										<Card class="overflow-hidden">
											<CardHeader class="bg-muted/50">
												<div class="flex items-center justify-between">
													<div>
														<CardTitle>{event.title}</CardTitle>
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
														<Button
															size="sm"
															variant="ghost"
															class="relative"
															onclick={() => openEventChat(event)}
														>
															<MessageSquare class="h-4 w-4" />
															{#if event.chatMessages.length > 0}
																<span
																	class="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center rounded-full bg-primary text-[10px] text-primary-foreground"
																>
																	{event.chatMessages.length}
																</span>
															{/if}
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
															{#each event.suggestions.filter((s) => !s.isDisqualified) as suggestion}
																<div class="rounded-lg border border-border p-4">
																	<div class="mb-3 flex items-center justify-between">
																		<div class="flex items-center gap-3">
																			<img
																				src={`https://image.tmdb.org/t/p/w92${suggestion.movieDetails?.posterPath}`}
																				alt={suggestion.movieDetails?.title}
																				class="h-12 w-8 rounded object-cover"
																			/>
																			<div>
																				<span class="font-medium"
																					>{suggestion.movieDetails?.title}</span
																				>
																				<span class="ml-2 text-sm text-muted-foreground">
																					Added by {suggestion.suggestedBy.name}
																				</span>
																			</div>
																		</div>
																		<div class="flex items-center gap-4">
																			<div class="flex items-center gap-1">
																				<ThumbsUp class="h-4 w-4 text-green-500" />
																				<span class="font-semibold">
																					{suggestion.votes.filter((v) => v.voteType === 'Upvote')
																						.length}
																				</span>
																			</div>
																			<div class="flex items-center gap-1">
																				<ThumbsDown class="h-4 w-4 text-red-500" />
																				<span class="font-semibold">
																					{suggestion.votes.filter((v) => v.voteType === 'Downvote')
																						.length}
																				</span>
																			</div>
																		</div>
																	</div>

																	<div class="flex gap-2">
																		<Button
																			size="sm"
																			variant={userVotes.get(suggestion.id) === 'Upvote'
																				? 'default'
																				: 'outline'}
																			class="flex-1"
																			disabled={!canVote(event)}
																			onclick={() => handleVote(suggestion.id, 'Upvote')}
																		>
																			<ThumbsUp class="mr-2 h-4 w-4" />
																			Upvote
																		</Button>
																		<Button
																			size="sm"
																			variant={userVotes.get(suggestion.id) === 'Downvote'
																				? 'default'
																				: 'outline'}
																			class="flex-1"
																			disabled={!canVote(event)}
																			onclick={() => handleVote(suggestion.id, 'Downvote')}
																		>
																			<ThumbsDown class="mr-2 h-4 w-4" />
																			Downvote
																		</Button>
																		<Button
																			size="sm"
																			variant={userVotes.get(suggestion.id) === 'Veto'
																				? 'destructive'
																				: 'outline'}
																			class="flex-1"
																			disabled={!canVote(event)}
																			onclick={() => handleVote(suggestion.id, 'Veto')}
																		>
																			<XCircle class="mr-2 h-4 w-4" />
																			Veto
																		</Button>
																	</div>
																</div>
															{/each}

															{#if event.suggestions.some((s) => s.isDisqualified)}
																<div class="mt-4">
																	<h4 class="mb-2 text-sm font-medium text-muted-foreground">
																		Disqualified (Vetoed)
																	</h4>
																	{#each event.suggestions.filter((s) => s.isDisqualified) as suggestion}
																		<div
																			class="rounded-lg border border-destructive/30 bg-destructive/5 p-3 opacity-60"
																		>
																			<div class="flex items-center gap-3">
																				<img
																					src={`https://image.tmdb.org/t/p/w92${suggestion.movieDetails?.posterPath}`}
																					alt={suggestion.movieDetails?.title}
																					class="h-10 w-7 rounded object-cover"
																				/>
																				<span class="font-medium line-through"
																					>{suggestion.movieDetails?.title}</span
																				>
																				<Badge variant="destructive" class="ml-auto">Vetoed</Badge>
																			</div>
																		</div>
																	{/each}
																</div>
															{/if}
														</div>
													</div>

													<div class="flex items-center justify-between rounded-lg bg-muted p-4">
														<div>
															<div class="font-medium">Add more options</div>
															<div class="text-sm text-muted-foreground">
																Suggest movies for everyone to vote on
															</div>
														</div>
														<Button size="sm">
															<Plus class="mr-2 h-4 w-4" />
															Add Movie
														</Button>
													</div>
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

														<div
															class="flex items-center justify-between rounded-lg border border-border p-4"
														>
															<div>
																<div class="font-medium">Event Discussion</div>
																<div class="text-sm text-muted-foreground">
																	{event.chatMessages.length} messages
																</div>
															</div>
															<Button
																size="sm"
																variant="outline"
																onclick={() => openEventChat(event)}
															>
																<MessageSquare class="mr-2 h-4 w-4" />
																Join Chat
															</Button>
														</div>
													</div>
												{/if}
											</CardContent>

											<CardFooter class="flex justify-between bg-muted/30">
												<Button variant="outline" size="sm">
													<Edit class="mr-2 h-4 w-4" />
													Edit Event
												</Button>
												<Button size="sm" onclick={() => openEventChat(event)}>
													<MessageSquare class="mr-2 h-4 w-4" />
													Event Chat ({event.chatMessages.length})
												</Button>
											</CardFooter>
										</Card>
									{/each}
								</div>
							{:else}
								<Card>
									<CardContent class="py-12 text-center">
										<Calendar class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
										<h3 class="mb-2 text-lg font-semibold">No upcoming movie nights</h3>
										<p class="mb-4 text-sm text-muted-foreground">
											Plan your first movie night with the group
										</p>
										<Button onclick={createNewEvent}>
											<Plus class="mr-2 h-4 w-4" />
											Plan Movie Night
										</Button>
									</CardContent>
								</Card>
							{/if}
						</TabsContent>

						<!-- Past Events Tab -->
						<TabsContent value="past" class="mt-6">
							{#if events.past.length > 0}
								<div class="grid gap-4 md:grid-cols-2">
									{#each events.past as event}
										<Card class="group transition-all hover:shadow-lg">
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
															{#each Array(5) as _, i}
																<Star
																	class={`h-4 w-4 ${
																		i <
																		Math.round(
																			event.ratings.reduce((acc, r) => acc + r.rating, 0) /
																				event.ratings.length || 0
																		)
																			? 'fill-primary text-primary'
																			: 'text-muted-foreground'
																	}`}
																/>
															{/each}
															<span class="ml-1 text-sm font-medium">
																{(
																	event.ratings.reduce((acc, r) => acc + r.rating, 0) /
																		event.ratings.length || 0
																).toFixed(1)}
															</span>
														</div>
													</div>

													<div class="flex items-center gap-2 pt-2">
														<Button
															variant="outline"
															size="sm"
															class="flex-1"
															onclick={() => openEventChat(event)}
														>
															<MessageSquare class="mr-2 h-4 w-4" />
															Chat ({event.chatMessages.length})
														</Button>
														<Button variant="outline" size="sm" class="flex-1">
															<Star class="mr-2 h-4 w-4" />
															Rate
														</Button>
													</div>
												</div>
											</CardContent>
										</Card>
									{/each}
								</div>
							{:else}
								<Card>
									<CardContent class="py-12 text-center">
										<Clock class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
										<h3 class="mb-2 text-lg font-semibold">No past events yet</h3>
										<p class="text-sm text-muted-foreground">
											Your group's watch history will appear here
										</p>
									</CardContent>
								</Card>
							{/if}
						</TabsContent>

						<!-- Members Tab -->
						<TabsContent value="members" class="mt-6">
							<Card>
								<CardHeader>
									<CardTitle>Group Members</CardTitle>
									<CardDescription
										>{activeGroup.members.length} people in this group</CardDescription
									>
								</CardHeader>
								<CardContent>
									<div class="space-y-3">
										{#each activeGroup.members as member}
											<div
												class="flex items-center justify-between rounded-lg border border-border p-4"
											>
												<div class="flex items-center gap-3">
													<Avatar>
														<AvatarFallback>{member.name.charAt(0)}</AvatarFallback>
													</Avatar>
													<div>
														<div class="font-medium">{member.name}</div>
														<div class="flex items-center gap-2 text-sm text-muted-foreground">
															{member.isAdmin ? 'Group Admin' : 'Member'}
															<span>•</span>
															<span
																>Joined {new Date(member.joinedAt).toLocaleDateString('en-US', {
																	month: 'short',
																	day: 'numeric'
																})}</span
															>
														</div>
													</div>
												</div>
												<div class="flex items-center gap-2">
													{#if member.isAdmin}
														<Badge variant="outline">Admin</Badge>
													{/if}
													<DropdownMenu>
														<DropdownMenuTrigger>
															<Button size="sm" variant="ghost">
																<MoreVertical class="h-4 w-4" />
															</Button>
														</DropdownMenuTrigger>
														<DropdownMenuContent align="end">
															<DropdownMenuItem>
																<MessageSquare class="mr-2 h-4 w-4" />
																Message
															</DropdownMenuItem>
															{#if !member.isAdmin}
																<DropdownMenuItem>
																	<Users class="mr-2 h-4 w-4" />
																	Make Admin
																</DropdownMenuItem>
															{/if}
															<DropdownMenuSeparator />
															<DropdownMenuItem class="text-destructive">
																<Trash2 class="mr-2 h-4 w-4" />
																Remove from Group
															</DropdownMenuItem>
														</DropdownMenuContent>
													</DropdownMenu>
												</div>
											</div>
										{/each}
									</div>
								</CardContent>
								<CardFooter class="flex flex-col gap-4">
									<Separator />
									<div class="flex w-full items-center justify-between">
										<div>
											<h4 class="font-medium">Invite new members</h4>
											<p class="text-sm text-muted-foreground">
												Share a link or send email invites
											</p>
										</div>
										<div class="flex gap-2">
											<Button variant="outline" onclick={inviteToGroup}>
												<Link class="mr-2 h-4 w-4" />
												Copy Link
											</Button>
											<Button onclick={inviteToGroup}>
												<Mail class="mr-2 h-4 w-4" />
												Email Invite
											</Button>
										</div>
									</div>
								</CardFooter>
							</Card>
						</TabsContent>

						<!-- Stats Tab -->
						<TabsContent value="stats" class="mt-6">
							<Card>
								<CardHeader>
									<CardTitle>Group Statistics</CardTitle>
									<CardDescription>Your movie night analytics</CardDescription>
								</CardHeader>
								<CardContent>
									<div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
										<div class="rounded-lg border border-border p-4">
											<div class="text-2xl font-bold">{stats.moviesWatched}</div>
											<div class="text-sm text-muted-foreground">Movies Watched</div>
										</div>
										<div class="rounded-lg border border-border p-4">
											<div class="text-2xl font-bold">{stats.totalVotes}</div>
											<div class="text-sm text-muted-foreground">Total Votes</div>
										</div>
										<div class="rounded-lg border border-border p-4">
											<div class="text-2xl font-bold">{stats.averageRating}/5</div>
											<div class="text-sm text-muted-foreground">Avg Rating</div>
										</div>
										<div class="rounded-lg border border-border p-4">
											<div class="text-2xl font-bold">{stats.streak} weeks</div>
											<div class="text-sm text-muted-foreground">Current Streak</div>
										</div>
									</div>

									<Separator class="my-6" />

									<h3 class="mb-4 font-semibold">Recent Activity</h3>
									<div class="space-y-3">
										{#each events.past.slice(0, 3) as event}
											<div class="flex items-center justify-between">
												<div class="flex items-center gap-2">
													<Film class="h-4 w-4 text-muted-foreground" />
													<span>{event.selectedMovie?.title}</span>
												</div>
												<div class="flex items-center gap-1">
													<Star class="h-3 w-3 fill-primary text-primary" />
													<span class="text-sm">
														{(
															event.ratings.reduce((acc, r) => acc + r.rating, 0) /
																event.ratings.length || 0
														).toFixed(1)}
													</span>
												</div>
											</div>
										{/each}
									</div>
								</CardContent>
							</Card>
						</TabsContent>
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
		<nav class="fixed right-0 bottom-0 left-0 border-t border-border bg-background p-4 md:hidden">
			<div class="grid grid-cols-4 gap-4">
				<button
					onclick={() => (activeTab = 'upcoming')}
					class={`flex flex-col items-center gap-1 ${activeTab === 'upcoming' ? 'text-primary' : 'text-muted-foreground'}`}
				>
					<Calendar class="h-5 w-5" />
					<span class="text-xs">Upcoming</span>
				</button>
				<button
					onclick={() => (activeTab = 'past')}
					class={`flex flex-col items-center gap-1 ${activeTab === 'past' ? 'text-primary' : 'text-muted-foreground'}`}
				>
					<Clock class="h-5 w-5" />
					<span class="text-xs">Past</span>
				</button>
				<button
					onclick={() => (activeTab = 'stats')}
					class={`flex flex-col items-center gap-1 ${activeTab === 'stats' ? 'text-primary' : 'text-muted-foreground'}`}
				>
					<Star class="h-5 w-5" />
					<span class="text-xs">Stats</span>
				</button>
				<button
					onclick={() => (activeTab = 'members')}
					class={`flex flex-col items-center gap-1 ${activeTab === 'members' ? 'text-primary' : 'text-muted-foreground'}`}
				>
					<Users class="h-5 w-5" />
					<span class="text-xs">Members</span>
				</button>
			</div>
		</nav>
	</main>
</div>

<style>
	/* Custom scrollbar for sidebar */
	aside > div:last-of-type {
		scrollbar-width: thin;
		scrollbar-color: hsl(var(--muted-foreground)) hsl(var(--muted));
	}

	aside > div:last-of-type::-webkit-scrollbar {
		width: 6px;
	}

	aside > div:last-of-type::-webkit-scrollbar-track {
		background: hsl(var(--muted));
	}

	aside > div:last-of-type::-webkit-scrollbar-thumb {
		background: hsl(var(--muted-foreground));
		border-radius: 3px;
	}

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
