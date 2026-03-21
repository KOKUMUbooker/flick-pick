<script lang="ts">
	import { EditIcon, MailPlus, Plus, Trash, Users } from '@lucide/svelte';
	import type { DBGroup } from '../../../types';
	import Button from '../ui/button/button.svelte';

	interface DesktopHeaderProps {
		selectedGroup: DBGroup | null;
		createNewEvent: () => void;
 		showJoinGroupsDialog: boolean;
		showSendInviteDialog: boolean;
		showAddGroupDialog: boolean;
		showDeleteWarnDialog: boolean;
	}
	let {
		createNewEvent,
 		selectedGroup,
		showJoinGroupsDialog = $bindable(),
		showSendInviteDialog = $bindable(),
		showAddGroupDialog = $bindable(),
		showDeleteWarnDialog = $bindable(),
	}: DesktopHeaderProps = $props();
</script>

<header
	class="sticky top-0 z-30 hidden border-b border-border bg-background/95 backdrop-blur supports-backdrop-filter:bg-background/60 md:block"
>
	<div class="flex h-16 items-center justify-between px-6">
		<div>
			<div class="flex flex-row gap-2 items-center">
				<h1 class="text-xl font-semibold">{selectedGroup?.name}</h1>
				{#if selectedGroup?.isUserAdmin}
					<Button variant="outline" onclick={()=>showAddGroupDialog=true}> <EditIcon/> </Button>
			        <Button variant="destructive" onclick={()=>showDeleteWarnDialog=true}> <Trash/> </Button>
				{/if}
			</div>
			<p class="text-sm text-muted-foreground">
				{selectedGroup?.description}
			</p>
		</div>

		<div class="flex items-center gap-2">
			<Button size="sm" variant="outline" onclick={()=>showJoinGroupsDialog = true}>
				<MailPlus class="mr-2 h-4 w-4" />
				Join Request
			</Button>
			{#if selectedGroup?.isUserAdmin}
				<Button size="sm" variant="outline" onclick={()=>showSendInviteDialog = true}>
					<Users class="mr-2 h-4 w-4" />
					Send invite
				</Button>
				<Button size="sm" onclick={createNewEvent}>
					<Plus class="mr-2 h-4 w-4" />
					New Event
				</Button>
			{/if}
			</div>
	</div>
</header>
