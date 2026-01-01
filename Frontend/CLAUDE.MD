# Todo Frontend - Claude Code Context

## Project Overview

This is a Svelte 5 Single Page Application (SPA) for a calendar and event management system with user authentication. The app features a complete calendar implementation with week views, multi-day events, overlapping event detection, and full CRUD operations for both calendars and events. It connects to backend services during development and features a modern, responsive UI with dark mode support.

The backend uses **CQRS (Command Query Responsibility Segregation)** architecture with Server-Sent Events (SSE) for real-time synchronization. The migration is in progress: Calendar and CalendarLink services are fully migrated to CQRS, while Event services still use traditional REST patterns.

**IMPORTANT: This is intentionally a SPA using svelte-routing. Do NOT suggest migrating to SvelteKit.**

## Tech Stack

- **Framework**: Svelte 5 (using runes and modern syntax)
- **Build Tool**: Vite (dev server on port 5155)
- **Language**: TypeScript
- **Styling**: Tailwind CSS v4
- **Routing**: svelte-routing (SPA routing, not SvelteKit)
- **Testing**: Vitest + Testing Library
- **Package Manager**: Bun (NOT npm/pnpm/yarn)

## Project Structure

```
src/
   App.svelte          # Main app component with routing
   main.ts             # App entry point
   app.css             # Global styles
   lib/
      components/
         calendar/          # Calendar system (13 components)
            Calendar.svelte             # Main calendar container with week navigation
            CalendarDay.svelte          # Individual day column with timeline
            CalendarGrid.svelte         # Week grid with event rendering
            CalendarHeader.svelte       # Navigation controls and calendar selector
            CalendarSelector.svelte     # Calendar dropdown menu
            CalendarMenu.svelte         # Calendar options burger menu
            CalendarModal.svelte        # Calendar creation modal
            CalendarEditModal.svelte    # Calendar edit/delete modal
            EventModal.svelte           # Event creation/editing modal
            TimeSpanEvent.svelte        # Event display with overlap handling
            GhostEvent.svelte           # Placeholder event display for overlapping events
            Timeline.svelte             # Hourly timeline (6am-11pm)
         modals/            # Modal component library
            Modal.svelte         # Base modal using HTML5 dialog
            FormModal.svelte     # Form submission modal
            AlertModal.svelte    # Simple alert modal
            ConfirmModal.svelte  # Confirmation modal
            README.md            # Complete modal documentation
         ui/                # Reusable UI components
            Button.svelte
            IconButton.svelte
            Input.svelte
            Label.svelte
            ErrorText.svelte
         forms/             # Form components
            FormField.svelte
         # Other components
         Header.svelte
         LoginButton.svelte
         Dropdown.svelte
         Profile.svelte
         Toast.svelte
         ToastContainer.svelte
         ProtectedRoute.svelte
         Card.svelte
      stores/               # Svelte stores for state management
         user.ts            # User authentication with token management
         darkMode.ts        # Dark mode toggle with localStorage
         calendars.ts       # Calendar CRUD with optimistic updates and CQRS/SSE
         calendarConfig.ts  # Calendar configuration and preferences
         events.ts          # Event management with date range queries (REST)
         sse.ts             # Server-Sent Events connection and event handling
         toast.ts           # Toast notification system
      types/                # TypeScript type definitions
         user.ts
         calendar.ts        # CalendarEvent, CalendarDay, EventLayout
         modal.ts           # ModalSize, ModalProps
         api/
            calendar.ts     # CalendarDto, CreateCalendarDto, EditCalendarDto
            calendarLink.ts # CalendarLinkDto, CreateCalendarLinkDto, EditCalendarLinkDto
            event.ts        # EventDto, CreateEventDto, EditEventDto, PaginationDto
            error.ts        # Error types
            response.ts     # API response types
            sse.ts          # Server-sent event type definitions (CQRS)
      utils/                # HTTP clients and utilities
         httpClient.ts               # Base HTTP client
         authorizedHttpClient.ts     # Authenticated requests
         loginClient.ts              # Login/auth endpoints
         userClient.ts               # User data endpoints
         calendarClient.ts           # Calendar CRUD API (CQRS with SSE)
         calendarLinkClient.ts       # Calendar Link CRUD API (CQRS with SSE)
         eventClient.ts              # Event CRUD API with pagination (REST - migration pending)
         serverSentEventClient.ts    # SSE connection management
         calendarUtils.ts            # Calendar utilities (date math, overlap detection)
         eventConverter.ts           # DTO to internal type conversion
         distinctBy.ts               # Utility function for array deduplication
         __tests__/
            calendarUtils.test.ts    # Calendar utility tests
      index.ts                 # Library exports
   tests/
      setup.ts               # Test configuration and setup
   vite-env.d.ts            # Vite environment type definitions
   main.ts                   # Application entry point
   routes/                  # Page components
      Home.svelte
      ProfilePage.svelte
      CalendarPage.svelte       # Main calendar page with URL navigation
      Demo.svelte               # Component showcase (development)
      Loading.svelte            # Loading state page
      Unauthorized.svelte       # Unauthorized access page
      Error.svelte
      NotFound.svelte
```

