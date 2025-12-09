<script lang="ts">
	import CalendarDay from './CalendarDay.svelte';
	import type { CalendarDay as CalendarDayType, CalendarEvent } from '$lib/types/calendar';
	import { isToday, isWeekend, formatDayHeader } from '$lib/utils/calendarUtils';
	import { eventsStore } from '$lib/stores/events';

	interface Props {
		weekDates: Date[];
		isMobile: boolean;
		currentDayIndex: number;
		/**
		 * Callback when an event is clicked
		 */
		onEventClick?: (event: CalendarEvent) => void;
	}

	let { weekDates, isMobile, currentDayIndex, onEventClick }: Props = $props();

	// Filter dates based on viewport - show 1 day on mobile, all 7 on desktop
	let datesToShow = $derived(isMobile ? [weekDates[currentDayIndex]] : weekDates);

	// Subscribe to events store reactively
	let storeState = $derived($eventsStore);

	/**
	 * Build a CalendarDay object from a Date
	 */
	function buildCalendarDay(date: Date): CalendarDayType {
		const dayHeader = formatDayHeader(date);
		// Extract just the day name (e.g., "Mon" from "Mon, Jan 15")
		const dayOfWeek = dayHeader.split(',')[0];

		// Filter events for this specific date from the reactive store state
		// Include events where: startDate <= date <= endDate
		const dateStr = date.toISOString().split('T')[0];
		const events = storeState.events.filter((event) => {
			// Event spans this day if date is between startDate and endDate (inclusive)
			return event.startDate <= dateStr && event.endDate >= dateStr;
		});

		return {
			date,
			isToday: isToday(date),
			isWeekend: isWeekend(date),
			dayOfMonth: date.getDate(),
			dayOfWeek,
			events
		};
	}
</script>

<div class="grid grid-cols-1 md:grid-cols-7 gap-2">
	{#each datesToShow as date (date.toISOString())}
		<CalendarDay day={buildCalendarDay(date)} onEventClick={onEventClick} />
	{/each}
</div>
