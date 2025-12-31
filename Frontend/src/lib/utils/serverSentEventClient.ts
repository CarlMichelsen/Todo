import type { ServerEvent } from '$lib/types/api/sse';

/**
 * Client for managing Server-Sent Events (SSE) connection
 * Handles connection lifecycle, event parsing, and automatic reconnection
 */
export class ServerSentEventClient {
	private eventSource: EventSource | null = null;
	private connectionId: string | null = null;
	private reconnectAttempts = 0;
	private maxReconnectAttempts = 5;
	private reconnectDelay = 1000; // Start with 1 second
	private maxReconnectDelay = 30000; // Max 30 seconds
	private listeners: Map<string, Set<(event: ServerEvent) => void>> = new Map();
	private errorListeners: Set<(error: Event) => void> = new Set();
	private connectionStateListeners: Set<(connected: boolean) => void> = new Set();
	private isConnected = false;

	constructor(
		private baseUrl: string = import.meta.env.VITE_API_URL || '',
		connectionId?: string
	) {
		this.connectionId = connectionId || null;
	}

	/**
	 * Connect to the SSE endpoint
	 * EventSource automatically sends cookies (credentials: 'include' equivalent)
	 * @param lastEventId - Optional Last-Event-ID for resuming from specific event
	 */
	connect(lastEventId?: string): void {
		if (this.eventSource) {
			console.warn('SSE: Already connected, disconnecting first');
			this.disconnect();
		}

		// Build URL with connectionId query parameter
		let url = `${this.baseUrl}/api/v1/ServerSentEvent`;

		if (this.connectionId) {
			url += `?connectionId=${encodeURIComponent(this.connectionId)}`;
			console.log('SSE: Connecting with connection ID:', this.connectionId);
		}

		console.log('SSE: Connecting to', url);

		try {
			// EventSource automatically includes cookies for same-origin requests
			this.eventSource = new EventSource(url, {
				withCredentials: true // Include cookies for CORS requests
			});

			// Set Last-Event-ID header if provided (for reconnection)
			if (lastEventId) {
				console.log('SSE: Resuming from event ID:', lastEventId);
			}

			this.setupEventListeners();
		} catch (error) {
			console.error('SSE: Failed to create EventSource', error);
			this.notifyConnectionState(false);
			this.scheduleReconnect();
		}
	}

	/**
	 * Disconnect from the SSE endpoint
	 */
	disconnect(): void {
		if (this.eventSource) {
			console.log('SSE: Disconnecting');
			this.eventSource.close();
			this.eventSource = null;
			this.isConnected = false;
			this.reconnectAttempts = 0;
			this.notifyConnectionState(false);
		}
	}

	/**
	 * Setup EventSource event listeners
	 */
	private setupEventListeners(): void {
		if (!this.eventSource) return;

		// Connection opened
		this.eventSource.onopen = () => {
			console.log('SSE: Connection established');
			this.isConnected = true;
			this.reconnectAttempts = 0;
			this.reconnectDelay = 1000; // Reset delay
			this.notifyConnectionState(true);
		};

		// Generic message handler (fallback for unnamed events)
		this.eventSource.onmessage = (event: MessageEvent) => {
			console.log('SSE: Received unnamed event', event.data);
			this.handleEvent(event);
		};

		// Error handler
		this.eventSource.onerror = (error: Event) => {
			console.error('SSE: Connection error', error);
			this.isConnected = false;
			this.notifyConnectionState(false);
			this.notifyError(error);

			// EventSource automatically attempts to reconnect
			// We'll add our own retry logic for more control
			if (this.eventSource?.readyState === EventSource.CLOSED) {
				console.log('SSE: Connection closed, scheduling reconnect');
				this.scheduleReconnect();
			}
		};

		// Listen for specific event types (calendar events)
		this.addEventListener('CreateCalendar');
		this.addEventListener('EditCalendar');
		this.addEventListener('DeleteCalendar');
		this.addEventListener('SelectCalendar');
	}

