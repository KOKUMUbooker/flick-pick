<script lang="ts">
	import { MoreVertical, RefreshCw, Trash2, Users } from '@lucide/svelte';
	import { createMutation, createQuery } from '@tanstack/svelte-query';
	import { onMount } from 'svelte';
	import { toast } from 'svelte-sonner';
	import { QUERY_KEYS, apiFetch, queryClient } from '../../../api';
	import { API_BASE_URL } from '../../../api/urls';
	import { getAppUser } from '../../../store';
	import type { DBGroup, GroupMember } from '../../../types';
	import CustomDialog from '../common/CustomDialog.svelte';
	import { AvatarFallback } from '../ui/avatar';
	import Avatar from '../ui/avatar/avatar.svelte';
	import Badge from '../ui/badge/badge.svelte';
	import Button from '../ui/button/button.svelte';
	import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card';
	import {
		DropdownMenuContent,
		DropdownMenuItem,
		DropdownMenuSeparator,
		DropdownMenuTrigger
	} from '../ui/dropdown-menu';
	import DropdownMenu from '../ui/dropdown-menu/dropdown-menu.svelte';
	import Skeleton from '../ui/skeleton/skeleton.svelte';
	import { TabsContent } from '../ui/tabs';

	interface MembersTabContentProps {
		selectedGroup: DBGroup | null;
	}

	let { selectedGroup }: MembersTabContentProps = $props();
	let user = getAppUser();

	let selectedMember = $state<GroupMember | null>(null);
	let roleAction = $state<'grant' | 'revoke' | null>(null);
	let showRoleChangeDialog = $state(false);
	let showMemberRemovalDialog = $state(false);

	let fetchMembers = $state(false);
	let membersQuery = createQuery<
		null, // variables type
		Error, // error type
		{ groupMembers: GroupMember[] } // response type
	>(() => ({
		queryKey: [QUERY_KEYS.MEMBERS + selectedGroup?.id],
		queryFn: async () => {
			return apiFetch(
				`${API_BASE_URL}/api/groups/${selectedGroup?.id}/members?userId=${user?.id}`,
				{
					method: 'GET',
					headers: { 'Content-Type': 'application/json' }
				}
			);
		},
		enabled: fetchMembers && selectedGroup != null
	}));

	onMount(async () => {
		fetchMembers = true;
		if (selectedGroup != null) await membersQuery.refetch();
	});

	let memberRemovalMutation = createMutation<
		{ message: string }, // response type
		Error, // error type
		void // variables type
	>(() => ({
		mutationFn: async (data) => {
			return apiFetch(
				`${API_BASE_URL}/api/groups/${selectedGroup?.id || ''}/leave?initiator=${encodeURIComponent(user?.id || '')}&userId=${encodeURIComponent(selectedMember?.userId || '')}`,
				{
					method: 'DELETE',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify(data)
				}
			);
		},
		onSuccess: async () => {
			await queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.MEMBERS + selectedGroup?.id] });
		}
	}));
	const onShowMemberRemovalDialog = (show: boolean) => {
		if (show) showMemberRemovalDialog = true;
		else {
			showMemberRemovalDialog = false;
			selectedMember = null;
		}
	};
	const onProceedMemberRemoval = async () => {
		const res = await memberRemovalMutation.mutateAsync();
		toast.success(res.message, { richColors: true });
		onShowMemberRemovalDialog(false);
	};

	let memberRoleChangeMutation = createMutation<
		{ message: string }, // response type
		Error, // error type
		{ MakeAdmin: boolean } // variables type
	>(() => ({
		mutationFn: async (data) => {
			return apiFetch(
				`${API_BASE_URL}/api/groups/${selectedGroup?.id || ''}/members/${selectedMember?.userId || ''}?initiator=${encodeURIComponent(user?.id || '')}&userId=${encodeURIComponent(selectedMember?.id || '')}`,
				{
					method: 'PATCH',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify(data)
				}
			);
		},
		onSuccess: async () => {
			await queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.MEMBERS + selectedGroup?.id] });
		}
	}));
	const onShowRoleChangeDialog = (show: boolean) => {
		if (show) showRoleChangeDialog = true;
		else {
			roleAction = null;
			showRoleChangeDialog = false;
			selectedMember = null;
		}
	};
	const onProceedMemberRoleChange = async () => {
		const res = await memberRoleChangeMutation.mutateAsync({
			MakeAdmin: roleAction == 'revoke' ? false : true
		});
		toast.success(res.message, { richColors: true });
		onShowRoleChangeDialog(false);
	};
</script>

