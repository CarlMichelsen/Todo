import { writable } from 'svelte/store';
import { ServerSentEventClient } from '$lib/utils/serverSentEventClient';
import type { ServerEvent } from '$lib/types/api/sse';

export interface SSEStoreState {
	connected: boolean;
	lastEventId: string | null;
	error: string | null;
}

function createSSEStore() {
	const { subscribe, set, update } = writable<SSEStoreState>({
		connected: false,
		lastEventId: null,
		error: null
	});

	let client: ServerSentEventClient | null = null;
	const eventListeners: Map<string, Set<(event: ServerEvent) => void>> = new Map();

	return {
		subscribe,

		/**
		 * Initialize SSE connection
		 * Should be called when user logs in
		 */
		connect(): void {
			if (client) {
				console.log('SSE Store: Already connected');
				return;
			}

			console.log('SSE Store: Initializing connection');
			client = new ServerSentEventClient();

			// Listen to connection state changes
			client.onConnectionStateChange((connected) => {
				update((state) => ({
					...state,
					connected,
					error: connected ? null : state.error
				}));
			});

			// Listen to errors
			client.onError((error) => {
				console.error('SSE Store: Connection error', error);
				update((state) => ({
					...state,
					error: 'SSE connection error'
				}));
			});

			// Setup event forwarding for registered listeners
			client.on('*', (event) => {
				// Update last event ID
				update((state) => ({
					...state,
					lastEventId: event.eventId
				}));

				// Forward to registered listeners
				const listeners = eventListeners.get(event.eventName);
				if (listeners) {
					listeners.forEach((callback) => {
						try {
							callback(event);
						} catch (error) {
							console.error(
								`SSE Store: Error in ${event.eventName} listener`,
								error
							);
						}
					});
				}

				// Forward to wildcard listeners
				const wildcardListeners = eventListeners.get('*');
				if (wildcardListeners) {
					wildcardListeners.forEach((callback) => {
						try {
							callback(event);
						} catch (error) {
							console.error('SSE Store: Error in wildcard listener', error);
						}
					});
				}
			});

			// Connect to SSE endpoint
			client.connect();
		},

		/**
		 * Disconnect SSE connection
		 * Should be called when user logs out
		 */
		disconnect(): void {
			if (client) {
				console.log('SSE Store: Disconnecting');
				client.disconnect();
				client = null;
			}

			// Clear all listeners
			eventListeners.clear();

			// Reset state
			set({
				connected: false,
				lastEventId: null,
				error: null
			});
		},

		/**
		 * Subscribe to a specific event type
		 * @param eventName - Event name to listen for (or '*' for all events)
		 * @param callback - Callback function to handle the event
		 * @returns Unsubscribe function
		 */
		on(eventName: string, callback: (event: ServerEvent) => void): () => void {
			if (!eventListeners.has(eventName)) {
				eventListeners.set(eventName, new Set());
			}

			const listeners = eventListeners.get(eventName)!;
			listeners.add(callback);

			// Return unsubscribe function
			return () => {
				listeners.delete(callback);
				if (listeners.size === 0) {
					eventListeners.delete(eventName);
				}
			};
		}
	};
}

export const sseStore = createSSEStore();
