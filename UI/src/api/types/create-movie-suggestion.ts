export interface TmdbSearchResponse {
    page: number
    results: TmdbMovieResult[]
    total_pages: number
    total_results: number
}

export interface TmdbMovieResult {
    id: number
    title: string
    poster_path: string | null
    release_date: string | null
    overview: string
    vote_average: number
}

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