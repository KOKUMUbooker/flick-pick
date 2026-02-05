import { API_BASE_URL } from "./urls";

export const AUTH_CACHE_KEY = "auth" as const;

interface SignUpData {
    FullName: string;
    Email: string;
    Password: string
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

interface LoginData {
    Email: string;
    Password: string
    ClientId: string;
}
interface LoginRes {
    AccessToken: string;
    AccessTokenExpiresAt: Date;
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

