import { writable } from 'svelte/store';
import type { JwtUser } from '$lib/types/user';
import { UserClient } from '$lib/utils/userClient';

/**
 * User store state type
 * - 'pending': Initial state, auth check not yet started or completed
 * - 'loading': Actively loading user data
 * - 'authenticated': User is logged in
 * - 'unauthenticated': User is not logged in (normal state)
 * - 'error': Failed to check authentication (network error, etc.)
 */
type UserStoreState = 'pending' | 'loading' | 'authenticated' | 'unauthenticated' | 'error';

/**
 * User store state
 */
interface UserState {
    user: JwtUser | null;
    state: UserStoreState;
    error: string | null;
}

/**
 * Create the user store with initial state
 */
function createUserStore() {
    const { subscribe, set, update } = writable<UserState>({
        user: null,
        state: 'pending',
        error: null
    });

    const userClient = new UserClient();

    return {
        subscribe,

        /**
         * Initialize the user store by checking authentication status
         * This should be called when the application starts
         */
        async initialize(): Promise<void> {
            update(state => ({ ...state, state: 'loading', error: null }));

            try {
                const user = await userClient.getCurrentUser();

                if (user) {
                    set({ user, state: 'authenticated', error: null });
                } else {
                    // 401 - not authenticated, which is normal
                    set({ user: null, state: 'unauthenticated', error: null });
                }
            } catch (error) {
                set({ user: null, state: 'unauthenticated', error: null });
            }
        },

        /**
         * Set the user data (e.g., after successful login)
         */
        setUser(user: JwtUser): void {
            set({ user, state: 'authenticated', error: null });
        },

        /**
         * Clear the user data (logout)
         */
        async logoutUser(): Promise<void> {
            try {
                await userClient.logout();
                set({ user: null, state: 'unauthenticated', error: null });
            }
            catch(error) {
                const errorMessage = error instanceof Error ? error.message : 'Failed to fetch user';
                console.error('User authentication check failed:', errorMessage);
                set({ user: null, state: 'error', error: errorMessage });
            }
        },

        /**
         * Refresh user data from the API
         */
        async refresh(): Promise<void> {
            update(state => ({ ...state, state: 'loading' }));

            try {
                const user = await userClient.getCurrentUser();

                if (user) {
                    update(state => ({ ...state, user, state: 'authenticated', error: null }));
                } else {
                    update(state => ({ ...state, user: null, state: 'unauthenticated', error: null }));
                }
            } catch (error) {
                set({ user: null, state: 'unauthenticated', error: null });
            }
        }
    };
}

/**
 * Global user store instance
 * Manages the current authenticated user state
 */
export const userStore = createUserStore();
