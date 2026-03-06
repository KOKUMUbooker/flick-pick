import type { UserDto } from "./movie";

export interface DBGroup {
    id: string;
    name: string;
    description?: string;
    createdById: string
}

export interface Member {
    id: number;
    name: string;
    isAdmin: boolean;
    joinedAt: string;
}

export interface ChatMessage {
    id: number;
    userId: number;
    user: UserDto;
    message: string;
    sentAt: string;
}

export interface MovieRating {
    id: number;
    userId: number;
    user: UserDto;
    rating: number;
    comment?: string;
    movieNightEventId: string
}

export type BadgeVariants = 'default' | 'outline' | 'destructive' | 'secondary';

export type StatsItem = {
    moviesWatched: number;
    totalVotes: number;
    averageRating: number;
    streak: number;
}