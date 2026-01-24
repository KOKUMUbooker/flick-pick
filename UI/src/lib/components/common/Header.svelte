<script lang="ts">
	import SunIcon from "@lucide/svelte/icons/sun";
 	import MoonIcon from "@lucide/svelte/icons/moon";
 
	import { toggleMode } from "mode-watcher";
	import { Button } from "$lib/components/ui/button/index.js";
	import Film from '@lucide/svelte/icons/film';
</script>

<div class="fixed top-0 bg-background/60 w-full">
    <div class="flex flex-row justify-between items-center py-4 px-6 mx-2">
        <div class="flex flex-row gap-2 justify-between items-center">
            <div class="bg-primary p-2 rounded-md">
                <Film/>
            </div>
            <div class="text-md font-bold">
                Movie Manager
            </div>
        </div>
        
        <div>
            <Button onclick={toggleMode} variant="outline" size="icon">
              <SunIcon
              class="h-[1.2rem] w-[1.2rem] scale-100 rotate-0 !transition-all dark:scale-0 dark:-rotate-90"
              />
              <MoonIcon
              class="absolute h-[1.2rem] w-[1.2rem] scale-0 rotate-90 !transition-all dark:scale-100 dark:rotate-0"
              />
              <span class="sr-only">Toggle theme</span>
            </Button>
        </div>
    </div>
</div>
