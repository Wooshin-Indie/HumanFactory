using UnityEditorInternal;
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
        public static readonly int LAYER_INTERACTABLE = 1 << 11;

        public static Color COLOR_TRANS = new Color(1f, 1f, 1f, 0f);
        public static Color COLOR_ARROW = new Color(1f, 1f, 1f, 1f);
        public static Color COLOR_WHITE = new Color(1f, 1f, 1f, 1f);
        public static Color COLOR_INVISIBLE = new Color(1f, 1f, 1f, 0.4f);
        public static Color COLOR_REDINVISIBLE = new Color(1f, 0.1f, 0.1f, 0.4f);
        public static Color COLOR_WARNING = new Color(1f, .1f, .1f, 1f);


		public static Color COLOR_LOCKSTAGE = new Color(1f, .1f, .1f, 1f);
        public static Color COLOR_UNLOCKSTAGE = new Color(1f, 1f, 1f, 1f);
        public static Color COLOR_CLEARSTAGE = new Color(0f, 1f, 0f, 1f);

        public static readonly string PATH_SFX = "Sounds/SFX/";

        public static readonly KeyCode[] KEYCODE_SHORTCUT_DEFAULT = {
			KeyCode.Q,
			KeyCode.W,
			KeyCode.E,
			KeyCode.R,
			KeyCode.A,
			KeyCode.S,
			KeyCode.D,
			KeyCode.F,
			KeyCode.Escape,
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Z
		};

        public static readonly int[] AREA_NUMBER =
        {
            0,
            79,
            81,
            27,
        };

        public static readonly string TABLE_STAGE = "StageDescription";
        public static readonly string TABLE_SETTINGUI = "SettingUI";
        public static readonly string TABLE_MENUUI = "MenuUI";
        public static readonly string TABLE_GAMEUI = "GameUI";
        public static readonly string TABLE_SUCCESSPOPUPUI = "SuccessPopupUI";
        public static readonly string TABLE_LOG = "Log";
        public static readonly string TABLE_INOUT = "InOut";

        public static readonly string ANIM_PARAM_IDLE = "IsIdle";
        public static readonly string ANIM_PARAM_WALK = "IsWalk";
        public static readonly string ANIM_PARAM_FIRE = "IsFire";
        public static readonly string ANIM_PARAM_DIE = "IsDead";
        public static readonly string ANIM_PARAM_RUN = "IsRun";
        public static readonly string ANIM_PARAM_JUMP = "IsJump";

        public static readonly Color COLOR_RED = new Color(.7f, 0f, 0f);
        public static readonly Color COLOR_GREEN = new Color(.16f, .5f, 0f);

        /** TCP Server Port **/
        public static readonly int PORT_VM_TCP = 61392;
        public static readonly int PORT_LAP_TCP = 61393;
        // TODO - Ip Addr Crypto needed
        public static readonly string IP_ADDR = "122.35.235.70";


		public static string DB_CONN_STR = "Server=localhost; Database=human_results; User ID=root; Password=;";
        public static int COUNT_GRAPH_MAX = 25;
	}
}