<script lang="ts">
	import { get } from 'svelte/store';
	import FormModal from '$lib/components/modals/FormModal.svelte';
	import ConfirmModal from '$lib/components/modals/ConfirmModal.svelte';
	import type { CalendarEvent } from '$lib/types/calendar';
	import { eventsStore } from '$lib/stores/events';
	import { calendarsStore } from '$lib/stores/calendars';
	import { combineDateAndTime, extractDateString, extractTimeString } from '$lib/utils/calendarUtils';
	import { EventClient } from '$lib/utils/eventClient';
	import { eventDtoToCalendarEvent } from '$lib/utils/eventConverter';
	import type { CreateEventDto, EditEventDto } from '$lib/types/api/event';

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

	// New states for API integration
	let isSubmitting = $state(false);
	let submitError = $state<string | null>(null);
	let showDeleteConfirm = $state(false);

	// Initialize form when event changes
	$effect(() => {
		if (event) {
			// Edit mode: pre-fill with event data
			title = event.title;
			description = event.description || '';
			startDate = extractDateString(event.start);
			endDate = extractDateString(event.end);
			startTime = extractTimeString(event.start);
			endTime = extractTimeString(event.end);
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

		// Validate that end is after start
		if (startDate && endDate && startTime && endTime) {
			const startDateTime = combineDateAndTime(startDate, startTime);
			const endDateTime = combineDateAndTime(endDate, endTime);

			if (startDateTime >= endDateTime) {
				newErrors.endTime = 'End must be after start';
			}
		}

		errors = newErrors;
		return Object.keys(newErrors).length === 0;
	}

	async function handleSubmit(): Promise<boolean> {
		// Clear previous errors
		submitError = null;

		// Validate form
		if (!validateForm()) {
			return false;
		}

		// Set loading state
		isSubmitting = true;

		try {
			const startDateTime = combineDateAndTime(startDate, startTime);
			const endDateTime = combineDateAndTime(endDate, endTime);

			// Get active calendar ID
			const calendarId = get(calendarsStore).activeCalendarId;

			if (!calendarId) {
				submitError = 'No calendar selected';
				isSubmitting = false;
				return false;
			}

			if (isEditMode && event) {
				// Edit mode: Call EventClient API
				const client = new EventClient();

				// Convert to EditEventDto
				const editDto: EditEventDto = {
					title: title.trim(),
					description: description.trim() || '', // API requires string, not undefined
					start: startDateTime.toISOString(),
					end: endDateTime.toISOString(),
					color: color
				};

				// Call API
				const eventDto = await client.updateEvent(calendarId, event.id, editDto);

				// Convert to CalendarEvent
				const calendarEvent = eventDtoToCalendarEvent(eventDto);

				// Update store
				eventsStore.updateEvent(event.id, calendarEvent);
			} else {
				// Create mode: Use EventClient API
				const client = new EventClient();

				// Convert to CreateEventDto
				const createDto: CreateEventDto = {
					title: title.trim(),
					description: description.trim() || '', // API requires string, not undefined
					start: startDateTime.toISOString(),
					end: endDateTime.toISOString(),
					color: color
				};

				// Call API
				const eventDto = await client.createEvent(calendarId, createDto);

				// Convert to CalendarEvent
				const calendarEvent = eventDtoToCalendarEvent(eventDto);

				// Update store
				eventsStore.addEvent(calendarEvent);
			}

			// Success: reset form and return true
			resetForm();
			return true;
		} catch (error) {
			// Handle error: display message and return false
			if (error instanceof Error) {
				submitError = error.message;
			} else {
				submitError = 'Failed to create event. Please try again.';
			}
			return false;
		} finally {
			isSubmitting = false;
		}
	}

	function handleCancel() {
		resetForm();
	}

	function handleDeleteClick() {
		// Show confirmation dialog instead of deleting immediately
		showDeleteConfirm = true;
	}

	async function handleConfirmDelete() {
		if (isEditMode && event) {
			try {
				isSubmitting = true;
				submitError = null;
				showDeleteConfirm = false; // Close confirmation dialog

				// Get active calendar ID
				const calendarId = get(calendarsStore).activeCalendarId;

				if (!calendarId) {
					submitError = 'No calendar selected';
					isSubmitting = false;
					return;
				}

				const client = new EventClient();
				await client.deleteEvent(calendarId, event.id);

				// Success: update store and close modal
				eventsStore.deleteEvent(event.id);
				isOpen = false;
			} catch (error) {
				submitError = error instanceof Error ? error.message : 'Failed to delete event';
				console.error('Delete event error:', error);
			} finally {
				isSubmitting = false;
			}
		}
	}

	function handleCancelDelete() {
		showDeleteConfirm = false;
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
		<!-- Error banner -->
		{#if submitError}
			<div
				class="mb-4 p-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg"
			>
				<p class="text-sm text-red-600 dark:text-red-400">{submitError}</p>
			</div>
		{/if}

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
					disabled={isSubmitting}
					class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400 focus:border-transparent {isSubmitting
						? 'opacity-50 cursor-not-allowed'
						: ''}"
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
					disabled={isSubmitting}
					class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400 focus:border-transparent {isSubmitting
						? 'opacity-50 cursor-not-allowed'
						: ''}"
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
						disabled={isSubmitting}
						class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400 focus:border-transparent {isSubmitting
							? 'opacity-50 cursor-not-allowed'
							: ''}"
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
						disabled={isSubmitting}
						class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400 focus:border-transparent {isSubmitting
							? 'opacity-50 cursor-not-allowed'
							: ''}"
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
						disabled={isSubmitting}
						class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400 focus:border-transparent {isSubmitting
							? 'opacity-50 cursor-not-allowed'
							: ''}"
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
						disabled={isSubmitting}
						class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400 focus:border-transparent {isSubmitting
							? 'opacity-50 cursor-not-allowed'
							: ''}"
					/>
					{#if errors.endTime}
						<p class="text-sm text-red-600 dark:text-red-400 mt-1">{errors.endTime}</p>
					{/if}
				</div>
			</div>

			<!-- Color and Delete Button -->
			<div>
				<!-- Header row with label and delete button -->
				<div class="flex items-center justify-between mb-2">
					<label class="block text-sm font-medium text-gray-700 dark:text-gray-300" for="color-selector">
						Color
					</label>
					{#if isEditMode}
						<button
							type="button"
							onclick={handleDeleteClick}
							disabled={isSubmitting}
							class="px-3 py-1.5 bg-red-600 hover:bg-red-700 dark:bg-red-500 dark:hover:bg-red-600 text-white rounded-lg transition-colors text-sm {isSubmitting
								? 'opacity-50 cursor-not-allowed'
								: ''}"
						>
							Delete Event
						</button>
					{/if}
				</div>

				<!-- Color swatches -->
				<div class="flex flex-wrap gap-2" id="color-selector">
					{#each colorOptions as colorOption}
						<button
							type="button"
							onclick={() => (color = colorOption.value)}
							disabled={isSubmitting}
							class="w-10 h-10 rounded-lg border-2 transition-all hover:scale-110 {isSubmitting
								? 'opacity-50 cursor-not-allowed'
								: ''}"
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
		</form>
	{/snippet}
</FormModal>

<ConfirmModal
	isOpen={showDeleteConfirm}
	title="Delete Event"
	message="Are you sure you want to delete this event? This action cannot be undone."
	confirmText="Delete"
	cancelText="Cancel"
	onConfirm={handleConfirmDelete}
	onCancel={handleCancelDelete}
/>
