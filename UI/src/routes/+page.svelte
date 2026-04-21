<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { Card, CardContent } from '$lib/components/ui/card';
	import Footer from '@/components/common/Footer.svelte';
	import ArrowIndicator from '@/components/home/arrow-indicator.svelte';
	import movieBg from '$lib/assets/movie-bg.jpg';
	import {
		ArrowRight,
		Ban,
		Bell,
		Clock,
		MessageSquare,
		RefreshCw,
		Shield,
		Sparkles,
		Trophy,
		UserCheck,
		UserPlus,
		Users,
		Vote,
		Zap
	} from '@lucide/svelte';
	import { appState } from '../store';

	// Features - Updated to match actual app capabilities
	const features = [
		{
			icon: Users,
			title: 'Group Management',
			description: 'Create groups, send invites, and manage members with admin controls.',
			color: 'from-primary/20 to-primary/5'
		},
		{
			icon: Vote,
			title: 'Fair Voting System',
			description: 'Upvote, downvote, or veto. One veto disqualifies - everyone has a voice.',
			color: 'from-purple-500/20 to-purple-500/5'
		},
		{
			icon: Zap,
			title: 'Real-time Updates',
			description: 'See votes and chat messages instantly with SignalR-powered live updates.',
			color: 'from-yellow-500/20 to-yellow-500/5'
		},
		{
			icon: Trophy,
			title: 'Smart Decision Engine',
			description: 'Automatic winner selection with intelligent tie-breaking algorithms.',
			color: 'from-amber-500/20 to-amber-500/5'
		},
		{
			icon: MessageSquare,
			title: 'Event-Scoped Chat',
			description: 'Discuss movie choices in real-time within each movie night event.',
			color: 'from-blue-500/20 to-blue-500/5'
		},
		{
			icon: Shield,
			title: 'Secure Authentication',
			description: 'JWT-based auth with email verification and password recovery.',
			color: 'from-green-500/20 to-green-500/5'
		}
	];

	// Key capabilities - Quick highlights
	const capabilities = [
		{
			icon: UserPlus,
			title: 'Join Requests',
			description: 'Request to join groups - admins approve or reject'
		},
		{
			icon: UserCheck,
			title: 'Admin Controls',
			description: 'Promote members, manage roles, remove users'
		},
		{
			icon: Ban,
			title: 'Veto Power',
			description: 'One veto disqualifies - unveto to restore'
		},
		{
			icon: RefreshCw,
			title: 'Fallback Refresh',
			description: 'Manual refresh buttons if real-time updates miss'
		},
		{
			icon: Bell,
			title: 'Live Notifications',
			description: 'Snackbar alerts for important events'
		},
		{
			icon: Clock,
			title: 'Event Scheduling',
			description: 'Set name, description, and time for movie nights'
		}
	];
</script>

