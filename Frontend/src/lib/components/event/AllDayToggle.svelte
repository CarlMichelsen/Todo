<script lang="ts">
	interface Props {
		value: boolean;
		onValueChange?: (isAllDay: boolean) => void;
	}
	
	let {
		value,
		onValueChange
	}: Props = $props();
</script>

<label class="flex items-center space-x-3 cursor-pointer">
	<input
		type="checkbox"
		bind:checked={value}
		onchange={(e) => {
			const target = e.target as HTMLInputElement;
			if (target?.checked !== undefined) {
				onValueChange?.(target.checked);
			}
		}}
		class="sr-only"
	/>
	<div class="relative">
		<div class="block bg-gray-300 w-14 h-8 rounded-full {value ? 'bg-blue-500' : ''} transition-colors duration-200"></div>
		<div class="absolute left-1 top-1 bg-white w-6 h-6 rounded-full transition-transform duration-200 {value ? 'translate-x-6' : ''}"></div>
	</div>
	<span class="text-sm font-medium text-gray-700 dark:text-gray-300">All-day event</span>
</label>