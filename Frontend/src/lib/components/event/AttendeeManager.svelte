<script lang="ts">
	import type { Attendee } from '$lib/types/calendar';
	
	interface Props {
		value: Attendee[];
		onValueChange?: (attendees: Attendee[]) => void;
	}
	
	let {
		value = [],
		onValueChange
	}: Props = $props();
	
	let newEmail = $state('');
	let errors = $state<Record<string, string>>({});
	
	function isValidEmail(email: string): boolean {
		const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
		return emailRegex.test(email);
	}
	
	function addAttendee() {
		const email = newEmail.trim();
		if (!email) {
			errors.add = 'Email is required';
			return;
		}
		
		if (!isValidEmail(email)) {
			errors.add = 'Please enter a valid email address';
			return;
		}
		
		if (value.some(a => a.email === email)) {
			errors.add = 'This email is already added';
			return;
		}
		
		onValueChange?.([...value, { email, commonName: undefined, isOrganizer: false }]);
		newEmail = '';
		errors = {};
	}
	
	function removeAttendee(email: string) {
		onValueChange?.(value.filter(a => a.email !== email));
	}
	
	function handleKeydown(e: KeyboardEvent) {
		if (e.key === 'Enter') {
			e.preventDefault();
			addAttendee();
		}
	}
</script>

<div class="space-y-3">
	<label for="attendee-email" class="block text-sm font-medium text-gray-700 dark:text-gray-300">Attendees</label>
	
	<!-- Add Attendee Input -->
	<div class="flex space-x-2">
		<input
			id="attendee-email"
			type="email"
			bind:value={newEmail}
			onkeydown={handleKeydown}
			placeholder="Add attendee by email..."
			class="flex-1 px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
		/>
		<button
			type="button"
			onclick={addAttendee}
			class="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-500"
		>
			Add
		</button>
	</div>
	
	{#if errors.add}
		<div class="text-sm text-red-600">{errors.add}</div>
	{/if}
	
	<!-- Selected Attendees -->
	<div class="flex flex-wrap gap-2">
		{#each value as attendee (attendee.email)}
			<div class="inline-flex items-center space-x-2 px-3 py-1 bg-blue-100 text-blue-800 rounded-full">
				<span class="w-6 h-6 rounded-full bg-blue-500 flex items-center justify-center text-white text-xs">
					{attendee.commonName?.charAt(0) || attendee.email.charAt(0)}
				</span>
				<span class="text-sm">
					{attendee.commonName || attendee.email}
					{attendee.isOrganizer && ' (Organizer)'}
				</span>
				{#if !attendee.isOrganizer}
					<button
						type="button"
						onclick={() => removeAttendee(attendee.email)}
						class="text-blue-600 hover:text-blue-800 ml-1"
						title="Remove attendee"
					>
						×
					</button>
				{/if}
			</div>
		{/each}
	</div>
</div>