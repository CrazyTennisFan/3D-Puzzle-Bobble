using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DBubble
{
    class ArrowObject
    {
        public Model model { get; protected set; }
        protected Matrix worldPosition = Matrix.Identity;

        public ArrowObject(Model m)
        {
            model = m;
            worldPosition = worldPosition * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(0, 0.02f, 0.46f)) * Matrix.CreateRotationX(30 * MathHelper.Pi / 180);
        }

        public Matrix getPosition()
        {
            return worldPosition;
        }

        public void updatePosition(int angle)
        {
            worldPosition = Matrix.Identity * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(-angle * MathHelper.Pi / 180) * Matrix.CreateTranslation(new Vector3(0, 0.02f, 0.46f)) * Matrix.CreateRotationX(30 * MathHelper.Pi / 180);
        }

    }
}
