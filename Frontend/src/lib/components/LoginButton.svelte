<script lang="ts">
	import { LoginClient, type AuthProvider } from '$lib/utils/loginClient';

	const loginClient = new LoginClient();
	const providers = loginClient.getAvailableProviders();

	let isOpen = $state(false);

	function toggleDropdown() {
		isOpen = !isOpen;
	}

	function handleLogin(provider: AuthProvider) {
		isOpen = false;
		loginClient.login({ provider });
	}

	function handleClickOutside(event: MouseEvent) {
		const target = event.target as HTMLElement;
		if (!target.closest('.login-dropdown')) {
			isOpen = false;
		}
	}

	$effect(() => {
		if (isOpen) {
			document.addEventListener('click', handleClickOutside);
			return () => {
				document.removeEventListener('click', handleClickOutside);
			};
		}
	});
</script>

<div class="relative login-dropdown">
	<button
		onclick={toggleDropdown}
		class="px-4 py-2 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg font-medium transition-colors text-sm"
	>
		Login
	</button>

	{#if isOpen}
		<div
			class="absolute top-full mt-2 right-0 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg shadow-lg overflow-hidden min-w-[160px] z-50"
		>
			{#each providers as provider}
				<button
					onclick={() => handleLogin(provider)}
					class="w-full px-4 py-2 text-left text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors text-sm"
				>
					{provider}
				</button>
			{/each}
		</div>
	{/if}
</div>
