import type { CalendarDate } from "@internationalized/date";

export interface MovieNightEventFormData {
    Id?: string;
    Name: string;
    Description: string;
}

export interface AddMovieNightEventFormData extends MovieNightEventFormData {
    ScheduledDate: CalendarDate | undefined;
    ScheduledTime: string
    ScheduledAt: Date
}

export interface AddMovieNightEventData extends MovieNightEventFormData {
    CreatedById: string;
    ScheduledAt: string;
    ConnectionId: string
}

export interface AddMovieNightEventRes {
    message: string;
    movieNightId: string;
}

export interface DeleteMovieEventData {
    Initiator: string;
    ConnectionId: string
}