<script lang="ts">
	import { toastStore } from '$lib/stores/toast';
	import Toast from './Toast.svelte';

	const store = toastStore;
</script>

<!-- Toast Container: Fixed position, bottom-right -->
<div class="fixed bottom-4 right-4 z-50 flex flex-col gap-2 pointer-events-none">
	{#each $store.toasts as toast (toast.id)}
		<div class="pointer-events-auto">
			<Toast {toast} onDismiss={() => store.removeToast(toast.id)} />
		</div>
	{/each}
</div>
