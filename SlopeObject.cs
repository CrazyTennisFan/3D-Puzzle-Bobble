using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DBubble
{
    class SlopeObject
    {
        public Model model { get; protected set; }
        protected Matrix worldPosition = Matrix.Identity;

        public SlopeObject(Model m)
        {
            model = m;
        }

        public Matrix getPosition()
        {
            return worldPosition *Matrix.CreateRotationX(MathHelper.Pi * 30 / 180); ;
            //return worldPosition * Matrix.CreateTranslation(new Vector3(0, 6, 0)) * Matrix.CreateRotationX(-MathHelper.Pi * 60 / 180); 
            
        }
    }
}
