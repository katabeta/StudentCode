/// <summary>
        /// Drives the robot straight forward for a given distance, using the PID controller. For use in autonomous mode.
        /// Increases autoStep by 1 after driving forward.
        /// </summary>
        /// <param name="distance">the distance to travel</param>
        public void driveStraight(float distance)
        {
            //Debug.Print("Starting to drive straight");

            float p = 1.0f;
            float i = 1.0f;
            float d = 1.0f;
            rightMotor.EnableSpeedPID(p, i, d);
            leftMotor.EnableSpeedPID(p, i, d);

            if (leftEncoder.Displacement >= distance)
            {
                autoStep++;
            }
            //not sure if/when Update() needs to be called    
            this.leftEncoder.Update();
            this.rightEncoder.Update();
            this.leftMotor.Throttle = 60;
            this.rightMotor.Throttle = 60;
            
            Debug.Print(leftEncoder.Displacement.ToString());
            
            //rightMotor.DisablePID();
            //leftMotor.DisablePID();
        }
