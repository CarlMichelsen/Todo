import { AuthorizedHttpClient } from './authorizedHttpClient';
import { HttpMethod } from './httpClient';
import type { CalendarLinkDto, CreateCalendarLinkDto, EditCalendarLinkDto } from '$lib/types/api/calendarLink';

export class CalendarLinkClient extends AuthorizedHttpClient {
	/**
	 * @deprecated Use getCalendarLinksForCalendar() instead
	 * Get all calendar links for the current user
	 */
	async getCalendarLinks(): Promise<CalendarLinkDto[]> {
		const response = await this.request<CalendarLinkDto[]>(
			HttpMethod.GET,
			'/api/v1/CalendarLink'
		);

		if (!response.ok) {
			throw new Error('Failed to fetch calendar links');
		}

		return response.data ?? [];
	}

	/**
	 * Get calendar links for a specific parent calendar
	 * Uses GET /api/v1/CalendarLink/calendar/{calendarId}
	 */
	async getCalendarLinksForCalendar(calendarId: string): Promise<CalendarLinkDto[]> {
		const response = await this.request<CalendarLinkDto[]>(
			HttpMethod.GET,
			`/api/v1/CalendarLink/calendar/${calendarId}`
		);

		if (!response.ok) {
			throw new Error('Failed to fetch calendar links');
		}

		return response.data ?? [];
	}

	/**
	 * Get a specific calendar link by ID
	 */
	async getCalendarLink(calendarLinkId: string): Promise<CalendarLinkDto> {
		const response = await this.request<CalendarLinkDto>(
			HttpMethod.GET,
			`/api/v1/CalendarLink/${calendarLinkId}`
		);

		if (response.status === 404) {
			throw new Error('Calendar link not found');
		}

		if (!response.ok) {
			throw new Error('Failed to fetch calendar link');
		}

		if (!response.data) {
			throw new Error('Calendar link not found');
		}

		return response.data;
	}

	/**
	 * Create a new calendar link associated with a parent calendar
	 * CQRS: Fire-and-forget - returns immediately, state updates via SSE
	 */
	async createCalendarLink(
		initialParentCalendarId: string,
		calendarLink: CreateCalendarLinkDto,
		commandId?: string
	): Promise<string> {
		const id = commandId || crypto.randomUUID();

		const response = await this.request<void, CreateCalendarLinkDto>(
			HttpMethod.POST,
			`/api/v1/CalendarLink/${initialParentCalendarId}`,
			calendarLink,
			{ headers: { 'X-Command-ID': id } }
		);

		if (response.status === 400) {
			throw new Error('Invalid calendar link data');
		}

		if (!response.ok) {
			throw new Error('Failed to create calendar link');
		}

		return id;
	}

	/**
	 * Update an existing calendar link
	 * CQRS: Fire-and-forget - returns immediately, state updates via SSE
	 */
	async updateCalendarLink(
		calendarLinkId: string,
		updates: EditCalendarLinkDto,
		commandId?: string
	): Promise<string> {
		const id = commandId || crypto.randomUUID();

		const response = await this.request<void, EditCalendarLinkDto>(
			HttpMethod.PUT,
			`/api/v1/CalendarLink/${calendarLinkId}`,
			updates,
			{ headers: { 'X-Command-ID': id } }
		);

		if (response.status === 404) {
			throw new Error('Calendar link not found');
		}

		if (response.status === 400) {
			throw new Error('Invalid calendar link data');
		}

		if (!response.ok) {
			throw new Error('Failed to update calendar link');
		}

		return id;
	}

	/**
	 * Delete a calendar link
	 */
	async deleteCalendarLink(calendarLinkId: string): Promise<void> {
		const response = await this.request<void>(
			HttpMethod.DELETE,
			`/api/v1/CalendarLink/${calendarLinkId}`
		);

		if (response.status === 404) {
			throw new Error('Calendar link not found');
		}

		if (!response.ok) {
			throw new Error('Failed to delete calendar link');
		}
	}
}
