<script lang="ts">
	import CalendarHeader from './CalendarHeader.svelte';
	import CalendarGrid from './CalendarGrid.svelte';
	import { getWeekStart, getWeekDates, addWeeks } from '$lib/utils/calendarUtils';

	// State management
	let currentWeekStart = $state(getWeekStart(new Date()));

	// Mobile: viewport detection (768px = Tailwind md breakpoint)
	let isMobile = $state(false);

	// Mobile: track which day of the week to display (0=Monday, 6=Sunday)
	let currentDayIndex = $state(0);

	// Derived state
	let weekDates = $derived(getWeekDates(currentWeekStart));

	// Initialize viewport detection and set currentDayIndex to today
	$effect(() => {
		// Set initial mobile state
		isMobile = window.innerWidth < 768;

		// Set currentDayIndex to today's position in week
		const today = new Date();
		const dayOfWeek = today.getDay();
		// Convert Sunday(0) to 6, Mon(1) to 0, Tue(2) to 1, etc.
		currentDayIndex = dayOfWeek === 0 ? 6 : dayOfWeek - 1;

		// Track resize
		const handleResize = () => {
			isMobile = window.innerWidth < 768;
		};

		window.addEventListener('resize', handleResize);
		return () => window.removeEventListener('resize', handleResize);
	});

	// Week navigation handlers (desktop)
	function handlePreviousWeek() {
		currentWeekStart = addWeeks(currentWeekStart, -1);
	}

	function handleNextWeek() {
		currentWeekStart = addWeeks(currentWeekStart, 1);
	}

	// Day navigation handlers (mobile)
	function handlePreviousDay() {
		if (currentDayIndex > 0) {
			// Go to previous day in same week
			currentDayIndex--;
		} else {
			// Currently on Monday, wrap to previous week's Sunday
			currentWeekStart = addWeeks(currentWeekStart, -1);
			currentDayIndex = 6;
		}
	}

	function handleNextDay() {
		if (currentDayIndex < 6) {
			// Go to next day in same week
			currentDayIndex++;
		} else {
			// Currently on Sunday, wrap to next week's Monday
			currentWeekStart = addWeeks(currentWeekStart, 1);
			currentDayIndex = 0;
		}
	}

	function handleToday() {
		const today = new Date();
		currentWeekStart = getWeekStart(today);

		// Set currentDayIndex to today's position in week
		const dayOfWeek = today.getDay();
		currentDayIndex = dayOfWeek === 0 ? 6 : dayOfWeek - 1;
	}
</script>

<div class="w-full h-full flex flex-col">
	<CalendarHeader
		currentWeekStart={currentWeekStart}
		currentDayIndex={currentDayIndex}
		weekDates={weekDates}
		isMobile={isMobile}
		onPreviousWeek={handlePreviousWeek}
		onNextWeek={handleNextWeek}
		onPreviousDay={handlePreviousDay}
		onNextDay={handleNextDay}
		onToday={handleToday}
	/>

	<CalendarGrid
		weekDates={weekDates}
		isMobile={isMobile}
		currentDayIndex={currentDayIndex}
	/>
</div>
