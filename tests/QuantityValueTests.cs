using System;
using k8s.Models;
using Xunit;
using static k8s.Models.ResourceQuantity.SuffixFormat;

namespace k8s.Tests
{
    public class QuantityValueTests
    {
        [Fact]
        public void Parse()
        {
            foreach (var (input, expect) in new[]
            {
                ("0", new ResourceQuantity(0, 0, DecimalSI)),
                ("0n", new ResourceQuantity(0, 0, DecimalSI)),
                ("0u", new ResourceQuantity(0, 0, DecimalSI)),
                ("0m", new ResourceQuantity(0, 0, DecimalSI)),
                ("0Ki", new ResourceQuantity(0, 0, BinarySI)),
                ("0k", new ResourceQuantity(0, 0, DecimalSI)),
                ("0Mi", new ResourceQuantity(0, 0, BinarySI)),
                ("0M", new ResourceQuantity(0, 0, DecimalSI)),
                ("0Gi", new ResourceQuantity(0, 0, BinarySI)),
                ("0G", new ResourceQuantity(0, 0, DecimalSI)),
                ("0Ti", new ResourceQuantity(0, 0, BinarySI)),
                ("0T", new ResourceQuantity(0, 0, DecimalSI)),

                // Quantity less numbers are allowed
                ("1", new ResourceQuantity(1, 0, DecimalSI)),

                // Binary suffixes
                ("1Ki", new ResourceQuantity(1024, 0, BinarySI)),
                ("8Ki", new ResourceQuantity(8 * 1024, 0, BinarySI)),
                ("7Mi", new ResourceQuantity(7 * 1024 * 1024, 0, BinarySI)),
                ("6Gi", new ResourceQuantity(6L * 1024 * 1024 * 1024, 0, BinarySI)),
                ("5Ti", new ResourceQuantity(5L * 1024 * 1024 * 1024 * 1024, 0, BinarySI)),
                ("4Pi", new ResourceQuantity(4L * 1024 * 1024 * 1024 * 1024 * 1024, 0, BinarySI)),
                ("3Ei", new ResourceQuantity(3L * 1024 * 1024 * 1024 * 1024 * 1024 * 1024, 0, BinarySI)),

                ("10Ti", new ResourceQuantity(10L * 1024 * 1024 * 1024 * 1024, 0, BinarySI)),
                ("100Ti", new ResourceQuantity(100L * 1024 * 1024 * 1024 * 1024, 0, BinarySI)),

                // Decimal suffixes
                ("5n", new ResourceQuantity(5, -9, DecimalSI)),
                ("4u", new ResourceQuantity(4, -6, DecimalSI)),
                ("3m", new ResourceQuantity(3, -3, DecimalSI)),
                ("9", new ResourceQuantity(9, 0, DecimalSI)),
                ("8k", new ResourceQuantity(8, 3, DecimalSI)),
                ("50k", new ResourceQuantity(5, 4, DecimalSI)),
                ("7M", new ResourceQuantity(7, 6, DecimalSI)),
                ("6G", new ResourceQuantity(6, 9, DecimalSI)),
                ("5T", new ResourceQuantity(5, 12, DecimalSI)),
                ("40T", new ResourceQuantity(4, 13, DecimalSI)),
                ("300T", new ResourceQuantity(3, 14, DecimalSI)),
                ("2P", new ResourceQuantity(2, 15, DecimalSI)),
                ("1E", new ResourceQuantity(1, 18, DecimalSI)),

                // Decimal exponents
                ("1E-3", new ResourceQuantity(1, -3, DecimalExponent)),
                ("1e3", new ResourceQuantity(1, 3, DecimalExponent)),
                ("1E6", new ResourceQuantity(1, 6, DecimalExponent)),
                ("1e9", new ResourceQuantity(1, 9, DecimalExponent)),
                ("1E12", new ResourceQuantity(1, 12, DecimalExponent)),
                ("1e15", new ResourceQuantity(1, 15, DecimalExponent)),
                ("1E18", new ResourceQuantity(1, 18, DecimalExponent)),

                // Nonstandard but still parsable
                ("1e14", new ResourceQuantity(1, 14, DecimalExponent)),
                ("1e13", new ResourceQuantity(1, 13, DecimalExponent)),
                ("1e3", new ResourceQuantity(1, 3, DecimalExponent)),
                ("100.035k", new ResourceQuantity(100035, 0, DecimalSI)),

                // Things that look like floating point
                ("0.001", new ResourceQuantity(1, -3, DecimalSI)),
                ("0.0005k", new ResourceQuantity(5, -1, DecimalSI)),
                ("0.005", new ResourceQuantity(5, -3, DecimalSI)),
                ("0.05", new ResourceQuantity(5, -2, DecimalSI)),
                ("0.5", new ResourceQuantity(5, -1, DecimalSI)),
                ("0.00050k", new ResourceQuantity(5, -1, DecimalSI)),
                ("0.00500", new ResourceQuantity(5, -3, DecimalSI)),
                ("0.05000", new ResourceQuantity(5, -2, DecimalSI)),
                ("0.50000", new ResourceQuantity(5, -1, DecimalSI)),
                ("0.5e0", new ResourceQuantity(5, -1, DecimalExponent)),
                ("0.5e-1", new ResourceQuantity(5, -2, DecimalExponent)),
                ("0.5e-2", new ResourceQuantity(5, -3, DecimalExponent)),
                ("0.5e0", new ResourceQuantity(5, -1, DecimalExponent)),
                ("10.035M", new ResourceQuantity(10035, 3, DecimalSI)),

                ("1.2e3", new ResourceQuantity(12, 2, DecimalExponent)),
                ("1.3E+6", new ResourceQuantity(13, 5, DecimalExponent)),
                ("1.40e9", new ResourceQuantity(14, 8, DecimalExponent)),
                ("1.53E12", new ResourceQuantity(153, 10, DecimalExponent)),
                ("1.6e15", new ResourceQuantity(16, 14, DecimalExponent)),
                ("1.7E18", new ResourceQuantity(17, 17, DecimalExponent)),

                ("9.01", new ResourceQuantity(901, -2, DecimalSI)),
                ("8.1k", new ResourceQuantity(81, 2, DecimalSI)),
                ("7.123456M", new ResourceQuantity(7123456, 0, DecimalSI)),
                ("6.987654321G", new ResourceQuantity(6987654321, 0, DecimalSI)),
                ("5.444T", new ResourceQuantity(5444, 9, DecimalSI)),
                ("40.1T", new ResourceQuantity(401, 11, DecimalSI)),
                ("300.2T", new ResourceQuantity(3002, 11, DecimalSI)),
                ("2.5P", new ResourceQuantity(25, 14, DecimalSI)),
                ("1.01E", new ResourceQuantity(101, 16, DecimalSI)),

                // Things that saturate/round
                ("3.001n", new ResourceQuantity(4, -9, DecimalSI)),
                ("1.1E-9", new ResourceQuantity(2, -9, DecimalExponent)),
                ("0.0000000001", new ResourceQuantity(1, -9, DecimalSI)),
                ("0.0000000005", new ResourceQuantity(1, -9, DecimalSI)),
                ("0.00000000050", new ResourceQuantity(1, -9, DecimalSI)),
                ("0.5e-9", new ResourceQuantity(1, -9, DecimalExponent)),
                ("0.9n", new ResourceQuantity(1, -9, DecimalSI)),
                ("0.00000012345", new ResourceQuantity(124, -9, DecimalSI)),
                ("0.00000012354", new ResourceQuantity(124, -9, DecimalSI)),
                ("9Ei", new ResourceQuantity(ResourceQuantity.MaxAllowed, 0, BinarySI)),
                ("9223372036854775807Ki", new ResourceQuantity(ResourceQuantity.MaxAllowed, 0, BinarySI)),
                ("12E", new ResourceQuantity(12, 18, DecimalSI)),

                // We'll accept fractional binary stuff, too.
                ("100.035Ki", new ResourceQuantity(10243584, -2, BinarySI)),
                ("0.5Mi", new ResourceQuantity(.5m * 1024 * 1024, 0, BinarySI)),
                ("0.05Gi", new ResourceQuantity(536870912, -1, BinarySI)),
                ("0.025Ti", new ResourceQuantity(274877906944, -1, BinarySI)),

                // Things written by trolls
                ("0.000000000001Ki", new ResourceQuantity(2, -9, DecimalSI)), // rounds up, changes format
                (".001", new ResourceQuantity(1, -3, DecimalSI)),
                (".0001k", new ResourceQuantity(100, -3, DecimalSI)),
                ("1.", new ResourceQuantity(1, 0, DecimalSI)),
                ("1.G", new ResourceQuantity(1, 9, DecimalSI))
            })
            {
                Assert.Equal(expect.ToString(), new ResourceQuantity(input).ToString());
            }

            foreach (var s in new[]
            {
                "1.1.M",
                "1+1.0M",
                "0.1mi",
                "0.1am",
                "aoeu",
                ".5i",
                "1i",
                "-3.01i",
                "-3.01e-"

                // TODO support trailing whitespace is forbidden
//                " 1",
//                "1 "
            })
            {
                Assert.ThrowsAny<Exception>(() => { new ResourceQuantity(s); });
            }
        }

