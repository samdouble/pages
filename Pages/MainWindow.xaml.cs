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

            Document doc = new Document();
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(@"..\..\..\..\bd1\BD1\Images.pdf", FileMode.Create));
                doc.Open();

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(@"..\..\..\..\bd1\BD1\bd.xml");
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
            System.Diagnostics.Process.Start(@"..\..\..\..\bd1\BD1\Images.pdf");
        }
    }
}
