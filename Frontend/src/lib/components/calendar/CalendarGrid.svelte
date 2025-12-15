<script lang="ts">
	import CalendarDay from './CalendarDay.svelte';
	import type { CalendarDay as CalendarDayType, CalendarEvent } from '$lib/types/calendar';
	import { isToday, isWeekend, formatDayHeader, eventOccursOnDate } from '$lib/utils/calendarUtils';
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
		// Include events where the event occurs on this date (ignoring time)
		const events = storeState.events.filter((event) => {
			return eventOccursOnDate(event.start, event.end, date);
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

{#if storeState.error}
	<div class="flex items-center justify-center h-full min-h-[400px]">
		<p class="text-red-500 dark:text-red-400">{storeState.error}</p>
	</div>
{:else}
	<div
		class="grid grid-cols-1 md:grid-cols-7 gap-2 transition-all duration-200 {storeState.loading
			? 'blur-sm opacity-60 pointer-events-none'
			: ''}"
	>
		{#each datesToShow as date (date.toISOString())}
			<CalendarDay day={buildCalendarDay(date)} onEventClick={onEventClick} />
		{/each}
	</div>
{/if}
