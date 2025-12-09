<script lang="ts">
	import FormModal from '$lib/components/modals/FormModal.svelte';
	import type { CalendarEvent } from '$lib/types/calendar';
	import { eventsStore } from '$lib/stores/events';

	interface Props {
		isOpen?: boolean;
		/**
		 * Initial date for new events (YYYY-MM-DD format)
		 */
		initialDate?: string;
		/**
		 * Event to edit. If provided, modal is in edit mode.
		 * If undefined, modal is in create mode.
		 */
		event?: CalendarEvent;
	}

	let { isOpen = $bindable(false), initialDate, event }: Props = $props();

	// Determine if we're editing or creating
	let isEditMode = $derived(event !== undefined);

	// Form state - initialize from event if editing, otherwise use defaults
	let title = $state('');
	let description = $state('');
	let startDate = $state('');
	let endDate = $state('');
	let startTime = $state('09:00');
	let endTime = $state('10:00');
	let color = $state('#ea580c');

	// Initialize form when event changes
	$effect(() => {
		if (event) {
			// Edit mode: pre-fill with event data
			title = event.title;
			description = event.description || '';
			startDate = event.startDate;
			endDate = event.endDate;
			startTime = event.startTime;
			endTime = event.endTime;
			color = event.color || '#ea580c';
		} else {
			// Create mode: reset to defaults
			title = '';
			description = '';
			startDate = initialDate || new Date().toISOString().split('T')[0];
			endDate = initialDate || new Date().toISOString().split('T')[0];
			startTime = '09:00';
			endTime = '10:00';
			color = '#ea580c';
		}
	});

	// Validation errors
	let errors = $state<Record<string, string>>({});

	function validateForm(): boolean {
		const newErrors: Record<string, string> = {};

		if (!title.trim()) {
			newErrors.title = 'Title is required';
		}

		if (!startDate) {
			newErrors.startDate = 'Start date is required';
		}

		if (!endDate) {
			newErrors.endDate = 'End date is required';
		}

		if (!startTime) {
			newErrors.startTime = 'Start time is required';
		}

		if (!endTime) {
			newErrors.endTime = 'End time is required';
		}

		if (startTime && endTime && startTime >= endTime) {
			newErrors.endTime = 'End time must be after start time';
		}

		errors = newErrors;
		return Object.keys(newErrors).length === 0;
	}

	function handleSubmit() {
		if (!validateForm()) {
			return;
		}

		if (isEditMode && event) {
			// Edit mode: update existing event
			eventsStore.updateEvent(event.id, {
				title: title.trim(),
				description: description.trim() || undefined,
				startDate,
				endDate,
				startTime,
				endTime,
				color
			});
		} else {
			// Create mode: add new event
			const newEvent: CalendarEvent = {
				id: crypto.randomUUID(),
				title: title.trim(),
				description: description.trim() || undefined,
				startDate,
				endDate,
				startTime,
				endTime,
				color
			};
			eventsStore.addEvent(newEvent);
		}

		// Reset form (modal close is handled by FormModal)
		resetForm();
	}

	function handleCancel() {
		resetForm();
	}

	function handleDelete() {
		if (isEditMode && event) {
			eventsStore.deleteEvent(event.id);
			isOpen = false;
		}
	}

	function resetForm() {
		// Don't reset here - let $effect handle it based on event prop
		// This ensures form stays in sync with event prop
		errors = {};
	}

	// Predefined color options
	const colorOptions = [
		{ name: 'Orange', value: '#ea580c' },
		{ name: 'Blue', value: '#3b82f6' },
		{ name: 'Green', value: '#10b981' },
		{ name: 'Purple', value: '#8b5cf6' },
		{ name: 'Pink', value: '#ec4899' },
		{ name: 'Amber', value: '#f59e0b' },
		{ name: 'Cyan', value: '#06b6d4' },
		{ name: 'Red', value: '#ef4444' }
	];
</script>