        [Fact]
        public void QuantityString()
        {
            foreach (var (input, expect, alternate) in new[]
            {
                (new ResourceQuantity(1024 * 1024 * 1024, 0, BinarySI), "1Gi", "1024Mi"),
                (new ResourceQuantity(300 * 1024 * 1024, 0, BinarySI), "300Mi", "307200Ki"),
                (new ResourceQuantity(6 * 1024, 0, BinarySI), "6Ki", ""),
                (new ResourceQuantity(1001 * 1024 * 1024 * 1024L, 0, BinarySI), "1001Gi", "1025024Mi"),
                (new ResourceQuantity(1024 * 1024 * 1024 * 1024L, 0, BinarySI), "1Ti", "1024Gi"),
                (new ResourceQuantity(5, 0, BinarySI), "5", "5000m"),
                (new ResourceQuantity(500, -3, BinarySI), "500m", "0.5"),
                (new ResourceQuantity(1, 9, DecimalSI), "1G", "1000M"),
                (new ResourceQuantity(1000, 6, DecimalSI), "1G", "0.001T"),
                (new ResourceQuantity(1000000, 3, DecimalSI), "1G", ""),
                (new ResourceQuantity(1000000000, 0, DecimalSI), "1G", ""),
                (new ResourceQuantity(1, -3, DecimalSI), "1m", "1000u"),
                (new ResourceQuantity(80, -3, DecimalSI), "80m", ""),
                (new ResourceQuantity(1080, -3, DecimalSI), "1080m", "1.08"),
                (new ResourceQuantity(108, -2, DecimalSI), "1080m", "1080000000n"),
                (new ResourceQuantity(10800, -4, DecimalSI), "1080m", ""),
                (new ResourceQuantity(300, 6, DecimalSI), "300M", ""),
                (new ResourceQuantity(1, 12, DecimalSI), "1T", ""),
                (new ResourceQuantity(1234567, 6, DecimalSI), "1234567M", ""),
                (new ResourceQuantity(1234567, -3, BinarySI), "1234567m", ""),
                (new ResourceQuantity(3, 3, DecimalSI), "3k", ""),
                (new ResourceQuantity(1025, 0, BinarySI), "1025", ""),
                (new ResourceQuantity(0, 0, DecimalSI), "0", ""),
                (new ResourceQuantity(0, 0, BinarySI), "0", ""),
                (new ResourceQuantity(1, 9, DecimalExponent), "1e9", ".001e12"),
                (new ResourceQuantity(1, -3, DecimalExponent), "1e-3", "0.001e0"),
                (new ResourceQuantity(1, -9, DecimalExponent), "1e-9", "1000e-12"),
                (new ResourceQuantity(80, -3, DecimalExponent), "80e-3", ""),
                (new ResourceQuantity(300, 6, DecimalExponent), "300e6", ""),
                (new ResourceQuantity(1, 12, DecimalExponent), "1e12", ""),
                (new ResourceQuantity(1, 3, DecimalExponent), "1e3", ""),
                (new ResourceQuantity(3, 3, DecimalExponent), "3e3", ""),
                (new ResourceQuantity(3, 3, DecimalSI), "3k", ""),
                (new ResourceQuantity(0, 0, DecimalExponent), "0", "00"),
                (new ResourceQuantity(1, -9, DecimalSI), "1n", ""),
                (new ResourceQuantity(80, -9, DecimalSI), "80n", ""),
                (new ResourceQuantity(1080, -9, DecimalSI), "1080n", ""),
                (new ResourceQuantity(108, -8, DecimalSI), "1080n", ""),
                (new ResourceQuantity(10800, -10, DecimalSI), "1080n", ""),
                (new ResourceQuantity(1, -6, DecimalSI), "1u", ""),
                (new ResourceQuantity(80, -6, DecimalSI), "80u", ""),
                (new ResourceQuantity(1080, -6, DecimalSI), "1080u", "")
            })
            {
                Assert.Equal(expect, input.ToString());
                Assert.Equal(expect, new ResourceQuantity(expect).ToString());

                if (string.IsNullOrEmpty(alternate))
                {
                    continue;
                }

                Assert.Equal(expect, new ResourceQuantity(alternate).ToString());
            }
        }
    }
}
