using System;
using System.Collections.Generic;

namespace Lucky.Framework
{
    public class BitTag
    {
        public static int TotalTags;
        private static BitTag[] byID;
        private static Dictionary<string, BitTag> byName;
        private int ID;
        private int Value;

        static BitTag()
        {
            TotalTags = 0;
            byID = new BitTag[32];
            byName = new(StringComparer.OrdinalIgnoreCase);
        }


        private static BitTag Get(string name) => byName[name];

        public BitTag(string name)
        {
            ID = TotalTags;
            Value = 1 << TotalTags;
            byID[ID] = this;
            byName[name] = this;
            TotalTags++;
        }

        public static implicit operator int(BitTag tag)
        {
            return tag.Value;
        }
    }
}