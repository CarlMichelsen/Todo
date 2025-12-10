/**
 * Calendar utility functions for date calculations
 * All functions use native JavaScript Date methods with no external dependencies
 */

/**
 * Get the start of the week (Monday) for a given date
 * @param date - The date to find the week start for
 * @returns A new Date object representing the Monday of the week
 */
export function getWeekStart(date: Date): Date {
	const d = new Date(date);
	const day = d.getDay();
	// Convert Sunday (0) to 7, Monday (1) stays 1, etc.
	const diff = day === 0 ? -6 : 1 - day;
	d.setDate(d.getDate() + diff);
	d.setHours(0, 0, 0, 0); // Set to start of day
	return d;
}

/**
 * Get array of 7 dates for the week containing the given date
 * Returns [Monday, Tuesday, ..., Sunday]
 * @param date - The date within the week
 * @returns Array of 7 Date objects representing the week
 */
export function getWeekDates(date: Date): Date[] {
	const weekStart = getWeekStart(date);
	const dates: Date[] = [];

	for (let i = 0; i < 7; i++) {
		const d = new Date(weekStart);
		d.setDate(weekStart.getDate() + i);
		dates.push(d);
	}

	return dates;
}

/**
 * Check if two dates are the same day (ignore time)
 * @param date1 - First date to compare
 * @param date2 - Second date to compare
 * @returns True if dates represent the same day
 */
export function isSameDay(date1: Date, date2: Date): boolean {
	return (
		date1.getFullYear() === date2.getFullYear() &&
		date1.getMonth() === date2.getMonth() &&
		date1.getDate() === date2.getDate()
	);
}

/**
 * Check if date is today
 * @param date - The date to check
 * @returns True if the date is today
 */
export function isToday(date: Date): boolean {
	return isSameDay(date, new Date());
}

/**
 * Check if date is a weekend (Saturday or Sunday)
 * @param date - The date to check
 * @returns True if the date is Saturday or Sunday
 */
export function isWeekend(date: Date): boolean {
	const day = date.getDay();
	return day === 0 || day === 6; // Sunday = 0, Saturday = 6
}

/**
 * Format date for display: "Mon, Jan 15"
 * @param date - The date to format
 * @returns Formatted date string
 */
export function formatDayHeader(date: Date): string {
	const dayNames = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
	const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

	const dayName = dayNames[date.getDay()];
	const monthName = monthNames[date.getMonth()];
	const dayOfMonth = date.getDate();

	return `${dayName}, ${monthName} ${dayOfMonth}`;
}

/**
 * Get month and year for week header: "January 2025"
 * Uses the month of the Thursday of the week (ISO week date standard)
 * This ensures the displayed month is the one that contains most of the week
 * @param weekStartDate - The Monday (start) of the week
 * @returns Formatted month and year string
 */
export function formatWeekMonth(weekStartDate: Date): string {
	const monthNames = [
		'January',
		'February',
		'March',
		'April',
		'May',
		'June',
		'July',
		'August',
		'September',
		'October',
		'November',
		'December'
	];

	// Get Thursday of the week (3 days after Monday)
	const thursday = new Date(weekStartDate);
	thursday.setDate(weekStartDate.getDate() + 3);

	const monthName = monthNames[thursday.getMonth()];
	const year = thursday.getFullYear();

	return `${monthName} ${year}`;
}

/**
 * Add or subtract weeks from a date
 * @param date - The starting date
 * @param weeks - Number of weeks to add (positive) or subtract (negative)
 * @returns A new Date object with the weeks added/subtracted
 */
export function addWeeks(date: Date, weeks: number): Date {
	const d = new Date(date);
	d.setDate(d.getDate() + weeks * 7);
	return d;
}

/**
 * Add or subtract days from a date
 * @param date - The starting date
 * @param days - Number of days to add (positive) or subtract (negative)
 * @returns A new Date object with the days added/subtracted
 */
export function addDays(date: Date, days: number): Date {
	const d = new Date(date);
	d.setDate(d.getDate() + days);
	return d;
}

/**
 * Event overlap detection and layout utilities
 */

import type { CalendarEvent, EventLayout } from '$lib/types/calendar';

/**
 * Convert HH:MM time string to minutes since midnight
 * @param time - Time in HH:MM format (e.g., "09:30")
 * @returns Minutes since midnight (0-1439)
 */
