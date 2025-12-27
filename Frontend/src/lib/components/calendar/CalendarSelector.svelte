<script lang="ts">
	import Dropdown from '$lib/components/Dropdown.svelte';
	import CalendarModal from './CalendarModal.svelte';
	import { calendarsStore } from '$lib/stores/calendars';

	// Subscribe to store
	let storeState = $derived($calendarsStore);
	let activeCalendar = $derived(
		storeState.calendars.find((c) => c.id === storeState.activeCalendarId)
	);

	// Modal state
	let isCalendarModalOpen = $state(false);

	function handleCalendarSelect(calendarId: string) {
		void calendarsStore.setActiveCalendar(calendarId);
	}

	function handleNewCalendar() {
		isCalendarModalOpen = true;
	}
</script>

<Dropdown align="left">
		{#snippet trigger()}
			<button
				class="flex items-center gap-2 px-4 py-2 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
			>
				{#if activeCalendar}
					<!-- Color indicator dot -->
					<div
						class="w-3 h-3 rounded-full"
						style="background-color: {activeCalendar.color}"
					></div>
					<span class="text-gray-900 dark:text-gray-100 font-medium truncate max-w-[110px] sm:max-w-[300px]" title={activeCalendar.title}>
						{activeCalendar.title}
					</span>
				{:else}
					<span class="text-gray-500 dark:text-gray-400">Select Calendar</span>
				{/if}
				<!-- Chevron icon -->
				<svg
					class="w-4 h-4 text-gray-500"
					fill="none"
					stroke="currentColor"
					viewBox="0 0 24 24"
				>
					<path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M19 9l-7 7-7-7"
					/>
				</svg>
			</button>
		{/snippet}

		{#snippet content()}
			<div class="py-1">
				<!-- Calendar list -->
				{#each storeState.calendars as calendar}
					<button
						onclick={() => handleCalendarSelect(calendar.id)}
						class="w-full px-4 py-2 text-left hover:bg-gray-100 dark:hover:bg-gray-700 flex items-center gap-2 transition-colors"
					>
						<!-- Color dot -->
						<div
							class="w-3 h-3 rounded-full flex-shrink-0"
							style="background-color: {calendar.color}"
						></div>
						<span class="text-gray-900 dark:text-gray-100 truncate max-w-[110px] sm:max-w-[300px] flex-1" title={calendar.title}>{calendar.title}</span>
						<!-- Active checkmark -->
						{#if calendar.id === storeState.activeCalendarId}
							<svg
								class="w-4 h-4 ml-auto text-orange-600 dark:text-orange-400"
								fill="currentColor"
								viewBox="0 0 20 20"
							>
								<path
									fill-rule="evenodd"
									d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
									clip-rule="evenodd"
								/>
							</svg>
						{/if}
					</button>
				{/each}

				<!-- Separator -->
				{#if storeState.calendars.length > 0}
					<div class="border-t border-gray-200 dark:border-gray-700 my-1"></div>
				{/if}

				<!-- New Calendar button -->
				<button
					onclick={handleNewCalendar}
					class="w-full px-4 py-2 text-left hover:bg-gray-100 dark:hover:bg-gray-700 flex items-center gap-2 text-orange-600 dark:text-orange-400 font-medium transition-colors"
				>
					<svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
						<path
							stroke-linecap="round"
							stroke-linejoin="round"
							stroke-width="2"
							d="M12 4v16m8-8H4"
						/>
					</svg>
					New
				</button>
			</div>
		{/snippet}
</Dropdown>

<!-- Calendar Modal -->
<CalendarModal bind:isOpen={isCalendarModalOpen} />
