using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DotsClone {
    public class DotsTheme : MonoBehaviour
    {
        public Color[] CurrentTheme;

        public Color GetColorForDotType(DotType dotType)
        {
            switch (dotType)
            {
                case DotType.DotTypeA:
                    return CurrentTheme[0];
                case DotType.DotTypeB:
                    return CurrentTheme[1];
                case DotType.DotTypeC:
                    return CurrentTheme[2];
                case DotType.DotTypeD:
                    return CurrentTheme[3];
                case DotType.DotTypeE:
                    return CurrentTheme[4];
                case DotType.DotTypeF:
                    return CurrentTheme[5];
                default:
                    return Color.black;
            }
        }
    }
}