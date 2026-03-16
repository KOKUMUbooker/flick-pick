export interface FetchedMovieSuggestion {
    id: string;
    movieId: number;
    suggestedBy: User;
    movieNightEventId: string;
    isDisqualified: boolean;
    movie: Movie;
    votes: Vote[];
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
    voteType: number;
    user: User;
}

export interface User {
    fullName: string;
    email: string;
}