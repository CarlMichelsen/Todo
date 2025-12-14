import { AuthorizedHttpClient } from './authorizedHttpClient';
import { HttpMethod } from './httpClient';
import type { EventDto, CreateEventDto, EditEventDto, PaginationDto } from '$lib/types/api/event';

/**
 * Client for event-related API operations
 * Handles CRUD operations for events via the backend API
 *
 * Note: This client works with API DTOs (EventDto, etc.) which are separate
 * from the CalendarEvent type used in the store. Consumers are responsible
 * for any necessary conversions.
 */
export class EventClient extends AuthorizedHttpClient {
	/**
	 * Get events for a specific date range
	 * Uses /api/v1/Event/current endpoint
	 *
	 * @param from - Start date (will be converted to ISO string)
	 * @param to - End date (will be converted to ISO string)
	 * @returns Array of EventDto objects with ISO date-time strings
	 */
	async getEventsForDateRange(from: Date, to: Date): Promise<EventDto[]> {
		try {
			const fromISO = from.toISOString();
			const toISO = to.toISOString();

			const events = await this.request<EventDto[]>(
				HttpMethod.GET,
				`/api/v1/Event/current?from=${encodeURIComponent(fromISO)}&to=${encodeURIComponent(toISO)}`
			);

			if (!events) {
				return [];
			}

			return events;
		} catch (error) {
			// Handle 401 gracefully (user not authenticated)
			if (error instanceof Error && error.message.includes('HTTP 401')) {
				return [];
			}
			throw error;
		}
	}

	/**
	 * Get paginated events (optional search)
	 * Uses /api/v1/Event endpoint
	 *
	 * @param page - Page number (1-indexed)
	 * @param pageSize - Number of items per page
	 * @param search - Optional search query
	 * @returns Pagination wrapper containing EventDto array
	 */
	async getEvents(
		page: number = 1,
		pageSize: number = 50,
		search?: string
	): Promise<PaginationDto<EventDto>> {
		try {
			let endpoint = `/api/v1/Event?Page=${page}&PageSize=${pageSize}`;
			if (search) {
				endpoint += `&search=${encodeURIComponent(search)}`;
			}

			const response = await this.request<PaginationDto<EventDto>>(HttpMethod.GET, endpoint);

			if (!response) {
				throw new Error('No response from server');
			}

			return response;
		} catch (error) {
			if (error instanceof Error && error.message.includes('HTTP 401')) {
				throw new Error('Authentication required');
			}
			throw error;
		}
	}

	/**
	 * Get a single event by ID
	 * Uses /api/v1/Event/{eventId} endpoint
	 *
	 * @param eventId - UUID of the event
	 * @returns EventDto with ISO date-time strings
	 */
	async getEvent(eventId: string): Promise<EventDto> {
		try {
			const event = await this.request<EventDto>(HttpMethod.GET, `/api/v1/Event/${eventId}`);

			if (!event) {
				throw new Error('Event not found');
			}

			return event;
		} catch (error) {
			if (error instanceof Error && error.message.includes('HTTP 404')) {
				throw new Error('Event not found');
			}
			throw error;
		}
	}

	/**
	 * Create a new event
	 * Uses POST /api/v1/Event endpoint
	 *
	 * @param event - CreateEventDto with all required fields (ISO strings for dates)
	 * @returns Created EventDto with generated ID
	 */
	async createEvent(event: CreateEventDto): Promise<EventDto> {
		try {
			const created = await this.request<EventDto>(HttpMethod.POST, '/api/v1/Event', event);

			if (!created) {
				throw new Error('Failed to create event');
			}

			return created;
		} catch (error) {
			if (error instanceof Error && error.message.includes('HTTP 400')) {
				throw new Error('Invalid event data');
			}
			throw error;
		}
	}

	/**
	 * Update an existing event
	 * Uses PUT /api/v1/Event/{eventId} endpoint
	 *
	 * @param eventId - UUID of the event to update
	 * @param updates - EditEventDto with partial/nullable fields (ISO strings for dates)
	 * @returns Updated EventDto
	 */
	async updateEvent(eventId: string, updates: EditEventDto): Promise<EventDto> {
		try {
			const updated = await this.request<EventDto>(
				HttpMethod.PUT,
				`/api/v1/Event/${eventId}`,
				updates
			);

			if (!updated) {
				throw new Error('Failed to update event');
			}

			return updated;
		} catch (error) {
			if (error instanceof Error && error.message.includes('HTTP 404')) {
				throw new Error('Event not found');
			}
			if (error instanceof Error && error.message.includes('HTTP 400')) {
				throw new Error('Invalid event data');
			}
			throw error;
		}
	}

	/**
	 * Delete an event
	 * Uses DELETE /api/v1/Event/{eventId} endpoint
	 *
	 * @param eventId - UUID of the event to delete
	 * @returns void (throws on error)
	 */
	async deleteEvent(eventId: string): Promise<void> {
		try {
			await this.request<boolean>(HttpMethod.DELETE, `/api/v1/Event/${eventId}`);
		} catch (error) {
			if (error instanceof Error && error.message.includes('HTTP 404')) {
				throw new Error('Event not found');
			}
			throw error;
		}
	}
}
