<script lang="ts">
	import { type Snippet } from 'svelte';

	interface Props {
		variant?: 'default' | 'blue' | 'yellow' | 'purple';
		class?: string;
		children: Snippet;
	}

	let {
		variant = 'default',
		class: className = '',
		children
	}: Props = $props();

	const variantClasses = {
		default: 'bg-white dark:bg-gray-800 shadow-lg border border-gray-200 dark:border-gray-700',
		blue: 'bg-blue-100 dark:bg-blue-950 border border-blue-300 dark:border-blue-700 shadow-lg',
		yellow: 'bg-yellow-100 dark:bg-purple-900 border border-yellow-400 dark:border-purple-600 shadow-lg',
		purple: 'bg-purple-100 dark:bg-purple-900 border border-purple-300 dark:border-purple-600 shadow-lg'
	};

	const baseClasses = 'rounded-lg p-6';
	const fullClasses = `${baseClasses} ${variantClasses[variant]} ${className}`;
</script>

<div class={fullClasses}>
	{@render children()}
</div>