## Feature Overview

### Calendar System
- **Week-based calendar view** with responsive mobile/desktop layouts
- **Multi-day event support** with sophisticated overlap detection
- **Side-by-side event layout** algorithm for overlapping events
- **Calendar CRUD operations** (create, edit, delete with confirmation)
- **Event CRUD operations** with date-time pickers and color selection
- **Calendar selection** with dropdown menu and server-side persistence
- **URL-based navigation** with week parameter (`/calendar?week=YYYY-MM-DD`)
- **Timeline display** from 6am to 11pm with hourly grid
- **Mobile responsive** with day-by-day navigation on small screens

### Modal Component Library
For comprehensive modal documentation and usage patterns, see:
**`src/lib/components/modals/README.md`**

The modal system provides:
- Base Modal component using HTML5 `<dialog>` element
- Specialized modal types: FormModal, AlertModal, ConfirmModal
- Complete usage examples and patterns
- Backdrop click and ESC key support
- Multiple sizes (sm, md, lg, full)
- Accessibility features (focus trap, ARIA attributes)
- Dark mode support

### Toast Notification System
- **Auto-dismissing notifications** with configurable duration
- **Four types**: error, success, info, warning
- **Maximum 5 concurrent toasts** with automatic queuing
- **Top-center positioning** with highest z-index (2147483647)
- **Smooth fade animations** for enter/exit

### Form Component Library
- **Button** - Multiple variants (primary, secondary, danger, ghost) with loading states
- **IconButton** - Icon-only buttons with accessibility labels
- **Input** - Text input with dark mode support
- **Label** - Form labels with consistent styling
- **ErrorText** - Error message display
- **FormField** - Wrapper combining label, input, and error display

### Authentication & Route Guards
- **ProtectedRoute component** guards authenticated pages
- **User session management** with automatic token refresh
- **Automatic redirect** to login for unauthorized access
- **Initialized on app mount** in App.svelte

## Key Conventions

### Component Style

- Use Svelte 5 runes syntax (`$state`, `$derived`, `$effect`, `$bindable`)
- TypeScript in `<script lang="ts">` blocks with explicit Props interfaces
- Dark mode support via Tailwind's `dark:` prefix (required for all new components)
- Components are in PascalCase files
- Use snippets for flexible component composition

### State Management

- **User authentication**: `$lib/stores/user.ts`
  - User session data with token management
  - Initialized on app mount in App.svelte
  - Provides user info and selected calendar ID

- **Dark mode**: `$lib/stores/darkMode.ts`
  - Theme toggle state
  - Persisted to localStorage
  - Applied via Tailwind dark mode classes

