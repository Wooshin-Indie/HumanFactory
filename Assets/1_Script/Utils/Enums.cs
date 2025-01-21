
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
        Add1 = 0,
        Sub1,
        Jump,
        Double,
        Button,
        ToggleButton,
        RotateButton
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
    }

    // HACK - 임시로 설정해둔 Enum
    // 나중에 BGM 정하고 수정해야합니다.
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

}