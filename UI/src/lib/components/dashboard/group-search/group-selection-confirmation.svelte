<!-- GroupSelectionConfirmation.svelte -->
<script lang="ts">
	import { Button } from "$lib/components/ui/button";
	import { Card, CardContent } from "$lib/components/ui/card";
	import { Users, Mail, Calendar, User, AlertCircle } from "@lucide/svelte";
	import type { GroupToJoin } from "../../../../api/types";
 
	interface GroupSelectionConfirmationProps {
		group: GroupToJoin;
		onConfirm: () => void;
		onCancel: () => void;
		isConfirming?: boolean;
	}

	let { 
		group, 
		onConfirm, 
		onCancel, 
		isConfirming = false, 
	}: GroupSelectionConfirmationProps = $props();
</script>

<div class="space-y-4 mt-4 ">
	<!-- Group Details Card -->
	<Card class="border-primary/20">
		<CardContent class="p-6">
			<div class="flex items-start gap-4">
				<div class="hidden sm:block">
					<div class="h-12 w-12 rounded-xl bg-primary/10 flex items-center justify-center">
						<Users class="h-6 w-6 text-primary" />
					</div>
				</div>
				<div class="flex-1">
					<h3 class="text-xl font-semibold mb-2">{group.name}</h3>
					<p class="text-sm text-muted-foreground mb-4">
						{group.description || "No description provided"}
					</p>
					
					<div class="grid grid-cols-1 sm:grid-cols-2 gap-3 text-sm">
						<div class="flex items-center gap-2">
							<User class="h-4 w-4 text-muted-foreground" />
							<span class="text-muted-foreground">Created by:</span>
							<span class="font-medium">{group.creatorFullName}</span>
						</div>
						<div class="flex items-center gap-2">
							<Mail class="h-4 w-4 text-muted-foreground" />
							<span class="text-muted-foreground">Contact:</span>
							<span class="font-medium">{group.creatorEmail}</span>
						</div>
						{#if group.memberCount !== undefined}
							<div class="flex items-center gap-2">
								<Users class="h-4 w-4 text-muted-foreground" />
								<span class="text-muted-foreground">Members:</span>
								<span class="font-medium">{group.memberCount}</span>
							</div>
						{/if}
						{#if group.createdAt}
							<div class="flex items-center gap-2">
								<Calendar class="h-4 w-4 text-muted-foreground" />
								<span class="text-muted-foreground">Created:</span>
								<span class="font-medium">{new Date(group.createdAt).toLocaleDateString()}</span>
							</div>
						{/if}
					</div>
				</div>
			</div>
		</CardContent>
	</Card>

	<!-- Confirmation Message -->
	<div class="rounded-lg bg-muted/30 p-4">
		<p class="text-sm">
			You're about to send a join request to <span class="font-semibold">{group.name}</span>. 
			The group admin will be notified and can approve or reject your request.
		</p>
	</div>

	<!-- Actions -->
	<div class="flex justify-end gap-3 pt-2">
		<Button variant="outline" onclick={onCancel} disabled={isConfirming}>
			Cancel
		</Button>
		<Button onclick={onConfirm} disabled={isConfirming}>
			{#if isConfirming}
				<div class="mr-2 h-4 w-4 animate-spin rounded-full border-2 border-background border-t-transparent" ></div>
				Sending Request...
			{:else}
				Confirm & Send Request
			{/if}
		</Button>
	</div>
</div>