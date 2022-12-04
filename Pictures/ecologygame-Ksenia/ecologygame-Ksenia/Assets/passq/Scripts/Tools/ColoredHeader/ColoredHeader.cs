using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColoredHeader
{
    public class ColoredHeader : MonoBehaviour
    {
        [SerializeField] private ColoredData data = new ColoredData();
        
        public Color HeaderColor { get { return data.HeaderColor; } }
        public Color TextColor { get { return data.TextColor; } }
        public Color ChildColor { get { return data.ChildColor; } }
        public Color ChildTextColor { get { return data.ChildTextColor; } }
        public string HeaderIcon { get { return data.HeaderIcon;} set { data.HeaderIcon = value; } }

        public void SetupData(ColoredData Data)
        {
            data = Data;
        }
    }

    [System.Serializable]
    public class ColoredData : ICloneable
    {
        public Color HeaderColor = Color.white;
        public Color TextColor = Color.black;

        public Color ChildColor = Color.gray;
        public Color ChildTextColor = Color.black;

        public string HeaderIcon = string.Empty;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
