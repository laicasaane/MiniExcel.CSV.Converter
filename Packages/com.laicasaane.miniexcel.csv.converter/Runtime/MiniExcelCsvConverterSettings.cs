using System.Collections.Generic;
using UnityEngine;

namespace MiniExcelLibs.Csv.Converter
{
    [CreateAssetMenu(fileName = nameof(MiniExcelCsvConverterSettings), 
                     menuName = "MiniExcel/CSV Converter Settings", order = 1)]
    public class MiniExcelCsvConverterSettings : ScriptableObject
    {
        [SerializeField]
        internal string _relativeExcelFolderPath = ".";

        [SerializeField]
        internal string _relativeCsvFolderPath = ".";

        [SerializeField]
        internal string _ignoreSheetsStartWith = "";

        [SerializeField]
        internal List<FileData> _excelFiles = new();

        internal void CopyFilesToMap(Dictionary<string, bool> map)
        {
            if (map == null) return;

            foreach (var (path, selected) in _excelFiles)
            {
                map[path] = selected;
            }
        }

        internal void ApplyMapToFiles(Dictionary<string, bool> map)
        {
            if (map == null) return;

            _excelFiles.Clear();

            foreach (var (path, selected) in map)
            {
                _excelFiles.Add(new FileData { path = path, selected = selected });
            }
        }
    }
}
