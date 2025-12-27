import { writable } from 'svelte/store';

export interface Toast {
	id: string;
	type: 'error' | 'success' | 'info' | 'warning';
	message: string;
	duration?: number; // ms, 0 = no auto-dismiss
}

interface ToastStore {
	toasts: Toast[];
}

function createToastStore() {
	const { subscribe, update } = writable<ToastStore>({ toasts: [] });

	return {
		subscribe,

		addToast(toast: Omit<Toast, 'id'>): string {
			const id = crypto.randomUUID();
			const newToast: Toast = {
				id,
				duration: 5000, // default 5s
				...toast
			};

			update((state) => ({
				toasts: [...state.toasts, newToast].slice(-5) // Max 5 toasts
			}));

			// Auto-dismiss if duration > 0
			if (newToast.duration && newToast.duration > 0) {
				setTimeout(() => {
					this.removeToast(id);
				}, newToast.duration);
			}

			return id;
		},

		removeToast(id: string): void {
			update((state) => ({
				toasts: state.toasts.filter((t) => t.id !== id)
			}));
		},

		// Convenience methods
		success(message: string, duration?: number): string {
			return this.addToast({ type: 'success', message, duration });
		},

		error(message: string, duration?: number): string {
			return this.addToast({ type: 'error', message, duration });
		},

		info(message: string, duration?: number): string {
			return this.addToast({ type: 'info', message, duration });
		},

		warning(message: string, duration?: number): string {
			return this.addToast({ type: 'warning', message, duration });
		},

		clear(): void {
			update(() => ({ toasts: [] }));
		}
	};
}

export const toastStore = createToastStore();
