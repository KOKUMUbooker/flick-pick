<script lang="ts">
	import { Badge } from '$lib/components/ui/badge';
	import { Button } from '$lib/components/ui/button';
	import { Card, CardContent, CardHeader, CardTitle } from '$lib/components/ui/card';
	import Footer from '@/components/common/Footer.svelte';
	import {
		ArrowRight,
		Calendar,
		CheckCircle,
		Clock,
		Film,
		MessageSquare,
		Play,
		Plus,
		Search,
		Share2,
		Sparkles,
		Star,
		ThumbsDown,
		ThumbsUp,
		Timer,
		TrendingUp,
		Users,
		Vote,
		XCircle,
		Zap
	} from '@lucide/svelte';

	// Active step state
	let activeStep = 0;

	// UPDATED STEPS: Now 4-step focused process
	const steps = [
		{
			id: 'create',
			title: 'Create & Invite',
			description: 'Start a group and invite friends',
			icon: Users,
			color: 'from-blue-500/20 to-blue-500/5',
			details: [
				'Create a group with a custom name and description',
				'Invite friends via shareable link or email',
				'Friends can vote instantly—no sign-up required',
				'Set group preferences for voting rules'
			]
		},
		{
			id: 'suggest',
			title: 'Suggest & Vote',
			description: 'Add movies and vote together',
			icon: Film,
			color: 'from-primary/20 to-primary/5',
			details: [
				'Search millions of movies from TMDB database',
				'Add movies with posters and streaming availability',
				'Vote thumbs up/down with real-time updates',
				'One veto power per person to rule out absolute no-gos'
			]
		},
		{
			id: 'decide',
			title: 'Make The Call',
			description: 'Pick the winner and schedule',
			icon: TrendingUp,
			color: 'from-green-500/20 to-green-500/5',
			details: [
				'See automatic winner based on votes and vetoes',
				'Check what streaming services everyone has access to',
				'Schedule a time that works for the group',
				'Send calendar invites with one click'
			]
		},
		{
			id: 'connect',
			title: 'Watch & Discuss',
			description: 'Track and chat about your movie',
			icon: MessageSquare,
			color: 'from-purple-500/20 to-purple-500/5',
			details: [
				'Mark when you actually start watching',
				'Post-watch discussion threads with spoiler tags',
				'Rate the movie together (group average shown)',
				'Save your watch history and plan the next one'
			]
		}
	];

	// UPDATED voting system - simpler, more focused
	const votingSystem = {
		upvote: {
			icon: ThumbsUp,
			title: 'Thumbs Up',
			description: 'Show you want to watch it',
			color: 'text-green-500 bg-green-500/10',
			note:""
		},
		downvote: {
			icon: ThumbsDown,
			title: 'Thumbs Down',
			description: 'Prefer other options',
			color: 'text-red-500 bg-red-500/10',
			note:""
		},
		veto: {
			icon: XCircle,
			title: 'Veto Power',
			description: 'Absolute no for one movie',
			color: 'text-orange-500 bg-orange-500/10',
			note: 'Each member gets one veto per voting session'
		}
	};

	// UPDATED FAQs - focused on decision-making only
	const faqs = [
		{
			question: 'Do friends need an account to vote?',
			answer:
				'No! Friends can vote via invite link without creating an account. Only the group creator needs an account to manage the group.'
		},
		{
			question: 'How does the veto system work?',
			answer: 'Each member gets one veto per voting session. A vetoed movie is immediately removed from consideration, ensuring nobody has to watch something they absolutely dislike.'
		},
		{
			question: 'Can we see what streaming services everyone has?',
			answer: 'Yes! Members can indicate which services they have (Netflix, Hulu, Prime, etc.), and you can filter movies to show only those available to everyone in the group.'
		},
		{
			question: 'What happens after we pick a movie?',
			answer: 'You can schedule a watch time, mark when you actually start watching, and then come back for post-movie discussions and ratings. We track your group\'s watch history over time.'
		},
		{
			question: 'Is there a limit to group size?',
			answer: 'Free groups support up to 15 members. Larger groups and advanced features are available in our Pro plan.'
		},
		{
			question: 'How do post-movie discussions work?',
			answer: 'After marking a movie as watched, a discussion thread opens automatically with spoiler/non-spoiler sections. Everyone can share thoughts and rate the movie.'
		}
	];

	// UPDATED post-watch features
	const postWatchFeatures = [
		{
			icon: Timer,
			title: 'Track Your Watch',
			description: 'Simple "We\'re watching now!" button records start time and estimates end based on movie runtime.'
		},
		{
			icon: MessageSquare,
			title: 'Spoiler-Safe Discussions',
			description: 'Separate threads for spoiler and non-spoiler conversations. Tag spoilers to protect those who haven\'t watched yet.'
		},
		{
			icon: Star,
			title: 'Group Ratings',
			description: 'Rate movies 1-10 individually, see group average, and track how your tastes align over time.'
		},
		{
			icon: Clock,
			title: 'Watch History',
			description: 'See all movies your group has watched together, with ratings and discussion highlights.'
		}
	];

	function nextStep() {
		activeStep = (activeStep + 1) % steps.length;
	}

	function prevStep() {
		activeStep = (activeStep - 1 + steps.length) % steps.length;
	}
