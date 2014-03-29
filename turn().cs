/// <summary>
        /// Turns the robot through a certain angle. Increases autoStep by 1 after turning.
        /// </summary>
        /// <param name="degrees">The angle through which to turn. If degrees is positive, turn to the right.</param>
        public void turn(int degrees)
        {
            leftEncoder.Displacement = 0;
            rightEncoder.Displacement = 0;
            float displacementPerDegree = .15f;
            //if turning right
            if (degrees > 0)
            {
                while (leftEncoder.Displacement < displacementPerDegree * degrees)
                {
                    this.leftEncoder.Update();
                    this.rightEncoder.Update();
                    leftMotor.Throttle = -60;
                    rightMotor.Throttle = 60;
                }
                leftMotor.Throttle = 0;
                rightMotor.Throttle = 0;
                autoStep++;
            }
            else
            {
                while (leftEncoder.Displacement < displacementPerDegree * (-1)*degrees)
                {
                    this.leftEncoder.Update();
                    this.rightEncoder.Update();
                    leftMotor.Throttle = 60;
                    rightMotor.Throttle = -60;
                }
                leftMotor.Throttle = 0;
                rightMotor.Throttle = 0;
                autoStep++;
            }
        }
