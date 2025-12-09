<script lang="ts">
	import type { CalendarEvent, EventLayout } from '$lib/types/calendar';

	interface Props {
		event: CalendarEvent;
		/**
		 * The date this event is being rendered for (YYYY-MM-DD format)
		 */
		currentDate: string;
		/**
		 * Layout information for handling overlaps (optional for backwards compatibility)
		 */
		layout?: EventLayout;
		/**
		 * Callback when event is clicked
		 */
		onclick?: (event: CalendarEvent) => void;
	}

	let { event, currentDate, layout, onclick }: Props = $props();

	// Convert HH:MM time string to pixels from top
	function timeToPixels(time: string): number {
		const [hours, minutes] = time.split(':').map(Number);
		const totalHours = hours + minutes / 60;
		return totalHours * 38.3; // 38.3px per hour
	}

	// Determine if this is a multi-day event
	const isMultiDay = $derived(event.startDate !== event.endDate);
	const startsBeforeToday = $derived(event.startDate < currentDate);
	const endsAfterToday = $derived(event.endDate > currentDate);

	// For multi-day events, adjust the display times
	const displayStartTime = $derived(startsBeforeToday ? '00:00' : event.startTime);
	const displayEndTime = $derived(endsAfterToday ? '23:59' : event.endTime);

	// Calculate position and height
	const topPosition = $derived(timeToPixels(displayStartTime));
	const bottomPosition = $derived(timeToPixels(displayEndTime));
	const height = $derived(bottomPosition - topPosition);

	// Default color if none provided
	const backgroundColor = $derived(event.color || '#ea580c');

	// Calculate horizontal positioning based on layout
	const leftOffset = $derived.by(() => {
		if (!layout || layout.totalColumns === 1) {
			return '0.25rem'; // Same as left-1 (4px)
		}
		const percentage = (layout.columnIndex / layout.totalColumns) * 100;
		return `calc(${percentage}% + 0.25rem)`;
	});

	const width = $derived.by(() => {
		if (!layout || layout.totalColumns === 1) {
			return 'calc(100% - 0.5rem)'; // Same as left-1 right-1 (minus 8px total)
		}
		const percentage = (100 / layout.totalColumns) - 1; // Subtract 1% for gap
		return `${percentage}%`;
	});
</script>

<div
	class="absolute px-2 py-1 overflow-hidden transition-transform hover:scale-x-[1.02] hover:z-10 cursor-pointer shadow-sm"
	class:rounded={!isMultiDay}
	class:rounded-t={isMultiDay && !startsBeforeToday}
	class:rounded-b={isMultiDay && !endsAfterToday}
	style="top: {topPosition}px; height: {height}px; left: {leftOffset}; width: {width}; background-color: {backgroundColor};"
	onclick={() => onclick?.(event)}
	role="button"
	tabindex="0"
	onkeydown={(e) => {
		if (e.key === 'Enter' || e.key === ' ') {
			e.preventDefault();
			onclick?.(event);
		}
	}}
>
	<!-- Continuation indicator at top for events starting before today -->
	{#if startsBeforeToday}
		<div class="absolute top-0 left-0 right-0 h-1 bg-white/40 flex items-center justify-center">
			<div class="text-white text-[10px] font-bold">▲</div>
		</div>
	{/if}

	<div class="text-xs font-semibold text-white truncate" class:mt-2={startsBeforeToday}>
		{event.title}
	</div>
	{#if height > 30}
		<div class="text-xs text-white/90">
			{displayStartTime} - {displayEndTime}
		</div>
	{/if}

	<!-- Continuation indicator at bottom for events ending after today -->
	{#if endsAfterToday}
		<div class="absolute bottom-0 left-0 right-0 h-1 bg-white/40 flex items-center justify-center">
			<div class="text-white text-[10px] font-bold">▼</div>
		</div>
	{/if}
</div>
