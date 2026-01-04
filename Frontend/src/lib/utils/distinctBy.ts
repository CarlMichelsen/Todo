export const distinctBy = <T>(array: T[], keyFn: (value: T) => string) => [
	...new Map(array.map((item) => [keyFn(item), item])).values()
];
