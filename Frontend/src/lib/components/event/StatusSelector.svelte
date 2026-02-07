<script lang="ts">
	import type { EventStatus } from '$lib/types/calendar';
	
	interface Props {
		value: EventStatus;
		onValueChange?: (status: EventStatus) => void;
		canEdit?: boolean;
	}
	
	let {
		value,
		onValueChange,
		canEdit = true
	}: Props = $props();
	
	const statusOptions: { value: EventStatus; label: string; color: string }[] = [
		{ value: 'confirmed', label: 'Confirmed', color: 'green' },
		{ value: 'tentative', label: 'Tentative', color: 'yellow' },
		{ value: 'cancelled', label: 'Cancelled', color: 'red' },
		{ value: 'pending', label: 'Pending', color: 'blue' }
	];
	
	let statusConfig = $derived(statusOptions.find(s => s.value === value));
</script>

{#if canEdit}
	<div class="space-y-2">
		<label for="event-status" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Event Status</label>
		<select 
			id="event-status"
			bind:value 
			onchange={(e) => {
				const target = e.target as HTMLSelectElement;
				if (target?.value) {
					onValueChange?.(target.value as EventStatus);
				}
			}}
			class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
		>
			{#each statusOptions as option}
				<option value={option.value}>{option.label}</option>
			{/each}
		</select>
	</div>
{:else if statusConfig}
	<div class="inline-flex items-center space-x-1 px-2 py-1 rounded-full text-xs font-medium
		bg-{statusConfig.color}-100 text-{statusConfig.color}-800 dark:bg-{statusConfig.color}-900 dark:text-{statusConfig.color}-200"
	>
		<div class="w-2 h-2 rounded-full bg-{statusConfig.color}-500"></div>
		<span>{statusConfig.label}</span>
	</div>
{/if}