import type { EventDto } from '$lib/types/api/event';
import type { CalendarEvent } from '$lib/types/calendar';

/**
 * Convert an EventDto (from API) to a CalendarEvent (for store/UI)
 * Handles conversion of ISO 8601 date strings to Date objects
 *
 * @param dto - Event DTO from API with ISO string dates
 * @returns CalendarEvent with Date objects
 */
export function eventDtoToCalendarEvent(dto: EventDto): CalendarEvent {
	return {
		id: dto.id,
		title: dto.title,
		description: dto.description,
		start: new Date(dto.start),
		end: new Date(dto.end),
		color: dto.color
	};
}