</script>

<div class="min-h-screen bg-background">
	<!-- Hero Section - UPDATED messaging -->
	<section class="relative px-4 pt-16 pb-24">
		<div class="absolute inset-0 -z-10 overflow-hidden">
			<div class="absolute -top-40 right-1/4 h-64 w-64 rounded-full bg-primary/10 blur-3xl" />
			<div class="absolute -bottom-40 left-1/4 h-64 w-64 rounded-full bg-secondary/10 blur-3xl" />
		</div>

		<div class="container mx-auto max-w-6xl text-center">
			<Badge class="mb-6 border-primary/20 bg-primary/10 px-4 py-1.5 text-sm text-primary">
				<Sparkles class="mr-1.5 h-3.5 w-3.5" />
				How It Works
			</Badge>

			<h1 class="mb-6 text-4xl font-bold tracking-tight sm:text-5xl md:text-6xl">
				From endless debates <br />
				<span class="text-primary">to "Let's watch!" in 5 minutes</span>
			</h1>

			<p class="mx-auto mb-10 max-w-3xl text-xl text-muted-foreground">
				GroupWatch solves the hardest part of movie nights: deciding what to watch. 
				Our smart voting system gets your group from planning to watching—fast.
			</p>

			<div class="flex justify-center gap-4">
				<Button size="lg" class="rounded-full">
					Start Your First Group
					<ArrowRight class="ml-2 h-5 w-5" />
				</Button>
			</div>
		</div>
	</section>

	<!--  Simpler 4-Step Process -->
	<section class="px-4 py-12">
		<div class="container mx-auto max-w-6xl">
			<div class="mb-12 text-center">
				<h2 class="mb-4 text-3xl font-bold">The 4-Step Solution</h2>
				<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
					No complex sync, no legal headaches—just smart coordination for real movie nights
				</p>
			</div>

 			<div class="mb-12">
				<div class="flex flex-wrap justify-center gap-4">
					{#each steps as step, i}
						<button
							onclick={() => (activeStep = i)}
							class={`flex items-center gap-3 rounded-xl px-6 py-4 transition-all duration-300 ${activeStep === i ? 'bg-primary text-primary-foreground shadow-lg' : 'bg-card hover:bg-muted'}`}
						>
							<step.icon class="h-5 w-5" />
							<div class="text-left">
								<div class="text-sm font-medium">Step {i + 1}</div>
								<div class="font-semibold">{step.title}</div>
							</div>
						</button>
					{/each}
				</div>
			</div>

			<!-- Active Step Content -->
			<div class="grid items-center gap-8 lg:grid-cols-2">
				<div class="space-y-6">
					<div class="inline-flex items-center gap-2 rounded-full bg-primary/10 px-4 py-2">
						<div
							class="flex h-8 w-8 items-center justify-center rounded-full bg-primary text-primary-foreground"
						>
							{activeStep + 1}
						</div>
						<span class="font-semibold text-primary">{steps[activeStep].title}</span>
					</div>

					<h3 class="text-3xl font-bold">{steps[activeStep].description}</h3>

					<ul class="space-y-4">
						{#each steps[activeStep].details as detail}
							<li class="flex items-start gap-3">
								<CheckCircle class="mt-0.5 h-5 w-5 flex-shrink-0 text-primary" />
								<span>{detail}</span>
							</li>
						{/each}
					</ul>

					<div class="flex gap-4 pt-4">
						<Button onclick={prevStep} variant="outline">Previous Step</Button>
						<Button onclick={nextStep}>
							Next: {steps[(activeStep + 1) % steps.length].title}
							<ArrowRight class="ml-2 h-4 w-4" />
						</Button>
					</div>
				</div>

				<div class="relative">
					<div
						class={`absolute inset-0 rounded-3xl ${steps[activeStep].color} opacity-50 blur-2xl`}
					/>
					<div class="relative rounded-2xl border border-border bg-card p-8 shadow-xl">
						{#if activeStep === 0}
							<!-- Create Group Mockup -->
							<div class="space-y-6">
								<div class="text-center">
									<div class="mx-auto mb-4 inline-flex h-16 w-16 items-center justify-center rounded-full bg-primary/10">
										<Users class="h-8 w-8 text-primary" />
									</div>
									<h4 class="mb-2 text-xl font-bold">Friday Film Club</h4>
									<p class="text-sm text-muted-foreground">3 members • Created just now</p>
								</div>
								
								<div class="space-y-3">
									<div class="flex items-center justify-between rounded-lg border border-border p-4">
										<div class="flex items-center gap-3">
											<div class="h-10 w-10 rounded-full bg-primary/20"></div>
											<div>
												<div class="font-medium">Alex (You)</div>
												<div class="text-xs text-muted-foreground">Group Admin</div>
											</div>
										</div>
									</div>
									
									<div class="rounded-lg border border-dashed border-primary/30 p-4 text-center">
										<Plus class="mx-auto mb-2 h-6 w-6 text-primary" />
										<div class="mb-2 font-medium">Invite Friends</div>
										<Button size="sm" class="w-full">
											<Share2 class="mr-2 h-4 w-4" />
											Copy Invite Link
										</Button>
									</div>
								</div>
							</div>
						{:else if activeStep === 1}
							<!-- Suggest & Vote Mockup -->
							<div class="space-y-4">
								<!-- Search bar -->
								<div class="flex gap-2">
									<div class="relative flex-1">
										<Search class="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
										<input 
											type="text" 
											placeholder="Search movies..." 
											class="w-full rounded-lg border border-border bg-background pl-10 pr-4 py-2 text-sm"
										/>
									</div>
									<Button size="sm">Search</Button>
								</div>
								
								<!-- Voting cards -->
								<div class="space-y-3">
									{#each [{ title: 'Dune: Part Two', votes: 5, vetoed: false, streaming: ['Netflix', 'Prime'] }, { title: 'Poor Things', votes: 3, vetoed: false, streaming: ['Hulu'] }, { title: 'Past Lives', votes: 1, vetoed: true, streaming: ['Netflix', 'Hulu'] }] as movie}
										<div class={`rounded-lg border p-4 ${movie.vetoed ? 'border-destructive/30 bg-destructive/5' : 'border-border'}`}>
											<div class="flex items-center justify-between mb-3">
												<span class="font-medium">{movie.title}</span>
												{#if movie.vetoed}
													<Badge variant="destructive">Vetoed</Badge>
												{:else}
													<div class="flex items-center gap-2">
														<ThumbsUp class="h-4 w-4 text-green-500" />
														<span class="font-semibold">{movie.votes}</span>
													</div>
												{/if}
											</div>
											<div class="flex items-center justify-between">
												<div class="flex gap-1">
													{#each movie.streaming as service}
														<Badge variant="outline">{service}</Badge>
													{/each}
												</div>
												<div class="flex gap-1">
													<Button size="sm" variant="outline" class="h-8 w-8 p-0">
														<ThumbsUp class="h-4 w-4" />
													</Button>
													<Button size="sm" variant="outline" class="h-8 w-8 p-0">
														<ThumbsDown class="h-4 w-4" />
													</Button>
													<Button size="sm" variant="outline" class="h-8 w-8 p-0">
														<XCircle class="h-4 w-4" />
													</Button>
												</div>
											</div>
										</div>
									{/each}
								</div>
							</div>
						{:else if activeStep === 2}
							<!-- Make The Call Mockup -->
							<div class="space-y-6">
								<!-- Winner announcement -->
								<div class="rounded-lg border border-primary/30 bg-primary/5 p-6 text-center">
									<div class="mx-auto mb-4 inline-flex h-12 w-12 items-center justify-center rounded-full bg-primary">
										<TrendingUp class="h-6 w-6 text-primary-foreground" />
									</div>
									<h4 class="mb-2 text-xl font-bold">Winner Selected!</h4>
									<p class="text-muted-foreground">Dune: Part Two</p>
									<div class="mt-4 text-sm">
										<div class="text-muted-foreground">Available on: <span class="font-medium">Netflix, Prime</span></div>
										<div class="text-muted-foreground">Runtime: <span class="font-medium">2h 46m</span></div>
									</div>
								</div>
								
								<!-- Schedule options -->
								<div class="space-y-4">
									<h5 class="font-semibold">Pick a Time</h5>
									<div class="grid grid-cols-3 gap-2">
										{#each ['Fri 8 PM', 'Sat 3 PM', 'Sun 7 PM'] as time}
											<Button variant="outline" class="h-auto py-3">
												<Calendar class="mr-2 h-4 w-4" />
												{time}
											</Button>
										{/each}
									</div>
									
									<Button class="w-full">
										<Calendar class="mr-2 h-4 w-4" />
										Send Calendar Invites
									</Button>
								</div>
							</div>
						{:else}
							<!-- Watch & Discuss Mockup -->
							<div class="space-y-6">
								<!-- Watch tracking -->
								<div class="rounded-lg border border-border p-6 text-center">
									<div class="mx-auto mb-4 inline-flex h-12 w-12 items-center justify-center rounded-full bg-green-500/10">
										<Timer class="h-6 w-6 text-green-500" />
									</div>
									<h4 class="mb-2 text-xl font-bold">We're Watching!</h4>
									<p class="text-sm text-muted-foreground">Started at 8:05 PM • Ends ~10:51 PM</p>
									<Button variant="outline" class="mt-4">Movie Finished</Button>
								</div>
								
								<!-- Post-watch features -->
								<div class="grid gap-3">
									<Button variant="outline" class="justify-start">
										<MessageSquare class="mr-3 h-4 w-4" />
										Start Discussion
									</Button>
									<Button variant="outline" class="justify-start">
										<Star class="mr-3 h-4 w-4" />
										Rate This Movie
									</Button>
									<Button variant="outline" class="justify-start">
										<Calendar class="mr-3 h-4 w-4" />
										Plan Next Movie Night
									</Button>
								</div>
							</div>
						{/if}
					</div>
				</div>
			</div>
		</div>
	</section>

	<!-- Voting System -->
	<section id="voting" class="bg-muted/30 px-4 py-20">
		<div class="container mx-auto max-w-6xl">
			<div class="mb-12 text-center">
				<h2 class="mb-4 text-3xl font-bold">Fair & Fast Decision Making</h2>
				<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
					Our voting system is designed to respect everyone's opinion while getting to a decision quickly
				</p>
			</div>

			<div class="mb-12 grid gap-6 md:grid-cols-3">
				{#each Object.entries(votingSystem) as [key, system]}
					<Card class="text-center">
						<CardHeader>
							<div
								class={`mx-auto mb-4 inline-flex h-12 w-12 items-center justify-center rounded-full ${system.color}`}
							>
								<system.icon class="h-6 w-6" />
							</div>
							<CardTitle>{system.title}</CardTitle>
						</CardHeader>
						<CardContent>
							<p class="mb-2 text-sm text-muted-foreground">{system.description}</p>
							{#if system.note}
								<p class="text-xs text-muted-foreground">{system.note}</p>
							{/if}
						</CardContent>
					</Card>
				{/each}
			</div>

			<!-- UPDATED: Added post-watch features -->
			<div class="mb-12 text-center">
				<h3 class="mb-8 text-2xl font-bold">More Than Just Voting</h3>
				<div class="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
					{#each postWatchFeatures as feature}
						<Card>
							<CardContent class="p-6">
								<div class="mx-auto mb-4 inline-flex h-12 w-12 items-center justify-center rounded-full bg-primary/10">
									<feature.icon class="h-6 w-6 text-primary" />
								</div>
								<h4 class="mb-2 font-semibold">{feature.title}</h4>
								<p class="text-sm text-muted-foreground">{feature.description}</p>
							</CardContent>
						</Card>
					{/each}
				</div>
			</div>
		</div>
	</section>

	<!-- FAQ Section - UPDATED questions -->
	<section id="faq" class="px-4 py-20">
		<div class="container mx-auto max-w-4xl">
			<div class="mb-12 text-center">
				<h2 class="mb-4 text-3xl font-bold">Frequently Asked Questions</h2>
				<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
					Everything you need to know about GroupWatch's focused approach
				</p>
			</div>

			<div class="space-y-4">
				{#each faqs as faq}
					<Card>
						<CardContent class="p-6">
							<div class="flex items-start gap-4">
								<div
									class="mt-1 flex h-6 w-6 items-center justify-center rounded-full bg-primary/10"
								>
									<Zap class="h-3 w-3 text-primary" />
								</div>
								<div class="flex-1">
									<h3 class="mb-2 text-lg font-semibold">{faq.question}</h3>
									<p class="text-muted-foreground">{faq.answer}</p>
								</div>
							</div>
						</CardContent>
					</Card>
				{/each}
			</div>
		</div>
	</section>

	<!-- Final CTA - UPDATED messaging -->
	<section class="px-4 py-20">
		<div class="container mx-auto max-w-4xl text-center">
			<Card class="border-primary/20 bg-gradient-to-b from-background to-primary/5">
				<CardContent class="p-12">
					<div
						class="mx-auto mb-6 inline-flex h-16 w-16 items-center justify-center rounded-full bg-primary"
					>
						<Vote class="h-8 w-8 text-primary-foreground" />
					</div>
					<h2 class="mb-4 text-3xl font-bold">Ready to end the "What should we watch?" debate?</h2>
					<p class="mx-auto mb-8 max-w-2xl text-lg text-muted-foreground">
						Join thousands of groups using GroupWatch to make movie night decisions fast and fair.
					</p>
					<div class="flex flex-col justify-center gap-4 sm:flex-row">
						<Button size="lg" class="h-14 px-8 text-base">
							Create Your First Group
							<ArrowRight class="ml-2 h-5 w-5" />
						</Button>
						<Button size="lg" variant="outline" class="h-14 px-8">
							See Example Group
						</Button>
					</div>
					<p class="mt-6 text-sm text-muted-foreground">
						No credit card required • Free forever for small groups
					</p>
				</CardContent>
			</Card>
		</div>
	</section>

	<Footer />
</div>

<style>
	/* Smooth scrolling for anchor links */
	html {
		scroll-behavior: smooth;
	}

	/* Step transition animation */
	.step-transition {
		transition: all 0.3s ease;
	}

	/* Pulse animation for active step */
	@keyframes pulse-glow {
		0%,
		100% {
			box-shadow: 0 0 0 0 rgba(var(--primary), 0.4);
		}
		50% {
			box-shadow: 0 0 0 10px rgba(var(--primary), 0);
		}
	}

	.pulse-glow {
		animation: pulse-glow 2s infinite;
	}
</style>