import type { UserDto } from './calendar';

export interface CalendarLinkDto {
	id: string; // UUID
	parentCalendars: string[]; // UUID array - calendars this link is associated with
	user: UserDto;
	createdAt: string; // ISO date-time
	title: string;
	calendarLink: string; // URI (ICS URL)
	color: string; // Hex color
}

export interface CreateCalendarLinkDto {
	title: string;
	calendarLink: string; // URI format
	color: string; // Hex color code (e.g., "#ea580c")
}

export interface EditCalendarLinkDto {
	title?: string | null;
	calendarLink?: string | null; // URI format
	color?: string | null; // Hex color code (nullable)
	deleteParentCalendarAssociation?: string[] | null; // UUID array
	addParentCalendarAssociation?: string[] | null; // UUID array
}
