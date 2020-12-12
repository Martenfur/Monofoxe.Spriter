using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public enum SpriterCurveType
	{
		[XmlEnum("linear")]
		Linear,

		[XmlEnum("instant")]
		Instant,

		[XmlEnum("quadratic")]
		Quadratic,

		[XmlEnum("cubic")]
		Cubic,

		[XmlEnum("quartic")]
		Quartic,

		[XmlEnum("quintic")]
		Quintic,

		[XmlEnum("bezier")]
		Bezier
	}
}
