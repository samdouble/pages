using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Pages
{
    class Comic
    {
        private List<Slot> slots = new List<Slot>();

        public Comic(XmlNode xmlComic)
        {
            List<XmlNode> xmlSlots = new List<XmlNode>(xmlComic.ChildNodes.Cast<XmlNode>());
            this.slots.AddRange(xmlSlots.Select(xmlSlot => new Slot(xmlSlot)));
        }

        public int GetSlotsCount()
        {
            return this.slots.Count;
        }

        public Slot GetNthSlot(int index)
        {
            if (index >= this.GetSlotsCount())
            {
                throw new System.ArgumentException();
            }
            return this.slots[index];
        }
    }
}
