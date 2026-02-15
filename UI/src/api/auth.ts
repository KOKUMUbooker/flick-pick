import type { User } from "../types";
import { API_BASE_URL } from "./urls";

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
export async function signUp(data: SignUpData): Promise<{ message: string }> {
    const res = await fetch(`${API_BASE_URL}/api/auth/sign-up`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
    });

    if (!res.ok) {
        throw new Error(`Failed to create user: ${res.status} ${res.statusText}`);
    }

    return res.json();
}

export interface LoginData {
    Email: string;
    Password: string
    ClientId: string;
}
export interface LoginRes {
    userDetails: User;
    emailVerificationToken: string
}
export async function logIn(data: LoginData): Promise<LoginRes> {
    const res = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
    });

    if (!res.ok) {
        throw new Error(`Failed to login user: ${res.status} ${res.statusText}`);
    }

    return res.json();
}

export interface PasswordResetData {
    PasswordVerificationToken: string;
    NewPassword: string
}
