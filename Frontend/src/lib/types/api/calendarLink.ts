export interface CreateCalendarLinkDto {
	title: string;
	calendarLink: string; // URI format
}

export interface EditCalendarLinkDto {
	title?: string | null;
	calendarLink?: string | null; // URI format
	deleteParentCalendarAssociation?: string[] | null; // UUID array
	addParentCalendarAssociation?: string[] | null; // UUID array
}
