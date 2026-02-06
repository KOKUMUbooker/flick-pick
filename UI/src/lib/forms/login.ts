import { z } from 'zod';

export const loginSchema = z.object({
    Email: z
        .string()
        .min(1, 'Email is required')
        .max(64, 'Email must be less than 64 characters')
        .email('Email must be a valid email address'),

    Password: z
        .string()
        .min(8, 'Password must be at least 8 characters')
        .max(32, 'Password must be less than 32 characters')
        .trim(),
});

