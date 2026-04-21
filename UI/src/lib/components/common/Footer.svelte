<script lang="ts">
	import tmdbLogo from '$lib/assets/TMDB-logo.svg';
	import { Github, Globe, Heart, Linkedin, Mail, ArrowUpRight, Sparkles } from '@lucide/svelte';
	import { Button } from '$lib/components/ui/button';
	import logo from '../../assets/logo.png';
</script>

<footer
	class="relative z-20 border-t border-border/50 bg-linear-to-b from-background to-muted/90 px-4 py-12"
>
	<div class="container mx-auto max-w-6xl">
		<div class="grid gap-8 md:grid-cols-3">
			<!-- Brand Column -->
			<div class="flex flex-col items-center gap-3 text-center md:items-start md:text-left">
				<div class="flex items-center gap-2">
					<!-- <Sparkles class="h-5 w-5 text-primary" /> -->
					<img src={logo} alt="WatchHive logo" class="h-16" />
					<span class="text-lg font-bold">WatchHive</span>
				</div>
				<p class="text-sm text-muted-foreground">
					Stop debating, start watching. Fair movie selection for groups.
				</p>
				<p class="text-xs text-muted-foreground/70">
					© {new Date().getFullYear()} All rights reserved.
				</p>
			</div>

			<!-- Creator Column -->
			<div class="flex flex-col items-center gap-3 text-center">
				<p class="text-xs font-medium tracking-wider text-muted-foreground uppercase">Created By</p>
				<div class="flex items-center gap-2">
					<div class="flex h-8 w-8 items-center justify-center rounded-full bg-primary/10">
						<span class="text-sm font-bold text-primary">BO</span>
					</div>
					<div class="text-left">
						<p class="font-semibold text-foreground">Booker Ochieng</p>
						<p class="flex items-center gap-1 text-xs text-muted-foreground">
							<span>Full Stack Developer</span>
							<span class="inline-flex items-center">
								<Heart class="h-3 w-3 fill-red-500 text-red-500" />
							</span>
						</p>
					</div>
				</div>

				<!-- Social Links -->
				<div class="mt-2 flex flex-wrap items-center justify-center gap-1">
					<Button
						href="https://github.com/KOKUMUbooker"
						target="_blank"
						variant="outline"
						rel="noopener noreferrer"
						class="inline-flex items-center gap-1.5 rounded-md px-2 py-1 text-xs text-muted-foreground transition-colors hover:bg-muted hover:text-foreground"
					>
						<Github class="h-3.5 w-3.5" />
						<span>GitHub</span>
						<ArrowUpRight class="h-3 w-3" />
					</Button>
					<Button
						href="https://booker-portfolio.netlify.app/"
						target="_blank"
						variant="outline"
						rel="noopener noreferrer"
						class="inline-flex items-center gap-1.5 rounded-md px-2 py-1 text-xs text-muted-foreground transition-colors hover:bg-muted hover:text-foreground"
					>
						<Globe class="h-3.5 w-3.5" />
						<span>Portfolio</span>
						<ArrowUpRight class="h-3 w-3" />
					</Button>
					<Button
						href="mailto:bookerochieng20@gmail.com"
						variant="outline"
						class="inline-flex items-center gap-1.5 rounded-md px-2 py-1 text-xs text-muted-foreground transition-colors hover:bg-muted hover:text-foreground"
					>
						<Mail class="h-3.5 w-3.5" />
						<span>Email</span>
					</Button>
					<Button
						href="https://www.linkedin.com/in/booker-ochieng"
						target="_blank"
						variant="outline"
						rel="noopener noreferrer"
						class="inline-flex items-center gap-1.5 rounded-md px-2 py-1 text-xs text-muted-foreground transition-colors hover:bg-muted hover:text-foreground"
					>
						<Linkedin class="h-3.5 w-3.5" />
						<span>LinkedIn</span>
						<ArrowUpRight class="h-3 w-3" />
					</Button>
				</div>
			</div>

			<!-- Attribution Column -->
			<div class="flex flex-col items-center gap-3 text-center md:items-end">
				<p class="text-xs font-medium tracking-wider text-muted-foreground uppercase">Powered By</p>
				<a
					href="https://www.themoviedb.org/"
					target="_blank"
					rel="noopener noreferrer"
					class="group flex flex-col items-center gap-2 rounded-lg border border-border/50 bg-background/50 p-3 backdrop-blur-sm transition-all hover:border-primary/30 hover:bg-background/80"
				>
					<img
						src={tmdbLogo}
						alt="TMDB Logo"
						class="h-5 transition-transform group-hover:scale-105"
					/>
					<span class="max-w-50 text-xs text-balance text-muted-foreground">
						This product uses the TMDB API but is not endorsed or certified by TMDB.
					</span>
				</a>
			</div>
		</div>

		<!-- Bottom Bar - Mobile Optimized Alternative -->
		<div class="mt-8 border-t border-border/30 pt-6 md:hidden">
			<div class="flex flex-col items-center gap-3 text-center">
				<p class="text-xs text-muted-foreground">
					Built with <Heart class="inline h-3 w-3 fill-red-500 text-red-500" /> by Booker Ochieng
				</p>
				<div class="flex items-center justify-center gap-4">
					<a
						href="https://github.com/KOKUMUbooker"
						target="_blank"
						rel="noopener noreferrer"
						class="text-muted-foreground transition-colors hover:text-foreground"
						aria-label="GitHub"
					>
						<Github class="h-5 w-5" />
					</a>
					<a
						href="https://booker-portfolio.netlify.app/"
						target="_blank"
						rel="noopener noreferrer"
						class="text-muted-foreground transition-colors hover:text-foreground"
						aria-label="Portfolio"
					>
						<Globe class="h-5 w-5" />
					</a>
					<a
						href="mailto:bookerochieng20@gmail.com"
						class="text-muted-foreground transition-colors hover:text-foreground"
						aria-label="Email"
					>
						<Mail class="h-5 w-5" />
					</a>
					<a
						href="https://www.linkedin.com/in/booker-ochieng"
						target="_blank"
						rel="noopener noreferrer"
						class="text-muted-foreground transition-colors hover:text-foreground"
						aria-label="LinkedIn"
					>
						<Linkedin class="h-5 w-5" />
					</a>
				</div>
			</div>
		</div>
	</div>
</footer>
