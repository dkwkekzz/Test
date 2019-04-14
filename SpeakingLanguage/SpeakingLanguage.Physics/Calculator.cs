using SpeakingLanguage.Core;
using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Physics
{
    static class Calculator
    {
        public static void ComputeForce(INode<int> iRoot, out int outPosX, out int outPosY)
        {
            var mess = iRoot[Constants.MESS].Value;
            var posX = iRoot[Constants.POSITION][Constants.X].Value;
            var posY = iRoot[Constants.POSITION][Constants.X].Value;
            var speedX = iRoot[Constants.SPEED][Constants.X].Value;
            var speedY = iRoot[Constants.SPEED][Constants.X].Value;
            var accelX = iRoot[Constants.ACCEL][Constants.Y].Value;
            var accelY = iRoot[Constants.ACCEL][Constants.Y].Value;
            speedX += accelX;
            speedY += accelY;
            posX += speedX;
            posY += speedY;

            if (speedX > 0)
            {
                speedX -= mess;
                if (speedX < 0)
                    speedX = 0;
            }
            else if (speedX < 0)
            {
                speedX += mess;
                if (speedX > 0)
                    speedX = 0;
            }

            if (speedY > 0)
            {
                speedY -= mess;
                if (speedY < 0)
                    speedY = 0;
            }
            else if (speedY < 0)
            {
                speedY += mess;
                if (speedY > 0)
                    speedY = 0;
            }

            outPosX = posX;
            outPosY = posY;
        }
    }
}
