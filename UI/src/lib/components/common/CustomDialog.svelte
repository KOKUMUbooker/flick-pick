<script lang="ts">
 import * as Dialog from "$lib/components/ui/dialog/index.js";
 import type { Snippet } from "svelte";
 import Button from "../ui/button/button.svelte";
 import Spinner from "../ui/spinner/spinner.svelte";

 interface Header  {
    title: string;
    description?: string
 }
 interface Actions {
    proceedTxt? : string;
    cancelTxt? : string;
    onProceed : () => void;
    onCancel? : () => void;
 }
 
 interface CustomDialogProps {
    open: boolean;
    onOpenChange: (open:boolean) => void;
    children: Snippet<[]>;
    isLoading?: boolean;
    header? : Header,
    width? : "sm" | "lg" | "xl" | "2xl" | "3xl" | "4xl" | "5xl" | "6xl" | "7xl",
    actions? : Actions
 }
 let {open = $bindable(),onOpenChange,children,header,width="lg",actions,isLoading}:CustomDialogProps = $props()
</script>
 
<Dialog.Root {onOpenChange} {open} >
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

    {#if actions}
        <Dialog.Footer>
            <Button disabled={isLoading} variant="outline" onclick={actions.onCancel || onOpenChange.bind(null,false)}>{actions.cancelTxt || "Cancel"}</Button>
            <Button disabled={isLoading} variant="default" onclick={actions.onProceed}>
                {#if isLoading}
					<Spinner />
                {/if}
                {actions.proceedTxt || "Proceed"}
            </Button>
        </Dialog.Footer>
    {/if}
</Dialog.Content>
</form>
</Dialog.Root>