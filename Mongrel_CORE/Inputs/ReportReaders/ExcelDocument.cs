using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExcelDataReader;
using Yipper;

namespace Mongrel.Inputs.ReportReaders
{
    public class ExcelDocument : IDisposable
    {
        public string FileName;
        public IEnumerable<string> SheetNames;
        private readonly FileStream _stream;
        private readonly IExcelDataReader _reader;

        public ExcelDocument(string fileName)
        {
            FileName = fileName;
            _stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
            SheetNames = GetSheetNames();

            _stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
            _reader = ExcelReaderFactory.CreateReader(_stream);
        }

        public IEnumerable<string> GetSheetNames()
        {
            var columnNames = new List<string>();
            using var columnReader = ExcelReaderFactory.CreateReader(_stream);
            try
            {
                do
                {
                    columnNames.Add(columnReader.Name);
                }
                while (columnReader.NextResult());
            }
            catch (Exception)
            {
                Logger.Instance.Error($"Error in reading sheet names for file {FileName}");
            }
            finally
            {
                columnReader?.Dispose();
            }
            columnReader?.Dispose();
            return columnNames;
        }

        public IEnumerable<(string sheetName, List<string> columnNames, IEnumerable<IEnumerable<string>> values)> GetRowsFromValidSheets(IEnumerable<(string name, int index)> validSheetAndIndex)
        {
            (string sheetName, List<string> columnNames, IEnumerable<IEnumerable<string>> rowValues) outValues = (null, null, null);

            foreach (var sheet in ValidSheetAndIndexes(validSheetAndIndex))
            {
                SeekToHeaderRow(sheet.index);
                outValues.sheetName = sheet.name;
                outValues.columnNames = GetRowValues().ToList();
                outValues.rowValues = GetRowsFromSheet();
                yield return outValues;
            }
        }

        private IEnumerable<(string name, int index)> ValidSheetAndIndexes(IEnumerable<(string name, int index)> validSheetAndIndexes)
        {
            var crazyLatch = true;

            if (crazyLatch)
            {
                crazyLatch = false;
                foreach (var validSheetAndIndex in validSheetAndIndexes
                             .Where(validSheetAndIndex => validSheetAndIndex.name.Equals(_reader.Name)))
                {
                    yield return validSheetAndIndex;
                }
            }

            while (_reader.NextResult())
            {
                foreach (var validSheetAndIndex in validSheetAndIndexes
                             .Where(validSheetAndIndex => validSheetAndIndex.name.Equals(_reader.Name)))
                {
                    yield return validSheetAndIndex;
                }
            }
        }

        private void SeekToHeaderRow(int index)
        {
            for (var i = -1; i < index; i++)
            {
                _reader.Read();
            }
        }

        private IEnumerable<IEnumerable<string>> GetRowsFromSheet()
        {
            while (_reader.Read())
            {
                yield return GetRowValues();
            }
        }

        private IEnumerable<string> GetRowValues()
        {
            var columnCount = _reader.FieldCount;

            for (var i = 0; i < columnCount; i++)
            {
                var readValue = _reader.GetValue(i);
                yield return readValue?.ToString();
            }
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _reader?.Dispose();
        }
    }
}