	/**
	 * Add listener for a specific event type
	 */
	private addEventListener(eventName: string): void {
		if (!this.eventSource) return;

		this.eventSource.addEventListener(eventName, (event: MessageEvent) => {
			console.log(`SSE: Received ${eventName} event`, event.data);
			this.handleEvent(event);
		});
	}

	/**
	 * Handle incoming SSE event
	 */
	private handleEvent(event: MessageEvent): void {
		try {
			const data = JSON.parse(event.data) as ServerEvent;

			// Validate event structure
			if (!data.eventName || !data.eventId) {
				console.warn('SSE: Invalid event structure', data);
				return;
			}

			// Notify listeners for this event type
			const listeners = this.listeners.get(data.eventName);
			if (listeners) {
				listeners.forEach((callback) => {
					try {
						callback(data);
					} catch (error) {
						console.error(`SSE: Error in ${data.eventName} listener`, error);
					}
				});
			}

			// Notify wildcard listeners (listening to all events)
			const wildcardListeners = this.listeners.get('*');
			if (wildcardListeners) {
				wildcardListeners.forEach((callback) => {
					try {
						callback(data);
					} catch (error) {
						console.error('SSE: Error in wildcard listener', error);
					}
				});
			}
		} catch (error) {
			console.error('SSE: Failed to parse event data', error);
		}
	}

	/**
	 * Schedule reconnection with exponential backoff
	 */
	private scheduleReconnect(): void {
		if (this.reconnectAttempts >= this.maxReconnectAttempts) {
			console.error('SSE: Max reconnection attempts reached');
			return;
		}

		this.reconnectAttempts++;
		const delay = Math.min(
			this.reconnectDelay * Math.pow(2, this.reconnectAttempts - 1),
			this.maxReconnectDelay
		);

		console.log(
			`SSE: Reconnecting in ${delay}ms (attempt ${this.reconnectAttempts}/${this.maxReconnectAttempts})`
		);

		setTimeout(() => {
			this.connect();
		}, delay);
	}

	/**
	 * Subscribe to a specific event type
	 * @param eventName - Event name to listen for (or '*' for all events)
	 * @param callback - Callback function to handle the event
	 * @returns Unsubscribe function
	 */
	on(eventName: string, callback: (event: ServerEvent) => void): () => void {
		if (!this.listeners.has(eventName)) {
			this.listeners.set(eventName, new Set());
		}

		const listeners = this.listeners.get(eventName)!;
		listeners.add(callback);

		// Return unsubscribe function
		return () => {
			listeners.delete(callback);
			if (listeners.size === 0) {
				this.listeners.delete(eventName);
			}
		};
	}

	/**
	 * Subscribe to connection errors
	 * @param callback - Callback function to handle errors
	 * @returns Unsubscribe function
	 */
	onError(callback: (error: Event) => void): () => void {
		this.errorListeners.add(callback);
		return () => {
			this.errorListeners.delete(callback);
		};
	}

	/**
	 * Subscribe to connection state changes
	 * @param callback - Callback function to handle state changes
	 * @returns Unsubscribe function
	 */
	onConnectionStateChange(callback: (connected: boolean) => void): () => void {
		this.connectionStateListeners.add(callback);
		// Immediately notify current state
		callback(this.isConnected);
		return () => {
			this.connectionStateListeners.delete(callback);
		};
	}

	/**
	 * Notify all error listeners
	 */
	private notifyError(error: Event): void {
		this.errorListeners.forEach((callback) => {
			try {
				callback(error);
			} catch (err) {
				console.error('SSE: Error in error listener', err);
			}
		});
	}

	/**
	 * Notify all connection state listeners
	 */
	private notifyConnectionState(connected: boolean): void {
		this.connectionStateListeners.forEach((callback) => {
			try {
				callback(connected);
			} catch (error) {
				console.error('SSE: Error in connection state listener', error);
			}
		});
	}

	/**
	 * Get current connection state
	 */
	get connected(): boolean {
		return this.isConnected;
	}

	/**
	 * Get EventSource ready state
	 */
	get readyState(): number {
		return this.eventSource?.readyState ?? EventSource.CLOSED;
	}
}
