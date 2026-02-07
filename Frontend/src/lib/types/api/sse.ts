import type { CalendarDto } from './calendar';
import type { CalendarLinkDto } from './calendarLink';
import type { EventDto } from './event';

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
	deleteParentCalendarAssociation: string[]; // null if calendar link edit has no deleted associations
	addParentCalendarAssociation: string[]; // null if calendar link edit has no added associations
	calendarLink: CalendarLinkDto; // contains identifier and source of current truth for the edited calendarLink
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
 * Event sent when an event is created
 */
export interface CreateEventEvent extends BaseServerEvent {
	eventName: 'CreateEvent';
	event: EventDto;
	calendarId: string;
}

/**
 * Event sent when an event is edited
 */
export interface EditEventEvent extends BaseServerEvent {
	eventName: 'EditEvent';
	event: EventDto;
	calendarId: string;
}

/**
 * Event sent when an event is deleted
 */
export interface DeleteEventEvent extends BaseServerEvent {
	eventName: 'DeleteEvent';
	calendarEventId: string;
	eventTitle: string;
	calendarId: string;
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
	| DeleteCalendarLinkEvent
	| CreateEventEvent
	| EditEventEvent
	| DeleteEventEvent;

export const SERVER_EVENT_NAMES = [
	'CreateCalendar',
	'EditCalendar',
	'DeleteCalendar',
	'SelectCalendar',
	'CreateCalendarLink',
	'EditCalendarLink',
	'DeleteCalendarLink',
	'CreateEvent',
	'EditEvent',
	'DeleteEvent'
] as const satisfies readonly ServerEvent['eventName'][];

// The following code makes typescript check that all events in ServerEvent are also present in SERVER_EVENT_NAMES.
// This is important because the events are registered from SERVER_EVENT_NAMES in ServerSentEventClient.
export type ServerEventName = (typeof SERVER_EVENT_NAMES)[number];

// Ensures no missing events
type AssertAllEventsCovered =
	Exclude<ServerEvent['eventName'], ServerEventName> extends never ? true : never;

// Ensures no duplicate events
type HasDuplicates<T extends readonly string[]> = T extends readonly [
	infer First,
	...infer Rest extends readonly string[]
]
	? First extends Rest[number]
		? true
		: HasDuplicates<Rest>
	: false;

type AssertNoDuplicates = HasDuplicates<typeof SERVER_EVENT_NAMES> extends false ? true : never;

// ⬇ forces the check
/* eslint-disable @typescript-eslint/no-unused-vars */
const _assertAllEventsCovered: AssertAllEventsCovered = true;

// Ensures no extra events
/* eslint-disable @typescript-eslint/no-unused-vars */
const _assertNoExtraEvents: readonly ServerEvent['eventName'][] = SERVER_EVENT_NAMES;

/* eslint-disable @typescript-eslint/no-unused-vars */
const _assertNoDuplicates: AssertNoDuplicates = true;
