import { writable, get } from 'svelte/store';
import type { CalendarEvent } from '$lib/types/calendar';
import { getWeekStart } from '$lib/utils/calendarUtils';
import { EventClient } from '$lib/utils/eventClient';
import {
	eventDtoToCalendarEvent,
	calendarEventToCreateDto,
	calendarEventToEditDto
} from '$lib/utils/eventConverter';
import { calendarsStore } from './calendars';
import { sseStore } from './sse';
import { toastStore } from './toast';
import type { CreateEventEvent, EditEventEvent, DeleteEventEvent } from '$lib/types/api/sse';

export interface EventStoreState {
	events: CalendarEvent[];
	dateRange: {
		start: Date;
		end: Date;
	} | null;
	loading: boolean;
	error: string | null;
}

function createEventsStore() {
	// Initialize with empty events - real data loads when calendar is selected
	const today = new Date();
	const currentWeekStart = getWeekStart(today);
	const currentWeekEnd = new Date(currentWeekStart);
	currentWeekEnd.setDate(currentWeekStart.getDate() + 6);
	currentWeekEnd.setHours(23, 59, 59, 999);

	const { subscribe, set, update } = writable<EventStoreState>({
		events: [],
		dateRange: {
			start: currentWeekStart,
			end: currentWeekEnd
		},
		loading: false,
		error: null
	});

	// Setup SSE event handlers
	setupSSEHandlers();

	/**
	 * Setup Server-Sent Events handlers for event operations
	 */
	function setupSSEHandlers() {
		// Handle CreateEvent events
		sseStore.on('CreateEvent', (event) => {
			const createEvent = event as CreateEventEvent;
			console.log('SSE: CreateEvent event received', createEvent);

			// Only process if event belongs to active calendar
			const activeCalendarId = get(calendarsStore).activeCalendarId;
			if (createEvent.calendarId !== activeCalendarId) {
				return;
			}

			const newEvent = eventDtoToCalendarEvent(createEvent.event);

			update((state) => {
				// Check if event already exists (avoid duplicates)
				const exists = state.events.some((e) => e.id === newEvent.id);
				if (exists) {
					return state;
				}

				// Add new event if it's within current date range
				if (isEventInDateRange(newEvent, state.dateRange)) {
					return {
						...state,
						events: [...state.events, newEvent]
					};
				}

				return state;
			});

			// Show success toast
			toastStore.success(`Event "${createEvent.event.title}" created`, 3000);
		});

		// Handle EditEvent events
		sseStore.on('EditEvent', (event) => {
			const editEvent = event as EditEventEvent;
			console.log('SSE: EditEvent event received', editEvent);

			// Only process if event belongs to active calendar
			const activeCalendarId = get(calendarsStore).activeCalendarId;
			if (editEvent.calendarId !== activeCalendarId) {
				return;
			}

			const updatedEvent = eventDtoToCalendarEvent(editEvent.event);

			update((state) => ({
				...state,
				events: state.events.map((e) => (e.id === updatedEvent.id ? updatedEvent : e))
			}));

			// Show success toast
			toastStore.success(`Event "${editEvent.event.title}" updated`, 3000);
		});

		// Handle DeleteEvent events
		sseStore.on('DeleteEvent', (event) => {
			const deleteEvent = event as DeleteEventEvent;
			console.log('SSE: DeleteEvent event received', deleteEvent);

			// Only process if event belongs to active calendar
			const activeCalendarId = get(calendarsStore).activeCalendarId;
			if (deleteEvent.calendarId !== activeCalendarId) {
				return;
			}

			update((state) => ({
				...state,
				events: state.events.filter((e) => e.id !== deleteEvent.calendarEventId)
			}));

			// Show success toast
			toastStore.success(`Event "${deleteEvent.eventTitle}" deleted`, 3000);
		});
	}

	/**
	 * Check if an event falls within the current date range
	 */
	function isEventInDateRange(
		event: CalendarEvent,
		dateRange: { start: Date; end: Date } | null
	): boolean {
		if (!dateRange) return true;

		const eventStart = new Date(event.start);
		const eventEnd = new Date(event.end);
		const rangeStart = new Date(dateRange.start);
		const rangeEnd = new Date(dateRange.end);

		return eventStart <= rangeEnd && eventEnd >= rangeStart;
	}

	return {
		subscribe,

		/**
		 * Set visible date range and load events for that range
		 * @param weekStart - Monday of week to load
		 */
		async setDateRange(weekStart: Date): Promise<void> {
			const weekEnd = new Date(weekStart);
			weekEnd.setDate(weekStart.getDate() + 6);
			weekEnd.setHours(23, 59, 59, 999);

			// Get active calendar ID from calendars store
			const calendarId = get(calendarsStore).activeCalendarId;

			if (!calendarId) {
				// No calendar available - set empty state with error
				update((state) => ({
					...state,
					events: [],
					loading: false,
					error: 'No calendar selected',
					dateRange: {
						start: weekStart,
						end: weekEnd
					}
				}));
				return;
			}

			update((state) => ({
				...state,
				events: [],
				loading: true,
				error: null,
				dateRange: {
					start: weekStart,
					end: weekEnd
				}
			}));

			try {
				const client = new EventClient();
				const eventDtos = await client.getEventsForDateRange(calendarId, weekStart, weekEnd);
				const events = eventDtos.map(eventDtoToCalendarEvent);

				update((state) => ({
					...state,
					events,
					loading: false,
					error: null
				}));
			} catch (error) {
				update((state) => ({
					...state,
					loading: false,
					error: error instanceof Error ? error.message : 'Failed to load events'
				}));
			}
		},

		/**
		 * Create a new event (CQRS command)
		 * Returns command ID for SSE correlation
		 */
		async createEvent(event: CalendarEvent): Promise<string> {
			const calendarId = get(calendarsStore).activeCalendarId;

			if (!calendarId) {
				throw new Error('No calendar selected');
			}

			// Add optimistic update immediately if event is in date range
			const currentState = get({ subscribe });
			if (isEventInDateRange(event, currentState.dateRange)) {
				update((state) => ({
					...state,
					events: [...state.events, event]
				}));
			}

			try {
				const client = new EventClient();
				const createDto = calendarEventToCreateDto(event);
				const commandId = await client.createEvent(calendarId, createDto);

				// Store command ID for SSE correlation (optional - events will update via SSE anyway)
				console.log('Event create command sent:', commandId);
				return commandId;
			} catch (error) {
				// Revert optimistic update on error
				update((state) => ({
					...state,
					events: state.events.filter((e) => e.id !== event.id)
				}));

				const errorMessage = error instanceof Error ? error.message : 'Failed to create event';
				toastStore.error(errorMessage, 5000);
				throw error;
			}
		},

		/**
		 * Update an existing event (CQRS command)
		 * Returns command ID for SSE correlation
		 */
		async updateEvent(event: CalendarEvent): Promise<string> {
			const calendarId = get(calendarsStore).activeCalendarId;

			if (!calendarId) {
				throw new Error('No calendar selected');
			}

			// Store original event for rollback
			const currentState = get({ subscribe });
			const originalEvent = currentState.events.find((e) => e.id === event.id);

			// Apply optimistic update immediately
			update((state) => ({
				...state,
				events: state.events.map((e) => (e.id === event.id ? event : e))
			}));

			try {
				const client = new EventClient();
				const editDto = calendarEventToEditDto(event);
				const commandId = await client.updateEvent(calendarId, event.id, editDto);

				// Store command ID for SSE correlation (optional - events will update via SSE anyway)
				console.log('Event update command sent:', commandId);
				return commandId;
			} catch (error) {
				// Revert optimistic update on error
				if (originalEvent) {
					update((state) => ({
						...state,
						events: state.events.map((e) => (e.id === event.id ? originalEvent : e))
					}));
				}

				const errorMessage = error instanceof Error ? error.message : 'Failed to update event';
				toastStore.error(errorMessage, 5000);
				throw error;
			}
		},

		/**
		 * Delete an event (CQRS command)
		 * Returns command ID for SSE correlation
		 */
		async deleteEvent(eventId: string): Promise<string> {
			const calendarId = get(calendarsStore).activeCalendarId;

			if (!calendarId) {
				throw new Error('No calendar selected');
			}

			// Store original event for rollback
			const currentState = get({ subscribe });
			const originalEvent = currentState.events.find((e) => e.id === eventId);

			// Apply optimistic update immediately
			update((state) => ({
				...state,
				events: state.events.filter((e) => e.id !== eventId)
			}));

			try {
				const client = new EventClient();
				const commandId = await client.deleteEvent(calendarId, eventId);

				// Store command ID for SSE correlation (optional - events will update via SSE anyway)
				console.log('Event delete command sent:', commandId);
				return commandId;
			} catch (error) {
				// Revert optimistic update on error
				if (originalEvent) {
					update((state) => ({
						...state,
						events: [...state.events, originalEvent]
					}));
				}

				const errorMessage = error instanceof Error ? error.message : 'Failed to delete event';
				toastStore.error(errorMessage, 5000);
				throw error;
			}
		},

		/**
		 * Legacy methods for backward compatibility (deprecated - use CQRS methods above)
		 * TODO: Remove after updating all callers
		 */
		addEvent(event: CalendarEvent): void {
			console.warn('addEvent is deprecated - use createEvent instead');
			update((state) => ({
				...state,
				events: [...state.events, event]
			}));
		},

		/**
		 * Clear all events
		 */
		clear(): void {
			set({
				events: [],
				dateRange: null,
				loading: false,
				error: null
			});
		}
	};
}

export const eventsStore = createEventsStore();
