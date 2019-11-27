using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Pages
{
    class Slot
    {
        private List<Panel> panels = new List<Panel>();
        public float paddingMaxGauchePct { get; set; }
        public float paddingMaxDroitePct { get; set; }
        public float paddingGauche { get; set; }
        public float paddingDroite { get; set; }

        public Slot(XmlNode xmlSlot)
        {
            foreach (XmlNode xmlPanel in xmlSlot.ChildNodes)
            {
                Panel panel = new Panel(xmlPanel);
                this.panels.Add(panel);
            }
            this.paddingMaxGauchePct = xmlSlot.Attributes["decoupageMaxGauche"] != null ? float.Parse(xmlSlot.Attributes["decoupageMaxGauche"].InnerText) : 0f;
            this.paddingMaxDroitePct = xmlSlot.Attributes["decoupageMaxDroite"] != null ? float.Parse(xmlSlot.Attributes["decoupageMaxDroite"].InnerText) : 0f;
            this.paddingGauche = 0f;
            this.paddingDroite = 0f;
        }

        public void SetHeight(float height)
        {
            this.panels.ForEach(panel => panel.SetHeight(height));
        }

        // On prend cases[0] puisque toutes les cases d'un même espace
        // doivent avoir la même largeur
        public float getLargeur()
        {
            return panels[0].getLargeur();
        }
        public float getHauteur()
        {
            return panels[0].getHauteur();
        }

        public void Decouper(PdfWriter procEcriture)
        {
            float decoupageGauche = (this.paddingMaxGauchePct * this.getLargeur() / 100) - this.paddingGauche;
            float decoupageDroite = (this.paddingMaxDroitePct * this.getLargeur() / 100) - this.paddingDroite;
            float offset = decoupageGauche + decoupageDroite;
            foreach (Panel casex in panels)
                casex.Decouper(procEcriture, decoupageGauche, offset);
        }

        public void Positionner(Document doc, float x, float y, float margeHaut, float margeGauche)
        {
            foreach (Panel casex in panels)
                casex.Positionner(doc, x, y, margeHaut, margeGauche);
        }

        public void AjouterBordures()
        {
            foreach (Panel casex in panels)
                casex.AjouterBordures();
        }

        public List<Element> getElements()
        {
            List<Element> elements = new List<Element>();
            foreach (Panel casex in panels)
                elements.AddRange(casex.getElements());
            return elements;
        }
    }
}
