using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DBubble
{
    class CeilingObject
    {
        public Model model { get; protected set; }
        protected Matrix worldPosition = Matrix.Identity;
        public Vector3 position;

        public CeilingObject(Model m)
        {
            model = m;
            position = new Vector3(0, 0.094f, -0.6f);
            worldPosition = worldPosition * Matrix.CreateTranslation(position) * Matrix.CreateRotationX(30 * MathHelper.Pi / 180);
        }

        public Matrix getPosition()
        {
            return worldPosition;
                        
        }

        public void setPosition()
        {
            position = new Vector3(0, 0.024f, 0.4f);
            worldPosition = worldPosition * Matrix.CreateTranslation(position) * Matrix.CreateRotationX(30 * MathHelper.Pi / 180);
        }

        public void ceilingDown()
        {
            position = position + new Vector3(0, 0, 0.1f);
            worldPosition = Matrix.Identity * Matrix.CreateTranslation(position) * Matrix.CreateRotationX(30 * MathHelper.Pi / 180);
        }
    }
}
