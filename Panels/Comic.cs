using iText.Kernel.Geom;
using iText.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Panels
{
    class Comic : IRenderable
    {
        private List<object> children = new List<object>();
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

            List<XmlNode> xmlNodes = new List<XmlNode>(xmlComic.ChildNodes.Cast<XmlNode>());
            foreach (XmlNode xmlNode in xmlNodes) {
                if (xmlNode.Name == "newpage") {
                    this.children.Add(new NewPage(this, xmlNode));
                }
                if (xmlNode.Name == "slot") {
                    this.children.Add(new Slot(this, xmlNode));
                }
            }
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
        public void Render(Document doc)
        {
            PageSize pageSize = doc.GetPdfDocument().GetDefaultPageSize();
            float hauteurCase = (pageSize.GetHeight() - this.topMargin - this.bottomMargin - (this.rowsPerPage - 1) * this.verticalPanelSpacing) / this.rowsPerPage;
            float rowWidth = pageSize.GetWidth() - this.rightMargin - this.leftMargin;
            int page = 1;
            float x = 0;
            float y = hauteurCase + this.verticalPanelSpacing;
            float noRangee = 1;
            for (int i = 0; i < this.children.Count;)
            {
                // Handle newpage elements
                if (this.children[i].GetType() == typeof(NewPage)) {
                    if (x != 0 || y != 0) {
                        doc.GetPdfDocument().AddNewPage();
                        page++;
                        x = 0;
                        y = 0;
                    }
                    i++;
                    continue;
                }

                int nbPanelsInRow = 0;
                // On trouve le nombre de cases qu'on peut fitter dans la rangée
                float minWidth = 0;
                float maxWidth = 0;
                for (; i + nbPanelsInRow < this.children.Count && minWidth < rowWidth; ++nbPanelsInRow)
                {
                    if (this.children[i + nbPanelsInRow].GetType() != typeof(Slot))
                        break;
                    Slot slot = (Slot) this.children[i + nbPanelsInRow];
                    slot.SetHeight(hauteurCase);
                    float largeurMinCase = slot.GetMinWidth();
                    float largeurMaxCase = slot.GetMaxWidth();
                    if (minWidth + largeurMinCase + (nbPanelsInRow >= 1 ? this.horizontalPanelSpacing : 0) > rowWidth)
                        break;
                    minWidth += largeurMinCase + (nbPanelsInRow >= 1 ? this.horizontalPanelSpacing : 0);
                    maxWidth += largeurMaxCase + (nbPanelsInRow >= 1 ? this.horizontalPanelSpacing : 0);
                }

                // =============================

                float decoupage = 0;
                float decoupageTotal = rowWidth - minWidth;
                float decoupageAlloueParCase = decoupageTotal / nbPanelsInRow;

                List<Slot> espacesSurLaRangee = new List<Slot>();
                for (int j = 0; j < nbPanelsInRow; ++j)
                    espacesSurLaRangee.Add((Slot) this.children[i + j]);

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
                    espace.Crop(doc);
                    espace.SetPosition(doc, page, this.leftMargin + x, pageSize.GetHeight() - this.topMargin - y);
                    espace.Render(doc);

                    x += espace.GetWidth() + this.horizontalPanelSpacing;
                }
                x = 0;
                i += nbPanelsInRow;
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