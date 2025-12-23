<script lang="ts">
	import Dropdown from '$lib/components/Dropdown.svelte';
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

<Dropdown align="right">
	{#snippet trigger()}
		<button
			class="p-2 rounded-lg bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
			aria-label="Calendar options"
		>
			<!-- Burger icon -->
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
					d="M4 6h16M4 12h16M4 18h16"
				/>
			</svg>
		</button>
	{/snippet}

	{#snippet content()}
		<div class="py-1 min-w-[180px]">
			<!-- Edit Calendar -->
			<button
				onclick={handleEditCalendar}
				disabled={!activeCalendar}
				class="w-full px-4 py-2 text-left hover:bg-gray-100 dark:hover:bg-gray-700 flex items-center gap-2 text-gray-900 dark:text-gray-100 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
			>
				<svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
					<path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
					/>
				</svg>
				Edit Calendar
			</button>
		</div>
	{/snippet}
</Dropdown>

<!-- Edit Modal -->
{#if activeCalendar}
	<CalendarEditModal bind:isOpen={isEditModalOpen} calendar={activeCalendar} />
{/if}
