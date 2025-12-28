export interface CreateCalendarLinkDto {
	title: string;
	calendarLink: string; // URI format
	color: string; // Hex color code (e.g., "#ea580c")
}

export interface EditCalendarLinkDto {
	title?: string | null;
	calendarLink?: string | null; // URI format
	color?: string | null; // Hex color code (nullable)
	deleteParentCalendarAssociation?: string[] | null; // UUID array
	addParentCalendarAssociation?: string[] | null; // UUID array
}
