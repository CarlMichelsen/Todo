<script lang="ts">
	import type { RecurrenceRule } from '$lib/types/calendar';
	import { addDays } from '$lib/utils/calendarUtils';
	
	interface Props {
		value?: RecurrenceRule | null;
		onValueChange?: (rule: RecurrenceRule | null) => void;
		disabled?: boolean;
		startDate?: Date;
	}
	
	let {
		value = $bindable(null),
		onValueChange,
		disabled = false,
		startDate = new Date()
	}: Props = $props();
	
	const weekDays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
	
	const frequencyPresets = [
		{ label: 'Daily', value: 'daily', interval: 1 },
		{ label: 'Weekly', value: 'weekly', interval: 1 },
		{ label: 'Monthly', value: 'monthly', interval: 1 },
		{ label: 'Yearly', value: 'yearly', interval: 1 }
	];
	
	// Simple date formatting function
	function formatDate(date: Date): string {
		return date.toLocaleDateString('en-US', { 
			month: 'short', 
			day: 'numeric', 
			year: 'numeric' 
		});
	}
	
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
	
	// Inline recurrence functions to avoid import issues
	function generateOccurrences(startDate: Date, rule: RecurrenceRule, maxCount: number): Date[] {
		const occurrences: Date[] = [];
		let currentDate = new Date(startDate);
		
		// For weekly recurrence, we need to find the first valid day
		if (rule.frequency === 'weekly' && rule.daysOfWeek && rule.daysOfWeek.length > 0) {
			const startDay = startDate.getDay();
			if (!rule.daysOfWeek.includes(startDay)) {
				const sortedDays = [...rule.daysOfWeek].sort((a, b) => a - b);
				const nextValidDay = sortedDays.find((day) => day > startDay) || sortedDays[0];
				const daysToAdd = nextValidDay <= startDay ? 7 - startDay + nextValidDay : nextValidDay - startDay;
				currentDate = addDays(startDate, daysToAdd);
			}
		}
		
		for (let i = 0; i < maxCount; i++) {
			if (i > 0) {
				const next = getNextOccurrence(currentDate, rule);
				if (!next) break;
				currentDate = next;
			}
			
			// Check end conditions
			if (rule.endDate && currentDate > rule.endDate) {
				break;
			}
			
			if (rule.count && i >= rule.count) {
				break;
			}
			
			occurrences.push(new Date(currentDate));
		}
		
		return occurrences;
	}
	
	function getNextOccurrence(currentDate: Date, rule: RecurrenceRule): Date | null {
		switch (rule.frequency) {
			case 'daily':
				return addDays(currentDate, rule.interval);
			case 'weekly':
				return addDays(currentDate, rule.interval * 7);
			case 'monthly':
				// Simple implementation - just add months
				const result = new Date(currentDate);
				result.setMonth(result.getMonth() + rule.interval);
				return result;
			case 'yearly':
				const yearResult = new Date(currentDate);
				yearResult.setFullYear(yearResult.getFullYear() + rule.interval);
				return yearResult;
			default:
				return null;
		}
	}
	
	function calculateRecurrenceEndDate(startDate: Date, rule: RecurrenceRule): Date | null {
		if (rule.endDate) {
			return rule.endDate;
		}
		
		if (rule.count) {
			const occurrences = generateOccurrences(startDate, rule, rule.count);
			return occurrences.length > 0 ? occurrences[occurrences.length - 1] : null;
		}
		
		return null;
	}
	
	// Preview and end date calculations
	let previewDates = $derived(value ? generateOccurrences(startDate, value, 3) : []);
	let recurrenceEndDate = $derived(value ? calculateRecurrenceEndDate(startDate, value) : null);
	let endInfo = $derived(value ? getEndInfo(value, startDate, recurrenceEndDate) : '');
	
	function getEndInfo(rule: RecurrenceRule, start: Date, endDate: Date | null): string {
		if (rule.endDate) {
			return `Ends on ${formatDate(rule.endDate)}`;
		} else if (rule.count) {
			return `Ends after ${rule.count} occurrence${rule.count === 1 ? '' : 's'}${endDate ? ` (last: ${formatDate(endDate)})` : ''}`;
		} else {
			return 'Never ends';
		}
	}
	
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
		if (value) {
			// Currently has recurrence - turn it off
			onValueChange?.(null);
		} else {
			// Currently no recurrence - turn it on with default weekly pattern
			onValueChange?.({
				frequency: 'weekly',
				interval: 1
			});
		}
	}
</script>

