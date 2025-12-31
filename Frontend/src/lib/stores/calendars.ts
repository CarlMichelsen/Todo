import { writable, get } from 'svelte/store';
import type { CalendarDto, CreateCalendarDto } from '$lib/types/api/calendar';
import { CalendarClient } from '$lib/utils/calendarClient';
import { userStore } from './user';
import { sseStore } from './sse';
import { toastStore } from './toast';
import type {
	CreateCalendarEvent,
	EditCalendarEvent,
	DeleteCalendarEvent,
	SelectCalendarEvent
} from '$lib/types/api/sse';

export interface CalendarStoreState {
	calendars: CalendarDto[];
	activeCalendarId: string | null;
	loading: boolean;
	error: string | null;
}

function createCalendarsStore() {
	const { subscribe, set, update } = writable<CalendarStoreState>({
		calendars: [],
		activeCalendarId: null,
		loading: false,
		error: null
	});

	// Setup SSE event handlers
	setupSSEHandlers();

	/**
	 * Setup Server-Sent Events handlers for calendar events
	 */
	function setupSSEHandlers() {
		// Handle CreateCalendar events
		sseStore.on('CreateCalendar', (event) => {
			const createEvent = event as CreateCalendarEvent;
			console.log('SSE: CreateCalendar event received', createEvent);

			update((state) => {
				// Check if calendar already exists (avoid duplicates)
				const exists = state.calendars.some((c) => c.id === createEvent.calendar.id);
				if (exists) {
					// Replace temporary ID calendar with real calendar
					return {
						...state,
						calendars: state.calendars.map((c) =>
							c.title === createEvent.calendar.title && c.color === createEvent.calendar.color
								? createEvent.calendar
								: c
						),
						// Update active calendar ID if it was the temp one
						activeCalendarId:
							state.calendars.find(
								(c) =>
									c.title === createEvent.calendar.title &&
									c.color === createEvent.calendar.color
							)?.id === state.activeCalendarId
								? createEvent.calendar.id
								: state.activeCalendarId
					};
				}

				// Add new calendar
				return {
					...state,
					calendars: [...state.calendars, createEvent.calendar]
				};
			});

			// Show success toast
			toastStore.success(`Calendar "${createEvent.calendar.title}" created`, 3000);
		});

		// Handle EditCalendar events
		sseStore.on('EditCalendar', (event) => {
			const editEvent = event as EditCalendarEvent;
			console.log('SSE: EditCalendar event received', editEvent);

			update((state) => ({
				...state,
				calendars: state.calendars.map((c) =>
					c.id === editEvent.calendar.id ? editEvent.calendar : c
				)
			}));

			// Show success toast
			toastStore.success(`Calendar "${editEvent.calendar.title}" updated`, 3000);
		});

		// Handle DeleteCalendar events
		sseStore.on('DeleteCalendar', (event) => {
			const deleteEvent = event as DeleteCalendarEvent;
			console.log('SSE: DeleteCalendar event received', deleteEvent);

			update((state) => {
				const newCalendars = state.calendars.filter((c) => c.id !== deleteEvent.calendarId);

				// If we deleted the active calendar, switch to first available
				const newActiveCalendarId =
					state.activeCalendarId === deleteEvent.calendarId
						? newCalendars.length > 0
							? newCalendars[0].id
							: null
						: state.activeCalendarId;

				return {
					...state,
					calendars: newCalendars,
					activeCalendarId: newActiveCalendarId
				};
			});

			// Show success toast
			toastStore.success(`Calendar "${deleteEvent.calendarTitle}" deleted`, 3000);
		});

		// Handle SelectCalendar events
		sseStore.on('SelectCalendar', (event) => {
			const selectEvent = event as SelectCalendarEvent;
			console.log('SSE: SelectCalendar event received', selectEvent);

			update((state) => ({
				...state,
				activeCalendarId: selectEvent.calendarId
			}));
		});
	}

	return {
		subscribe,

		/**
		 * Initialize calendars store from user's calendars
		 * - Fetches user's calendars
		 * - Sets active calendar from user's selectedCalendarId
		 * - Falls back to first calendar if selectedCalendarId not found
		 */
		async initialize(): Promise<void> {
			update((state) => ({
				...state,
				loading: true,
				error: null
			}));

			try {
				const client = new CalendarClient();
				const calendars = await client.getCalendars();

				// Get user's selected calendar ID from user store
				const user = get(userStore).user;
				const selectedCalendarId = user?.selectedCalendarId;

				// Try to use user's selected calendar, fall back to first calendar
				let activeCalendarId: string | null = null;
				if (selectedCalendarId && calendars.some((c) => c.id === selectedCalendarId)) {
					activeCalendarId = selectedCalendarId;
				} else if (calendars.length > 0) {
					activeCalendarId = calendars[0].id;
				}

				update((state) => ({
					...state,
					calendars,
					activeCalendarId,
					loading: false,
					error: null
				}));
			} catch (error) {
				console.error('Failed to initialize calendars:', error);
				update((state) => ({
					...state,
					loading: false,
					error: error instanceof Error ? error.message : 'Failed to initialize calendars'
				}));
			}
		},

		/**
		 * Set the active calendar and persist to server
		 * @param calendarId - ID of the calendar to set as active
		 */
		async setActiveCalendar(calendarId: string): Promise<void> {
			const state = get({ subscribe });

			// Verify calendar exists
			const calendar = state.calendars.find((c) => c.id === calendarId);
			if (!calendar) {
				console.warn(`Attempted to set non-existent calendar as active: ${calendarId}`);
				return;
			}

			// Optimistic update - update local state immediately
			update((s) => ({
				...s,
				activeCalendarId: calendarId
			}));

			// Persist to server
			try {
				const client = new CalendarClient();
				await client.selectCalendar(calendarId);
				// Server call succeeded - local state already updated
			} catch (error) {
				console.error('Failed to persist calendar selection to server:', error);
				// Keep local state updated even if server call fails
				// This allows offline usage and graceful degradation
			}
		},

		/**
		 * Create a new calendar
		 * CQRS: Fire-and-forget pattern - sends command and returns immediately
		 * State will be updated via SSE event handler when server confirms
		 * @param calendar - CreateCalendarDto with title and color
		 */
		async createCalendar(calendar: CreateCalendarDto): Promise<void> {
			const client = new CalendarClient();

			// Send command (fire and forget)
			await client.createCalendar(calendar);

			// Return immediately - SSE handler will update state when event arrives
		},

		/**
		 * Update an existing calendar
		 * CQRS: Fire-and-forget pattern - sends command and returns immediately
		 * State will be updated via SSE event handler when server confirms
		 * @param calendarId - ID of the calendar to update
		 * @param updates - EditCalendarDto with partial updates
		 */
		async updateCalendar(
			calendarId: string,
			updates: { title?: string | null; color?: string | null }
		): Promise<void> {
			const client = new CalendarClient();

			// Send command (fire and forget)
			await client.updateCalendar(calendarId, updates);

			// Return immediately - SSE handler will update state when event arrives
		},

		/**
		 * Delete a calendar
		 * CQRS: Fire-and-forget pattern - sends command and returns immediately
		 * State will be updated via SSE event handler when server confirms
		 * @param calendarId - ID of the calendar to delete
		 */
		async deleteCalendar(calendarId: string): Promise<void> {
			const client = new CalendarClient();

			// Send command (fire and forget)
			await client.deleteCalendar(calendarId);

			// Return immediately - SSE handler will update state when event arrives
		},

		/**
		 * Clear all calendars (for logout, etc.)
		 */
		clear(): void {
			set({
				calendars: [],
				activeCalendarId: null,
				loading: false,
				error: null
			});
		}
	};
}

export const calendarsStore = createCalendarsStore();
