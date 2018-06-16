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
using System.Xml;

namespace Pages
{
    public partial class MainWindow : Window
    {
        private float margeGauche = 5f;
        private float margeDroite = 5f;
        private float margeHaut = 64f;
        private float margeBas = 64f;

        private float espaceHEntreCases = 10f;
        private float espaceVEntreCases = 10f;

        public MainWindow()
        {
            InitializeComponent();

            Document doc = new Document();
            try
            {
                PdfWriter procEcriture = PdfWriter.GetInstance(doc, new FileStream(@"..\..\Images.pdf", FileMode.Create));
                doc.Open();

                float hauteurCase = (doc.PageSize.Height - margeHaut - margeBas - 3 * espaceVEntreCases) / 4;
                float x = 0;
                float y = 0;

                XmlDocument xml = new XmlDocument();
                xml.Load(@"..\..\..\..\bd1\bd.xml");
                foreach (XmlNode espace in xml.DocumentElement.ChildNodes)
                {
                    foreach (XmlNode casex in espace.ChildNodes)
                    {
                        Element item = null;
                        foreach (XmlNode element in casex.ChildNodes)
                        {
                            if (element.Name == "image")
                                item = new Image(element);
                            else if (element.Name == "texte")
                                item = new Texte(element);

                            item.Redimensionner(hauteurCase);
                            item.Decouper(procEcriture);

                            if (element.Name == "image")
                            {
                                if (x + item.getLargeur() > doc.PageSize.Width - margeDroite - margeGauche)
                                {
                                    x = 0;
                                    y += item.getHauteur() + espaceVEntreCases;
                                }
                            }
                            
                            item.Positionner(doc, x, y, margeHaut, margeGauche);

                            item.AjouterBordures();

                            doc.Add(item.getImage());

                            //doc.NewPage();
                        }
                        x += item.getLargeur() + espaceHEntreCases;
                    }
                }
                /*
                doc.NewPage();

                // Image 002
                iTextSharp.text.Image gif2 = iTextSharp.text.Image.GetInstance(@"..\..\images\image002.png");
                gif2.ScalePercent(16f);
                gif2.Border = iTextSharp.text.Rectangle.BOX;
                gif2.BorderColor = BaseColor.BLACK;
                gif2.BorderWidth = 8f;
                gif2.SetAbsolutePosition(32f + 0.16f * gif.Width + 10f, doc.PageSize.Height - 0.16f * gif2.Height - 64f);
                doc.Add(gif2);*/
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
