using CommandLine.Text;
using CommandLine;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

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
      Document doc = new Document(PageSize.A4);
      try
      {
          PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(@"" + opts.Output, FileMode.Create));
          doc.Open();
          Comic comic = new Comic(opts.Config, opts.Images);
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
