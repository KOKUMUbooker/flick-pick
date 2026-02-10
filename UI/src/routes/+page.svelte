<script lang="ts">
	import {
		ArrowRight,
		Users,
		Calendar,
		Vote,
		Film,
		Sparkles,
		Play,
		CheckCircle,
		Clock,
		Zap,
		Shield,
		MessageSquare,
		ChevronRight,
		Star,

		TrendingUp,

		Timer


	} from '@lucide/svelte';
	import { Button } from '$lib/components/ui/button';
	import { Card, CardContent } from '$lib/components/ui/card';
	import Footer from '@/components/common/Footer.svelte';

	// Stats
	const stats = [
		{ label: 'Groups Created', value: '1,234+' },
		{ label: 'Movies Planned', value: '8,912' },
		{ label: 'Hours Saved', value: '5,678+' },
		{ label: 'Happy Users', value: '4,321' }
	];

	// Features

	const features = [
		{
			icon: Vote,
			title: 'Smart Voting',
			description: 'Vote thumbs up/down with veto power. See real-time results and rankings.',
			color: 'from-primary/20 to-primary/5'
		},
		{
			icon: Film,
			title: 'Movie Discovery',
			description: 'Search millions of movies with streaming availability and ratings.',
			color: 'from-purple-500/20 to-purple-500/5'
		},
		{
			icon: Calendar,
			title: 'Easy Scheduling',
			description: 'Find times that work for everyone and send calendar invites.',
			color: 'from-green-500/20 to-green-500/5'
		},
		{
			icon: MessageSquare,
			title: 'Post-Watch Chat',
			description: 'Discuss movies after watching with spoiler-safe threads.',
			color: 'from-blue-500/20 to-blue-500/5'
		},
		{
			icon: Star,
			title: 'Group Ratings',
			description: 'Rate movies together and see how your tastes align over time.',
			color: 'from-yellow-500/20 to-yellow-500/5'
		},
		{
			icon: Timer,
			title: 'Watch Tracking',
			description: 'Mark when you start watching and build a group watch history.',
			color: 'from-orange-500/20 to-orange-500/5'
		}
	];
	

	// How it works steps
	const steps = [
		{
			number: '01',
			title: 'Create & Invite',
			description: 'Start a group and invite friends in seconds.',
			icon: Users,
			details: 'No sign-up needed for friends to vote'
		},
		{
			number: '02',
			title: 'Suggest & Vote',
			description: 'Add movies and vote together in real-time.',
			icon: Vote,
			details: 'Veto power ensures no bad choices'
		},
		{
			number: '03',
			title: 'Pick & Schedule',
			description: 'See the winner and schedule your movie night.',
			icon: TrendingUp,
			details: 'Check streaming availability for everyone'
		},
		{
			number: '04',
			title: 'Watch & Discuss',
			description: 'Track your watch and chat about it after.',
			icon: MessageSquare,
			details: 'Spoiler-safe discussions and group ratings'
		}
	];
 
</script>

