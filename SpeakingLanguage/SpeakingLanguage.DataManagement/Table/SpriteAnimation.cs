using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SpeakingLanguage.DataManagement.Table
{
    [Serializable]
    public class SpriteAnimation
    {
        [XmlAttribute]
        public string Code;
        public Size Size;

        [XmlArray("AnimationDatas")]
        public List<AnimationData> AnimationDatas;
    }

    [Serializable]
    public class Size
    {
        [XmlAttribute]
        public float Width;
        [XmlAttribute]
        public float Height;
    }

    [Serializable]
    public class AnimationData
    {
        [XmlAttribute]
        public string Animation;
        [XmlAttribute]
        public float FrameRate;

        [XmlArray("SpriteFrames")]
        public List<SpriteFrame> SpriteFrames;
        
    }

    [Serializable]
    public class SpriteFrame
    {
        [XmlAttribute]
        public int TextureIndex;
        public Uvs Uvs;
    }

    [Serializable]
    public class Uvs
    {
        [XmlAttribute]
        public float X;
        [XmlAttribute]
        public float Y;
        [XmlAttribute]
        public float Width;
        [XmlAttribute]
        public float Height;
    }
}
