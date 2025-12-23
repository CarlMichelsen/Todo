import { AuthorizedHttpClient } from './authorizedHttpClient';
import { HttpMethod } from './httpClient';
import type { EventDto, CreateEventDto, EditEventDto, PaginationDto } from '$lib/types/api/event';
import type { BadRequestResponse } from '$lib/types/api/response';

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
	 * Uses /api/v1/Event/span/{calendarId} endpoint
	 *
	 * @param calendarId - UUID of the calendar
	 * @param from - Start date (will be converted to ISO string)
	 * @param to - End date (will be converted to ISO string)
	 * @returns Array of EventDto objects with ISO date-time strings
	 */
	async getEventsForDateRange(calendarId: string, from: Date, to: Date): Promise<EventDto[]> {
		const fromISO = from.toISOString();
		const toISO = to.toISOString();

		const response = await this.request<EventDto[]>(
			HttpMethod.GET,
			`/api/v1/Event/span/${calendarId}?from=${encodeURIComponent(fromISO)}&to=${encodeURIComponent(toISO)}`
		);

		if (!response.ok) {
			console.error('Failed to fetch events for date range:', {
				status: response.status,
				from: fromISO,
				to: toISO,
				error: response.data
			});
			return [];
		}

		return response.data ?? [];
	}

	/**
	 * Get paginated events (optional search)
	 * Uses /api/v1/Event/{calendarId} endpoint
	 *
	 * @param calendarId - UUID of the calendar
	 * @param page - Page number (1-indexed)
	 * @param pageSize - Number of items per page
	 * @param search - Optional search query
	 * @returns Pagination wrapper containing EventDto array
	 */
	async getEvents(
		calendarId: string,
		page: number = 1,
		pageSize: number = 50,
		search?: string
	): Promise<PaginationDto<EventDto>> {
		let endpoint = `/api/v1/Event/${calendarId}?Page=${page}&PageSize=${pageSize}`;
		if (search) {
			endpoint += `&search=${encodeURIComponent(search)}`;
		}

		const response = await this.request<PaginationDto<EventDto>>(HttpMethod.GET, endpoint);

		if (!response.ok) {
			if (response.status === 400) {
				const badRequest = response as BadRequestResponse;
				console.error('Validation error fetching events:', {
					errors: badRequest.data.errors,
					detail: badRequest.data.detail
				});
				throw new Error(`Invalid pagination parameters: ${badRequest.data.detail || 'Validation failed'}`);
			}

			console.error('Failed to fetch events:', response.data);
			throw new Error(`Failed to fetch events: ${response.data.title || 'Unknown error'}`);
		}

		if (!response.data) {
			throw new Error('No data returned from server');
		}

		return response.data;
	}

	/**
	 * Get a single event by ID
	 * Uses /api/v1/Event/{calendarId}/{eventId} endpoint
	 *
	 * @param calendarId - UUID of the calendar
	 * @param eventId - UUID of the event
	 * @returns EventDto with ISO date-time strings
	 */
	async getEvent(calendarId: string, eventId: string): Promise<EventDto> {
		const response = await this.request<EventDto>(HttpMethod.GET, `/api/v1/Event/${calendarId}/${eventId}`);

		if (!response.ok) {
			if (response.status === 404) {
				console.warn(`Event not found: ${eventId}`);
				throw new Error('Event not found');
			}

			if (response.status === 400) {
				const badRequest = response as BadRequestResponse;
				console.error('Invalid event ID:', {
					eventId,
					errors: badRequest.data.errors
				});
				throw new Error('Invalid event ID format');
			}

			console.error('Failed to fetch event:', response.data);
			throw new Error(`Failed to fetch event: ${response.data.title || 'Unknown error'}`);
		}

		if (!response.data) {
			throw new Error('Event not found');
		}

		return response.data;
	}

	/**
	 * Create a new event
	 * Uses POST /api/v1/Event/{calendarId} endpoint
	 *
	 * @param calendarId - UUID of the calendar
	 * @param event - CreateEventDto with all required fields (ISO strings for dates)
	 * @returns Created EventDto with generated ID
	 */
	async createEvent(calendarId: string, event: CreateEventDto): Promise<EventDto> {
		const response = await this.request<EventDto>(HttpMethod.POST, `/api/v1/Event/${calendarId}`, event);

		if (!response.ok) {
			if (response.status === 400) {
				const badRequest = response as BadRequestResponse;
				console.error('Event validation failed:', {
					errors: badRequest.data.errors,
					detail: badRequest.data.detail,
					event
				});

				// Format validation errors for display
				const errorMessages = badRequest.data.errors
					? Object.entries(badRequest.data.errors)
							.map(([field, messages]) => `${field}: ${messages.join(', ')}`)
							.join('; ')
					: badRequest.data.detail || 'Validation failed';

				throw new Error(`Invalid event data: ${errorMessages}`);
			}

			console.error('Failed to create event:', response.data);
			throw new Error(`Failed to create event: ${response.data.title || 'Unknown error'}`);
		}

		if (!response.data) {
			throw new Error('Failed to create event: No data returned');
		}

		return response.data;
	}

	/**
	 * Update an existing event
	 * Uses PUT /api/v1/Event/{calendarId}/{eventId} endpoint
	 *
	 * @param calendarId - UUID of the calendar
	 * @param eventId - UUID of the event to update
	 * @param updates - EditEventDto with partial/nullable fields (ISO strings for dates)
	 * @returns Updated EventDto
	 */
	async updateEvent(calendarId: string, eventId: string, updates: EditEventDto): Promise<EventDto> {
		const response = await this.request<EventDto>(
			HttpMethod.PUT,
			`/api/v1/Event/${calendarId}/${eventId}`,
			updates
		);

		if (!response.ok) {
			if (response.status === 404) {
				console.warn(`Event not found for update: ${eventId}`);
				throw new Error('Event not found');
			}

			if (response.status === 400) {
				const badRequest = response as BadRequestResponse;
				console.error('Event update validation failed:', {
					eventId,
					errors: badRequest.data.errors,
					detail: badRequest.data.detail,
					updates
				});

				// Format validation errors for display
				const errorMessages = badRequest.data.errors
					? Object.entries(badRequest.data.errors)
							.map(([field, messages]) => `${field}: ${messages.join(', ')}`)
							.join('; ')
					: badRequest.data.detail || 'Validation failed';

				throw new Error(`Invalid event data: ${errorMessages}`);
			}

			console.error('Failed to update event:', response.data);
			throw new Error(`Failed to update event: ${response.data.title || 'Unknown error'}`);
		}

		if (!response.data) {
			throw new Error('Failed to update event: No data returned');
		}

		return response.data;
	}

	/**
	 * Delete an event
	 * Uses DELETE /api/v1/Event/{calendarId}/{eventId} endpoint
	 *
	 * @param calendarId - UUID of the calendar
	 * @param eventId - UUID of the event to delete
	 * @returns void (throws on error)
	 */
	async deleteEvent(calendarId: string, eventId: string): Promise<void> {
		const response = await this.request<boolean>(HttpMethod.DELETE, `/api/v1/Event/${calendarId}/${eventId}`);

		if (!response.ok) {
			if (response.status === 404) {
				console.warn(`Event not found for deletion: ${eventId}`);
				throw new Error('Event not found');
			}

			console.error('Failed to delete event:', response.data);
			throw new Error(`Failed to delete event: ${response.data.title || 'Unknown error'}`);
		}
	}
}
