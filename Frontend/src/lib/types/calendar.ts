/**
 * Calendar type definitions
 */

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
