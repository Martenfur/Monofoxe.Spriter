using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterVarlineKey : SpriterKey
	{
		[XmlAttribute("val")]
		public string Value;

		[XmlIgnore]
		public SpriterVarValue VariableValue;
	}
}
