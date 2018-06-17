using System.Xml;

namespace Pages
{
    class Texte : Element
    {
        private float scale;

        public Texte(XmlNode element) : base(element)
        {
            this.scale = float.Parse(element.Attributes["scale"]?.InnerText);
        }

        public override void Redimensionner(float hauteurCase)
        {
            this.image.ScalePercent(this.scale);
        }
    }
}
