<script lang="ts">
	import { type Snippet } from 'svelte';

	interface Props {
		isOpen?: boolean;
		size?: 'sm' | 'md' | 'lg' | 'full';
		showClose?: boolean;
		closeOnBackdrop?: boolean;
		onClose?: () => void;
		header?: Snippet;
		content: Snippet;
		footer?: Snippet;
	}

	let {
		isOpen = $bindable(false),
		size = 'md',
		showClose = true,
		closeOnBackdrop = true,
		onClose,
		header,
		content,
		footer
	}: Props = $props();

	let dialogElement: HTMLDialogElement;

	// Reactive effect to show/hide dialog
	$effect(() => {
		if (!dialogElement) return;

		if (isOpen && !dialogElement.open) {
			dialogElement.showModal();
		} else if (!isOpen && dialogElement.open) {
			dialogElement.close();
		}
	});

	function handleClose() {
		isOpen = false;
		onClose?.();
	}

	function handleBackdropClick(event: MouseEvent) {
		if (!closeOnBackdrop) return;

		// Only close if clicking directly on the dialog element (the backdrop)
		// Clicks on the modal content (inner div) won't trigger this
		if (event.target === dialogElement) {
			handleClose();
		}
	}

	// Handle native dialog close event (ESC key)
	function handleDialogClose() {
		handleClose();
	}
</script>

<dialog
	bind:this={dialogElement}
	onpointerdown={handleBackdropClick}
	onclose={handleDialogClose}
	class="fixed inset-0 m-auto p-0 border-0 bg-transparent max-w-none max-h-none backdrop:bg-black/50 backdrop:fixed backdrop:inset-0 backdrop:dark:bg-black/70 backdrop:animate-in backdrop:fade-in-0 backdrop:duration-200 open:animate-in open:fade-in-0 open:slide-in-from-bottom-4 open:duration-300"
	class:max-w-sm={size === 'sm'}
	class:max-w-2xl={size === 'md'}
	class:max-w-4xl={size === 'lg'}
	class:w-[95vw]={size === 'full'}
	class:h-[95vh]={size === 'full'}
	aria-modal="true"
>
	<div
		class="relative max-h-[90vh] overflow-y-auto bg-white dark:bg-gray-800 rounded-lg shadow-xl border border-gray-200 dark:border-gray-700"
		class:h-full={size === 'full'}
	>
		<!-- Close button -->
		{#if showClose}
			<button
				onclick={handleClose}
				class="absolute top-4 right-4 p-2 rounded-lg text-gray-500 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700 hover:text-gray-900 dark:hover:text-gray-100 transition-colors z-10"
				aria-label="Close modal"
				type="button"
			>
				<!-- X icon (SVG) -->
				<svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
					<path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M6 18L18 6M6 6l12 12"
					/>
				</svg>
			</button>
		{/if}

		<!-- Header -->
		{#if header}
			<div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
				{@render header()}
			</div>
		{/if}

		<!-- Content -->
		<div class="px-6 py-4 text-gray-700 dark:text-gray-300">
			{@render content()}
		</div>

		<!-- Footer -->
		{#if footer}
			<div class="px-6 py-4 border-t border-gray-200 dark:border-gray-700">
				{@render footer()}
			</div>
		{/if}
	</div>
</dialog>
