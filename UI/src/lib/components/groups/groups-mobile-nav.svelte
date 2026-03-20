<script lang="ts">
	import { Calendar, Clock, Star, UserPlus, Users } from '@lucide/svelte';
	let { activeTab = $bindable<string>() } = $props();
</script>

<nav class="fixed right-0 bottom-0 left-0 border-t border-border bg-background p-4 md:hidden">
	<div class="grid grid-cols-4 gap-4">
		<button
			onclick={() => (activeTab = 'upcoming')}
			class={`flex flex-col items-center gap-1 ${activeTab === 'upcoming' ? 'text-primary' : 'text-muted-foreground'}`}
		>
			<Calendar class="h-5 w-5" />
			<span class="text-xs">Upcoming</span>
		</button>
		<button
			onclick={() => (activeTab = 'past')}
			class={`flex flex-col items-center gap-1 ${activeTab === 'past' ? 'text-primary' : 'text-muted-foreground'}`}
		>
			<Clock class="h-5 w-5" />
			<span class="text-xs">Past</span>
		</button>
		<button
			onclick={() => (activeTab = 'members')}
			class={`flex flex-col items-center gap-1 ${activeTab === 'members' ? 'text-primary' : 'text-muted-foreground'}`}
		>
			<Users class="h-5 w-5" />
			<span class="text-xs">Members</span>
		</button>
		<button
			onclick={() => (activeTab = 'invites')}
			class={`flex flex-col items-center gap-1 ${activeTab === 'invites' ? 'text-primary' : 'text-muted-foreground'}`}
		>
			<UserPlus class="h-5 w-5" />
			<span class="text-xs">Invites</span>
		</button>
	</div>
</nav>
