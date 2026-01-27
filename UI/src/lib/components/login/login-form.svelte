<script lang="ts">
  import { Button } from "$lib/components/ui/button/index.js";
  import {
  	Field,
  	FieldDescription,
  	FieldGroup,
  	FieldLabel
  } from "$lib/components/ui/field/index.js";
  import { Input } from "$lib/components/ui/input/index.js";
  import { cn, type WithElementRef } from "$lib/utils.js";
  import type { HTMLFormAttributes } from "svelte/elements";
  let {
    ref = $bindable(null),
    class: className,
    ...restProps
  }: WithElementRef<HTMLFormAttributes> = $props();
  const id = $props.id();
</script>

<form class={cn("flex flex-col gap-6", className)} bind:this={ref} {...restProps}>
  <FieldGroup>
    <div class="flex flex-col items-center gap-1 text-center">
      <h1 class="text-2xl font-bold">Login to your account</h1>
      <p class="text-muted-foreground text-sm text-balance">
        Enter your email below to login to your account
      </p>
    </div>
    <Field>
      <FieldLabel for="email-{id}">Email</FieldLabel>
      <Input id="email-{id}" type="email" placeholder="m@example.com" required />
    </Field>
    <Field>
      <div class="flex items-center">
        <FieldLabel for="password-{id}">Password</FieldLabel>
        <a href="/forgot-password" class="ms-auto text-sm underline-offset-4 hover:underline">
          Forgot your password?
        </a>
      </div>
      <Input id="password-{id}" type="password" required />
    </Field>
    <Field>
      <Button type="submit">Login</Button>
    </Field>
    <Field>
      <FieldDescription class="text-center">
        Don't have an account?
        <a href="/signup" class="underline underline-offset-4">Sign up</a>
      </FieldDescription>
    </Field>
  </FieldGroup>
</form>