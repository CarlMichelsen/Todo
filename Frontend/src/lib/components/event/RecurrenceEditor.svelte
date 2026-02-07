<script lang="ts">
	import type { RecurrenceRule } from '$lib/types/calendar';
	
	interface Props {
		value?: RecurrenceRule | null;
		onValueChange?: (rule: RecurrenceRule | null) => void;
	}
	
	let {
		value = null,
		onValueChange
	}: Props = $props();
	
	const weekDays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
	
	const frequencyPresets = [
		{ label: 'Daily', value: 'daily', interval: 1 },
		{ label: 'Weekly', value: 'weekly', interval: 1 },
		{ label: 'Monthly', value: 'monthly', interval: 1 },
		{ label: 'Yearly', value: 'yearly', interval: 1 }
	];
	
	let hasRecurrence = $derived(!!value);
	let frequency = $derived(value?.frequency || 'weekly');
	let interval = $derived(value?.interval || 1);
	
	function generateRecurrenceText(rule: RecurrenceRule): string {
		const { frequency, interval, daysOfWeek, dayOfMonth } = rule;
		
		if (frequency === 'daily') {
			return interval === 1 ? 'Daily' : `Every ${interval} days`;
		} else if (frequency === 'weekly') {
			if (!daysOfWeek || daysOfWeek.length === 0) return 'Weekly';
			const dayNames = daysOfWeek.map((d: number) => weekDays[d]).join(', ');
			return interval === 1 ? `Weekly on ${dayNames}` : `Every ${interval} weeks on ${dayNames}`;
		} else if (frequency === 'monthly') {
			if (dayOfMonth) {
				return interval === 1 ? `Monthly on day ${dayOfMonth}` : `Every ${interval} months on day ${dayOfMonth}`;
			}
			return interval === 1 ? 'Monthly' : `Every ${interval} months`;
		} else if (frequency === 'yearly') {
			return interval === 1 ? 'Yearly' : `Every ${interval} years`;
		}
		return '';
	}
	
	let recurrenceText = $derived(value ? generateRecurrenceText(value) : '');
	
	function handleFrequencyChange(newFrequency: RecurrenceRule['frequency']) {
		if (value) {
			onValueChange?.({
				...value,
				frequency: newFrequency,
				interval: 1,
				daysOfWeek: newFrequency === 'weekly' ? value.daysOfWeek : undefined,
				dayOfMonth: newFrequency === 'monthly' ? value.dayOfMonth : undefined
			});
		} else {
			onValueChange?.({
				frequency: newFrequency,
				interval: 1
			});
		}
	}
	
	function toggleRecurrence() {
		if (hasRecurrence) {
			onValueChange?.(null);
		} else {
			onValueChange?.({
				frequency: 'weekly',
				interval: 1
			});
		}
	}
</script>

<!-- Recurrence Toggle -->
<div class="space-y-4">
	<label class="flex items-center space-x-3 cursor-pointer">
		<input
			type="checkbox"
			bind:checked={hasRecurrence}
			onchange={toggleRecurrence}
			class="sr-only"
		/>
		<div class="relative">
			<div class="block bg-gray-300 w-14 h-8 rounded-full {hasRecurrence ? 'bg-blue-500' : ''} transition-colors duration-200"></div>
			<div class="absolute left-1 top-1 bg-white w-6 h-6 rounded-full transition-transform duration-200 {hasRecurrence ? 'translate-x-6' : ''}"></div>
		</div>
		<span class="text-sm font-medium text-gray-700 dark:text-gray-300">Repeating event</span>
	</label>
	
	{#if hasRecurrence && value}
		<!-- Frequency Selection -->
		<div class="space-y-3">
			<div class="space-y-2">
				<div class="text-sm font-medium text-gray-700 dark:text-gray-300">Repeat</div>
				<div class="grid grid-cols-4 gap-2">
					{#each frequencyPresets as preset}
						<button
							type="button"
							onclick={() => handleFrequencyChange(preset.value as RecurrenceRule['frequency'])}
							class="px-3 py-2 text-sm border rounded-lg
								{frequency === preset.value ? 'bg-blue-500 text-white border-blue-500' : 'hover:bg-gray-50 dark:hover:bg-gray-700'}"
						>
							{preset.label}
						</button>
					{/each}
				</div>
			</div>
			
			<!-- Interval -->
			<div class="flex items-center space-x-3">
				<label for="recurrence-interval" class="text-sm font-medium text-gray-700 dark:text-gray-300">Every</label>
				<input
						id="recurrence-interval"
						type="number"
						bind:value={interval}
						oninput={() => {
							if (value) {
								onValueChange?.({ ...value, interval });
							}
						}}
						min="1"
						class="w-16 px-2 py-1 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
					/>
				<span class="text-sm text-gray-600 dark:text-gray-400">
					{frequency === 'daily' ? 'day(s)' : 
					 frequency === 'weekly' ? 'week(s)' :
					 frequency === 'monthly' ? 'month(s)' : 'year(s)'}
				</span>
			</div>
			
			<!-- Weekly Days Selection -->
			{#if frequency === 'weekly'}
				<div class="space-y-2">
					<div class="text-sm font-medium text-gray-700 dark:text-gray-300">On</div>
					<div class="flex space-x-2">
						{#each weekDays as day, i}
							<button
									type="button"
									onclick={() => {
										const daysOfWeek = value.daysOfWeek || [];
										if (daysOfWeek.includes(i)) {
											onValueChange?.({
												...value,
												daysOfWeek: daysOfWeek.filter((d: number) => d !== i)
											});
										} else {
											onValueChange?.({
												...value,
												daysOfWeek: [...daysOfWeek, i]
											});
										}
									}}
									class="w-8 h-8 rounded-full border text-sm
										{value.daysOfWeek?.includes(i) ? 'bg-blue-500 text-white border-blue-500' : 'hover:bg-gray-50 dark:hover:bg-gray-700'}"
							>
								{day.charAt(0)}
							</button>
						{/each}
					</div>
				</div>
			{/if}
		</div>
		
		<!-- Recurrence Summary -->
		<div class="p-3 bg-blue-50 dark:bg-blue-900 rounded-lg text-sm text-blue-800 dark:text-blue-200">
			<div class="font-medium mb-1">Recurrence:</div>
			<div>{recurrenceText}</div>
		</div>
	{/if}
</div>