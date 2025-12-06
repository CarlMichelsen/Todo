import { ApiConfig, HttpClient, HttpMethod, type RequestOptions } from './httpClient';

/**
 * Abstract HTTP client with automatic token refresh on 401 Unauthorized responses
 * Extends HttpClient to handle authentication token refresh automatically
 */
export abstract class AuthorizedHttpClient extends HttpClient {
    private refreshUrl: string;

    constructor(config?: Partial<ApiConfig>) {
        super(config);
        this.refreshUrl = this.getRefreshUrl();
    }

    /**
     * Determine the refresh endpoint URL based on environment
     */
    private getRefreshUrl(): string {
        // Check if we're in development mode
        const isDev = import.meta.env.DEV;

        if (isDev) {
            // Use development URL
            return 'http://localhost:5220/api/v1/Auth/Refresh';
        }

        // In production
        return 'https://identity.survivethething.com/api/v1/Auth/Refresh';
    }

    /**
     * Override request method to handle 401 errors with automatic token refresh
     */
    protected override async request<T, D = unknown>(
        method: HttpMethod,
        endpoint: string,
        data?: D,
        options?: RequestOptions
    ): Promise<T | void> {
        try {
            // Try the original request
            return await super.request<T, D>(method, endpoint, data, options);
        } catch (error) {
            // Check if this is a 401 Unauthorized error
            if (error instanceof Error && error.message.includes('HTTP 401')) {
                // Attempt to refresh the token
                try {
                    await this.refreshToken();

                    // Retry the original request once after successful refresh
                    return await super.request<T, D>(method, endpoint, data, options);
                } catch (refreshError) {
                    // If refresh fails, user needs to login
                    if (refreshError instanceof Error && refreshError.message.includes('HTTP 401')) {
                        throw new Error('Authentication expired. Please login again.');
                    }
                    // Re-throw other refresh errors
                    throw new Error(`Token refresh failed: ${refreshError instanceof Error ? refreshError.message : 'Unknown error'}`);
                }
            }

            // Re-throw non-401 errors
            throw error;
        }
    }

    /**
     * Refresh the authentication token by calling the refresh endpoint
     */
    private async refreshToken(): Promise<void> {
        const response = await fetch(this.refreshUrl, {
            method: 'GET',
            credentials: 'include', // Include httponly cookies
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }
    }
}
