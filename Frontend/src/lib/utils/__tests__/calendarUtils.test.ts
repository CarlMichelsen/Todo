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
			startDate: '2025-01-15',
			endDate: '2025-01-15',
			startTime,
			endTime
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
		const createEvent = (
			id: string,
			startTime: string,
			endTime: string,
			startDate = '2025-01-15',
			endDate = '2025-01-15'
		): CalendarEvent => ({
			id,
			title: `Event ${id}`,
			startDate,
			endDate,
			startTime,
			endTime
		});

		it('should return empty map for no events', () => {
			const layout = calculateEventLayout([]);
			expect(layout.size).toBe(0);
		});

		it('should assign single column for single event', () => {
			const events = [createEvent('1', '09:00', '10:00')];
			const layout = calculateEventLayout(events);
			expect(layout.get('1')).toEqual({ columnIndex: 0, totalColumns: 1 });
		});

		it('should assign single column for non-overlapping events', () => {
			const events = [
				createEvent('1', '09:00', '10:00'),
				createEvent('2', '10:00', '11:00'),
				createEvent('3', '11:00', '12:00')
			];
			const layout = calculateEventLayout(events);
			expect(layout.get('1')).toEqual({ columnIndex: 0, totalColumns: 1 });
			expect(layout.get('2')).toEqual({ columnIndex: 0, totalColumns: 1 });
			expect(layout.get('3')).toEqual({ columnIndex: 0, totalColumns: 1 });
		});

		it('should assign two columns for two overlapping events', () => {
			const events = [
				createEvent('1', '09:00', '11:00'),
				createEvent('2', '10:00', '12:00')
			];
			const layout = calculateEventLayout(events);
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
			const layout = calculateEventLayout(events);
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
			const layout = calculateEventLayout(events);
			// All three events are in same overlap group (1 overlaps 2, 2 overlaps 3)
			// They should all have the same totalColumns
			expect(layout.get('1')?.totalColumns).toBe(layout.get('2')?.totalColumns);
			expect(layout.get('2')?.totalColumns).toBe(layout.get('3')?.totalColumns);
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
			const layout = calculateEventLayout(events);

			// Events 1, 2, 3, 4 form one overlap group (transitive closure)
			// 1 overlaps 2 and 3, 3 overlaps 4 -> all in same group
			const totalCols = layout.get('1')?.totalColumns;
			expect(layout.get('2')?.totalColumns).toBe(totalCols);
			expect(layout.get('3')?.totalColumns).toBe(totalCols);
			expect(layout.get('4')?.totalColumns).toBe(totalCols);

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
			const layout = calculateEventLayout(events);
			// Longer event should get column 0 (sorted first)
			expect(layout.get('long')?.columnIndex).toBe(0);
			expect(layout.get('short')?.columnIndex).toBe(1);
		});
	});
});
