export type ModalSize = 'sm' | 'md' | 'lg' | 'full';

export interface ModalProps {
	isOpen?: boolean;
	size?: ModalSize;
	showClose?: boolean;
	closeOnBackdrop?: boolean;
	onClose?: () => void;
}
