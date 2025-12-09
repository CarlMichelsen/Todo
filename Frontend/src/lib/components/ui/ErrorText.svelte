<script lang="ts">
	interface Props {
		error?: string;
		class?: string;
	}

	let { error, class: className }: Props = $props();

	const baseClasses = 'text-sm text-red-600 dark:text-red-400 mt-1';
	const classes = $derived(`${baseClasses} ${className || ''}`);
</script>

{#if error}
	<p class={classes}>{error}</p>
{/if}
