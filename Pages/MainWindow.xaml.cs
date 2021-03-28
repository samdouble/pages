using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;

namespace Pages
{
    public partial class MainWindow : Window
    {  
        public MainWindow()
        {
            InitializeComponent();

            Document doc = new Document(PageSize.A4);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(@"..\..\..\..\BD\BD0\Images.pdf", FileMode.Create));
                doc.Open();

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(@"..\..\..\..\BD\BD0\bd.xml");
                Comic comic = new Comic(xmlDocument.DocumentElement);
                comic.Render(doc, writer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                doc.Close();
            }
            System.Diagnostics.Process.Start(@"..\..\..\..\BD\BD0\Images.pdf");
        }
    }
}
