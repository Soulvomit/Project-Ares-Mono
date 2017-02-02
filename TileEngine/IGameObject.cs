using Microsoft.Xna.Framework;

namespace TileEngine
{
    public interface IGameObject
    {
        string Name { get; }
        Vector2 Position { get; set;  }
        float Angle { set; }
        float Scale { set; }
    }
}
