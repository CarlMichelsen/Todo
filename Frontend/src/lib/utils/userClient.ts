import { AuthorizedHttpClient } from './authorizedHttpClient';
import { HttpMethod } from './httpClient';
import type { JwtUser } from '$lib/types/user';

/**
 * Client for user-related API operations
 * Handles fetching current user information
 */
export class UserClient extends AuthorizedHttpClient {
    /**
     * Get the current authenticated user
     * Returns user data if authenticated, null if not authenticated (401)
     * Throws error for other failure cases
     */
    async getCurrentUser(): Promise<JwtUser | null> {
        try {
            const user = await this.request<JwtUser>(HttpMethod.GET, '/api/v1/User');

            if (!user) {
                return null;
            }

            return user;
        } catch (error) {
            // 401 means not authenticated - this is normal, return null
            if (error instanceof Error && error.message.includes('HTTP 401')) {
                return null;
            }

            // Re-throw other errors (network issues, 500s, etc.)
            throw error;
        }
    }
}
