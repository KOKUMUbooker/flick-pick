import type { AuthResponseData } from "../api";
import type { AppState } from "../types";

export const authState = $state<AppState>({
    user: undefined,
    hubConnection: undefined
});

export function logIn(authData: AuthResponseData) {
    const { userDetails } = authData;
    authState.user = userDetails;
}

export async function logOut() {
    authState.user = undefined
    authState.hubConnection = undefined
}

export function isLoggedIn() {
    return authState.user != undefined
}
