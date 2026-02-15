import type { IAuthState, IAuthData } from "../types";

export const authState = $state<IAuthState>({
    user: undefined
});

export function logIn({ user }: IAuthData) {
    authState.user = user
}

export async function logOut() {
    authState.user = undefined
}

export function isLoggedIn() {
    return authState.user != undefined
}