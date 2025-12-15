import { writable } from 'svelte/store';
import type { CalendarEvent } from '$lib/types/calendar';
import { generateMockEvents } from '$lib/utils/mockEvents';
import { getWeekStart } from '$lib/utils/calendarUtils';
import { EventClient } from '$lib/utils/eventClient';
import { eventDtoToCalendarEvent } from '$lib/utils/eventConverter';

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
	// Initialize with current week's events to avoid race condition
	const today = new Date();
	const currentWeekStart = getWeekStart(today);
	const currentWeekEnd = new Date(currentWeekStart);
	currentWeekEnd.setDate(currentWeekStart.getDate() + 6);
	currentWeekEnd.setHours(23, 59, 59, 999);

	const { subscribe, set, update } = writable<EventStoreState>({
		events: generateMockEvents(currentWeekStart),
		dateRange: {
			start: currentWeekStart,
			end: currentWeekEnd
		},
		loading: false,
		error: null
	});

	return {
		subscribe,

		/**
		 * Set the visible date range and load events for that range
		 * @param weekStart - Monday of the week to load
		 */
		async setDateRange(weekStart: Date): Promise<void> {
			const weekEnd = new Date(weekStart);
			weekEnd.setDate(weekStart.getDate() + 6);
			weekEnd.setHours(23, 59, 59, 999);

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
				const eventDtos = await client.getEventsForDateRange(weekStart, weekEnd);
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
		 * Add a new event
		 */
		addEvent(event: CalendarEvent): void {
			update((state) => ({
				...state,
				events: [...state.events, event]
			}));
		},

		/**
		 * Update an existing event
		 */
		updateEvent(id: string, updates: Partial<CalendarEvent>): void {
			update((state) => ({
				...state,
				events: state.events.map((e) => (e.id === id ? { ...e, ...updates } : e))
			}));
		},

		/**
		 * Delete an event
		 */
		deleteEvent(id: string): void {
			update((state) => ({
				...state,
				events: state.events.filter((e) => e.id !== id)
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
