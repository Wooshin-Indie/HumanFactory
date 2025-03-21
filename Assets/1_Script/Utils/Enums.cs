
namespace HumanFactory
{
    public enum GridType
    {
        Empty       = 0,
        Pad         = 1,
        Building    = 2
    }

    public enum PadType
    {
        DirUp       = 0,
        DirRight    = 1,
        DirDown     = 2,
        DirLeft     = 3,
        DirNone     = 4,
    }

    public enum BuildingType
    {
        None = -1,
        Add = 0,
        Sub,        
		Double,
		NewInput,   // 3
		Jump,
		Jump0,
        Toggle,
        Rotate      // 7
    }

    public enum CameraType
    {
        None = -1,
        Main = 0,
        Menu = 1,
        Game = 2,
        Setting = 3,
    }

    public enum ButtonInputType
    {
        New = 0,
    }

    public enum ButtonToggleType
    {
        Off = 0,
        On = 1,
    }

    public enum HumanOperandType
    {
        None = 0,
        Operand1 = 1,
        Operand2 = 2,
    }

    public enum SoundType 
    { 
        Bgm = 0,
        Sfx = 1,
        Noise = 2,
    }

    public enum BGMType
    {
        BGM_1 = 0,
        BGM_2,
        BGM_3,
        BGM_4,
        BGM_5,
        BGM_6,
        BGM_7,
        BGM_8,
        None
    }

    public enum SFXType
    {
        UI_Hover,
        UI_Paper,
        Click,
        Noise,
        Typing,
        Back,
        Shot,
        Stamp,
        Writing,
        Typewriting,
        Checkbox,
        ButtonPress,
        Beep,
        UI_Click,
        Wind,
        Zoom,
        Button_Put,
        Button_Remove,
        LinkSuccess,
        Fanfare
    }

    public enum InputMode   // InputMode in GameScene
    {
        None = 0,
        Pad,
        Building
    }

    public enum EffectType
    {
        Add = 0,
        Addi,
        Subi,

    }

    public enum ExecuteType
    {
        None = 0, // Stop누를 때 None으로 감
        Play = 1,
        Pause = 2
    }

	public enum ShortcutActionEnum
    {
        None= -1,
        Add_Button = 0,
        Sub_Button,
		Double_Button,
		Input_Button,
		Jump_Button,
		JumpZero_Button,
        Toggle_Button,
        Rotate_Button,
		Back = 8,
        ChangeMode_1,
        ChangeMode_2,
        Zoom_Map
	}
}