<script lang="ts">
	import type { CalendarEvent } from '$lib/types/calendar';

	interface Props {
		event: CalendarEvent;
		/**
		 * The date this event is being rendered for (YYYY-MM-DD format)
		 */
		currentDate: string;
		/**
		 * Callback when event is clicked
		 */
		onclick?: (event: CalendarEvent) => void;
	}

	let { event, currentDate, onclick }: Props = $props();

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
</script>

<div
	class="absolute left-1 right-1 px-2 py-1 overflow-hidden transition-transform hover:scale-x-[1.02] hover:z-10 cursor-pointer shadow-sm"
	class:rounded={!isMultiDay}
	class:rounded-t={isMultiDay && !startsBeforeToday}
	class:rounded-b={isMultiDay && !endsAfterToday}
	style="top: {topPosition}px; height: {height}px; background-color: {backgroundColor};"
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
