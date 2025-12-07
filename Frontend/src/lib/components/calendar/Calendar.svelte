<script lang="ts">
	import CalendarHeader from './CalendarHeader.svelte';
	import CalendarGrid from './CalendarGrid.svelte';
	import { getWeekStart, getWeekDates, addWeeks } from '$lib/utils/calendarUtils';

	// State management
	let currentWeekStart = $state(getWeekStart(new Date()));
	let selectedDate = $state<Date | null>(null);

	// Derived state
	let weekDates = $derived(getWeekDates(currentWeekStart));

	// Event handlers
	function handlePreviousWeek() {
		currentWeekStart = addWeeks(currentWeekStart, -1);
	}

	function handleNextWeek() {
		currentWeekStart = addWeeks(currentWeekStart, 1);
	}

	function handleToday() {
		currentWeekStart = getWeekStart(new Date());
	}

	function handleDayClick(date: Date) {
		selectedDate = date;
		console.log('Day clicked:', date);
		// Future: Open day detail modal or navigate to day view
	}
</script>

<div class="w-full h-full flex flex-col">
	<CalendarHeader
		currentWeekStart={currentWeekStart}
		onPreviousWeek={handlePreviousWeek}
		onNextWeek={handleNextWeek}
		onToday={handleToday}
	/>

	<CalendarGrid weekDates={weekDates} onDayClick={handleDayClick} />
</div>