- **Calendars**: `$lib/stores/calendars.ts`
  - Calendar list and active calendar selection
  - Full CRUD operations (create, update, delete) via CQRS commands
  - SSE event listeners for real-time state synchronization
  - Optimistic updates for calendar selection (fire-and-forget pattern)
  - Calendar links embedded in calendar structure
  - Server-side calendar and calendar link selection persistence

- **Server-Sent Events (SSE)**: `$lib/stores/sse.ts`
  - EventSource-based real-time connection to `/api/v1/ServerSentEvent`
  - Automatic reconnection with exponential backoff
  - Multi-tab support via connection ID
  - Event listener pattern with specific event names
  - Pending command tracking with timeout-based promise resolution

- **Events**: `$lib/stores/events.ts`
  - Event management for date range queries (REST - CQRS migration pending)
  - Automatic reload when calendar or date range changes
  - Optimistic updates for event operations
  - Loading and error states
  - Empty array initialization (no mock data)

- **Toast notifications**: `$lib/stores/toast.ts`
  - Toast queue management with max 5 concurrent
  - Type-safe message handling (error, success, info, warning)
  - Auto-dismiss timers

### API Communication

- **Development Setup**:
  - Frontend: port 5155
  - Backend API: port 5035
  - Identity/Login Service: port 5220
