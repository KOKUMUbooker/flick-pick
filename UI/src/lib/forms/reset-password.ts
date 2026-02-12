import { z } from 'zod';

export const resetPasswordBaseSchema = z.object({
    NewPassword: z
        .string()
        .min(8, 'Password must be at least 8 characters')
        .max(32, 'Password must be less than 32 characters')
        .trim(),

    PasswordConfirm: z
        .string()
});

export const resetPasswordSchema = resetPasswordBaseSchema.superRefine(
    ({ NewPassword, PasswordConfirm }, ctx) => {
        if (NewPassword !== PasswordConfirm) {
            ctx.addIssue({
                code: 'custom',
                message: 'New Password and Confirm Password must match',
                path: ['PasswordConfirm'],
            });
        }
    }
);
