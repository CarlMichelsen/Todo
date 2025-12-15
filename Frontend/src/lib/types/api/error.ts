/**
 * RFC 7807 Problem Details for HTTP APIs
 * Used for structured error responses from the backend
 */
export interface ProblemDetails {
	/** URI reference identifying the problem type */
	type?: string;
	/** Short, human-readable summary */
	title?: string;
	/** HTTP status code */
	status?: number;
	/** Human-readable explanation specific to this occurrence */
	detail?: string;
	/** URI reference identifying the specific occurrence */
	instance?: string;
}

/**
 * Validation Problem Details
 * Extends ProblemDetails with field-level validation errors
 */
export interface ValidationProblemDetails extends ProblemDetails {
	/** Dictionary of field names to error messages */
	errors?: Record<string, string[]>;
}

export class UnauthorizedError extends Error {
	constructor(message: string) {
		super(message);
	}

	public get status(): number {
		return 401;
	}
}