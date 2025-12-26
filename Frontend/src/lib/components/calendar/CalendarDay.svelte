<script lang="ts">
	import Card from '$lib/components/Card.svelte';
	import Timeline from './Timeline.svelte';
	import { getCalendarConfig } from '$lib/stores/calendarConfig';
	import type { CalendarDay, CalendarEvent } from '$lib/types/calendar';

	interface Props {
		day: CalendarDay;
		/**
		 * Callback when an event is clicked
		 */
		onEventClick?: (event: CalendarEvent) => void;
	}

	let { day, onEventClick }: Props = $props();

	// Get calendar configuration
	const config = getCalendarConfig();
	const containerHeight = $derived(config.hourHeight * 24); // 24 hours

	// Build custom classes for today indicator (removed fixed height)
	let customClasses = $derived(
		day.isToday
			? 'border-l-4 border-l-orange-600 dark:border-l-orange-400'
			: ''
	);
</script>

<Card variant={day.isWeekend ? 'blue' : 'default'} class={customClasses} padding={false} style="height: {containerHeight}px;">
	<div class="flex flex-col h-full px-2">
		<!-- Day header -->
		<div class="flex items-center justify-between mb-2">
			<div class="text-sm font-semibold uppercase text-gray-600 dark:text-gray-400">
				{day.dayOfWeek}
			</div>
			<div class="text-2xl font-bold">
				<span class={day.isToday ? 'text-orange-600 dark:text-orange-400' : ''}>
					{day.dayOfMonth}
				</span>
			</div>
		</div>

		<!-- Timeline with events -->
		<div class="flex-1 overflow-hidden">
			<Timeline date={day.date} events={day.events} onEventClick={onEventClick} />
		</div>
	</div>
</Card>
