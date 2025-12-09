/**
 * Calendar type definitions
 */

/**
 * Represents a calendar event that can span one or more days
 */
export interface CalendarEvent {
	/** Unique identifier for the event */
	id: string;
	/** Event title */
	title: string;
	/** Optional event description */
	description?: string;
	/** Start date in ISO format (YYYY-MM-DD) */
	startDate: string;
	/** End date in ISO format (YYYY-MM-DD) */
	endDate: string;
	/** Start time in HH:MM format (e.g., "09:30") */
	startTime: string;
	/** End time in HH:MM format (e.g., "10:30") */
	endTime: string;
	/** Optional hex color for event bar (default: orange #ea580c) */
	color?: string;
}

/**
 * Represents a calendar day with metadata
 */
export interface CalendarDay {
	/** The Date object for this day */
	date: Date;
	/** Whether this day is today */
	isToday: boolean;
	/** Whether this day is a weekend (Saturday or Sunday) */
	isWeekend: boolean;
	/** The day of the month (1-31) */
	dayOfMonth: number;
	/** Short day name ("Sun", "Mon", etc.) */
	dayOfWeek: string;
	/** Events scheduled for this day */
	events: CalendarEvent[];
}

/**
 * Calendar state management
 */
export interface CalendarState {
	/** The Monday (start) of the currently displayed week */
	currentWeekStart: Date;
	/** The currently selected date, or null if none selected */
	selectedDate: Date | null;
}
