<script lang="ts">
	import { formatWeekMonth, formatDayHeader } from '$lib/utils/calendarUtils';
	import CalendarSelector from './CalendarSelector.svelte';
	import CalendarMenu from './CalendarMenu.svelte';

	interface Props {
		currentWeekStart: Date;
		currentDayIndex: number;
		weekDates: Date[];
		isMobile: boolean;
		isViewingToday: boolean;
		shouldHighlightPrevious: boolean;
		shouldHighlightNext: boolean;
		onPreviousWeek: () => void;
		onNextWeek: () => void;
		onPreviousDay: () => void;
		onNextDay: () => void;
		onToday: () => void;
		onAddEvent: () => void;
	}

	let {
		currentWeekStart,
		currentDayIndex,
		weekDates,
		isMobile,
		isViewingToday,
		shouldHighlightPrevious,
		shouldHighlightNext,
		onPreviousWeek,
		onNextWeek,
		onPreviousDay,
		onNextDay,
		onToday,
		onAddEvent
	}: Props = $props();
</script>

<div class="flex flex-col sm:flex-row items-start sm:items-center justify-between mb-6 gap-4">
	<!-- Left Side: Title and Calendar Selector -->
	<div class="flex items-center justify-between sm:justify-start w-full sm:w-auto gap-4">
		<h2 class="text-2xl font-bold text-gray-900 dark:text-gray-100">
			{#if isMobile}
				{formatDayHeader(weekDates[currentDayIndex])}
			{:else}
				{formatWeekMonth(currentWeekStart)}
			{/if}
		</h2>

		<!-- Calendar Selector -->
		<CalendarSelector />
	</div>

	<!-- Right Side: Navigation Buttons -->
	<div class="flex gap-2 items-center w-full sm:w-auto justify-center sm:justify-end">
		<!-- Add Event Button -->
		<button
			onclick={onAddEvent}
			class="p-2 sm:px-4 sm:py-2 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg font-medium transition-colors flex items-center gap-2"
			aria-label="Add new event"
		>
			<svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
				<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
			</svg>
			<span class="hidden sm:inline">Add Event</span>
		</button>

		<!-- Separator -->
		<div class="h-8 w-px bg-gray-300 dark:bg-gray-600"></div>

		{#if isMobile}
			<!-- Mobile: Day navigation (icon only) -->
			<button
				onclick={onPreviousDay}
				class={`p-2 rounded-lg font-medium transition-colors border-2 ${
					shouldHighlightPrevious
						? 'bg-green-100 dark:bg-green-900 border-green-500 dark:border-green-400 text-green-700 dark:text-green-300 hover:bg-green-200 dark:hover:bg-green-800'
						: 'bg-white dark:bg-gray-800 border-gray-300 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 text-gray-900 dark:text-gray-100'
				}`}
				aria-label="Previous day"
			>
				←
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
				class={`p-2 rounded-lg font-medium transition-colors border-2 ${
					shouldHighlightNext
						? 'bg-green-100 dark:bg-green-900 border-green-500 dark:border-green-400 text-green-700 dark:text-green-300 hover:bg-green-200 dark:hover:bg-green-800'
						: 'bg-white dark:bg-gray-800 border-gray-300 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 text-gray-900 dark:text-gray-100'
				}`}
				aria-label="Next day"
			>
				→
			</button>
		{:else}
			<!-- Desktop: Week navigation -->
			<button
				onclick={onPreviousWeek}
				class={`px-4 py-2 rounded-lg font-medium transition-colors border-2 ${
					shouldHighlightPrevious
						? 'bg-green-100 dark:bg-green-900 border-green-500 dark:border-green-400 text-green-700 dark:text-green-300 hover:bg-green-200 dark:hover:bg-green-800'
						: 'bg-gray-200 dark:bg-gray-700 border-gray-200 dark:border-gray-700 hover:bg-gray-300 dark:hover:bg-gray-600 text-gray-900 dark:text-gray-100'
				}`}
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
				class={`px-4 py-2 rounded-lg font-medium transition-colors border-2 ${
					shouldHighlightNext
						? 'bg-green-100 dark:bg-green-900 border-green-500 dark:border-green-400 text-green-700 dark:text-green-300 hover:bg-green-200 dark:hover:bg-green-800'
						: 'bg-gray-200 dark:bg-gray-700 border-gray-200 dark:border-gray-700 hover:bg-gray-300 dark:hover:bg-gray-600 text-gray-900 dark:text-gray-100'
				}`}
				aria-label="Next week"
			>
				Next →
			</button>
		{/if}

		<!-- Calendar Menu -->
		<CalendarMenu />
	</div>
</div>
