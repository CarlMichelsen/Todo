<script lang="ts">
	import Calendar from '$lib/components/calendar/Calendar.svelte';
	import { getWeekStart } from '$lib/utils/calendarUtils';

	// Parse URL parameter immediately during initialization
	function parseInitialWeekStart(): Date | undefined {
		if (typeof window === 'undefined') return undefined;

		const params = new URLSearchParams(window.location.search);
		const weekStartParam = params.get('weekStart');

		if (weekStartParam) {
			try {
				// Parse YYYY-MM-DD explicitly to avoid timezone issues
				const [year, month, day] = weekStartParam.split('-').map(Number);
				if (year && month && day) {
					const parsed = new Date(year, month - 1, day); // Month is 0-indexed
					if (!isNaN(parsed.getTime())) {
						// Ensure it's a Monday (getWeekStart normalizes)
						return getWeekStart(parsed);
					}
				}
			} catch {
				// Invalid date, will use default
			}
		}

		return undefined;
	}

	let initialWeekStart = $state<Date | undefined>(parseInitialWeekStart());

	function handleWeekChange(weekStart: Date) {
		// Format date using local time to avoid UTC conversion
		const year = weekStart.getFullYear();
		const month = String(weekStart.getMonth() + 1).padStart(2, '0');
		const day = String(weekStart.getDate()).padStart(2, '0');
		const dateStr = `${year}-${month}-${day}`;

		const url = new URL(window.location.href);
		url.searchParams.set('weekStart', dateStr);
		window.history.pushState({}, '', url);
	}
</script>

<div class="w-full px-4 py-8 h-full">
	<Calendar {initialWeekStart} onWeekChange={handleWeekChange} />
</div>
