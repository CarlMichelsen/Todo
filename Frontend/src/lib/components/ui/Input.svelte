<script lang="ts">
	interface Props {
		type?: 'text' | 'email' | 'password' | 'date' | 'time' | 'number' | 'tel' | 'url';
		value: string;
		onchange?: (value: string) => void;
		oninput?: (value: string) => void;
		placeholder?: string;
		disabled?: boolean;
		readonly?: boolean;
		required?: boolean;
		error?: boolean;
		id?: string;
		name?: string;
		min?: string | number;
		max?: string | number;
		step?: string | number;
		class?: string;
	}

	let {
		type = 'text',
		value = $bindable(''),
		onchange,
		oninput,
		placeholder,
		disabled = false,
		readonly = false,
		required = false,
		error = false,
		id,
		name,
		min,
		max,
		step,
		class: className
	}: Props = $props();

	const baseClasses =
		'w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 transition-colors';

	const stateClasses = $derived.by(() => {
		if (error) {
			return 'border-red-500 focus:ring-red-500 focus:border-red-500 dark:border-red-400';
		}
		return 'border-gray-300 focus:ring-blue-500 focus:border-blue-500 dark:border-gray-600 dark:focus:ring-blue-400 dark:focus:border-blue-400';
	});

	const colorClasses =
		'bg-white text-gray-900 dark:bg-gray-800 dark:text-white placeholder-gray-400 dark:placeholder-gray-500';

	const disabledClasses = $derived(
		disabled ? 'opacity-50 cursor-not-allowed' : readonly ? 'bg-gray-50 dark:bg-gray-700' : ''
	);

	const classes = $derived(
		`${baseClasses} ${stateClasses} ${colorClasses} ${disabledClasses} ${className || ''}`
	);

	function handleInput(e: Event) {
		const target = e.target as HTMLInputElement;
		value = target.value;
		oninput?.(target.value);
	}

	function handleChange(e: Event) {
		const target = e.target as HTMLInputElement;
		onchange?.(target.value);
	}
</script>

<input
	{type}
	{id}
	{name}
	{placeholder}
	{disabled}
	{readonly}
	{required}
	{min}
	{max}
	{step}
	{value}
	class={classes}
	oninput={handleInput}
	onchange={handleChange}
/>
