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


export const SERVER_EVENT_NAMES = [
  "CreateCalendar",
  "EditCalendar",
  "DeleteCalendar",
  "SelectCalendar",
  "CreateCalendarLink",
  "EditCalendarLink",
  "DeleteCalendarLink",
] as const satisfies readonly ServerEvent["eventName"][];


// The following code makes typescript check that all events in ServerEvent are also present in SERVER_EVENT_NAMES.
// This is important because the events are registered from SERVER_EVENT_NAMES in ServerSentEventClient.
export type ServerEventName = typeof SERVER_EVENT_NAMES[number];

// Ensures no missing events
type AssertAllEventsCovered =
  Exclude<ServerEvent["eventName"], ServerEventName> extends never
    ? true
    : never;

// ⬇ forces the check
const _assertAllEventsCovered: AssertAllEventsCovered = true;

// Ensures no extra events
const _assertNoExtraEvents: readonly ServerEvent["eventName"][] =
  SERVER_EVENT_NAMES;