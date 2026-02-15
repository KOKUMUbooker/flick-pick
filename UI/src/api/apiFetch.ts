import { logOut } from "../store";
import { ApiError } from "../types/error";

let isRefreshing = false;
let refreshPromise: Promise<void> | null = null;

export async function apiFetch<T>(
    url: string,
    options?: RequestInit,
    retry = true
): Promise<T> {

    const res = await fetch(url, {
        credentials: "include", // REQUIRED for HttpOnly cookies
        headers: {
            "Content-Type": "application/json",
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

async function refreshToken(): Promise<void> {
    const res = await fetch("/auth/refresh", {
        method: "POST",
        credentials: "include"
    });

    if (!res.ok) {
        // Refresh failed → session dead
        logOut();
        window.location.href = "/login";
        throw new Error("Session expired");
    }
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

    await refreshPromise;

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