<div class="min-h-screen overflow-hidden bg-background">
	<!-- Hero Section -->
	<section class="relative px-4 pt-20 pb-32 md:pt-32 md:pb-48">
		<!-- Background effects -->
		<div class="absolute inset-0 -z-10 overflow-hidden">
			<div class="absolute -top-40 -right-40 h-80 w-80 rounded-full bg-primary/10 blur-3xl" />
			<div class="absolute -bottom-40 -left-40 h-80 w-80 rounded-full bg-secondary/10 blur-3xl" />
		</div>

		<div class="container mx-auto max-w-6xl">
			<div class="text-center">
				<div
					class="mb-8 inline-flex items-center gap-2 rounded-full border border-primary/20 bg-primary/5 px-4 py-1.5"
				>
					<Sparkles class="h-3.5 w-3.5 text-primary" />
					<span class="text-sm font-medium text-primary">No more endless group chats</span>
				</div>

				<h1 class="mb-6 text-5xl font-bold tracking-tight sm:text-6xl md:text-7xl lg:text-8xl">
					Movie nights,
					<span class="block text-primary">simplified.</span>
				</h1>

				<p class="mx-auto mb-10 max-w-2xl text-xl text-muted-foreground">
					Stop debating what to watch. GroupWatch helps friends plan movie nights with voting,
					scheduling, and streaming checks—all in one place.
				</p>

				<div class="mb-16 flex flex-col justify-center gap-4 sm:flex-row">
					<Button size="lg" class="h-14 rounded-full px-8 text-base">
						Start Your First Group — It's Free
						<ArrowRight class="ml-2 h-5 w-5" />
					</Button>
					<!-- <Button size="lg" variant="outline" class="h-14 rounded-full px-8">
						<Play class="mr-2 h-5 w-5" />
						Watch Demo
					</Button> -->
				</div>

				<!-- Stats -->
				<div class="mx-auto grid max-w-3xl grid-cols-2 gap-6 md:grid-cols-4">
					{#each stats as stat}
						<div class="text-center">
							<div class="mb-1 text-3xl font-bold text-foreground">{stat.value}</div>
							<div class="text-sm text-muted-foreground">{stat.label}</div>
						</div>
					{/each}
				</div>
			</div>
		</div>
	</section>

	<!-- How It Works -->
	<section id="how-it-works" class="px-4 py-20">
		<div class="container mx-auto max-w-6xl">
			<div class="mb-16 text-center">
				<h2 class="mb-4 text-3xl font-bold sm:text-4xl">How GroupWatch Works</h2>
				<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
					Four simple steps from planning to pressing play
				</p>
			</div>

			<div class="relative">
				<!-- Connecting line -->
				<div
					class="absolute top-1/2 left-1/2 hidden h-0.5 w-full -translate-x-1/2 -translate-y-1/2 bg-border lg:block"
				/>

				<div class="grid gap-8 lg:grid-cols-4">
					{#each steps as step, i}
						<div class="relative">
							<Card
								class={`h-full border-border/50 transition-colors duration-300 hover:border-primary/30 ${i % 2 === 0 ? 'lg:mt-8' : 'lg:-mt-8'}`}
							>
								<CardContent class="p-8">
									<div
										class="mb-6 flex h-12 w-12 items-center justify-center rounded-full bg-primary/10 text-primary"
									>
										<step.icon class="h-6 w-6" />
									</div>
									<div class="mb-2 text-sm font-semibold text-primary">{step.number}</div>
									<h3 class="mb-3 text-xl font-semibold">{step.title}</h3>
									<p class="text-muted-foreground">{step.description}</p>
								</CardContent>
							</Card>
						</div>
					{/each}
				</div>
			</div>
		</div>
	</section>

	<!-- Features Grid -->
	<section id="features" class="bg-muted/30 px-4 py-20">
		<div class="container mx-auto max-w-6xl">
			<div class="mb-16 text-center">
				<h2 class="mb-4 text-3xl font-bold sm:text-4xl">Everything You Need</h2>
				<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
					Features designed to make group movie nights effortless
				</p>
			</div>

			<div class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
				{#each features as feature}
					<div
						class="group relative overflow-hidden rounded-2xl border border-border bg-card p-6 transition-all duration-300 hover:scale-[1.02] hover:shadow-xl"
					>
						<div
							class={`absolute inset-0 bg-gradient-to-br ${feature.color} opacity-0 transition-opacity duration-300 group-hover:opacity-100`}
						/>
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

	<!-- CTA Section -->
	<section class="px-4 py-20">
		<div class="container mx-auto max-w-4xl">
			<div
				class="relative overflow-hidden rounded-3xl border border-border bg-gradient-to-br from-primary/5 to-secondary/5 p-8 md:p-12"
			>
				<div class="relative z-10 text-center">
					<h2 class="mb-4 text-3xl font-bold sm:text-4xl">Ready to watch with friends?</h2>
					<p class="mx-auto mb-8 max-w-2xl text-lg text-muted-foreground">
						Join thousands of groups enjoying stress-free movie nights.
					</p>
					<div class="flex flex-col justify-center gap-4 sm:flex-row">
						<Button size="lg" class="h-14 rounded-full px-8 text-base">
							Get Started Free
							<ArrowRight class="ml-2 h-5 w-5" />
						</Button>
						<Button size="lg" variant="outline" class="h-14 rounded-full px-8">
							Schedule a Demo
						</Button>
					</div>
					<p class="mt-6 text-sm text-muted-foreground">
						No credit card required • Free forever for up to 10 members
					</p>
				</div>
			</div>
		</div>
	</section>

	<Footer />
</div>

<style>
	/* Smooth scrolling */
	html {
		scroll-behavior: smooth;
	}

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
