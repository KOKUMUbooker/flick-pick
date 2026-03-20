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
    header? : Header,
    width? : "sm" | "md" | "lg" | "xl" | "2xl" | "3xl" | "4xl"
 }
 let {open = $bindable(),onOpenChange,children,header,width="sm"}:CustomDialogProps = $props()
</script>
 
<Dialog.Root {onOpenChange} {open}>
 <form>
  <Dialog.Content  class={`sm:max-w-${width}`}>
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