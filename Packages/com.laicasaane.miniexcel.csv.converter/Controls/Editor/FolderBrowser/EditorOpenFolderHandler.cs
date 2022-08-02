using UnityEditor;

namespace MiniExcelLibs.Controls.Editor
{
    public class EditorOpenFolderHandler : IOpenFolderHandler
    {
        public string OpenFolder(string title, string folder, string defaultName)
            => EditorUtility.OpenFolderPanel(title, folder, defaultName);
    }
}
