<script lang="ts">
	import { Link } from 'svelte-routing';
	import LoginButton from '$lib/components/LoginButton.svelte';
</script>

<div class="min-h-[60vh] flex items-center justify-center px-4">
	<div class="text-center max-w-md">
		<div class="mb-8">
			<!-- Lock icon -->
			<svg
				class="w-24 h-24 mx-auto text-orange-500 dark:text-orange-400"
				fill="none"
				stroke="currentColor"
				viewBox="0 0 24 24"
			>
				<path
					stroke-linecap="round"
					stroke-linejoin="round"
					stroke-width="2"
					d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
				/>
			</svg>
		</div>

		<h1 class="text-4xl font-bold text-gray-900 dark:text-gray-100 mb-4">
			Authentication Required
		</h1>

		<p class="text-lg text-gray-600 dark:text-gray-400 mb-8">
			You need to be logged in to access this page.
		</p>

		<div class="flex flex-col sm:flex-row gap-4 justify-center">
			<!-- Login button with large size -->
			<LoginButton size="lg" />

			<!-- Back to home link -->
			<Link
				to="/"
				class="px-6 py-3 bg-gray-200 dark:bg-gray-700 hover:bg-gray-300 dark:hover:bg-gray-600 text-gray-900 dark:text-gray-100 rounded-lg font-medium transition-colors"
			>
				Go to Home
			</Link>
		</div>
	</div>
</div>
