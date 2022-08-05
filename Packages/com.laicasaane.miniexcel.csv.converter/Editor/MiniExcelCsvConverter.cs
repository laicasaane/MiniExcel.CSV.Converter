using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniExcelLibs.Utilities;
using UnityEditor;

namespace MiniExcelLibs.Csv.Converter
{
    public static class MiniExcelCsvConverter
    {
        public const string EXCEL_EXT = ".xlsx";
        public const string CSV_EXT = ".csv";
        public const string EXCEL_FILTER = "*.xlsx";

        private const string PROGRESS_TITLE = "Excel to CSV Converter";

        public static void Convert(MiniExcelCsvConverterSettings settings)
        {
            var relativeCsvFolderPath = settings._relativeCsvFolderPath;
            var absoluteCsvFolderPath = PathUtility.GetAbsolutePath(relativeCsvFolderPath);
            var absoluteExcelFolderPath = PathUtility.GetAbsolutePath(settings._relativeExcelFolderPath);
            var ignoreSheetsStartWith = settings._ignoreSheetsStartWith;
            var files = settings._excelFiles;
            var filesToConvert = new List<string>();
            var csvFiles = new List<CsvFileData>();

            foreach (var file in files)
            {
                if (file.selected)
                    filesToConvert.Add(file.path);
            }

            //          Delete Folder + Create Folder + File Count
            var steps = 1             + 1             + filesToConvert.Count;
            var step = 1f / steps;

            for (var i = 0; i < steps; i++)
            {
                if (i == 0)
                {
                    DeleteCsvFolder(relativeCsvFolderPath, absoluteCsvFolderPath, step * i);
                    continue;
                }

                if (i == 1)
                {
                    CreateCsvFolder(relativeCsvFolderPath, absoluteCsvFolderPath, step * i);
                    continue;
                }

                var index = i - 2;
                var filePath = filesToConvert[index];

                ConvertCsv(
                    absoluteExcelFolderPath
                    , absoluteCsvFolderPath
                    , filePath
                    , ignoreSheetsStartWith
                    , step * i
                    , step
                    , csvFiles
                );
            }

            if (settings._postProcessor)
                settings._postProcessor.Process(csvFiles);
        }

        private static void DeleteCsvFolder(string relative, string absolute, float progress)
        {
            EditorUtility.DisplayProgressBar(PROGRESS_TITLE
                , $"Delete directory: {relative}"
                , progress
            );

            if (Directory.Exists(absolute))
            {
                Directory.Delete(absolute, true);
            }

            EditorUtility.ClearProgressBar();
        }

        private static void CreateCsvFolder(string relative, string absolute, float progress)
        {
            EditorUtility.DisplayProgressBar(PROGRESS_TITLE
                , $"Create directory: {relative}"
                , progress
            );

            if (Directory.Exists(absolute) == false)
            {
                Directory.CreateDirectory(absolute);
            }

            EditorUtility.ClearProgressBar();
        }

        private static void ConvertCsv(
            string excelFolderPath
            , string csvFolderPath
            , string filePath
            , string ignoreSheetsStartWith
            , float progress
            , float step
            , List<CsvFileData> csvFiles
        )
        {
            var excelFilePath = Path.Combine(excelFolderPath, $"{filePath}{EXCEL_EXT}").ToPlatformPath();

            if (File.Exists(excelFilePath) == false)
                return;

            using var stream = File.OpenRead(excelFilePath);

            var sheetNames = new List<string>();

            if (string.IsNullOrWhiteSpace(ignoreSheetsStartWith))
                MiniExcel.GetSheetNames(stream, sheetNames);
            else
                MiniExcel.GetSheetNames(stream, sheetNames, x => x.StartsWith(ignoreSheetsStartWith) == false);

            if (sheetNames.Count <= 0)
                return;

            var miniStep = step / sheetNames.Count;
            var sheetCsvFolderPath = Path.Combine(csvFolderPath, filePath).ToPlatformPath();

            if (Directory.Exists(sheetCsvFolderPath) == false)
                Directory.CreateDirectory(sheetCsvFolderPath);

            for (var i = 0; i < sheetNames.Count; i++)
            {
                var sheetName = sheetNames[i];

                EditorUtility.DisplayProgressBar(PROGRESS_TITLE
                    , $"Convert: {filePath} : {sheetName}"
                    , progress + (miniStep * i)
                );

                try
                {
                    var csvFilePath = Path.Combine(sheetCsvFolderPath, $"{sheetName}{CSV_EXT}");

                    using (var csvStream = new FileStream(csvFilePath, FileMode.CreateNew))
                    {
                        var rows = MiniExcel.Query(excelFilePath, false, sheetName, ExcelType.XLSX)
                        .Where(x => x.Keys.Any(k => x[k] != null));

                        MiniExcel.SaveAs(csvStream, rows, printHeader: false, excelType: ExcelType.CSV);
                    }

                    csvFiles.Add(new CsvFileData(sheetName, csvFilePath, filePath));
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }

                EditorUtility.ClearProgressBar();
            }
        }
    }
}
