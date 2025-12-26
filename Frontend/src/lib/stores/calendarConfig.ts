import { getContext, setContext } from 'svelte';

export interface CalendarConfig {
	hourHeight: number;
}

const CALENDAR_CONFIG_KEY = Symbol('calendarConfig');

export function setCalendarConfig(config: CalendarConfig): void {
	setContext(CALENDAR_CONFIG_KEY, config);
}

export function getCalendarConfig(): CalendarConfig {
	return getContext<CalendarConfig>(CALENDAR_CONFIG_KEY);
}
