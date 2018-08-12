using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace HCIProject
{
    /// <summary>
    /// Interaction logic for mailSystemUC.xaml
    /// </summary>
    public partial class mailSystemUC : UserControl
    {
        public mailSystemUC()
        {
            InitializeComponent();            
            
            cmbFontFamily.SelectionChanged += (s, e) =>
              emailContent.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, e.AddedItems[0]);

            cmbFontSize.SelectionChanged += (s, e) =>
             emailContent.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, e.AddedItems[0]);
            
        }

        private void btnBold_Click(object sender, RoutedEventArgs e)
        {
            emailContent.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
        }

        private void btnItalic_Click(object sender, RoutedEventArgs e)
        {
            emailContent.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
        }

        private void btnUnderline_Click(object sender, RoutedEventArgs e)
        {
            emailContent.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
        }

        private void btnClearAll_Click(object sender, RoutedEventArgs e)
        {
            emailContent.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
            emailContent.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
            emailContent.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            emailContent.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, "Arial");
            emailContent.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, "12");
            emailContent.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
        }
        
        public void viewEmail(EmailItem myEmail) {

            using (MemoryStream ms = new MemoryStream())
            {
                TextRange tr = new TextRange(myEmail.messageBody.Start, myEmail.messageBody.End);
                tr.Save(ms, DataFormats.Xaml);
                ms.Seek(0, SeekOrigin.Begin);
                tr = new TextRange(emailContent.CaretPosition, emailContent.CaretPosition);
                tr.Load(ms, DataFormats.Xaml);
            }
        }

        private void _SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            emailContent.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, _colorPicker.SelectedColorText);
        }
    }
}
