using System.Collections.Generic;
using UnityEngine;

namespace MiniExcelLibs.Csv.Converter
{
    public abstract class MiniExcelCsvPostProcessor : ScriptableObject
    {
        /// <summary>
        /// Process CSV files after the conversion.
        /// </summary>
        /// <param name="csvFiles">The paths of exported CSV files.</param>
        public abstract void Process(IReadOnlyList<CsvFileData> csvFiles);
    }
}
