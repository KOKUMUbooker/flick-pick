import type { Genre } from "./genre";

// export interface Movie {
//     id: number;
//     title: string;
//     genre: string;
//     imgUrl: string;
//     releaseDate: Date;
//     rating: number;
// }

export interface Movie {
    Id: number;
    Title: string;
    Minutes: number;
    Description: string;
    AgeRating: string;
    Genre: Genre;
    ImgUrl: string;
    ReleaseDate: Date;
    TrailerUrl: string;
    Rating: number;
    AddedBy: string;
    CreatedAt: Date;
    Verified: boolean
}

// Accurate DB versions
export interface MovieNightEvent {
    groupId: string,
    scheduledAt: string,
    createdById: string,
    isLocked: boolean,
    selectedMovieTmdbId?: number,
    movieSuggestions: MovieSuggestion[]
}

export interface MovieSuggestion {
    id: string,
    tmdbId: number,
    movieDetails: TMDBMovieDetails,
    suggestedBy: { fullName: string, email: string },
    votes: Vote[],
    isDisqualified: boolean,
}

export interface TMDBMovieDetails {
    tmdbId: number,
    title: string,
    overview: string,
    posterPath: string,
    runtime: number,
    releaseDate: string
}

export interface Vote {
    userId: string;
    movieSuggestionId: string;
    voteType: VoteType
    user: { fullName: string, email: string }
}

export enum VoteType {
    Upvote = 1,
    Downvote = 2,
    Veto = 3
}
