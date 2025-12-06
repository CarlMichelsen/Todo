<script lang="ts">
	import { Link } from 'svelte-routing';
</script>

<div class="max-w-2xl mx-auto px-4 py-16 text-center">
	<h1 class="text-6xl font-bold text-orange-600 dark:text-orange-500 mb-6">
		404
	</h1>
	<h2 class="text-3xl font-semibold mb-4">Page Not Found</h2>
	<p class="text-xl mb-8 text-gray-600 dark:text-gray-400">
		The page you're looking for doesn't exist.
	</p>
	<Link
		to="/"
		class="inline-block px-6 py-3 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg font-medium transition-colors shadow-sm hover:shadow-md"
	>
		Go back home
	</Link>
</div>
