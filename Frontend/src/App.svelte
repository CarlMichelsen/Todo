<script lang="ts">
	import Router from 'svelte-spa-router';
	import Header from '$lib/components/Header.svelte';
	import Home from './routes/Home.svelte';
	import NotFound from './routes/NotFound.svelte';

	// Define routes using svelte-spa-router
	const routes = {
		'/': Home,
		'*': NotFound
	};
</script>

<div class="min-h-screen flex flex-col bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100 transition-colors">
	<Header />
	<main class="flex-1">
		<Router {routes} />
	</main>
</div>
