import { handleApiError } from '../utils/error-handler';
import { MutationCache, QueryCache, QueryClient } from '@tanstack/svelte-query'

export const queryClient = new QueryClient({
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