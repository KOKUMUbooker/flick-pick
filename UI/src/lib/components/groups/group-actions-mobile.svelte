<script lang="ts">
	import { EditIcon, MailPlus, Plus, Trash, Users } from '@lucide/svelte';
	import type { DBGroup } from "../../../types";
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
        showDeleteWarnDialog = $bindable(),
	}: GroupActionsMobile = $props();
</script>

<div class="flex h-16 items-center justify-between">
    <div class="flex flex-row gap-2 items-center">
        <Button variant="outline" onclick={()=>showAddGroupDialog=true}> <EditIcon/> </Button>
        <Button variant="destructive" onclick={()=>showDeleteWarnDialog=true}> <Trash/> </Button>
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