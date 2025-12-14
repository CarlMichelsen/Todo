/**
 * API event DTO matching backend EventDto from OpenAPI spec
 */
export interface EventDto {
	/** Unique identifier (UUID format) */
	id: string;
	/** Event title */
	title: string;
	/** Event description */
	description: string;
	/** Start date-time (ISO 8601 string) */
	start: string;
	/** End date-time (ISO 8601 string) */
	end: string;
	/** Hex color code (e.g., "#3b82f6") */
	color: string;
}

/**
 * Create event request body from OpenAPI spec
 * All fields are required
 */
export interface CreateEventDto {
	/** Event title */
	title: string;
	/** Event description */
	description: string;
	/** Start date-time (ISO 8601 string) */
	start: string;
	/** End date-time (ISO 8601 string) */
	end: string;
	/** Hex color code (e.g., "#3b82f6") */
	color: string;
}

/**
 * Update event request body from OpenAPI spec
 * All fields are optional/nullable for partial updates
 */
export interface EditEventDto {
	/** Event title (nullable) */
	title?: string | null;
	/** Event description (nullable) */
	description?: string | null;
	/** Start date-time (ISO 8601 string, nullable) */
	start?: string | null;
	/** End date-time (ISO 8601 string, nullable) */
	end?: string | null;
	/** Hex color code (nullable) */
	color?: string | null;
}

/**
 * Pagination response wrapper from OpenAPI spec
 * Generic type T represents the type of items in the data array
 */
export interface PaginationDto<T> {
	/** Array of items for current page */
	data: T[];
	/** Current page number (1-indexed) */
	page: number;
	/** Number of items per page */
	pageSize: number;
	/** Total number of items across all pages */
	totalCount: number;
	/** Total number of pages */
	totalPages: number;
	/** Whether there is a previous page */
	hasPrevious: boolean;
	/** Whether there is a next page */
	hasNext: boolean;
	/** Previous page number (null if no previous page) */
	previousPage?: number | null;
	/** Next page number (null if no next page) */
	nextPage?: number | null;
	/** Index of first item on current page (0-indexed) */
	firstItemIndex: number;
	/** Index of last item on current page (0-indexed) */
	lastItemIndex: number;
	/** Number of items on current page */
	itemCount: number;
	/** Whether the current page is empty */
	isEmpty: boolean;
	/** Whether this is the first page */
	isFirstPage: boolean;
	/** Whether this is the last page */
	isLastPage: boolean;
}
