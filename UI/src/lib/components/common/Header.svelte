<!-- src/lib/components/layout/Header.svelte -->
<script lang="ts">
	import {
		Home,
		LayoutDashboard,
		LogIn,
		MessageCircleQuestionMark,
		Play,
		User
	} from '@lucide/svelte';
	import MoonIcon from '@lucide/svelte/icons/moon';
	import SunIcon from '@lucide/svelte/icons/sun';
	import { page } from '$app/stores';
	import { Button } from '$lib/components/ui/button/index.js';
	import { toggleMode } from 'mode-watcher';
	import { authState, logOut } from '../../../store';
	import { createMutation } from '@tanstack/svelte-query';
	import { apiFetch } from '../../../api';
	import { API_BASE_URL } from '../../../api/urls';
	import { goto } from '$app/navigation';
	import LoadingOverlay from './LoadingOverlay.svelte';

	// Authentication state (in real app, this would come from a store)
	let isAuthenticated = $derived(authState.user != undefined);
	let user = $derived(authState.user);

	// Navigation items
	const navItems = [
		{ name: 'Home', href: '/', icon: Home },
		{ name: 'How it works', href: '/how-it-works', icon: MessageCircleQuestionMark }
	];

	let logoutMutation = createMutation<
		{message:string}, // response type
		Error, // error type
		void // variables type
	>(() => ({
		mutationFn: async () => {
			return apiFetch(`${API_BASE_URL}/api/auth/logout`, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
 			});
		}
	}));

	// Mobile menu state
	let mobileMenuOpen = $state(false);
	const handleLogout = async () => { 
		await logoutMutation.mutateAsync();
		logOut(); // Clear local user state
		goto("/login")
	}
</script>

