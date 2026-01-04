import { writable, get } from 'svelte/store';
import type { CalendarDto } from '$lib/types/api/calendar';
import { CalendarClient } from '$lib/utils/calendarClient';
import { userStore } from './user';
import { sseStore } from './sse';
import { toastStore } from './toast';
import type {
	CreateCalendarEvent,
	EditCalendarEvent,
	DeleteCalendarEvent,
	SelectCalendarEvent,
	CreateCalendarLinkEvent,
	EditCalendarLinkEvent,
	DeleteCalendarLinkEvent
} from '$lib/types/api/sse';
import { distinctBy } from '$lib/utils/distinctBy';

export interface CalendarStoreState {
	calendars: CalendarDto[];
	activeCalendarId: string | null;
	editingCalendarId: string | null;
	loading: boolean;
	error: string | null;
}

function createCalendarsStore() {
	const { subscribe, set, update } = writable<CalendarStoreState>({
		calendars: [],
		activeCalendarId: null,
		editingCalendarId: null,
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
									c.title === createEvent.calendar.title && c.color === createEvent.calendar.color
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

		// Handle CreateCalendarLink events
		sseStore.on('CreateCalendarLink', (event) => {
			const createEvent = event as CreateCalendarLinkEvent;
			console.log('SSE: CreateCalendarLink event received', createEvent);

			update((state) => {
				const newCalendars = state.calendars.map((c) => {
					const calendarWithCreatedLink = createEvent.calendarLink.parentCalendars.find(
						(pc) => pc == c.id
					);
					if (!calendarWithCreatedLink) {
						return c;
					}

					c.calendarLinks = distinctBy(
						[createEvent.calendarLink, ...c.calendarLinks],
						(cl) => cl.id
					);
					return c;
				});

				return {
					...state,
					calendars: [...newCalendars]
				};
			});

			// Show success toast
			toastStore.success(`Calendar link "${createEvent.calendarLink.title}" created`, 3000);
		});

		// Handle EditCalendarLink events
		sseStore.on('EditCalendarLink', (event) => {
			const editEvent = event as EditCalendarLinkEvent;
			console.log('SSE: EditCalendarLink event received', editEvent);

			const linkId = editEvent.calendarLink.id;
			const deleteAssociations = editEvent.deleteParentCalendarAssociation;
			const addAssociations = editEvent.addParentCalendarAssociation;

			update((state) => {
				const newCalendars = state.calendars.map((c) => {
					// Find if this calendar has the link being edited
					const existingLinkIndex = c.calendarLinks.findIndex((cl) => cl.id === linkId);

					if (existingLinkIndex === -1) {
						// Check if we need to add this link to this calendar
						if (addAssociations?.includes(c.id)) {
							c.calendarLinks = [...c.calendarLinks, editEvent.calendarLink];
						}
						return c;
					}

					// Check if this link should be removed from this calendar
					if (deleteAssociations?.includes(c.id)) {
						c.calendarLinks = c.calendarLinks.filter((cl) => cl.id !== linkId);
						return c;
					}

					// Replace with the updated link from the event
					c.calendarLinks = [...c.calendarLinks];
					c.calendarLinks[existingLinkIndex] = editEvent.calendarLink;

					return c;
				});

				return {
					...state,
					calendars: [...newCalendars]
				};
			});

			// Show success toast
			toastStore.success(`Calendar link updated`, 3000);
		});

		// Handle DeleteCalendarLink events
		sseStore.on('DeleteCalendarLink', (event) => {
			const deleteEvent = event as DeleteCalendarLinkEvent;
			console.log('SSE: DeleteCalendarLink event received', deleteEvent);

			update((state) => {
				const newCalendars = state.calendars.map((c) => {
					c.calendarLinks = c.calendarLinks.filter((cl) => cl.id !== deleteEvent.calendarLinkId);
					return c;
				});

				return {
					...state,
					calendars: [...newCalendars]
				};
			});

			// Show success toast
			toastStore.success(`Calendar link "${deleteEvent.title}" deleted`, 3000);
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

		setEditingCalendar(calendarId: string | null): void {
			update((state) => {
				return {
					...state,
					editingCalendarId: calendarId
				};
			});
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
		},

		/**
		 * Clear all calendars (for logout, etc.)
		 */
		clear(): void {
			set({
				calendars: [],
				activeCalendarId: null,
				editingCalendarId: null,
				loading: false,
				error: null
			});
		}
	};
}

export const calendarsStore = createCalendarsStore();
