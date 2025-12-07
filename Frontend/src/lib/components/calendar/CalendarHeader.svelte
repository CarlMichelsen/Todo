<script lang="ts">
	import { formatWeekMonth } from '$lib/utils/calendarUtils';

	interface Props {
		currentWeekStart: Date;
		onPreviousWeek: () => void;
		onNextWeek: () => void;
		onToday: () => void;
	}

	let { currentWeekStart, onPreviousWeek, onNextWeek, onToday }: Props = $props();
</script>

<div class="flex flex-col sm:flex-row items-center justify-between mb-6 gap-4">
	<!-- Month/Year Display -->
	<h2 class="text-2xl font-bold text-gray-900 dark:text-gray-100">
		{formatWeekMonth(currentWeekStart)}
	</h2>

	<!-- Navigation Buttons -->
	<div class="flex gap-2">
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
	</div>
</div>
