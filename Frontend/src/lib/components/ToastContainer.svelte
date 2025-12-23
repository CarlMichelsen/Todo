<script lang="ts">
	import { toastStore } from '$lib/stores/toast';
	import Toast from './Toast.svelte';

	const store = toastStore;
</script>

<!-- Toast Container: Fixed position, top-center, above dialog backdrop -->
<div class="fixed top-4 left-1/2 -translate-x-1/2 z-[2147483647] flex flex-col gap-2 pointer-events-none">
	{#each $store.toasts as toast (toast.id)}
		<div class="pointer-events-auto">
			<Toast {toast} onDismiss={() => store.removeToast(toast.id)} />
		</div>
	{/each}
</div>
