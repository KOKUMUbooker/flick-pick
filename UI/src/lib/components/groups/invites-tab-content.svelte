<script lang="ts">
	import {
		AlertCircle,
		Ban,
		Check,
		CheckCircle,
		Clock,
		LogIn,
		Mail,
		RefreshCw,
		UserPlus,
		Users,
		X,
		XCircle
	} from '@lucide/svelte';
	import { createQuery } from '@tanstack/svelte-query';
	import { QUERY_KEYS, apiFetch } from '../../../api';
	import type { GroupInvitationItem, GroupInvitationStatus, GroupInvitationsRes } from '../../../api/types/group-invites';
	import { API_BASE_URL } from '../../../api/urls';
	import { getAppUser } from '../../../store';
	import type { DBGroup } from '../../../types';
	import { Badge } from '../ui/badge';
	import { Button } from '../ui/button';
	import { Card, CardContent } from '../ui/card';
	import Skeleton from '../ui/skeleton/skeleton.svelte';

	interface InvitesTabContentProps {
		selectedGroup: DBGroup | null;
        allowAutomaticFetch?: boolean
	}

    let user = getAppUser();
	let { selectedGroup ,allowAutomaticFetch = false}: InvitesTabContentProps = $props();
    
    let _invitesTabQuery = createQuery<
		null, // variables type
		Error, // error type
		GroupInvitationsRes // response type
	>(() => ({
		queryKey: [QUERY_KEYS.GROUP_INVITES + user?.id || ""],
		queryFn: async (data) => {
			return apiFetch(`${API_BASE_URL}/api/group/invites?userId=${encodeURIComponent(user?.id || "")}`, {
				method: 'GET',
				headers: { 'Content-Type': 'application/json' }
			});
		},
		enabled: selectedGroup != null || allowAutomaticFetch
	}));

	let invitesQuery = $state(_invitesTabQuery);
    let isGettingInvites = $derived(invitesQuery.isPending || invitesQuery.isFetching);

    // Helper functions to determine invitation type
    function isAdminInvite(invitation: GroupInvitationItem): boolean {
        return invitation.createdBy.email !== invitation.invitee.email;
    }

    function isUserRequest(invitation: GroupInvitationItem): boolean {
        return invitation.createdBy.email === invitation.invitee.email;
    }

    // Status badge helper
    function getStatusBadge(status: GroupInvitationStatus) {
        const statusConfig = {
            pending: { variant: 'outline', class: 'bg-yellow-500/10 text-yellow-600 border-yellow-200', icon: Clock, label: 'Pending' },
            approved: { variant: 'outline', class: 'bg-green-500/10 text-green-600 border-green-200', icon: CheckCircle, label: 'Approved' },
            cancelled: { variant: 'outline', class: 'bg-red-500/10 text-red-600 border-red-200', icon: XCircle, label: 'Cancelled' }
        };
        
        const config = statusConfig[status];
        return config;
    }

    // Action placeholders
    function handleAccept(invitation: GroupInvitationItem) {
        console.log('Accept invitation:', invitation);
        // TODO: Implement accept logic
    }

    function handleCancel(invitation: GroupInvitationItem) {
        console.log('Cancel invitation:', invitation);
        // TODO: Implement cancel logic
    }

    function handleReject(invitation: GroupInvitationItem) {
        console.log('Reject invitation:', invitation);
        // TODO: Implement reject logic
    }

    function handleRefetch() {
        invitesQuery.refetch();
    }
</script>

