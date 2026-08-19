using AchtungDieKurve.Game.Drawable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve.Game.AI
{
    public class AiPlayer : Player
    {
        public event ControlEvent Controlling;

        // Steering state written by AiDriver, cleared each round.
        public int SteerCommand;        // -1 left, 0 straight, 1 right
        public int DecisionCooldown;    // updates until the next decision
        public Kurve AttackTarget;
        public double AttackUntil;      // game time in ms
        public double NextAttackRoll;   // game time in ms

        // Effective tunables: the configured values with a per-round jitter.
        public float Aggressiveness;
        public float Precision;

        public AiPlayer(Properties context, Keys left, Keys right, Color colour, IContainer container)
            : base(context, left, right, colour, container, true)
        {
        }

        protected override void Control(KeyboardState keyboardState, GameTime gameTime)
        {
            Controlling?.Invoke(this, keyboardState, gameTime);
        }

        public void TurnRight()
        {
            Angle += TurnStep;
        }

        public void TurnLeft()
        {
            Angle -= TurnStep;
        }

        public override void Reset()
        {
            base.Reset();
            SteerCommand = 0;
            DecisionCooldown = 0;
            AttackTarget = null;
            AttackUntil = 0;
            NextAttackRoll = 0;
            Aggressiveness = Jitter(GameBase.Defaults.AiAggressiveness);
            Precision = Jitter(GameBase.Defaults.AiPrecision);
        }

        private float Jitter(float value)
        {
            return MathHelper.Clamp(value + ((float)Context.Rand.NextDouble() - 0.5f) * 0.2f, 0f, 1f);
        }
    }
}
