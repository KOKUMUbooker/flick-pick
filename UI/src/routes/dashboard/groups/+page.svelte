<script lang="ts">
	import { Avatar, AvatarFallback } from '$lib/components/ui/avatar';
	import { Badge } from '$lib/components/ui/badge';
	import { Button } from '$lib/components/ui/button';
	import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '$lib/components/ui/card';
	import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from '$lib/components/ui/dropdown-menu';
	import { Progress } from '$lib/components/ui/progress';
	import { Separator } from '$lib/components/ui/separator';
	import { Tabs, TabsContent, TabsList, TabsTrigger } from '$lib/components/ui/tabs';
	import {
		Bell,
		Calendar,
		ChevronRight,
		Clock,
		Edit,
		Film,
		Link,
		Mail,
		MessageSquare,
		MoreVertical,
		Play,
		Plus,
		Search,
		Settings,
		Share2,
		Star,
		ThumbsDown,
		ThumbsUp,
		Trash2,
		Users,
		Vote,
		XCircle
	} from '@lucide/svelte';
	import { onMount } from 'svelte';
	
	// State management
	let activeGroup = $state<Group | null>(null);
	let groups = $state<Group[]>([]);
	let activeTab = $state('upcoming');
	let sidebarCollapsed = $state(false);
	let searchQuery = $state('');
	
	// Types
	interface Group {
		id: number;
		name: string;
		description: string;
		members: Member[];
		unreadCount: number;
		upcomingEvents: number;
		lastActivity: string;
		isActive?: boolean;
	}
	
	interface Member {
		id: number;
		name: string;
		role: 'admin' | 'member';
		avatar: string;
	}
	
	// Mock data - in real app, fetch from API
	const mockGroups: Group[] = [
		{
			id: 1,
			name: 'Friday Film Club',
			description: 'Weekly movie nights with close friends',
			members: [
				{ id: 1, name: 'Alex Johnson', role: 'admin', avatar: '' },
				{ id: 2, name: 'Sam Taylor', role: 'member', avatar: '' },
				{ id: 3, name: 'Jordan Lee', role: 'member', avatar: '' },
				{ id: 4, name: 'Taylor Smith', role: 'member', avatar: '' },
				{ id: 5, name: 'Morgan Wells', role: 'member', avatar: '' }
			],
			unreadCount: 3,
			upcomingEvents: 2,
			lastActivity: '10 min ago'
		},
		{
			id: 2,
			name: 'Horror Nights',
			description: 'Monthly scary movie sessions',
			members: [
				{ id: 1, name: 'Alex Johnson', role: 'admin', avatar: '' },
				{ id: 6, name: 'Casey Kim', role: 'member', avatar: '' },
				{ id: 7, name: 'Riley Chen', role: 'member', avatar: '' }
			],
			unreadCount: 0,
			upcomingEvents: 0,
			lastActivity: '2 days ago'
		},
		{
			id: 3,
			name: 'Family Movie Night',
			description: 'Kid-friendly weekend movies',
			members: [
				{ id: 1, name: 'Alex Johnson', role: 'admin', avatar: '' },
				{ id: 8, name: 'Jamie Wilson', role: 'member', avatar: '' },
				{ id: 9, name: 'Taylor Jr', role: 'member', avatar: '' },
				{ id: 10, name: 'Morgan Jr', role: 'member', avatar: '' }
			],
			unreadCount: 1,
			upcomingEvents: 1,
			lastActivity: '1 hour ago'
		},
		{
			id: 4,
			name: 'Oscar Watch Party',
			description: 'Annual Academy Awards viewing',
			members: [
				{ id: 1, name: 'Alex Johnson', role: 'admin', avatar: '' },
				{ id: 2, name: 'Sam Taylor', role: 'member', avatar: '' },
				{ id: 3, name: 'Jordan Lee', role: 'member', avatar: '' },
				{ id: 11, name: 'Drew Patel', role: 'member', avatar: '' }
			],
			unreadCount: 0,
			upcomingEvents: 0,
			lastActivity: '3 weeks ago'
		}
	];
	
	// Events data for active group
	let events = $state({
		upcoming: [] as any[],
		past: [] as any[]
	});
	
	let generalChat = $state([] as any[]);
	let stats = $state({
		moviesWatched: 0,
		totalVotes: 0,
		averageRating: 0,
		streak: 0
	});
	
	// Filtered groups based on search
	const filteredGroups = $derived(
		searchQuery 
			? groups.filter(g => 
				g.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
				g.description.toLowerCase().includes(searchQuery.toLowerCase())
			)
			: groups
	);
	
	// Initialize
	onMount(() => {
		groups = mockGroups.map(g => ({ ...g, isActive: false }));
		
		// Set first group as active by default
		if (groups.length > 0) {
			setActiveGroup(groups[0]);
		}
	});
	
	// Set active group and load its data
	function setActiveGroup(group: Group) {
		// Update active state
		groups = groups.map(g => ({
			...g,
			isActive: g.id === group.id
		}));
		
		activeGroup = { ...group, isActive: true };
		
		// Load group-specific data (in real app, fetch from API)
		loadGroupData(group.id);
	}
	
	function loadGroupData(groupId: number) {
		// Mock data loading based on group ID
		if (groupId === 1) {
			// Friday Film Club data
			events = {
				upcoming: [
					{
						id: 1,
						title: 'Sci-Fi Night',
						status: 'voting',
						movieOptions: [
							{ id: 1, title: 'Dune: Part Two', votes: 5, vetoed: false, addedBy: 'Alex' },
							{ id: 2, title: 'Interstellar', votes: 3, vetoed: false, addedBy: 'Sam' },
							{ id: 3, title: 'Arrival', votes: 2, vetoed: true, addedBy: 'Jordan' }
						],
						scheduledTime: 'Tomorrow, 8:00 PM',
						votingEnds: 'in 4 hours',
						participants: 4
					},
					{
						id: 2,
						title: 'Comedy Special',
						status: 'scheduled',
						winningMovie: 'The Holdovers',
						scheduledTime: 'Saturday, 7:30 PM',
						participants: 5,
						streamingOn: ['Netflix', 'Hulu']
					}
				],
				past: [
					{
						id: 3,
						title: 'Horror Night',
						watchedMovie: 'Hereditary',
						watchedDate: 'Last Friday',
						groupRating: 8.2,
						discussionCount: 12
					},
					{
						id: 4,
						title: 'Oscar Watch',
						watchedMovie: 'Poor Things',
						watchedDate: '2 weeks ago',
						groupRating: 9.0,
						discussionCount: 8
					}
				]
			};
			
			generalChat = [
				{ id: 1, user: 'Alex', message: 'Anyone up for something sci-fi this week?', time: '2 hours ago' },
				{ id: 2, user: 'Sam', message: 'Yes! I just watched Dune trailer again 🔥', time: '1 hour ago' },
				{ id: 3, user: 'Jordan', message: 'Can we avoid anything too long? Got early morning Saturday', time: '45 min ago' },
				{ id: 4, user: 'Taylor', message: 'How about we start voting for options?', time: '30 min ago' }
			];
			
			stats = {
				moviesWatched: 8,
				totalVotes: 42,
				averageRating: 7.8,
				streak: 3
			};
		} else {
			// Default/empty data for other groups
			events = { upcoming: [], past: [] };
			generalChat = [];
			stats = { moviesWatched: 0, totalVotes: 0, averageRating: 0, streak: 0 };
		}
	}
	
	function createNewEvent() {
		if (!activeGroup) return;
		console.log('Create event for group:', activeGroup.id);
		// In real app: navigate to event creation
	}
	
	function inviteToGroup() {
		if (!activeGroup) return;
		console.log('Invite to group:', activeGroup.id);
		// Show invite modal
	}
	
	function createNewGroup() {
		console.log('Create new group');
		// In real app: show create group modal
	}
	
	function toggleSidebar() {
		sidebarCollapsed = !sidebarCollapsed;
	}
	
	// User's vote state
	let userVotes = $state(new Map());
	
	function handleVote(movieId: number, voteType: 'up' | 'down' | 'veto') {
		userVotes.set(movieId, voteType);
		console.log(`Voted ${voteType} on movie ${movieId}`);
		// In real app: call API with SignalR
	}
