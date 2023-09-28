namespace Mongrel.Common
{
    internal class InputFileFormats
    {
        public enum FileFormats
        {
            Unknown = -1,
            Csv = 0,
            Excel = 1,
        }

        public static FileFormats GetFileFormatsFromName(string name)
        {
            return name switch
            {
                ".csv" => FileFormats.Csv,
                ".xlsx" => FileFormats.Excel,
                ".xls" => FileFormats.Excel,
                _ => FileFormats.Unknown
            };
        }
    }
}