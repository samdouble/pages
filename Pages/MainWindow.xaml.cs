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
                float largeurRangee = doc.PageSize.Width - margeDroite - margeGauche;

                List<Espace> espaces = LireXML(@"..\..\..\..\bd1\bd.xml", hauteurCase);

                float x = 0;
                float y = 0;
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

                    foreach (Espace espace in espacesSurLaRangee)
                    {
                        Console.WriteLine(espace.paddingGauche + " " + espace.paddingDroite);
                        espace.Decouper(procEcriture);

                        espace.Positionner(doc, x, y, margeHaut, margeGauche);

                        espace.AjouterBordures();

                        List<Element> els = espace.getElements();
                        foreach (Element element in els)
                            doc.Add(element.getImage());

                        x += espace.getLargeur() + espaceHEntreCases;

                        //doc.NewPage();
                    }
                    x = 0;
                    y += espaces[i].getHauteur() + espaceVEntreCases;
                    i += nbCasesDansLaRangee;
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
