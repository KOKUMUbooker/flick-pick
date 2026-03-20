<!-- UserSelectionConfirmation.svelte -->
<script lang="ts">
	import { Button } from "$lib/components/ui/button";
	import { Card, CardContent } from "$lib/components/ui/card";
	import { Mail, User, UserPlus, Users } from "@lucide/svelte";
	import type { UserToInvite } from "../../../../api/types";
	import type { DBGroup } from "../../../../types";
 
	interface UserSelectionConfirmationProps {
		user: UserToInvite;
		group: DBGroup | null;
		onConfirm: () => void;
		onCancel: () => void;
		isConfirming?: boolean;
 	}

	let { 
		user, 
		group, 
		onConfirm, 
		onCancel, 
		isConfirming = false, 
 	}: UserSelectionConfirmationProps = $props();
</script>

<div class="space-y-4 mt-4">
	<!-- User Details Card -->
	<Card class="border-primary/20">
		<CardContent class="p-6">
			<div class="flex items-start gap-4">
				<div class="hidden sm:block">
					<div class="h-12 w-12 rounded-xl bg-primary/10 flex items-center justify-center">
						<User class="h-6 w-6 text-primary" />
					</div>
				</div>
				<div class="flex-1">
					<h3 class="text-xl font-semibold mb-2">{user.fullName}</h3>
					
					<div class="grid grid-cols-1 gap-3 text-sm">
						<div class="flex items-center gap-2">
							<Mail class="h-4 w-4 text-muted-foreground" />
							<span class="text-muted-foreground">Email:</span>
							<span class="font-medium">{user.email}</span>
						</div>
					</div>
				</div>
			</div>
		</CardContent>
	</Card>

	<!-- Confirmation Message -->
	<div class="rounded-lg bg-muted/30 p-4">
		<p class="text-sm">
			You're about to send a group invitation to <span class="font-semibold">{user.fullName}</span> 
			for <span class="font-semibold">{group?.name || "your group"}</span>. 
			They will receive a notification and can choose to accept or decline.
		</p>
	</div>

	<!-- Actions -->
	<div class="flex justify-end gap-3 pt-2">
		<Button variant="outline" onclick={onCancel} disabled={isConfirming}>
			Cancel
		</Button>
		<Button onclick={onConfirm} disabled={isConfirming} class="gap-2">
			{#if isConfirming}
				<div class="h-4 w-4 animate-spin rounded-full border-2 border-background border-t-transparent" ></div>
				Sending Invite...
			{:else}
				<UserPlus class="h-4 w-4" />
				Confirm & Send Invite
			{/if}
		</Button>
	</div>
</div>