using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ClientDatabaseApp.Service
{
    public static class RichTextBoxHelper
    {
        public static string GetTextFromRichTextBox(RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            if (string.IsNullOrEmpty(textRange.Text))
                return null;
            return textRange.Text.Trim();
        }
    }
}
