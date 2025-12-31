import { AuthorizedHttpClient } from './authorizedHttpClient';
import { HttpMethod } from './httpClient';
import type { CalendarDto, CreateCalendarDto, EditCalendarDto } from '$lib/types/api/calendar';
import type { BadRequestResponse } from '$lib/types/api/response';

/**
 * Client for calendar-related API operations
 * Handles CRUD operations for calendars via the backend API
 */
export class CalendarClient extends AuthorizedHttpClient {
	/**
	 * Get all calendars for the current user
	 * Uses GET /api/v1/Calendar endpoint
	 *
	 * @returns Array of CalendarDto objects
	 */
	async getCalendars(): Promise<CalendarDto[]> {
		const response = await this.request<CalendarDto[]>(HttpMethod.GET, '/api/v1/Calendar');

		if (!response.ok) {
			console.error('Failed to fetch calendars:', {
				status: response.status,
				error: response.data
			});
			return [];
		}

		return response.data ?? [];
	}

	/**
	 * Get a single calendar by ID
	 * Uses GET /api/v1/Calendar/{calendarId} endpoint
	 *
	 * @param calendarId - UUID of the calendar
	 * @returns CalendarDto
	 */
	async getCalendar(calendarId: string): Promise<CalendarDto> {
		const response = await this.request<CalendarDto>(
			HttpMethod.GET,
			`/api/v1/Calendar/${calendarId}`
		);

		if (!response.ok) {
			if (response.status === 404) {
				console.warn(`Calendar not found: ${calendarId}`);
				throw new Error('Calendar not found');
			}

			if (response.status === 400) {
				const badRequest = response as BadRequestResponse;
				console.error('Invalid calendar ID:', {
					calendarId,
					errors: badRequest.data.errors
				});
				throw new Error('Invalid calendar ID format');
			}

			console.error('Failed to fetch calendar:', response.data);
			throw new Error(`Failed to fetch calendar: ${response.data.title || 'Unknown error'}`);
		}

		if (!response.data) {
			throw new Error('Calendar not found');
		}

		return response.data;
	}

	/**
	 * Select a calendar as the active calendar for the current user
	 * Uses POST /api/v1/Calendar/{calendarId} endpoint
	 * This persists the selection to the server so it's restored on next login
	 * CQRS: Returns command ID for SSE correlation
	 *
	 * @param calendarId - UUID of the calendar to select
	 * @param commandId - Optional command ID (generated if not provided)
	 * @returns Command ID for SSE event correlation
	 */
	async selectCalendar(calendarId: string, commandId?: string): Promise<string> {
		const id = commandId || crypto.randomUUID();

		const response = await this.request<void>(
			HttpMethod.POST,
			`/api/v1/Calendar/${calendarId}`,
			undefined,
			{ headers: { 'X-Command-ID': id } }
		);

		if (!response.ok) {
			if (response.status === 404) {
				throw new Error('Calendar not found');
			}

			if (response.status === 400) {
				throw new Error('Invalid calendar ID format');
			}

			throw new Error('Failed to select calendar');
		}

		return id;
	}

	/**
	 * Create a new calendar
	 * Uses POST /api/v1/Calendar endpoint
	 * CQRS: Returns command ID for SSE correlation
	 *
	 * @param calendar - CreateCalendarDto with all required fields
	 * @param commandId - Optional command ID (generated if not provided)
	 * @returns Command ID for SSE event correlation
	 */
	async createCalendar(calendar: CreateCalendarDto, commandId?: string): Promise<string> {
		const id = commandId || crypto.randomUUID();

		const response = await this.request<void>(
			HttpMethod.POST,
			'/api/v1/Calendar',
			calendar,
			{ headers: { 'X-Command-ID': id } }
		);

		if (!response.ok) {
			if (response.status === 400) {
				const badRequest = response as BadRequestResponse;
				console.error('Calendar validation failed:', {
					errors: badRequest.data.errors,
					detail: badRequest.data.detail,
					calendar
				});

				// Format validation errors for display
				const errorMessages = badRequest.data.errors
					? Object.entries(badRequest.data.errors)
							.map(([field, messages]) => `${field}: ${messages.join(', ')}`)
							.join('; ')
					: badRequest.data.detail || 'Validation failed';

				throw new Error(`Invalid calendar data: ${errorMessages}`);
			}

			throw new Error('Failed to create calendar');
		}

		return id;
	}

	/**
	 * Update an existing calendar
	 * Uses PUT /api/v1/Calendar/{calendarId} endpoint
	 * CQRS: Returns command ID for SSE correlation
	 *
	 * @param calendarId - UUID of the calendar to update
	 * @param updates - EditCalendarDto with partial/nullable fields
	 * @param commandId - Optional command ID (generated if not provided)
	 * @returns Command ID for SSE event correlation
	 */
	async updateCalendar(calendarId: string, updates: EditCalendarDto, commandId?: string): Promise<string> {
		const id = commandId || crypto.randomUUID();

		const response = await this.request<void>(
			HttpMethod.PUT,
			`/api/v1/Calendar/${calendarId}`,
			updates,
			{ headers: { 'X-Command-ID': id } }
		);

		if (!response.ok) {
			if (response.status === 404) {
				throw new Error('Calendar not found');
			}

			if (response.status === 400) {
				const badRequest = response as BadRequestResponse;
				console.error('Calendar update validation failed:', {
					calendarId,
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

				throw new Error(`Invalid calendar data: ${errorMessages}`);
			}

			throw new Error('Failed to update calendar');
		}

		return id;
	}

	/**
	 * Delete a calendar
	 * Uses DELETE /api/v1/Calendar/{calendarId} endpoint
	 * CQRS: Returns command ID for SSE correlation
	 *
	 * @param calendarId - UUID of the calendar to delete
	 * @param commandId - Optional command ID (generated if not provided)
	 * @returns Command ID for SSE event correlation
	 */
	async deleteCalendar(calendarId: string, commandId?: string): Promise<string> {
		const id = commandId || crypto.randomUUID();

		const response = await this.request<boolean>(
			HttpMethod.DELETE,
			`/api/v1/Calendar/${calendarId}`,
			undefined,
			{ headers: { 'X-Command-ID': id } }
		);

		if (!response.ok) {
			if (response.status === 404) {
				console.warn(`Calendar not found for deletion: ${calendarId}`);
				throw new Error('Calendar not found');
			}

			console.error('Failed to delete calendar:', response.data);
			throw new Error(`Failed to delete calendar: ${response.data.title || 'Unknown error'}`);
		}

		return id;
	}
}
