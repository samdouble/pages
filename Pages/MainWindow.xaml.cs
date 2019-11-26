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
        private float margeGauche = 25f;
        private float margeDroite = 25f;
        private float margeHaut = 64f;
        private float margeBas = 64f;

        private float espaceHEntreCases = 10f;
        private float espaceVEntreCases = 10f;

        private int nbRangeesParPage = 3;
   
        public MainWindow()
        {
            InitializeComponent();

            Document doc = new Document();
            try
            {
                PdfWriter procEcriture = PdfWriter.GetInstance(doc, new FileStream(@"..\..\..\..\bd1\BD1\Images.pdf", FileMode.Create));
                doc.Open();

                float hauteurCase = (doc.PageSize.Height - margeHaut - margeBas - (nbRangeesParPage - 1) * espaceVEntreCases) / nbRangeesParPage;
                float largeurRangee = doc.PageSize.Width - margeDroite - margeGauche;

                List<Espace> espaces = LireXML(@"..\..\..\..\bd1\BD1\bd.xml", hauteurCase);

                float x = 0;
                float y = 0;
                float noRangee = 0;
                for (int i = 0; i < espaces.Count; )
                {
                    int nbCasesDansLaRangee = 0;

                    // On trouve le nombre de cases qu'on peut fitter dans la rangée
                    float largeurMin = 0;
                    float largeurMax = 0;
                    for ( ; i + nbCasesDansLaRangee < espaces.Count && largeurMin < largeurRangee; ++nbCasesDansLaRangee)
                    {
                        Espace espace = espaces[i + nbCasesDansLaRangee];
                        float largeurMinCase = espace.getLargeur() * (1 - ((espace.paddingMaxGauchePct + espace.paddingMaxDroitePct) / 100));
                        float largeurMaxCase = espace.getLargeur();
                        if (largeurMin + largeurMinCase + (nbCasesDansLaRangee >= 1 ? espaceHEntreCases : 0) > largeurRangee)
                            break;
                        largeurMin += largeurMinCase + (nbCasesDansLaRangee >= 1 ? espaceHEntreCases : 0);
                        largeurMax += largeurMaxCase + (nbCasesDansLaRangee >= 1 ? espaceHEntreCases : 0);
                    }

                    // =============================

                    float decoupage = 0;
                    float decoupageTotal = largeurRangee - largeurMin;
                    float decoupageAlloueParCase = decoupageTotal / nbCasesDansLaRangee;

                    List<Espace> espacesSurLaRangee = new List<Espace>();
                    for (int j = 0; j < nbCasesDansLaRangee; ++j)
                        espacesSurLaRangee.Add(espaces[i + j]);

                    // On essaie d'égaliser les côtés de chaque bord des images
                    foreach (Espace espace in espacesSurLaRangee)
                    {
                        float decoupageGauche, decoupageDroite;
                        float decoupagePossibleGauche = espace.getLargeur() * espace.paddingMaxGauchePct / 100;
                        float decoupagePossibleDroite = espace.getLargeur() * espace.paddingMaxDroitePct / 100;

                        if (decoupagePossibleGauche + decoupagePossibleDroite <= decoupageAlloueParCase)
                        {
                            float decoupageEquilibre = Math.Min(decoupagePossibleGauche, decoupagePossibleDroite);
                            decoupageGauche = Math.Min(decoupageEquilibre, decoupageAlloueParCase / 2);
                            decoupageDroite = Math.Min(decoupageEquilibre, decoupageAlloueParCase / 2);
                            espace.paddingGauche = decoupageGauche;
                            espace.paddingDroite = decoupageDroite;

                            decoupage += decoupageGauche;
                            decoupage += decoupageDroite;
                        }
                    }

                    // On ajoute le padding qu'il faut pour remplir la rangée le plus équitablement possible
                    List<Espace> espacesSurLaRangeeTries = espacesSurLaRangee.OrderBy(e => e.paddingMaxGauchePct + e.paddingMaxDroitePct).ToList();
                    while (decoupage < Math.Min(decoupageTotal, espacesSurLaRangee.Sum(e => (e.paddingMaxGauchePct + e.paddingMaxDroitePct) * e.getLargeur() / 100)))
                    {
                        foreach (Espace espace in espacesSurLaRangeeTries)
                        {
                            if (decoupage < decoupageTotal && espace.paddingGauche < (espace.paddingMaxGauchePct * espace.getLargeur() / 100))
                            {
                                espace.paddingGauche++;
                                decoupage++;
                            }

                            if (decoupage < decoupageTotal && espace.paddingDroite < (espace.paddingMaxDroitePct * espace.getLargeur() / 100))
                            {
                                espace.paddingDroite++;
                                decoupage++;
                            }
                        }
                    }

                    // On procède au découpage et positionnement de l'image
                    foreach (Espace espace in espacesSurLaRangee)
                    {
                        espace.Decouper(procEcriture);

                        espace.Positionner(doc, x, y, margeHaut, margeGauche);

                        espace.AjouterBordures();

                        List<Element> els = espace.getElements();
                        foreach (Element element in els)
                        {
                            doc.Add(element.getImage());

                            PdfContentByte cb = procEcriture.DirectContent;
                            cb.RoundRectangle(200f, 500f, 200f, 200f, 5f);
                            cb.SetColorFill(BaseColor.WHITE);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.SetLineWidth(2f);
                            cb.FillStroke();

                            Paragraph paragraph = new Paragraph(
                                    "This could be a very long sentence that needs to be wrapped.",
                                    FontFactory.GetFont("dax-black")
                                );
                            ColumnText ct = new ColumnText(procEcriture.DirectContent);
                            cb.SetColorFill(BaseColor.BLACK);
                            ct.SetSimpleColumn(new Rectangle(206f, 700f, 400f, 600f));
                            ct.AddElement(paragraph);
                            ct.Go();
                            Console.WriteLine(ct.LastX);
                            Console.WriteLine(ct.YLine);
                        }
                            

                        x += espace.getLargeur() + espaceHEntreCases;
                    }
                    x = 0;
                    i += nbCasesDansLaRangee;
                    ++noRangee;
                    
                    if (noRangee % nbRangeesParPage == 0)
                    {
                        doc.NewPage();
                        y = 0;
                    }
                    else if (i < espaces.Count)
                    {
                        y += espaces[i].getHauteur() + espaceVEntreCases;
                    }
                }
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

        private List<Espace> LireXML(string fichierXML, float hauteurCase)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(fichierXML);
            // Console.WriteLine(xml.ChildNodes[1].ChildNodes.Count);

            List<Espace> espaces = new List<Espace>();
            foreach (XmlNode espaceXML in xml.DocumentElement.ChildNodes)
            {
                List<Case> cases = new List<Case>();
                foreach (XmlNode caseXML in espaceXML.ChildNodes)
                {
                    List<Element> elements = new List<Element>();
                    foreach (XmlNode elementXML in caseXML.ChildNodes)
                    {
                        Element element = null;
                        if (elementXML.Name == "image")
                            element = new Image(elementXML);
                        else if (elementXML.Name == "texte")
                            element = new Texte(elementXML);

                        element.Redimensionner(hauteurCase);
                        elements.Add(element);
                    }
                    cases.Add(new Case(elements));
                }
                Espace espace = new Espace(cases);
                espace.paddingMaxGauchePct = espaceXML.Attributes["decoupageMaxGauche"] != null ? float.Parse(espaceXML.Attributes["decoupageMaxGauche"].InnerText) : 0f;
                espace.paddingMaxDroitePct = espaceXML.Attributes["decoupageMaxDroite"] != null ? float.Parse(espaceXML.Attributes["decoupageMaxDroite"].InnerText) : 0f;
                espaces.Add(espace);
            }
            return espaces;
        }
    }
}
