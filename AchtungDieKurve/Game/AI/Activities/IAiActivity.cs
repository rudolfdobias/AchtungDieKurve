using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AchtungDieKurve.Game.AI.Activities
{
    public interface IAiActivity
    {
        AiActivity Type { get; }
        Vector2 Propose(ArcMatrix matrix);
        Vector2 DrawPropose(ArcMatrix matrix, SpriteBatch sb);
    }
}
