import { z } from 'zod';

export const forgotPasswordSchema = z.object({
    Email: z
        .string()
        .min(1, 'Email is required')
        .max(64, 'Email must be less than 64 characters')
        .email('Email must be a valid email address')
});

