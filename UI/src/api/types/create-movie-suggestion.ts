import type { DBGroup } from "../../types"

export interface TmdbSearchResponse {
    page: number
    results: TmdbMovieResult[]
    totalPages: number
    totalResults: number
}

export interface TmdbMovieResult {
    tmdbId: number;
    title: string;
    posterPath?: string;
    releaseDate?: string
    overview?: string
    voteAverage?: number
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
    movieEventId: string;
    movieNightId: string
    onCancel: () => void
    onSuggestionAdded: () => void
}

export type SearchState = 'idle' | 'searching' | 'results' | 'confirming' | 'success'