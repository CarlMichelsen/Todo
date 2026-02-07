import type { EventDto, CreateEventDto, EditEventDto, RecurrenceDto } from '$lib/types/api/event';
import type { CalendarEvent, EventStatus, RecurrenceRule } from '$lib/types/calendar';

/**
 * Convert EventStatus number to EventStatus enum
 */
function numberToStatus(status: number): EventStatus {
	const statusMap: Record<number, EventStatus> = {
		0: 'confirmed',
		1: 'tentative',
		2: 'cancelled',
		3: 'pending'
	};
	return statusMap[status] || 'confirmed';
}

/**
 * Convert RecurrenceDto to RecurrenceRule
 */
function dtoToRecurrence(dto: RecurrenceDto | null): RecurrenceRule | null {
	if (!dto || !dto.isRecurring) {
		return null;
	}

	const frequencyMap: Record<number, RecurrenceRule['frequency']> = {
		0: 'daily',
		1: 'weekly',
		2: 'monthly',
		3: 'yearly'
	};

	return {
		frequency: frequencyMap[dto.pattern || 0] || 'weekly',
		interval: dto.intervalValue || 1,
		daysOfWeek: dto.daysOfWeek,
		dayOfMonth: dto.dayOfMonth,
		endDate: dto.endDate ? new Date(dto.endDate) : undefined,
		count: dto.occurrences
	};
}

/**
 * Convert RecurrenceRule to RecurrenceDto
 */
function recurrenceToDto(rule: RecurrenceRule): RecurrenceDto {
	const frequencyMap: Record<RecurrenceRule['frequency'], number> = {
		daily: 0,
		weekly: 1,
		monthly: 2,
		yearly: 3
	};

	const patternMap: Record<RecurrenceRule['frequency'], number> = {
		daily: 0,
		weekly: 1,
		monthly: 2,
		yearly: 3
	};

	return {
		isRecurring: true,
		pattern: frequencyMap[rule.frequency] || 1,
		intervalValue: rule.interval,
		daysOfWeek: rule.daysOfWeek,
		dayOfMonth: rule.dayOfMonth,
		endDate: rule.endDate?.toISOString(),
		occurrences: rule.count
	};
}

/**
 * Convert EventStatus enum to number
 */
function statusToNumber(status: EventStatus): number {
	const statusMap: Record<EventStatus, number> = {
		confirmed: 0,
		tentative: 1,
		cancelled: 2,
		pending: 3
	};
	return statusMap[status] || 0;
}

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
		color: dto.color,
		location: dto.location || undefined,
		isAllDay: dto.isAllDay,
		status: numberToStatus(dto.status),
		attendees: dto.attendees?.map((a) => ({
			email: a.email,
			commonName: a.commonName,
			isOrganizer: dto.createdBy.userId === a.email // Simple organizer detection
		})),
		recurrence: dtoToRecurrence(dto.recurrence) || undefined
	};
}

/**
 * Convert a CalendarEvent to CreateEventDto for API requests
 * Handles conversion of Date objects to ISO 8601 strings
 *
 * @param event - CalendarEvent with Date objects
 * @returns CreateEventDto with ISO string dates
 */
export function calendarEventToCreateDto(event: CalendarEvent): CreateEventDto {
	return {
		title: event.title,
		description: event.description || '',
		attendees:
			event.attendees?.map((a) => ({
				email: a.email,
				commonName: a.commonName
			})) || [],
		status: statusToNumber(event.status || 'confirmed'),
		location: event.location || undefined,
		isAllDay: event.isAllDay || false,
		start: event.start.toISOString(),
		end: event.end.toISOString(),
		color: event.color || '#ea580c',
		recurrence: event.recurrence ? recurrenceToDto(event.recurrence) : null
	};
}

/**
 * Convert a CalendarEvent to EditEventDto for API requests
 * Handles conversion of Date objects to ISO 8601 strings
 *
 * @param event - CalendarEvent with Date objects
 * @returns EditEventDto with ISO string dates
 */
export function calendarEventToEditDto(event: CalendarEvent): EditEventDto {
	return {
		title: event.title,
		description: event.description,
		start: event.start.toISOString(),
		end: event.end.toISOString(),
		color: event.color,
		recurrence: event.recurrence ? recurrenceToDto(event.recurrence) : null
	};
}
