<script lang="ts">
	import { Calendar, Clock, Star, Users } from '@lucide/svelte';
	let props: { activeTab: string } = $props();
</script>

<nav class="fixed right-0 bottom-0 left-0 border-t border-border bg-background p-4 md:hidden">
	<div class="grid grid-cols-4 gap-4">
		<button
			onclick={() => (props.activeTab = 'upcoming')}
			class={`flex flex-col items-center gap-1 ${props.activeTab === 'upcoming' ? 'text-primary' : 'text-muted-foreground'}`}
		>
			<Calendar class="h-5 w-5" />
			<span class="text-xs">Upcoming</span>
		</button>
		<button
			onclick={() => (props.activeTab = 'past')}
			class={`flex flex-col items-center gap-1 ${props.activeTab === 'past' ? 'text-primary' : 'text-muted-foreground'}`}
		>
			<Clock class="h-5 w-5" />
			<span class="text-xs">Past</span>
		</button>
		<button
			onclick={() => (props.activeTab = 'stats')}
			class={`flex flex-col items-center gap-1 ${props.activeTab === 'stats' ? 'text-primary' : 'text-muted-foreground'}`}
		>
			<Star class="h-5 w-5" />
			<span class="text-xs">Stats</span>
		</button>
		<button
			onclick={() => (props.activeTab = 'members')}
			class={`flex flex-col items-center gap-1 ${props.activeTab === 'members' ? 'text-primary' : 'text-muted-foreground'}`}
		>
			<Users class="h-5 w-5" />
			<span class="text-xs">Members</span>
		</button>
	</div>
</nav>
