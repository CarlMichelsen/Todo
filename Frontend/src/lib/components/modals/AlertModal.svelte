<script lang="ts">
	import Modal from './Modal.svelte';

	interface Props {
		isOpen?: boolean;
		title: string;
		message: string;
		buttonText?: string;
		onClose?: () => void;
	}

	let {
		isOpen = $bindable(false),
		title,
		message,
		buttonText = 'OK',
		onClose
	}: Props = $props();

	function handleClose() {
		isOpen = false;
		onClose?.();
	}
</script>

<Modal bind:isOpen size="sm" onClose={handleClose} showClose={false}>
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
		<div class="flex justify-center">
			<button
				onclick={handleClose}
				class="px-6 py-2 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg transition-colors font-medium"
			>
				{buttonText}
			</button>
		</div>
	{/snippet}
</Modal>
