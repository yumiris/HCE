using System.Xml.Serialization;

namespace Promise.Library.OpenSauce.Rasterizer.ShaderExtensions
{
    public class ShaderExtensions
    {
        [XmlElement(ElementName = "Enabled")]
        public bool IsEnabled { get; set; } = true;

        [XmlElement(ElementName = "Object")]
        public ShaderObject ShaderObject { get; set; } = new ShaderObject();

        public Environment Environment { get; set; } = new Environment();
        public Effect Effect { get; set; } = new Effect();
    }
}