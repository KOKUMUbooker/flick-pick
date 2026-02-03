<!-- src/routes/dashboard/admin-movies/+page.svelte -->
<script lang="ts">
	import { onMount } from 'svelte';
	import { dummyMovies } from '../../../../data/movies';
	import {
		Film,
		CheckCircle,
		XCircle,
		AlertCircle,
		Search,
		Filter,
		User,
		Calendar,
		Eye,
		Edit,
		Trash2,
		Download,
		Clock,
		Star
	} from '@lucide/svelte';

	import { Button } from '$lib/components/ui/button';
	import { Card, CardContent, CardHeader, CardTitle } from '$lib/components/ui/card';
	import { Input } from '$lib/components/ui/input';
	import { Badge } from '$lib/components/ui/badge';
	import { Separator } from '$lib/components/ui/separator';

	// State
	let allMovies = [];
	let searchQuery = '';
	let filterStatus = 'all'; // all, pending, Verified, rejected
	let selectedMovie = null;

	onMount(() => {
		allMovies = [...dummyMovies].map((movie) => ({
			...movie,
			status: movie.Verified ? 'Verified' : Math.random() > 0.7 ? 'pending' : 'Verified',
			submittedBy: movie.AddedBy === 'admin' ? 'Admin' : 'User'
		}));
	});

	// Filtered movies
	$: filteredMovies = allMovies.filter((movie) => {
		const matchesSearch =
			!searchQuery ||
			movie.Title.toLowerCase().includes(searchQuery.toLowerCase()) ||
			movie.Genre.toLowerCase().includes(searchQuery.toLowerCase());

		const matchesStatus = filterStatus === 'all' || movie.status === filterStatus;

		return matchesSearch && matchesStatus;
	});

	// Stats
	$: stats = {
		total: allMovies.length,
		pending: allMovies.filter((m) => m.status === 'pending').length,
		Verified: allMovies.filter((m) => m.status === 'Verified').length,
		rejected: allMovies.filter((m) => m.status === 'rejected').length || 0
	};

	// Helper functions
	const formatDate = (date: Date) => {
		return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
	};

	const formatDuration = (minutes: number) => {
		const hours = Math.floor(minutes / 60);
		const mins = minutes % 60;
		return hours > 0 ? `${hours}h ${mins}m` : `${mins}m`;
	};

	const approveMovie = (movieId: number) => {
		allMovies = allMovies.map((m) =>
			m.Id === movieId ? { ...m, status: 'Verified', Verified: true } : m
		);
	};

	const rejectMovie = (movieId: number) => {
		allMovies = allMovies.map((m) =>
			m.Id === movieId ? { ...m, status: 'rejected', Verified: false } : m
		);
	};

	const deleteMovie = (movieId: number) => {
		if (confirm('Permanently delete this movie?')) {
			allMovies = allMovies.filter((m) => m.Id !== movieId);
			selectedMovie = null;
		}
	};

	const viewMovieDetails = (movie: any) => {
		selectedMovie = movie;
	};

	const closeDetails = () => {
		selectedMovie = null;
	};

	const exportMovies = () => {
		alert(`Exported ${allMovies.length} movies`);
	};
</script>

<svelte:head>
	<title>Admin - Movies Management</title>
</svelte:head>