export function timeToMinutes(time: string): number {
	const [hours, minutes] = time.split(':').map(Number);
	return hours * 60 + minutes;
}

/**
 * Convert Date object to minutes since midnight
 * @param date - Date object
 * @returns Minutes since midnight (0-1439)
 */
export function dateToMinutes(date: Date): number {
	return date.getHours() * 60 + date.getMinutes();
}

/**
 * Date/Time conversion utilities for CalendarEvent with Date objects
 */

/**
 * Combine separate date and time strings into a Date object
 * @param dateStr - ISO date string (YYYY-MM-DD)
 * @param timeStr - Time string (HH:MM)
 * @returns Date object with combined date and time
 */
export function combineDateAndTime(dateStr: string, timeStr: string): Date {
	const [hours, minutes] = timeStr.split(':').map(Number);
	const date = new Date(dateStr);
	date.setHours(hours, minutes, 0, 0);
	return date;
}

/**
 * Extract date portion from Date object as ISO string
 * @param date - Date object
 * @returns ISO date string (YYYY-MM-DD)
 */
export function extractDateString(date: Date): string {
	return date.toISOString().split('T')[0];
}

/**
 * Extract time portion from Date object as HH:MM string
 * @param date - Date object
 * @returns Time string (HH:MM)
 */
export function extractTimeString(date: Date): string {
	const hours = String(date.getHours()).padStart(2, '0');
	const minutes = String(date.getMinutes()).padStart(2, '0');
	return `${hours}:${minutes}`;
}

/**
 * Check if an event occurs on a specific date (ignores time)
 * @param eventStart - Event start Date
 * @param eventEnd - Event end Date
 * @param targetDate - Date to check against
 * @returns True if event spans the target date
 */
export function eventOccursOnDate(eventStart: Date, eventEnd: Date, targetDate: Date): boolean {
	// Normalize all dates to midnight for comparison
	const startDay = new Date(eventStart);
	startDay.setHours(0, 0, 0, 0);

	const endDay = new Date(eventEnd);
	endDay.setHours(0, 0, 0, 0);

	const targetDay = new Date(targetDate);
	targetDay.setHours(0, 0, 0, 0);

	return startDay.getTime() <= targetDay.getTime() && endDay.getTime() >= targetDay.getTime();
}

/**
 * Check if an event spans multiple days (ignores time)
 * @param eventStart - Event start Date
 * @param eventEnd - Event end Date
 * @returns True if event spans more than one calendar day
 */
export function isMultiDayEvent(eventStart: Date, eventEnd: Date): boolean {
	return !isSameDay(eventStart, eventEnd);
}

/**
 * Get the start time for display (00:00 if event started before target date)
 * @param eventStart - Event start Date
 * @param targetDate - Date being rendered
 * @returns Date object with appropriate time
 */
export function getDisplayStartTime(eventStart: Date, targetDate: Date): Date {
	const targetDay = new Date(targetDate);
	targetDay.setHours(0, 0, 0, 0);

	const eventStartDay = new Date(eventStart);
	eventStartDay.setHours(0, 0, 0, 0);

	if (eventStartDay.getTime() < targetDay.getTime()) {
		// Event started before this day, show from 00:00
		const result = new Date(targetDate);
		result.setHours(0, 0, 0, 0);
		return result;
	}

	return eventStart;
}

/**
 * Get the end time for display (23:59 if event continues after target date)
 * @param eventEnd - Event end Date
 * @param targetDate - Date being rendered
 * @returns Date object with appropriate time
 */
export function getDisplayEndTime(eventEnd: Date, targetDate: Date): Date {
	const targetDay = new Date(targetDate);
	targetDay.setHours(0, 0, 0, 0);

	const eventEndDay = new Date(eventEnd);
	eventEndDay.setHours(0, 0, 0, 0);

	if (eventEndDay.getTime() > targetDay.getTime()) {
		// Event continues after this day, show until 23:59
		const result = new Date(targetDate);
		result.setHours(23, 59, 0, 0);
		return result;
	}

	return eventEnd;
}

/**
 * Check if two events overlap in time
 * Events overlap if one starts before the other ends
 * @param event1 - First event
 * @param event2 - Second event
 * @returns True if events have overlapping time ranges
 */
