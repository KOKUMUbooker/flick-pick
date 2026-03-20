<script lang="ts">
	import { MailPlus, Plus, Users } from '@lucide/svelte';
	import type { DBGroup } from '../../../types';
	import Button from '../ui/button/button.svelte';

	interface DesktopHeaderProps {
		selectedGroup: DBGroup | null;
		createNewEvent: () => void;
		inviteToGroup: () => void;
		showJoinGroupsDialog: boolean;
	}
	let {createNewEvent, inviteToGroup,selectedGroup,showJoinGroupsDialog = $bindable()}: DesktopHeaderProps = $props();
</script>

<header
	class="sticky top-0 z-30 hidden border-b border-border bg-background/95 backdrop-blur supports-backdrop-filter:bg-background/60 md:block"
>
	<div class="flex h-16 items-center justify-between px-6">
		<div>
			<h1 class="text-xl font-semibold">{selectedGroup?.name}</h1>
			<p class="text-sm text-muted-foreground">
				{selectedGroup?.description}
			</p>
		</div>

		{#if selectedGroup?.isUserAdmin}
			<div class="flex items-center gap-2">
			<Button size="sm" variant="outline" onclick={()=>showJoinGroupsDialog = true}>
					<MailPlus class="mr-2 h-4 w-4" />
					Join Request
				</Button>
				<Button size="sm" variant="outline" onclick={inviteToGroup}>
					<Users class="mr-2 h-4 w-4" />
					Send invite
				</Button>
				<Button size="sm" onclick={createNewEvent}>
					<Plus class="mr-2 h-4 w-4" />
					New Event
				</Button>
			</div>
		{/if}
	</div>
</header>
