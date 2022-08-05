using System.Collections.Generic;
using UnityEngine;
using MiniExcelLibs.Csv.Converter;

namespace Project.Editor
{
    [CreateAssetMenu(fileName = nameof(CsvPostProcessor),
                     menuName = "MiniExcel/CSV Post Processor", order = 1)]
    public class CsvPostProcessor : MiniExcelCsvPostProcessor
    {
        public override void Process(IReadOnlyList<CsvFileData> csvFiles)
        {
            foreach (var file in csvFiles)
            {
                Debug.Log(file.Path);
            }
        }
    }
}
