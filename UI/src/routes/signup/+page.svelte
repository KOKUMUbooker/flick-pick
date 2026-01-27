<script>
  import GalleryVerticalEndIcon from "@lucide/svelte/icons/gallery-vertical-end";
  import SignupForm from "$lib/components/signup/signup-form.svelte";
    import mvImg from "$lib/assets/movie.jpg"
</script>

<div class="grid min-h-svh lg:grid-cols-2">
  <div class="flex flex-col gap-4 p-6 md:p-10">
    <div class="flex flex-1 items-center justify-center">
      <div class="w-full max-w-xs">
        <SignupForm />
      </div>
    </div>
  </div>
  <div class="bg-muted relative hidden lg:block">
    <img
      src={mvImg}
      alt="sign up side"
      class="absolute inset-0 h-full w-full object-cover dark:brightness-[0.8] dark:grayscale"
    />
  </div>
</div>
