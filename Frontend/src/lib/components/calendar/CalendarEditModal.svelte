<script lang="ts">
	import FormModal from '$lib/components/modals/FormModal.svelte';
	import ConfirmModal from '$lib/components/modals/ConfirmModal.svelte';
	import { calendarsStore } from '$lib/stores/calendars';
	import { toastStore } from '$lib/stores/toast';
	import type { CalendarDto } from '$lib/types/api/calendar';

	interface Props {
		isOpen?: boolean;
		calendar?: CalendarDto;
	}

	let { isOpen = $bindable(false), calendar }: Props = $props();

	// Form state
	let title = $state('');
	let color = $state('#ea580c');
	let isSubmitting = $state(false);
	let submitError = $state<string | null>(null);
	let errors = $state<Record<string, string>>({});

	// Delete confirmation
	let showDeleteConfirm = $state(false);
	let isDeleting = $state(false);

	// Same color options as CalendarModal
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

	// Initialize form when calendar changes
	$effect(() => {
		if (calendar) {
			title = calendar.title;
			color = calendar.color;
		}
	});

	function validateForm(): boolean {
		const newErrors: Record<string, string> = {};
		if (!title.trim()) {
			newErrors.title = 'Calendar title is required';
		}
		errors = newErrors;
		return Object.keys(newErrors).length === 0;
	}

	async function handleSubmit(): Promise<boolean> {
		if (!calendar) return false;

		submitError = null;
		if (!validateForm()) return false;

		isSubmitting = true;
		try {
			const updates = {
				title: title.trim(),
				color: color
			};
			const updated = await calendarsStore.updateCalendar(calendar.id, updates);
			toastStore.success(`Calendar "${updated.title}" updated successfully`, 3000);
			return true; // Close modal
		} catch (error) {
			const errorMessage = error instanceof Error ? error.message : 'Failed to update calendar';
			toastStore.error(errorMessage, 5000);
			submitError = errorMessage;
			return false;
		} finally {
			isSubmitting = false;
		}
	}

	function handleDeleteClick() {
		showDeleteConfirm = true;
	}

	async function handleConfirmDelete() {
		if (!calendar) return;

		isDeleting = true;
		try {
			await calendarsStore.deleteCalendar(calendar.id);
			toastStore.success(`Calendar "${calendar.title}" deleted successfully`, 3000);
			showDeleteConfirm = false;
			isOpen = false;
		} catch (error) {
			const errorMessage = error instanceof Error ? error.message : 'Failed to delete calendar';
			toastStore.error(errorMessage, 5000);
		} finally {
			isDeleting = false;
		}
	}

	function resetForm() {
		if (calendar) {
			title = calendar.title;
			color = calendar.color;
		}
		errors = {};
		submitError = null;
	}

	// Reset when modal closes
	$effect(() => {
		if (!isOpen) {
			resetForm();
			showDeleteConfirm = false;
		}
	});
</script>

<FormModal
	bind:isOpen
	title="Edit Calendar"
	size="md"
	submitText="Save Changes"
	onSubmit={handleSubmit}
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
			<!-- Title Input -->
			<div>
				<label
					for="edit-calendar-title"
					class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
				>
					Calendar Name *
				</label>
				<input
					type="text"
					id="edit-calendar-title"
					bind:value={title}
					disabled={isSubmitting}
					placeholder="My Calendar"
					class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 focus:border-transparent {isSubmitting
						? 'opacity-50 cursor-not-allowed'
						: ''}"
				/>
				{#if errors.title}
					<p class="text-sm text-red-600 dark:text-red-400 mt-1">{errors.title}</p>
				{/if}
			</div>

			<!-- Color Picker -->
			<div>
				<label
					for="edit-calendar-color"
					class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
				>
					Color *
				</label>
				<div id="edit-calendar-color" class="flex flex-wrap gap-2" role="radiogroup">
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

			<!-- Delete Button -->
			<div class="pt-4 border-t border-gray-200 dark:border-gray-700">
				<button
					type="button"
					onclick={handleDeleteClick}
					disabled={isSubmitting}
					class="w-full px-4 py-2 bg-red-600 hover:bg-red-700 dark:bg-red-500 dark:hover:bg-red-600 text-white rounded-lg font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
				>
					Delete Calendar
				</button>
			</div>
		</form>
	{/snippet}
</FormModal>

<!-- Delete Confirmation Modal -->
<ConfirmModal
	bind:isOpen={showDeleteConfirm}
	title="Delete Calendar"
	message={`Are you sure you want to delete "${calendar?.title}"? This action cannot be undone.`}
	confirmText="Delete"
	cancelText="Cancel"
	onConfirm={handleConfirmDelete}
/>
