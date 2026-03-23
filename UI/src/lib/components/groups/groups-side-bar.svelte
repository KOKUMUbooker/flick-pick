<script lang="ts">
	import { ChevronLeft, Plus, RefreshCw, Search, Users } from '@lucide/svelte';
	import type { DBGroup } from '../../../types';
	import CustomDialog from '../common/CustomDialog.svelte';
	import AddGroupForm from '../dashboard/forms/add-group-form.svelte';
	import Button from '../ui/button/button.svelte';
	import Skeleton from '../ui/skeleton/skeleton.svelte';

	interface GroupsSideBarProps {
		searchQuery: string;
		sidebarOpen: boolean;
		selectedGroup: DBGroup | null;
		filteredGroups: DBGroup[];
		toggleSidebar: () => void;
		isFetching: boolean;
		isPending: boolean;
		refetchGroups: () => void;
	}

	let {
		sidebarOpen,
		toggleSidebar,
		filteredGroups,
		selectedGroup = $bindable(),
		searchQuery = $bindable(),
		isFetching,
		refetchGroups,
		isPending
	}: GroupsSideBarProps = $props();

	let showCreateGroup = $state(false);
	const onShowCreateGroupDialog = (show: boolean) => {
		showCreateGroup = show;
	};
</script>

<CustomDialog bind:open={showCreateGroup} onOpenChange={onShowCreateGroupDialog}>
	<AddGroupForm onOpenChange={onShowCreateGroupDialog} />
</CustomDialog>
<aside
	class={`
			fixed inset-y-0 left-0 z-45 flex
			w-80 flex-col border-r border-border bg-card
			transition-transform duration-300 ease-in-out
			md:static
			${sidebarOpen ? 'translate-x-0' : '-translate-x-full md:translate-x-0'}
		`}
>
	<!-- Sidebar Header -->
	<div class="flex h-16 items-center justify-between border-b border-border px-4">
		<div class="flex flex-row items-center gap-2">
			<h2 class="text-lg font-semibold">Your Groups</h2>
			<Button variant="ghost" onclick={refetchGroups} disabled={isFetching || isPending}>
				<RefreshCw />
			</Button>
		</div>
		<Button size="sm" variant="ghost" onclick={toggleSidebar} class="md:hidden">
			<ChevronLeft class="h-4 w-4" />
		</Button>
	</div>

	<!-- Search -->
	<div class="border-b border-border p-4">
		<div class="relative">
			<Search class="absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
			<input
				bind:value={searchQuery}
				type="text"
				placeholder="Search groups..."
				class="w-full rounded-lg border border-border bg-background py-2 pr-4 pl-10 text-sm"
			/>
		</div>
	</div>

	<!-- Groups List -->
	<div class="max-h-40vh flex-1 overflow-y-auto p-2">
		{#if isPending}
			<div class="grid space-y-2">
				{#each [1, 2, 3, 4, 5] as count (count)}
					<Skeleton class="h-16" />
				{/each}
			</div>
		{:else if filteredGroups.length === 0}
			<div class="p-4 text-center">
				<p class="text-sm text-muted-foreground">No groups found</p>
			</div>
		{:else}
			<div class="space-y-1">
				{#each filteredGroups as group (group.id)}
					<button
						onclick={() => (selectedGroup = group)}
						class={`flex w-full items-center justify-between rounded-lg p-2 text-left transition-colors ${
							group.id == selectedGroup?.id
								? 'bg-primary text-primary-foreground'
								: 'hover:bg-muted'
						}`}
					>
						<div class="flex items-center gap-3 overflow-hidden">
							<div class="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10">
								<Users class="h-5 w-5" />
							</div>
							<div class="min-w-0 flex-1">
								<div class="truncate font-medium">{group.name}</div>
								<div class="truncate text-xs opacity-80">
									{group.membersCount}
									{`member${group.membersCount > 1 ? 's' : ''}`}
								</div>
							</div>
						</div>
					</button>
				{/each}
			</div>
		{/if}
	</div>

	<!-- Create Group Button -->
	<div class="border-t border-border p-4">
		<Button class="w-full" onclick={() => (showCreateGroup = true)}>
			<Plus class="mr-2 h-4 w-4" />
			Create New Group
		</Button>
	</div>
</aside>
