import type { VoteType } from "../../types";

export interface FetchedMovieSuggestion {
    id: string;
    movieId: number;
    suggestedBy: User;
    movieNightEventId: string;
    isDisqualified: boolean;
    movie: Movie;
    upvoteCount: number;
    downVoteCount: number;
    userVote?: VoteType
}

export interface Movie {
    tmdbId: number;
    title: string;
    posterPath: string;
    overview: string;
    releaseDate: string; // ISO datetime string
    voteAverage: number;
}

export interface Vote {
    Id: string;
    voteType: number;
    user: User;
    movieSuggestionId: string;
    movieEventId: string;
}

export interface User {
    fullName: string;
    email: string;
}