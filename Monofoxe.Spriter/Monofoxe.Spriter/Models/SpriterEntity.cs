using System.Collections.Generic;
using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterEntity : SpriterElement
	{
		public SpriterData Spriter;

		[XmlElement("obj_info")]
		public SpriterObjectInfo[] ObjectInfos;

		[XmlElement("character_map")]
		public SpriterCharacterMap[] CharacterMaps;

		[XmlElement("animation")]
		public SpriterAnimation[] Animations;

		[XmlArray("var_defs"), XmlArrayItem("i")]
		public SpriterVarDef[] Variables;

		private Dictionary<string, SpriterCharacterMap> _characterMaps;
		private Dictionary<string, SpriterVarDef> _variables;
		private bool _initialized = false;


		public SpriterCharacterMap GetCharacterMap(string name)
		{
			InitDictionaries();
			return _characterMaps[name];
		}

		public bool TryGetCharacterMap(string name, out SpriterCharacterMap map)
		{
			InitDictionaries();
			return _characterMaps.TryGetValue(name, out map);
		}

		public SpriterVarDef GetVariable(string name)
		{
			InitDictionaries();
			return _variables[name];
		}

		public bool TryGetVariable(string name, out SpriterVarDef variable)
		{
			InitDictionaries();
			return _variables.TryGetValue(name, out variable);
		}


		private void InitDictionaries()
		{
			if (_initialized)
			{
				return;
			}
			_initialized = true;

			_characterMaps = new Dictionary<string, SpriterCharacterMap>();
			_variables = new Dictionary<string, SpriterVarDef>();

			for (var i = 0; i < CharacterMaps?.Length; i += 1)
			{
				_characterMaps.Add(CharacterMaps[i].Name, CharacterMaps[i]);
			}
			for (var i = 0; i < Variables?.Length; i += 1)
			{
				_variables.Add(Variables[i].Name, Variables[i]);
			}
		}
	}

}
