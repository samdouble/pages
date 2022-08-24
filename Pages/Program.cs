using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using iText.Layout;
using iText.Kernel.Geom;
using PdfDocument = iText.Kernel.Pdf.PdfDocument;
using PdfWriter = iText.Kernel.Pdf.PdfWriter;

namespace Pages
{
  class Program
  {
    public class Options
    {
        [Option('c', "config", Required = true, HelpText = "Path to the XML file")]
        public string Config { get; set; }

        [Option('i', "images", Required = true, HelpText = "Path to the folder containing the images")]
        public string Images { get; set; }

        [Option('o', "output", Default = "Images.pdf", HelpText = "Name of the output PDF")]
        public string Output { get; set; }
    }

    static void Main(string[] args)
    {
      Parser.Default.ParseArguments<Options>(args)
        .WithParsed(RunOptions)
        .WithNotParsed(HandleParseError);
    }

    static void RunOptions(Options opts)
    {
      Console.WriteLine("Starting PDF generation...");
      PdfWriter pdfWriter = new PdfWriter(new FileStream(@"" + opts.Output, FileMode.Create));
      PdfDocument pdf = new PdfDocument(pdfWriter);
      Document doc = new Document(pdf);
      pdf.SetDefaultPageSize(PageSize.A4);
      try
      {
          Comic comic = new Comic(opts.Config, opts.Images);
          comic.Render(doc, pdfWriter);
      }
      catch (Exception ex)
      {
          Console.WriteLine(ex);
      }
      finally
      {
          doc.Close();
      }
      Console.WriteLine("Generated " + opts.Output);
    }

    static void HandleParseError(IEnumerable<Error> errs)
    {
      foreach (Error err in errs) {
         Console.WriteLine("Error", err.ToString());
      }
    }
  }
}
