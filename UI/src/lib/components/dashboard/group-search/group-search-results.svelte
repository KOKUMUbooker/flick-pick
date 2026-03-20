<!-- SearchResults.svelte -->
<script lang="ts">
	import { Button } from "$lib/components/ui/button";
	import { Card, CardContent } from "$lib/components/ui/card";
	import { ChevronLeft, ChevronRight, Users } from "@lucide/svelte";
	import type { FetchGroupsToJoinRes, GroupToJoin, CursorDirection, FetchGroupsToJoinQueryData } from "../../../../api/types";
 
	interface SearchResultsProps {
		results: FetchGroupsToJoinRes | null;
		searchQuery: string;
		userId: string;
 		onSelect: (group: GroupToJoin) => void;
        searchGroups:(data: FetchGroupsToJoinQueryData)=> Promise<FetchGroupsToJoinRes>
	}

	let { 
		results, 
		searchQuery, 
		userId, 
 		onSelect, 
        searchGroups
	}: SearchResultsProps = $props();

	let isNavigating = $state(false);
 
	async function handlePageChange(direction: CursorDirection) {
		await searchGroups({direction,query:searchQuery,userId,cursor : direction == "next"? results?.nextCursor : results?.prevCursor})
	}

</script>

{#if results?.results && results.results.length > 0}
	<div class="mt-4 space-y-3">
		{#each results.results as group (group.id)}
			<Card 
				class="cursor-pointer transition-all hover:border-primary/30 hover:shadow-md"
				onclick={() => onSelect(group)}
			>
				<CardContent class="p-4">
					<div class="flex items-start justify-between gap-4">
						<div class="flex-1">
							<div class="flex items-center gap-2 mb-1">
								<Users class="h-4 w-4 text-muted-foreground" />
								<h3 class="font-semibold text-lg">{group.name}</h3>
							</div>
							<p class="text-sm text-muted-foreground line-clamp-2">
								{group.description || "No description available"}
							</p>
							<div class="mt-2 flex items-center gap-4 text-xs text-muted-foreground">
								<span>Created by: {group.creatorFullName}</span>
								{#if group.memberCount !== undefined}
									<span>• {group.memberCount} members</span>
								{/if}
							</div>
						</div>
						<Button size="sm" variant="outline" class="shrink-0">
							Select
						</Button>
					</div>
				</CardContent>
			</Card>
		{/each}

		<!-- Pagination Controls -->
		<div class="flex items-center justify-between gap-4 pt-4">
			<div class="text-sm text-muted-foreground">
				Showing {results.results.length} results
			</div>
			<div class="flex gap-2">
				{#if results.hasPrevPage}
					<Button
						variant="outline"
						size="sm"
						disabled={isNavigating}
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
						disabled={isNavigating}
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
			<Users class="h-8 w-8 text-muted-foreground" />
		</div>
		<h3 class="mb-2 text-lg font-semibold">No groups found</h3>
		<p class="text-sm text-muted-foreground">
			No groups matching "{searchQuery}" were found. Try a different search term.
		</p>
	</div>
{/if}