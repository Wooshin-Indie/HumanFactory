using UnityEngine;

namespace HumanFactory
{
    public static class Constants
    {
        public static readonly float HUMAN_POS_Z = 0;
        public static readonly float CAMERA_POS_Z = -10;

        public static readonly int[] DIR_X = { 0, 1, 0, -1, 0 };
        public static readonly int[] DIR_Y = { 1, 0, -1, 0, 0 };


        public static string TAG_NONE = "Untagged";
        public static string TAG_CAMERA = "MainCamera";

        public static readonly int LAYER_CLICKABLE = 1 << 10;

        public static Color COLOR_TRANS = new Color(0f, 1f, 0f, 0f);
        public static Color COLOR_ARROW = new Color(0f, 1f, 0f, 1f);
    }
}