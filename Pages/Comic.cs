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
            foreach (XmlNode xmlSlot in xmlComic.ChildNodes)
            {
                Slot slot = new Slot(xmlSlot);
                this.slots.Add(slot);
            }
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
