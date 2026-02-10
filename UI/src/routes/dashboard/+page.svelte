<script lang="ts">
	import { Badge } from '$lib/components/ui/badge';
	import { Button } from '$lib/components/ui/button';
	import { Card, CardContent } from '$lib/components/ui/card';
	import {
		Calendar,
		ChevronRight,
		Clock,
		Film,
		MessageSquare,
		Plus,
		Sparkles,
		Users,
		Vote
	} from '@lucide/svelte';
	
	// Mock data
	const userGroups = [
		{ id: 1, name: 'Friday Film Club', members: 5, upcomingEvents: 1, unread: 3 },
		{ id: 2, name: 'Horror Nights', members: 3, upcomingEvents: 0, unread: 0 },
		{ id: 3, name: 'Family Movie Night', members: 4, upcomingEvents: 1, unread: 1 }
	];
	
	const upcomingEvents = [
		{ 
			id: 1, 
			group: 'Friday Film Club', 
			movie: 'Dune: Part Two', 
			date: 'Tomorrow, 8:00 PM', 
			votingEnds: 'in 4 hours',
			status: 'voting'
		},
		{ 
			id: 2, 
			group: 'Family Movie Night', 
			movie: 'The Super Mario Bros. Movie', 
			date: 'Saturday, 3:00 PM', 
			votingEnds: null,
			status: 'scheduled'
		}
	];
	
	const recentActivity = [
		{ group: 'Friday Film Club', action: 'Alex voted for Dune: Part Two', time: '10 min ago' },
		{ group: 'Horror Nights', action: 'Sam suggested "Hereditary"', time: '2 hours ago' },
		{ group: 'Friday Film Club', action: 'Movie night scheduled for tomorrow', time: '1 day ago' }
	];
	
	const quickActions = [
		{ icon: Plus, label: 'Create Group', description: 'Start a new movie group', color: 'bg-primary' },
		{ icon: Calendar, label: 'Plan Movie Night', description: 'Schedule in any group', color: 'bg-green-500' },
		{ icon: Film, label: 'Suggest Movies', description: 'Add to existing votes', color: 'bg-purple-500' },
		{ icon: Users, label: 'Invite Friends', description: 'Grow your groups', color: 'bg-blue-500' }
	];
</script>