export function eventsOverlap(event1: CalendarEvent, event2: CalendarEvent): boolean {
	const e1Start = dateToMinutes(event1.start);
	const e1End = dateToMinutes(event1.end);
	const e2Start = dateToMinutes(event2.start);
	const e2End = dateToMinutes(event2.end);

	// Classic interval overlap: A starts before B ends AND B starts before A ends
	return e1Start < e2End && e2Start < e1End;
}

/**
 * Group events that overlap (transitive closure)
 * If A overlaps B and B overlaps C, all three are in the same group
 * @param sortedEvents - Events sorted by start time
 * @returns Array of event groups
 */
function buildOverlapGroups(sortedEvents: CalendarEvent[]): CalendarEvent[][] {
	const groups: CalendarEvent[][] = [];
	const visited = new Set<string>();

	for (const event of sortedEvents) {
		if (visited.has(event.id)) continue;

		// Start a new group with this event
		const group: CalendarEvent[] = [event];
		visited.add(event.id);

		// Find all events that overlap with any event in the group (transitive)
		let i = 0;
		while (i < group.length) {
			const current = group[i];

			// Check remaining events for overlaps
			for (const candidate of sortedEvents) {
				if (!visited.has(candidate.id) && eventsOverlap(current, candidate)) {
					group.push(candidate);
					visited.add(candidate.id);
				}
			}
			i++;
		}

		groups.push(group);
	}

	return groups;
}

/**
 * Assign column positions to overlapping events within a group
 * Uses greedy algorithm to assign each event to the leftmost available column
 * @param group - Array of overlapping events
 * @param layout - Map to store layout results
 */
function assignColumnsToGroup(group: CalendarEvent[], layout: Map<string, EventLayout>): void {
	// Sort group by start time
	const sorted = [...group].sort((a, b) => dateToMinutes(a.start) - dateToMinutes(b.start));

	// Track which columns are occupied and when they end
	const columns: Array<{ event: CalendarEvent; endTime: number }> = [];

	for (const event of sorted) {
		const eventStart = dateToMinutes(event.start);
		const eventEnd = dateToMinutes(event.end);

		// Remove events from columns that have ended before this event starts
		for (let i = columns.length - 1; i >= 0; i--) {
			if (columns[i].endTime <= eventStart) {
				columns.splice(i, 1);
			}
		}

		// Find first free column index
		let columnIndex = 0;
		const occupiedColumns = new Set(
			columns.map((_, idx) => idx).filter((idx) => idx < columns.length)
		);

		while (occupiedColumns.has(columnIndex)) {
			columnIndex++;
		}

		// Place event in this column
		if (columnIndex < columns.length) {
			columns[columnIndex] = { event, endTime: eventEnd };
		} else {
			columns.push({ event, endTime: eventEnd });
		}

		// Store initial layout (will update totalColumns later)
		layout.set(event.id, { columnIndex, totalColumns: 1 });
	}

	// Update all events in group with the final totalColumns count
	const maxColumns = columns.length;
	for (const event of group) {
		const current = layout.get(event.id);
		if (current) {
			layout.set(event.id, { ...current, totalColumns: maxColumns });
		}
	}
}

/**
 * Calculate layout positions for overlapping events
 * Returns column index and total columns for each event
 * @param events - Array of events for a single day
 * @returns Map of event ID to layout information
 */
export function calculateEventLayout(events: CalendarEvent[]): Map<string, EventLayout> {
	if (events.length === 0) return new Map();

	// Sort events by start time, then by duration (longer first)
	const sortedEvents = [...events].sort((a, b) => {
		const aStart = dateToMinutes(a.start);
		const bStart = dateToMinutes(b.start);
		if (aStart !== bStart) return aStart - bStart;

		// If same start time, longer events first (helps minimize columns)
		const aDuration = dateToMinutes(a.end) - aStart;
		const bDuration = dateToMinutes(b.end) - bStart;
		return bDuration - aDuration;
	});

	// Build overlap groups
	const overlapGroups = buildOverlapGroups(sortedEvents);

	// Assign columns within each group
	const layout = new Map<string, EventLayout>();

	for (const group of overlapGroups) {
		if (group.length === 1) {
			// Single event, no overlap - use full width
			layout.set(group[0].id, { columnIndex: 0, totalColumns: 1 });
		} else {
			// Multiple overlapping events - assign columns
			assignColumnsToGroup(group, layout);
		}
	}

	return layout;
}
