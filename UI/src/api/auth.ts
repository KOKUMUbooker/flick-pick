import type { User } from "../types";

export const AUTH_CACHE_KEY = "auth" as const;

export interface SignUpData {
    FullName: string;
    Email: string;
    Password: string
}
export interface SignUpRes {
    message: string;
    emailVerificationToken: string;
}

export interface LoginData {
    Email: string;
    Password: string
    ClientId: string;
}

export interface AuthResponseData {
    userDetails: User;
    accessToken: string;
    refreshToken: string;
    accessTokenExpiresAt: string;
}

export interface LoginRes extends AuthResponseData {
    emailVerificationToken: string;
}

export interface PasswordResetData {
    PasswordVerificationToken: string;
    NewPassword: string
}
