<script lang="ts">
	import { Users, Plus } from '@lucide/svelte';
	import Button from '../ui/button/button.svelte';
	import type { Group } from '../../../types';

	interface DesktopHeaderProps {
		activeGroup: Group;
		createNewEvent: () => void;
		inviteToGroup: () => void;
	}
	const props: DesktopHeaderProps = $props();
</script>

<header
	class="sticky top-0 z-30 hidden border-b border-border bg-background/95 backdrop-blur supports-backdrop-filter:bg-background/60 md:block"
>
	<div class="flex h-16 items-center justify-between px-6">
		<div>
			<h1 class="text-xl font-semibold">{props.activeGroup.name}</h1>
			<p class="text-sm text-muted-foreground">
				{props.activeGroup.members.length} members • {props.activeGroup.description}
			</p>
		</div>

		<div class="flex items-center gap-2">
			<Button size="sm" variant="outline" onclick={props.inviteToGroup}>
				<Users class="mr-2 h-4 w-4" />
				Invite
			</Button>
			<Button size="sm" onclick={props.createNewEvent}>
				<Plus class="mr-2 h-4 w-4" />
				New Event
			</Button>
		</div>
	</div>
</header>
