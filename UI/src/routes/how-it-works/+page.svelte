<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { Card, CardContent } from '$lib/components/ui/card';
	import Footer from '@/components/common/Footer.svelte';
	import {
		ArrowRight,
		Bell,
		Calendar,
		CheckCircle,
		MessageSquare,
		RefreshCw,
		Sparkles,
		Trophy,
		UserPlus,
		Users,
		Vote,
		Zap
	} from '@lucide/svelte';

	// Main process steps
	const mainSteps = [
		{
			number: '01',
			title: 'Create Your Account',
			description: 'Sign up with email verification to get started.',
			icon: UserPlus,
			color: 'from-blue-500/20 to-blue-500/5',
			details: [
				'Email verification for security',
				'Password recovery options',
				'JWT-based authentication'
			]
		},
		{
			number: '02',
			title: 'Join or Create a Group',
			description: 'Start a new group or request to join an existing one.',
			icon: Users,
			color: 'from-primary/20 to-primary/5',
			details: [
				'Create groups as admin',
				'Send and receive invitations',
				'Request to join existing groups',
				'Admin approval for new members'
			]
		},
		{
			number: '03',
			title: 'Schedule a Movie Night',
			description: 'Admins create events with name, description, and time.',
			icon: Calendar,
			color: 'from-green-500/20 to-green-500/5',
			details: [
				'Set event name and description',
				'Schedule date and time',
				'Each event has dedicated chat'
			]
		},
		{
			number: '04',
			title: 'Suggest & Vote',
			description: 'Everyone suggests movies and votes in real-time.',
			icon: Vote,
			color: 'from-purple-500/20 to-purple-500/5',
			details: [
				'One suggestion per member',
				'Real-time voting updates',
				'Veto power to block unwanted movies',
				'Unveto option available'
			]
		},
		{
			number: '05',
			title: 'Pick the Winner',
			description: 'Admin computes results and winner is announced.',
			icon: Trophy,
			color: 'from-amber-500/20 to-amber-500/5',
			details: [
				'Automatic score calculation',
				'Smart tie-breaking system',
				'Everyone sees results instantly'
			]
		}
	];

	// Voting system details
	const votingMechanics = [
		{
			action: 'Upvote',
			icon: '👍',
			effect: '+1 point',
			description: 'Support a movie you want to watch',
			color: 'text-green-500'
		},
		{
			action: 'Downvote',
			icon: '👎',
			effect: '-1 point',
			description: 'Vote against a movie',
			color: 'text-red-500'
		},
		{
			action: 'Veto',
			icon: '🚫',
			effect: 'Disqualifies',
			description: 'Block a movie completely',
			color: 'text-destructive'
		}
	];

	// Tie-breaking rules in order
	const tieBreakers = [
		{
			level: 1,
			title: 'Highest Upvote Count',
			description: 'The movie with the most upvotes wins - popularity matters!',
			icon: '📈'
		},
		{
			level: 2,
			title: 'Lowest Downvote Count',
			description: 'If still tied, the least disliked movie takes it.',
			icon: '📉'
		},
		{
			level: 3,
			title: 'TMDB Rating',
			description: 'Critically acclaimed movies get the edge.',
			icon: '⭐'
		},
		{
			level: 4,
			title: 'First Suggested',
			description: 'The earliest suggestion wins if all else is equal.',
			icon: '⏰'
		}
	];

	// Group roles and permissions
	const roles = [
		{
			role: 'Group Creator',
			permissions: [
				'Full admin control',
				'Cannot be demoted by other admins',
				'Manage all group settings'
			],
			icon: '👑'
		},
		{
			role: 'Admin',
			permissions: [
				'Approve/reject join requests',
				'Create movie night events',
				'Promote members to admin',
				'Remove members from group',
				'Compute final results'
			],
			icon: '⭐'
		},
		{
			role: 'Member',
			permissions: [
				'Suggest one movie per event',
				'Vote on all suggestions',
				'Use veto power once',
				'Chat in event discussions'
			],
			icon: '👤'
		}
	];
</script>

