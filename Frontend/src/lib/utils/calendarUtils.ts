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
