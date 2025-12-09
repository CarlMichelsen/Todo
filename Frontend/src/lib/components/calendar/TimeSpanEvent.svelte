<script lang="ts">
	import type { CalendarEvent } from '$lib/types/calendar';

	interface Props {
		event: CalendarEvent;
		/**
		 * Callback when event is clicked
		 */
		onclick?: (event: CalendarEvent) => void;
	}

	let { event, onclick }: Props = $props();

	// Convert HH:MM time string to pixels from top
	function timeToPixels(time: string): number {
		const [hours, minutes] = time.split(':').map(Number);
		const totalHours = hours + minutes / 60;
		return totalHours * 38.3; // 38.3px per hour
	}

	// Calculate position and height
	const topPosition = $derived(timeToPixels(event.startTime));
	const bottomPosition = $derived(timeToPixels(event.endTime));
	const height = $derived(bottomPosition - topPosition);

	// Default color if none provided
	const backgroundColor = $derived(event.color || '#ea580c');
</script>

<div
	class="absolute left-1 right-1 rounded px-2 py-1 overflow-hidden transition-transform hover:scale-x-[1.02] hover:z-10 cursor-pointer shadow-sm"
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
	<div class="text-xs font-semibold text-white truncate">
		{event.title}
	</div>
	{#if height > 30}
		<div class="text-xs text-white/90">
			{event.startTime} - {event.endTime}
		</div>
	{/if}
</div>
