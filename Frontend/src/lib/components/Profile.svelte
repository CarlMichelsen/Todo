<script lang="ts">
	import { userStore } from '$lib/stores/user';
	import { UserClient } from '$lib/utils/userClient';
	import { navigate } from 'svelte-routing';
	import LoginButton from './LoginButton.svelte';
	import Dropdown from './Dropdown.svelte';

	const userClient = new UserClient();

	function handleViewProfile() {
		navigate('/profile');
	}

	async function handleLogout() {
		await userClient.logout();
		userStore.clearUser();
	}
</script>

{#if $userStore.state === 'authenticated' && $userStore.user}
	<!-- User Profile Display -->
	<div class="flex items-center gap-3">
		<!-- Profile Image -->
		<img
			src={$userStore.user.profileMedium || $userStore.user.profile}
			alt={$userStore.user.username}
			class="w-10 h-10 rounded-full object-cover border-2 border-gray-200 dark:border-gray-700"
		/>

		<!-- Burger Menu Dropdown -->
		<Dropdown>
			{#snippet trigger()}
				<button
					class="p-2 rounded-lg bg-gray-100 dark:bg-gray-700 hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors"
					aria-label="User menu"
				>
					<!-- Burger Icon (three horizontal lines) -->
					<svg
						class="w-5 h-5 text-gray-700 dark:text-gray-200"
						fill="none"
						stroke="currentColor"
						viewBox="0 0 24 24"
					>
						<path
							stroke-linecap="round"
							stroke-linejoin="round"
							stroke-width="2"
							d="M4 6h16M4 12h16M4 18h16"
						/>
					</svg>
				</button>
			{/snippet}

			{#snippet content()}
				<!-- User Info Section -->
				<div class="px-4 py-3 border-b border-gray-200 dark:border-gray-700">
					<p class="text-sm font-medium text-gray-900 dark:text-gray-100">
						{$userStore.user?.username}
					</p>
					<p class="text-xs text-gray-500 dark:text-gray-400 truncate">
						{$userStore.user?.email}
					</p>
				</div>

				<!-- View Profile Link -->
				<button
					onclick={handleViewProfile}
					class="w-full px-4 py-2 text-left text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors text-sm"
				>
					View Profile
				</button>

				<!-- Logout Button -->
				<button
					onclick={handleLogout}
					class="w-full px-4 py-2 text-left text-red-600 dark:text-red-400 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors text-sm font-medium"
				>
					Logout
				</button>
			{/snippet}
		</Dropdown>
	</div>
{:else}
	<!-- Show Login Button when not authenticated -->
	<LoginButton />
{/if}
