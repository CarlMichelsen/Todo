<script lang="ts">
	import { Link } from 'svelte-routing';

	// Parse the error query parameter from URL
	let errorMessage = $derived(
		new URLSearchParams(window.location.search).get('error') ||
		'An unexpected error occurred. Please try again.'
	);
</script>

<div class="max-w-2xl mx-auto px-4 py-16 text-center">
	<h1 class="text-6xl font-bold text-red-600 dark:text-red-500 mb-6">
		Error
	</h1>
	<h2 class="text-3xl font-semibold mb-4">Something went wrong</h2>
	<p class="text-xl mb-8 text-gray-600 dark:text-gray-400">
		{errorMessage}
	</p>
	<Link
		to="/"
		class="inline-block px-6 py-3 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg font-medium transition-colors shadow-sm hover:shadow-md"
	>
		Go back home
	</Link>
</div>
