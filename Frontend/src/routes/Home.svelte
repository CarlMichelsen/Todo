<script lang="ts">
	import { Link } from 'svelte-routing';
	import Card from '$lib/components/Card.svelte';
	import { userStore } from '$lib/stores/user';
</script>

<div class="max-w-4xl mx-auto px-4 py-8">
	<!-- Hero Section -->
	<div class="text-center mb-12">
		<h1 class="text-5xl font-bold text-orange-600 dark:text-orange-400 mb-4">
			Welcome to Todo
		</h1>
		<p class="text-xl text-gray-600 dark:text-gray-400">
			Organize your tasks efficiently and boost your productivity
		</p>
	</div>

	<!-- Quick Access Grid -->
	<div class="grid md:grid-cols-2 gap-6 mb-6">
		<!-- Features Card -->
		<Card variant="blue">
			<h2 class="text-2xl font-semibold mb-4 text-blue-900 dark:text-blue-200">
				Features
			</h2>
			<ul class="space-y-2 text-blue-800 dark:text-blue-100">
				<li>✓ Intuitive task management</li>
				<li>✓ User authentication and profiles</li>
				<li>✓ Dark mode support</li>
				<li>✓ Modern, responsive design</li>
			</ul>
		</Card>

		<!-- Calendar Card -->
		<Card variant="purple">
			<h2 class="text-2xl font-semibold mb-4 text-purple-900 dark:text-purple-200">
				Calendar
			</h2>
			<p class="text-purple-800 dark:text-purple-100 mb-4">
				View your weekly schedule and manage your tasks by day.
			</p>
			<Link
				to="/calendar"
				class="inline-block px-6 py-3 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg font-medium transition-colors"
			>
				Open Calendar
			</Link>
		</Card>
	</div>

	<!-- Conditional CTA based on auth state -->
	{#if $userStore.state === 'authenticated' && $userStore.user}
		<Card>
			<h2 class="text-2xl font-semibold mb-3 text-gray-900 dark:text-gray-100">
				Welcome back, {$userStore.user.username}!
			</h2>
			<p class="text-gray-600 dark:text-gray-400 mb-4">
				Ready to manage your tasks?
			</p>
			<Link
				to="/profile"
				class="inline-block px-6 py-3 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg font-medium transition-colors"
			>
				View Profile
			</Link>
		</Card>
	{:else}
		<Card>
			<h2 class="text-2xl font-semibold mb-3 text-gray-900 dark:text-gray-100">
				Get Started
			</h2>
			<p class="text-gray-600 dark:text-gray-400 mb-4">
				Sign in to start managing your tasks and boost your productivity.
			</p>
			<p class="text-sm text-gray-500 dark:text-gray-500">
				Click the login button in the header to get started.
			</p>
		</Card>
	{/if}
</div>
