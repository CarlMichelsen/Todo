<script lang="ts">
	import { userStore } from '$lib/stores/user';
	import Unauthorized from '../../routes/Unauthorized.svelte';
	import type { Snippet } from 'svelte';

	interface Props {
		/**
		 * Content to render when authenticated
		 */
		children: Snippet;
	}

	let { children }: Props = $props();

	// Derive whether we should show the protected content
	let isAuthenticated = $derived($userStore.state === 'authenticated');
	let isCheckingAuth = $derived(
		$userStore.state === 'pending'
	);
</script>

{#if isAuthenticated}
	<!-- User is authenticated, show protected content -->
	{@render children()}
{:else if isCheckingAuth}
	<!-- Still checking authentication, show loading state -->
	<div class="max-w-4xl mx-auto px-4 py-8">
		<p class="text-gray-600 dark:text-gray-400">Checking authentication...</p>
	</div>
{:else}
	<!-- Not authenticated, show unauthorized page -->
	<Unauthorized />
{/if}
