<script lang="ts">
	import { Button } from "$lib/components/ui/button";
	import { Card, CardContent } from "$lib/components/ui/card";
	import { ChevronLeft, ChevronRight, Mail, User, UserPlus } from "@lucide/svelte";
	import type { CursorDirection, FetchedUsersToInviteQueryData, FetchUsersToInviteRes, UserToInvite } from "../../../../api/types";
 
	interface UserToInviteSearchResultsProps {
		results: FetchUsersToInviteRes | null;
		searchQuery: string;
		userId: string;
		groupId: string;
 		onSelect: (user: UserToInvite) => void;
		searchUsersToInvite : (data: FetchedUsersToInviteQueryData) => Promise<FetchUsersToInviteRes>
	}

	let { 
		results, 
		searchQuery, 
		userId,
		groupId,
 		onSelect, 
		searchUsersToInvite 
	}: UserToInviteSearchResultsProps = $props();

	async function handlePageChange(direction: CursorDirection) {
		await searchUsersToInvite({
			direction,
			query: searchQuery,
			groupId,
			userId,
			cursor : direction == "next"? results?.nextCursor : results?.prevCursor,
		})
	}
</script>

{#if results?.results && results.results.length > 0}
	<div class="mt-4 space-y-3">
		{#each results.results as user (user.id)}
			<Card 
				class="cursor-pointer transition-all hover:border-primary/30 hover:shadow-md"
				onclick={() => onSelect(user)}
			>
				<CardContent class="p-4">
					<div class="flex items-start justify-between gap-4">
						<div class="flex-1">
							<div class="flex items-center gap-2 mb-1">
								<User class="h-4 w-4 text-muted-foreground" />
								<h3 class="font-semibold text-lg">{user.fullName}</h3>
							</div>
							<div class="flex items-center gap-2 text-sm text-muted-foreground">
								<Mail class="h-3.5 w-3.5" />
								<span>{user.email}</span>
							</div>
						</div>
						<Button size="sm" variant="outline" class="shrink-0 gap-1">
							<UserPlus class="h-4 w-4" />
							<span class="hidden sm:inline">Invite</span>
						</Button>
					</div>
				</CardContent>
			</Card>
		{/each}

		<!-- Pagination Controls -->
		<div class="flex items-center justify-between gap-4 pt-4">
			<div class="text-sm text-muted-foreground">
				Showing {results.results.length} {results.results.length === 1 ? 'user' : 'users'}
			</div>
			<div class="flex gap-2">
				{#if results.hasPrevPage}
					<Button
						variant="outline"
						size="sm"
 						onclick={() => handlePageChange('prev')}
					>
						<ChevronLeft class="mr-1 h-4 w-4" />
						Previous
 					</Button>
				{/if}
				
				{#if results.hasNextPage}
					<Button
						variant="outline"
						size="sm"
 						onclick={() => handlePageChange('next')}
					>
						Next
						<ChevronRight class="ml-1 h-4 w-4" />
 					</Button>
				{/if}
			</div>
		</div>
	</div>
{:else if searchQuery && searchQuery.trim() !== ''}
	<div class="mt-8 flex flex-col items-center justify-center text-center">
		<div class="mb-4 rounded-full bg-muted p-4">
			<User class="h-8 w-8 text-muted-foreground" />
		</div>
		<h3 class="mb-2 text-lg font-semibold">No users found</h3>
		<p class="text-sm text-muted-foreground">
			No users matching "{searchQuery}" were found. Try a different search term.
		</p>
	</div>
{/if}