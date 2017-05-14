using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace _3DBubble
{
    class _3DBall
    {
        public Model model { get; protected set; }
        protected Matrix worldPosition = Matrix.Identity;

        public Boolean hasVisited;
        public Boolean needDeleted;
        public Boolean canDrop;
        public int arrayPositionX;
        public int arrayPositionY;
        public float pitchAngle;
        int ballColor;   //1.Green  2.Blue  3.Red  
                         //4.Yellow 5.Cyan  6.Purple
        
        public Vector3 position;
        public Vector3 movingUnitDirection;

        public float verticalVelocity = 0;
        public float horizontalVelocity = 0;
        public float gravity = 0.0001f;
        public float scale = 1.0f;


        float speed = 0.01f;

        public _3DBall(int color, int x, int y)
        {
            arrayPositionX = x;
            arrayPositionY = y;
            ballColor = color;
            pitchAngle = 0;

            movingUnitDirection = new Vector3(0, 0, 1);

            hasVisited = false;
            needDeleted = false;
            canDrop = false;
        }
        /*
        public Vector3 getHorizontalRampPositionVector()
        {
            return ballOnHorizontalRampPositionVector;
        }
        */
        public void setModel(Model m)
        {
            model = m;
        }

        public int getBallColor()
        {
            return ballColor; 
        }

        private void calculateWorldPosition()
        {            
            worldPosition = Matrix.Identity * Matrix.CreateScale(scale) * Matrix.CreateFromAxisAngle(Vector3.Cross(new Vector3(0, 1, 0), movingUnitDirection), pitchAngle)
                                            * Matrix.CreateTranslation(position)
                                            * Matrix.CreateRotationX((float)Math.PI / 6);
        }

        public Matrix getWorldPosition()
        {
            calculateWorldPosition();
            return worldPosition;
        }

        
        public void moveOnScreen(int arrowAngle)
        {
            verticalVelocity = (float)Math.Sqrt(Math.Pow(verticalVelocity, 2) - 2 * ((-gravity) * (float)Math.Cos(arrowAngle * Math.PI / 180)) * (movingUnitDirection.Z * verticalVelocity));
            pitchAngle += (float)Math.PI / 90 * (verticalVelocity / (float)Math.Cos(arrowAngle * Math.PI / 180)) * 100;
            position += new Vector3((movingUnitDirection.X * horizontalVelocity), 0, (movingUnitDirection.Z * verticalVelocity));
       
            calculateWorldPosition();
        
        }
        
        public void translation(Vector3 vector)
        {
            position = vector;
            worldPosition = worldPosition * Matrix.CreateTranslation(position) * Matrix.CreateRotationX(MathHelper.Pi * 30 / 180);
           
        }
        
    }
}
