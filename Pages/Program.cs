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
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("STARTING");
      Document doc = new Document(PageSize.A4);
      try
      {
          PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(@"Images.pdf", FileMode.Create));
          doc.Open();
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.Load(@"bd.xml");
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
      Console.WriteLine("DONE");
    }
  }
}
