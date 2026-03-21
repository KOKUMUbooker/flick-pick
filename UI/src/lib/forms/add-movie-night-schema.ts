import { CalendarDate } from "@internationalized/date";
import z from "zod";
import type { AddMovieNightEventFormData } from "../../api/types";
import type { MovieNightEvent } from "../../types";

const today = new Date();
// today.setHours(0, 0, 0, 0);

export const addMovieNightSchema = z.object({
    Id: z
        .string()
        .optional(),
    Name: z
        .string()
        .min(1, 'Name is required')
        .max(64, 'Name must be less than 64 characters')
        .trim(),
    Description: z
        .string()
        .optional(),
    ScheduledAt: z
        .date()
        .refine(date => date >= today, {
            message: "Scheduled date cannot be a date & time earlier than current time"
        })
})

export function getDefaultMovieEventFormData(data?: MovieNightEvent): AddMovieNightEventFormData {
    const today = new Date();
    let ScheduledTime = '21:00';
    let ScheduledDate = new CalendarDate(today.getFullYear(), today.getMonth() + 1, today.getDate());

    if (data && data.scheduledAt) {
        const sDate = new Date(data.scheduledAt);
        let mins = `${sDate.getMinutes()}`
        if (mins.length == 1) mins = '0' + mins;
        ScheduledTime = sDate.getHours() + ":" + mins
        ScheduledDate = new CalendarDate(sDate.getFullYear(), sDate.getMonth() + 1, sDate.getDate());
    }

    return {
        Id: data?.id || "",
        Name: data?.name || '',
        Description: data?.description || '',
        ScheduledDate,
        ScheduledTime,
        ScheduledAt: data?.scheduledAt ? new Date(data.scheduledAt) : new Date()
    }
}