<script lang="ts">
	import { formatWeekMonth, formatDayHeader } from '$lib/utils/calendarUtils';

	interface Props {
		currentWeekStart: Date;
		currentDayIndex: number;
		weekDates: Date[];
		isMobile: boolean;
		onPreviousWeek: () => void;
		onNextWeek: () => void;
		onPreviousDay: () => void;
		onNextDay: () => void;
		onToday: () => void;
	}

	let {
		currentWeekStart,
		currentDayIndex,
		weekDates,
		isMobile,
		onPreviousWeek,
		onNextWeek,
		onPreviousDay,
		onNextDay,
		onToday
	}: Props = $props();
</script>

<div class="flex flex-col sm:flex-row items-center justify-between mb-6 gap-4">
	<!-- Title Display - conditional based on viewport -->
	<h2 class="text-2xl font-bold text-gray-900 dark:text-gray-100">
		{#if isMobile}
			{formatDayHeader(weekDates[currentDayIndex])}
		{:else}
			{formatWeekMonth(currentWeekStart)}
		{/if}
	</h2>

	<!-- Navigation Buttons - conditional based on viewport -->
	<div class="flex gap-2">
		{#if isMobile}
			<!-- Mobile: Day navigation -->
			<button
				onclick={onPreviousDay}
				class="px-4 py-2 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
				aria-label="Previous day"
			>
				← Prev Day
			</button>
			<button
				onclick={onToday}
				class="px-4 py-2 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg font-medium transition-colors"
				aria-label="Go to today"
			>
				Today
			</button>
			<button
				onclick={onNextDay}
				class="px-4 py-2 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
				aria-label="Next day"
			>
				Next Day →
			</button>
		{:else}
			<!-- Desktop: Week navigation -->
			<button
				onclick={onPreviousWeek}
				class="px-4 py-2 bg-gray-200 dark:bg-gray-700 hover:bg-gray-300 dark:hover:bg-gray-600 text-gray-900 dark:text-gray-100 rounded-lg transition-colors"
				aria-label="Previous week"
			>
				← Previous
			</button>
			<button
				onclick={onToday}
				class="px-4 py-2 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg font-medium transition-colors"
				aria-label="Go to today"
			>
				Today
			</button>
			<button
				onclick={onNextWeek}
				class="px-4 py-2 bg-gray-200 dark:bg-gray-700 hover:bg-gray-300 dark:hover:bg-gray-600 text-gray-900 dark:text-gray-100 rounded-lg transition-colors"
				aria-label="Next week"
			>
				Next →
			</button>
		{/if}
	</div>
</div>
