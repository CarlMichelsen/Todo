import type { CalendarEvent } from '$lib/types/calendar';
import { isToday, combineDateAndTime, extractDateString } from './calendarUtils';

/**
 * Generate mock events for a given week
 * @param weekStart - Monday of the week
 * @returns Array of CalendarEvent objects
 */
export function generateMockEvents(weekStart: Date): CalendarEvent[] {
	const events: CalendarEvent[] = [];

	// Generate events for each day of the week
	for (let i = 0; i < 7; i++) {
		const date = new Date(weekStart);
		date.setDate(weekStart.getDate() + i);
		const dateStr = extractDateString(date);
		const dayOfWeek = date.getDay();

		// Today's events
		if (isToday(date)) {
			events.push(
				{
					id: `${dateStr}-1`,
					title: 'Team Meeting',
					start: combineDateAndTime(dateStr, '09:00'),
					end: combineDateAndTime(dateStr, '10:30'),
					color: '#3b82f6' // blue
				},
				{
					id: `${dateStr}-2`,
					title: 'Lunch Break',
					start: combineDateAndTime(dateStr, '12:00'),
					end: combineDateAndTime(dateStr, '13:00'),
					color: '#10b981' // green
				},
				{
					id: `${dateStr}-3`,
					title: 'Project Work',
					start: combineDateAndTime(dateStr, '14:00'),
					end: combineDateAndTime(dateStr, '17:00'),
					color: '#ea580c' // orange
				}
			);
		}
		// Monday events
		else if (dayOfWeek === 1) {
			events.push({
				id: `${dateStr}-1`,
				title: 'Weekly Planning',
				start: combineDateAndTime(dateStr, '08:00'),
				end: combineDateAndTime(dateStr, '09:00'),
				color: '#8b5cf6' // purple
			});
		}
		// Wednesday events
		else if (dayOfWeek === 3) {
			events.push(
				{
					id: `${dateStr}-1`,
					title: 'Client Call',
					start: combineDateAndTime(dateStr, '10:00'),
					end: combineDateAndTime(dateStr, '11:00'),
					color: '#f59e0b' // amber
				},
				{
					id: `${dateStr}-2`,
					title: 'Design Review',
					start: combineDateAndTime(dateStr, '15:30'),
					end: combineDateAndTime(dateStr, '16:30'),
					color: '#ec4899' // pink
				}
			);
		}
		// Friday events
		else if (dayOfWeek === 5) {
			events.push({
				id: `${dateStr}-1`,
				title: 'Team Retrospective',
				start: combineDateAndTime(dateStr, '16:00'),
				end: combineDateAndTime(dateStr, '17:30'),
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
		start: combineDateAndTime(extractDateString(tuesday), '09:00'),
		end: combineDateAndTime(extractDateString(thursday), '17:00'),
		color: '#8b5cf6' // purple
	});

	// Workshop spanning 2 days (Wed-Thu)
	const wednesday = new Date(weekStart);
	wednesday.setDate(weekStart.getDate() + 2);

	events.push({
		id: `multi-2`,
		title: 'Leadership Workshop',
		description: 'Two-day leadership training',
		start: combineDateAndTime(extractDateString(wednesday), '13:00'),
		end: combineDateAndTime(extractDateString(thursday), '16:00'),
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
		start: combineDateAndTime(extractDateString(friday), '18:00'),
		end: combineDateAndTime(extractDateString(sunday), '20:00'),
		color: '#ef4444' // red
	});

	return events;
}
