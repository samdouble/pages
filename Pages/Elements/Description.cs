using iTextSharp.text;
using iTextSharp.text.pdf;
using Pages.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Pages
{
    class Description : Text
    {
        public Description(XmlNode element, Panel parent) : base(element, parent)
        {
            this.font.SetColor(255, 0, 0);
        }
    }
}
