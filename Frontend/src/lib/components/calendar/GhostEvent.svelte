<script lang="ts">
	import { getCalendarConfig } from '$lib/stores/calendarConfig';

	interface Props {
		/**
		 * Start time in HH:MM format (e.g., "14:00")
		 */
		startTime: string;
		/**
		 * Duration in minutes (from config)
		 */
		duration: number;
		/**
		 * Current mouse Y position in pixels
		 */
		mouseY: number;
		/**
		 * Callback when ghost event is clicked
		 */
		onclick?: () => void;
	}

	let { startTime, duration, mouseY, onclick }: Props = $props();

	const config = getCalendarConfig();
	const hourHeight = $derived(config.hourHeight);

	// Convert start time to pixels
	function timeToPixels(time: string): number {
		const [hours, minutes] = time.split(':').map(Number);
		const totalHours = hours + minutes / 60;
		return totalHours * hourHeight;
	}

	// Calculate end time
	const endTime = $derived.by(() => {
		const [hours, minutes] = startTime.split(':').map(Number);
		const totalMinutes = hours * 60 + minutes + duration;
		const endHours = Math.floor(totalMinutes / 60);
		const endMinutes = totalMinutes % 60;
		return `${String(endHours).padStart(2, '0')}:${String(endMinutes).padStart(2, '0')}`;
	});

	// Calculate position and height
	const topPosition = $derived(timeToPixels(startTime));
	const bottomPosition = $derived(timeToPixels(endTime));
	const height = $derived(bottomPosition - topPosition);
</script>

<!-- Visible ghost event with clickable area -->
<div
	class="absolute left-1 right-1 px-2 py-1 overflow-hidden rounded border-2 border-dashed border-orange-400 dark:border-orange-500 bg-orange-200/30 dark:bg-orange-500/20 hover:bg-orange-200/50 dark:hover:bg-orange-500/30 transition-colors cursor-pointer pointer-events-auto z-0"
	style="top: {topPosition}px; height: {height}px;"
	onclick={onclick}
	role="button"
	tabindex="0"
	onkeydown={(e) => {
		if (e.key === 'Enter' || e.key === ' ') {
			e.preventDefault();
			onclick?.();
		}
	}}
>
	<div class="text-xs font-semibold text-orange-700 dark:text-orange-300 truncate">
		New Event
	</div>
	{#if height > 20}
		<div class="text-xs text-orange-600 dark:text-orange-400">
			{startTime} - {endTime}
		</div>
	{/if}
</div>
