<script lang="ts">
	import type { CalendarEvent } from '$lib/types/calendar';
	import TimeSpanEvent from './TimeSpanEvent.svelte';
	import { calculateEventLayout, isSameDay } from '$lib/utils/calendarUtils';

	interface Props {
		date: Date;
		events: CalendarEvent[];
		/**
		 * Callback when an event is clicked
		 */
		onEventClick?: (event: CalendarEvent) => void;
		/**
		 * Optional callback that runs every time the current time line updates (once per minute)
		 */
		onTimeUpdate?: (currentTime: Date) => void;
	}

	let { date, events, onEventClick, onTimeUpdate }: Props = $props();

	// Generate 24 hours (0-23)
	const hours = Array.from({ length: 24 }, (_, i) => i);

	// Format hour for display (24-hour format without minutes)
	function formatHour(hour: number): string {
		return String(hour).padStart(2, '0');
	}

	// Calculate layout for overlapping events
	const eventLayout = $derived(calculateEventLayout(events));

	// Current time tracking
	let currentTime = $state(new Date());

	// Check if we should show the current time indicator (only if date is today)
	const isToday = $derived(isSameDay(date, currentTime));

	// Calculate position of current time line (in pixels from top)
	const currentTimePosition = $derived.by(() => {
		if (!isToday) return null;
		const hours = currentTime.getHours();
		const minutes = currentTime.getMinutes();
		const totalHours = hours + minutes / 60;
		return totalHours * 38.3; // 38.3px per hour
	});

	// Update current time every minute
	$effect(() => {
		// Initial update
		currentTime = new Date();

		// Calculate milliseconds until next minute boundary
		const now = new Date();
		const msUntilNextMinute = (60 - now.getSeconds()) * 1000 - now.getMilliseconds();

		// Set timeout to sync with minute boundary
		const initialTimeout = setTimeout(() => {
			currentTime = new Date();
			onTimeUpdate?.(currentTime);

			// Then update every minute
			const interval = setInterval(() => {
				currentTime = new Date();
				onTimeUpdate?.(currentTime);
			}, 60000); // 60 seconds

			return () => clearInterval(interval);
		}, msUntilNextMinute);

		// Cleanup
		return () => {
			clearTimeout(initialTimeout);
		};
	});
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
				<TimeSpanEvent {event} currentDate={date} layout={eventLayout.get(event.id)} onclick={onEventClick} />
			{/each}
		</div>

		<!-- Current time indicator (shows above events) -->
		{#if currentTimePosition !== null}
			<div
				class="absolute w-full pointer-events-none z-10"
				style="top: {currentTimePosition}px;"
			>
				<!-- Red circle indicator -->
				<div class="absolute -left-[3px] w-2 h-2 bg-red-500 rounded-full"></div>
				<!-- Red line -->
				<div class="w-full h-[2px] bg-red-500"></div>
			</div>
		{/if}
	</div>
</div>
