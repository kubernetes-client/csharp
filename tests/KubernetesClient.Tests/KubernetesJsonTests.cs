using System;
using Xunit;

namespace k8s.Tests;

public class KubernetesJsonTests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA1812:'KubernetesJsonTests.RfcTime' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it 'static' (Module in Visual Basic). (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1812)", Justification = "json type")]
    private class RfcTime
    {
        public DateTime rfc3339 { get; set; }
        public DateTime rfc3339micro { get; set; }
        public DateTime Rfc3339nano { get; set; }
    }


    [Fact]
    public void RFC3339()
    {
        /* go code to generate the json https://go.dev/play/p/Owyci2bAKfe

        const RFC3339Micro = "2006-01-02T15:04:05.000000Z07:00"
        const RFC3339Nano = "2006-01-02T15:04:05.000000000Z07:00"

        func main() {
            t := time.Now()
            type Time struct {
                RFC3339      string `json:"rfc3339"`
                RFC3339Micro string `json:"rfc3339micro"`
                RFC3339Nano  string `json:"rfc3339nano"`
            }
            t1 := Time{
                RFC3339:      t.Format(time.RFC3339),
                RFC3339Micro: t.Format(RFC3339Micro),
                RFC3339Nano:  t.Format(RFC3339Nano),
            }
            b, err := json.Marshal(t1)
            if err != nil {
                fmt.Println("error:", err)
            }
            fmt.Println(string(b))
        }
        */

        // second changed from go playground
        var json = "{\"rfc3339\":\"2009-11-10T23:45:12Z\",\"rfc3339micro\":\"2009-11-10T23:45:12.123456Z\",\"rfc3339nano\":\"2009-11-10T13:24:56.123456789Z\"}\r\n";

        var t = KubernetesJson.Deserialize<RfcTime>(json);

        Assert.Equal(new DateTime(2009, 11, 10, 23, 45, 12, DateTimeKind.Utc), t.rfc3339);

        Assert.Equal(2009, t.rfc3339micro.Year);
        Assert.Equal(11, t.rfc3339micro.Month);
        Assert.Equal(10, t.rfc3339micro.Day);
        Assert.Equal(23, t.rfc3339micro.Hour);
        Assert.Equal(45, t.rfc3339micro.Minute);
        Assert.Equal(12, t.rfc3339micro.Second);
        Assert.Equal(123, t.rfc3339micro.Millisecond);

        Assert.Equal(2009, t.Rfc3339nano.Year);
        Assert.Equal(11, t.Rfc3339nano.Month);
        Assert.Equal(10, t.Rfc3339nano.Day);
        Assert.Equal(13, t.Rfc3339nano.Hour);
        Assert.Equal(24, t.Rfc3339nano.Minute);
        Assert.Equal(56, t.Rfc3339nano.Second);
        Assert.Equal(123, t.Rfc3339nano.Millisecond);

#if NET7_0_OR_GREATER
        Assert.Equal(456, t.rfc3339micro.Microsecond);
        Assert.Equal(456, t.Rfc3339nano.Microsecond);
        Assert.Equal(700, t.Rfc3339nano.Nanosecond);
#endif
    }
}
