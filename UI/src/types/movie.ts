export interface MovieNightEvent {
    id: string;
    name: string,
    description?: string,
    groupId: string,
    scheduledAt: string,
    createdById: string,
    isLocked: boolean,
    selectedMovieTmdbId?: number,
    movieSuggestions: MovieSuggestion[],
    movieNightRatings: MovieNightRating[],
    selectedMovie: TMDBMovieDetails
}

export interface MovieNightRating {
    MovieNightEventId: string;
    UserId: string;
    Rating: string;
    Comment?: string
}

export interface UserDto {
    fullName: string;
    email: string;
}

export interface MovieSuggestion {
    id: string,
    tmdbId: number,
    movieDetails: TMDBMovieDetails,
    suggestedBy: UserDto,
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
    user: UserDto
}

export enum VoteType {
    Upvote = 1,
    Downvote = 2,
    Veto = 3
}
