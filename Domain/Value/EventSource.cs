namespace Domain.Value;

public enum EventSource
{
    Internal = 0,      // Created in your app
    IcsImport = 1,     // Imported from ICS
    IcsSync = 2,       // Synced from external calendar
}