// namespace Lucky.Framework.Animation
// {
//     public class SpriteData
//     {
//         public SpriteData(Atlas atlas)
// 		{
// 			this.Sprite = new Sprite(atlas, "");
// 			this.Atlas = atlas;
// 		}
//
// 		public void Add(XmlElement xml, string overridePath = null)
// 		{
// 			SpriteDataSource spriteDataSource = new SpriteDataSource();
// 			spriteDataSource.XML = xml;
// 			spriteDataSource.Path = spriteDataSource.XML.Attr("path");
// 			spriteDataSource.OverridePath = overridePath;
// 			string text = "Sprite '" + spriteDataSource.XML.Name + "': ";
// 			if (!spriteDataSource.XML.HasAttr("path") && string.IsNullOrEmpty(overridePath))
// 			{
// 				throw new Exception(text + "'path' is missing!");
// 			}
// 			HashSet<string> hashSet = new HashSet<string>();
// 			foreach (object obj in spriteDataSource.XML.GetElementsByTagName("Anim"))
// 			{
// 				XmlElement xmlElement = (XmlElement)obj;
// 				this.CheckAnimXML(xmlElement, text, hashSet);
// 			}
// 			foreach (object obj2 in spriteDataSource.XML.GetElementsByTagName("Loop"))
// 			{
// 				XmlElement xmlElement2 = (XmlElement)obj2;
// 				this.CheckAnimXML(xmlElement2, text, hashSet);
// 			}
// 			if (spriteDataSource.XML.HasAttr("start") && !hashSet.Contains(spriteDataSource.XML.Attr("start")))
// 			{
// 				throw new Exception(text + "starting animation '" + spriteDataSource.XML.Attr("start") + "' is missing!");
// 			}
// 			if (spriteDataSource.XML.HasChild("Justify") && spriteDataSource.XML.HasChild("Origin"))
// 			{
// 				throw new Exception(text + "has both Origin and Justify tags!");
// 			}
// 			string text2 = spriteDataSource.XML.Attr("path", "");
// 			float num = spriteDataSource.XML.AttrFloat("delay", 0f);
// 			foreach (object obj3 in spriteDataSource.XML.GetElementsByTagName("Anim"))
// 			{
// 				XmlElement xmlElement3 = (XmlElement)obj3;
// 				Chooser<string> chooser;
// 				if (xmlElement3.HasAttr("goto"))
// 				{
// 					chooser = Chooser<string>.FromString<string>(xmlElement3.Attr("goto"));
// 				}
// 				else
// 				{
// 					chooser = null;
// 				}
// 				string text3 = xmlElement3.Attr("id");
// 				string text4 = xmlElement3.Attr("path", "");
// 				int[] array = Calc.ReadCSVIntWithTricks(xmlElement3.Attr("frames", ""));
// 				if (!string.IsNullOrEmpty(overridePath) && this.HasFrames(this.Atlas, overridePath + text4, array))
// 				{
// 					text4 = overridePath + text4;
// 				}
// 				else
// 				{
// 					text4 = text2 + text4;
// 				}
// 				this.Sprite.Add(text3, text4, xmlElement3.AttrFloat("delay", num), chooser, array);
// 			}
// 			foreach (object obj4 in spriteDataSource.XML.GetElementsByTagName("Loop"))
// 			{
// 				XmlElement xmlElement4 = (XmlElement)obj4;
// 				string text5 = xmlElement4.Attr("id");
// 				string text6 = xmlElement4.Attr("path", "");
// 				int[] array2 = Calc.ReadCSVIntWithTricks(xmlElement4.Attr("frames", ""));
// 				if (!string.IsNullOrEmpty(overridePath) && this.HasFrames(this.Atlas, overridePath + text6, array2))
// 				{
// 					text6 = overridePath + text6;
// 				}
// 				else
// 				{
// 					text6 = text2 + text6;
// 				}
// 				this.Sprite.AddLoop(text5, text6, xmlElement4.AttrFloat("delay", num), array2);
// 			}
// 			if (spriteDataSource.XML.HasChild("Center"))
// 			{
// 				this.Sprite.CenterOrigin();
// 				this.Sprite.Justify = new Vector2?(new Vector2(0.5f, 0.5f));
// 			}
// 			else if (spriteDataSource.XML.HasChild("Justify"))
// 			{
// 				this.Sprite.JustifyOrigin(spriteDataSource.XML.ChildPosition("Justify"));
// 				this.Sprite.Justify = new Vector2?(spriteDataSource.XML.ChildPosition("Justify"));
// 			}
// 			else if (spriteDataSource.XML.HasChild("Origin"))
// 			{
// 				this.Sprite.Origin = spriteDataSource.XML.ChildPosition("Origin");
// 			}
// 			if (spriteDataSource.XML.HasChild("Position"))
// 			{
// 				this.Sprite.Position = spriteDataSource.XML.ChildPosition("Position");
// 			}
// 			if (spriteDataSource.XML.HasAttr("start"))
// 			{
// 				this.Sprite.Play(spriteDataSource.XML.Attr("start"), false, false);
// 			}
// 			this.Sources.Add(spriteDataSource);
// 		}
//
// 		private bool HasFrames(Atlas atlas, string path, int[] frames = null)
// 		{
// 			if (frames == null || frames.Length == 0)
// 			{
// 				return atlas.GetAtlasSubtexturesAt(path, 0) != null;
// 			}
// 			for (int i = 0; i < frames.Length; i++)
// 			{
// 				if (atlas.GetAtlasSubtexturesAt(path, frames[i]) == null)
// 				{
// 					return false;
// 				}
// 			}
// 			return true;
// 		}
//
// 		private void CheckAnimXML(XmlElement xml, string prefix, HashSet<string> ids)
// 		{
// 			if (!xml.HasAttr("id"))
// 			{
// 				throw new Exception(prefix + "'id' is missing on " + xml.Name + "!");
// 			}
// 			if (ids.Contains(xml.Attr("id")))
// 			{
// 				throw new Exception(prefix + "multiple animations with id '" + xml.Attr("id") + "'!");
// 			}
// 			ids.Add(xml.Attr("id"));
// 		}
//
// 		public Sprite Create()
// 		{
// 			return this.Sprite.CreateClone();
// 		}
//
// 		public Sprite CreateOn(Sprite sprite)
// 		{
// 			return this.Sprite.CloneInto(sprite);
// 		}
//
// 		public List<SpriteDataSource> Sources = new List<SpriteDataSource>();
//
// 		public Sprite Sprite;
//
// 		public Atlas Atlas;
// 	}
//     }
// }