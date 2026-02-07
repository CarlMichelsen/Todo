<script lang="ts">
	interface Props {
		value?: string;
		onValueChange?: (location: string) => void;
		placeholder?: string;
	}
	
	let {
		value = '',
		onValueChange,
		placeholder = 'Add location (optional)'
	}: Props = $props();
</script>

<div class="space-y-2">
	<label for="event-location" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Location</label>
	<div class="relative">
		<input
			id="event-location"
			type="text"
			bind:value
			oninput={(e) => {
				const target = e.target as HTMLInputElement;
				if (target?.value !== undefined) {
					onValueChange?.(target.value);
				}
			}}
			placeholder={placeholder}
			class="w-full px-3 py-2 pr-10 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
		/>
		<div class="absolute right-2 top-2 text-gray-400 pointer-events-none">
			📍
		</div>
	</div>
</div>