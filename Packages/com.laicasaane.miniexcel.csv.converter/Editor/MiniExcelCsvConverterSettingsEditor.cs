using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using MiniExcelLibs.Controls;
using MiniExcelLibs.Controls.Editor;
using MiniExcelLibs.Utilities;

namespace MiniExcelLibs.Csv.Converter
{
    [CustomEditor(typeof(MiniExcelCsvConverterSettings))]
    public class MiniExcelCsvConverterSettingsEditor : UnityEditor.Editor
    {
        public static readonly string folderBrowserExcelUssName = "folder-browser-excel";
        public static readonly string ignoreExcelsUssName = "ignore-excels";
        public static readonly string folderBrowserCsvUssName = "folder-browser-csv";
        public static readonly string ignoreSheetsUssName = "ignore-sheets";
        public static readonly string postProcessorUssName = "post-processor";
        public static readonly string fileListUssName = "file-list";
        public static readonly string fileItemUssClassName = "file-list__item";
        public static readonly string openExcelButtonUssName = "open-excel-button";
        public static readonly string locateExcelButtonUssName = "locate-excel-button";
        public static readonly string locateCsvButtonUssName = "locate-csv-button";
        public static readonly string refreshButtonUssName = "refresh-button";
        public static readonly string convertButtonUssName = "convert-button";

        [SerializeField]
        private VisualTreeAsset _visualTree;

        [SerializeField]
        private ThemeStyleSheet _darkTheme;

        [SerializeField]
        private ThemeStyleSheet _lightTheme;

        private readonly Dictionary<string, bool> _fileMapPrev = new();
        private readonly Dictionary<string, bool> _fileMapNew = new();

        private MiniExcelCsvConverterSettings _settings;
        private ListView _excelFileList;
        private bool _isConverting;

        public override VisualElement CreateInspectorGUI()
        {
            _settings = target as MiniExcelCsvConverterSettings;

            var root = new VisualElement();
            _visualTree.CloneTree(root);

            root.styleSheets.Add(EditorGUIUtility.isProSkin ? _darkTheme : _lightTheme);

            /// FOLDER BROWSER --- EXCEL
            {
                var folderBrowserExcel = root.Q<FolderBrowser>(folderBrowserExcelUssName);

                if (folderBrowserExcel != null)
                {
                    folderBrowserExcel.openFolderHandler = new EditorOpenFolderHandler();
                    folderBrowserExcel.bindingPath = nameof(MiniExcelCsvConverterSettings._relativeExcelFolderPath);
                    folderBrowserExcel.onValueChanged += FolderBrowserExcel_onValueChanged;
                }
            }

            /// IGNORE SHEETS START WITH CHARACTERS
            {
                var ignoreExcels = root.Q<TextField>(ignoreExcelsUssName);

                if (ignoreExcels != null)
                {
                    ignoreExcels.bindingPath = nameof(MiniExcelCsvConverterSettings._ignoreExcelsStartWith);
                }
            }

            /// FOLDER BROWSER --- CSV
            {
                var folderBrowserCsv = root.Q<FolderBrowser>(folderBrowserCsvUssName);

                if (folderBrowserCsv != null)
                {
                    folderBrowserCsv.openFolderHandler = new EditorOpenFolderHandler();
                    folderBrowserCsv.bindingPath = nameof(MiniExcelCsvConverterSettings._relativeCsvFolderPath);
                    folderBrowserCsv.onValueChanged += FolderBrowserCsv_onValueChanged;
                }
            }

            /// IGNORE SHEETS START WITH CHARACTERS
            {
                var ignoreSheets = root.Q<TextField>(ignoreSheetsUssName);

                if (ignoreSheets != null)
                {
                    ignoreSheets.bindingPath = nameof(MiniExcelCsvConverterSettings._ignoreSheetsStartWith);
                }
            }

            /// POST-PROCESSOR
            {
                var postProcessor = root.Q<ObjectField>(postProcessorUssName);

                if (postProcessor != null)
                {
                    postProcessor.objectType = typeof(MiniExcelCsvPostProcessor);
                    postProcessor.bindingPath = nameof(MiniExcelCsvConverterSettings._postProcessor);
                }
            }

            /// REFRESH EXCEL FILES BUTTON
            {
                var refreshButton = root.Q<Button>(refreshButtonUssName);

                if (refreshButton != null)
                    refreshButton.clicked += RefreshButton_clicked;
            }

            /// CONVERT TO CSV
            {
                var convertButton = root.Q<Button>(convertButtonUssName);

                if (convertButton != null)
                    convertButton.clicked += ConvertButton_clicked;
            }

            /// FILE LIST
            {
                _excelFileList = root.Q<ListView>(fileListUssName);

                if (_excelFileList != null)
                {
                    _excelFileList.bindingPath = nameof(MiniExcelCsvConverterSettings._excelFiles);
                    _excelFileList.makeItem = ExcelFileList_MakeItem;
                    _excelFileList.bindItem = ExcelFileList_BindItem;
                }
            }

            root.Bind(serializedObject);
            return root;
        }

        private void FolderBrowserExcel_onValueChanged(string absolutePath, string relativePath)
        {
            if (_settings is null)
                return;

            _settings._relativeExcelFolderPath = relativePath;

            RefreshFileList(absolutePath);

            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssetIfDirty(target);
        }

        private void FolderBrowserCsv_onValueChanged(string absolutePath, string relativePath)
        {
            if (_settings is null)
                return;

            _settings._relativeCsvFolderPath = relativePath;

            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssetIfDirty(target);
        }

