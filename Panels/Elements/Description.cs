﻿using iText.Kernel.Colors;
using iText.Layout;
using Panels.Elements;
using System.Xml;

namespace Panels
{
    class Description : Text
    {
        protected bool visible = true;

        public Description(XmlNode element, Panel parent) : base(element, parent)
        {
            this.color = ColorConstants.RED;
            this.visible = element.Attributes["visible"] != null
                ? bool.Parse(element.Attributes["visible"].InnerText)
                : true;
        }

        public override void Render(Document doc)
        {
            if (this.visible)
            {
                base.Render(doc);
            }
        }
    }
}
