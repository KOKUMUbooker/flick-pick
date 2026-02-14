 import { toast } from "svelte-sonner";
import type { ApiError } from "../types/error";

export function handleApiError(error: unknown) {
    if (!(error instanceof Error)) {
        toast.error("Something went wrong");
        return;
    }

    // If it's our structured ApiError
    const apiError = error as ApiError;

    switch (apiError.code) {
        case "TOKEN_EXPIRED":
            toast.error("Your verification link has expired.", { richColors: true });
            break;

        case "UNAUTHORIZED":
            toast.error("You must be logged in.",{ richColors: true });
            break;

        case "VALIDATION_ERROR":
            toast.error(apiError.message,{ richColors: true });
            break;

        default:
            toast.error(apiError.message || "Unexpected error occurred",{ richColors: true });
    }
}
