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
		default: 'bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700',
		blue: 'bg-blue-100 dark:bg-slate-800 border-4 border-blue-300 dark:border-slate-600 shadow-lg',
		yellow: 'bg-yellow-100 dark:bg-purple-900 border-4 border-yellow-400 dark:border-purple-600',
		purple: 'bg-purple-100 dark:bg-purple-900 border-4 border-purple-300 dark:border-purple-600'
	};

	const baseClasses = 'rounded-lg p-6';
	const fullClasses = `${baseClasses} ${variantClasses[variant]} ${className}`;
</script>

<div class={fullClasses}>
	{@render children()}
</div>