<div>
    {#if isGettingInvites}
        <!-- Skeletons -->
        <div class="space-y-4">
            {#each Array(3) as _, i (i)}
                <Card>
                    <CardContent class="p-6">
                        <div class="flex items-start justify-between">
                            <div class="flex-1 space-y-3">
                                <Skeleton class="h-5 w-32" />
                                <Skeleton class="h-4 w-48" />
                                <div class="flex gap-4">
                                    <Skeleton class="h-4 w-24" />
                                    <Skeleton class="h-4 w-24" />
                                </div>
                            </div>
                            <div class="flex gap-2">
                                <Skeleton class="h-8 w-16" />
                                <Skeleton class="h-8 w-16" />
                            </div>
                        </div>
                    </CardContent>
                </Card>
            {/each}
        </div>
    {:else}
        {#if invitesQuery.data?.invitations && invitesQuery.data.invitations.length > 0}
            <!-- Render fetched results -->
            <div class="space-y-4">
                {#each invitesQuery.data.invitations as invitation (invitation.id)}
                    {@const statusConfig = getStatusBadge(invitation.status)}
                    {@const isAdmin = isAdminInvite(invitation)}
                    {@const isRequest = isUserRequest(invitation)}
                    
                    <Card class="group transition-all hover:shadow-md hover:border-primary/20">
                        <CardContent class="p-6">
                            <div class="flex flex-col md:flex-row md:items-start gap-4">
                                <!-- Icon/Avatar -->
                                <div class="hidden md:block">
                                    <div class={`h-10 w-10 rounded-lg flex items-center justify-center ${
                                        isAdmin 
                                            ? 'bg-primary/10 text-primary' 
                                            : 'bg-purple-500/10 text-purple-500'
                                    }`}>
                                        {#if isAdmin}
                                            <UserPlus class="h-5 w-5" />
                                        {:else}
                                            <LogIn class="h-5 w-5" />
                                        {/if}
                                    </div>
                                </div>

                                <!-- Content -->
                                <div class="flex-1">
                                    <div class="flex items-start justify-between">
                                        <div>
                                            <!-- Badge + Type -->
                                            <div class="flex items-center gap-2 mb-2">
                                                <Badge variant="outline" class={`${statusConfig.class} gap-1`}>
                                                    <statusConfig.icon class="h-3 w-3" />
                                                    {statusConfig.label}
                                                </Badge>
                                                {#if isAdmin}
                                                    <Badge variant="secondary" class="gap-1">
                                                        <UserPlus class="h-3 w-3" />
                                                        Admin Invite
                                                    </Badge>
                                                {:else}
                                                    <Badge variant="secondary" class="gap-1">
                                                        <LogIn class="h-3 w-3" />
                                                        Join Request
                                                    </Badge>
                                                {/if}
                                            </div>
                                            
                                            <!-- Group Info -->
                                            <h4 class="text-lg font-semibold mb-1">{invitation.group.name}</h4>
                                            <p class="text-sm text-muted-foreground mb-3">{invitation.group.description || "No description"}</p>
                                            
                                            <!-- People involved -->
                                            <div class="space-y-1 text-sm">
                                                <div class="flex items-center gap-2">
                                                    <Users class="h-4 w-4 text-muted-foreground" />
                                                    <span class="font-medium">Group:</span>
                                                    <span class="text-muted-foreground">{invitation.group.name}</span>
                                                </div>
                                                
                                                {#if isAdmin}
                                                    <!-- Admin inviting someone -->
                                                    <div class="flex items-center gap-2">
                                                        <UserPlus class="h-4 w-4 text-muted-foreground" />
                                                        <span class="font-medium">Invited by:</span>
                                                        <span class="text-muted-foreground">{invitation.createdBy.fullName} ({invitation.createdBy.email})</span>
                                                    </div>
                                                    <div class="flex items-center gap-2">
                                                        <Mail class="h-4 w-4 text-muted-foreground" />
                                                        <span class="font-medium">Invitee:</span>
                                                        <span class="text-muted-foreground">{invitation.invitee.fullName} ({invitation.invitee.email})</span>
                                                    </div>
                                                {:else}
                                                    <!-- User requesting to join -->
                                                    <div class="flex items-center gap-2">
                                                        <LogIn class="h-4 w-4 text-muted-foreground" />
                                                        <span class="font-medium">Requested by:</span>
                                                        <span class="text-muted-foreground">{invitation.createdBy.fullName}</span>
                                                    </div>
                                                {/if}
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <!-- Actions -->
                                <div class="flex flex-row md:flex-col gap-2 md:ml-auto">
                                    {#if invitation.status === 'pending'}
                                        {#if isAdmin}
                                            <!-- Admin Invite: Cancel -->
                                            <Button 
                                                size="sm" 
                                                variant="outline"
                                                class="text-red-600 hover:text-red-700 hover:bg-red-50"
                                                onclick={() => handleCancel(invitation)}
                                            >
                                                <Ban class="h-4 w-4 md:mr-2" />
                                                <span class="hidden md:inline">Cancel</span>
                                            </Button>
                                        {:else}
                                            <!-- User Request: Accept/Reject -->
                                            <div class="flex gap-2">
                                                <Button 
                                                    size="sm"
                                                    class="bg-green-600 hover:bg-green-700 text-white"
                                                    onclick={() => handleAccept(invitation)}
                                                >
                                                    <Check class="h-4 w-4 md:mr-2" />
                                                    <span class="hidden md:inline">Accept</span>
                                                </Button>
                                                <Button 
                                                    size="sm" 
                                                    variant="outline"
                                                    class="text-red-600 hover:text-red-700 hover:bg-red-50"
                                                    onclick={() => handleReject(invitation)}
                                                >
                                                    <X class="h-4 w-4 md:mr-2" />
                                                    <span class="hidden md:inline">Reject</span>
                                                </Button>
                                            </div>
                                        {/if}
                                    {/if}
                                </div>
                            </div>
                        </CardContent>
                    </Card>
                {/each}
            </div>
        {:else if invitesQuery.data && invitesQuery.data.invitations.length === 0}
            <!-- No invites -->
            <Card class="border">
                <CardContent class="py-12 text-center">
                    <div class="mb-4 inline-flex h-12 w-12 items-center justify-center rounded-full bg-muted">
                        <Mail class="h-6 w-6 text-muted-foreground" />
                    </div>
                    <h3 class="mb-2 text-lg font-semibold">No invitations found</h3>
                    <p class="text-sm text-muted-foreground">
                        You don't have any pending group invitations or requests.
                    </p>
                </CardContent>
            </Card>
        {:else if invitesQuery.error}
            <!-- Error state -->
            <Card class="border-destructive">
                <CardContent class="py-12 text-center">
                    <div class="mb-4 inline-flex h-12 w-12 items-center justify-center rounded-full bg-destructive/10">
                        <AlertCircle class="h-6 w-6 text-destructive" />
                    </div>
                    <h3 class="mb-2 text-lg font-semibold">Failed to load invitations</h3>
                    <p class="text-sm text-muted-foreground mb-4">
                        {invitesQuery.error.message || "Something went wrong while loading your invitations."}
                    </p>
                    <Button variant="outline" onclick={handleRefetch}>
                        <RefreshCw class="mr-2 h-4 w-4" />
                        Try Again
                    </Button>
                </CardContent>
            </Card>
        {/if}
    {/if}
</div>