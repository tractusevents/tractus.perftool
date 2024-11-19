// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using System.Text.Json.Serialization;
public class SemicolonSeparatedStringArrayConverter : JsonConverter<string[]>
{
    public override string[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Convert from a semicolon-separated string back to a string array
        var stringValue = reader.GetString();
        return stringValue?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
    }

    public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
    {
        // Join the array elements with semicolons and add a trailing semicolon
        var semicolonSeparated = string.Join(';', value) + ";";
        writer.WriteStringValue(semicolonSeparated);
    }
}
