using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AchtungDieKurve.Game.Core;
using AchtungDieKurve.Game.Drawable;

namespace AchtungDieKurve.Game.AI.Activities
{
    internal struct DirectionCandidate
    {
        public int AngleStart;
        public int AngleEnd;
        public PotentialCollision BlockA;
        public PotentialCollision BlockB;

        public float GapSize
        {
            get
            {
                if (BlockA.Owner is Wall || BlockB.Owner is Wall)
                {
                    return 1f;
                }
                //@todo: count the "meat" of blocks into the gap
                return Vector2.Distance(new Vector2(BlockA.Center.X, BlockA.Center.Y),
                    new Vector2(BlockB.Center.Y, BlockB.Center.Y));
            }
        }

        public double AngleCenter
        {
            get { return Trigonometry.AnglesCenter(AngleStart, AngleEnd); }
        }

        public DirectionCandidate(int angleStart, int angleEnd, PotentialCollision blockA, PotentialCollision blockB) : this()
        {
            AngleStart = angleStart;
            AngleEnd = angleEnd;
            BlockA = blockA;
            BlockB = blockB;
        }
    }
    internal class Survive : IAiActivity
    {
        
        public AiActivity Type
        {
            get { return AiActivity.Survive; }
        }

        public Vector2 Propose(ArcMatrix matrix)
        {
            if (matrix.Arc.Count == 0)
            {
                return new Vector2(0, matrix.Player.Angle);
            }
            var bestDirection = new DirectionCandidate();
            var sortedAngles = matrix.Arc.Keys.ToList();
            sortedAngles.Sort();
            for (var i = 0; i < sortedAngles.Count; i++)
            {
                if (i == 0)
                {
                    bestDirection = new DirectionCandidate(sortedAngles.Last(), sortedAngles[i], matrix.Arc[sortedAngles.Last()], matrix.Arc[sortedAngles[i]]);
                }
                if (i == sortedAngles.Count -1)
                {
                    break;
                }
                var delta = new DirectionCandidate(sortedAngles[i], sortedAngles[i+1], matrix.Arc[sortedAngles[i]], matrix.Arc[sortedAngles[i+1]]);
                if (delta.GapSize > bestDirection.GapSize)
                {
                    bestDirection = delta;
                }
            }

            return bestDirection.GapSize > matrix.Player.Radius * 2 
                ? new Vector2(1f,(float)bestDirection.AngleCenter)
                : new Vector2(1f, (float)Trigonometry.AngleBetween(matrix.Arc[sortedAngles.First()].Center, matrix.Arc[sortedAngles.Last()].Center));
        }

        public Vector2 Proposeold(ArcMatrix matrix)
        {
            if (matrix.Arc.Count == 0)
            {
                return new Vector2(0, matrix.Player.Angle);
            }

            var radius = 0;
            var st = 0;
            var e = 0;
            var row = 0;
            var start = 0;
            var end = 0;
            var st2 = 360;
            var e2 = 0;
            int radius2;
            double desiredAngle = 0;

            for (int i = 0; i < 360; i++)
            {
                var position = Trigonometry.WrapAngleDegrees(i);
                if (matrix.Arc[position] == null)
                {
                    if (radius < row)
                    {
                        radius = row;
                        e = i;
                        st = start;
                    }
                    else
                    {
                        row++;
                        end = i;
                    }
                }
                else
                {
                    row = 0;
                    end = 0;
                    start = i;
                }
            }

            for (int i = 359; i >= 0; i--)
            {
                var position = Trigonometry.WrapAngleDegrees(i);
                if (matrix.Arc[position] != null)
                {
                    break;
                }
                st2 = i;
            }
            for (int i = 0; i < 359; i++)
            {
                var position = Trigonometry.WrapAngleDegrees(i);
                
                if (matrix.Arc[position] != null)
                {
                    break;
                }
                e2 = i;
            }

            radius2 = ((360 - st2) + e2);

            if (radius2 > radius)
            {
                desiredAngle = MathHelper.WrapAngle(
                    MathHelper.ToRadians(st2 + (radius2/2))
                    );
            }
            else
            {
                desiredAngle = MathHelper.WrapAngle(
                    MathHelper.ToRadians(st + (radius/2))
                    );
            }
            if ((int)desiredAngle == 0)
            {
                desiredAngle = matrix.Player.Angle;
            }
            return new Vector2(1f, (float) desiredAngle);
        }

        public Vector2 DrawPropose(ArcMatrix matrix, SpriteBatch sb)
        {

            return Propose(matrix);

            /*
             *  desiredAngle = MathHelper.WrapAngle(
                    MathHelper.ToRadians(st2 + (radius2 / 2))
                    );

                for (int i = st2; i < radius2; i++)
                {
                    SB.Draw(CommonResources.whitepixel, new Rectangle(
                        (int)(Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(st2 + i))) * 50) + (int)player.CollisionBounds.Center.X,
                        (int)(Math.Sin(MathHelper.WrapAngle(MathHelper.ToRadians(st2 + i))) * 50) + (int)player.CollisionBounds.Center.Y,
                        4,4
                        ), Color.White);
                }    */
        }
    }
}