</script>

<div class="flex min-h-screen bg-background">
	<!-- Sidebar -->
	<aside 
		class={`flex flex-col border-r border-border bg-card transition-all duration-300 ${sidebarCollapsed ? 'w-0 md:w-16' : 'w-full md:w-80'}`}
	>
		{#if !sidebarCollapsed}
			<!-- Expanded Sidebar -->
			<div class="flex h-16 items-center justify-between border-b border-border px-4">
				<h2 class="text-lg font-semibold">Your Groups</h2>
				<Button size="sm" variant="ghost" onclick={toggleSidebar} class="hidden md:inline-flex">
					<ChevronRight class="h-4 w-4 rotate-180" />
				</Button>
			</div>
			
			<!-- Search -->
			<div class="border-b border-border p-4">
				<div class="relative">
					<Search class="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
					<input
						bind:value={searchQuery}
						type="text"
						placeholder="Search groups..."
						class="w-full rounded-lg border border-border bg-background py-2 pl-10 pr-4 text-sm"
					/>
				</div>
			</div>
			
			<!-- Groups List -->
			<div class="flex-1 overflow-y-auto p-2">
				{#if filteredGroups.length === 0}
					<div class="p-4 text-center">
						<p class="text-sm text-muted-foreground">No groups found</p>
					</div>
				{:else}
					<div class="space-y-1">
						{#each filteredGroups as group}
							<button
								onclick={() => setActiveGroup(group)}
								class={`flex w-full items-center justify-between rounded-lg p-3 text-left transition-colors ${group.isActive ? 'bg-primary text-primary-foreground' : 'hover:bg-muted'}`}
							>
								<div class="flex items-center gap-3 overflow-hidden">
									<div class="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10">
										<Users class="h-5 w-5" />
									</div>
									<div class="min-w-0 flex-1">
										<div class="truncate font-medium">{group.name}</div>
										<div class="truncate text-xs opacity-80">
											{group.members.length} members • {group.lastActivity}
										</div>
									</div>
								</div>
								<div class="flex items-center gap-1">
									{#if group.unreadCount > 0}
										<Badge variant="default" class="h-5 min-w-5 px-1 text-xs">
											{group.unreadCount}
										</Badge>
									{/if}
									{#if group.upcomingEvents > 0}
										<Badge variant="outline" class="h-5 text-xs">
											{group.upcomingEvents}
										</Badge>
									{/if}
								</div>
							</button>
						{/each}
					</div>
				{/if}
			</div>
			
			<!-- Create Group Button -->
			<div class="border-t border-border p-4">
				<Button class="w-full" onclick={createNewGroup}>
					<Plus class="mr-2 h-4 w-4" />
					Create New Group
				</Button>
			</div>
		{:else}
			<!-- Collapsed Sidebar -->
			<div class="flex flex-col items-center py-4">
				<Button size="sm" variant="ghost" onclick={toggleSidebar} class="mb-4">
					<ChevronRight class="h-4 w-4" />
				</Button>
				<div class="space-y-2">
					{#each groups.slice(0, 4) as group}
						<button
							onclick={() => setActiveGroup(group)}
							class={`flex h-10 w-10 items-center justify-center rounded-lg ${group.isActive ? 'bg-primary text-primary-foreground' : 'bg-muted hover:bg-muted/80'}`}
							title={group.name}
						>
							<div class="text-xs font-medium">{group.name.charAt(0)}</div>
						</button>
					{/each}
					<Button
						size="sm"
						variant="ghost"
						onclick={createNewGroup}
						class="h-10 w-10"
						title="Create Group"
					>
						<Plus class="h-4 w-4" />
					</Button>
				</div>
			</div>
		{/if}
	</aside>
	
	<!-- Main Content -->
	<main class="flex-1">
		{#if activeGroup}
			<!-- Group Header -->
			<header class="sticky top-0 z-50 border-b border-border bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
				<div class="flex h-16 items-center justify-between px-4 md:px-6">
					<div class="flex items-center gap-3">
						<Button size="sm" variant="ghost" onclick={toggleSidebar} class="md:hidden">
							<ChevronRight class="h-4 w-4" />
						</Button>
						<div>
							<h1 class="text-xl font-semibold">{activeGroup.name}</h1>
							<p class="text-sm text-muted-foreground">
								{activeGroup.members.length} members • {activeGroup.description}
							</p>
						</div>
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
							<DropdownMenuTrigger  >
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
				<Tabs value={activeTab} onValueChange={(value) => activeTab = value} class="mb-8">
					<TabsList class="grid w-full grid-cols-4 md:w-auto">
						<TabsTrigger value="upcoming" class="data-[state=active]:bg-primary data-[state=active]:text-primary-foreground">
							<Calendar class="mr-2 h-4 w-4" />
							Upcoming
						</TabsTrigger>
						<TabsTrigger value="past" class="data-[state=active]:bg-primary data-[state=active]:text-primary-foreground">
							<Clock class="mr-2 h-4 w-4" />
							Past Events
						</TabsTrigger>
						<TabsTrigger value="chat" class="data-[state=active]:bg-primary data-[state=active]:text-primary-foreground">
							<MessageSquare class="mr-2 h-4 w-4" />
							Group Chat
						</TabsTrigger>
						<TabsTrigger value="members" class="data-[state=active]:bg-primary data-[state=active]:text-primary-foreground">
							<Users class="mr-2 h-4 w-4" />
							Members
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
														{#if event.status === 'voting'}
															Voting ends {event.votingEnds}
														{:else}
															Scheduled for {event.scheduledTime}
														{/if}
													</CardDescription>
												</div>
												<Badge variant={event.status === 'voting' ? 'default' : 'outline'}>
													{event.status === 'voting' ? 'Voting Active' : 'Scheduled'}
												</Badge>
											</div>
										</CardHeader>
										
										<CardContent class="p-6">
											{#if event.status === 'voting'}
												<!-- Voting Interface -->
												<div class="mb-6">
													<h3 class="mb-4 font-semibold">Cast Your Vote</h3>
													<div class="space-y-3">
														{#each event.movieOptions as movie}
															<div class={`rounded-lg border p-4 ${movie.vetoed ? 'border-destructive/30 bg-destructive/5' : 'border-border'}`}>
																<div class="mb-3 flex items-center justify-between">
																	<div class="flex items-center gap-3">
																		<span class="font-medium">{movie.title}</span>
																		<span class="text-sm text-muted-foreground">Added by {movie.addedBy}</span>
																		{#if movie.vetoed}
																			<Badge variant="destructive" >Vetoed</Badge>
																		{/if}
																	</div>
																	<div class="flex items-center gap-2">
																		<ThumbsUp class="h-4 w-4 text-green-500" />
																		<span class="font-semibold">{movie.votes}</span>
																	</div>
																</div>
																
																{#if !movie.vetoed}
																	<div class="flex gap-2">
																		<Button 
																			size="sm" 
																			variant={userVotes.get(movie.id) === 'up' ? 'default' : 'outline'}
																			class="flex-1"
																			onclick={() => handleVote(movie.id, 'up')}
																		>
																			<ThumbsUp class="mr-2 h-4 w-4" />
																			Upvote
																		</Button>
																		<Button 
																			size="sm" 
																			variant={userVotes.get(movie.id) === 'down' ? 'default' : 'outline'}
																			class="flex-1"
																			onclick={() => handleVote(movie.id, 'down')}
																		>
																			<ThumbsDown class="mr-2 h-4 w-4" />
																			Downvote
																		</Button>
																		<Button 
																			size="sm" 
																			variant={userVotes.get(movie.id) === 'veto' ? 'destructive' : 'outline'}
																			class="flex-1"
																			onclick={() => handleVote(movie.id, 'veto')}
																		>
																			<XCircle class="mr-2 h-4 w-4" />
																			Veto
																		</Button>
																	</div>
																{/if}
															</div>
														{/each}
													</div>
												</div>
												
												<div class="flex items-center justify-between rounded-lg bg-muted p-4">
													<div>
														<div class="font-medium">Add more options</div>
														<div class="text-sm text-muted-foreground">Suggest movies for everyone to vote on</div>
													</div>
													<Button size="sm">
														<Plus class="mr-2 h-4 w-4" />
														Add Movie
													</Button>
												</div>
											{:else}
												<!-- Scheduled Event Details -->
												<div class="space-y-4">
													<div class="rounded-lg bg-primary/5 p-4">
														<div class="mb-2 flex items-center gap-2">
															<Film class="h-5 w-5 text-primary" />
															<span class="text-lg font-semibold">{event.winningMovie}</span>
														</div>
														<div class="grid gap-4 sm:grid-cols-2">
															<div>
																<div class="text-sm text-muted-foreground">When</div>
																<div class="font-medium">{event.scheduledTime}</div>
															</div>
															<div>
																<div class="text-sm text-muted-foreground">Streaming On</div>
																<div class="flex gap-1">
																	{#each event.streamingOn as service}
																		<Badge variant="outline">{service}</Badge>
																	{/each}
																</div>
															</div>
														</div>
													</div>
													
													<div class="flex items-center gap-4 rounded-lg border border-border p-4">
														<div class="flex-1">
															<div class="font-medium">{event.participants} of {activeGroup.members.length} attending</div>
															<div class="text-sm text-muted-foreground">Click to RSVP or update availability</div>
														</div>
														<Button>
															<Calendar class="mr-2 h-4 w-4" />
															Add to Calendar
														</Button>
													</div>
												</div>
											{/if}
										</CardContent>
										
										<CardFooter class="bg-muted/30 flex justify-between">
											<Button variant="outline" size="sm">
												<Edit class="mr-2 h-4 w-4" />
												Edit Event
											</Button>
											<Button size="sm">
												{#if event.status === 'voting'}
													<Vote class="mr-2 h-4 w-4" />
													Finalize Voting
												{:else}
													<Play class="mr-2 h-4 w-4" />
													Start Watching
												{/if}
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
									<Card class="group cursor-pointer transition-all hover:shadow-lg">
										<CardContent class="p-6">
											<div class="mb-4 flex items-start justify-between">
												<div>
													<h3 class="mb-1 text-lg font-semibold">{event.watchedMovie}</h3>
													<p class="text-sm text-muted-foreground">{event.title} • {event.watchedDate}</p>
												</div>
												<Badge class="bg-primary/10 text-primary">
													<Star class="mr-1 h-3 w-3" />
													{event.groupRating}/10
												</Badge>
											</div>
											
											<div class="space-y-3">
												<div class="flex items-center justify-between text-sm">
													<span class="text-muted-foreground">Group Rating</span>
													<span class="font-medium">{event.groupRating}/10</span>
												</div>
												<Progress value={event.groupRating * 10} class="h-2" />
												
												<div class="flex items-center gap-4 pt-2">
													<Button variant="outline" size="sm" class="flex-1">
														<MessageSquare class="mr-2 h-4 w-4" />
														View Discussion ({event.discussionCount})
													</Button>
													<Button variant="outline" size="sm" class="flex-1">
														<Star class="mr-2 h-4 w-4" />
														Rate Again
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
					
					<!-- Group Chat Tab -->
					<TabsContent value="chat" class="mt-6">
						<Card>
							<CardHeader>
								<CardTitle>Group Chat</CardTitle>
								<CardDescription>General discussion for {activeGroup.name}</CardDescription>
							</CardHeader>
							<CardContent>
								<div class="mb-6 space-y-4 max-h-[400px] overflow-y-auto pr-2">
									{#each generalChat as message}
										<div class="flex gap-3">
											<Avatar class="h-8 w-8">
												<AvatarFallback>{message.user.charAt(0)}</AvatarFallback>
											</Avatar>
											<div class="flex-1">
												<div class="mb-1 flex items-center gap-2">
													<span class="font-medium">{message.user}</span>
													<span class="text-xs text-muted-foreground">{message.time}</span>
												</div>
												<p class="rounded-lg bg-muted p-3">{message.message}</p>
											</div>
										</div>
									{/each}
								</div>
								
								<div class="flex gap-2">
									<input 
										type="text" 
										placeholder="Type a message..." 
										class="flex-1 rounded-lg border border-border bg-background px-4 py-2"
									/>
									<Button>Send</Button>
								</div>
							</CardContent>
						</Card>
					</TabsContent>
					
					<!-- Members Tab -->
					<TabsContent value="members" class="mt-6">
						<Card>
							<CardHeader>
								<CardTitle>Group Members</CardTitle>
								<CardDescription>{activeGroup.members.length} people in this group</CardDescription>
							</CardHeader>
							<CardContent>
								<div class="space-y-3">
									{#each activeGroup.members as member}
										<div class="flex items-center justify-between rounded-lg border border-border p-4">
											<div class="flex items-center gap-3">
												<Avatar>
													<AvatarFallback>{member.name.charAt(0)}</AvatarFallback>
												</Avatar>
												<div>
													<div class="font-medium">{member.name}</div>
													<div class="flex items-center gap-2 text-sm text-muted-foreground">
														{member.role === 'admin' ? 'Group Admin' : 'Member'}
														<span>•</span>
														<span>Joined Jan 15</span>
													</div>
												</div>
											</div>
											<div class="flex items-center gap-2">
												{#if member.role === 'admin'}
													<Badge variant="outline">Admin</Badge>
												{/if}
												<DropdownMenu>
													<DropdownMenuTrigger  >
														<Button size="sm" variant="ghost">
															<MoreVertical class="h-4 w-4" />
														</Button>
													</DropdownMenuTrigger>
													<DropdownMenuContent align="end">
														<DropdownMenuItem>
															<MessageSquare class="mr-2 h-4 w-4" />
															Message
														</DropdownMenuItem>
														{#if member.role !== 'admin'}
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
										<p class="text-sm text-muted-foreground">Share a link or send email invites</p>
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
						
						<!-- Group Stats -->
						<Card class="mt-6">
							<CardHeader>
								<CardTitle>Group Statistics</CardTitle>
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
										<div class="text-2xl font-bold">{stats.averageRating}/10</div>
										<div class="text-sm text-muted-foreground">Avg Rating</div>
									</div>
									<div class="rounded-lg border border-border p-4">
										<div class="text-2xl font-bold">{stats.streak} weeks</div>
										<div class="text-sm text-muted-foreground">Current Streak</div>
									</div>
								</div>
							</CardContent>
						</Card>
					</TabsContent>
				</Tabs>
			</div>
		{:else}
			<!-- No Group Selected State -->
			<div class="flex h-full items-center justify-center p-8">
				<div class="max-w-md text-center">
					<Users class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
					<h2 class="mb-2 text-2xl font-semibold">Welcome to GroupWatch</h2>
					<p class="mb-6 text-muted-foreground">
						Select a group from the sidebar or create your first group to start planning movie nights.
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
		<nav class="fixed bottom-0 left-0 right-0 border-t border-border bg-background p-4 md:hidden">
			<div class="grid grid-cols-4 gap-4">
				<button onclick={() => activeTab = 'upcoming'} class={`flex flex-col items-center gap-1 ${activeTab === 'upcoming' ? 'text-primary' : 'text-muted-foreground'}`}>
					<Calendar class="h-5 w-5" />
					<span class="text-xs">Upcoming</span>
				</button>
				<button onclick={() => activeTab = 'past'} class={`flex flex-col items-center gap-1 ${activeTab === 'past' ? 'text-primary' : 'text-muted-foreground'}`}>
					<Clock class="h-5 w-5" />
					<span class="text-xs">Past</span>
				</button>
				<button onclick={() => activeTab = 'chat'} class={`flex flex-col items-center gap-1 ${activeTab === 'chat' ? 'text-primary' : 'text-muted-foreground'}`}>
					<MessageSquare class="h-5 w-5" />
					<span class="text-xs">Chat</span>
				</button>
				<button onclick={() => activeTab = 'members'} class={`flex flex-col items-center gap-1 ${activeTab === 'members' ? 'text-primary' : 'text-muted-foreground'}`}>
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
	
	/* Smooth sidebar transitions */
	aside {
		transition: width 0.3s ease;
	}
	
	/* Mobile responsive */
	@media (max-width: 768px) {
		aside:not(.w-0) {
			position: fixed;
			top: 0;
			left: 0;
			bottom: 0;
			z-index: 50;
			width: 100% !important;
			background: hsl(var(--background));
		}
	}
</style>