namespace MiniExcelLibs.Csv.Converter
{
    public readonly struct CsvFileData
    {
        public readonly string Name;
        public readonly string Path;
        public readonly string ExcelPath;

        public CsvFileData(string name, string path, string excelPath)
        {
            Name = name;
            Path = path;
            ExcelPath = excelPath;
        }
    }
}
