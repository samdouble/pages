using iText.Layout;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Panels
{
    class NewPage : IRenderable
    {
        private Comic parent;
        private List<Panel> panels = new List<Panel>();
        public float paddingMaxGauchePct { get; set; }
        public float paddingMaxDroitePct { get; set; }
        public float paddingGauche { get; set; }
        public float paddingDroite { get; set; }
        public float height { get; set; }

        public NewPage(Comic parent, XmlNode xmlSlot)
        {
            this.parent = parent;
            List<XmlNode> xmlPanels = new List<XmlNode>(xmlSlot.ChildNodes.Cast<XmlNode>());
            this.panels.AddRange(xmlPanels.Select(xmlPanel => new Panel(parent, xmlPanel)));
            this.paddingMaxGauchePct = xmlSlot.Attributes["maxCropLeft"] != null ? float.Parse(xmlSlot.Attributes["maxCropLeft"].InnerText) : 0f;
            this.paddingMaxDroitePct = xmlSlot.Attributes["maxCropRight"] != null ? float.Parse(xmlSlot.Attributes["maxCropRight"].InnerText) : 0f;
            this.paddingGauche = 0f;
            this.paddingDroite = 0f;
        }

        // IPositionable
        public void SetPosition(Document doc, int noPage, float x, float y)
        {
            int nbPanelsInSlot = this.panels.Count;
            float panelHeight =
                (this.height - (nbPanelsInSlot - 1) * parent.getVerticalPanelSpacing()) / nbPanelsInSlot;
            for (int i = 0; i < nbPanelsInSlot; i++)
            {
                Panel panel = this.panels[i];
                panel.Crop(doc, 0, 0, 0, 0);
                panel.SetPosition(noPage, x, y - i * panelHeight - (i - 1) * parent.getVerticalPanelSpacing());
            }
        }

        // IRenderable
        public void Render(Document doc)
        {
            this.panels.ForEach(panel => panel.Render(doc));
        }
    }
}
