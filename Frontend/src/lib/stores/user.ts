import { writable } from 'svelte/store';
import type { PersonalUserDto } from '$lib/types/user';
import { UserClient } from '$lib/utils/userClient';
import { sseStore } from './sse';

/**
 * User store state type
 * - 'pending': Initial state, auth check not yet started or completed
 * - 'authenticated': User is logged in
 * - 'unauthenticated': User is not logged in (normal state)
 * - 'error': Failed to check authentication (network error, etc.)
 */
type UserStoreState = 'pending' | 'authenticated' | 'unauthenticated' | 'error';

/**
 * User store state
 */
interface UserState {
	user: PersonalUserDto | null;
	state: UserStoreState;
	error: string | null;
	connectionId: string | null;
}

/**
 * Create the user store with initial state
 */
function createUserStore() {
	const { subscribe, set, update } = writable<UserState>({
		user: null,
		state: 'pending',
		error: null,
		connectionId: null
	});

	const userClient = new UserClient();

	return {
		subscribe,

		/**
		 * Initialize the user store by checking authentication status
		 * This should be called when the application starts
		 * @param connectionId - Optional connection ID for SSE multi-tab support
		 */
		async initialize(connectionId?: string): Promise<void> {
			update((state) => ({
				...state,
				state: 'pending',
				error: null,
				connectionId: connectionId || null
			}));

			try {
				const user = await userClient.getCurrentUser();

				if (user) {
					set({ user, state: 'authenticated', error: null, connectionId: connectionId || null });
					// Connect to SSE when authenticated with connectionId
					sseStore.connect(connectionId);
				} else {
					// 401 - not authenticated, which is normal
					set({
						user: null,
						state: 'unauthenticated',
						error: null,
						connectionId: connectionId || null
					});
				}
			} catch {
				set({
					user: null,
					state: 'unauthenticated',
					error: null,
					connectionId: connectionId || null
				});
			}
		},

		/**
		 * Set the user data (e.g., after successful login)
		 * @param connectionId - Optional connection ID for SSE multi-tab support
		 */
		setUser(user: PersonalUserDto, connectionId?: string): void {
			set({ user, state: 'authenticated', error: null, connectionId: connectionId || null });
			// Connect to SSE when user logs in with connectionId
			sseStore.connect(connectionId);
		},

		/**
		 * Clear the user data (logout)
		 */
		async logoutUser(): Promise<void> {
			try {
				await userClient.logout();
				set({ user: null, state: 'unauthenticated', error: null, connectionId: null });
				// Disconnect SSE when user logs out
				sseStore.disconnect();
			} catch (error) {
				const errorMessage = error instanceof Error ? error.message : 'Failed to fetch user';
				console.error('User authentication check failed:', errorMessage);
				set({ user: null, state: 'error', error: errorMessage, connectionId: null });
				// Disconnect SSE even on error
				sseStore.disconnect();
			}
		},

		/**
		 * Refresh user data from the API
		 */
		async refresh(): Promise<void> {
			try {
				const user = await userClient.getCurrentUser();

				if (user) {
					update((state) => ({ ...state, user, state: 'authenticated', error: null }));
				} else {
					update((state) => ({ ...state, user: null, state: 'unauthenticated', error: null }));
				}
			} catch {
				update((state) => ({ ...state, user: null, state: 'unauthenticated', error: null }));
			}
		}
	};
}

/**
 * Global user store instance
 * Manages the current authenticated user state
 */
export const userStore = createUserStore();
