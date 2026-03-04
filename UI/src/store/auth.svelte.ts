import type { AuthResponseData } from "../api";
import type { AppState } from "../types";

export const appState = $state<AppState>({
    user: undefined,
    hubConnection: undefined
});

export function logIn(authData: AuthResponseData) {
    const { userDetails } = authData;
    appState.user = userDetails;
}

export async function logOut() {
    appState.user = undefined
    appState.hubConnection = undefined
}

export function isLoggedIn() {
    return appState.user != undefined
}
