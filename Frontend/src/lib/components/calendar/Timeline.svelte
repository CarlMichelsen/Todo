<script lang="ts">
	import type { CalendarEvent } from '$lib/types/calendar';
	import TimeSpanEvent from './TimeSpanEvent.svelte';
	import GhostEvent from './GhostEvent.svelte';
	import {
		calculateEventLayout,
		isSameDay,
		pixelsToTime,
		roundTimeToInterval,
		extractDateString
	} from '$lib/utils/calendarUtils';
	import { getCalendarConfig } from '$lib/stores/calendarConfig';

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
		/**
		 * Callback when ghost event is clicked
		 */
		onGhostEventClick?: (date: string, startTime: string, endTime: string) => void;
	}

	let { date, events, onEventClick, onTimeUpdate, onGhostEventClick }: Props = $props();

	// Get calendar configuration
	const config = getCalendarConfig();
	const hourHeight = $derived(config.hourHeight);
	const halfHourHeight = $derived(hourHeight / 2);
	const ghostDuration = $derived(config.ghostEventDuration);
	const snapInterval = $derived(config.ghostEventSnapInterval);

	// Ghost event state
	let isHovering = $state(false);
	let mouseY = $state(0);
	let isMobile = $state(false);
	let hoveredEventId = $state<string | null>(null);

	// Detect mobile on mount
	$effect(() => {
		isMobile = window.matchMedia('(max-width: 767px)').matches;
	});

	// Calculate ghost event time from mouse position
	const ghostStartTime = $derived.by(() => {
		if (!isHovering || isMobile) return null;

		// Convert pixels to time
		const rawTime = pixelsToTime(mouseY, hourHeight);

		// Snap to interval
		const snappedTime = roundTimeToInterval(rawTime, snapInterval);

		return snappedTime;
	});

	// Generate 24 hours (0-23)
	const hours = Array.from({ length: 24 }, (_, i) => i);

	// Format hour for display (24-hour format without minutes)
	function formatHour(hour: number): string {
		return String(hour).padStart(2, '0');
	}

	// Ghost event click handler
	function handleGhostEventClick() {
		if (!ghostStartTime) return;

		// Calculate end time
		const [hours, minutes] = ghostStartTime.split(':').map(Number);
		const totalMinutes = hours * 60 + minutes + ghostDuration;
		const endHours = Math.floor(totalMinutes / 60);
		const endMinutes = totalMinutes % 60;
		const endTime = `${String(endHours).padStart(2, '0')}:${String(endMinutes).padStart(2, '0')}`;

		// Get date string
		const dateStr = extractDateString(date);

		// Call parent callback
		onGhostEventClick?.(dateStr, ghostStartTime, endTime);
	}

	// Event hover change handler
	function handleEventHoverChange(eventId: string | null) {
		hoveredEventId = eventId;
	}

	// Calculate layout for overlapping events
	const eventLayout = $derived(calculateEventLayout(events, date));

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
		return totalHours * hourHeight; // Dynamic hourHeight
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
			<div class="flex items-start justify-end pr-2 text-gray-500 dark:text-gray-500" style="height: {hourHeight}px;">
				{formatHour(hour)}
			</div>
		{/each}
	</div>

	<!-- Right: Timeline grid + events -->
	<div
		class="relative flex-1"
		role="region"
		aria-label="Calendar timeline"
		onmousemove={(e) => {
			if (!isMobile) {
				isHovering = true;
				// Use currentTarget to get position relative to timeline container, not the event target
				const rect = e.currentTarget.getBoundingClientRect();
				mouseY = e.clientY - rect.top;
			}
		}}
		onmouseleave={() => {
			isHovering = false;
		}}
	>
		<!-- Background grid lines -->
		<div class="absolute inset-0">
			{#each hours as hour}
				<!-- Full hour line -->
				<div
					class="absolute w-full border-t border-gray-300 dark:border-gray-700"
					style="top: {hour * hourHeight}px;"
				></div>
				<!-- Half-hour line (lighter) -->
				{#if hour < 23}
					<div
						class="absolute w-full border-t border-gray-200 dark:border-gray-800"
						style="top: {hour * hourHeight + halfHourHeight}px;"
					></div>
				{/if}
			{/each}
		</div>

		<!-- Events layer -->
		<div class="absolute inset-0">
			{#each events as event (event.id)}
				<TimeSpanEvent
					{event}
					currentDate={date}
					layout={eventLayout.get(event.id)}
					onclick={onEventClick}
					onHoverChange={handleEventHoverChange}
				/>
			{/each}
		</div>

		<!-- Ghost event layer (shows on hover, hidden when over real event) -->
		{#if ghostStartTime && !isMobile && !hoveredEventId}
			<div class="absolute inset-0 pointer-events-none">
				<GhostEvent
					startTime={ghostStartTime}
					duration={ghostDuration}
					mouseY={mouseY}
					onclick={handleGhostEventClick}
				/>
			</div>
		{/if}

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
