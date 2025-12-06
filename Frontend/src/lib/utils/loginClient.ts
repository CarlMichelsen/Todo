import { AuthorizedHttpClient } from './authorizedHttpClient';
import { HttpMethod } from './httpClient';

/**
 * All supported authentication providers
 */
export const AUTH_PROVIDERS = ['GitHub', 'Discord', 'Test'] as const;

/**
 * Supported authentication provider type
 */
export type AuthProvider = typeof AUTH_PROVIDERS[number];

/**
 * Login redirect options
 */
export interface LoginOptions {
    provider: AuthProvider;
    successRedirectUrl?: string;
    errorRedirectUrl?: string;
}

/**
 * Client for handling login redirects and authentication provider management
 * Extends AuthorizedHttpClient to leverage environment-specific URL handling
 */
export class LoginClient extends AuthorizedHttpClient {
    /**
     * Redirect the user to the login page for the specified provider
     * This will navigate the browser to the OAuth login flow
     */
    login(options: LoginOptions): void {
        const baseUrl = this.getBaseUrlValue();
        const url = new URL(`${baseUrl}/api/v1/Auth/Login/${options.provider}`);

        // Add optional query parameters
        if (options.successRedirectUrl) {
            url.searchParams.append('SuccessRedirectUrl', options.successRedirectUrl);
        }

        if (options.errorRedirectUrl) {
            url.searchParams.append('ErrorRedirectUrl', options.errorRedirectUrl);
        }

        // Redirect the browser
        window.location.href = url.toString();
    }

    /**
     * Get list of available authentication providers
     * Excludes 'Test' provider in production
     */
    getAvailableProviders(): AuthProvider[] {
        const isDev = import.meta.env.DEV;

        if (isDev) {
            // Return all providers in development
            return [...AUTH_PROVIDERS];
        }

        // Exclude 'Test' provider in production
        return AUTH_PROVIDERS.filter(provider => provider !== 'Test');
    }

    /**
     * Logout the current user
     * Calls the logout endpoint and optionally redirects
     */
    async logout(redirectUrl?: string): Promise<void> {
        await this.request<void>(HttpMethod.DELETE, '/api/v1/Auth/Logout');

        // Redirect after successful logout
        if (redirectUrl) {
            window.location.href = redirectUrl;
        }
    }
}