<div class="min-h-screen bg-background">
	<!-- Main Content -->
	<main class="container mx-auto px-4 py-6 lg:py-8">
		<!-- Top Header Section -->
		<section class="mb-8">
			<div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
				<div>
					<h1 class="text-2xl lg:text-3xl font-bold">Welcome back, Alex! </h1>
					<p class="text-muted-foreground mt-1">What's your group watching next?</p>
				</div>
				<div class="text-4xl">👋</div>
				<!-- <Badge class="bg-primary/10 text-primary self-start sm:self-center">
					<Sparkles class="mr-1 h-3 w-3" />
					Pro Member
				</Badge> -->
			</div>
		</section>
		
		<!-- Responsive Layout Grid -->
		<div class="flex flex-col lg:flex-row lg:gap-8">
			<!-- Left Sidebar - Groups (Hidden on mobile, shown on desktop) -->
			<aside class="hidden lg:block lg:w-1/4 xl:w-1/5 ">
				<section class="sticky top-24 space-y-6">
					<!-- Groups Section -->
					<div>
						<div class="flex items-center justify-between mb-4">
							<h2 class="text-lg font-semibold">Your Groups</h2>
							<Button size="sm" variant="ghost">
								<Plus class="h-4 w-4" />
							</Button>
						</div>
						<div class="space-y-2">
							{#each userGroups as group}
								<a 
									href={`/groups/${group.id}`}
									class="group flex items-center justify-between rounded-lg border border-border p-3 transition-colors hover:border-primary/30 hover:bg-muted/50"
								>
									<div class="flex items-center gap-3">
										<div class="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10">
											<Users class="h-5 w-5 text-primary" />
										</div>
										<div class="min-w-0">
											<div class="font-medium truncate">{group.name}</div>
											<div class="text-xs text-muted-foreground">{group.members} members</div>
										</div>
									</div>
									<div class="flex items-center gap-2">
										{#if group.unread > 0}
											<Badge class="bg-primary text-xs min-w-[20px] justify-center">{group.unread}</Badge>
										{/if}
										<ChevronRight class="h-4 w-4 text-muted-foreground transition-transform group-hover:translate-x-0.5" />
									</div>
								</a>
							{/each}
						</div>
						<Button class="mt-4 w-full" variant="outline">
							<Plus class="mr-2 h-4 w-4" />
							Create New Group
						</Button>
					</div>
					
					<!-- Quick Stats -->
					<div class="rounded-lg border border-border p-4">
						<h3 class="mb-3 font-semibold">Your Movie Stats</h3>
						<div class="space-y-3">
							<div class="flex justify-between">
								<span class="text-sm text-muted-foreground">Groups</span>
								<span class="font-semibold">3</span>
							</div>
							<div class="flex justify-between">
								<span class="text-sm text-muted-foreground">Movies Planned</span>
								<span class="font-semibold">12</span>
							</div>
							<div class="flex justify-between">
								<span class="text-sm text-muted-foreground">Votes Cast</span>
								<span class="font-semibold">47</span>
							</div>
						</div>
					</div>
				</section>
			</aside>
			
			<!-- Main Content Area -->
			<div class="sm:ml-0 ml-4 flex-1">
				<!-- Quick Actions -->
				<section class="mb-8">
					<h2 class="mb-4 text-lg font-semibold">Quick Actions</h2>
					<div class="grid grid-cols-2 gap-4">
						{#each quickActions as action}
							<button class="group rounded-xl border border-border bg-card py-4 px-4 p-5 text-left transition-all hover:border-primary/30 hover:shadow-md active:scale-[0.98]">
								<div class={`mb-3 inline-flex h-12 w-12 items-center justify-center rounded-lg ${action.color} text-white`}>
									<action.icon class="h-6 w-6" />
								</div>
								<div class="font-medium text-base mb-1">{action.label}</div>
								<div class="text-sm text-muted-foreground">{action.description}</div>
							</button>
						{/each}
					</div>
				</section>
				
				<!-- Upcoming Events & Activity Side-by-Side on Desktop -->
				<div class="flex flex-col lg:flex-row lg:gap-8">
					<!-- Upcoming Events (Main Content) -->
					<div class="flex-1 mb-8 lg:mb-0">
						<section>
							<div class="flex items-center justify-between mb-4">
								<h2 class="text-lg font-semibold">Upcoming Movie Nights</h2>
								<Button size="sm" variant="outline">
									<Calendar class="mr-2 h-4 w-4" />
									View Calendar
								</Button>
							</div>
							
							{#if upcomingEvents.length > 0}
								<div class="space-y-4">
									{#each upcomingEvents as event}
										<Card class="group transition-all hover:shadow-lg hover:border-primary/30">
											<CardContent class="p-6">
												<div class="flex flex-col md:flex-row md:items-center justify-between gap-4">
													<div class="flex-1">
														<div class="mb-2 flex items-center gap-2">
															<Badge variant={event.status === 'voting' ? 'default' : 'outline'}>
																{event.status === 'voting' ? 'Voting Active' : 'Scheduled'}
															</Badge>
															<span class="text-sm text-muted-foreground">{event.group}</span>
														</div>
														<h3 class="mb-2 text-xl font-semibold">{event.movie}</h3>
														<div class="flex flex-wrap items-center gap-4 text-sm text-muted-foreground">
															<div class="flex items-center gap-1">
																<Calendar class="h-4 w-4" />
																{event.date}
															</div>
															{#if event.votingEnds}
																<div class="flex items-center gap-1">
																	<Clock class="h-4 w-4" />
																	Voting ends {event.votingEnds}
																</div>
															{/if}
														</div>
													</div>
													<Button size="sm" class="self-start md:self-center">
														{#if event.status === 'voting'}
															<Vote class="mr-2 h-4 w-4" />
															Vote Now
														{:else}
															View Details
														{/if}
													</Button>
												</div>
											</CardContent>
										</Card>
									{/each}
								</div>
							{:else}
								<Card class="border-dashed">
									<CardContent class="py-12 text-center">
										<Calendar class="mx-auto mb-4 h-12 w-12 text-muted-foreground" />
										<h3 class="mb-2 text-lg font-semibold">No upcoming movie nights</h3>
										<p class="mb-4 text-sm text-muted-foreground">
											Plan your first movie night with friends
										</p>
										<Button>
											<Plus class="mr-2 h-4 w-4" />
											Plan Movie Night
										</Button>
									</CardContent>
								</Card>
							{/if}
						</section>
					</div>
					
				 
				</div>
				
				 
			</div>
		</div>
		
		<!-- Mobile Bottom Navigation -->
		<nav class="fixed bottom-0 left-0 right-0 border-t border-border bg-background p-4 lg:hidden z-50">
			<div class="grid grid-cols-4 gap-4">
				<a href="/dashboard" class="flex flex-col items-center gap-1 text-primary">
					<Calendar class="h-5 w-5" />
					<span class="text-xs">Home</span>
				</a>
				<a href="/groups" class="flex flex-col items-center gap-1 text-muted-foreground">
					<Users class="h-5 w-5" />
					<span class="text-xs">Groups</span>
				</a>
				<a href="/calendar" class="flex flex-col items-center gap-1 text-muted-foreground">
					<Film class="h-5 w-5" />
					<span class="text-xs">Movies</span>
				</a>
				<a href="/activity" class="flex flex-col items-center gap-1 text-muted-foreground">
					<MessageSquare class="h-5 w-5" />
					<span class="text-xs">Activity</span>
				</a>
			</div>
		</nav>
	</main>
</div>

<style>
	/* Adjust for bottom nav */
	main {
		padding-bottom: 5rem;
	}
	
	/* Smooth transitions */
	* {
		transition: border-color 0.2s, background-color 0.2s, transform 0.2s;
	}
	
	/* Better scroll for mobile */
	@media (max-width: 1024px) {
		html {
			scroll-padding-bottom: 80px;
		}
	}
</style>