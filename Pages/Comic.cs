using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Comic(XmlNode xmlComic)
        {
            this.leftMargin = float.Parse(xmlComic.Attributes["leftMargin"].InnerText);
            this.rightMargin = float.Parse(xmlComic.Attributes["rightMargin"].InnerText);
            this.topMargin = float.Parse(xmlComic.Attributes["topMargin"].InnerText);
            this.bottomMargin = float.Parse(xmlComic.Attributes["bottomMargin"].InnerText);
            this.horizontalPanelSpacing = float.Parse(xmlComic.Attributes["horizontalPanelSpacing"].InnerText);
            this.verticalPanelSpacing = float.Parse(xmlComic.Attributes["verticalPanelSpacing"].InnerText);
            this.rowsPerPage = float.Parse(xmlComic.Attributes["rowsPerPage"].InnerText);

            List<XmlNode> xmlSlots = new List<XmlNode>(xmlComic.ChildNodes.Cast<XmlNode>());
            this.slots.AddRange(xmlSlots.Select(xmlSlot => new Slot(xmlSlot)));
        }

        public int GetSlotsCount()
        {
            return this.slots.Count;
        }

        // IRenderable
        public void Render(Document doc, PdfWriter writer)
        {
            float hauteurCase = (doc.PageSize.Height - this.topMargin - this.bottomMargin - (this.rowsPerPage - 1) * this.verticalPanelSpacing) / this.rowsPerPage;
            float largeurRangee = doc.PageSize.Width - this.rightMargin - this.leftMargin;
            float x = 0;
            float y = hauteurCase + this.verticalPanelSpacing;
            float noRangee = 1;
            for (int i = 0; i < this.GetSlotsCount();)
            {
                int nbCasesDansLaRangee = 0;

                // On trouve le nombre de cases qu'on peut fitter dans la rangée
                float largeurMin = 0;
                float largeurMax = 0;
                for (; i + nbCasesDansLaRangee < this.GetSlotsCount() && largeurMin < largeurRangee; ++nbCasesDansLaRangee)
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
                    espace.Decouper(writer);

                    espace.SetPosition(this.leftMargin + x, doc.PageSize.Height - this.topMargin - y);

                    espace.Render(doc, writer);

                    /*foreach (Element element in els)
                    {
                        PdfContentByte cb = writer.DirectContent;
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
                    }*/

                    x += espace.GetWidth() + this.horizontalPanelSpacing;
                }
                x = 0;
                i += nbCasesDansLaRangee;
                ++noRangee;

                if (noRangee % this.rowsPerPage == 0)
                {
                    doc.NewPage();
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