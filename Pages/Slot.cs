using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Pages
{
    public class Slot : IRenderable
    {
        [XmlAttribute]
        public float maxCropLeft { get; set; }
        [XmlAttribute]
        public float maxCropRight { get; set; }
        [XmlElement(ElementName = "panel")]
        public List<Panel> panels { get; set; } = new List<Panel>();
        public float paddingMaxGauchePct { get; set; }
        public float paddingMaxDroitePct { get; set; }
        public float paddingGauche { get; set; } = 0f;
        public float paddingDroite { get; set; } = 0f;
        public float height { get; set; }

        public void Crop(PdfWriter procEcriture)
        {
            int nbPanelsInSlot = this.panels.Count;
            float decoupageGauche = (this.paddingMaxGauchePct * this.GetWidth() / 100) - this.paddingGauche;
            float decoupageDroite = (this.paddingMaxDroitePct * this.GetWidth() / 100) - this.paddingDroite;
            float horizontalOffset = decoupageGauche + decoupageDroite;
            for (int i = 0; i < nbPanelsInSlot; i++)
            {
                Panel panel = this.panels[i];
                panel.Crop(procEcriture, decoupageGauche, horizontalOffset);
            }
        }

        public float GetMinWidth()
        {
            float minPctAvailable = 1 - ((this.paddingMaxGauchePct + this.paddingMaxDroitePct) / 100);
            return minPctAvailable * this.GetWidth();
        }

        public float GetHeight()
        {
            return this.height;
        }

        public float GetMaxWidth()
        {
            return panels[0].GetWidth();
        }

        public float GetWidth()
        {
            return panels[0].GetWidth();
        }

        public void SetHeight(float height)
        {
            this.height = height;
            int nbPanelsInSlot = this.panels.Count;
            float panelHeight = (height - (nbPanelsInSlot - 1) * parent.GetVerticalPanelSpacing()) / nbPanelsInSlot;
            this.panels.ForEach(panel => panel.SetHeight(panelHeight));
        }

        // IPositionable
        public void SetPosition(PdfWriter procEcriture, float x, float y)
        {
            int nbPanelsInSlot = this.panels.Count;
            float panelHeight =
                (this.height - (nbPanelsInSlot - 1) * parent.GetVerticalPanelSpacing()) / nbPanelsInSlot;
            for (int i = 0; i < nbPanelsInSlot; i++)
            {
                Panel panel = this.panels[i];
                panel.Crop(procEcriture, 0, 0, 0, 0);
                panel.SetPosition(x, y - i * panelHeight - (i - 1) * parent.GetVerticalPanelSpacing());
            }
        }

        // IRenderable
        public void Render(Document doc, PdfWriter writer, IRenderable parent)
        {
            this.panels.ForEach(panel => panel.Render(doc, writer, this));
        }
    }
}
