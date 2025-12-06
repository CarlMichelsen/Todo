<script lang="ts">
	import { Link } from 'svelte-routing';

	// Example of Svelte 5 runes usage
	let count = $state(0);
	let doubled = $derived(count * 2);

	function increment() {
		count++;
	}

	// Example effect - runs when count changes
	$effect(() => {
		console.log(`Count is now: ${count}`);
	});
</script>

<div class="max-w-4xl mx-auto px-4 py-8">
	<h1 class="text-5xl font-bold text-orange-600 dark:text-orange-400 mb-8">
		Welcome to Todo App
	</h1>
	
	<!-- Make this card VERY different in light vs dark -->
	<div class="bg-blue-100 dark:bg-slate-800 rounded-lg p-6 mb-8 shadow-lg border-4 border-blue-300 dark:border-slate-600">
		<h2 class="text-2xl font-semibold mb-4 text-blue-900 dark:text-blue-200">Svelte 5 Runes Demo</h2>
		<p class="text-lg mb-2 text-blue-800 dark:text-blue-100">Count: <span class="font-mono font-bold">{count}</span></p>
		<p class="text-lg mb-4 text-blue-800 dark:text-blue-100">Doubled: <span class="font-mono font-bold">{doubled}</span></p>
		<button
			onclick={increment}
			class="px-6 py-3 bg-orange-600 hover:bg-orange-700 dark:bg-orange-500 dark:hover:bg-orange-600 text-white rounded-lg font-medium transition-colors shadow-sm hover:shadow-md"
		>
			Increment
		</button>
	</div>

	<!-- Make navigation VERY different too -->
	<nav class="bg-yellow-100 dark:bg-purple-900 rounded-lg p-6 border-4 border-yellow-400 dark:border-purple-600">
		<p class="mb-3 text-yellow-900 dark:text-purple-100 font-medium">
			This is the home page. Client-side routing is working!
		</p>
		<p class="text-yellow-900 dark:text-purple-100">
			Try navigating to <Link
				to="/some-route"
				class="text-orange-600 dark:text-orange-300 underline hover:text-orange-700 dark:hover:text-orange-200 font-bold"
			>/some-route</Link> to see the 404 page.
		</p>
	</nav>
</div>
