<script lang="ts">
	import Modal from './Modal.svelte';
	import { type Snippet } from 'svelte';

	interface Props {
		isOpen?: boolean;
		title: string;
		size?: 'sm' | 'md' | 'lg' | 'full';
		submitText?: string;
		cancelText?: string;
		onSubmit: () => void;
		onCancel?: () => void;
		formContent: Snippet;
	}

	let {
		isOpen = $bindable(false),
		title,
		size = 'md',
		submitText = 'Submit',
		cancelText = 'Cancel',
		onSubmit,
		onCancel,
		formContent
	}: Props = $props();

	function handleSubmit() {
		onSubmit();
		isOpen = false;
	}

	function handleCancel() {
		onCancel?.();
		isOpen = false;
	}
</script>

<Modal bind:isOpen {size} onClose={handleCancel}>
	{#snippet header()}
		<h2 class="text-2xl font-bold text-gray-900 dark:text-gray-100">
			{title}
		</h2>
	{/snippet}

	{#snippet content()}
		{@render formContent()}
	{/snippet}

	{#snippet footer()}
		<div class="flex justify-end gap-3">
			<button
				onclick={handleCancel}
				type="button"
				class="px-4 py-2 bg-gray-200 hover:bg-gray-300 dark:bg-gray-700 dark:hover:bg-gray-600 text-gray-900 dark:text-gray-100 rounded-lg transition-colors"
			>
				{cancelText}
			</button>
			<button
				onclick={handleSubmit}
				type="button"
				class="px-4 py-2 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg transition-colors"
			>
				{submitText}
			</button>
		</div>
	{/snippet}
</Modal>