        private void RefreshFileList(string absolutePath)
        {
            _fileMapPrev.Clear();
            _fileMapNew.Clear();
            _settings.CopyFilesToMap(_fileMapPrev);

            var ignoreStartPattern = _settings._ignoreExcelsStartWith;

            foreach (var filePath in Directory.EnumerateFiles(absolutePath, MiniExcelCsvConverter.EXCEL_FILTER))
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);

                if (string.IsNullOrWhiteSpace(ignoreStartPattern) == false
                    && fileName.StartsWith(ignoreStartPattern))
                    continue;

                if (_fileMapPrev.TryGetValue(fileName, out var selected) == false)
                    selected = true;

                _fileMapNew[fileName] = selected;
            }

            _settings.ApplyMapToFiles(_fileMapNew);
        }

        private VisualElement ExcelFileList_MakeItem()
        {
            var fileItem = new BindableElement();
            fileItem.AddToClassList(fileItemUssClassName);
            fileItem.Add(new Toggle { bindingPath = nameof(FileData.selected) });
            fileItem.Add(new Label { bindingPath = nameof(FileData.path) });

            var openExcelButton = new Button {
                text = "Open",
                name = openExcelButtonUssName
            };

            var locateExcelButton = new Button {
                text = "Locate",
                name = locateExcelButtonUssName
            };

            var locateCsvButton = new Button {
                text = "CSV",
                name = locateCsvButtonUssName
            };

            fileItem.Add(openExcelButton);
            fileItem.Add(locateExcelButton);
            fileItem.Add(locateCsvButton);

            return fileItem;
        }

        private void ExcelFileList_BindItem(VisualElement ve, int index)
        {
            if (!(ve is BindableElement fileItem) 
                || _settings is null 
                || _excelFileList is null
                || _excelFileList.itemsSource is null)
                return;

            var itemsSource = _excelFileList.itemsSource;

            if ((uint)index >= (uint)itemsSource.Count)
                return;

            var fileProp = (SerializedProperty)itemsSource[index];
            fileItem.bindingPath = fileProp.propertyPath;
            fileItem.Bind(fileProp.serializedObject);

            var openExcelButton = fileItem.Q<Button>(openExcelButtonUssName);

            if (openExcelButton != null)
                openExcelButton.clicked += () => OpenExcelFileAt(index);

            var locateExcelButton = fileItem.Q<Button>(locateExcelButtonUssName);

            if (locateExcelButton != null)
                locateExcelButton.clicked += () => LocateExcelFileAt(index);

            var locateCsvButton = fileItem.Q<Button>(locateCsvButtonUssName);

            if (locateCsvButton != null)
                locateCsvButton.clicked += () => LocateCSVFolderAt(index);
        }

        private void RefreshButton_clicked()
        {
            if (_settings is null)
                return;

            RefreshFileList(PathUtility.GetAbsolutePath(_settings._relativeExcelFolderPath));

            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssetIfDirty(target);
        }

        private bool TryGetExcelFilePathAt(int index, out string filePath)
        {
            var files = _settings._excelFiles;

            if ((uint)index >= (uint)files.Count)
            {
                filePath = string.Empty;
                return false;
            }

            var file = files[index];
            var folderPath = PathUtility.GetAbsolutePath(_settings._relativeExcelFolderPath);
            filePath = Path.Combine(folderPath, $"{file.path}{MiniExcelCsvConverter.EXCEL_EXT}").ToPlatformPath();
            return true;
        }

        private bool TryGetCsvFolderPathAt(int index, out string path, out string rootPath)
        {
            var files = _settings._excelFiles;

            if ((uint)index >= (uint)files.Count)
            {
                rootPath = string.Empty;
                path = string.Empty;
                return false;
            }

            var file = files[index];
            rootPath = PathUtility.GetAbsolutePath(_settings._relativeCsvFolderPath).ToPlatformPath();
            path = Path.Combine(rootPath, file.path);
            return true;
        }

        private void OpenExcelFileAt(int index)
        {
            if (TryGetExcelFilePathAt(index, out var filePath))
            {
                System.Diagnostics.Process.Start(filePath);
            }
        }

        private void LocateExcelFileAt(int index)
        {
            if (TryGetExcelFilePathAt(index, out var filePath))
            {
                LocateFileInExplorer(filePath);
            }
        }

        private void LocateCSVFolderAt(int index)
        {
            if (TryGetCsvFolderPathAt(index, out var folderPath, out var rootPath))
            {
                if (Directory.Exists(folderPath))
                    LocateFolderInExplorer(folderPath);
                else
                    LocateFolderInExplorer(rootPath);
            }
        }

        private static void LocateFolderInExplorer(string path)
        {
#if UNITY_EDITOR_WIN
            System.Diagnostics.Process.Start("explorer.exe", path);
#elif UNITY_EDITOR_OSX
            var argument = $"-R \"{path}\"";
            System.Diagnostics.Process.Start("open", argument);
#endif
        }

        private static void LocateFileInExplorer(string path)
        {
#if UNITY_EDITOR_WIN
            var argument = $"/e,/select,\"{path}\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
#elif UNITY_EDITOR_OSX
            var argument = $"-R \"{path}\"";
            System.Diagnostics.Process.Start("open", argument);
#endif
        }

        private void ConvertButton_clicked()
        {
            if (_isConverting)
                return;

            _isConverting = true;

            MiniExcelCsvConverter.Convert(_settings);

            _isConverting = false;
        }
    }
}
