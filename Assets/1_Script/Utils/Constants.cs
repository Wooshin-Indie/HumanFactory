using UnityEngine;

namespace HumanFactory
{
    public static class Constants
    {
        public static readonly float HUMAN_POS_Z = 0;
        public static readonly float CAMERA_POS_Z = -10;

        public static readonly int[] DIR_X = { 0, 1, 0, -1, 0 };
        public static readonly int[] DIR_Y = { 1, 0, -1, 0, 0 };

        public static readonly string TAG_NONE = "Untagged";
        public static readonly string TAG_CAMERA = "MainCamera";
        public static readonly string TAG_PAGES = "Pages";

        public static readonly int LAYER_CLICKABLE = 1 << 10;

        public static Color COLOR_TRANS = new Color(1f, 1f, 1f, 0f);
        public static Color COLOR_ARROW = new Color(0f, 1f, 0f, 1f);
        public static Color COLOR_WHITE = new Color(1f, 1f, 1f, 1f);
        public static Color COLOR_INVISIBLE = new Color(1f, 1f, 1f, 0.3f);

        public static readonly string PATH_SFX = "Sounds/SFX/";

        public static readonly KeyCode[] KEYCODE_SHORTCUT_DEFAULT = {
			KeyCode.Q,
			KeyCode.W,
			KeyCode.E,
			KeyCode.R,
			KeyCode.A,
			KeyCode.S,
			KeyCode.D,
			KeyCode.Escape,
            KeyCode.Alpha1,
            KeyCode.Alpha2
		};

        public static readonly string TABLE_STAGE = "StageDescription";
        public static readonly string TABLE_SETTINGUI = "SettingUI";
        public static readonly string TABLE_MENUUI = "MenuUI";
        public static readonly string TABLE_GAMEUI = "GameUI";

        public static readonly string ANIM_PARAM_IDLE = "IsIdle";
        public static readonly string ANIM_PARAM_WALK = "IsWalk";
        public static readonly string ANIM_PARAM_FIRE = "IsFire";
        public static readonly string ANIM_PARAM_DIE = "IsDead";
        public static readonly string ANIM_PARAM_RUN = "IsRun";
    }
}