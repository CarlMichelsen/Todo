<script lang="ts">
	import Calendar from '$lib/components/calendar/Calendar.svelte';
	import { onMount } from 'svelte';
	import { getWeekStart } from '$lib/utils/calendarUtils';

	let initialWeekStart = $state<Date | undefined>(undefined);

	onMount(() => {
		const params = new URLSearchParams(window.location.search);
		const weekStartParam = params.get('weekStart');

		if (weekStartParam) {
			try {
				const parsed = new Date(weekStartParam + 'T00:00:00');
				if (!isNaN(parsed.getTime())) {
					// Ensure it's a Monday (getWeekStart normalizes)
					initialWeekStart = getWeekStart(parsed);
				}
			} catch {
				// Invalid date, will use default
			}
		}
	});

	function handleWeekChange(weekStart: Date) {
		const dateStr = weekStart.toISOString().split('T')[0]; // YYYY-MM-DD
		const url = new URL(window.location.href);
		url.searchParams.set('weekStart', dateStr);
		window.history.pushState({}, '', url);
	}
</script>

<div class="w-full px-4 py-8 h-full">
	<Calendar {initialWeekStart} onWeekChange={handleWeekChange} />
</div>
