import { ProblemDetails, UnauthorizedError, ValidationProblemDetails } from "$lib/types/api/error";
import { ApiResponse } from "$lib/types/api/response";

/**
 * Configuration for the API client
 */
export interface ApiConfig {
    baseUrl: string;
    timeout?: number;
    headers?: Record<string, string>;
}

/**
 * HTTP request options
 */
export interface RequestOptions {
    headers?: Record<string, string>;
    timeout?: number;
    credentials?: RequestCredentials;
}

/**
 * HTTP methods enum
 */
export enum HttpMethod {
    GET = 'GET',
    POST = 'POST',
    PUT = 'PUT',
    PATCH = 'PATCH',
    DELETE = 'DELETE'
}

/**
 * Base HTTP client class for making REST API calls
 * Provides a centralized place for request handling, error management, and configuration
 */
export abstract class HttpClient {
    private baseUrl: string;
    private defaultHeaders: Record<string, string>;
    private timeout: number;

    constructor(config?: Partial<ApiConfig>) {
        // Use development URL if in dev mode, otherwise use production URL
        this.baseUrl = this.getBaseUrl(config?.baseUrl);
        this.timeout = config?.timeout || 30000; // 30 seconds default
        this.defaultHeaders = {
            'Content-Type': 'application/json',
            ...config?.headers
        };
    }

    /**
     * Make HTTP request
     */
    protected async request<T, D = unknown>(
        method: HttpMethod,
        endpoint: string,
        data?: D,
        options?: RequestOptions
    ): Promise<ApiResponse<T>> {
        const url = this.buildUrl(endpoint);
        const headers = this.mergeHeaders(options);

        const response = await this.fetchWithTimeout(
            url,
            {
                method,
                headers,
                body: data ? JSON.stringify(data) : undefined,
                credentials: options?.credentials || 'include'
            },
            options?.timeout || this.timeout
        );

        return this.handleResponse<T>(response);
    }

    /**
     * Determine the base URL based on environment
     */
    private getBaseUrl(configUrl?: string): string {
        if (configUrl) {
            return configUrl;
        }

        // Check if we're in development mode
        const isDev = import.meta.env.DEV;

        if (isDev) {
            // Use environment variable or default dev URL
            return import.meta.env.VITE_API_URL || 'http://localhost:5035';
        }

        // In production, use the same host as the frontend
        return '';
    }

    /**
     * Build full URL from endpoint path
     */
    private buildUrl(endpoint: string): string {
        // Remove leading slash if present to avoid double slashes
        const path = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
        return `${this.baseUrl}/${path}`;
    }

    /**
     * Merge headers with defaults
     */
    private mergeHeaders(options?: RequestOptions): Record<string, string> {
        return {
            ...this.defaultHeaders,
            ...options?.headers
        };
    }

    /**
     * Handle fetch with timeout
     */
    private async fetchWithTimeout(
        url: string,
        options: RequestInit,
        timeout: number
    ): Promise<Response> {
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), timeout);

        try {
            const response = await fetch(url, {
                ...options,
                signal: controller.signal
            });
            clearTimeout(timeoutId);
            return response;
        } catch (error) {
            clearTimeout(timeoutId);
            if (error instanceof Error && error.name === 'AbortError') {
                throw new Error('Request timeout');
            }
            throw error;
        }
    }

    /**
     * Handle response and parse JSON
     */
    private async handleResponse<T>(response: Response): Promise<ApiResponse<T>> {
        if (!response.ok) {
            switch (response.status) {
                case 400:
                    return {
                        ok: false,
                        status: 400,
                        data: await response.json()
                    };

                case 401:
                    const errorText = await response.text();
                    throw new UnauthorizedError(`HTTP ${response.status}: ${errorText || response.statusText}`);

                default:
                    return {
                        ok: false,
                        status: response.status,
                        data: await response.json()
                    }
            }
        }

        // Handle empty responses (204 No Content)
        if (response.status === 204) {
            return {
                ok: true,
                status: response.status,
                data: null
            };
        }

        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            return {
                ok: true,
                status: response.status,
                data: response.json() as T
            };
        }

        return {
            ok: true,
            status: response.status,
            data: response.text() as T
        };
    }

    /**
     * Get current base URL
     */
    getBaseUrlValue(): string {
        return this.baseUrl;
    }
}
