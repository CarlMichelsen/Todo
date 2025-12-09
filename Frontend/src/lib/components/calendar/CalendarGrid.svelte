<script lang="ts">
	import CalendarDay from './CalendarDay.svelte';
	import type { CalendarDay as CalendarDayType, CalendarEvent } from '$lib/types/calendar';
	import { isToday, isWeekend, formatDayHeader } from '$lib/utils/calendarUtils';

	interface Props {
		weekDates: Date[];
		isMobile: boolean;
		currentDayIndex: number;
	}

	let { weekDates, isMobile, currentDayIndex }: Props = $props();

	// Filter dates based on viewport - show 1 day on mobile, all 7 on desktop
	let datesToShow = $derived(isMobile ? [weekDates[currentDayIndex]] : weekDates);

	/**
	 * Build a CalendarDay object from a Date with mock events
	 */
	function buildCalendarDay(date: Date): CalendarDayType {
		const dayHeader = formatDayHeader(date);
		// Extract just the day name (e.g., "Mon" from "Mon, Jan 15")
		const dayOfWeek = dayHeader.split(',')[0];

		// Mock events for demonstration
		const mockEvents: CalendarEvent[] = [];

		// Add sample events based on day
		if (isToday(date)) {
			mockEvents.push(
				{
					id: '1',
					title: 'Team Meeting',
					startTime: '09:00',
					endTime: '10:30',
					color: '#3b82f6' // blue
				},
				{
					id: '2',
					title: 'Lunch Break',
					startTime: '12:00',
					endTime: '13:00',
					color: '#10b981' // green
				},
				{
					id: '3',
					title: 'Project Work',
					startTime: '14:00',
					endTime: '17:00',
					color: '#ea580c' // orange
				}
			);
		} else if (date.getDay() === 1) {
			// Monday
			mockEvents.push({
				id: '4',
				title: 'Weekly Planning',
				startTime: '08:00',
				endTime: '09:00',
				color: '#8b5cf6' // purple
			});
		} else if (date.getDay() === 3) {
			// Wednesday
			mockEvents.push(
				{
					id: '5',
					title: 'Client Call',
					startTime: '10:00',
					endTime: '11:00',
					color: '#f59e0b' // amber
				},
				{
					id: '6',
					title: 'Design Review',
					startTime: '15:30',
					endTime: '16:30',
					color: '#ec4899' // pink
				}
			);
		} else if (date.getDay() === 5) {
			// Friday
			mockEvents.push({
				id: '7',
				title: 'Team Retrospective',
				startTime: '16:00',
				endTime: '17:30',
				color: '#06b6d4' // cyan
			});
		}

		return {
			date,
			isToday: isToday(date),
			isWeekend: isWeekend(date),
			dayOfMonth: date.getDate(),
			dayOfWeek,
			events: mockEvents
		};
	}
</script>

<div class="grid grid-cols-1 md:grid-cols-7 gap-2">
	{#each datesToShow as date (date.toISOString())}
		<CalendarDay day={buildCalendarDay(date)} />
	{/each}
</div>
