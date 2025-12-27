<script lang="ts">
	import CalendarEditModal from './CalendarEditModal.svelte';
	import { calendarsStore } from '$lib/stores/calendars';

	let storeState = $derived($calendarsStore);
	let activeCalendar = $derived(
		storeState.calendars.find((c) => c.id === storeState.activeCalendarId)
	);

	let isEditModalOpen = $state(false);

	function handleEditCalendar() {
		isEditModalOpen = true;
	}
</script>

<button
	onclick={handleEditCalendar}
	disabled={!activeCalendar}
	class="flex items-center gap-2 px-4 py-2.5 rounded-lg bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
	aria-label="Calendar settings"
>
	<!-- Gear/Settings icon -->
	<svg
		class="w-5 h-5 text-gray-700 dark:text-gray-300"
		fill="none"
		stroke="currentColor"
		viewBox="0 0 24 24"
	>
		<path
			stroke-linecap="round"
			stroke-linejoin="round"
			stroke-width="2"
			d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"
		/>
		<path
			stroke-linecap="round"
			stroke-linejoin="round"
			stroke-width="2"
			d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
		/>
	</svg>
</button>

<!-- Edit Modal -->
{#if activeCalendar}
	<CalendarEditModal bind:isOpen={isEditModalOpen} calendar={activeCalendar} />
{/if}
