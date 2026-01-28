<!-- src/routes/dashboard/reviews/+page.svelte -->
<script lang="ts">
  import { onMount } from 'svelte';
  import { dummyMovies } from '../../../data/movies';
  import { 
    MessageSquare,
    Star,
    Calendar,
    Film,
    ThumbsUp,
    Edit,
    Trash2,
    PlusCircle,
    Filter,
    Search
  } from '@lucide/svelte';
  
  import { Button } from '$lib/components/ui/button';
  import { Card, CardContent, CardHeader, CardTitle } from '$lib/components/ui/card';
  import { Input } from '$lib/components/ui/input';
  import { Badge } from '$lib/components/ui/badge';
  import { Separator } from '$lib/components/ui/separator';

  // User's reviews
  let reviews = [
    {
      id: 1,
      movie: dummyMovies[10], // Interstellar
      rating: 9.5,
      text: "Absolutely breathtaking. The visuals, music, and emotional depth make this a modern masterpiece. Nolan's best work.",
      likes: 24,
      date: new Date('2024-01-15'),
      edited: false
    },
    {
      id: 2,
      movie: dummyMovies[0], // Hereditary
      rating: 8.0,
      text: "Masterfully crafted horror that sticks with you. Toni Collette's performance is unforgettable.",
      likes: 18,
      date: new Date('2024-01-10'),
      edited: true
    },
    {
      id: 3,
      movie: dummyMovies[1], // A Quiet Place
      rating: 7.5,
      text: "Creative premise and excellent tension throughout. The family dynamics add emotional weight to the horror.",
      likes: 12,
      date: new Date('2024-01-05'),
      edited: false
    },
    {
      id: 4,
      movie: dummyMovies[13], // Parasite
      rating: 9.0,
      text: "Brilliant social commentary wrapped in a thrilling package. The class dynamics are portrayed with incredible precision.",
      likes: 31,
      date: new Date('2023-12-20'),
      edited: false
    }
  ];

  // State
  let searchQuery = '';
  let sortBy = 'recent'; // recent, rating, likes
  
  // New review
  let newReview = {
    movieId: '',
    rating: 8.0,
    text: '',
    showForm: false
  };

  // Filtered reviews
  $: filteredReviews = reviews.filter(review => {
    const matchesSearch = !searchQuery || 
      review.movie.Title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      review.text.toLowerCase().includes(searchQuery.toLowerCase());
    
    return matchesSearch;
  }).sort((a, b) => {
    switch(sortBy) {
      case 'rating':
        return b.rating - a.rating;
      case 'likes':
        return b.likes - a.likes;
      default: // recent
        return b.date.getTime() - a.date.getTime();
    }
  });

  // Stats
  $: stats = {
    totalReviews: reviews.length,
    avgRating: reviews.length > 0 
      ? (reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length).toFixed(1)
      : '0.0',
    totalLikes: reviews.reduce((sum, r) => sum + r.likes, 0),
    mostReviewedGenre: reviews.length > 0
      ? getMostCommonGenre()
      : 'None'
  };

  function getMostCommonGenre() {
    const genres = reviews.map(r => r.movie.Genre);
    const counts = {};
    genres.forEach(g => counts[g] = (counts[g] || 0) + 1);
    return Object.keys(counts).reduce((a, b) => counts[a] > counts[b] ? a : b);
  }

  const submitReview = () => {
    if (!newReview.text.trim()) return;
    
    const movie = dummyMovies.find(m => m.Id.toString() === newReview.movieId);
    if (!movie) return;

    const newReviewObj = {
      id: reviews.length + 1,
      movie: movie,
      rating: newReview.rating,
      text: newReview.text,
      likes: 0,
      date: new Date(),
      edited: false
    };

    reviews = [newReviewObj, ...reviews];
    newReview = { movieId: '', rating: 8.0, text: '', showForm: false };
  };

  const deleteReview = (id: number) => {
    if (confirm('Delete this review?')) {
      reviews = reviews.filter(r => r.id !== id);
    }
  };

  const editReview = (id: number) => {
    // In real app, this would open an edit form
    alert('Edit form would open for review #' + id);
  };
</script>

