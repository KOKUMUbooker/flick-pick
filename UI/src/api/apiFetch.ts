import { ACCESS_TOKEN_STORE_NAME, logIn, logOut } from "../store";
import { ApiError } from "../types/error";
import type { AuthResponseData } from "./auth";

let isRefreshing = false;
let refreshPromise: Promise<AuthResponseData> | null = null;

export async function apiFetch<T>(
    url: string,
    options?: RequestInit,
    retry = true
): Promise<T> {

    const res = await fetch(url, {
        // credentials: "include", // REQUIRED for HttpOnly cookies
        headers: {
            "Content-Type": "application/json",
            'Authorization': `Bearer ${localStorage.getItem(ACCESS_TOKEN_STORE_NAME)}`,
            ...options?.headers
        },
        ...options
    });

    // If unauthorized and allowed to retry → attempt refresh
    if (res.status === 401 && retry && !url.includes("/api/auth/refresh") && !url.includes("/api/auth/login")) {
        return handleRefreshAndRetry<T>(url, options);
    }

    if (!res.ok) {
        throw await buildApiError(res);
    }

    return res.json();
}

async function refreshToken(): Promise<AuthResponseData> {
    const res = await fetch("/auth/refresh", {
        method: "POST",
        headers: {
            'Authorization': `Bearer ${localStorage.getItem(ACCESS_TOKEN_STORE_NAME)}`,
            'Content-Type': 'application/json'
        }
        // credentials: "include"
    });

    if (!res.ok) {
        // Refresh failed → session dead
        logOut();
        window.location.href = "/login";
        throw new Error("Session expired");
    }

    return await res.json()
}

async function handleRefreshAndRetry<T>(
    url: string,
    options?: RequestInit
): Promise<T> {

    if (!isRefreshing) {
        isRefreshing = true;

        refreshPromise = refreshToken()
            .finally(() => {
                isRefreshing = false;
            });
    }

    const authRes = await refreshPromise;
    if (authRes != null) logIn(authRes)

    // Retry original request (IMPORTANT: retry = false)
    return apiFetch<T>(url, options, false);
}

async function buildApiError(res: Response) {
    try {
        const errorData = await res.json();

        return new ApiError(
            errorData.message || "Request failed",
            errorData.error,
            res.status
        );
    } catch {
        const text = await res.text();
        return new ApiError(text || "Request failed", undefined, res.status);
    }
}
