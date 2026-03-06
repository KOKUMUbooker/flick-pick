// // src/routes/+layout.ts
// import { apiFetch, queryClient, type AuthResponseData } from '../../api';
// import { API_BASE_URL } from '../../api/urls';
// import { isLoggedIn, logIn } from '../../store';
// import type { LayoutLoad } from './$types';

// export const load: LayoutLoad = async () => {
//     if (isLoggedIn()) return

//     try {

//         const res = await apiFetch<AuthResponseData>(`${API_BASE_URL}/api/auth/me`, {
//             method: 'GET',
//             headers: { 'Content-Type': 'application/json' },
//         });
//         console.log("Fetching user details .... ", res)
//         logIn(res)
//     } catch (err) {
//         console.error('Failed to fetch user:', err);
//     }
// };

import { apiFetch, QUERY_KEYS, queryClient, type AuthResponseData } from '../../api';
import { API_BASE_URL } from '../../api/urls';
import { isLoggedIn, logIn } from '../../store';
import type { LayoutLoad } from './$types';

export const load: LayoutLoad = async () => {
    if (isLoggedIn()) return;

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

    } catch (err) {
        console.error('Failed to fetch user:', err);
    }
};