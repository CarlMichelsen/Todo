import { writable, get } from 'svelte/store';
import type { CalendarDto, CreateCalendarDto } from '$lib/types/api/calendar';
import { CalendarClient } from '$lib/utils/calendarClient';
import { userStore } from './user';

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
		 * Set the active calendar
		 * @param calendarId - ID of the calendar to set as active
		 */
		setActiveCalendar(calendarId: string): void {
			update((state) => {
				// Verify calendar exists
				const calendar = state.calendars.find((c) => c.id === calendarId);
				if (!calendar) {
					console.warn(`Attempted to set non-existent calendar as active: ${calendarId}`);
					return state;
				}

				return {
					...state,
					activeCalendarId: calendarId
				};
			});
		},

		/**
		 * Create a new calendar
		 * @param calendar - CreateCalendarDto with title and color
		 */
		async createCalendar(calendar: CreateCalendarDto): Promise<CalendarDto> {
			const client = new CalendarClient();
			const createdCalendar = await client.createCalendar(calendar);

			update((state) => ({
				...state,
				calendars: [...state.calendars, createdCalendar],
				// Set as active if it's the first calendar
				activeCalendarId: state.calendars.length === 0 ? createdCalendar.id : state.activeCalendarId
			}));

			return createdCalendar;
		},

		/**
		 * Update an existing calendar
		 * @param calendarId - ID of the calendar to update
		 * @param updates - EditCalendarDto with partial updates
		 */
		async updateCalendar(
			calendarId: string,
			updates: { title?: string | null; color?: string | null }
		): Promise<CalendarDto> {
			const client = new CalendarClient();
			const updatedCalendar = await client.updateCalendar(calendarId, updates);

			update((state) => ({
				...state,
				calendars: state.calendars.map((c) => (c.id === calendarId ? updatedCalendar : c))
			}));

			return updatedCalendar;
		},

		/**
		 * Delete a calendar
		 * @param calendarId - ID of the calendar to delete
		 */
		async deleteCalendar(calendarId: string): Promise<void> {
			const client = new CalendarClient();
			await client.deleteCalendar(calendarId);

			update((state) => {
				const newCalendars = state.calendars.filter((c) => c.id !== calendarId);

				// If we deleted the active calendar, switch to first available calendar
				const newActiveCalendarId =
					state.activeCalendarId === calendarId
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
