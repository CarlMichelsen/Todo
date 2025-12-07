<script lang="ts">
	import { Link } from 'svelte-routing';
	import Modal from '$lib/components/modals/Modal.svelte';
	import ConfirmModal from '$lib/components/modals/ConfirmModal.svelte';
	import Card from '$lib/components/Card.svelte';

	// Example of Svelte 5 runes usage
	let count = $state(0);
	let doubled = $derived(count * 2);

	// Modal states
	let showBasicModal = $state(false);
	let showConfirmModal = $state(false);
	let showFormModal = $state(false);
	let formData = $state({ name: '', email: '' });

	function increment() {
		count++;
	}

	function handleConfirmDelete() {
		console.log('Item deleted!');
	}

	function handleFormSubmit() {
		console.log('Form submitted:', formData);
		showFormModal = false;
		formData = { name: '', email: '' };
	}

	// Example effect - runs when count changes
	$effect(() => {
		console.log(`Count is now: ${count}`);
	});
</script>

<div class="max-w-4xl mx-auto px-4 py-8">
	<h1 class="text-5xl font-bold text-orange-600 dark:text-orange-400 mb-8">
		Welcome to Todo App
	</h1>
	
	<!-- Make this card VERY different in light vs dark -->
	<Card variant="blue" class="mb-8">
		<h2 class="text-2xl font-semibold mb-4 text-blue-900 dark:text-blue-200">Svelte 5 Runes Demo</h2>
		<p class="text-lg mb-2 text-blue-800 dark:text-blue-100">Count: <span class="font-mono font-bold">{count}</span></p>
		<p class="text-lg mb-4 text-blue-800 dark:text-blue-100">Doubled: <span class="font-mono font-bold">{doubled}</span></p>
		<button
			onclick={increment}
			class="px-6 py-3 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg font-medium transition-colors shadow-sm hover:shadow-md"
		>
			Increment
		</button>
	</Card>

	<!-- Make navigation VERY different too -->
	<Card variant="yellow">
		<p class="mb-3 text-yellow-900 dark:text-purple-100 font-medium">
			This is the home page. Client-side routing is working!
		</p>
		<p class="text-yellow-900 dark:text-purple-100">
			Try navigating to <Link
				to="/some-route"
				class="text-orange-600 dark:text-orange-300 underline hover:text-orange-700 dark:hover:text-orange-200 font-bold"
			>/some-route</Link> to see the 404 page.
		</p>
	</Card>

	<!-- Modal Examples -->
	<Card class="mt-8">
		<h2 class="text-2xl font-semibold mb-4 text-gray-900 dark:text-gray-100">Modal System Demo</h2>
		<div class="flex flex-wrap gap-3">
			<button
				onclick={() => showBasicModal = true}
				class="px-4 py-2 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg transition-colors"
			>
				Open Basic Modal
			</button>
			<button
				onclick={() => showConfirmModal = true}
				class="px-4 py-2 bg-red-600 hover:bg-red-700 dark:bg-red-500 dark:hover:bg-red-600 text-white rounded-lg transition-colors"
			>
				Open Confirm Dialog
			</button>
			<button
				onclick={() => showFormModal = true}
				class="px-4 py-2 bg-blue-600 hover:bg-blue-700 dark:bg-blue-500 dark:hover:bg-blue-600 text-white rounded-lg transition-colors"
			>
				Open Form Modal
			</button>
		</div>
	</Card>
</div>

<!-- Basic Modal -->
<Modal bind:isOpen={showBasicModal} size="md">
	{#snippet header()}
		<h2 class="text-2xl font-bold text-gray-900 dark:text-gray-100">Welcome!</h2>
	{/snippet}

	{#snippet content()}
		<p class="text-gray-700 dark:text-gray-300 mb-4">
			This is a basic modal dialog using the native HTML dialog element.
		</p>
		<p class="text-gray-700 dark:text-gray-300">
			Try pressing ESC to close it, or click the backdrop!
		</p>
	{/snippet}

	{#snippet footer()}
		<div class="flex justify-end">
			<button
				onclick={() => showBasicModal = false}
				class="px-4 py-2 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg transition-colors"
			>
				Got it!
			</button>
		</div>
	{/snippet}
</Modal>

<!-- Confirm Modal -->
<ConfirmModal
	bind:isOpen={showConfirmModal}
	title="Confirm Deletion"
	message="Are you sure you want to delete this item? This action cannot be undone."
	confirmText="Delete"
	cancelText="Cancel"
	onConfirm={handleConfirmDelete}
/>

<!-- Form Modal -->
<Modal bind:isOpen={showFormModal} size="md">
	{#snippet header()}
		<h2 class="text-2xl font-bold text-gray-900 dark:text-gray-100">Add New Item</h2>
	{/snippet}

	{#snippet content()}
		<form onsubmit={(e) => { e.preventDefault(); handleFormSubmit(); }} class="space-y-4">
			<div>
				<label for="name" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
					Name
				</label>
				<input
					id="name"
					type="text"
					bind:value={formData.name}
					class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400"
					required
				/>
			</div>
			<div>
				<label for="email" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
					Email
				</label>
				<input
					id="email"
					type="email"
					bind:value={formData.email}
					class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-orange-500 dark:focus:ring-orange-400"
					required
				/>
			</div>
		</form>
	{/snippet}

	{#snippet footer()}
		<div class="flex justify-end gap-3">
			<button
				onclick={() => showFormModal = false}
				class="px-4 py-2 bg-gray-200 hover:bg-gray-300 dark:bg-gray-700 dark:hover:bg-gray-600 text-gray-900 dark:text-gray-100 rounded-lg transition-colors"
			>
				Cancel
			</button>
			<button
				onclick={handleFormSubmit}
				class="px-4 py-2 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg transition-colors"
			>
				Submit
			</button>
		</div>
	{/snippet}
</Modal>