<div class="min-h-screen overflow-hidden bg-background">
	<!-- Sticky Background Container -->
	<div class="relative">
		<!-- Fixed Background Image - This stays fixed while scrolling -->
		<div
			class="fixed inset-0 z-0 h-screen w-full"
			style="background-image: url('{movieBg}'); background-size: cover; background-position: center; background-repeat: no-repeat;"
		>
			<!-- Dark overlay for better text readability -->
			<div
				class="absolute inset-0 bg-background/60 backdrop-blur-[1px] dark:bg-background/70"
			></div>
		</div>

		<!-- Hero Section - Content sits above the fixed background -->
		<section class="relative z-10 px-4 pt-20 pb-32 md:pt-32 md:pb-48">
			<div class="container mx-auto max-w-6xl">
				<div class="text-center">
					<div
						class="mb-8 inline-flex items-center gap-2 rounded-full border border-primary/20 bg-primary/5 px-4 py-1.5 backdrop-blur-sm"
					>
						<Sparkles class="h-3.5 w-3.5 text-primary" />
						<span class="text-sm font-medium text-primary">Stop the endless scrolling</span>
					</div>

					<h1
						class="mb-6 text-5xl font-bold tracking-tight text-foreground sm:text-6xl md:text-7xl lg:text-8xl"
					>
						Movie Nights,
						<span class="block text-primary">decided fairly.</span>
					</h1>

					<p class="mx-auto mb-10 max-w-2xl text-xl text-foreground/90">
						WatchHive helps groups pick movies without the drama. Vote, veto, and let our decision
						engine find the perfect film everyone can agree on.
					</p>

					<div class="mb-16 flex flex-col justify-center gap-4 sm:flex-row">
						<Button
							size="lg"
							class="h-14 rounded-2xl px-14 text-base"
							href={appState.user ? '/dashboard' : '/signup'}
						>
							Create Your Group — It's Free
							<ArrowRight class="ml-4 h-5 w-5" />
						</Button>
					</div>
					<ArrowIndicator />
				</div>
			</div>
		</section>
	</div>

	<!-- Key Capabilities - This section covers the sticky background -->
	<section class="relative z-20 bg-background px-4 py-20" id="quickOverview">
		<div class="container mx-auto max-w-6xl">
			<div class="mb-16 text-center">
				<h2 class="mb-4 text-3xl font-bold sm:text-4xl">Built for Group Harmony</h2>
				<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
					Everything you need to make group movie decisions fair and fun
				</p>
			</div>

			<div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
				{#each capabilities as capability (capability.title)}
					<div class="flex items-start gap-4 rounded-lg border border-border/50 bg-card p-4">
						<div class="rounded-lg bg-primary/10 p-2">
							<capability.icon class="h-5 w-5 text-primary" />
						</div>
						<div>
							<h3 class="font-semibold">{capability.title}</h3>
							<p class="text-sm text-muted-foreground">{capability.description}</p>
						</div>
					</div>
				{/each}
			</div>
		</div>
	</section>

	<!-- Features Grid -->
	<section id="features" class="relative z-20 bg-background px-4 py-20">
		<div class="container mx-auto max-w-6xl">
			<div class="mb-16 text-center">
				<h2 class="mb-4 text-3xl font-bold sm:text-4xl">Powerful Features</h2>
				<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
					Designed to make group movie selection effortless and democratic
				</p>
			</div>

			<div class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
				{#each features as feature (feature.title)}
					<div
						class="group relative overflow-hidden rounded-2xl border border-border bg-card p-6 transition-all duration-300 hover:scale-[1.02] hover:shadow-xl"
					>
						<div
							class={`absolute inset-0 bg-linear-to-br ${feature.color} opacity-0 transition-opacity duration-300 group-hover:opacity-100`}
						></div>
						<div class="relative">
							<div class="mb-4 inline-flex rounded-lg bg-primary/10 p-3">
								<feature.icon class="h-6 w-6 text-primary" />
							</div>
							<h3 class="mb-2 text-lg font-semibold">{feature.title}</h3>
							<p class="text-sm text-muted-foreground">{feature.description}</p>
						</div>
					</div>
				{/each}
			</div>
		</div>
	</section>

	<!-- How Decision Engine Works - Teaser -->
	<section class="relative z-20 bg-muted/30 px-4 py-20">
		<div class="container mx-auto max-w-4xl">
			<div class="text-center">
				<div class="mb-6 inline-flex rounded-full bg-primary/10 px-4 py-1.5">
					<span class="text-sm font-medium text-primary">The Secret Sauce</span>
				</div>
				<h2 class="mb-4 text-3xl font-bold sm:text-4xl">Fair Decision Making</h2>
				<p class="mx-auto mb-8 max-w-2xl text-lg text-muted-foreground">
					Our intelligent decision engine ensures everyone's voice matters
				</p>
			</div>

			<div class="grid gap-6 md:grid-cols-3">
				<Card class="border-border/50">
					<CardContent class="p-6 text-center">
						<div class="mb-3 text-2xl font-bold text-primary">+1 / -1</div>
						<h3 class="mb-2 font-semibold">Simple Voting</h3>
						<p class="text-sm text-muted-foreground">
							Upvotes increase chances, downvotes decrease them
						</p>
					</CardContent>
				</Card>
				<Card class="border-border/50">
					<CardContent class="p-6 text-center">
						<div class="mb-3 text-2xl font-bold text-destructive">Veto</div>
						<h3 class="mb-2 font-semibold">Absolute Veto</h3>
						<p class="text-sm text-muted-foreground">
							One veto disqualifies - nobody watches what they hate
						</p>
					</CardContent>
				</Card>
				<Card class="border-border/50">
					<CardContent class="p-6 text-center">
						<div class="mb-3 text-2xl font-bold text-primary">4 Tiers</div>
						<h3 class="mb-2 font-semibold">Smart Tie-Breaking</h3>
						<p class="text-sm text-muted-foreground">
							Multiple levels ensure a clear winner every time
						</p>
					</CardContent>
				</Card>
			</div>

			<div class="mt-8 text-center">
				<Button variant="outline" class="rounded-full" href="/how-it-works">
					See How It Works
					<ArrowRight class="ml-2 h-4 w-4" />
				</Button>
			</div>
		</div>
	</section>

	<!-- CTA Section -->
	<section class="relative z-20 bg-background px-4 py-20">
		<div class="container mx-auto max-w-4xl">
			<div
				class="relative overflow-hidden rounded-3xl border border-border bg-linear-to-br from-primary/5 to-secondary/5 p-8 md:p-12"
			>
				<div class="relative z-10 text-center">
					<h2 class="mb-4 text-3xl font-bold sm:text-4xl">Ready to end the movie night debates?</h2>
					<p class="mx-auto mb-8 max-w-2xl text-lg text-muted-foreground">
						Join groups already using WatchHive to pick movies fairly and efficiently.
					</p>
					<div class="flex flex-col justify-center gap-4 sm:flex-row">
						<Button
							size="lg"
							class="h-14 rounded-2xl px-8 text-base"
							href={appState.user ? '/dashboard' : '/signup'}
						>
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
