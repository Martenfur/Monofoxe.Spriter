using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterMainlineKey : SpriterKey
	{
		[XmlElement("bone_ref")]
		public SpriterRef[] BoneRefs;

		[XmlElement("object_ref")]
		public SpriterObjectRef[] ObjectRefs;
	}
}