<!-- Recurrence Toggle -->
<div class="space-y-4">
	<label class="flex items-center space-x-3 {disabled ? 'cursor-not-allowed opacity-50' : 'cursor-pointer'}">
		<input
			type="checkbox"
			checked={hasRecurrence}
			onchange={toggleRecurrence}
			class="sr-only"
			disabled={disabled}
		/>
		<div class="relative">
			<div class="block bg-gray-300 w-14 h-8 rounded-full {hasRecurrence ? 'bg-blue-500' : ''} {disabled ? 'opacity-50' : ''} transition-colors duration-200"></div>
			<div class="absolute left-1 top-1 bg-white w-6 h-6 rounded-full transition-transform duration-200 {hasRecurrence ? 'translate-x-6' : ''} {disabled ? '' : ''}"></div>
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
							disabled={disabled}
							class="px-3 py-2 text-sm border rounded-lg transition-colors
								{frequency === preset.value ? 'bg-blue-500 text-white border-blue-500' : 'hover:bg-gray-50 dark:hover:bg-gray-700'}
								{disabled ? 'opacity-50 cursor-not-allowed' : ''}"
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
						value={interval}
						oninput={(e) => {
							const target = e.target as HTMLInputElement;
							const newInterval = parseInt(target.value) || 1;
							if (value) {
								onValueChange?.({ ...value, interval: newInterval });
							}
						}}
						min="1"
						disabled={disabled}
						class="w-16 px-2 py-1 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white {disabled ? 'opacity-50 cursor-not-allowed' : ''}"
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
									disabled={disabled}
									class="w-8 h-8 rounded-full border text-sm transition-colors
										{value.daysOfWeek?.includes(i) ? 'bg-blue-500 text-white border-blue-500' : 'hover:bg-gray-50 dark:hover:bg-gray-700'}
										{disabled ? 'opacity-50 cursor-not-allowed' : ''}"
							>
								{day.charAt(0)}
							</button>
						{/each}
					</div>
				</div>
			{/if}
			
			<!-- End Conditions -->
			<div class="space-y-2">
				<div class="text-sm font-medium text-gray-700 dark:text-gray-300">Ends</div>
				<div class="space-y-2">
					<label class="flex items-center space-x-2 {disabled ? 'cursor-not-allowed opacity-50' : 'cursor-pointer'}">
						<input
							type="radio"
							name="end-condition"
							checked={!value.endDate && !value.count}
							onchange={() => {
								onValueChange?.({
									...value,
									endDate: undefined,
									count: undefined
								});
							}}
							disabled={disabled}
							class="text-blue-500 focus:ring-blue-500"
						/>
						<span class="text-sm text-gray-700 dark:text-gray-300">Never</span>
					</label>
					
					<label class="flex items-center space-x-2 {disabled ? 'cursor-not-allowed opacity-50' : 'cursor-pointer'}">
						<input
							type="radio"
							name="end-condition"
							checked={!!value.endDate}
							onchange={() => {
								const today = new Date();
								const futureDate = new Date(today);
								futureDate.setDate(today.getDate() + 30); // Default to 30 days from now
								onValueChange?.({
									...value,
									endDate: futureDate,
									count: undefined
								});
							}}
							disabled={disabled}
							class="text-blue-500 focus:ring-blue-500"
						/>
						<span class="text-sm text-gray-700 dark:text-gray-300">On</span>
						<input
							type="date"
							value={value.endDate ? value.endDate.toISOString().split('T')[0] : ''}
							oninput={(e) => {
								const target = e.target as HTMLInputElement;
								if (target.value) {
									onValueChange?.({
										...value,
										endDate: new Date(target.value),
										count: undefined
									});
								}
							}}
							class="ml-2 px-2 py-1 text-sm border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white {disabled || !value.endDate ? 'opacity-50 cursor-not-allowed' : ''}"
							disabled={disabled || !value.endDate}
						/>
					</label>
					
					<label class="flex items-center space-x-2 {disabled ? 'cursor-not-allowed opacity-50' : 'cursor-pointer'}">
						<input
							type="radio"
							name="end-condition"
							checked={!!value.count}
							onchange={() => {
								onValueChange?.({
									...value,
									endDate: undefined,
									count: 10 // Default to 10 occurrences
								});
							}}
							disabled={disabled}
							class="text-blue-500 focus:ring-blue-500"
						/>
						<span class="text-sm text-gray-700 dark:text-gray-300">After</span>
						<input
							type="number"
							value={value.count || ''}
							oninput={(e) => {
								const target = e.target as HTMLInputElement;
								const newCount = parseInt(target.value) || 1;
								onValueChange?.({
									...value,
									endDate: undefined,
									count: newCount
								});
							}}
							min="1"
							class="ml-2 w-16 px-2 py-1 text-sm border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white {disabled || !value.count ? 'opacity-50 cursor-not-allowed' : ''}"
							disabled={disabled || !value.count}
						/>
						<span class="text-sm text-gray-700 dark:text-gray-300">occurrences</span>
					</label>
				</div>
			</div>
		</div>
		
		<!-- Recurrence Preview and Summary -->
		<div class="space-y-3">
			<!-- Recurrence Summary -->
			<div class="p-3 bg-blue-50 dark:bg-blue-900 rounded-lg text-sm text-blue-800 dark:text-blue-200">
				<div class="font-medium mb-1">Recurrence:</div>
				<div>{recurrenceText}</div>
			</div>
			
			<!-- Preview Dates -->
			{#if previewDates.length > 0}
				<div class="p-3 bg-gray-50 dark:bg-gray-800 rounded-lg text-sm">
					<div class="font-medium text-gray-700 dark:text-gray-300 mb-2">Next 3 occurrences:</div>
					<div class="space-y-1">
						{#each previewDates as date, i}
							<div class="flex items-center space-x-2">
								<span class="text-gray-600 dark:text-gray-400">
									{i + 1}.
								</span>
								<span class="text-gray-800 dark:text-gray-200">
									{formatDate(date)}
								</span>
							</div>
						{/each}
					</div>
				</div>
			{/if}
			
			<!-- End Information -->
			{#if endInfo}
				<div class="text-sm text-gray-600 dark:text-gray-400 px-3">
					{endInfo}
				</div>
			{/if}
		</div>
	{/if}
</div>