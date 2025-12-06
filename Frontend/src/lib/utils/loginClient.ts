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
}

/**
 * Client for handling login redirects and authentication provider management
 * Extends AuthorizedHttpClient to leverage environment-specific URL handling
 */
export class LoginClient extends AuthorizedHttpClient {
    /**
     * Redirect the user to the login page for the specified provider
     * This will navigate the browser to the OAuth login flow
     * Success redirects to current URL, errors redirect to /error
     */
    login(options: LoginOptions): void {
        const baseUrl = this.getAuthBaseUrl();
        const url = new URL(`${baseUrl}/api/v1/Auth/Login/${options.provider}`);

        // Add redirect URLs based on current location
        url.searchParams.append('SuccessRedirectUrl', window.location.href);

        // Create error URL with pathname-based routing (HTML5 history mode)
        const errorUrl = new URL(window.location.href);
        errorUrl.pathname = '/error';
        errorUrl.hash = '';
        url.searchParams.append('ErrorRedirectUrl', errorUrl.toString());

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
}
