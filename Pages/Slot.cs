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
    class Slot : IPositionable, IRenderable
    {
        private List<Panel> panels = new List<Panel>();
        public float paddingMaxGauchePct { get; set; }
        public float paddingMaxDroitePct { get; set; }
        public float paddingGauche { get; set; }
        public float paddingDroite { get; set; }

        public Slot(XmlNode xmlSlot)
        {
            List<XmlNode> xmlPanels = new List<XmlNode>(xmlSlot.ChildNodes.Cast<XmlNode>());
            this.panels.AddRange(xmlPanels.Select(xmlPanel => new Panel(xmlPanel)));
            this.paddingMaxGauchePct = xmlSlot.Attributes["maxCropLeft"] != null ? float.Parse(xmlSlot.Attributes["maxCropLeft"].InnerText) : 0f;
            this.paddingMaxDroitePct = xmlSlot.Attributes["maxCropRight"] != null ? float.Parse(xmlSlot.Attributes["maxCropRight"].InnerText) : 0f;
            this.paddingGauche = 0f;
            this.paddingDroite = 0f;
        }

        public void SetHeight(float height)
        {
            this.panels.ForEach(panel => panel.SetHeight(height));
        }

        public float GetWidth()
        {
            return panels[0].getLargeur();
        }

        public float GetMinWidth()
        {
            float minPctAvailable = (1 - ((this.paddingMaxGauchePct + this.paddingMaxDroitePct) / 100));
            return minPctAvailable * this.GetWidth();
        }

        public float GetMaxWidth()
        {
            return panels[0].getLargeur();
        }

        public float getHauteur()
        {
            return panels[0].getHauteur();
        }

        public void Decouper(PdfWriter procEcriture)
        {
            float decoupageGauche = (this.paddingMaxGauchePct * this.GetWidth() / 100) - this.paddingGauche;
            float decoupageDroite = (this.paddingMaxDroitePct * this.GetWidth() / 100) - this.paddingDroite;
            float offset = decoupageGauche + decoupageDroite;
            foreach (Panel casex in panels)
                casex.Decouper(procEcriture, decoupageGauche, offset);
        }

        // IPositionable
        public void SetPosition(float x, float y)
        {
            this.panels.ForEach(panel => panel.SetPosition(x, y));
        }

        // IRenderable
        public void Render(Document doc, PdfWriter writer)
        {
            this.panels.ForEach(panel => panel.Render(doc, writer));
        }
    }
}
