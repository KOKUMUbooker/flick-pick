<script lang="ts">
	import { Link, Mail, MessageSquare, MoreVertical, Trash2, Users } from '@lucide/svelte';
	import type { DBGroup, GroupMember,   } from '../../../types';
	import { AvatarFallback } from '../ui/avatar';
	import Avatar from '../ui/avatar/avatar.svelte';
	import Badge from '../ui/badge/badge.svelte';
	import Button from '../ui/button/button.svelte';
	import {
		Card,
		CardContent,
		CardDescription,
		CardFooter,
		CardHeader,
		CardTitle
	} from '../ui/card';
	import {
		DropdownMenuContent,
		DropdownMenuItem,
		DropdownMenuSeparator,
		DropdownMenuTrigger
	} from '../ui/dropdown-menu';
	import DropdownMenu from '../ui/dropdown-menu/dropdown-menu.svelte';
	import Separator from '../ui/separator/separator.svelte';
	import { TabsContent } from '../ui/tabs';
	import { createQuery } from '@tanstack/svelte-query';
	import { QUERY_KEYS, apiFetch } from '../../../api';
	import { API_BASE_URL } from '../../../api/urls';
	import { getAppUser } from '../../../store';
	import { onMount } from 'svelte';
	import Skeleton from '../ui/skeleton/skeleton.svelte';

	interface MembersTabContentProps {
		selectedGroup: DBGroup | null;
		inviteToGroup: () => void;
	}

	let { inviteToGroup, selectedGroup }: MembersTabContentProps = $props();
	let user = getAppUser();

	let fetchMembers = $state(false)
	let membersQuery = createQuery<
		null, // variables type
		Error, // error type
		{ groupMembers: GroupMember[] } // response type
	>(() => ({
		queryKey: [QUERY_KEYS.MEMBERS + selectedGroup?.id],
		queryFn: async () => {
			return apiFetch(`${API_BASE_URL}/api/groups/${selectedGroup?.id}/members?userId=${user?.id}`, {
				method: 'GET',
				headers: { 'Content-Type': 'application/json' }
			});
		},
		enabled: fetchMembers && selectedGroup != null
	}));

	onMount(async ()=>{
		fetchMembers = true
		if (selectedGroup != null) await membersQuery.refetch()
	})

 </script>

<TabsContent value="members" class="mt-6">
	<Card>
		<CardHeader>
			<CardTitle>Group Members</CardTitle>
			<CardDescription>{membersQuery.data?.groupMembers?.length ?? 0} people in this group</CardDescription>
		</CardHeader>
		<CardContent>
			<div class="space-y-3">
				{#if membersQuery.isFetching || membersQuery.isPending}
					<div class="grid space-y-2">
						{#each [1,2,3,4,5] as count (count)}
							<Skeleton class="h-16" />
						{/each}
					</div>
				{:else}
					{#if  membersQuery.data != undefined}
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
									<DropdownMenu>
										<DropdownMenuTrigger>
											<Button size="sm" variant="ghost">
												<MoreVertical class="h-4 w-4" />
											</Button>
										</DropdownMenuTrigger>
										<DropdownMenuContent align="end">
											<DropdownMenuItem>
												<MessageSquare class="mr-2 h-4 w-4" />
												Message
											</DropdownMenuItem>
											{#if !member.isAdmin}
												<DropdownMenuItem>
													<Users class="mr-2 h-4 w-4" />
													Make Admin
												</DropdownMenuItem>
											{/if}
											<DropdownMenuSeparator />
											<DropdownMenuItem class="text-destructive">
												<Trash2 class="mr-2 h-4 w-4" />
												Remove from Group
											</DropdownMenuItem>
										</DropdownMenuContent>
									</DropdownMenu>
								</div>
							</div>
						{/each}
					{/if}
				{/if}
			</div>
		</CardContent>
		<CardFooter class="flex flex-col gap-4">
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
		</CardFooter>
	</Card>
</TabsContent>
