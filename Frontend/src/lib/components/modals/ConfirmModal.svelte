<script lang="ts">
	import Modal from './Modal.svelte';

	interface Props {
		isOpen?: boolean;
		title: string;
		message: string;
		confirmText?: string;
		cancelText?: string;
		onConfirm: () => void;
		onCancel?: () => void;
	}

	let {
		isOpen = $bindable(false),
		title,
		message,
		confirmText = 'Confirm',
		cancelText = 'Cancel',
		onConfirm,
		onCancel
	}: Props = $props();

	function handleConfirm() {
		onConfirm();
		isOpen = false;
	}

	function handleCancel() {
		onCancel?.();
		isOpen = false;
	}
</script>

<Modal bind:isOpen size="sm" onClose={handleCancel}>
	{#snippet header()}
		<h2 class="text-xl font-bold text-gray-900 dark:text-gray-100">
			{title}
		</h2>
	{/snippet}

	{#snippet content()}
		<p class="text-gray-700 dark:text-gray-300">
			{message}
		</p>
	{/snippet}

	{#snippet footer()}
		<div class="flex justify-end gap-3">
			<button
				onclick={handleCancel}
				class="px-4 py-2 bg-gray-200 hover:bg-gray-300 dark:bg-gray-700 dark:hover:bg-gray-600 text-gray-900 dark:text-gray-100 rounded-lg transition-colors"
			>
				{cancelText}
			</button>
			<button
				onclick={handleConfirm}
				class="px-4 py-2 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg transition-colors"
			>
				{confirmText}
			</button>
		</div>
	{/snippet}
</Modal>
