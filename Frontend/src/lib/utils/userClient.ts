import { AuthorizedHttpClient } from './authorizedHttpClient';
import { HttpMethod } from './httpClient';
import type { PersonalUserDto } from '$lib/types/user';

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
    async getCurrentUser(): Promise<PersonalUserDto | null> {
        const response = await this.request<PersonalUserDto>(HttpMethod.GET, '/api/v1/User');

        if (!response.ok) {
            console.error('Failed to get current user:', {
                status: response.status,
                type: response.data.type,
                title: response.data.title,
                detail: response.data.detail
            });
            return null;
        }

        return response.data;
    }
}
