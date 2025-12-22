using System.Text.Json;

namespace Test.Util;

public static class TestJsonOptions
{
    public static JsonSerializerOptions Default => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}