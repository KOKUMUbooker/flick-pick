import { apiFetch, QUERY_KEYS, queryClient, type AuthResponseData } from '../api';
import { API_BASE_URL } from '../api/urls';
import { isLoggedIn, logIn, startHubConnection } from '../store';
import type { LayoutLoad } from './$types';
import { getLoggedStat } from "../utils/logged-status"

// If user stat was previously 1 in lclStore, meaning user previously logged in,
// refetch the user "loggedIn" status from server
export const load: LayoutLoad = async () => {
    const lclStoreLoggedStat = getLoggedStat();
    if (lclStoreLoggedStat === "0" || isLoggedIn()) return;

    try {
        // fetchQuery returns the data directly
        const userData = await queryClient.fetchQuery({
            queryKey: [QUERY_KEYS.USER],
            queryFn: async () => {
                const res = await apiFetch<AuthResponseData>(
                    `${API_BASE_URL}/api/auth/me`,
                    {
                        method: 'GET',
                        headers: { 'Content-Type': 'application/json' },
                    });
                return res;
            },
            staleTime: 5 * 60 * 1000,
            gcTime: 10 * 60 * 1000,
        });

        logIn(userData);

        //  Connect to signalR hub
        await startHubConnection();

    } catch (err) {
        console.error('Failed to fetch user:', err);
    }
};
export const ssr = false;