<div class="space-y-6">
	<!-- Header -->
	<div class="flex items-center justify-between">
		<div>
			<h1 class="text-2xl font-bold">Movies Management</h1>
			<p class="text-muted-foreground">Manage all movies in the system</p>
		</div>
		<Button variant="outline" on:click={exportMovies}>
			<Download class="mr-2 h-4 w-4" />
			Export
		</Button>
	</div>

	<!-- Stats -->
	<div class="grid grid-cols-2 gap-4 md:grid-cols-4">
		<Card>
			<CardContent class="pt-6">
				<div class="text-center">
					<div class="text-2xl font-bold">{stats.total}</div>
					<p class="text-sm text-muted-foreground">Total Movies</p>
				</div>
			</CardContent>
		</Card>
		<Card>
			<CardContent class="pt-6">
				<div class="text-center">
					<div class="text-2xl font-bold text-yellow-500">{stats.pending}</div>
					<p class="text-sm text-muted-foreground">Pending Review</p>
				</div>
			</CardContent>
		</Card>
		<Card>
			<CardContent class="pt-6">
				<div class="text-center">
					<div class="text-2xl font-bold text-green-500">{stats.Verified}</div>
					<p class="text-sm text-muted-foreground">Verified</p>
				</div>
			</CardContent>
		</Card>
		<Card>
			<CardContent class="pt-6">
				<div class="text-center">
					<div class="text-2xl font-bold text-red-500">{stats.rejected}</div>
					<p class="text-sm text-muted-foreground">Rejected</p>
				</div>
			</CardContent>
		</Card>
	</div>

	<!-- Search and Filter -->
	<Card>
		<CardContent class="pt-6">
			<div class="flex flex-col gap-4 md:flex-row">
				<div class="relative flex-1">
					<Search
						class="absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 transform text-muted-foreground"
					/>
					<Input
						bind:value={searchQuery}
						placeholder="Search movies by title or genre..."
						class="pl-10"
					/>
				</div>

				<div class="flex gap-2">
					<Button
						variant={filterStatus === 'all' ? 'default' : 'outline'}
						size="sm"
						on:click={() => (filterStatus = 'all')}
					>
						All ({stats.total})
					</Button>
					<Button
						variant={filterStatus === 'pending' ? 'default' : 'outline'}
						size="sm"
						on:click={() => (filterStatus = 'pending')}
					>
						<AlertCircle class="mr-1 h-4 w-4" />
						Pending ({stats.pending})
					</Button>
					<Button
						variant={filterStatus === 'Verified' ? 'default' : 'outline'}
						size="sm"
						on:click={() => (filterStatus = 'Verified')}
					>
						<CheckCircle class="mr-1 h-4 w-4" />
						Verified ({stats.Verified})
					</Button>
				</div>
			</div>
		</CardContent>
	</Card>

	<!-- Movies List -->
	<div class="space-y-4">
		{#if filteredMovies.length === 0}
			<Card>
				<CardContent class="py-12 text-center">
					<Film class="mx-auto mb-4 h-12 w-12 text-muted-foreground opacity-50" />
					<h3 class="mb-2 text-lg font-semibold">No movies found</h3>
					<p class="text-muted-foreground">
						{searchQuery ? 'Try a different search' : 'No movies match the current filter'}
					</p>
				</CardContent>
			</Card>
		{:else}
			{#each filteredMovies as movie}
				<Card class="transition-colors hover:bg-accent/50">
					<CardContent class="p-6">
						<div class="flex items-start gap-4">
							<!-- Movie Poster -->
							<img src={movie.ImgUrl} alt={movie.Title} class="h-24 w-16 rounded object-cover" />

							<!-- Movie Details -->
							<div class="flex-1">
								<div class="mb-3 flex items-start justify-between">
									<div>
										<div class="mb-1 flex items-center gap-2">
											<h3 class="text-lg font-semibold">{movie.Title}</h3>
											<Badge
												variant={movie.status === 'Verified'
													? 'default'
													: movie.status === 'pending'
														? 'outline'
														: 'destructive'}
											>
												{movie.status === 'Verified'
													? 'Verified'
													: movie.status === 'pending'
														? 'Pending Review'
														: 'Rejected'}
											</Badge>
										</div>

										<div class="mb-2 flex items-center gap-3 text-sm text-muted-foreground">
											<span>{movie.Genre}</span>
											<span>•</span>
											<span>{movie.ReleaseDate.getFullYear()}</span>
											<span>•</span>
											<div class="flex items-center gap-1">
												<Star class="h-3 w-3 fill-current text-yellow-500" />
												<span>{movie.Rating.toFixed(1)}</span>
											</div>
										</div>
									</div>

									<div class="flex items-center gap-1">
										<Button variant="ghost" size="sm" on:click={() => viewMovieDetails(movie)}>
											<Eye class="h-4 w-4" />
										</Button>
										<Button variant="ghost" size="sm">
											<Edit class="h-4 w-4" />
										</Button>
									</div>
								</div>

								<!-- Metadata -->
								<div class="flex items-center gap-4 text-sm text-muted-foreground">
									<div class="flex items-center gap-1">
										<User class="h-4 w-4" />
										<span>Added by: {movie.submittedBy}</span>
									</div>
									<div class="flex items-center gap-1">
										<Calendar class="h-4 w-4" />
										<span>{formatDate(movie.CreatedAt)}</span>
									</div>
									<div class="flex items-center gap-1">
										<Clock class="h-4 w-4" />
										<span>{formatDuration(movie.Minutes)}</span>
									</div>
								</div>

								<!-- Actions for pending movies -->
								{#if movie.status === 'pending'}
									<div class="mt-4 flex gap-2">
										<Button size="sm" class="gap-2" on:click={() => approveMovie(movie.Id)}>
											<CheckCircle class="h-4 w-4" />
											Approve
										</Button>
										<Button
											variant="destructive"
											size="sm"
											class="gap-2"
											on:click={() => rejectMovie(movie.Id)}
										>
											<XCircle class="h-4 w-4" />
											Reject
										</Button>
									</div>
								{:else if movie.status === 'rejected'}
									<div class="mt-4 flex gap-2">
										<Button size="sm" class="gap-2" on:click={() => approveMovie(movie.Id)}>
											<CheckCircle class="h-4 w-4" />
											Re-approve
										</Button>
										<Button
											variant="destructive"
											size="sm"
											class="gap-2"
											on:click={() => deleteMovie(movie.Id)}
										>
											<Trash2 class="h-4 w-4" />
											Delete
										</Button>
									</div>
								{:else}
									<div class="mt-4 flex gap-2">
										<Button variant="outline" size="sm" on:click={() => (movie.status = 'pending')}>
											Mark for Review
										</Button>
										<Button
											variant="destructive"
											size="sm"
											class="gap-2"
											on:click={() => deleteMovie(movie.Id)}
										>
											<Trash2 class="h-4 w-4" />
											Delete
										</Button>
									</div>
								{/if}
							</div>
						</div>
					</CardContent>
				</Card>
			{/each}
		{/if}
	</div>
</div>

<!-- Movie Details Modal -->
{#if selectedMovie}
	<div class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
		<Card class="max-h-[90vh] w-full max-w-2xl overflow-y-auto">
			<CardHeader>
				<div class="flex items-start justify-between">
					<CardTitle>{selectedMovie.Title}</CardTitle>
					<Button variant="ghost" size="sm" on:click={closeDetails}>Close</Button>
				</div>
			</CardHeader>
			<CardContent class="space-y-6">
				<!-- Movie Poster -->
				<div class="flex justify-center">
					<img
						src={selectedMovie.ImgUrl}
						alt={selectedMovie.Title}
						class="h-72 w-48 rounded-lg object-cover"
					/>
				</div>

				<!-- Movie Info -->
				<div class="grid grid-cols-2 gap-4">
					<div>
						<h4 class="text-sm font-medium text-muted-foreground">Genre</h4>
						<p>{selectedMovie.Genre}</p>
					</div>
					<div>
						<h4 class="text-sm font-medium text-muted-foreground">Release Year</h4>
						<p>{selectedMovie.ReleaseDate.getFullYear()}</p>
					</div>
					<div>
						<h4 class="text-sm font-medium text-muted-foreground">Duration</h4>
						<p>{formatDuration(selectedMovie.Minutes)}</p>
					</div>
					<div>
						<h4 class="text-sm font-medium text-muted-foreground">Rating</h4>
						<p>{selectedMovie.Rating.toFixed(1)}/10</p>
					</div>
					<div>
						<h4 class="text-sm font-medium text-muted-foreground">Age Rating</h4>
						<p>{selectedMovie.AgeRating}</p>
					</div>
					<div>
						<h4 class="text-sm font-medium text-muted-foreground">Status</h4>
						<Badge
							variant={selectedMovie.status === 'Verified'
								? 'default'
								: selectedMovie.status === 'pending'
									? 'outline'
									: 'destructive'}
						>
							{selectedMovie.status}
						</Badge>
					</div>
					<div>
						<h4 class="text-sm font-medium text-muted-foreground">Added By</h4>
						<p>{selectedMovie.submittedBy}</p>
					</div>
					<div>
						<h4 class="text-sm font-medium text-muted-foreground">Added On</h4>
						<p>{formatDate(selectedMovie.CreatedAt)}</p>
					</div>
				</div>

				<!-- Description -->
				<div>
					<h4 class="mb-2 text-sm font-medium text-muted-foreground">Description</h4>
					<p class="text-gray-700 dark:text-gray-300">{selectedMovie.Description}</p>
				</div>

				<!-- Admin Actions -->
				<Separator />

				<div class="flex gap-3">
					{#if selectedMovie.status === 'pending'}
						<Button
							class="flex-1"
							on:click={() => {
								approveMovie(selectedMovie.Id);
								closeDetails();
							}}
						>
							<CheckCircle class="mr-2 h-4 w-4" />
							Approve Movie
						</Button>
						<Button
							variant="destructive"
							class="flex-1"
							on:click={() => {
								rejectMovie(selectedMovie.Id);
								closeDetails();
							}}
						>
							<XCircle class="mr-2 h-4 w-4" />
							Reject Movie
						</Button>
					{:else}
						<Button variant="outline" class="flex-1">
							<Edit class="mr-2 h-4 w-4" />
							Edit Details
						</Button>
						<Button
							variant="destructive"
							class="flex-1"
							on:click={() => {
								deleteMovie(selectedMovie.Id);
								closeDetails();
							}}
						>
							<Trash2 class="mr-2 h-4 w-4" />
							Delete Movie
						</Button>
					{/if}
				</div>
			</CardContent>
		</Card>
	</div>
{/if}
