/**
 * JWT User data returned from the User endpoint
 */
export interface JwtUser {
    userId: string;
    username: string;
    email: string;
    accessTokenId: string;
    tokenIssuedAt: string;
    tokenExpiresAt: string;
    issuer: string;
    audience: string;
    authenticationProvider: string;
    authenticationProviderId: string;
    roles: string[];
    profile: string;
    profileMedium?: string | null;
    profileLarge?: string | null;
}