<svelte:head>
  <title>My Reviews</title>
</svelte:head>

<div class="space-y-6">
  <!-- Header -->
  <div class="flex items-center justify-between">
    <div>
      <h1 class="text-2xl font-bold">My Reviews</h1>
      <p class="text-muted-foreground">
        {reviews.length} review{reviews.length !== 1 ? 's' : ''} written
      </p>
    </div>
    <Button on:click={() => newReview.showForm = true}>
      <PlusCircle class="h-4 w-4 mr-2" />
      Write Review
    </Button>
  </div>

  <!-- Stats -->
  <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
    <Card>
      <CardContent class="pt-6 text-center">
        <div class="text-2xl font-bold">{stats.totalReviews}</div>
        <p class="text-sm text-muted-foreground">Total Reviews</p>
      </CardContent>
    </Card>
    <Card>
      <CardContent class="pt-6 text-center">
        <div class="text-2xl font-bold">{stats.avgRating}/10</div>
        <p class="text-sm text-muted-foreground">Avg Rating</p>
      </CardContent>
    </Card>
    <Card>
      <CardContent class="pt-6 text-center">
        <div class="text-2xl font-bold">{stats.totalLikes}</div>
        <p class="text-sm text-muted-foreground">Total Likes</p>
      </CardContent>
    </Card>
    <Card>
      <CardContent class="pt-6 text-center">
        <div class="text-2xl font-bold">{stats.mostReviewedGenre}</div>
        <p class="text-sm text-muted-foreground">Top Genre</p>
      </CardContent>
    </Card>
  </div>

  <!-- Search and Sort -->
  <Card>
    <CardContent class="pt-6">
      <div class="flex flex-col md:flex-row gap-4">
        <div class="flex-1 relative">
          <Search class="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input 
            bind:value={searchQuery}
            placeholder="Search reviews..."
            class="pl-10"
          />
        </div>
        
        <div class="flex gap-2">
          <Button 
            variant={sortBy === 'recent' ? 'default' : 'outline'} 
            size="sm"
            on:click={() => sortBy = 'recent'}
          >
            Recent
          </Button>
          <Button 
            variant={sortBy === 'rating' ? 'default' : 'outline'} 
            size="sm"
            on:click={() => sortBy = 'rating'}
          >
            Rating
          </Button>
          <Button 
            variant={sortBy === 'likes' ? 'default' : 'outline'} 
            size="sm"
            on:click={() => sortBy = 'likes'}
          >
            Most Liked
          </Button>
        </div>
      </div>
    </CardContent>
  </Card>

  <!-- New Review Form -->
  {#if newReview.showForm}
    <Card class="border-primary/20">
      <CardContent class="p-6">
        <div class="space-y-4">
          <div class="flex items-center justify-between">
            <h3 class="font-semibold">Write New Review</h3>
            <Button 
              variant="ghost" 
              size="sm"
              on:click={() => newReview.showForm = false}
            >
              Cancel
            </Button>
          </div>
          
          <div class="space-y-2">
            <label class="text-sm font-medium">Select Movie</label>
            <select 
              bind:value={newReview.movieId}
              class="w-full p-2 border rounded-md"
            >
              <option value="">Choose a movie...</option>
              {#each dummyMovies as movie}
                <option value={movie.Id}>{movie.Title} ({movie.ReleaseDate.getFullYear()})</option>
              {/each}
            </select>
          </div>
          
          <div class="space-y-2">
            <label class="text-sm font-medium">Your Rating: {newReview.rating}/10</label>
            <div class="flex items-center gap-2">
              <input 
                type="range" 
                min="1" 
                max="10" 
                step="0.5"
                bind:value={newReview.rating}
                class="flex-1"
              />
              <div class="flex items-center gap-1">
                <Star class="h-4 w-4 text-yellow-500 fill-current" />
                <span class="font-medium">{newReview.rating}</span>
              </div>
            </div>
          </div>
          
          <div class="space-y-2">
            <label class="text-sm font-medium">Review Text</label>
            <textarea 
              bind:value={newReview.text}
              placeholder="Share your thoughts about this movie..."
              rows="3"
              class="w-full p-2 border rounded-md"
            />
          </div>
          
          <Button 
            class="w-full" 
            on:click={submitReview}
            disabled={!newReview.movieId || !newReview.text.trim()}
          >
            <MessageSquare class="h-4 w-4 mr-2" />
            Post Review
          </Button>
        </div>
      </CardContent>
    </Card>
  {/if}

  <!-- Reviews List -->
  <div class="space-y-4">
    {#if filteredReviews.length === 0}
      <Card>
        <CardContent class="py-12 text-center">
          <MessageSquare class="h-12 w-12 text-muted-foreground mx-auto mb-4 opacity-50" />
          <h3 class="text-lg font-semibold mb-2">No reviews found</h3>
          <p class="text-muted-foreground mb-4">
            {searchQuery ? 'Try a different search' : 'Write your first review'}
          </p>
          <Button on:click={() => newReview.showForm = true}>
            <PlusCircle class="h-4 w-4 mr-2" />
            Write Review
          </Button>
        </CardContent>
      </Card>
    {:else}
      {#each filteredReviews as review}
        <Card class="hover:bg-accent/50 transition-colors">
          <CardContent class="p-6">
            <!-- Movie Header -->
            <div class="flex items-start gap-4 mb-4">
              <img
                src={review.movie.ImgUrl}
                alt={review.movie.Title}
                class="w-16 h-24 object-cover rounded"
              />
              <div class="flex-1">
                <div class="flex items-start justify-between">
                  <div>
                    <h3 class="font-semibold text-lg">{review.movie.Title}</h3>
                    <div class="flex items-center gap-2 mt-1">
                      <Badge variant="outline" class="text-xs">
                        {review.movie.Genre}
                      </Badge>
                      <span class="text-sm text-muted-foreground">
                        {review.movie.ReleaseDate.getFullYear()}
                      </span>
                    </div>
                  </div>
                  
                  <div class="flex items-center gap-1">
                    {#each Array.from({ length: 10 }) as _, i}
                      <Star 
                        class="h-4 w-4 {i < Math.floor(review.rating) ? 'text-yellow-500 fill-current' : 'text-gray-300'}" 
                      />
                    {/each}
                    <span class="font-medium ml-2">{review.rating}/10</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Review Text -->
            <div class="mb-4">
              <p class="text-gray-700 dark:text-gray-300">{review.text}</p>
            </div>

            <!-- Review Metadata -->
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-4 text-sm text-muted-foreground">
                <div class="flex items-center gap-1">
                  <Calendar class="h-4 w-4" />
                  <span>{review.date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })}</span>
                  {#if review.edited }
                    <Badge variant="outline" class="ml-2 text-xs">Edited</Badge>
                  {/if}
                </div>
                
                <div class="flex items-center gap-1">
                  <ThumbsUp class="h-4 w-4" />
                  <span>{review.likes} like{review.likes !== 1 ? 's' : ''}</span>
                </div>
              </div>

              <!-- Actions -->
              <div class="flex gap-2">
                <Button 
                  variant="ghost" 
                  size="sm"
                  on:click={() => editReview(review.id)}
                >
                  <Edit class="h-4 w-4 mr-1" />
                  Edit
                </Button>
                <Button 
                  variant="ghost" 
                  size="sm"
                  class="text-destructive"
                  on:click={() => deleteReview(review.id)}
                >
                  <Trash2 class="h-4 w-4 mr-1" />
                  Delete
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>
      {/each}
    {/if}
  </div>

  <!-- Review Tips -->
  <Card class="bg-primary/5 border-primary/10">
    <CardContent class="p-6">
      <div class="flex items-start gap-3">
        <MessageSquare class="h-5 w-5 text-primary mt-0.5" />
        <div>
          <h3 class="font-semibold mb-2">Writing Great Reviews</h3>
          <ul class="text-sm text-muted-foreground space-y-1">
            <li>• Be specific about what you liked or disliked</li>
            <li>• Mention standout performances or technical aspects</li>
            <li>• Avoid spoilers or use spoiler warnings</li>
            <li>• Share how the movie made you feel</li>
          </ul>
        </div>
      </div>
    </CardContent>
  </Card>
</div>