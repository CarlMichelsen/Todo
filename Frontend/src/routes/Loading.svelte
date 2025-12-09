<script lang="ts">
	import { darkMode } from '$lib/stores/darkMode';
</script>

<div class="min-h-screen flex items-center justify-center bg-white dark:bg-gray-900 transition-colors">
	<div class="text-center">
		<!-- Spinner -->
		<div class="inline-block relative w-20 h-20 mb-6">
			<div
				class="absolute border-4 border-orange-200 dark:border-orange-900 rounded-full w-20 h-20"
			></div>
			<div
				class="absolute border-4 border-orange-600 dark:border-orange-400 border-t-transparent rounded-full w-20 h-20 animate-spin"
			></div>
		</div>

		<!-- Loading text -->
		<h2 class="text-2xl font-semibold text-gray-900 dark:text-gray-100 mb-2">Loading</h2>
		<p class="text-gray-600 dark:text-gray-400">Checking authentication...</p>
	</div>
</div>
