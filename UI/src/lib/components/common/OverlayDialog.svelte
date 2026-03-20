<script lang="ts">
	import { X } from "@lucide/svelte";
	import type { Snippet } from "svelte";
	import Button from "../ui/button/button.svelte";
    interface Header  {
        title: string;
        description?: string
    }
 
    interface OverlayDialogProps {
        open: boolean;
        onOpenChange: (open:boolean) => void;
        children: Snippet<[]>;
        title:string;
        description?:string;
     }
    let { open = $bindable(), onOpenChange, children, description, title }:OverlayDialogProps = $props()
</script>

 <div class="fixed inset-0 z-100 bg-background/80 backdrop-blur-sm">
	<div
		class="fixed inset-4 md:inset-auto md:top-1/2 md:left-1/2 md:h-auto md:w-full md:max-w-3xl md:-translate-x-1/2 md:-translate-y-1/2"
	>
		<div class="overflow-hidden rounded-lg border bg-card shadow-lg">
			<div class="flex items-center justify-between border-b p-4">
				<div class="flex items-center gap-2">
					<h2 class="text-lg font-semibold">
						{title}
					</h2>
                    {#if description}
                        <p class="text-sm">{description}</p>
                    {/if}
				</div>
				<Button
					onclick={onOpenChange.bind(null,false)}
                    variant="ghost"
 					aria-label="Close"
				>
					<X class="h-4 w-4" />
				</Button>
			</div>

            <div class="max-h-[calc(100vh-12rem)] overflow-y-auto p-4">
                {@render children()}
            </div>
        </div>
    </div>
</div>
 