<script lang="ts">
	import CalendarHeader from './CalendarHeader.svelte';
	import CalendarGrid from './CalendarGrid.svelte';
	import EventModal from './EventModal.svelte';
	import { getWeekStart, getWeekDates, addWeeks } from '$lib/utils/calendarUtils';
	import { eventsStore } from '$lib/stores/events';
	import { calendarsStore } from '$lib/stores/calendars';
	import { setCalendarConfig } from '$lib/stores/calendarConfig';
	import type { CalendarEvent } from '$lib/types/calendar';

	// Props
	interface Props {
		initialWeekStart?: Date;
		onWeekChange?: (weekStart: Date) => void;
		hourHeight?: number;
	}

	let { initialWeekStart, onWeekChange, hourHeight = 40 }: Props = $props();

	// Set calendar configuration for all children components
	setCalendarConfig({ hourHeight});

	// State management
	let currentWeekStart = $state(
		initialWeekStart ? initialWeekStart : getWeekStart(new Date())
	);

	// Mobile: viewport detection (768px = Tailwind md breakpoint)
	let isMobile = $state(false);

	// Mobile: track which day of the week to display (0=Monday, 6=Sunday)
	let currentDayIndex = $state(0);

	// Modal state
	let isEventModalOpen = $state(false);
	let editingEvent = $state<CalendarEvent | undefined>(undefined);

	// Derived state
	let weekDates = $derived(getWeekDates(currentWeekStart));

	// Current viewed date range (edge-based comparison)
	let currentRangeStart = $derived.by(() => {
		if (isMobile) {
			// Mobile: start of current day
			const start = new Date(weekDates[currentDayIndex]);
			start.setHours(0, 0, 0, 0);
			return start.getTime();
		} else {
			// Desktop: start of week (Monday 00:00)
			const start = new Date(currentWeekStart);
			start.setHours(0, 0, 0, 0);
			return start.getTime();
		}
	});

	let currentRangeEnd = $derived.by(() => {
		if (isMobile) {
			// Mobile: end of current day
			const end = new Date(weekDates[currentDayIndex]);
			end.setHours(23, 59, 59, 999);
			return end.getTime();
		} else {
			// Desktop: end of week (Sunday 23:59)
			const end = new Date(currentWeekStart);
			end.setDate(currentWeekStart.getDate() + 6);
			end.setHours(23, 59, 59, 999);
			return end.getTime();
		}
	});

	// Today's date range
	let todayStart = $derived.by(() => {
		const start = new Date();
		start.setHours(0, 0, 0, 0);
		return start.getTime();
	});

	let todayEnd = $derived.by(() => {
		const end = new Date();
		end.setHours(23, 59, 59, 999);
		return end.getTime();
	});

	// Check if current range contains today
	let isViewingToday = $derived(currentRangeStart <= todayStart && currentRangeEnd >= todayEnd);

	// Highlight previous if current range is entirely AFTER today
	let shouldHighlightPrevious = $derived(currentRangeStart > todayEnd);

	// Highlight next if current range is entirely BEFORE today
	let shouldHighlightNext = $derived(currentRangeEnd < todayStart);

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

	// Load events when week changes, but only if calendar is ready
	$effect(() => {
		const calendarState = $calendarsStore;

		// Wait for calendar store to initialize and ensure there's an active calendar
		if (!calendarState.loading && calendarState.activeCalendarId) {
			void eventsStore.setDateRange(currentWeekStart);
		}
	});

	// Notify parent of week changes for URL updates
	$effect(() => {
		if (onWeekChange) {
			onWeekChange(currentWeekStart);
		}
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

	function handleEventClick(event: CalendarEvent) {
		editingEvent = event;
		isEventModalOpen = true;
	}

	function handleAddEvent() {
		editingEvent = undefined; // Clear editing event for create mode
		isEventModalOpen = true;
	}

	// Get initial date for the modal based on current view
	let initialDate = $derived(() => {
		if (isMobile) {
			return weekDates[currentDayIndex].toISOString().split('T')[0];
		}
		return new Date().toISOString().split('T')[0];
	});
</script>

<div class="w-full h-full flex flex-col">
	<CalendarHeader
		currentWeekStart={currentWeekStart}
		currentDayIndex={currentDayIndex}
		weekDates={weekDates}
		isMobile={isMobile}
		isViewingToday={isViewingToday}
		shouldHighlightPrevious={shouldHighlightPrevious}
		shouldHighlightNext={shouldHighlightNext}
		onPreviousWeek={handlePreviousWeek}
		onNextWeek={handleNextWeek}
		onPreviousDay={handlePreviousDay}
		onNextDay={handleNextDay}
		onToday={handleToday}
		onAddEvent={handleAddEvent}
	/>

	<CalendarGrid
		weekDates={weekDates}
		isMobile={isMobile}
		currentDayIndex={currentDayIndex}
		onEventClick={handleEventClick}
	/>

	<!-- Event Modal -->
	<EventModal bind:isOpen={isEventModalOpen} event={editingEvent} initialDate={initialDate()} />
</div>
