import { SvelteDate } from "svelte/reactivity";
import type { AuthResponseData } from "../api";
import type { IAuthState } from "../types";

export const authState = $state<IAuthState>({
    user: undefined,
    accessToken: undefined,
    accessTokenExpiresAt: undefined,
    refreshToken: undefined
});

export function logIn(authData: AuthResponseData) {
    const { userDetails, accessToken, accessTokenExpiresAt, refreshToken } = authData;
    const tknExpiration = new SvelteDate(accessTokenExpiresAt).getTime()

    authState.user = userDetails;
    authState.accessToken = accessToken;
    authState.refreshToken = refreshToken;
    authState.accessTokenExpiresAt = tknExpiration;
}

export async function logOut() {
    authState.user = undefined
    authState.refreshToken = undefined
    authState.accessToken = undefined
    authState.accessTokenExpiresAt = undefined
}

export function isLoggedIn() {
    return authState.user != undefined
}
