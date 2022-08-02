using UnityEngine.UIElements;
using MiniExcelLibs.Utilities;

namespace MiniExcelLibs.Controls
{
    public delegate void FolderBrowserValueChanged(string absolutePath, string relativePath);

    public partial class FolderBrowser : VisualElement
    {
        public static readonly string ussClassName = "folder-browser";
        public static readonly string rootGroupUssClassName = "root";
        public static readonly string inputGroupUssClassName = "input-group";
        public static readonly string absolutePathGroupUssClassName = "absolute-path-group";
        public static readonly string hiddenUssClassName = "hidden";

        private const string BROWSER_TITLE = "Select Folder";

        private readonly TextField _textField;
        private readonly Button _button;
        private readonly VisualElement _absolutePathGroup;
        private readonly Label _absolutePath;

        public IOpenFolderHandler openFolderHandler { get; set; }

        public string label
        {
            get => _textField.label;
            set => _textField.label = value;
        }

        public bool multiline
        {
            get => _textField.multiline;
            set => _textField.multiline = value;
        }

        public string buttonText
        {
            get => _button.text;
            set => _button.text = value;
        }

        public string text
        {
            get => _textField.text;
        }

        public string bindingPath
        {
            get => _textField.bindingPath;
            set => _textField.bindingPath = value;
        }

        public string browserTitle { get; set; }

        public bool isRelativePath { get; set; }

        public bool showAbsolutePath
        {
            get => _absolutePathGroup.visible;

            set
            {
                _absolutePathGroup.visible = value;

                if (value)
                    _absolutePathGroup.RemoveFromClassList(hiddenUssClassName);
                else
                    _absolutePath.AddToClassList(hiddenUssClassName);
            }
        }

        public event FolderBrowserValueChanged onValueChanged;

        public FolderBrowser() : this(null) { }

        public FolderBrowser(string label)
        {
            AddToClassList(ussClassName);

            var root = new VisualElement();
            root.AddToClassList(rootGroupUssClassName);

            var inputGroup = new VisualElement();
            inputGroup.AddToClassList(inputGroupUssClassName);

            _textField = new TextField(label) {
                isReadOnly = true
            };

            _textField.RegisterValueChangedCallback(TextField_valueChanged);

            _button = new Button();
            _button.clicked += Button_clicked;

            inputGroup.Add(_textField);
            inputGroup.Add(_button);

            _absolutePathGroup = new VisualElement {
                visible = false
            };

            _absolutePathGroup.AddToClassList(absolutePathGroupUssClassName);
            _absolutePathGroup.AddToClassList(hiddenUssClassName);

            _absolutePath = new Label();
            _absolutePathGroup.Add(_absolutePath);

            root.Add(inputGroup);
            root.Add(_absolutePathGroup);
            Add(root);
        }

        private void TextField_valueChanged(ChangeEvent<string> ev)
        {
            if (showAbsolutePath == false)
                return;

            _absolutePath.text = isRelativePath ? PathUtility.GetAbsolutePath(ev.newValue) : ev.newValue;
        }

        private void Button_clicked()
        {
            var absolutePath = isRelativePath ? PathUtility.GetAbsolutePath(_textField.text) : _textField.text;

            if (System.IO.Directory.Exists(absolutePath) == false)
                absolutePath = PathUtility.GetRootPath();

            if (openFolderHandler != null)
                absolutePath = openFolderHandler.OpenFolder(browserTitle ?? BROWSER_TITLE, absolutePath, "");
            
            if (string.IsNullOrEmpty(absolutePath) == false)
                onValueChanged?.Invoke(absolutePath, PathUtility.GetRelativePath(absolutePath));
        }
    }
}
