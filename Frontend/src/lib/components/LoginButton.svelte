<script lang="ts">
	import { LoginClient, type AuthProvider } from '$lib/utils/loginClient';
	import Dropdown from './Dropdown.svelte';

	const loginClient = new LoginClient();
	const providers = loginClient.getAvailableProviders();

	function handleLogin(provider: AuthProvider) {
		loginClient.login({ provider });
	}
</script>

<Dropdown>
	{#snippet trigger()}
		<button
			class="px-4 py-2 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg font-medium transition-colors text-sm"
		>
			Login
		</button>
	{/snippet}

	{#snippet content()}
		{#each providers as provider}
			<button
				onclick={() => handleLogin(provider)}
				class="w-full px-4 py-2 text-left text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors text-sm"
			>
				{provider}
			</button>
		{/each}
	{/snippet}
</Dropdown>
