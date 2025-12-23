<script lang="ts">
	import type { Toast } from '$lib/stores/toast';

	interface Props {
		toast: Toast;
		onDismiss: () => void;
	}

	let { toast, onDismiss }: Props = $props();

	// Icon and color mappings
	const config = {
		error: {
			icon: '✕',
			bg: 'bg-red-500 dark:bg-red-600',
			border: 'border-red-600 dark:border-red-700',
			text: 'text-white'
		},
		success: {
			icon: '✓',
			bg: 'bg-green-500 dark:bg-green-600',
			border: 'border-green-600 dark:border-green-700',
			text: 'text-white'
		},
		info: {
			icon: 'ℹ',
			bg: 'bg-blue-500 dark:bg-blue-600',
			border: 'border-blue-600 dark:border-blue-700',
			text: 'text-white'
		},
		warning: {
			icon: '⚠',
			bg: 'bg-orange-500 dark:bg-orange-600',
			border: 'border-orange-600 dark:border-orange-700',
			text: 'text-white'
		}
	};

	const style = $derived(config[toast.type]);
</script>

<div
	class="relative flex items-start gap-3 p-4 rounded-lg shadow-lg border-2 min-w-[300px] max-w-[400px] animate-slide-in {style.bg} {style.border}"
	role="alert"
>
	<!-- Icon -->
	<div class="flex-shrink-0 text-2xl {style.text}">
		{style.icon}
	</div>

	<!-- Message -->
	<div class="flex-1 {style.text}">
		<p class="text-sm font-medium break-words">{toast.message}</p>
	</div>

	<!-- Close Button -->
	<button
		onclick={onDismiss}
		class="flex-shrink-0 {style.text} hover:opacity-75 transition-opacity"
		aria-label="Dismiss notification"
	>
		<svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
			<path
				fill-rule="evenodd"
				d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
				clip-rule="evenodd"
			/>
		</svg>
	</button>

	<!-- Progress Bar (if duration > 0) -->
	{#if toast.duration && toast.duration > 0}
		<div class="absolute bottom-0 left-0 right-0 h-1 bg-white/30 rounded-b-lg overflow-hidden">
			<div
				class="h-full bg-white/60 animate-shrink"
				style="animation-duration: {toast.duration}ms"
			></div>
		</div>
	{/if}
</div>

<style>
	@keyframes slide-in {
		from {
			transform: translateX(100%);
			opacity: 0;
		}
		to {
			transform: translateX(0);
			opacity: 1;
		}
	}

	@keyframes shrink {
		from {
			width: 100%;
		}
		to {
			width: 0%;
		}
	}

	.animate-slide-in {
		animation: slide-in 0.3s ease-out;
	}

	.animate-shrink {
		animation: shrink linear;
	}
</style>
