using System;

namespace MiniExcelLibs.Csv.Converter
{
    [Serializable]
    public class FileData
    {
        public string path = "";
        public bool selected = false;

        public void Deconstruct(out string path, out bool selected)
        {
            path = this.path;
            selected = this.selected;
        }
    }
}
