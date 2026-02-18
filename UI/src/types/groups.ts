export interface Group {
    id: number;
    name: string;
    description: string;
    createdById: number;
    members: Member[];
    unreadCount: number;
    lastActivity: string;
    isActive?: boolean;
    upcomingEventsCount: number;
}

export interface Member {
    id: number;
    name: string;
    isAdmin: boolean;
    avatar: string;
    joinedAt: string;
}

export interface MovieNightEvent {
    id: number;
    groupId: number;
    scheduledAt: string;
    isLocked: boolean;
    selectedMovieTmdbId?: number;
    selectedMovie?: MovieDetails;
    title?: string;
    suggestions: MovieSuggestion[];
    chatMessages: ChatMessage[];
    ratings: MovieRating[];
    participants: number;
}

export interface MovieSuggestion {
    id: number;
    tmdbId: number;
    movieDetails?: MovieDetails;
    suggestedBy: UserInfo;
    votes: Vote[];
    isDisqualified: boolean;
    score: number;
}

export type VoteType = 'Upvote' | 'Downvote' | 'Veto';

export interface Vote {
    id: number;
    userId: number;
    user: UserInfo;
    voteType: VoteType;
}

export interface ChatMessage {
    id: number;
    userId: number;
    user: UserInfo;
    message: string;
    createdAt: string;
}

export interface MovieRating {
    id: number;
    userId: number;
    user: UserInfo;
    rating: number;
    comment?: string;
}

export interface MovieDetails {
    tmdbId: number;
    title: string;
    overview: string;
    posterPath: string;
    runtime: number;
    releaseDate: string;
}

export interface UserInfo {
    id: number;
    name: string;
    avatar?: string;
}

export type BadgeVariants = 'default' | 'outline' | 'destructive' | 'secondary';

export type EventItem = {
    upcoming: MovieNightEvent[];
    past: MovieNightEvent[];
}
export type StatsItem = {
    moviesWatched: number;
    totalVotes: number;
    averageRating: number;
    streak: number;
}