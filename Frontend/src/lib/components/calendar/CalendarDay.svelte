<script lang="ts">
	import Card from '$lib/components/Card.svelte';
	import type { CalendarDay } from '$lib/types/calendar';

	interface Props {
		day: CalendarDay;
		onclick?: (date: Date) => void;
	}

	let { day, onclick }: Props = $props();

	function handleClick() {
		if (onclick) {
			onclick(day.date);
		}
	}

	// Build custom classes for today indicator
	let customClasses = $derived(
		day.isToday
			? 'border-l-4 border-l-orange-600 dark:border-l-orange-400 min-h-[240px] h-full'
			: 'min-h-[240px] h-full'
	);
</script>

<button
	onclick={handleClick}
	class="w-full h-full hover:brightness-95 dark:hover:brightness-110 transition-all cursor-pointer"
>
	<Card variant={day.isWeekend ? 'blue' : 'default'} class={customClasses}>
		<div class="flex flex-col h-full">
			<!-- Day header -->
			<div class="flex items-center justify-between mb-3">
				<div class="text-sm font-semibold uppercase text-gray-600 dark:text-gray-400">
					{day.dayOfWeek}
				</div>
				<div class="text-2xl font-bold">
					<span class={day.isToday ? 'text-orange-600 dark:text-orange-400' : ''}>
						{day.dayOfMonth}
					</span>
				</div>
			</div>

			<!-- Events container -->
			<div class="flex-1 space-y-1 overflow-y-auto">
				<!-- Future: Events will be rendered here -->
			</div>
		</div>
	</Card>
</button>
