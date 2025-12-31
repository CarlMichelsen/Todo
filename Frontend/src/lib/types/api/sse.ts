import type { CalendarDto } from './calendar';
import type { CalendarLinkDto } from './calendarLink';

/**
 * Base interface for all server-sent events
 */
export interface BaseServerEvent {
	eventName: string;
	eventId: string;
	dispatchedAt: string;
}

/**
 * Event sent when a calendar is created
 */
export interface CreateCalendarEvent extends BaseServerEvent {
	eventName: 'CreateCalendar';
	calendar: CalendarDto;
}

/**
 * Event sent when a calendar is edited
 */
export interface EditCalendarEvent extends BaseServerEvent {
	eventName: 'EditCalendar';
	calendar: CalendarDto;
}

/**
 * Event sent when a calendar is deleted
 */
export interface DeleteCalendarEvent extends BaseServerEvent {
	eventName: 'DeleteCalendar';
	calendarId: string;
	calendarTitle: string;
}

/**
 * Event sent when a calendar is selected
 */
export interface SelectCalendarEvent extends BaseServerEvent {
	eventName: 'SelectCalendar';
	calendarId: string;
}

/**
 * Event sent when a calendar link is created
 */
export interface CreateCalendarLinkEvent extends BaseServerEvent {
	eventName: 'CreateCalendarLink';
	calendarLink: CalendarLinkDto;
}

/**
 * Event sent when a calendar link is edited
 */
export interface EditCalendarLinkEvent extends BaseServerEvent {
	eventName: 'EditCalendarLink';
	calendarLink: CalendarLinkDto;
}

/**
 * Event sent when a calendar link is deleted
 */
export interface DeleteCalendarLinkEvent extends BaseServerEvent {
	eventName: 'DeleteCalendarLink';
	calendarLinkId: string;
	title: string;
	productId: string | null;
}

/**
 * Union type of all possible server events
 */
export type ServerEvent =
	| CreateCalendarEvent
	| EditCalendarEvent
	| DeleteCalendarEvent
	| SelectCalendarEvent
	| CreateCalendarLinkEvent
	| EditCalendarLinkEvent
	| DeleteCalendarLinkEvent;
