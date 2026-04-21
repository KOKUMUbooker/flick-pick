import { handleApiError } from '../utils/error-handler';
import { MutationCache, QueryCache, QueryClient } from '@tanstack/svelte-query'

export const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            refetchOnWindowFocus: false,
            refetchOnReconnect: false,
            refetchOnMount: false,
            staleTime: 5 * 60 * 1000,    // treat data as fresh for 5 min
        }
    },
    queryCache: new QueryCache({
        onError: (error) => {
            handleApiError(error);
        }
    }),
    mutationCache: new MutationCache({
        onError: (error) => {
            handleApiError(error);
        }
    }),
});