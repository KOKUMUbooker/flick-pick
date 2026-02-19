<script lang="ts">
	import { ChevronLeft, Plus, Search, Users } from '@lucide/svelte';
	import type { Group } from '../../../types';
	import Badge from '../ui/badge/badge.svelte';
	import Button from '../ui/button/button.svelte';

	interface GroupsSideBarProps {
		sidebarOpen: boolean;
		toggleSidebar: () => void;
		searchQuery: string;
		filteredGroups: Group[];
		createNewGroup: () => void;
		setActiveGroup: (group: Group) => void;
	}

	const props: GroupsSideBarProps = $props();
</script>

<aside
	class={`
			fixed inset-y-0 left-0 z-50 flex
			w-80 flex-col border-r border-border bg-card
			transition-transform duration-300 ease-in-out
			md:static
			${props.sidebarOpen ? 'translate-x-0' : '-translate-x-full md:translate-x-0'}
		`}
>
	<!-- Sidebar Header -->
	<div class="flex h-16 items-center justify-between border-b border-border px-4">
		<h2 class="text-lg font-semibold">Your Groups</h2>
		<Button size="sm" variant="ghost" onclick={props.toggleSidebar} class="md:hidden">
			<ChevronLeft class="h-4 w-4" />
		</Button>
	</div>

	<!-- Search -->
	<div class="border-b border-border p-4">
		<div class="relative">
			<Search class="absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
			<input
				bind:value={props.searchQuery}
				type="text"
				placeholder="Search groups..."
				class="w-full rounded-lg border border-border bg-background py-2 pr-4 pl-10 text-sm"
			/>
		</div>
	</div>

	<!-- Groups List -->
	<div class="flex-1 overflow-y-auto p-2">
		{#if props.filteredGroups.length === 0}
			<div class="p-4 text-center">
				<p class="text-sm text-muted-foreground">No groups found</p>
			</div>
		{:else}
			<div class="space-y-1">
				{#each props.filteredGroups as group}
					<button
						onclick={() => props.setActiveGroup(group)}
						class={`flex w-full items-center justify-between rounded-lg p-3 text-left transition-colors ${
							group.isActive ? 'bg-primary text-primary-foreground' : 'hover:bg-muted'
						}`}
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
							{#if group.upcomingEventsCount > 0}
								<Badge variant="outline" class="h-5 text-xs">
									{group.upcomingEventsCount}
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
		<Button class="w-full" onclick={props.createNewGroup}>
			<Plus class="mr-2 h-4 w-4" />
			Create New Group
		</Button>
	</div>
</aside>
