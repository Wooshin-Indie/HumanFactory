
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
        Sfx = 0,
    }
}