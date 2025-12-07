<script lang="ts">
	import CalendarDay from './CalendarDay.svelte';
	import type { CalendarDay as CalendarDayType } from '$lib/types/calendar';
	import { isToday, isWeekend, formatDayHeader } from '$lib/utils/calendarUtils';

	interface Props {
		weekDates: Date[];
		onDayClick?: (date: Date) => void;
	}

	let { weekDates, onDayClick }: Props = $props();

	/**
	 * Build a CalendarDay object from a Date
	 */
	function buildCalendarDay(date: Date): CalendarDayType {
		const dayHeader = formatDayHeader(date);
		// Extract just the day name (e.g., "Mon" from "Mon, Jan 15")
		const dayOfWeek = dayHeader.split(',')[0];

		return {
			date,
			isToday: isToday(date),
			isWeekend: isWeekend(date),
			dayOfMonth: date.getDate(),
			dayOfWeek
		};
	}
</script>

<div class="grid grid-cols-1 md:grid-cols-7 gap-2">
	{#each weekDates as date (date.toISOString())}
		<CalendarDay day={buildCalendarDay(date)} onclick={onDayClick} />
	{/each}
</div>
