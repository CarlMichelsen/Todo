<script lang="ts">
	import { type Snippet } from 'svelte';

	interface Props {
		trigger: Snippet;
		content: Snippet;
		align?: 'left' | 'right';
	}

	let { trigger, content, align = 'right' }: Props = $props();

	let isOpen = $state(false);
	let dropdownRef: HTMLDivElement;

	function toggleDropdown(event: MouseEvent) {
		event.stopPropagation();
		isOpen = !isOpen;
	}

	function handleKeydown(event: KeyboardEvent) {
		if (event.key === 'Enter' || event.key === ' ') {
			event.preventDefault();
			isOpen = !isOpen;
		}
	}

	function handleClickOutside(event: MouseEvent) {
		if (dropdownRef && !dropdownRef.contains(event.target as Node)) {
			isOpen = false;
		}
	}

	$effect(() => {
		if (isOpen) {
			// Use requestAnimationFrame for better timing control than setTimeout
			const rafId = requestAnimationFrame(() => {
				document.addEventListener('click', handleClickOutside);
			});

			return () => {
				cancelAnimationFrame(rafId);
				document.removeEventListener('click', handleClickOutside);
			};
		}
	});
</script>

<div class="relative" bind:this={dropdownRef}>
	<!-- Trigger -->
	<div onclick={toggleDropdown} onkeydown={handleKeydown} role="button" tabindex="0">
		{@render trigger()}
	</div>

	<!-- Dropdown Content -->
	{#if isOpen}
		<div
			class="absolute top-full mt-2 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg shadow-lg overflow-hidden min-w-[160px] z-50"
			class:right-0={align === 'right'}
			class:left-0={align === 'left'}
		>
			{@render content()}
		</div>
	{/if}
</div>
