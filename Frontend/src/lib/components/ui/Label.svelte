<script lang="ts">
	import type { Snippet } from 'svelte';

	interface Props {
		for: string;
		required?: boolean;
		class?: string;
		children: Snippet;
	}

	let { for: htmlFor, required = false, class: className, children }: Props = $props();

	const baseClasses = 'block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1';
	const classes = $derived(`${baseClasses} ${className || ''}`);
</script>

<label for={htmlFor} class={classes}>
	{@render children()}
	{#if required}<span class="text-red-600 dark:text-red-400 ml-1">*</span>{/if}
</label>