<FormModal
	bind:isOpen
	title={isEditMode ? 'Edit Event' : 'Create Event'}
	size="full"
	submitText={isEditMode ? 'Save Changes' : 'Create Event'}
	cancelText="Cancel"
	onSubmit={handleSubmit}
	onCancel={handleCancel}
>
	{#snippet formContent()}
		<form class="space-y-4" onsubmit={(e) => e.preventDefault()}>
			<!-- Title -->
			<div>
				<label
					for="event-title"
					class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
				>
					Title *
				</label>
				<input
					type="text"
					id="event-title"
					bind:value={title}
					class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400 focus:border-transparent"
					placeholder="Event title"
				/>
				{#if errors.title}
					<p class="text-sm text-red-600 dark:text-red-400 mt-1">{errors.title}</p>
				{/if}
			</div>

			<!-- Description -->
			<div>
				<label
					for="event-description"
					class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
				>
					Description
				</label>
				<textarea
					id="event-description"
					bind:value={description}
					rows="3"
					class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400 focus:border-transparent"
					placeholder="Optional description"
				></textarea>
			</div>

			<!-- Date Range -->
			<div class="grid grid-cols-2 gap-4">
				<div>
					<label
						for="event-start-date"
						class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
					>
						Start Date *
					</label>
					<input
						type="date"
						id="event-start-date"
						bind:value={startDate}
						class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400 focus:border-transparent"
					/>
					{#if errors.startDate}
						<p class="text-sm text-red-600 dark:text-red-400 mt-1">{errors.startDate}</p>
					{/if}
				</div>

				<div>
					<label
						for="event-end-date"
						class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
					>
						End Date *
					</label>
					<input
						type="date"
						id="event-end-date"
						bind:value={endDate}
						class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400 focus:border-transparent"
					/>
					{#if errors.endDate}
						<p class="text-sm text-red-600 dark:text-red-400 mt-1">{errors.endDate}</p>
					{/if}
				</div>
			</div>

			<!-- Time Range -->
			<div class="grid grid-cols-2 gap-4">
				<div>
					<label
						for="event-start-time"
						class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
					>
						Start Time *
					</label>
					<input
						type="time"
						id="event-start-time"
						bind:value={startTime}
						class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400 focus:border-transparent"
					/>
					{#if errors.startTime}
						<p class="text-sm text-red-600 dark:text-red-400 mt-1">{errors.startTime}</p>
					{/if}
				</div>

				<div>
					<label
						for="event-end-time"
						class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
					>
						End Time *
					</label>
					<input
						type="time"
						id="event-end-time"
						bind:value={endTime}
						class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400 focus:border-transparent"
					/>
					{#if errors.endTime}
						<p class="text-sm text-red-600 dark:text-red-400 mt-1">{errors.endTime}</p>
					{/if}
				</div>
			</div>

			<!-- Color -->
			<div>
				<label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2" for="color-selector">
					Color
				</label>
				<div class="flex flex-wrap gap-2" id="color-selector">
					{#each colorOptions as colorOption}
						<button
							type="button"
							onclick={() => (color = colorOption.value)}
							class="w-10 h-10 rounded-lg border-2 transition-all hover:scale-110"
							class:border-gray-900={color === colorOption.value}
							class:dark:border-white={color === colorOption.value}
							class:border-gray-300={color !== colorOption.value}
							class:dark:border-gray-600={color !== colorOption.value}
							style="background-color: {colorOption.value}"
							title={colorOption.name}
							aria-label={`Select ${colorOption.name} color`}
						></button>
					{/each}
				</div>
			</div>

			<!-- Delete button (only in edit mode) -->
			{#if isEditMode}
				<div class="pt-4 border-t border-gray-200 dark:border-gray-700">
					<button
						type="button"
						onclick={handleDelete}
						class="px-4 py-2 bg-red-600 hover:bg-red-700 dark:bg-red-500 dark:hover:bg-red-600 text-white rounded-lg transition-colors text-sm"
					>
						Delete Event
					</button>
				</div>
			{/if}
		</form>
	{/snippet}
</FormModal>
