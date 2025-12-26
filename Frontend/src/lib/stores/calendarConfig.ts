import { getContext, setContext } from 'svelte';

export interface CalendarConfig {
	hourHeight: number;
	ghostEventDuration: number;
	ghostEventSnapInterval: number;
}

const CALENDAR_CONFIG_KEY = Symbol('calendarConfig');

export function setCalendarConfig(config: Partial<CalendarConfig>): void {
	const fullConfig: CalendarConfig = {
		hourHeight: config.hourHeight ?? 40,
		ghostEventDuration: config.ghostEventDuration ?? 30,
		ghostEventSnapInterval: config.ghostEventSnapInterval ?? 30
	};
	setContext(CALENDAR_CONFIG_KEY, fullConfig);
}

export function getCalendarConfig(): CalendarConfig {
	return getContext<CalendarConfig>(CALENDAR_CONFIG_KEY);
}
