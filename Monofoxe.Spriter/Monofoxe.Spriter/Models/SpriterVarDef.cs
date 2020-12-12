using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterVarDef : SpriterElement
	{
		[XmlAttribute("type")]
		public SpriterVarType Type;

		[XmlAttribute("default")]
		public string DefaultValue;

		[XmlIgnore]
		public SpriterVarValue VariableValue;
	}
}
