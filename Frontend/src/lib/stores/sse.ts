import { writable } from 'svelte/store';
import { ServerSentEventClient } from '$lib/utils/serverSentEventClient';
import type { ServerEvent } from '$lib/types/api/sse';

export interface PendingCommand {
	commandId: string;
	commandType: string;
	timestamp: number;
	timeoutId: ReturnType<typeof setTimeout>;
	resolve: (event: ServerEvent) => void;
	reject: (error: Error) => void;
}

export interface SSEStoreState {
	connected: boolean;
	lastEventId: string | null;
	error: string | null;
	pendingCommands: Map<string, PendingCommand>;
}

function createSSEStore() {
	const { subscribe, set, update } = writable<SSEStoreState>({
		connected: false,
		lastEventId: null,
		error: null,
		pendingCommands: new Map()
	});

	let client: ServerSentEventClient | null = null;
	const eventListeners: Map<string, Set<(event: ServerEvent) => void>> = new Map();

	return {
		subscribe,

		/**
		 * Initialize SSE connection
		 * Should be called when user logs in
		 * @param connectionId - Optional connection ID for multi-tab support
		 */
		connect(connectionId?: string): void {
			if (client) {
				console.log('SSE Store: Already connected');
				return;
			}

			console.log('SSE Store: Initializing connection', { connectionId });
			client = new ServerSentEventClient(undefined, connectionId);

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
				// Check for pending command match and resolve it
				let pendingCommand: PendingCommand | undefined;
				update((state) => {
					pendingCommand = state.pendingCommands.get(event.eventId);

					if (pendingCommand) {
						// Clear timeout and remove from pending
						clearTimeout(pendingCommand.timeoutId);
						const newCommands = new Map(state.pendingCommands);
						newCommands.delete(event.eventId);

						return {
							...state,
							lastEventId: event.eventId,
							pendingCommands: newCommands
						};
					}

					// No pending command, just update last event ID
					return {
						...state,
						lastEventId: event.eventId
					};
				});

				// Resolve the pending promise if there was one
				if (pendingCommand) {
					pendingCommand.resolve(event);
				}

				// Forward to registered listeners
				const listeners = eventListeners.get(event.eventName);
				if (listeners) {
					listeners.forEach((callback) => {
						try {
							callback(event);
						} catch (error) {
							console.error(`SSE Store: Error in ${event.eventName} listener`, error);
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
				error: null,
				pendingCommands: new Map()
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
		},

		/**
		 * Wait for a specific SSE event by command ID
		 * Returns promise that resolves when matching event arrives
		 * Rejects on timeout
		 * @param commandId - Command ID to wait for
		 * @param eventType - Event type expected (for error messages)
		 * @param timeout - Timeout in milliseconds (default 20000)
		 * @returns Promise that resolves with the event
		 */
		waitForEvent(
			commandId: string,
			eventType: string,
			timeout: number = 20000
		): Promise<ServerEvent> {
			return new Promise((resolve, reject) => {
				const timeoutId = setTimeout(() => {
					// Remove from pending commands
					update((state) => {
						const newCommands = new Map(state.pendingCommands);
						newCommands.delete(commandId);
						return { ...state, pendingCommands: newCommands };
					});

					reject(
						new Error(
							`Timeout waiting for ${eventType} event. ` +
								`The operation may still complete in the background.`
						)
					);
				}, timeout);

				// Register pending command
				update((state) => {
					const newCommands = new Map(state.pendingCommands);
					newCommands.set(commandId, {
						commandId,
						commandType: eventType,
						timestamp: Date.now(),
						timeoutId,
						resolve,
						reject
					});
					return { ...state, pendingCommands: newCommands };
				});
			});
		},

		/**
		 * Cancel waiting for a command
		 * Used for cleanup
		 * @param commandId - Command ID to cancel
		 */
		cancelCommand(commandId: string): void {
			update((state) => {
				const pending = state.pendingCommands.get(commandId);
				if (pending) {
					clearTimeout(pending.timeoutId);
					const newCommands = new Map(state.pendingCommands);
					newCommands.delete(commandId);
					return { ...state, pendingCommands: newCommands };
				}
				return state;
			});
		}
	};
}

export const sseStore = createSSEStore();
