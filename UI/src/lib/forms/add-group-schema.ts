import z from "zod";

export const addGroupSchema = z.object({
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
        .optional()
})