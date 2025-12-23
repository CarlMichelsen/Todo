<script lang="ts">
	import { onMount } from 'svelte';
	import { Router, Route } from 'svelte-routing';
	import Header from '$lib/components/Header.svelte';
	import ProtectedRoute from '$lib/components/ProtectedRoute.svelte';
	import Home from './routes/Home.svelte';
	import Demo from './routes/Demo.svelte';
	import ProfilePage from './routes/ProfilePage.svelte';
	import CalendarPage from './routes/CalendarPage.svelte';
	import Error from './routes/Error.svelte';
	import NotFound from './routes/NotFound.svelte';
	import Loading from './routes/Loading.svelte';
	import { userStore } from '$lib/stores/user';
	import { calendarsStore } from '$lib/stores/calendars';

	// Initialize user authentication and calendars on app startup
	onMount(async () => {
		await userStore.initialize();

		// Initialize calendars if user is authenticated
		if ($userStore.state === 'authenticated') {
			await calendarsStore.initialize();
		}
	});
</script>

{#if $userStore.state === 'pending'}
	<Loading />
{:else}
	<div class="min-h-screen flex flex-col bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100 transition-colors">
		<Header />
		<main class="flex-1">
			<Router>
				<Route path="/"><Home /></Route>
				{#if import.meta.env.DEV}
					<Route path="/demo"><Demo /></Route>
				{/if}
				<Route path="/profile">
					<ProtectedRoute>
						<ProfilePage />
					</ProtectedRoute>
				</Route>
				<Route path="/calendar">
					<ProtectedRoute>
						<CalendarPage />
					</ProtectedRoute>
				</Route>
				<Route path="/error"><Error /></Route>
				<Route><NotFound /></Route>
			</Router>
		</main>
	</div>
{/if}
