using Yipper;
using static Mongrel.Common.Utils;

namespace Mongrel.Outputs.OutputTypes;

public class Kml : OutputFile
{
    private readonly FileStream _fileStream;
    private readonly StreamWriter _writer;
    public string FilePath;

    public Kml(string filePath)
    {
        FilePath = filePath;

        if (File.Exists(filePath)) File.Delete(filePath);

        _fileStream = File.OpenWrite(FilePath);
        _writer = new StreamWriter(_fileStream);

        WriteKmlHeader();
    }

    public override void WriteLocation(Locations location)
    {
        if (!ValidateLatLon(location.ConvertedLat, location.ConvertedLon))
        {
            Logger.Instance.Warning($"Invalid lat/lon skipping in KML write : {location.ConvertedLat}, {location.ConvertedLon}");
            return;
        }

        WritePlaceMark(location);
    }

    private void WriteKmlHeader()
    {
        _writer.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        _writer.WriteLine(@"<kml xmlns=""http://www.opengis.net/kml/2.2"">");
        WriteStartOrEndTag("Document", true);
    }

    private void WritePlaceMark(Locations location)
    {
        WriteStartOrEndTag("Placemark", true);

        WriteInlineTag("name", location.Origin);
        WriteInlineTag("description", location.Notes);
        WritePoint(location.ConvertedLat, location.ConvertedLon, TryConvertToDouble(location.Altitude));

        WriteStartOrEndTag("Placemark", false);
    }

    private void WritePoint(double latitude, double longitude, double? altitude)
    {
        WriteStartOrEndTag("Point");

        WriteInlineTag("coordinates", $"{longitude},{latitude},{altitude ?? 0}");

        WriteStartOrEndTag("Point", false);
    }

    private void WriteInlineTag(string tagName, string tagValue)
    {
        _writer.WriteLine($"<{tagName}>{tagValue}</{tagName}>");
    }

    private void WriteStartOrEndTag(string tag, bool start = true)
    {
        var check = start ? "" : "/";
        _writer.WriteLine($"<{check}{tag}>");
    }

    public void WriteEnders()
    {
        WriteStartOrEndTag("Document", false);
        WriteStartOrEndTag("kml", false);
    }

    public override void Dispose()
    {
        WriteEnders();
        _writer?.Dispose();
        _fileStream?.Dispose();
    }
}