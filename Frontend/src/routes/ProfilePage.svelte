<script lang="ts">
	import { userStore } from '$lib/stores/user';
	import Card from '$lib/components/Card.svelte';

	function formatDate(dateString: string): string {
		return new Date(dateString).toLocaleString();
	}
</script>

<!-- ProtectedRoute guarantees user is authenticated -->
<div class="max-w-4xl mx-auto px-4 py-8">
		<h1 class="text-4xl font-bold text-orange-600 dark:text-orange-400 mb-8">
			User Profile
		</h1>

		<!-- Profile Image Section -->
		<Card class="mb-6">
			<div class="flex items-center gap-6">
				<img
					src={$userStore.user!.profileLarge || $userStore.user!.profileMedium || $userStore.user!.profile}
					alt={$userStore.user!.userName}
					class="w-32 h-32 rounded-full object-cover border-4 border-orange-500 dark:border-orange-400"
				/>
				<div>
					<h2 class="text-3xl font-bold text-gray-900 dark:text-gray-100 mb-2">
						{$userStore.user!.userName}
					</h2>
					<p class="text-lg text-gray-600 dark:text-gray-400">
						{$userStore.user!.email}
					</p>
				</div>
			</div>
		</Card>

		<!-- Personal Information -->
		<Card class="mb-6">
			<h3 class="text-2xl font-semibold text-gray-900 dark:text-gray-100 mb-4 border-b border-gray-200 dark:border-gray-700 pb-2">
				Personal Information
			</h3>
			<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
				<div>
					<p class="text-sm font-medium text-gray-500 dark:text-gray-400">User ID</p>
					<p class="text-base text-gray-900 dark:text-gray-100 font-mono">{$userStore.user!.userId}</p>
				</div>
				<div>
					<p class="text-sm font-medium text-gray-500 dark:text-gray-400">Username</p>
					<p class="text-base text-gray-900 dark:text-gray-100">{$userStore.user!.userName}</p>
				</div>
				<div>
					<p class="text-sm font-medium text-gray-500 dark:text-gray-400">Email</p>
					<p class="text-base text-gray-900 dark:text-gray-100">{$userStore.user!.email}</p>
				</div>
				<div>
					<p class="text-sm font-medium text-gray-500 dark:text-gray-400">Selected Calendar ID</p>
					<p class="text-base text-gray-900 dark:text-gray-100 font-mono">{$userStore.user!.selectedCalendarId}</p>
				</div>
			</div>
		</Card>

		<!-- Authentication Information -->
		<Card class="mb-6">
			<h3 class="text-2xl font-semibold text-gray-900 dark:text-gray-100 mb-4 border-b border-gray-200 dark:border-gray-700 pb-2">
				Authentication Information
			</h3>
			<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
				<div>
					<p class="text-sm font-medium text-gray-500 dark:text-gray-400">Provider</p>
					<p class="text-base text-gray-900 dark:text-gray-100">{$userStore.user!.authenticationProvider}</p>
				</div>
				<div>
					<p class="text-sm font-medium text-gray-500 dark:text-gray-400">Provider ID</p>
					<p class="text-base text-gray-900 dark:text-gray-100 font-mono break-all">{$userStore.user!.authenticationProviderId}</p>
				</div>
			</div>
		</Card>

		<!-- Token Information -->
		<Card class="mb-6">
			<h3 class="text-2xl font-semibold text-gray-900 dark:text-gray-100 mb-4 border-b border-gray-200 dark:border-gray-700 pb-2">
				Token Information
			</h3>
			<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
				<div>
					<p class="text-sm font-medium text-gray-500 dark:text-gray-400">Access Token ID</p>
					<p class="text-base text-gray-900 dark:text-gray-100 font-mono break-all">{$userStore.user!.accessTokenId}</p>
				</div>
				<div>
					<p class="text-sm font-medium text-gray-500 dark:text-gray-400">Issued At</p>
					<p class="text-base text-gray-900 dark:text-gray-100">{formatDate($userStore.user!.tokenIssuedAt)}</p>
				</div>
				<div>
					<p class="text-sm font-medium text-gray-500 dark:text-gray-400">Expires At</p>
					<p class="text-base text-gray-900 dark:text-gray-100">{formatDate($userStore.user!.tokenExpiresAt)}</p>
				</div>
			</div>
		</Card>
</div>
