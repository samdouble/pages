using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Pages
{
    class Comic : IRenderable
    {
        private List<Slot> slots = new List<Slot>();
        private float leftMargin;
        private float rightMargin;
        private float topMargin;
        private float bottomMargin;
        private float horizontalPanelSpacing;
        private float verticalPanelSpacing;
        private float rowsPerPage;
        private string imagesFolderPath;

        public Comic(string configFile, string imagesFolderPath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Console.WriteLine("Reading config file at " + @"" + configFile);
            this.imagesFolderPath = imagesFolderPath;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(@"" + configFile);
            XmlNode xmlComic = xmlDocument.DocumentElement;
            this.leftMargin = float.Parse(xmlComic.Attributes["leftMargin"].InnerText);
            this.rightMargin = float.Parse(xmlComic.Attributes["rightMargin"].InnerText);
            this.topMargin = float.Parse(xmlComic.Attributes["topMargin"].InnerText);
            this.bottomMargin = float.Parse(xmlComic.Attributes["bottomMargin"].InnerText);
            this.horizontalPanelSpacing = float.Parse(xmlComic.Attributes["horizontalPanelSpacing"].InnerText);
            this.verticalPanelSpacing = float.Parse(xmlComic.Attributes["verticalPanelSpacing"].InnerText);
            this.rowsPerPage = float.Parse(xmlComic.Attributes["rowsPerPage"].InnerText);

            List<XmlNode> xmlSlots = new List<XmlNode>(xmlComic.ChildNodes.Cast<XmlNode>());
            this.slots.AddRange(xmlSlots.Select(xmlSlot => new Slot(this, xmlSlot)));
        }

        public string GetImagesFolderPath()
        {
            return this.imagesFolderPath;
        }

        public float getVerticalPanelSpacing()
        {
            return this.verticalPanelSpacing;
        }

        // IRenderable
        public void Render(Document doc, PdfWriter writer)
        {
            PageSize pageSize = doc.GetPdfDocument().GetDefaultPageSize();
            float hauteurCase = (pageSize.GetHeight() - this.topMargin - this.bottomMargin - (this.rowsPerPage - 1) * this.verticalPanelSpacing) / this.rowsPerPage;
            float largeurRangee = pageSize.GetWidth() - this.rightMargin - this.leftMargin;
            int page = 1;
            float x = 0;
            float y = hauteurCase + this.verticalPanelSpacing;
            float noRangee = 1;
            for (int i = 0; i < this.slots.Count;)
            {
                int nbCasesDansLaRangee = 0;

                // On trouve le nombre de cases qu'on peut fitter dans la rangée
                float largeurMin = 0;
                float largeurMax = 0;
                for (; i + nbCasesDansLaRangee < this.slots.Count && largeurMin < largeurRangee; ++nbCasesDansLaRangee)
                {
                    Slot slot = this.slots[i + nbCasesDansLaRangee];
                    slot.SetHeight(hauteurCase);
                    float largeurMinCase = slot.GetMinWidth();
                    float largeurMaxCase = slot.GetMaxWidth();
                    if (largeurMin + largeurMinCase + (nbCasesDansLaRangee >= 1 ? this.horizontalPanelSpacing : 0) > largeurRangee)
                        break;
                    largeurMin += largeurMinCase + (nbCasesDansLaRangee >= 1 ? this.horizontalPanelSpacing : 0);
                    largeurMax += largeurMaxCase + (nbCasesDansLaRangee >= 1 ? this.horizontalPanelSpacing : 0);
                }

                // =============================

                float decoupage = 0;
                float decoupageTotal = largeurRangee - largeurMin;
                float decoupageAlloueParCase = decoupageTotal / nbCasesDansLaRangee;

                List<Slot> espacesSurLaRangee = new List<Slot>();
                for (int j = 0; j < nbCasesDansLaRangee; ++j)
                    espacesSurLaRangee.Add(this.slots[i + j]);

                // On essaie d'égaliser les côtés de chaque bord des images
                foreach (Slot espace in espacesSurLaRangee)
                {
                    float decoupageGauche, decoupageDroite;
                    float decoupagePossibleGauche = espace.GetWidth() * espace.paddingMaxGauchePct / 100;
                    float decoupagePossibleDroite = espace.GetWidth() * espace.paddingMaxDroitePct / 100;

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
                List<Slot> espacesSurLaRangeeTries = espacesSurLaRangee.OrderBy(e => e.paddingMaxGauchePct + e.paddingMaxDroitePct).ToList();
                while (decoupage < Math.Min(decoupageTotal, espacesSurLaRangee.Sum(e => (e.paddingMaxGauchePct + e.paddingMaxDroitePct) * e.GetWidth() / 100)))
                {
                    foreach (Slot espace in espacesSurLaRangeeTries)
                    {
                        if (decoupage < decoupageTotal && espace.paddingGauche < (espace.paddingMaxGauchePct * espace.GetWidth() / 100))
                        {
                            espace.paddingGauche++;
                            decoupage++;
                        }

                        if (decoupage < decoupageTotal && espace.paddingDroite < (espace.paddingMaxDroitePct * espace.GetWidth() / 100))
                        {
                            espace.paddingDroite++;
                            decoupage++;
                        }
                    }
                }

                // On procède au découpage et positionnement de l'image
                foreach (Slot espace in espacesSurLaRangee)
                {
                    espace.Crop(writer);
                    espace.SetPosition(writer, page, this.leftMargin + x, pageSize.GetHeight() - this.topMargin - y);
                    espace.Render(doc, writer);

                    x += espace.GetWidth() + this.horizontalPanelSpacing;
                }
                x = 0;
                i += nbCasesDansLaRangee;
                ++noRangee;

                if (noRangee % this.rowsPerPage == 0)
                {
                    doc.GetPdfDocument().AddNewPage();
                    page++;
                    y = 0;
                }
                else
                {
                    y += hauteurCase + this.verticalPanelSpacing;
                }
            }
        }
    }
}