using CommandLine;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using System;
using System.Collections.Generic;

namespace Pages
{
    class Program
    {
        public class Options
        {
            [Option('c', "config", Required = true, HelpText = "Path to the XML file")]
            public string Config { get; set; } = string.Empty;

            [Option('i', "images", Required = true, HelpText = "Path to the folder containing the images")]
            public string Images { get; set; } = string.Empty;

            [Option('o', "output", Default = "Images.pdf", HelpText = "Name of the output PDF")]
            public string Output { get; set; } = string.Empty;
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
            PdfWriter writer = new PdfWriter(@"" + opts.Output);
            PdfDocument pdfDocument = new PdfDocument(writer);
            pdfDocument.SetDefaultPageSize(PageSize.A4);
            Document document = new Document(pdfDocument);
            Comic comic = new Comic(opts.Config, opts.Images);
            comic.Render(document);
            document.Close();
            pdfDocument.Close();
            Console.WriteLine("Generated " + opts.Output);
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (Error err in errs)
            {
                Console.WriteLine("Error", err.ToString());
            }
        }
    }
}
