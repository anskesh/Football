using Mirror;

namespace Football
{
    public enum EColor
    {
        Default,
        Red,
        Green,
        Blue,
        Yellow,
    }
    
    public struct PlayerSettings : NetworkMessage
    {
        public EColor Color;
    }
}