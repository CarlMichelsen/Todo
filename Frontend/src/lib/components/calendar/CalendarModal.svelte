<script lang="ts">
	import FormModal from '$lib/components/modals/FormModal.svelte';
	import { calendarsStore } from '$lib/stores/calendars';
	import { toastStore } from '$lib/stores/toast';
	import type { CreateCalendarDto } from '$lib/types/api/calendar';

	interface Props {
		isOpen?: boolean;
	}

	let { isOpen = $bindable(false) }: Props = $props();

	// Form state
	let title = $state('');
	let color = $state('#ea580c'); // Default orange
	let isSubmitting = $state(false);
	let submitError = $state<string | null>(null);
	let errors = $state<Record<string, string>>({});

	// Same color options as EventModal for consistency
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

	function validateForm(): boolean {
		const newErrors: Record<string, string> = {};
		if (!title.trim()) {
			newErrors.title = 'Calendar title is required';
		}
		errors = newErrors;
		return Object.keys(newErrors).length === 0;
	}

	async function handleSubmit(): Promise<boolean> {
		submitError = null;
		if (!validateForm()) return false;

		isSubmitting = true;
		try {
			const createDto: CreateCalendarDto = {
				title: title.trim(),
				color: color
			};
			const created = await calendarsStore.createCalendar(createDto);
			toastStore.success(`Calendar "${created.title}" created successfully`, 3000);
			resetForm();
			return true; // Close modal
		} catch (error) {
			const errorMessage = error instanceof Error ? error.message : 'Failed to create calendar';
			if (errorMessage.includes('Invalid calendar data:')) {
				// Parse field-level validation errors
				const validationPart = errorMessage.replace('Invalid calendar data: ', '');
				const fieldErrors = validationPart.split('; ');
				fieldErrors.forEach((fieldError) => {
					const [field, ...messageParts] = fieldError.split(': ');
					const message = messageParts.join(': ');
					toastStore.error(`${field}: ${message}`, 7000);
				});
				submitError = 'Please fix the validation errors';
			} else {
				toastStore.error(errorMessage, 5000);
				submitError = errorMessage;
			}
			return false; // Keep modal open
		} finally {
			isSubmitting = false;
		}
	}

	function resetForm() {
		title = '';
		color = '#ea580c';
		errors = {};
		submitError = null;
	}

	// Reset when modal closes
	$effect(() => {
		if (!isOpen) resetForm();
	});
</script>

<FormModal
	bind:isOpen
	title="Create Calendar"
	size="md"
	submitText="Create Calendar"
	onSubmit={handleSubmit}
>
	{#snippet formContent()}
		<!-- Error banner (if submitError) -->
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
					for="calendar-title"
					class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
				>
					Calendar Name *
				</label>
				<input
					type="text"
					id="calendar-title"
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
					for="calendar-color"
					class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2"
				>
					Color *
				</label>
				<div id="calendar-color" class="flex flex-wrap gap-2" role="radiogroup">
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
