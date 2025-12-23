/**
 * Simplified user representation used in Calendar and Event DTOs
 */
export interface UserDto {
	/** Unique identifier (UUID format) */
	userId: string;
	/** User's display name */
	userName: string;
	/** Profile picture URL */
	profile: string;
}

/**
 * API calendar DTO matching backend CalendarDto from OpenAPI spec
 */
export interface CalendarDto {
	/** Unique identifier (UUID format) */
	id: string;
	/** Calendar title */
	title: string;
	/** Hex color code (e.g., "#ea580c") */
	color: string;
	/** Calendar owner information */
	owner: UserDto;
}

/**
 * Create calendar request body from OpenAPI spec
 * All fields are required
 */
export interface CreateCalendarDto {
	/** Calendar title */
	title: string;
	/** Hex color code (e.g., "#ea580c") */
	color: string;
}

/**
 * Update calendar request body from OpenAPI spec
 * All fields are optional/nullable for partial updates
 */
export interface EditCalendarDto {
	/** Calendar title (nullable) */
	title?: string | null;
	/** Hex color code (nullable) */
	color?: string | null;
}
