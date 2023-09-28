using System;
using System.Collections.Generic;

namespace Mongrel.Inputs.ReportReaders
{
    public abstract class ReportReader : IDisposable
    {
        protected ExcelDocument Document;
        protected IEnumerable<(string name, int index)> ValidSheetAndIndex;

        protected ReportReader(ExcelDocument document, IEnumerable<(string name, int index)> validSheetAndIndex)
        {
            Document = document;
            ValidSheetAndIndex = validSheetAndIndex;
        }
        public abstract IEnumerable<Locations> GetLocations();

        public static IEnumerable<Dictionary<string, string>> GetDirtyLocationDict(IEnumerable<(string sheetName, List<string> columnNames, IEnumerable<IEnumerable<string>> values)> sheets)
        {
            foreach (var (sheetName, columnNames, values) in sheets)
            {
                foreach (var dirtyLocation in values)
                {
                    var outputDictionary = new Dictionary<string, string> { { "sheetName", sheetName } };
                    var index = 0;

                    foreach (var item in dirtyLocation)
                    {
                        if (index >= columnNames.Count) break;

                        try
                        {
                            outputDictionary.Add(columnNames[index], item ?? "");
                        }
                        catch (ArgumentException)
                        {
                            outputDictionary[columnNames[index]] ??= item;
                        }
                        index++;
                    }
                    yield return outputDictionary;
                }
            }
        }

        public void Dispose()
        {
            Document?.Dispose();
        }
    }
}