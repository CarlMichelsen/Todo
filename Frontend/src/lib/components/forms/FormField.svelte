<script lang="ts">
	import type { Snippet } from 'svelte';
	import Label from '$lib/components/ui/Label.svelte';
	import ErrorText from '$lib/components/ui/ErrorText.svelte';

	interface Props {
		label: string;
		error?: string;
		required?: boolean;
		htmlFor: string;
		class?: string;
		children: Snippet;
	}

	let { label, error, required = false, htmlFor, class: className, children }: Props = $props();

	const containerClasses = $derived(className || '');
</script>

<div class={containerClasses}>
	<Label for={htmlFor} {required}>
		{label}
	</Label>
	{@render children()}
	<ErrorText {error} />
</div>
