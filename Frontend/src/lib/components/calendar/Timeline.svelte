<script lang="ts">
	import type { CalendarEvent } from '$lib/types/calendar';
	import TimeSpanEvent from './TimeSpanEvent.svelte';

	interface Props {
		date: Date;
		events: CalendarEvent[];
		/**
		 * Callback when an event is clicked
		 */
		onEventClick?: (event: CalendarEvent) => void;
	}

	let { date, events, onEventClick }: Props = $props();

	// Generate 24 hours (0-23)
	const hours = Array.from({ length: 24 }, (_, i) => i);

	// Format hour for display (24-hour format without minutes)
	function formatHour(hour: number): string {
		return String(hour).padStart(2, '0');
	}
</script>

<div class="relative h-full flex text-xs">
	<!-- Left: Time labels (40px wide) -->
	<div class="w-[22px] flex-shrink-0">
		{#each hours as hour}
			<div class="h-[38.3px] flex items-start justify-end pr-2 text-gray-500 dark:text-gray-500">
				{formatHour(hour)}
			</div>
		{/each}
	</div>

	<!-- Right: Timeline grid + events -->
	<div class="relative flex-1">
		<!-- Background grid lines -->
		<div class="absolute inset-0">
			{#each hours as hour}
				<!-- Full hour line -->
				<div
					class="absolute w-full border-t border-gray-300 dark:border-gray-700"
					style="top: {hour * 38.3}px;"
				></div>
				<!-- Half-hour line (lighter) -->
				{#if hour < 23}
					<div
						class="absolute w-full border-t border-gray-200 dark:border-gray-800"
						style="top: {hour * 38.3 + 19.15}px;"
					></div>
				{/if}
			{/each}
		</div>

		<!-- Events layer -->
		<div class="absolute inset-0">
			{#each events as event (event.id)}
				<TimeSpanEvent {event} onclick={onEventClick} />
			{/each}
		</div>
	</div>
</div>
