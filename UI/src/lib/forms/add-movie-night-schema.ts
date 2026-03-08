import z from "zod";

const today = new Date();
// today.setHours(0, 0, 0, 0);

export const addMovieNightSchema = z.object({
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