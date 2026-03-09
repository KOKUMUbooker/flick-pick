export interface TmdbSearchResponse {
    page: number
    results: TmdbMovieResult[]
    totalPages: number
    totalResults: number
}

export interface TmdbMovieResult {
    tmdbId: number
    title: string
    posterPath: string | null
    releaseDate: string | null
    overview: string
    voteAverage: number
}


// public int TmdbId { get; set; }
// public string Title { get; set; } = null!;
// public string PosterPath { get; set; } = null!;
// public DateTime? ReleaseDate { get; set; }
// public string? Overview { get; set; }
// public double? VoteAverage { get; set; }

export interface MovieSuggestion {
    id: string
    movieId: number
    title: string
    posterPath: string | null
    releaseDate: string | null
    overview: string
    voteAverage: number
    suggestedBy: string
    suggestedAt: Date
    votes: number
    userVoted?: boolean
}

export interface MovieNightSuggestion extends MovieSuggestion {
    movieNightId: string
}

export interface SuggestionFlowProps {
    movieNightId: string
    onCancel: () => void
    onSuggestionAdded: (suggestion: MovieSuggestion) => void
}

export type SearchState = 'idle' | 'searching' | 'results' | 'confirming' | 'success'