<TabsContent value="members" class="mt-2">
	<div class="grid">
		<div class="flex justify-end pb-4">
			<Button
				variant="outline"
				onclick={membersQuery.refetch}
				disabled={membersQuery.isPending || membersQuery.isFetching}
			>
				<RefreshCw />
				Refetch</Button
			>
		</div>
		<Card>
			<CardHeader>
				<CardTitle>Group Members</CardTitle>
				<CardDescription
					>{membersQuery.data?.groupMembers?.length ?? 0} people in this group</CardDescription
				>
			</CardHeader>
			<CardContent>
				<div class="">
					{#if membersQuery.isFetching || membersQuery.isPending}
						<div class="grid space-y-2">
							{#each [1, 2, 3, 4, 5] as count (count)}
								<Skeleton class="h-16" />
							{/each}
						</div>
					{:else if membersQuery.data != undefined}
						{#each membersQuery.data.groupMembers as member (member.id)}
							<div class="flex items-center justify-between rounded-lg border border-border p-4">
								<div class="flex items-center gap-3">
									<Avatar>
										<AvatarFallback>{member.fullName.charAt(0)}</AvatarFallback>
									</Avatar>
									<div>
										<div class="font-medium">{member.fullName}</div>
										<div class="flex items-center gap-2 text-sm text-muted-foreground">
											{member.isAdmin ? 'Group Admin' : 'Member'}
											<span>•</span>
											<span
												>Joined {new Date(member.joinedAt).toLocaleDateString('en-US', {
													month: 'short',
													day: 'numeric'
												})}</span
											>
										</div>
									</div>
								</div>
								<div class="flex items-center gap-2">
									{#if member.isAdmin}
										<Badge variant="outline">Admin</Badge>
									{/if}
									{#if selectedGroup?.isUserAdmin && member.email != user?.email}
										<DropdownMenu>
											<DropdownMenuTrigger>
												<Button size="sm" variant="ghost">
													<MoreVertical class="h-4 w-4" />
												</Button>
											</DropdownMenuTrigger>
											<DropdownMenuContent align="end">
												<DropdownMenuItem
													onclick={() => {
														selectedMember = member;
														roleAction = member.isAdmin ? 'revoke' : 'grant';
														showRoleChangeDialog = true;
													}}
												>
													<Users class="mr-2 h-4 w-4" />
													{member.isAdmin ? 'Revoke Admin role' : 'Make Admin'}
												</DropdownMenuItem>
												<DropdownMenuSeparator />
												<DropdownMenuItem
													class="text-destructive"
													onclick={() => {
														selectedMember = member;
														showMemberRemovalDialog = true;
													}}
												>
													<Trash2 class="mr-2 h-4 w-4" />
													Remove from Group
												</DropdownMenuItem>
											</DropdownMenuContent>
										</DropdownMenu>
									{/if}
								</div>
							</div>
						{/each}
					{/if}
				</div>
			</CardContent>
			<!-- <CardFooter class="flex flex-col gap-4">
			<Separator />
			<div class="flex w-full items-center justify-between">
				<div>
					<h4 class="font-medium">Invite new members</h4>
					<p class="text-sm text-muted-foreground">Share a link or send email invites</p>
				</div>
				<div class="flex gap-2">
					<Button variant="outline" onclick={inviteToGroup}>
						<Link class="mr-2 h-4 w-4" />
						Copy Link
					</Button>
					<Button onclick={inviteToGroup}>
						<Mail class="mr-2 h-4 w-4" />
						Email Invite
					</Button>
				</div>
			</div>
		</CardFooter> -->
		</Card>
	</div>

	<CustomDialog
		header={{ title: `${roleAction ? `${roleAction.toUpperCase()} role` : 'Change role'}` }}
		bind:open={showRoleChangeDialog}
		onOpenChange={onShowRoleChangeDialog}
		isLoading={memberRoleChangeMutation.isPending}
		actions={{ onProceed: onProceedMemberRoleChange }}
	>
		<p>
			Are you sure you want to {roleAction == 'grant'
				? `grant ${selectedMember?.fullName} an admin role?`
				: `revoke ${selectedMember?.fullName} of their admin role?`}
		</p>
	</CustomDialog>
	<CustomDialog
		header={{ title: 'Remove group member' }}
		bind:open={showMemberRemovalDialog}
		onOpenChange={onShowMemberRemovalDialog}
		isLoading={memberRemovalMutation.isPending}
		actions={{ onProceed: onProceedMemberRemoval }}
	>
		<p>Are you sure you want to remove {selectedMember?.fullName} from the group?.</p>
	</CustomDialog>
</TabsContent>
