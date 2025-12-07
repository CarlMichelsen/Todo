<script lang="ts">
	import { darkMode } from '$lib/stores/darkMode';
	import { navigate } from 'svelte-routing';
	import Profile from './Profile.svelte';
	import Dropdown from './Dropdown.svelte';

	function toggleDarkMode() {
		darkMode.update((value: boolean) => !value);
	}

	function goToHome() {
		navigate('/');
	}
</script>

<header class="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 transition-colors">
	<div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
		<div class="flex justify-between items-center h-14">
			<!-- Logo/Title -->
			<button onclick={goToHome} class="flex items-center hover:opacity-80 transition-opacity">
				<h1 class="text-xl font-bold text-orange-600 dark:text-orange-400">
					Todo
				</h1>
			</button>

			<!-- Right side: Dev tools + Dark mode toggle + Profile -->
			<div class="flex items-center gap-3">
				<!-- Developer Tools (dev only) -->
				{#if import.meta.env.DEV}
					<Dropdown align="right">
						{#snippet trigger()}
							<button
								class="p-2 rounded-lg bg-gray-100 dark:bg-gray-700 hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors"
								aria-label="Developer tools"
								title="Developer tools"
							>
								🔧
							</button>
						{/snippet}

						{#snippet content()}
							<div class="py-2 min-w-[180px]">
								<!-- Header -->
								<div class="px-4 py-2 border-b border-gray-200 dark:border-gray-700">
									<p class="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase">
										Dev Tools
									</p>
								</div>

								<!-- Demo Page Link -->
								<button
									onclick={() => navigate('/demo')}
									class="w-full px-4 py-2 text-left text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors text-sm flex items-center gap-2"
								>
									<span>🎨</span>
									<span>Component Demo</span>
								</button>

								<!-- Error Page Link -->
								<button
									onclick={() => navigate('/error')}
									class="w-full px-4 py-2 text-left text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors text-sm flex items-center gap-2"
								>
									<span>⚠️</span>
									<span>Error Page</span>
								</button>

								<!-- 404 Page Link -->
								<button
									onclick={() => navigate('/404')}
									class="w-full px-4 py-2 text-left text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors text-sm flex items-center gap-2"
								>
									<span>🔍</span>
									<span>404 Page</span>
								</button>

								<!-- API Info -->
								<div class="px-4 py-2 border-t border-gray-200 dark:border-gray-700 mt-2">
									<p class="text-xs text-gray-500 dark:text-gray-400">
										API: {import.meta.env.VITE_API_URL || 'localhost:5035'}
									</p>
								</div>
							</div>
						{/snippet}
					</Dropdown>
				{/if}

				<!-- Dark Mode Toggle -->
				<button
					onclick={toggleDarkMode}
					class="p-2 rounded-lg bg-gray-100 dark:bg-gray-700 hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors"
					aria-label="Toggle dark mode"
					title={$darkMode ? 'Switch to light mode' : 'Switch to dark mode'}
				>
					{$darkMode ? '☀️' : '🌙'}
				</button>

				<!-- Profile -->
				<Profile />
			</div>
		</div>
	</div>
</header>
