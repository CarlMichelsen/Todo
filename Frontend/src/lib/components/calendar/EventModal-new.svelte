<script lang="ts">
	import { get } from 'svelte/store';
	import FormModal from '$lib/components/modals/FormModal.svelte';
	import type { CalendarEvent } from '$lib/types/calendar';
	import { eventsStore } from '$lib/stores/events';
	import { calendarsStore } from '$lib/stores/calendars';
	import { toastStore } from '$lib/stores/toast';
	import { combineDateAndTime, extractDateString, extractTimeString } from '$lib/utils/calendarUtils';

	interface Props {
		isOpen?: boolean;
		/**
		 * Initial date for new events (YYYY-MM-DD format)
		 */
		initialDate?: string;
		/**
		 * Initial start time for new events (HH:MM format)
		 */
		initialStartTime?: string;
		/**
		 * Initial end time for new events (HH:MM format)
		 */
		initialEndTime?: string;
		/**
		 * Event to edit. If provided, modal is in edit mode.
		 * If undefined, modal is in create mode.
		 */
		event?: CalendarEvent;
	}

	let {
		isOpen = $bindable(false),
		initialDate,
		initialStartTime,
		initialEndTime,
		event
	}: Props = $props();

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

	// New fields for enhanced event support
	let location = $state('');
	let isAllDay = $state(false);
	let attendees = $state<{ email: string; commonName?: string; isOrganizer?: boolean }[]>([]);

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
			location = (event as any).location || '';
			isAllDay = (event as any).isAllDay || false;
			attendees = (event as any).attendees || [];
		} else {
			// Create mode: reset to defaults
			title = '';
			description = '';
			startDate = initialDate || new Date().toISOString().split('T')[0];
			endDate = initialDate || new Date().toISOString().split('T')[0];
			startTime = initialStartTime || '09:00';
			endTime = initialEndTime || '10:00';
			color = '#ea580c';
			location = '';
			isAllDay = false;
			attendees = [];
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

		errors = newErrors;
		return Object.keys(newErrors).length === 0;
	}

	function buildCalendarEvent(): CalendarEvent {
		return {
			id: event?.id || crypto.randomUUID(),
			title: title.trim(),
			description: description.trim() || undefined,
			start: new Date(startDate),
			end: new Date(endDate),
			color: color,
			location: location.trim() || undefined,
			isAllDay: isAllDay,
			status: 'confirmed',
			attendees: attendees,
			recurrence: undefined
		};
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
			const eventToSubmit = buildCalendarEvent();

			// Get active calendar ID
			const calendarId = get(calendarsStore).activeCalendarId;

			if (!calendarId) {
				submitError = 'No calendar selected';
				isSubmitting = false;
				return false;
			}

			if (isEditMode && event) {
				// Edit mode: Use CQRS store method
				await eventsStore.updateEvent(eventToSubmit);
			} else {
				// Create mode: Use CQRS store method
				await eventsStore.createEvent(eventToSubmit);
			}

			// Success: reset form, show toast, and return true
			submitError = '';
			toastStore.success(
				isEditMode ? 'Event updated successfully' : 'Event created successfully',
				3000
			);
			return true;
		} catch (error) {
			submitError = error instanceof Error ? error.message : 'Failed to save event';
			return false;
		} finally {
			isSubmitting = false;
		}
	}

	function handleCancelDelete() {
		showDeleteConfirm = false;
	}

	function resetForm() {
		// Reset form to defaults
		title = '';
		description = '';
		startDate = initialDate || new Date().toISOString().split('T')[0];
		endDate = initialDate || new Date().toISOString().split('T')[0];
		startTime = initialStartTime || '09:00';
		endTime = initialEndTime || '10:00';
		color = '#ea580c';
		location = '';
		isAllDay = false;
		attendees = [];
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
		<!-- Basic Info Section -->
		<div class="space-y-4 mb-6">
			<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
				<!-- Left Column -->
				<div class="space-y-4">
					<div>
						<label class="block text-sm font-medium text-gray-700 dark:text-gray-300" for="title">Event Title</label>
						<input
								id="title"
								type="text"
								bind:value={title}
								class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
								disabled={isSubmitting}
							/>
							{#if errors.title}
								<p class="text-sm text-red-600 dark:text-red-400 mt-1">{errors.title}</p>
							{/if}
						</div>
					
					<div>
						<label class="block text-sm font-medium text-gray-700 dark:text-gray-300" for="description">Description</label>
						<textarea
								id="description"
								bind:value={description}
								rows="3"
								disabled={isSubmitting}
								class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400 focus:border-transparent"
								placeholder="Optional description"
							></textarea>
					</div>
				</div>
				
				<!-- Right Column -->
				<div class="space-y-4">
					<!-- All-day Toggle -->
					<div class="space-y-2">
						<label class="flex items-center space-x-3 cursor-pointer">
							<input
									type="checkbox"
									bind:checked={isAllDay}
									disabled={isSubmitting}
									class="sr-only"
							/>
							<div class="relative">
								<div class="block bg-gray-300 w-14 h-8 rounded-full {isAllDay ? 'bg-blue-500' : ''} transition-colors duration-200"></div>
								<div class="absolute left-1 top-1 bg-white w-6 h-6 rounded-full transition-transform duration-200 {isAllDay ? 'translate-x-6' : ''}"></div>
							</div>
							<span class="text-sm font-medium text-gray-700 dark:text-gray-300">All-day event</span>
						</label>
					</div>
					
					<!-- Date/Time Section -->
					<div class="space-y-3">
						<div>
							<label class="block text-sm font-medium text-gray-700 dark:text-gray-300" for="start-date">Start</label>
								{#if isAllDay}
									<input
											type="date"
											id="start-date"
											bind:value={startDate}
											disabled={isSubmitting}
											class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
										/>
								{:else}
									<input
											type="datetime-local"
											id="start-date"
											bind:value={startDate}
											disabled={isSubmitting}
											class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
										/>
								{/if}
							</div>
							
							<div>
								<label class="block text-sm font-medium text-gray-700 dark:text-gray-300" for="end-date">End</label>
								{#if isAllDay}
									<input
											type="date"
											id="end-date"
											bind:value={endDate}
											disabled={isSubmitting}
											class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
										/>
								{:else}
									<input
											type="datetime-local"
											id="end-date"
											bind:value={endDate}
											disabled={isSubmitting}
											class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
										/>
								{/if}
							</div>
						</div>
					</div>
				</div>
			</div>
			
			<!-- Attendees Section -->
			<div class="space-y-3">
				<label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Attendees</label>
				
				<!-- Add Attendee Input -->
				<div class="flex space-x-2 mb-2">
					<input
								type="email"
								bind:value={newEmail}
								disabled={isSubmitting}
								placeholder="Add attendee by email..."
								class="flex-1 px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
							/>
					<button
								type="button"
								onclick={addAttendee}
								disabled={isSubmitting}
								class="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-500"
							>
							Add
						</button>
				</div>
				
				<!-- Validation Error -->
				{#if submitError?.includes('attendee')}
					<div class="text-sm text-red-600 dark:text-red-400 mb-2">{submitError}</div>
				{/if}
				
				<!-- Selected Attendees -->
				<div class="flex flex-wrap gap-2">
					{#each attendees as attendee (attendee.email)}
						<div class="inline-flex items-center space-x-2 px-3 py-1 bg-blue-100 text-blue-800 rounded-full">
								<span class="w-6 h-6 rounded-full bg-blue-500 flex items-center justify-center text-white text-xs">
									{attendee.commonName?.charAt(0) || attendee.email.charAt(0)}
								</span>
								<span class="text-sm">
									{attendee.commonName || attendee.email}
									{attendee.isOrganizer && '(Organizer)'}
								</span>
								{!attendee.isOrganizer && (
									<button
												type="button"
												onclick={() => removeAttendee(attendee.email)}
												disabled={isSubmitting}
												class="text-blue-600 hover:text-blue-800 ml-1"
												title="Remove attendee"
											>
												×
											</button>
								)}
							</div>
					{/each}
				</div>
			</div>
			
			<!-- Location Section -->
			<div class="space-y-2">
				<label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Location</label>
				<div class="relative">
					<input
								id="location"
								type="text"
								bind:value={location}
								disabled={isSubmitting}
								placeholder="Add location (optional)"
								class="w-full px-3 py-2 pr-10 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
							/>
					<div class="absolute right-2 top-2 text-gray-400 pointer-events-none">
								📍
					</div>
				</div>
			</div>
			
			<!-- Color Selection -->
			<div>
				<label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Event Color</label>
				<div class="grid grid-cols-4 gap-2 mt-2">
					{#each colorOptions as option}
						<button
									type="button"
									onclick={() => color = option.value}
									disabled={isSubmitting}
									class="h-8 rounded-lg border-2 {color === option.value ? 'border-gray-800' : 'border-transparent'}"
									style="background-color: {option.value}"
									title={option.name}
							></button>
					{/each}
				</div>
			</div>
		</div>
		
		<!-- Recurrence Section -->
		{#if !isEditMode}
			<div class="space-y-4 border-t pt-4">
				<div class="flex items-center space-x-3">
					<input
								type="checkbox"
								bind:checked={hasRecurrence}
								disabled={isSubmitting}
								class="sr-only"
							/>
							<div class="relative">
								<div class="block bg-gray-300 w-14 h-8 rounded-full {hasRecurrence ? 'bg-blue-500' : ''} transition-colors duration-200"></div>
								<div class="absolute left-1 top-1 bg-white w-6 h-6 rounded-full transition-transform duration-200 {hasRecurrence ? 'translate-x-6' : ''}"></div>
							</div>
							<span class="text-sm font-medium text-gray-700 dark:text-gray-300">Repeating event</span>
						</label>
					<div class="flex items-center space-x-3">
						<label class="text-sm font-medium text-gray-700 dark:text-gray-300">Every</label>
						<input
									type="number"
									bind:value={interval}
									min="1"
									disabled={!hasRecurrence || isSubmitting}
									class="w-16 px-2 py-1 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
								/>
						<span class="text-sm text-gray-600 dark:text-gray-400">
							{frequency === 'daily' ? 'day(s)' : 
								frequency === 'weekly' ? 'week(s)' :
								frequency === 'monthly' ? 'month(s)' : 'year(s)'}
						</span>
					</div>
				</div>
			</div>
		{/if}
	{/snippet}

	<!-- Error banner -->
	{#if submitError}
		<div class="mb-4 p-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg">
			<p class="text-sm text-red-600 dark:text-red-400">{submitError}</p>
		</div>
	{/if}
</FormModal>