<div class="min-h-screen bg-background">
	<!-- Hero Section -->
	<section class="relative px-4 pt-20 pb-16 md:pt-32 md:pb-24">
		<div class="absolute inset-0 -z-10 overflow-hidden">
			<div class="absolute -top-40 -right-40 h-80 w-80 rounded-full bg-primary/10 blur-3xl"></div>
			<div
				class="absolute -bottom-40 -left-40 h-80 w-80 rounded-full bg-secondary/10 blur-3xl"
			></div>
		</div>

		<div class="container mx-auto max-w-6xl">
			<div class="text-center">
				<div
					class="mb-8 inline-flex items-center gap-2 rounded-full border border-primary/20 bg-primary/5 px-4 py-1.5"
				>
					<Sparkles class="h-3.5 w-3.5 text-primary" />
					<span class="text-sm font-medium text-primary">From Chaos to Consensus</span>
				</div>

				<h1 class="mb-6 text-5xl font-bold tracking-tight sm:text-6xl lg:text-7xl">
					How WatchHive
					<span class="block text-primary">Works</span>
				</h1>

				<p class="mx-auto max-w-2xl text-xl text-muted-foreground">
					A simple, fair system to help groups pick movies everyone can enjoy. No more endless
					debates or compromise fatigue.
				</p>
			</div>
		</div>
	</section>

	<!-- Main Process Timeline -->
	<section class="px-4 py-16">
		<div class="container mx-auto max-w-5xl">
			<div class="relative">
				<!-- Vertical timeline line -->
				<div
					class="absolute top-0 bottom-0 left-8 w-0.5 bg-border md:left-1/2 md:-translate-x-1/2"
				></div>

				<div class="space-y-12">
					{#each mainSteps as step, index (step.number)}
						<div
							class={`relative flex flex-col gap-8 md:flex-row ${
								index % 2 === 0 ? 'md:flex-row' : 'md:flex-row-reverse'
							}`}
						>
							<!-- Timeline dot -->
							<div
								class="absolute left-8 z-10 flex h-12 w-12 -translate-x-1/2 items-center justify-center rounded-full border-4 border-background bg-primary text-primary-foreground md:left-1/2"
							>
								<span class="text-sm font-bold">{step.number}</span>
							</div>

							<!-- Content -->
							<div class={`ml-20 md:ml-0 md:w-1/2 ${index % 2 === 0 ? 'md:pr-12' : 'md:pl-12'}`}>
								<Card
									class="h-full overflow-hidden border-border/50 transition-all hover:shadow-lg"
								>
									<CardContent class="p-6">
										<div class="mb-4 inline-flex rounded-lg bg-primary/10 p-3">
											<step.icon class="h-6 w-6 text-primary" />
										</div>
										<h3 class="mb-3 text-xl font-semibold">{step.title}</h3>
										<p class="mb-4 text-muted-foreground">{step.description}</p>
										<ul class="space-y-2">
											{#each step.details as detail (detail)}
												<li class="flex items-start gap-2 text-sm">
													<CheckCircle class="mt-0.5 h-4 w-4 shrink-0 text-primary" />
													<span class="text-muted-foreground">{detail}</span>
												</li>
											{/each}
										</ul>
									</CardContent>
								</Card>
							</div>
						</div>
					{/each}
				</div>
			</div>
		</div>
	</section>

	<!-- Voting System Deep Dive -->
	<section class="bg-muted/30 px-4 py-20">
		<div class="container mx-auto max-w-6xl">
			<div class="mb-16 text-center">
				<h2 class="mb-4 text-3xl font-bold sm:text-4xl">The Voting System</h2>
				<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
					Every voice matters. Our voting system ensures fair representation for everyone.
				</p>
			</div>

			<!-- Voting mechanics cards -->
			<div class="mb-16 grid gap-6 md:grid-cols-3">
				{#each votingMechanics as mechanic (mechanic.description)}
					<Card class="border-border/50">
						<CardContent class="p-6 text-center">
							<div class="mb-4 text-5xl">{mechanic.icon}</div>
							<h3 class="mb-2 text-xl font-semibold">{mechanic.action}</h3>
							<div class={`mb-2 text-2xl font-bold ${mechanic.color}`}>
								{mechanic.effect}
							</div>
							<p class="text-sm text-muted-foreground">{mechanic.description}</p>
						</CardContent>
					</Card>
				{/each}
			</div>

			<!-- Score calculation -->
			<Card class="mb-12 border-border/50 bg-primary/5">
				<CardContent class="p-8">
					<div class="text-center">
						<h3 class="mb-4 text-2xl font-bold">How Scores Are Calculated</h3>
						<div class="inline-block rounded-lg bg-background px-8 py-4">
							<code class="font-mono text-xl"> Score = Upvotes - Downvotes </code>
						</div>
						<p class="mt-4 text-muted-foreground">
							Vetoed movies are automatically disqualified regardless of score
						</p>
					</div>
				</CardContent>
			</Card>

			<!-- Tie-breaking rules -->
			<div class="text-center">
				<h3 class="mb-8 text-2xl font-bold">Tie-Breaking Rules</h3>
				<div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
					{#each tieBreakers as rule (rule.title)}
						<Card class="border-border/50">
							<CardContent class="p-6 text-center">
								<div class="mb-3 text-4xl">{rule.icon}</div>
								<div
									class="mb-2 inline-block rounded-full bg-primary/10 px-3 py-1 text-xs font-semibold text-primary"
								>
									Level {rule.level}
								</div>
								<h4 class="mb-2 font-semibold">{rule.title}</h4>
								<p class="text-sm text-muted-foreground">{rule.description}</p>
							</CardContent>
						</Card>
					{/each}
				</div>
			</div>
		</div>
	</section>

	<!-- Roles and Permissions -->
	<section class="px-4 py-20">
		<div class="container mx-auto max-w-6xl">
			<div class="mb-16 text-center">
				<h2 class="mb-4 text-3xl font-bold sm:text-4xl">Group Roles & Permissions</h2>
				<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
					Clear roles ensure smooth group management and fair decision-making
				</p>
			</div>

			<div class="grid gap-6 md:grid-cols-3">
				{#each roles as role (role.role)}
					<Card class="border-border/50">
						<CardContent class="p-6">
							<div class="mb-4 text-center">
								<div class="mb-3 text-5xl">{role.icon}</div>
								<h3 class="text-xl font-semibold">{role.role}</h3>
							</div>
							<ul class="space-y-2">
								{#each role.permissions as permission (permission)}
									<li class="flex items-start gap-2 text-sm">
										<CheckCircle class="mt-0.5 h-4 w-4 shrink-0 text-primary" />
										<span class="text-muted-foreground">{permission}</span>
									</li>
								{/each}
							</ul>
						</CardContent>
					</Card>
				{/each}
			</div>
		</div>
	</section>

	<!-- Real-time Features -->
	<section class="bg-muted/30 px-4 py-20">
		<div class="container mx-auto max-w-4xl">
			<div class="mb-12 text-center">
				<h2 class="mb-4 text-3xl font-bold sm:text-4xl">Powered by Real-Time Updates</h2>
				<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
					SignalR keeps everyone in sync instantly
				</p>
			</div>

			<div class="grid gap-6 sm:grid-cols-2">
				<Card class="border-border/50">
					<CardContent class="p-6">
						<div class="mb-4 flex items-center gap-3">
							<div class="rounded-lg bg-blue-500/10 p-2">
								<Zap class="h-5 w-5 text-blue-500" />
							</div>
							<h3 class="font-semibold">Live Voting</h3>
						</div>
						<p class="text-sm text-muted-foreground">
							See votes appear instantly as group members cast them. No refresh needed.
						</p>
					</CardContent>
				</Card>

				<Card class="border-border/50">
					<CardContent class="p-6">
						<div class="mb-4 flex items-center gap-3">
							<div class="rounded-lg bg-green-500/10 p-2">
								<MessageSquare class="h-5 w-5 text-green-500" />
							</div>
							<h3 class="font-semibold">Event Chat</h3>
						</div>
						<p class="text-sm text-muted-foreground">
							Discuss movie choices in real-time within each event's dedicated chat room.
						</p>
					</CardContent>
				</Card>

				<Card class="border-border/50">
					<CardContent class="p-6">
						<div class="mb-4 flex items-center gap-3">
							<div class="rounded-lg bg-amber-500/10 p-2">
								<Bell class="h-5 w-5 text-amber-500" />
							</div>
							<h3 class="font-semibold">Instant Notifications</h3>
						</div>
						<p class="text-sm text-muted-foreground">
							Get snackbar alerts when winners are selected or important events occur.
						</p>
					</CardContent>
				</Card>

				<Card class="border-border/50">
					<CardContent class="p-6">
						<div class="mb-4 flex items-center gap-3">
							<div class="rounded-lg bg-purple-500/10 p-2">
								<RefreshCw class="h-5 w-5 text-purple-500" />
							</div>
							<h3 class="font-semibold">Fallback Refresh</h3>
						</div>
						<p class="text-sm text-muted-foreground">
							Manual refresh buttons throughout the app ensure you never miss updates.
						</p>
					</CardContent>
				</Card>
			</div>
		</div>
	</section>

	<!-- Example Flow -->
	<section class="px-4 py-20">
		<div class="container mx-auto max-w-4xl">
			<div class="mb-12 text-center">
				<h2 class="mb-4 text-3xl font-bold sm:text-4xl">A Real Example</h2>
				<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
					See how WatchHive turns chaos into consensus
				</p>
			</div>

			<Card class="border-border/50 bg-linear-to-br from-primary/5 to-secondary/5">
				<CardContent class="p-8">
					<div class="space-y-6">
						<div class="flex items-start gap-4">
							<div
								class="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/20 text-sm font-bold text-primary"
							>
								1
							</div>
							<div>
								<p class="font-medium">
									Sarah creates a group "Friday Movie Club" and invites 5 friends
								</p>
								<p class="text-sm text-muted-foreground">They all accept and join the group</p>
							</div>
						</div>

						<div class="flex items-start gap-4">
							<div
								class="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/20 text-sm font-bold text-primary"
							>
								2
							</div>
							<div>
								<p class="font-medium">Sarah schedules a movie night for Friday at 8 PM</p>
								<p class="text-sm text-muted-foreground">"Friday Night Flicks - Sci-Fi Edition"</p>
							</div>
						</div>

						<div class="flex items-start gap-4">
							<div
								class="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/20 text-sm font-bold text-primary"
							>
								3
							</div>
							<div>
								<p class="font-medium">Everyone suggests one movie each:</p>
								<ul class="mt-2 space-y-1 text-sm text-muted-foreground">
									<li>• Alex suggests "Dune" (8 upvotes, 1 downvote)</li>
									<li>• Maria suggests "Inception" (7 upvotes, 2 downvotes)</li>
									<li>• James suggests "The Matrix" (6 upvotes, 1 downvote)</li>
									<li>• Lisa suggests "Interstellar" - but Tom vetos it (disqualified)</li>
								</ul>
							</div>
						</div>

						<div class="flex items-start gap-4">
							<div
								class="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/20 text-sm font-bold text-primary"
							>
								4
							</div>
							<div>
								<p class="font-medium">Sarah hits "Compute Results"</p>
								<p class="text-sm text-muted-foreground">
									"Dune" wins with a score of 7 (8 upvotes - 1 downvote)
								</p>
							</div>
						</div>

						<div class="flex items-start gap-4">
							<div
								class="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/20 text-sm font-bold text-primary"
							>
								5
							</div>
							<div>
								<p class="font-medium">
									Everyone gets a notification: "🎬 Dune has been selected!"
								</p>
								<p class="text-sm text-muted-foreground">
									The event is locked, and they're ready for Friday
								</p>
							</div>
						</div>
					</div>
				</CardContent>
			</Card>
		</div>
	</section>

	<!-- CTA Section -->
	<section class="px-4 py-20">
		<div class="container mx-auto max-w-4xl">
			<div
				class="relative overflow-hidden rounded-3xl border border-border bg-linear-to-br from-primary/5 to-secondary/5 p-8 md:p-12"
			>
				<div class="relative z-10 text-center">
					<h2 class="mb-4 text-3xl font-bold sm:text-4xl">Ready to Try It Yourself?</h2>
					<p class="mx-auto mb-8 max-w-2xl text-lg text-muted-foreground">
						Create your first group and experience fair movie selection today.
					</p>
					<div class="flex flex-col justify-center gap-4 sm:flex-row">
						<Button size="lg" class="h-14 rounded-2xl px-8 text-base" href="/signup">
							Get Started Free
							<ArrowRight class="ml-2 h-5 w-5" />
						</Button>
					</div>
				</div>
			</div>
		</div>
	</section>

	<Footer />
</div>

<style>
	/* Custom scrollbar */
	::-webkit-scrollbar {
		width: 10px;
	}

	::-webkit-scrollbar-track {
		background: hsl(var(--background));
	}

	::-webkit-scrollbar-thumb {
		background: hsl(var(--muted));
		border-radius: 5px;
	}

	::-webkit-scrollbar-thumb:hover {
		background: hsl(var(--muted-foreground));
	}
</style>
