<script lang="ts">
	import { darkMode } from '$lib/stores/darkMode';
	import LoginButton from './LoginButton.svelte';

	function toggleDarkMode() {
		darkMode.update((value: boolean) => !value);
	}
</script>

<header class="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 transition-colors">
	<div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
		<div class="flex justify-between items-center h-14">
			<!-- Logo/Title -->
			<div class="flex items-center">
				<h1 class="text-xl font-bold text-orange-600 dark:text-orange-400">
					Todo App
				</h1>
			</div>

			<!-- Right side: Dark mode toggle + Login button -->
			<div class="flex items-center gap-3">
				<!-- Dark Mode Toggle -->
				<button
					onclick={toggleDarkMode}
					class="p-2 rounded-lg bg-gray-100 dark:bg-gray-700 hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors"
					aria-label="Toggle dark mode"
					title={$darkMode ? 'Switch to light mode' : 'Switch to dark mode'}
				>
					{$darkMode ? '☀️' : '🌙'}
				</button>

				<!-- Login Button -->
				<LoginButton />
			</div>
		</div>
	</div>
</header>
