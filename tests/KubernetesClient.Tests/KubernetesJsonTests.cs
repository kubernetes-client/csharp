using System;
using System.Text.Json;
using Xunit;
using k8s.Models;

namespace k8s.Tests;

public class KubernetesJsonTests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA1812:'KubernetesJsonTests.RfcTime' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it 'static' (Module in Visual Basic). (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1812)", Justification = "json type")]
    private class RfcTime
    {
        public DateTime Rfc3339 { get; set; }
        public DateTime Rfc3339micro { get; set; }
        public DateTime Rfc3339nano { get; set; }
        public DateTime Rfc3339nanolenient1 { get; set; }
        public DateTime Rfc3339nanolenient2 { get; set; }
        public DateTime Rfc3339nanolenient3 { get; set; }
        public DateTime Rfc3339nanolenient4 { get; set; }
        public DateTime Rfc3339nanolenient5 { get; set; }
        public DateTime Rfc3339nanolenient6 { get; set; }
        public DateTime Rfc3339nanolenient7 { get; set; }
        public DateTime Rfc3339nanolenient8 { get; set; }
        public DateTime Rfc3339nanolenient9 { get; set; }
    }


    [Fact]
    public void RFC3339()
    {
        /* go code to generate the json https://go.dev/play/p/VL95pugm6o8

        const RFC3339Micro = "2006-01-02T15:04:05.000000Z07:00"
        const RFC3339Nano = "2006-01-02T15:04:05.000000000Z07:00"

        func main() {
            t := time.Now()
            type Time struct {
                RFC3339      string `json:"rfc3339"`
                RFC3339Micro string `json:"rfc3339micro"`
                RFC3339Nano  string `json:"rfc3339nano"`

                RFC3339NanoLenient1 string `json:"rfc3339nanolenient1"`
                RFC3339NanoLenient2 string `json:"rfc3339nanolenient2"`
                RFC3339NanoLenient3 string `json:"rfc3339nanolenient3"`
                RFC3339NanoLenient4 string `json:"rfc3339nanolenient4"`
                RFC3339NanoLenient5 string `json:"rfc3339nanolenient5"`
                RFC3339NanoLenient6 string `json:"rfc3339nanolenient6"`
                RFC3339NanoLenient7 string `json:"rfc3339nanolenient7"`
                RFC3339NanoLenient8 string `json:"rfc3339nanolenient8"`
                RFC3339NanoLenient9 string `json:"rfc3339nanolenient9"`
            }
            t1 := Time{
                RFC3339:      t.Add(45 * time.Minute).Add(12 * time.Second).Add(123456789 * time.Nanosecond).Format(time.RFC3339),
                RFC3339Micro: t.Add(45 * time.Minute).Add(12 * time.Second).Add(123456789 * time.Nanosecond).Format(RFC3339Micro),
                RFC3339Nano:  t.Add(24 * time.Minute).Add(56 * time.Second).Add(123456789 * time.Nanosecond).Format(RFC3339Nano),

                RFC3339NanoLenient1: t.Add(100000000 * time.Nanosecond).Format(time.RFC3339Nano),
                RFC3339NanoLenient2: t.Add(120000000 * time.Nanosecond).Format(time.RFC3339Nano),
                RFC3339NanoLenient3: t.Add(123000000 * time.Nanosecond).Format(time.RFC3339Nano),
                RFC3339NanoLenient4: t.Add(123400000 * time.Nanosecond).Format(time.RFC3339Nano),
                RFC3339NanoLenient5: t.Add(123450000 * time.Nanosecond).Format(time.RFC3339Nano),
                RFC3339NanoLenient6: t.Add(123456000 * time.Nanosecond).Format(time.RFC3339Nano),
                RFC3339NanoLenient7: t.Add(123456700 * time.Nanosecond).Format(time.RFC3339Nano),
                RFC3339NanoLenient8: t.Add(123456780 * time.Nanosecond).Format(time.RFC3339Nano),
                RFC3339NanoLenient9: t.Add(123456789 * time.Nanosecond).Format(time.RFC3339Nano),
            }
            b, err := json.Marshal(t1)
            if err != nil {
                fmt.Println("error:", err)
            }
            fmt.Println(string(b))
        }
        */

        var json = "{\"rfc3339\":\"2009-11-10T23:45:12Z\",\"rfc3339micro\":\"2009-11-10T23:45:12.123456Z\",\"rfc3339nano\":\"2009-11-10T23:24:56.123456789Z\",\"rfc3339nanolenient1\":\"2009-11-10T23:00:00.1Z\",\"rfc3339nanolenient2\":\"2009-11-10T23:00:00.12Z\",\"rfc3339nanolenient3\":\"2009-11-10T23:00:00.123Z\",\"rfc3339nanolenient4\":\"2009-11-10T23:00:00.1234Z\",\"rfc3339nanolenient5\":\"2009-11-10T23:00:00.12345Z\",\"rfc3339nanolenient6\":\"2009-11-10T23:00:00.123456Z\",\"rfc3339nanolenient7\":\"2009-11-10T23:00:00.1234567Z\",\"rfc3339nanolenient8\":\"2009-11-10T23:00:00.12345678Z\",\"rfc3339nanolenient9\":\"2009-11-10T23:00:00.123456789Z\"}\r\n";

        var t = KubernetesJson.Deserialize<RfcTime>(json);

        Assert.Equal(new DateTime(2009, 11, 10, 23, 45, 12, DateTimeKind.Utc), t.Rfc3339);

        Assert.Equal(2009, t.Rfc3339micro.Year);
        Assert.Equal(11, t.Rfc3339micro.Month);
        Assert.Equal(10, t.Rfc3339micro.Day);
        Assert.Equal(23, t.Rfc3339micro.Hour);
        Assert.Equal(45, t.Rfc3339micro.Minute);
        Assert.Equal(12, t.Rfc3339micro.Second);
        Assert.Equal(123, t.Rfc3339micro.Millisecond);

        Assert.Equal(2009, t.Rfc3339nano.Year);
        Assert.Equal(11, t.Rfc3339nano.Month);
        Assert.Equal(10, t.Rfc3339nano.Day);
        Assert.Equal(23, t.Rfc3339nano.Hour);
        Assert.Equal(24, t.Rfc3339nano.Minute);
        Assert.Equal(56, t.Rfc3339nano.Second);
        Assert.Equal(123, t.Rfc3339nano.Millisecond);

#if NET7_0_OR_GREATER
        Assert.Equal(456, t.Rfc3339micro.Microsecond);
        Assert.Equal(456, t.Rfc3339nano.Microsecond);
        Assert.Equal(700, t.Rfc3339nano.Nanosecond);

        Assert.Equal(100, t.Rfc3339nanolenient1.Millisecond);
        Assert.Equal(120, t.Rfc3339nanolenient2.Millisecond);
        Assert.Equal(123, t.Rfc3339nanolenient3.Millisecond);

        Assert.Equal(400, t.Rfc3339nanolenient4.Microsecond);
        Assert.Equal(450, t.Rfc3339nanolenient5.Microsecond);
        Assert.Equal(456, t.Rfc3339nanolenient6.Microsecond);

        Assert.Equal(456, t.Rfc3339nanolenient7.Microsecond);
        Assert.Equal(456, t.Rfc3339nanolenient8.Microsecond);
        Assert.Equal(456, t.Rfc3339nanolenient9.Microsecond);

        Assert.Equal(700, t.Rfc3339nanolenient7.Nanosecond);
        Assert.Equal(700, t.Rfc3339nanolenient8.Nanosecond);
        Assert.Equal(700, t.Rfc3339nanolenient9.Nanosecond);
#endif

    }

    [Fact]
    public void ReadWriteDatesJson()
    {
        var kManifest = """
        {
          "apiVersion": "v1",
          "kind": "Secret",
          "metadata": {
            "creationTimestamp": "2025-09-03T05:15:53Z",
            "name": "test-secret"
          },
          "type": "Opaque"
        }
        """;

        var objFromJson = KubernetesJson.Deserialize<V1Secret>(kManifest);
        var jsonFromObj = KubernetesJson.Serialize(objFromJson);

        // Format Json
        var jsonFromObj2 = JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonElement>(jsonFromObj), new JsonSerializerOptions() { WriteIndented = true });

        Assert.Equal(kManifest, jsonFromObj2);
    }

    [Fact]
    public void DateTimeWithFractionalSecondsAlwaysHasSixDigits()
    {
        // Test that datetime fields with fractional seconds always output exactly 6 decimal places
        // This is required by Kubernetes API which expects RFC3339Micro format

        // Create a datetime with 5 digits of precision (962170 microseconds = .96217 seconds)
        var dt = new DateTime(2025, 11, 17, 22, 52, 34, 962, DateTimeKind.Utc).AddTicks(1700);

        var secret = new V1Secret
        {
            Metadata = new V1ObjectMeta
            {
                Name = "test-secret",
                CreationTimestamp = dt,
            },
        };

        var json = KubernetesJson.Serialize(secret);

        // Verify the datetime is serialized with exactly 6 decimal places
        Assert.Contains("2025-11-17T22:52:34.962170Z", json);

        // Also verify it doesn't have 5 digits (which would fail in Kubernetes)
        Assert.DoesNotContain("2025-11-17T22:52:34.96217Z", json);
    }

    [Fact]
    public void DateTimeWithTimezoneOffsetParsesCorrectly()
    {
        // Test that datetime with explicit timezone offset (e.g., +00:00) is parsed correctly
        // This uses the "last resort" general DateTimeOffset parsing fallback
        var json = "{\"metadata\":{\"creationTimestamp\":\"2025-12-12T16:16:55.079293+00:00\"}}";

        var secret = KubernetesJson.Deserialize<V1Secret>(json);

        Assert.NotNull(secret.Metadata?.CreationTimestamp);
        var dt = secret.Metadata.CreationTimestamp.Value;

        Assert.Equal(2025, dt.Year);
        Assert.Equal(12, dt.Month);
        Assert.Equal(12, dt.Day);
        Assert.Equal(16, dt.Hour);
        Assert.Equal(16, dt.Minute);
        Assert.Equal(55, dt.Second);
        Assert.Equal(79, dt.Millisecond);
#if NET7_0_OR_GREATER
        Assert.Equal(293, dt.Microsecond);
#endif
    }

    [Fact]
    public void DateTimeNonRfc3339FormatIsRejected()
    {
        var json = "{\"metadata\":{\"creationTimestamp\":\"12/31/2023\"}}";

        Assert.Throws<FormatException>(() => KubernetesJson.Deserialize<V1Secret>(json));
    }
}
