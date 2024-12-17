
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
        Button,
    }

    public enum CameraType
    {
        None = -1,
        Main = 0,
        Menu = 1,
        Game = 2,
        Setting = 3,
    }

    public enum ButtonType
    {
        NewInput    = 0,
        Rotate      = 1,
        Stop        = 2,
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

    }

    public enum InputMode   // InputMode in GameScene
    {
        None = 0,
        Pad,
        Building,
        Running
    }

}