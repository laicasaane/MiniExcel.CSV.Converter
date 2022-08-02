using UnityEngine.UIElements;

namespace MiniExcelLibs.Controls
{
    partial class FolderBrowser
    {
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription _label = new() {
                name = "label",
                defaultValue = ""
            };

            readonly UxmlBoolAttributeDescription _multiline = new() {
                name = "multiline",
                defaultValue = false
            };

            readonly UxmlStringAttributeDescription _buttonText = new() {
                name = "button-text",
                defaultValue = "Browse"
            };

            readonly UxmlStringAttributeDescription _bindingPath = new() {
                name = "binding-path",
                defaultValue = string.Empty
            };

            readonly UxmlStringAttributeDescription _browserTitle = new() {
                name = "browser-title",
                defaultValue = BROWSER_TITLE
            };

            readonly UxmlBoolAttributeDescription _isRelativePath = new() {
                name = "is-relative-path",
                defaultValue = false
            };

            readonly UxmlBoolAttributeDescription _showAbsolutePath = new() {
                name = "show-absolute-path",
                defaultValue = false
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var browser = ve as FolderBrowser;

                browser.label = _label.GetValueFromBag(bag, cc);
                browser.multiline = _multiline.GetValueFromBag(bag, cc);
                browser.buttonText = _buttonText.GetValueFromBag(bag, cc);
                browser.bindingPath = _bindingPath.GetValueFromBag(bag, cc);
                browser.browserTitle = _browserTitle.GetValueFromBag(bag, cc);
                browser.isRelativePath = _isRelativePath.GetValueFromBag(bag, cc);
                browser.showAbsolutePath = _showAbsolutePath.GetValueFromBag(bag, cc);
            }
        }
    }
}