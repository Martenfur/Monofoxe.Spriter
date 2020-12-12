using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterObject : SpriterSpatial
	{
		[XmlAttribute("animation")]
		public int AnimationId;

		[XmlAttribute("entity")]
		public int EntityId;

		[XmlAttribute("folder")]
		public int FolderId;

		[XmlAttribute("file")]
		public int FileId;

		[XmlAttribute("pivot_x")]
		public float PivotX;

		[XmlAttribute("pivot_y")]
		public float PivotY;

		[XmlAttribute("t")]
		public float T;

		public SpriterObject()
		{
			PivotX = float.NaN;
			PivotY = float.NaN;
		}
	}

}