<LoadingOverlay show={logoutMutation.isPending} message={"Logging you out..."} spinnerSize="md"/>
<header class="fixed top-0 z-10 w-full border-b border-border bg-background/80 backdrop-blur-sm">
	<div class="container mx-auto px-4 sm:px-6 lg:px-8">
		<div class="flex h-16 items-center justify-between">
			<!-- Logo -->
			<div class="flex items-center">
				<a href="/" class="flex items-center gap-3">
					<div
						class="flex h-8 w-8 items-center justify-center rounded-lg bg-linear-to-br from-primary to-primary/80"
					>
						<Play class="h-4 w-4 text-primary-foreground" />
					</div>
					<span class="text-xl font-bold tracking-tight">WatchHive</span>
				</a>
			</div>

			<!-- Right side: Auth + Theme Toggle -->
			<div class="flex items-center gap-4">
				<nav class="hidden md:ml-10 md:flex md:items-center md:space-x-6">
					{#each navItems as item}
						<a
							href={item.href}
							class="text-sm font-medium transition-colors hover:text-primary data-current:text-primary"
							data-current={$page.url.pathname === item.href}
						>
							<div class="flex items-center gap-2">
								{#if item.icon}
									<svelte:component this={item.icon} class="h-4 w-4" />
								{/if}
								<span>{item.name}</span>
							</div>
						</a>
					{/each}
				</nav>

				<!-- Authentication Section -->
				<div class="flex items-center gap-2">
					{#if isAuthenticated && user}
						<!-- Authenticated User -->
						<div class="flex items-center gap-3">
							<!-- User Profile Dropdown (simplified) -->
							<div class="hidden items-center gap-2 md:flex">
								<div
									class="flex items-center gap-2 rounded-lg px-3 py-2 transition-colors hover:bg-accent"
								>
									<div class="flex h-8 w-8 items-center justify-center rounded-full bg-primary/10">
										<User class="h-4 w-4 text-primary" />
									</div>
									<span class="text-sm font-medium">{user.fullName}</span>
								</div>

								<!-- Dashboard Button -->
								<a href="/dashboard">
									<Button variant="outline" size="sm" class="gap-2">
										<LayoutDashboard class="h-4 w-4" />
										Dashboard
									</Button>
								</a>

								<!-- Logout Button -->
								<Button
									variant="ghost"
									size="sm"
									onclick={handleLogout}
								>
									Logout
								</Button>
							</div>

							<!-- Mobile: Just the dashboard button -->
							<div class="md:hidden">
								<a href="/dashboard">
									<Button variant="outline" size="sm" class="gap-2">
										<LayoutDashboard class="h-4 w-4" />
									</Button>
								</a>
							</div>
						</div>
					{:else}
						<!-- Not Authenticated -->
						<a href="/login">
							<Button variant="default" size="sm" class="gap-2">
								<LogIn class="h-4 w-4" />
								<span class="hidden sm:inline">Login</span>
							</Button>
						</a>
					{/if}
				</div>

				<!-- Mobile Menu Button -->
				<button
					type="button"
					class="inline-flex items-center justify-center rounded-md p-2 text-foreground hover:bg-accent hover:text-accent-foreground focus:ring-2 focus:ring-primary focus:outline-none md:hidden"
					onclick={() => (mobileMenuOpen = !mobileMenuOpen)}
					aria-label={mobileMenuOpen ? 'Close menu' : 'Open menu'}
				>
					<svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
						{#if mobileMenuOpen}
							<path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M6 18L18 6M6 6l12 12"
							/>
						{:else}
							<path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M4 6h16M4 12h16M4 18h16"
							/>
						{/if}
					</svg>
				</button>

				<!-- Theme Toggle -->
				<Button
					onclick={toggleMode}
					variant="ghost"
					size="icon"
					class="hidden sm:inline-flex"
					aria-label="Toggle theme"
				>
					<SunIcon class="h-5 w-5 scale-100 rotate-0 transition-all dark:scale-0 dark:-rotate-90" />
					<MoonIcon
						class="absolute h-5 w-5 scale-0 rotate-90 transition-all dark:scale-100 dark:rotate-0"
					/>
				</Button>
			</div>
		</div>

		<!-- Mobile Menu -->
		{#if mobileMenuOpen}
			<div class="md:hidden">
				<div class="space-y-1 border-t border-border px-2 pt-4 pb-3">
					{#each navItems as item}
						<a
							href={item.href}
							class="flex items-center gap-3 rounded-md px-3 py-2 text-base font-medium hover:bg-accent hover:text-accent-foreground data-current:bg-accent data-current:text-accent-foreground"
							data-current={$page.url.pathname === item.href}
							onclick={() => (mobileMenuOpen = false)}
						>
							{#if item.icon}
								<svelte:component this={item.icon} class="h-5 w-5" />
							{/if}
							{item.name}
						</a>
					{/each}

					<!-- Mobile Theme Toggle -->
					<button
						onclick={() => {
							toggleMode();
							mobileMenuOpen = false;
						}}
						class="flex w-full items-center gap-3 rounded-md px-3 py-2 text-base font-medium hover:bg-accent hover:text-accent-foreground"
					>
						<SunIcon
							class="h-5 w-5 scale-100 rotate-0 transition-all dark:scale-0 dark:-rotate-90"
						/>
						<MoonIcon
							class="absolute h-5 w-5 scale-0 rotate-90 transition-all dark:scale-100 dark:rotate-0"
						/>
						<span>Toggle Theme</span>
					</button>

					{#if isAuthenticated && user}
						<!-- Mobile Auth Options -->
						<div class="border-t border-border pt-4">
							<div class="flex items-center gap-3 px-3 py-2">
								<div class="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10">
									<User class="h-5 w-5 text-primary" />
								</div>
								<div>
									<p class="text-sm font-medium">{user.fullName}</p>
									<p class="text-xs text-muted-foreground">{user.email}</p>
								</div>
							</div>

							<a
								href="/dashboard"
								class="flex items-center gap-3 rounded-md px-3 py-2 text-base font-medium hover:bg-accent hover:text-accent-foreground"
								onclick={() => (mobileMenuOpen = false)}
							>
								<LayoutDashboard class="h-5 w-5" />
								Dashboard
							</a>

							<button
								onclick={handleLogout}
								class="flex w-full items-center gap-3 rounded-md px-3 py-2 text-base font-medium hover:bg-destructive/10 hover:text-destructive"
							>
								<LogIn class="h-5 w-5 rotate-180" />
								Logout
							</button>
						</div>
					{:else}
						<!-- Mobile Login/Signup -->
						<div class="border-t border-border pt-4">
							<a
								href="/login"
								class="mb-2 flex items-center gap-3 rounded-md px-3 py-2 text-base font-medium hover:bg-accent hover:text-accent-foreground"
								onclick={() => (mobileMenuOpen = false)}
							>
								<LogIn class="h-5 w-5" />
								Login
							</a>
							<a
								href="/register"
								class="flex items-center gap-3 rounded-md px-3 py-2 text-base font-medium hover:bg-accent hover:text-accent-foreground"
								onclick={() => (mobileMenuOpen = false)}
							>
								<User class="h-5 w-5" />
								Sign Up
							</a>
						</div>
					{/if}
				</div>
			</div>
		{/if}
	</div>
</header>

<style>
	/* Active link indicator */
	a[data-current='true'] {
		position: relative;
	}

	a[data-current='true']:after {
		content: '';
		position: absolute;
		bottom: -6px;
		left: 0;
		right: 0;
		height: 2px;
		background-color: hsl(var(--primary));
		border-radius: 1px;
	}

	@media (max-width: 768px) {
		a[data-current='true']:after {
			display: none;
		}
	}
</style>
