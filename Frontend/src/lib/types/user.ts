/**
 * Personal User data returned from the User endpoint
 * Includes user details and selected calendar information
 */
export interface PersonalUserDto {
    userId: string;
    userName: string;
    email: string;
    selectedCalendarId: string;
    accessTokenId: string;
    tokenIssuedAt: string;
    tokenExpiresAt: string;
    authenticationProvider: string;
    authenticationProviderId: string;
    profile: string;
    profileMedium?: string | null;
    profileLarge?: string | null;
}
