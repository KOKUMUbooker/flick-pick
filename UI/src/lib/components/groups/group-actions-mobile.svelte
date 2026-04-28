<script lang="ts">
	import { EditIcon, MailPlus, Plus, Trash, Users } from '@lucide/svelte';
	import type { DBGroup } from '../../../types';
	import Button from '../ui/button/button.svelte';

	interface GroupActionsMobile {
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
		showDeleteWarnDialog = $bindable()
	}: GroupActionsMobile = $props();
</script>

<div class="flex flex-wrap items-center justify-between gap-2">
	<div class="flex flex-row items-center gap-2">
		<Button variant="outline" onclick={() => (showAddGroupDialog = true)}>
			<EditIcon />
		</Button>
		<Button variant="destructive" onclick={() => (showDeleteWarnDialog = true)}>
			<Trash />
		</Button>
	</div>

	<div class="flex flex-wrap gap-2">
		<Button size="sm" variant="outline" onclick={() => (showJoinGroupsDialog = true)}>
			<MailPlus class="h-4 w-4" />
			<span class="hidden sm:inline">Join Request</span>
		</Button>

		{#if selectedGroup?.isUserAdmin}
			<Button size="sm" variant="outline" onclick={() => (showSendInviteDialog = true)}>
				<Users class="h-4 w-4" />
				<span class="hidden sm:inline">Send Invite</span>
			</Button>

			<Button size="sm" onclick={createNewEvent}>
				<Plus class="h-4 w-4" />
				<span class="hidden sm:inline">New Event</span>
			</Button>
		{/if}
	</div>
</div>