- **Environment Variables**:
  - `VITE_API_URL` - Backend API URL (dev: http://localhost:5035)
  - In production, services are accessible via same-host paths

- **HTTP clients** in `$lib/utils/`:
  - `httpClient.ts` - Base HTTP client with error handling
  - `authorizedHttpClient.ts` - Adds authentication token to requests
  - `loginClient.ts` - Identity service endpoints (port 5220 in dev)
  - `userClient.ts` - User data endpoints
  - `serverSentEventClient.ts` - SSE connection and event handling
    - EventSource connection to `/api/v1/ServerSentEvent`
    - Event listener registration and wildcard support
    - Automatic reconnection and error handling
  - `calendarClient.ts` - **Calendar CRUD (CQRS)**
    - GET `/api/v1/Calendar` - Query all calendars
    - GET `/api/v1/Calendar/{id}` - Query single calendar
    - POST `/api/v1/Calendar` - Command: create calendar (returns command ID)
    - PUT `/api/v1/Calendar/{id}` - Command: update calendar (returns command ID)
    - DELETE `/api/v1/Calendar/{id}` - Command: delete calendar (returns command ID)
    - POST `/api/v1/Calendar/{calendarId}` - Command: select calendar (returns command ID)
    - All commands send `X-Command-ID` header for SSE correlation
  - `calendarLinkClient.ts` - **Calendar Link CRUD (CQRS)**
    - GET `/api/v1/CalendarLink/calendar/{calendarId}` - Query calendar links
    - GET `/api/v1/CalendarLink/{id}` - Query single link
    - POST `/api/v1/CalendarLink` - Command: create calendar link (returns command ID)
    - PUT `/api/v1/CalendarLink/{id}` - Command: update calendar link (returns command ID)
    - DELETE `/api/v1/CalendarLink/{id}` - Command: delete calendar link
    - All commands send `X-Command-ID` header for SSE correlation
  - `eventClient.ts` - **Event CRUD (REST - migration to CQRS pending)**
    - GET `/api/v1/Event/{calendarId}` - Paginated event list
    - GET `/api/v1/Event/span/{calendarId}` - Date range query
    - GET/POST/PUT/DELETE `/api/v1/Event/{calendarId}/{eventId}` - Traditional REST
    - Pagination support for event lists

### Routing

Routes defined in App.svelte with Router/Route components:
- `/` - Home page
- `/calendar` - Calendar page with current week view
- `/calendar?week=YYYY-MM-DD` - Calendar with specific week
- `/profile` - User profile page (protected)
- `/demo` - Component showcase (development only)
- `/loading` - Loading state page
- `/unauthorized` - Unauthorized access page
- `/error` - Error page
- `/404` - Not found page

**Protected Routes:**
Use ProtectedRoute component to guard authenticated pages:
```svelte
<Route path="/profile">
  <ProtectedRoute>
    <ProfilePage />
  </ProtectedRoute>
</Route>
```

## Component Libraries

### Modal System
For comprehensive modal documentation and usage patterns, see:
**`src/lib/components/modals/README.md`**

The README contains:
- Complete usage examples for all modal types
- Pattern for creating custom modal types
- Props documentation
- Accessibility guidelines
- Styling customization tips

Quick example:
```svelte
<ConfirmModal
  bind:isOpen={showConfirm}
  title="Delete Calendar"
  message="Are you sure?"
  confirmText="Delete"
  onConfirm={handleDelete}
/>
```

### Calendar Components
Located in `src/lib/components/calendar/`:
- **Calendar.svelte** - Main container, manages week state and navigation
- **CalendarGrid.svelte** - Renders week grid with 7 day columns
- **CalendarDay.svelte** - Individual day column with timeline and events
- **Timeline.svelte** - Hourly grid lines from 6am-11pm
- **TimeSpanEvent.svelte** - Event display with click handlers and overlap positioning
- **CalendarHeader.svelte** - Week/day navigation, calendar selector, add event button
- **CalendarSelector.svelte** - Dropdown menu for switching calendars
- **CalendarMenu.svelte** - Burger menu for calendar edit/delete options
- **CalendarModal.svelte** - Modal for creating new calendars
- **CalendarEditModal.svelte** - Modal for editing/deleting calendars
- **EventModal.svelte** - Modal for creating/editing events with date-time pickers

### UI Components
Located in `src/lib/components/ui/`:
- **Button.svelte** - Variants: primary, secondary, danger, ghost, with loading states
- **IconButton.svelte** - Icon-only buttons with aria-labels
- **Input.svelte** - Text input with dark mode styling
- **Label.svelte** - Form label component
- **ErrorText.svelte** - Error message display with red styling

### Form Components
- **FormField.svelte** - Combines Label, Input/control, and ErrorText in consistent layout

## CQRS Architecture

The backend is migrating from traditional REST to CQRS (Command Query Responsibility Segregation) with Server-Sent Events (SSE) for real-time synchronization.

### Migration Status

| Service | CREATE | READ | UPDATE | DELETE | Status |
|---------|--------|------|--------|--------|--------|
| **Calendar** | CQRS | REST | CQRS | CQRS | ✅ Fully Migrated |
| **CalendarLink** | CQRS | REST | CQRS | CQRS | ✅ Fully Migrated |
| **Event** | REST | REST | REST | REST | ⏳ Pending Migration |

### CQRS Pattern Overview

**Commands** (state-changing operations):
- Send request with `X-Command-ID` header containing unique UUID
- Server returns command ID immediately (202 Accepted pattern)
- Server processes command asynchronously
- Server broadcasts SSE event when command completes
- Frontend listens for event via `serverSentEventClient` and updates state

**Queries** (read-only operations):
- Traditional REST GET requests
- Return complete data synchronously
- No command ID needed

### SSE Event Types

SSE event types are defined in `src/lib/types/api/sse.ts`. Supported events:
- **CreateCalendar**, **EditCalendar**, **DeleteCalendar** - Calendar operations
- **SelectCalendar** - Calendar selection persistence
- **CreateCalendarLink**, **EditCalendarLink**, **DeleteCalendarLink** - Calendar link operations

All events include: `eventName`, `eventId` (unique identifier for correlation), and `dispatchedAt` (ISO timestamp).

### Implementation Patterns

#### CQRS Command Pattern

Commands return a command ID for SSE correlation. Example from `calendarClient.ts`:
```typescript
async createCalendar(dto: CreateCalendarDto, commandId?: string): Promise<string> {
    const id = commandId || crypto.randomUUID();
    await this.request<void>(HttpMethod.POST, `/api/v1/Calendar`, dto, {
        headers: { 'X-Command-ID': id }
    });
    return id; // Return command ID for SSE correlation
}
```

#### SSE Event Handling

Stores listen for SSE events and update state. See event listeners in `calendars.ts` for Calendar and CalendarLink event examples.

#### Fire-and-Forget Pattern

Calendar selection uses optimistic updates for instant UI feedback:
1. Update local state immediately
2. Send command to backend (non-blocking)
3. SSE event confirms on server-side success
4. Backend errors are logged but don't revert local state

### SSE Connection Lifecycle

1. User authenticates → `userStore` initializes SSE connection via `serverSentEventClient`
2. Connection ID generated per app session for multi-tab support
3. EventSource connects to `/api/v1/ServerSentEvent?connectionId={id}`
4. Automatic reconnection on failure with exponential backoff
5. User logs out → SSE connection closed

### Important CQRS Notes

- **Command IDs**: All commands must include unique `X-Command-ID` header for idempotency
- **Idempotency**: Sending same command ID twice produces same result
- **Async Processing**: Commands are processed asynchronously; listen for SSE event confirmation
- **Event Correlation**: SSE events include `eventId` for correlation with command ID
- **Calendar Links**: Embedded in `CalendarDto`, updates reflected in calendar events
- **Multi-Tab Sync**: Connection ID ensures each browser tab receives own SSE events

## Important Implementation Patterns

### Svelte 5 Runes
- **`$state`** - Reactive state variables
- **`$derived`** - Computed values that update automatically
- **`$derived.by`** - Computed values with complex logic
- **`$bindable`** - Two-way binding for component props
- **`$effect`** - Side effects (replace legacy `$:` statements)
- **Snippets** - Flexible component composition (replace slots)

### Optimistic Updates
Calendar selection uses optimistic updates for instant UI feedback:
1. Update local state immediately
2. Persist to server in background
3. Log errors but don't revert on failure
4. Provides graceful degradation for offline scenarios

### Event Overlap Detection
The `calendarUtils.ts` file (520 lines) provides sophisticated calendar utilities:
- **Overlap detection**: Identifies overlapping events within a day
- **Layout calculation**: Assigns width and offset percentages for side-by-side display
- **Multi-day event handling**: Calculates display start/end times for events spanning days
- **Date arithmetic**: Week calculations, date comparisons, time conversions
- **Formatting functions**: Display formats for dates, times, and durations

Algorithm summary:
1. Group events by overlapping time windows
2. Within each group, calculate columns needed
3. Assign each event a column index and width
4. Convert to CSS percentages for positioning

### Error Handling Patterns
- **API clients** throw typed errors with descriptive messages
- **Stores** catch errors and update error state
- **Toast notifications** display user-friendly error messages
- **Graceful degradation** for offline/network failures
- **Loading states** prevent duplicate requests

### Dark Mode
- All components must support dark mode
- Use Tailwind's `dark:` prefix for dark mode variants
- State managed in `darkMode.ts` store
- Persisted to localStorage
- Applied via class on document root

## Development Commands

**IMPORTANT: This project uses Bun. Always use `bun` commands, NOT npm/pnpm/yarn.**

```bash
bun run dev          # Start dev server (port 5155)
bun run build        # Production build
bun run preview      # Preview production build
bun run check        # Type check with svelte-check
bun run check:watch  # Watch mode type checking
bun run format       # Format with Prettier
bun run lint         # Lint with ESLint + Prettier check
bun run test         # Run tests with Vitest

# Installing dependencies
bun install          # Install all dependencies
bun add <package>    # Add a new dependency
bun add -d <package> # Add a dev dependency
```

## Environment Variables

Required in `.env` file:

- `VITE_API_URL` - Backend API URL (dev: http://localhost:5035, prod: /api)

Note: The identity service (port 5220) is handled separately in the login client.

## MCP Integration

This project has access to the Svelte MCP server. When working with Svelte code:

1. Use `list-sections` to find relevant documentation
2. Use `get-documentation` to fetch detailed docs
3. Use `svelte-autofixer` to validate Svelte code before committing
4. Use `playground-link` if user wants to test code snippets

## Code Quality

- ESLint configured with Svelte plugin
- Prettier with Svelte and Tailwind plugins
- TypeScript strict mode enabled
- Vitest + Testing Library for unit and component tests
- Always run `bun run lint` and `bun run check` before committing

## Things to Watch Out For

1. **SPA Architecture**: This is a Single Page Application - do NOT suggest SvelteKit migration
2. **Bun Package Manager**: Always use `bun` commands, never npm/pnpm/yarn
3. **Port Configuration**: Backend API (5035) and Identity Service (5220) are dev-only ports
4. **Svelte 5 Syntax**: Use runes (`$state`, `$derived`, `$effect`), not legacy reactive declarations
5. **Import Paths**: Use `$lib` alias for library imports
6. **Dark Mode**: All new UI must support dark mode with Tailwind `dark:` classes
7. **Type Safety**: Maintain TypeScript types, especially for API responses
8. **Authentication**: Ensure protected routes use ProtectedRoute component
9. **API Errors**: Handle API failures gracefully with proper error states
10. **Calendar Date Handling**: All calendar utilities use local timezone, events use ISO format from API
11. **Modal Usage**: Always use `bind:isOpen` on modals. See `modals/README.md` for complete documentation and patterns
12. **Toast Notifications**: Maximum 5 concurrent toasts. Use appropriate type (error, success, info, warning)
13. **Event Overlap**: Calendar layout algorithm in `calendarUtils.ts` handles complex overlapping events
14. **Optimistic Updates**: Calendar selection updates UI immediately, then syncs to server in background
15. **No Mock Data**: Events store initializes with empty array. Real data loads when calendar is selected
16. **CQRS Architecture**: Calendar and CalendarLink use CQRS with SSE; Events still use traditional REST. See SSE event definitions in `src/lib/types/api/sse.ts`
17. **Command IDs**: CQRS commands include optional `X-Command-ID` header (generated if not provided) for idempotency and SSE correlation
18. **SSE Events**: Listen for SSE events in stores (see `calendars.ts` for examples). All events include `eventId` for correlation
19. **Fire-and-Forget Pattern**: Calendar selection is optimistic; command failures are logged but don't revert UI state
20. **Event Migration**: When migrating Event service to CQRS, follow patterns in `calendarClient.ts` (commands) and `calendars.ts` (SSE handlers)

## CQRS Migration Roadmap

The following endpoints are pending migration to CQRS architecture:

### Event Service Migration (In Progress)

**Current State**: Event CRUD operations use traditional REST patterns with direct response-based updates

**Migration Steps**:
1. Add SSE event types for Event operations (`CreateEvent`, `EditEvent`, `DeleteEvent`) in `src/lib/types/api/sse.ts`
2. Update `eventClient.ts` to send `X-Command-ID` header with commands and return command ID
3. Add SSE event listeners to `events.ts` store following the pattern used in `calendars.ts`
4. Update components (`EventModal.svelte`) to listen for SSE confirmation instead of direct response
5. Add integration tests for Event CQRS flow

**Reference**: See `calendarClient.ts` and `calendars.ts` for complete CQRS implementation patterns

## Future Considerations

- Migrate Event service to CQRS (in progress)
- Implement event reminders and notifications
- Add calendar sharing and permissions system
- Implement recurring events (daily, weekly, monthly patterns)
- Add event search and advanced filtering
- Consider event categories and tags
- Implement drag-and-drop event rescheduling
- Add bulk event operations (delete multiple, duplicate, etc.)
- Implement calendar export (iCal/ICS format)
- Add calendar import functionality
- Consider event attachments and notes
- Implement event color customization per event (currently inherits from calendar)
