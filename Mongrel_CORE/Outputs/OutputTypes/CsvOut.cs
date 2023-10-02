using System.Globalization;
using System.Text;
using CsvHelper;

namespace Mongrel.Outputs.OutputTypes;

public class CsvOut : OutputFile
{
    public string FilePath;
    public int LastRowId;
    private readonly StreamWriter _streamWriter;
    private readonly CsvWriter _csvWriter;

    public CsvOut(string filePath)
    {
        FilePath = filePath;
        LastRowId = 0;

        if (File.Exists(FilePath)) File.Delete(FilePath);

        _streamWriter = new StreamWriter(FilePath, true, Encoding.UTF8);
        _csvWriter = new CsvWriter(_streamWriter, CultureInfo.InvariantCulture);

        _csvWriter.WriteHeader<CsvRecord>();
    }

    public override void WriteLocation(Locations rowData)
    {
        LastRowId++;

        var record = new CsvRecord(rowData, LastRowId);

        _csvWriter.NextRecord();
        _csvWriter.WriteRecord(record);
    }

    public override void Dispose()
    {
        _csvWriter?.Dispose();
        _streamWriter?.Dispose();
    }
}