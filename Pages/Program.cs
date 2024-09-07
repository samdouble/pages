using CommandLine;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

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
            Document doc = new Document(PageSize.A4);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Comic));

                FileStream fs = new FileStream(opts.Config, FileMode.Open);
                XmlReader reader = XmlReader.Create(fs);
                Comic comic = (Comic)serializer.Deserialize(reader);
                fs.Close();

                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(@"" + opts.Output, FileMode.Create));
                doc.Open();
                comic?.SetImagesFolderPath(opts.Images);
                comic?.Render(doc, writer, comic);
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
            foreach (Error err in errs)
            {
                Console.WriteLine("Error", err.ToString());
            }
        }
    }
}
