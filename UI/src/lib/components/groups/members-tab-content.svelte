<script lang="ts">
	import { MoreVertical, MessageSquare, Users, Trash2, Link, Mail } from '@lucide/svelte';
	import { AvatarFallback } from '../ui/avatar';
	import {
		Card,
		CardHeader,
		CardTitle,
		CardDescription,
		CardContent,
		CardFooter
	} from '../ui/card';
	import {
		DropdownMenuTrigger,
		DropdownMenuContent,
		DropdownMenuItem,
		DropdownMenuSeparator
	} from '../ui/dropdown-menu';
	import { TabsContent } from '../ui/tabs';
	import Avatar from '../ui/avatar/avatar.svelte';
	import Badge from '../ui/badge/badge.svelte';
	import DropdownMenu from '../ui/dropdown-menu/dropdown-menu.svelte';
	import Button from '../ui/button/button.svelte';
	import Separator from '../ui/separator/separator.svelte';
	import type { Group } from '../../../types';

	interface MembersTabContentProps {
		activeGroup: Group | null;
		inviteToGroup: () => void;
	}

	let props = $props();
</script>

<TabsContent value="members" class="mt-6">
	<Card>
		<CardHeader>
			<CardTitle>Group Members</CardTitle>
			<CardDescription>{props.activeGroup.members.length} people in this group</CardDescription>
		</CardHeader>
		<CardContent>
			<div class="space-y-3">
				{#each props.activeGroup.members as member}
					<div class="flex items-center justify-between rounded-lg border border-border p-4">
						<div class="flex items-center gap-3">
							<Avatar>
								<AvatarFallback>{member.name.charAt(0)}</AvatarFallback>
							</Avatar>
							<div>
								<div class="font-medium">{member.name}</div>
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
					<Button variant="outline" onclick={props.inviteToGroup}>
						<Link class="mr-2 h-4 w-4" />
						Copy Link
					</Button>
					<Button onclick={props.inviteToGroup}>
						<Mail class="mr-2 h-4 w-4" />
						Email Invite
					</Button>
				</div>
			</div>
		</CardFooter>
	</Card>
</TabsContent>
