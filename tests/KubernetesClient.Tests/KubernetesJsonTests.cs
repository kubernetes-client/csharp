using System;
using Xunit;

namespace k8s.Tests;

public class KubernetesJsonTests
{
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

        var json = "{\"rfc3339\":\"2009-11-10T23:00:00Z\",\"rfc3339micro\":\"2009-11-10T23:00:00.000000Z\",\"rfc3339nano\":\"2009-11-10T13:00:00.000000000Z\"}\r\n";

        var t = KubernetesJson.Deserialize<RfcTime>(json);
    }
}
