using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace Pages
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Document doc = new Document();
            try
            {
                PdfWriter.GetInstance(doc, new FileStream(@"..\..\Images.pdf", FileMode.Create));
                doc.Open();

                // Image 001
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(@"..\..\images\image001.png");
                gif.ScalePercent(16f);
                gif.Border = iTextSharp.text.Rectangle.BOX;
                gif.BorderColor = BaseColor.BLACK;
                gif.BorderWidth = 8f;
                gif.SetAbsolutePosition(32f, doc.PageSize.Height - 0.16f * gif.Height - 64f);
                doc.Add(gif);

                doc.NewPage();

                // Image 002
                iTextSharp.text.Image gif2 = iTextSharp.text.Image.GetInstance(@"..\..\images\image002.png");
                gif2.ScalePercent(16f);
                gif2.Border = iTextSharp.text.Rectangle.BOX;
                gif2.BorderColor = BaseColor.BLACK;
                gif2.BorderWidth = 8f;
                gif2.SetAbsolutePosition(32f + 0.16f * gif.Width + 10f, doc.PageSize.Height - 0.16f * gif2.Height - 64f);
                doc.Add(gif2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                doc.Close();
            }
        }
    }
}
