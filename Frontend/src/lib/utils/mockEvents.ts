import type { CalendarEvent } from '$lib/types/calendar';
import { isToday } from './calendarUtils';

/**
 * Generate mock events for a given week
 * @param weekStart - Monday of the week
 * @returns Array of CalendarEvent objects
 */
export function generateMockEvents(weekStart: Date): CalendarEvent[] {
	const events: CalendarEvent[] = [];

	// Helper to format date as YYYY-MM-DD
	const formatDate = (date: Date): string => {
		return date.toISOString().split('T')[0];
	};

	// Generate events for each day of the week
	for (let i = 0; i < 7; i++) {
		const date = new Date(weekStart);
		date.setDate(weekStart.getDate() + i);
		const dateStr = formatDate(date);
		const dayOfWeek = date.getDay();

		// Today's events
		if (isToday(date)) {
			events.push(
				{
					id: `${dateStr}-1`,
					title: 'Team Meeting',
					startDate: dateStr,
					endDate: dateStr,
					startTime: '09:00',
					endTime: '10:30',
					color: '#3b82f6' // blue
				},
				{
					id: `${dateStr}-2`,
					title: 'Lunch Break',
					startDate: dateStr,
					endDate: dateStr,
					startTime: '12:00',
					endTime: '13:00',
					color: '#10b981' // green
				},
				{
					id: `${dateStr}-3`,
					title: 'Project Work',
					startDate: dateStr,
					endDate: dateStr,
					startTime: '14:00',
					endTime: '17:00',
					color: '#ea580c' // orange
				}
			);
		}
		// Monday events
		else if (dayOfWeek === 1) {
			events.push({
				id: `${dateStr}-1`,
				title: 'Weekly Planning',
				startDate: dateStr,
				endDate: dateStr,
				startTime: '08:00',
				endTime: '09:00',
				color: '#8b5cf6' // purple
			});
		}
		// Wednesday events
		else if (dayOfWeek === 3) {
			events.push(
				{
					id: `${dateStr}-1`,
					title: 'Client Call',
					startDate: dateStr,
					endDate: dateStr,
					startTime: '10:00',
					endTime: '11:00',
					color: '#f59e0b' // amber
				},
				{
					id: `${dateStr}-2`,
					title: 'Design Review',
					startDate: dateStr,
					endDate: dateStr,
					startTime: '15:30',
					endTime: '16:30',
					color: '#ec4899' // pink
				}
			);
		}
		// Friday events
		else if (dayOfWeek === 5) {
			events.push({
				id: `${dateStr}-1`,
				title: 'Team Retrospective',
				startDate: dateStr,
				endDate: dateStr,
				startTime: '16:00',
				endTime: '17:30',
				color: '#06b6d4' // cyan
			});
		}
	}

	// Add some multi-day events for testing
	// Conference spanning 3 days (Tue-Thu)
	const tuesday = new Date(weekStart);
	tuesday.setDate(weekStart.getDate() + 1);
	const thursday = new Date(weekStart);
	thursday.setDate(weekStart.getDate() + 3);

	events.push({
		id: `multi-1`,
		title: 'Tech Conference',
		description: 'Annual technology conference',
		startDate: formatDate(tuesday),
		endDate: formatDate(thursday),
		startTime: '09:00',
		endTime: '17:00',
		color: '#8b5cf6' // purple
	});

	// Workshop spanning 2 days (Wed-Thu)
	const wednesday = new Date(weekStart);
	wednesday.setDate(weekStart.getDate() + 2);

	events.push({
		id: `multi-2`,
		title: 'Leadership Workshop',
		description: 'Two-day leadership training',
		startDate: formatDate(wednesday),
		endDate: formatDate(thursday),
		startTime: '13:00',
		endTime: '16:00',
		color: '#ec4899' // pink
	});

	// Weekend project spanning Fri-Sun
	const friday = new Date(weekStart);
	friday.setDate(weekStart.getDate() + 4);
	const sunday = new Date(weekStart);
	sunday.setDate(weekStart.getDate() + 6);

	events.push({
		id: `multi-3`,
		title: 'Hackathon',
		description: 'Weekend coding competition',
		startDate: formatDate(friday),
		endDate: formatDate(sunday),
		startTime: '18:00',
		endTime: '20:00',
		color: '#ef4444' // red
	});

	return events;
}
