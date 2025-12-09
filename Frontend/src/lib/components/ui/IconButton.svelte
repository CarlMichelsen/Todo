<script lang="ts">
	import type { Snippet } from 'svelte';

	interface Props {
		onclick: (event: MouseEvent) => void;
		ariaLabel: string;
		size?: 'sm' | 'md' | 'lg';
		variant?: 'default' | 'primary' | 'danger' | 'ghost';
		disabled?: boolean;
		type?: 'button' | 'submit' | 'reset';
		class?: string;
		children: Snippet;
	}

	let {
		onclick,
		ariaLabel,
		size = 'md',
		variant = 'default',
		disabled = false,
		type = 'button',
		class: className,
		children
	}: Props = $props();

	const baseClasses =
		'inline-flex items-center justify-center rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed';

	const variantClasses = $derived.by(() => {
		switch (variant) {
			case 'primary':
				return 'bg-blue-600 hover:bg-blue-700 text-white focus:ring-blue-500 dark:bg-blue-500 dark:hover:bg-blue-600';
			case 'danger':
				return 'bg-red-600 hover:bg-red-700 text-white focus:ring-red-500 dark:bg-red-500 dark:hover:bg-red-600';
			case 'ghost':
				return 'bg-transparent hover:bg-gray-100 text-gray-700 focus:ring-gray-500 dark:hover:bg-gray-800 dark:text-gray-300';
			case 'default':
			default:
				return 'bg-gray-200 hover:bg-gray-300 text-gray-700 focus:ring-gray-500 dark:bg-gray-700 dark:hover:bg-gray-600 dark:text-gray-300';
		}
	});

	const sizeClasses = $derived.by(() => {
		switch (size) {
			case 'sm':
				return 'p-1 text-sm';
			case 'md':
				return 'p-2 text-base';
			case 'lg':
				return 'p-3 text-lg';
		}
	});

	const classes = $derived(`${baseClasses} ${variantClasses} ${sizeClasses} ${className || ''}`);
</script>

<button {type} class={classes} {disabled} onclick={onclick} aria-label={ariaLabel}>
	{@render children()}
</button>
