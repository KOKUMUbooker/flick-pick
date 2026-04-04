<script lang="ts">
  import { Skeleton } from "$lib/components/ui/skeleton";
  import { Avatar, AvatarFallback } from "$lib/components/ui/avatar";
</script>

<div class="flex gap-3">
  <Avatar class="h-8 w-8">
    <AvatarFallback>
      <Skeleton class="h-full w-full rounded-full" />
    </AvatarFallback>
  </Avatar>
  <div class="flex-1">
    <div class="mb-1 flex items-center gap-2">
      <Skeleton class="h-4 w-32" />
      <Skeleton class="h-3 w-20" />
    </div>
    <Skeleton class="h-16 w-full rounded-lg" />
  </div>
</div>