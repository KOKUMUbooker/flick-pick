<script lang="ts">
	import { ArrowLeft, Check, Loader2, X } from "@lucide/svelte";
	import { createMutation } from "@tanstack/svelte-query";
	import { toast } from "svelte-sonner";
	import { apiFetch, QUERY_KEYS, queryClient } from "../../../api";
	import type { CreateInvitationData, CreateInvitationRes, FetchedUsersToInviteQueryData, FetchUsersToInviteRes, SearchState, UserToInvite } from "../../../api/types";
	import { API_BASE_URL } from "../../../api/urls";
	import { getAppUser } from "../../../store";
	import type { DBGroup } from "../../../types";
	import SearchBar from "../common/SearchBar.svelte";
	import UserSelectionConfirmation from "./user-search/user-selection-confirmation.svelte";
	import UserToInviteSearchResults from "./user-search/user-to-invite-search-results.svelte";

	interface SearchUsersToInviteFlowProps {
		selectedGroup: DBGroup | null;
		onCancel: ()=>void
		onGroupSelectConfirm: ()=>void
	}

	let { onCancel, onGroupSelectConfirm, selectedGroup } : SearchUsersToInviteFlowProps = $props()
    let searchQuery = $state("")
    let currentState: SearchState = $state('idle');
    let selectedUserToInvite: UserToInvite | null = $state(null);
	let searchResults: FetchUsersToInviteRes | null = $state(null);
	let isSearching = $state(false);
	let error = $state<string | null>(null);
	let user = getAppUser();

    async function searchUsersToInvite(data: FetchedUsersToInviteQueryData): Promise<FetchUsersToInviteRes> {
        const { direction = "next", query, userId, cursor, limit = 10 } = data;
		return queryClient.fetchQuery({
            queryKey: [QUERY_KEYS.USERS_TO_INVITE + query + selectedGroup?.id ],
            queryFn: async () => {
                const response = await apiFetch<FetchUsersToInviteRes>(
                    `${API_BASE_URL}/api/users/invite/search?groupId=${encodeURIComponent(selectedGroup?.id || "")}&query=${query}&userId=${encodeURIComponent(userId)}&direction=${direction}&cursor=${encodeURIComponent(cursor || "")}&limit=${limit}`,
                    {
                        method: 'GET',
                        headers: { 'Content-Type': 'application/json' }
                    }
                );
                return response;
			},
			staleTime: 5 * 60 * 1000, // Cache for 5 minutes
		});
	}

    const onSearch = async (query:string) => {
        if (!query.trim()) {
			searchResults = null;
			currentState = 'idle';
			return;
		}

        try {
            searchQuery = query;
            isSearching = true;
            error = null;
            currentState = 'searching';

           const res = await searchUsersToInvite({ 
				query, 
		    	direction:"next",
				userId:user?.id || "",
				groupId:selectedGroup?.id || ''
			})
           if (!res) {
				currentState = 'idle';
 				return
			}

            searchResults = res;
			currentState =  'results' ;
        } catch(err){
            error = err instanceof Error ? err.message : 'Failed to get groups to join';
			console.error(error)
			currentState = 'idle';
        } finally{
            isSearching = false
        }
    }

    // Handle user selection
	function handleSelectUserToInvite(user: UserToInvite) {
		selectedUserToInvite = user;
		currentState = 'confirming';
	}

	let createInvitationMutation = createMutation<
		CreateInvitationRes, // response type
		Error, // error type
		CreateInvitationData // variables type
	>(() => ({
		mutationFn: async (data) => {
			return apiFetch(`${API_BASE_URL}/api/group/${selectedGroup?.id || ""}/invite`, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(data)
			});
		},
		onSuccess: async () => {
			await queryClient.invalidateQueries({
				queryKey: [QUERY_KEYS.GROUP_INVITES + user?.id || "" + selectedGroup?.id || ""],
			});
		}
	}));

    // Handle user selection confirmation
	async function handleConfirmUserToInvite() {
		if (!user) return toast.error("Ensure you're logged in before proceeding", {richColors : true} )
		if (!selectedUserToInvite) return toast.error("No user to invite has been selected", {richColors : true} )
		if (!selectedGroup) return toast.error("No group under selection", {richColors : true} )

		const res = await createInvitationMutation.mutateAsync({CreatedById : user.id, InviteeUserId : selectedUserToInvite.id })
		toast.success(res.message,{richColors:true});
		
		onGroupSelectConfirm()
	}

    // Handle back button
	function handleBack() {
		if (currentState === 'confirming') {
			currentState = 'results';
			selectedUserToInvite = null;
		} else if (currentState === 'results') {
			currentState = 'idle';
			searchQuery = '';
			searchResults = null;
		} else {
			onCancel();
		}
	}

</script>

<div class="fixed inset-0 z-100 bg-background/80 backdrop-blur-sm">
	<div
		class="fixed inset-4 md:inset-auto md:top-1/2 md:left-1/2 md:h-auto md:w-full md:max-w-3xl md:-translate-x-1/2 md:-translate-y-1/2"
	>
		<div class="overflow-hidden rounded-lg border bg-card shadow-lg">
			<!-- Header -->
			<div class="flex items-center justify-between border-b p-4">
				<div class="flex items-center gap-2">
					{#if currentState !== 'idle'}
						<button
							onclick={handleBack}
							class="rounded-full p-2 transition-colors hover:bg-muted"
							aria-label="Go back"
						>
							<ArrowLeft class="h-4 w-4" />
						</button>
					{/if}
					<h2 class="text-lg font-semibold">
						{#if currentState === 'confirming'}
							Confirm User Selection
						{:else if currentState === 'success'}
							User invitation sent successfully
						{:else}
							Search for a user to invite to the group : {selectedGroup?.name}
						{/if}
					</h2>
				</div>
				<button
					onclick={onCancel}
					class="rounded-full p-2 transition-colors hover:bg-muted"
					aria-label="Close"
				>
					<X class="h-4 w-4" />
				</button>
			</div>

			<!-- Content -->
			 <div class="max-h-[calc(100vh-12rem)] overflow-y-auto p-4">
				{#if error}
					<div class="mb-4 rounded-md bg-destructive/10 p-3 text-sm text-destructive">
						{error}
					</div>
				{/if}

				{#if currentState === 'success'}
					<div class="flex flex-col items-center justify-center py-12 text-center">
						<div class="mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-primary/10">
							<Check class="h-8 w-8 text-primary" />
						</div>
						<h3 class="mb-2 text-xl font-semibold">Group successfully </h3>
						<p class="text-muted-foreground">
							An invite has been successfully sent to {selectedUserToInvite?.fullName}
						</p>
					</div>
				{:else}
					 <SearchBar {onSearch} query={searchQuery} placeholder="Start typing the user name..."/>

					{#if currentState === 'searching'}
						<div class="flex justify-center py-12">
							<Loader2 class="h-8 w-8 animate-spin text-primary" />
						</div>
					{/if}

					{#if currentState === 'results'}
						<UserToInviteSearchResults results={searchResults} searchQuery={searchQuery} userId={user?.id || ""} onSelect={handleSelectUserToInvite}  groupId={selectedGroup?.id || ""} searchUsersToInvite={searchUsersToInvite} />
					{/if}

					{#if currentState === 'confirming' && selectedUserToInvite}
						<UserSelectionConfirmation
							user={selectedUserToInvite}
							group={selectedGroup}
							onConfirm={handleConfirmUserToInvite}
							onCancel={() => handleBack()}
						/>
					{/if}
				{/if}
			</div>
		</div>
	</div>
</div>
