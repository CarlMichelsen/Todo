import { AuthorizedHttpClient } from './authorizedHttpClient';
import { HttpMethod } from './httpClient';
import type { CalendarDto } from '$lib/types/api/calendar';
import type { CreateCalendarLinkDto, EditCalendarLinkDto } from '$lib/types/api/calendarLink';

export class CalendarLinkClient extends AuthorizedHttpClient {
	/**
	 * Get all calendar links for the current user
	 */
	async getCalendarLinks(): Promise<CalendarDto[]> {
		const response = await this.request<CalendarDto[]>(
			HttpMethod.GET,
			'/api/v1/CalendarLink'
		);

		if (!response.ok) {
			throw new Error('Failed to fetch calendar links');
		}

		return response.data ?? [];
	}

	/**
	 * Get a specific calendar link by ID
	 */
	async getCalendarLink(calendarLinkId: string): Promise<CalendarDto> {
		const response = await this.request<CalendarDto>(
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
	 */
	async createCalendarLink(
		initialParentCalendarId: string,
		calendarLink: CreateCalendarLinkDto
	): Promise<CalendarDto> {
		const response = await this.request<CalendarDto, CreateCalendarLinkDto>(
			HttpMethod.POST,
			`/api/v1/CalendarLink/${initialParentCalendarId}`,
			calendarLink
		);

		if (response.status === 400) {
			throw new Error('Invalid calendar link data');
		}

		if (!response.ok) {
			throw new Error('Failed to create calendar link');
		}

		if (!response.data) {
			throw new Error('Failed to create calendar link: No data returned');
		}

		return response.data;
	}

	/**
	 * Update an existing calendar link
	 */
	async updateCalendarLink(
		calendarLinkId: string,
		updates: EditCalendarLinkDto
	): Promise<CalendarDto> {
		const response = await this.request<CalendarDto, EditCalendarLinkDto>(
			HttpMethod.PUT,
			`/api/v1/CalendarLink/${calendarLinkId}`,
			updates
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

		if (!response.data) {
			throw new Error('Failed to update calendar link: No data returned');
		}

		return response.data;
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
