<script lang="ts">
 import * as Dialog from "$lib/components/ui/dialog/index.js";
 import type { Snippet } from "svelte";

 interface Header  {
        title: string;
        description?: string
 }
 
 interface CustomDialogProps {
    open: boolean;
    onOpenChange: (open:boolean) => void;
    children: Snippet<[]>;
    header? : Header
 }
 let {open = $bindable(),onOpenChange,children,header}:CustomDialogProps = $props()
</script>
 
<Dialog.Root {onOpenChange} {open}>
 <form>
  <Dialog.Content  class="sm:max-w-106.25">
  {#if header}
    <Dialog.Header>
        <Dialog.Title>{header.title}</Dialog.Title>
        {#if header.description}
            <Dialog.Description>
                {header.description}
            </Dialog.Description>
        {/if}
    </Dialog.Header>
   {/if}

    {@render children()}

</Dialog.Content>
 </form>
</Dialog.Root>