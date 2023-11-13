using System.Text.RegularExpressions;
using CoordinateSharp;
using SharpKml.Dom;
using TimeSpan = System.TimeSpan;

namespace Mongrel.Common;

public static class Utils
{
    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    public static double NormalizeCordStr(string latOrLonString)
    {
        const double defaultOut = 99999.0;

        if (string.IsNullOrEmpty(latOrLonString) || latOrLonString.Contains('E')) return defaultOut;

        if (double.TryParse(latOrLonString, out var converted)) return converted;

        var splitDegreeMinSec = Regex.Split(latOrLonString, @"[^.\d]+", RegexOptions.None, TimeSpan.FromMilliseconds(100));

        if (!(splitDegreeMinSec.Length == 4 && string.IsNullOrWhiteSpace(splitDegreeMinSec[3]))) return defaultOut;

        if (double.TryParse(splitDegreeMinSec[0], out var degree) &&
            double.TryParse(splitDegreeMinSec[1], out var minutes) &&
            double.TryParse(splitDegreeMinSec[2], out var seconds))
        {
            return degree + (minutes / 60.0) + (seconds / 3600.0);
        }
        return defaultOut;
    }

    public static int GetLocationsHashCode(Locations locations) => ($"{locations.Mgrs}{locations.Altitude}").GetHashCode();

    public static string GetMgrs(double lat, double lon)
    {
        if (!ValidateLatLon(lat, lon)) return "INVALID";

        var el = new EagerLoad(EagerLoadType.UTM_MGRS);
        return new Coordinate(lat, lon, el).MGRS.ToString();
    }

    public static bool ValidateLatLon(double lat, double lon) => lat is >= -90 and <= 90 && lon is >= -180 and <= 180;

    public static string NormalizeTimeStr(string timeStr)
    {
        if (string.IsNullOrEmpty(timeStr)) return "";


        if (long.TryParse(timeStr, out var unixTime))
        {
            return unixTime == 0 ? "" : DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("yyyy/MM/dd");
        }

        var split = timeStr.Split(' ');
        if (split.Length >= 3) timeStr = $"{split[0]} {split[1]}";

        return DateTime.TryParse(Regex.Replace(timeStr, @"([(UTC]+)([+]|[-])([0-9]+)[)]", "", RegexOptions.None, TimeSpan.FromMilliseconds(100)), out var timestamp)
            ? timestamp.ToString("yyyy/MM/dd") : "";
    }

    public static (string sofex, string exhibit, string deviceType) ParseReportFileName(string reportName)
    {
        var defaultReturn = ("", "", "");

        reportName = Regex.Replace(reportName, @"(.*?)\\", "", RegexOptions.None, TimeSpan.FromMilliseconds(100));

        if (string.IsNullOrEmpty(reportName)) return defaultReturn;

        var splitFile = reportName.Split('_');

        return splitFile.Length switch
        {
            < 4 => defaultReturn,
            >= 5 => !splitFile[0].Contains("SOFEX")
                ? (splitFile[0],
                    Regex.Replace(splitFile[1], @"([^0-9])+", "", RegexOptions.None, TimeSpan.FromMilliseconds(100)),
                    splitFile[2])
                : (splitFile[1],
                    Regex.Replace(splitFile[2], @"([^0-9])+", "", RegexOptions.None, TimeSpan.FromMilliseconds(100)),
                    splitFile[3]),
            _ => splitFile.Length == 4 ? (splitFile[0], splitFile[1][2..], splitFile[2]) : defaultReturn
        };
    }

    public static (string BSSID, string SSID) GetBssidSSid(string data)
    {
        (string bssid, string ssid) outString = ("", "");

        if (string.IsNullOrEmpty(data)) return outString;

        var bssidPattern = new Regex(@"BSSID:(?:$|\W)([0-9A-Fa-f]{2}([:-]|$|)){6}", RegexOptions.None, TimeSpan.FromMilliseconds(100));

        outString.bssid = bssidPattern.Match(data).ToString().Trim();

        var containsSsid = data.Contains("SSID:");
        if (!string.IsNullOrEmpty(outString.bssid) && containsSsid)
        {
            outString.ssid = data.Replace(outString.bssid, "\r").Trim();
        }
        else if (containsSsid)
        {
            outString.ssid = data;
        }

        return outString;
    }

    public static double? TryConvertToDouble(string toConvert)
    {
        try
        {
            if (double.TryParse(toConvert, out var result)) return result;
        }
        catch
        {
            // ignored
        }

        return null;
    }
}