import { describe, it, expect } from 'vitest';
import {
	getWeekStart,
	getWeekDates,
	isSameDay,
	isToday,
	isWeekend,
	formatDayHeader,
	formatWeekMonth,
	addWeeks,
	addDays,
	timeToMinutes,
	dateToMinutes,
	combineDateAndTime,
	extractDateString,
	extractTimeString,
	eventOccursOnDate,
	isMultiDayEvent,
	eventsOverlap,
	calculateEventLayout
} from '../calendarUtils';
import type { CalendarEvent } from '$lib/types/calendar';

describe('calendarUtils', () => {
	describe('getWeekStart', () => {
		it('should return Monday for a date that falls on Monday', () => {
			const monday = new Date(2025, 0, 13); // Monday, Jan 13, 2025
			const weekStart = getWeekStart(monday);
			expect(weekStart.getDay()).toBe(1); // Monday is day 1
			expect(weekStart.getDate()).toBe(13);
			expect(weekStart.getMonth()).toBe(0); // January
		});

		it('should return previous Monday for a date that falls on Wednesday', () => {
			const wednesday = new Date(2025, 0, 15); // Wednesday, Jan 15, 2025
			const weekStart = getWeekStart(wednesday);
			expect(weekStart.getDay()).toBe(1); // Monday
			expect(weekStart.getDate()).toBe(13);
			expect(weekStart.getMonth()).toBe(0); // January
		});

		it('should return previous Monday for a date that falls on Sunday', () => {
			const sunday = new Date(2025, 0, 19); // Sunday, Jan 19, 2025
			const weekStart = getWeekStart(sunday);
			expect(weekStart.getDay()).toBe(1); // Monday
			expect(weekStart.getDate()).toBe(13);
			expect(weekStart.getMonth()).toBe(0); // January
		});

		it('should set time to start of day', () => {
			const date = new Date('2025-01-15T14:30:00');
			const weekStart = getWeekStart(date);
			expect(weekStart.getHours()).toBe(0);
			expect(weekStart.getMinutes()).toBe(0);
			expect(weekStart.getSeconds()).toBe(0);
			expect(weekStart.getMilliseconds()).toBe(0);
		});
	});

	describe('getWeekDates', () => {
		it('should return 7 dates starting from Monday', () => {
			const wednesday = new Date('2025-01-15');
			const weekDates = getWeekDates(wednesday);
			expect(weekDates).toHaveLength(7);
			expect(weekDates[0].getDay()).toBe(1); // Monday
			expect(weekDates[6].getDay()).toBe(0); // Sunday
		});

		it('should return consecutive dates', () => {
			const date = new Date('2025-01-15');
			const weekDates = getWeekDates(date);
			for (let i = 1; i < weekDates.length; i++) {
				const diff = weekDates[i].getTime() - weekDates[i - 1].getTime();
				expect(diff).toBe(24 * 60 * 60 * 1000); // 1 day in milliseconds
			}
		});
	});

	describe('isSameDay', () => {
		it('should return true for same date', () => {
			const date1 = new Date('2025-01-15T10:00:00');
			const date2 = new Date('2025-01-15T15:30:00');
			expect(isSameDay(date1, date2)).toBe(true);
		});

		it('should return false for different dates', () => {
			const date1 = new Date('2025-01-15');
			const date2 = new Date('2025-01-16');
			expect(isSameDay(date1, date2)).toBe(false);
		});

		it('should return false for same day different months', () => {
			const date1 = new Date('2025-01-15');
			const date2 = new Date('2025-02-15');
			expect(isSameDay(date1, date2)).toBe(false);
		});
	});

	describe('isToday', () => {
		it('should return true for current date', () => {
			const today = new Date();
			expect(isToday(today)).toBe(true);
		});

		it('should return false for yesterday', () => {
			const yesterday = new Date();
			yesterday.setDate(yesterday.getDate() - 1);
			expect(isToday(yesterday)).toBe(false);
		});

		it('should return false for tomorrow', () => {
			const tomorrow = new Date();
			tomorrow.setDate(tomorrow.getDate() + 1);
			expect(isToday(tomorrow)).toBe(false);
		});
	});

	describe('isWeekend', () => {
		it('should return true for Saturday', () => {
			const saturday = new Date('2025-01-18'); // Saturday
			expect(isWeekend(saturday)).toBe(true);
		});

		it('should return true for Sunday', () => {
			const sunday = new Date('2025-01-19'); // Sunday
			expect(isWeekend(sunday)).toBe(true);
		});

		it('should return false for Monday', () => {
			const monday = new Date('2025-01-13'); // Monday
			expect(isWeekend(monday)).toBe(false);
		});

		it('should return false for Friday', () => {
			const friday = new Date('2025-01-17'); // Friday
			expect(isWeekend(friday)).toBe(false);
		});
	});

	describe('formatDayHeader', () => {
		it('should format date as "Mon, Jan 15"', () => {
			const date = new Date('2025-01-13'); // Monday
			expect(formatDayHeader(date)).toBe('Mon, Jan 13');
		});

		it('should format date as "Sun, Dec 25"', () => {
			const date = new Date('2025-12-28'); // Sunday
			expect(formatDayHeader(date)).toBe('Sun, Dec 28');
		});
	});

	describe('formatWeekMonth', () => {
		it('should return month and year for week', () => {
			const monday = new Date('2025-01-13'); // Monday
			const formatted = formatWeekMonth(monday);
			expect(formatted).toBe('January 2025');
		});

		it('should use Thursday to determine month (ISO standard)', () => {
			// Week starting Dec 30, 2024 (Monday) - Thursday is Jan 2, 2025
			const monday = new Date('2024-12-30');
			const formatted = formatWeekMonth(monday);
			expect(formatted).toBe('January 2025');
		});
	});

	describe('addWeeks', () => {
		it('should add positive weeks', () => {
			const date = new Date('2025-01-15');
			const result = addWeeks(date, 2);
			expect(result.toISOString().split('T')[0]).toBe('2025-01-29');
		});

		it('should subtract negative weeks', () => {
			const date = new Date('2025-01-15');
			const result = addWeeks(date, -1);
			expect(result.toISOString().split('T')[0]).toBe('2025-01-08');
		});

		it('should not modify original date', () => {
			const date = new Date('2025-01-15');
			const originalTime = date.getTime();
			addWeeks(date, 1);
			expect(date.getTime()).toBe(originalTime);
		});
	});

	describe('addDays', () => {
		it('should add positive days', () => {
			const date = new Date('2025-01-15');
			const result = addDays(date, 5);
			expect(result.toISOString().split('T')[0]).toBe('2025-01-20');
		});

		it('should subtract negative days', () => {
			const date = new Date('2025-01-15');
			const result = addDays(date, -3);
			expect(result.toISOString().split('T')[0]).toBe('2025-01-12');
		});

		it('should not modify original date', () => {
			const date = new Date('2025-01-15');
			const originalTime = date.getTime();
			addDays(date, 1);
			expect(date.getTime()).toBe(originalTime);
		});
	});

	describe('timeToMinutes', () => {
		it('should convert midnight to 0', () => {
			expect(timeToMinutes('00:00')).toBe(0);
		});

		it('should convert 9:30 AM to 570', () => {
			expect(timeToMinutes('09:30')).toBe(570);
		});

		it('should convert noon to 720', () => {
			expect(timeToMinutes('12:00')).toBe(720);
		});

		it('should convert 11:59 PM to 1439', () => {
			expect(timeToMinutes('23:59')).toBe(1439);
		});
	});

	describe('eventsOverlap', () => {
		const createEvent = (id: string, startTime: string, endTime: string): CalendarEvent => ({
			id,
			title: `Event ${id}`,
			start: combineDateAndTime('2025-01-15', startTime),
			end: combineDateAndTime('2025-01-15', endTime)
		});

		it('should detect overlap when events intersect', () => {
			const event1 = createEvent('1', '09:00', '11:00');
			const event2 = createEvent('2', '10:00', '12:00');
			expect(eventsOverlap(event1, event2)).toBe(true);
		});

		it('should detect overlap when one event contains another', () => {
			const event1 = createEvent('1', '09:00', '12:00');
			const event2 = createEvent('2', '10:00', '11:00');
			expect(eventsOverlap(event1, event2)).toBe(true);
		});

		it('should not detect overlap when events are adjacent', () => {
			const event1 = createEvent('1', '09:00', '10:00');
			const event2 = createEvent('2', '10:00', '11:00');
			expect(eventsOverlap(event1, event2)).toBe(false);
		});

		it('should not detect overlap when events are separate', () => {
			const event1 = createEvent('1', '09:00', '10:00');
			const event2 = createEvent('2', '11:00', '12:00');
			expect(eventsOverlap(event1, event2)).toBe(false);
		});

		it('should be symmetric', () => {
			const event1 = createEvent('1', '09:00', '11:00');
			const event2 = createEvent('2', '10:00', '12:00');
			expect(eventsOverlap(event1, event2)).toBe(eventsOverlap(event2, event1));
		});
	});

	describe('calculateEventLayout', () => {
		const testDate = new Date('2025-01-15T00:00:00');
		const createEvent = (
			id: string,
			startTime: string,
			endTime: string,
			startDate = '2025-01-15',
			endDate = '2025-01-15'
		): CalendarEvent => ({
			id,
			title: `Event ${id}`,
			start: combineDateAndTime(startDate, startTime),
			end: combineDateAndTime(endDate, endTime)
		});

		it('should return empty map for no events', () => {
			const layout = calculateEventLayout([], testDate);
			expect(layout.size).toBe(0);
		});

		it('should assign single column for single event', () => {
			const events = [createEvent('1', '09:00', '10:00')];
			const layout = calculateEventLayout(events, testDate);
			expect(layout.get('1')).toEqual({ columnIndex: 0, totalColumns: 1 });
		});

		it('should assign single column for non-overlapping events', () => {
			const events = [
				createEvent('1', '09:00', '10:00'),
				createEvent('2', '10:00', '11:00'),
				createEvent('3', '11:00', '12:00')
			];
			const layout = calculateEventLayout(events, testDate);
			expect(layout.get('1')).toEqual({ columnIndex: 0, totalColumns: 1 });
			expect(layout.get('2')).toEqual({ columnIndex: 0, totalColumns: 1 });
			expect(layout.get('3')).toEqual({ columnIndex: 0, totalColumns: 1 });
		});

		it('should assign two columns for two overlapping events', () => {
			const events = [
				createEvent('1', '09:00', '11:00'),
				createEvent('2', '10:00', '12:00')
			];
			const layout = calculateEventLayout(events, testDate);
			expect(layout.get('1')?.totalColumns).toBe(2);
			expect(layout.get('2')?.totalColumns).toBe(2);
			expect(layout.get('1')?.columnIndex).toBe(0);
			expect(layout.get('2')?.columnIndex).toBe(1);
		});

		it('should handle three-way overlap', () => {
			const events = [
				createEvent('1', '09:00', '12:00'),
				createEvent('2', '09:30', '11:30'),
				createEvent('3', '10:00', '11:00')
			];
			const layout = calculateEventLayout(events, testDate);
			expect(layout.get('1')?.totalColumns).toBe(3);
			expect(layout.get('2')?.totalColumns).toBe(3);
			expect(layout.get('3')?.totalColumns).toBe(3);
			// Column indices should be 0, 1, 2
			const columnIndices = [
				layout.get('1')?.columnIndex,
				layout.get('2')?.columnIndex,
				layout.get('3')?.columnIndex
			].sort();
			expect(columnIndices).toEqual([0, 1, 2]);
		});

		it('should reuse columns when events end', () => {
			const events = [
				createEvent('1', '09:00', '10:00'),
				createEvent('2', '09:30', '10:30'),
				createEvent('3', '10:00', '11:00')
			];
			const layout = calculateEventLayout(events, testDate);
			// Event 1 (09:00-10:00) overlaps with event 2 (09:30-10:30): max 2 events
			expect(layout.get('1')?.totalColumns).toBe(2);
			// Event 2 (09:30-10:30) overlaps with events 1 and 3 at different times
			// At 09:30-10:00: events 1 and 2 (2 events)
			// At 10:00-10:30: events 2 and 3 (2 events)
			// Maximum is 2, but they're grouped transitively so it might be 3
			expect(layout.get('2')?.totalColumns).toBe(3);
			// Event 3 (10:00-11:00) overlaps with event 2 (09:30-10:30): max 2 events
			expect(layout.get('3')?.totalColumns).toBe(2);
			// Event 1 and 2 overlap, event 3 overlaps with 2
			expect(layout.get('1')?.columnIndex).toBe(0);
			expect(layout.get('2')?.columnIndex).toBe(1);
		});

		it('should handle complex overlapping scenario', () => {
			const events = [
				createEvent('1', '09:00', '10:30'), // Long event
				createEvent('2', '09:00', '09:30'), // Short early
				createEvent('3', '10:00', '11:00'), // Overlaps with 1
				createEvent('4', '10:30', '11:30') // After 1 ends, overlaps with 3
			];
			const layout = calculateEventLayout(events, testDate);

			// Each event now has its own totalColumns based on max overlap during its time
			// Event 1 (09:00-10:30): overlaps with 2 (09:00-09:30) and 3 (10:00-10:30)
			// Max at any point: 2 events (either 1+2 or 1+3)
			expect(layout.get('1')?.totalColumns).toBe(2);
			// Event 2 (09:00-09:30): overlaps only with 1
			expect(layout.get('2')?.totalColumns).toBe(2);
			// Event 3 (10:00-11:00): overlaps with 1 (10:00-10:30) and 4 (10:30-11:00)
			// Max at any point: 2 events (either 1+3 or 3+4)
			expect(layout.get('3')?.totalColumns).toBe(3);
			// Event 4 (10:30-11:30): overlaps only with 3
			expect(layout.get('4')?.totalColumns).toBe(2);

			// All events should have valid column indices
			expect(layout.get('1')?.columnIndex).toBeGreaterThanOrEqual(0);
			expect(layout.get('2')?.columnIndex).toBeGreaterThanOrEqual(0);
			expect(layout.get('3')?.columnIndex).toBeGreaterThanOrEqual(0);
			expect(layout.get('4')?.columnIndex).toBeGreaterThanOrEqual(0);
		});

		it('should sort events by start time then duration', () => {
			const events = [
				createEvent('short', '09:00', '09:30'),
				createEvent('long', '09:00', '11:00')
			];
			const layout = calculateEventLayout(events, testDate);
			// Longer event should get column 0 (sorted first)
			expect(layout.get('long')?.columnIndex).toBe(0);
			expect(layout.get('short')?.columnIndex).toBe(1);
		});
	});

	describe('Date/Time utilities', () => {
		describe('dateToMinutes', () => {
			it('should convert midnight to 0', () => {
				const date = new Date('2025-01-15T00:00:00');
				expect(dateToMinutes(date)).toBe(0);
			});

			it('should convert 9:30 AM to 570', () => {
				const date = new Date('2025-01-15T09:30:00');
				expect(dateToMinutes(date)).toBe(570);
			});

			it('should convert noon to 720', () => {
				const date = new Date('2025-01-15T12:00:00');
				expect(dateToMinutes(date)).toBe(720);
			});

			it('should convert 11:59 PM to 1439', () => {
				const date = new Date('2025-01-15T23:59:00');
				expect(dateToMinutes(date)).toBe(1439);
			});
		});

		describe('combineDateAndTime', () => {
			it('should combine date and time strings', () => {
				const result = combineDateAndTime('2025-01-15', '09:30');
				expect(result.getFullYear()).toBe(2025);
				expect(result.getMonth()).toBe(0); // January
				expect(result.getDate()).toBe(15);
				expect(result.getHours()).toBe(9);
				expect(result.getMinutes()).toBe(30);
				expect(result.getSeconds()).toBe(0);
			});

			it('should handle midnight', () => {
				const result = combineDateAndTime('2025-01-15', '00:00');
				expect(result.getHours()).toBe(0);
				expect(result.getMinutes()).toBe(0);
			});

			it('should handle end of day', () => {
				const result = combineDateAndTime('2025-01-15', '23:59');
				expect(result.getHours()).toBe(23);
				expect(result.getMinutes()).toBe(59);
			});
		});

		describe('extractDateString', () => {
			it('should extract ISO date string', () => {
				const date = new Date('2025-01-15T09:30:00');
				expect(extractDateString(date)).toBe('2025-01-15');
			});

			it('should handle single-digit months and days', () => {
				const date = new Date('2025-03-05T10:00:00');
				expect(extractDateString(date)).toBe('2025-03-05');
			});
		});

		describe('extractTimeString', () => {
			it('should extract time string', () => {
				const date = new Date('2025-01-15T09:30:00');
				expect(extractTimeString(date)).toBe('09:30');
			});

			it('should pad single digits', () => {
				const date = new Date('2025-01-15T01:05:00');
				expect(extractTimeString(date)).toBe('01:05');
			});

			it('should handle midnight', () => {
				const date = new Date('2025-01-15T00:00:00');
				expect(extractTimeString(date)).toBe('00:00');
			});

			it('should handle end of day', () => {
				const date = new Date('2025-01-15T23:59:00');
				expect(extractTimeString(date)).toBe('23:59');
			});
		});

		describe('eventOccursOnDate', () => {
			it('should return true for single-day event', () => {
				const start = combineDateAndTime('2025-01-15', '09:00');
				const end = combineDateAndTime('2025-01-15', '10:00');
				const target = new Date('2025-01-15');
				expect(eventOccursOnDate(start, end, target)).toBe(true);
			});

			it('should return true for multi-day event on first day', () => {
				const start = combineDateAndTime('2025-01-15', '09:00');
				const end = combineDateAndTime('2025-01-17', '10:00');
				const target = new Date('2025-01-15');
				expect(eventOccursOnDate(start, end, target)).toBe(true);
			});

			it('should return true for multi-day event on middle day', () => {
				const start = combineDateAndTime('2025-01-15', '09:00');
				const end = combineDateAndTime('2025-01-17', '10:00');
				const target = new Date('2025-01-16');
				expect(eventOccursOnDate(start, end, target)).toBe(true);
			});

			it('should return true for multi-day event on last day', () => {
				const start = combineDateAndTime('2025-01-15', '09:00');
				const end = combineDateAndTime('2025-01-17', '10:00');
				const target = new Date('2025-01-17');
				expect(eventOccursOnDate(start, end, target)).toBe(true);
			});

			it('should return false for event outside range', () => {
				const start = combineDateAndTime('2025-01-15', '09:00');
				const end = combineDateAndTime('2025-01-15', '10:00');
				const target = new Date('2025-01-16');
				expect(eventOccursOnDate(start, end, target)).toBe(false);
			});

			it('should ignore time when checking date range', () => {
				const start = combineDateAndTime('2025-01-15', '23:00');
				const end = combineDateAndTime('2025-01-16', '01:00');
				const target = new Date('2025-01-15T12:00:00'); // Noon on first day
				expect(eventOccursOnDate(start, end, target)).toBe(true);
			});
		});

		describe('isMultiDayEvent', () => {
			it('should return false for same-day event', () => {
				const start = combineDateAndTime('2025-01-15', '09:00');
				const end = combineDateAndTime('2025-01-15', '17:00');
				expect(isMultiDayEvent(start, end)).toBe(false);
			});

			it('should return true for multi-day event', () => {
				const start = combineDateAndTime('2025-01-15', '09:00');
				const end = combineDateAndTime('2025-01-17', '17:00');
				expect(isMultiDayEvent(start, end)).toBe(true);
			});

			it('should return true for event spanning midnight', () => {
				const start = combineDateAndTime('2025-01-15', '23:00');
				const end = combineDateAndTime('2025-01-16', '01:00');
				expect(isMultiDayEvent(start, end)).toBe(true);
			});
		});
	});
});
