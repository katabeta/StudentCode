// <copyright file="StudentCode.cs" company="Pioneers in Engineering">
// Licensed to Pioneers in Engineering under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  Pioneers in Engineering licenses 
// this file to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
//  with the License.  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

namespace StudentPiER
{
    using System;
    using PiE.HAL.GHIElectronics.NETMF.FEZ;
    using PiE.HAL.GHIElectronics.NETMF.Hardware;
    using PiE.HAL.Microsoft.SPOT;
    using PiE.HAL.Microsoft.SPOT.Hardware;
    using PiEAPI;

    /// <summary>
    /// Student Code template
    /// </summary>
    public class StudentCode : RobotCode
    {
        /// <summary>
        /// This is your robot
        /// </summary>
        private Robot robot;

        /// <summary>
        /// This stopwatch measures time, in seconds
        /// </summary>
        private Stopwatch stopwatch;

        /// <summary>
        /// The right drive motor, on connector M0
        /// </summary>
        private GrizzlyBear rightMotor;

        /// <summary>
        /// The left drive motor, on connector M1
        /// </summary>
        private GrizzlyBear leftMotor;

        /// <summary>
        /// The sonar sensor on connector A5
        /// </summary>
        private AnalogSonarDistanceSensor sonar;

        /// <summary>
        /// A flag to toggle RFID usage in the code
        /// </summary>
        //private bool useRfid;

        /// <summary>
        /// The rfid sensor
        /// </summary>
        //private Rfid rfid;

        /// <summary>
        /// This is your I2CEncoder
        /// </summary>
        private GrizzlyEncoder leftEncoder;
        private GrizzlyEncoder rightEncoder;
        
        //Step to use with the ^^ encoder
        private float Step = 0.0125f;

        //Create a new MicroMaestro and servos
        private MicroMaestro mm;
        private ServoMotor servo0;

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref="StudentPiER.StudentCode"/> class.
        /// </summary>
        /// <param name='robot'>
        ///   The Robot to associate with this StudentCode
        /// </param>
        public StudentCode(Robot robot)
        {
            this.robot = robot;
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
            //this.useRfid = false;
            //if (this.useRfid)
            //{
            //    this.rfid = new Rfid(robot);
            //}
            this.leftMotor = new GrizzlyBear(robot, Watson.Motor.M0);
            this.rightMotor = new GrizzlyBear(robot, Watson.Motor.M1);
            this.sonar = new AnalogSonarDistanceSensor(robot, Watson.Analog.A5);
            this.leftEncoder = new GrizzlyEncoder(Step, this.leftMotor, this.robot);
            this.rightEncoder = new GrizzlyEncoder(Step, this.rightMotor, this.robot);
        }
            

        /// <summary>
        /// Main method which initializes the robot, and starts
        /// it running. Do not modify.
        /// </summary>
        public static void Main()
        {
            // Initialize robot
            Robot robot = new Robot("1", "COM4");
            Debug.Print("Code loaded successfully!");
            Supervisor supervisor = new Supervisor(new StudentCode(robot));
            supervisor.RunCode();
        }

        /// <summary>
        ///  The Robot to use.
        /// </summary>
        /// <returns>
        ///   Robot associated with this StudentCode.
        /// </returns>
        public Robot GetRobot()
        {
            return this.robot;
        }

       
        /// <summary>
        /// The robot will call this method every time it needs to run the
        /// user-controlled student code
        /// The StudentCode should basically treat this as a chance to read all the
        /// new PiEMOS analog/digital values and then use them to update the
        /// actuator states
        /// </summary>
        
        public void SpeedAdjust()
        {
            double timeStart = this.stopwatch.ElapsedTime;
            float startRightDisplacement = this.rightEncoder.Displacement;
            float startLeftDisplacement = this.leftEncoder.Displacement;

            double time = stopwatch.ElapsedTime;
            float leftWheelSpeed = this.leftEncoder.Displacement / (float)stopwatch.ElapsedTime;
            float rightWheelSpeed = this.rightEncoder.Displacement / (float)stopwatch.ElapsedTime;

            float SpeedRatio = leftWheelSpeed / rightWheelSpeed;
        }


        public int GetTrueThrottle(int baseThrottle, Boolean slow, int deadpan)
        {
            if (!slow)
            {
                return baseThrottle;
            }
            if (baseThrottle > deadpan)
            {
                return ((int)(baseThrottle * 0.4) + 40);
            }
            if (baseThrottle < -1 * deadpan)
            {
                return ((int)(baseThrottle * 0.4) - 40);
            }
            else
            {
                return 0;
            }
        }


        public void TeleoperatedCode()
        {
            //Enable the Rfid Scanner - Press Right Button.

            if (this.robot.FeedbackDigitalVals[5] == true)
            {
                this.useRfid = true;
                Debug.Print("Scanning of the tag in progress!");

                if (rfid.CurrentItemScanned != null)
                {
                    //If we are in the range of a box, find it's info
                    uint boxItemID = rfid.CurrentItemScanned.ItemID;
                    int boxIDCurrent = rfid.CurrentItemScanned.GroupId;
                    int boxTypeCurrent = rfid.CurrentItemScanned.GroupType;

                    Debug.Print("ItemID  = " + boxItemID);
                    Debug.Print("GroupID  = " + boxIDCurrent);
                    Debug.Print("GroupType  = " + boxIDCurrent);

                    //Send Rfid Code - Press Left Button
                    //Allow pilot to choose when to release code
                    //Automates the process of choosing which code to release
                    if (this.robot.FeedbackDigitalVals[4] == true)
                    {
                        if (boxTypeCurrent == FieldItem.LeftReleaseCode)
                        {
                            //release LeftReleaseCode
                            this.robot.SendReleaseCode();
                        }

                        if (boxTypeCurrent == FieldItem.RightReleaseCode)
                        {
                            //realese RightReleaseCode
                            this.robot.SendReleaseCode();
                        }
                    }
                }
                else
                {
                    Debug.Print("null");
                }

                Debug.Print("LastItemScanned:");
                //Test if we've ever found a box
                if (rfid.LastItemScanned != null)
                {
                    //Info about last scanned item
                    int boxIdLast = rfid.LastItemScanned.GroupId;
                    int boxTypeLast = rfid.LastItemScanned.GroupType;

                    Debug.Print("ItemID  = " + boxItemID);
                    Debug.Print("GroupID  = " + boxIdLast);
                    Debug.Print("GroupdType  = " + boxTypeLast);
                }
                else
                {
                    Debug.Print("null");
                }

            }
            else
            {
                this.useRfid = false;
            }


            //New MicroMaestros
            mm = new MicroMaestro(robot, 1);
            //New Servos
            servo0 = new ServoMotor(robot, mm, 0, 0, 75, 0);

            //if (this.robot.FeedbackDigitalVals[0] == true)
            //{
            //    servo0.Write();
            //}

            //Debug.Print("Tele-op " + this.stopwatch.ElapsedTime);
            this.rightMotor.Throttle = GetTrueThrottle(this.robot.PiEMOSAnalogVals[1], this.robot.PiEMOSDigitalVals[6], 5);
            this.leftMotor.Throttle = -1 * GetTrueThrottle(this.robot.PiEMOSAnalogVals[3], this.robot.PiEMOSDigitalVals[6], 5);

            this.robot.FeedbackAnalogVals[0] = this.rightMotor.Throttle;
            this.robot.FeedbackAnalogVals[1] = this.leftMotor.Throttle;

            //Regular Driving
            //Tank Mode - Press Right Stick
            if (this.robot.PiEMOSDigitalVals[7] == false)
            {
                this.rightMotor.Throttle = this.robot.PiEMOSAnalogVals[1];
                this.leftMotor.Throttle = this.robot.PiEMOSAnalogVals[3];


                this.robot.FeedbackAnalogVals[0] = this.rightMotor.Throttle;
                this.robot.FeedbackAnalogVals[1] = this.leftMotor.Throttle;
            }

            //Arcade Mode - Press Right Stick
            if (this.robot.PiEMOSDigitalVals[7] == true)
            {
                this.rightMotor.Throttle = (this.robot.PiEMOSAnalogVals[3] - this.robot.PiEMOSAnalogVals[0]);
                this.leftMotor.Throttle = (this.robot.PiEMOSAnalogVals[3] + this.robot.PiEMOSAnalogVals[0]);
            }

            if (this.robot.PiEMOSDigitalVals[7] == true)
            {
                this.rightMotor.Throttle = (this.robot.PiEMOSAnalogVals[3] - this.robot.PiEMOSAnalogVals[0]);
                this.leftMotor.Throttle = (this.robot.PiEMOSAnalogVals[3] + this.robot.PiEMOSAnalogVals[0]);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Slow Mode - Left Stick
            if (this.robot.PiEMOSDigitalVals[6] == true)
            {

                //Tank Mode - Press Right Stick
                if (this.robot.PiEMOSDigitalVals[7] == false)
                {
                    this.rightMotor.Throttle = (int)(this.robot.PiEMOSAnalogVals[1] * 0.4) + 40;
                    this.leftMotor.Throttle = (int)(this.robot.PiEMOSAnalogVals[3] * 0.4) - 40;


                    this.robot.FeedbackAnalogVals[0] = this.rightMotor.Throttle;
                    this.robot.FeedbackAnalogVals[1] = this.leftMotor.Throttle;
                }

                //Arcade Mode - Press Right Stick
                if (this.robot.PiEMOSDigitalVals[7] == true)
                {
                    this.rightMotor.Throttle = (this.robot.PiEMOSAnalogVals[3] - this.robot.PiEMOSAnalogVals[1]) * 2 / 5 + 40;
                    this.leftMotor.Throttle = (this.robot.PiEMOSAnalogVals[3] + this.robot.PiEMOSAnalogVals[1]) * 2 / 5 - 40;
                }
            }

            else
            {
                //Tank Mode - Press Right Stick
                if (this.robot.PiEMOSDigitalVals[7] == false)
                {
                    this.rightMotor.Throttle = this.robot.PiEMOSAnalogVals[1];
                    this.leftMotor.Throttle = this.robot.PiEMOSAnalogVals[3];


                    this.robot.FeedbackAnalogVals[0] = this.rightMotor.Throttle;
                    this.robot.FeedbackAnalogVals[1] = this.leftMotor.Throttle;
                }

                //Arcade Mode - Press Right Stick
                if (this.robot.PiEMOSDigitalVals[7] == true)
                {
                    this.rightMotor.Throttle = (this.robot.PiEMOSAnalogVals[3] - this.robot.PiEMOSAnalogVals[0]);
                    this.leftMotor.Throttle = (this.robot.PiEMOSAnalogVals[3] + this.robot.PiEMOSAnalogVals[0]);
                }
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////


        }


        /// <summary>
        /// The robot will call this method every time it needs to run the
        /// autonomous student code
        /// The StudentCode should basically treat this as a chance to change motors
        /// and servos based on non user-controlled input like sensors. But you
        /// don't need sensors, as this example demonstrates.
        /// </summary>
        
        public void AutonomousCode()
        {

            //if (/*some button = true*/)
            //{
            //    /*this.leftMotor.Throttle = -60;
            //    this.rightMotor.Throttle = 60;*/

            //    float leftDisplacement = this.leftEncoder.Displacement;
            //    float rightDisplacement = this.rightEncoder.Displacement;

            //    //move forward initially
            //    if (leftDisplacement < 6.0)
            //    {
            //        Debug.Print("forward");
            //        this.rightMotor.Throttle = 90;
            //        this.leftMotor.Throttle = 90;
            //    }

            //    //turn right (to get to the goal)
            //    else if (leftDisplacement < 6.0 + 4.5 * 3.14 )
            //    {
            //        Debug.Print("Turn Right");
            //        this.rightMotor.Throttle = -50;
            //        this.leftMotor.Throttle = 50;
            //    }

            //        //move forward
            //        else if (leftDisplacement < 6.0 + 4.5 * 3.14 + 4.0)
            //    {
            //        Debug.Print("forward 2");
            //        this.rightMotor.Throttle = 90;
            //        this.leftMotor.Throttle = 90;
            //    }

            //    //turn left (to face the goal)
            //    else if (leftDisplacement < 6.0 + 4.5 * 3.14 + 4.0 - 4.5 * 3.14)
            //    {
            //        Debug.Print("Turn Left");
            //        this.rightMotor.Throttle = 50;
            //        this.leftMotor.Throttle = -50;
            //    }
            //}

            //else
            //{
            //    /////////////////////////////////////////////////////
            //    //Mirror for other side of the field
            //    /*this.leftMotor.Throttle = -60;
            //    this.rightMotor.Throttle = 60;*/

            //    float leftDisplacement = this.leftEncoder.Displacement;
            //    float rightDisplacement = this.rightEncoder.Displacement;

            //    //move forward initially
            //    if (leftDisplacement < 6.0)
            //    {
            //        Debug.Print("forward");
            //        this.rightMotor.Throttle = 90;
            //        this.leftMotor.Throttle = 90;
            //    }

            //    //turn left (to get to the goal)
            //    else if (leftDisplacement < 6.0 - 4.5 * 3.14)
            //    {
            //        Debug.Print("Turn Right");
            //        this.rightMotor.Throttle = -50;
            //        this.leftMotor.Throttle = 50;
            //    }

            //    //move forward
            //    else if (leftDisplacement < 6.0 - 4.5 * 3.14 + 4.0)
            //    {
            //        Debug.Print("forward 2");
            //        this.rightMotor.Throttle = 90;
            //        this.leftMotor.Throttle = 90;
            //    }

            //    //turn right (to face the goal)
            //    else if (leftDisplacement < 6.0 - 4.5 * 3.14 + 4.0 + 4.5 * 3.14)
            //    {
            //        Debug.Print("Turn Left");
            //        this.rightMotor.Throttle = 50;
            //        this.leftMotor.Throttle = -50;
            //    }
            //}

            //Max's set
            float leftDisplacement = this.leftEncoder.Displacement;
            float rightDisplacement = this.rightEncoder.Displacement;
            float TotalDisplacement = Math.Abs((int)this.leftEncoder.Displacement) + Math.Abs((int)this.rightEncoder.Displacement);

            this.robot.SendConsoleMessage("TotalDisplacement = " + Math.Abs((int)this.leftEncoder.Displacement) + Math.Abs((int)this.rightEncoder.Displacement));
            
            
            //move forward initially
            if (TotalDisplacement < 40.0)
            {
                Debug.Print("Forward");
                this.rightMotor.Throttle = 60;
                this.leftMotor.Throttle = -60;
            }
            //turn right (to get to the goal)
            else if (TotalDisplacement < 60.0)
            {
                Debug.Print("Turn Right");
                this.rightMotor.Throttle = -90;
                this.leftMotor.Throttle = -90;
            }
            //move forward
            else if (TotalDisplacement < 80.0)
            {
                Debug.Print("Forward 2");
                this.rightMotor.Throttle = 60;
                this.leftMotor.Throttle = -60;
            }
            //turn left (to face the goal)
            else if (TotalDisplacement < 100)
            {
                Debug.Print("Turn Left");
                this.rightMotor.Throttle = 90;
                this.leftMotor.Throttle = 90;
            }


        }
        

        /// <summary>
        /// The robot will call this method periodically while it is disabled
        /// during the autonomous period. Actuators will not be updated during
        /// this time.
        /// </summary>
        public void DisabledAutonomousCode()
        {
            this.stopwatch.Reset(); // Restart stopwatch before start of autonomous
        }

        /// <summary>
        /// The robot will call this method periodically while it is disabled
        /// during the user controlled period. Actuators will not be updated
        /// during this time.
        /// </summary>
        public void DisabledTeleoperatedCode()
        {
        }

        /// <summary>
        /// This is called whenever the supervisor disables studentcode.
        /// </summary>
        public void WatchdogReset()
        {
        }

        /// <summary>
        /// Send the GroupType of a FieldItem object to PiEMOS.
        /// Populates two indices of FeedbackDigitalVals.
        /// </summary>
        /// <param name="item">the FieldItem to send infotmaion about</param>
        /// <param name="index1">first index to use</param>
        /// <param name="index2">second index to use</param>
        private void ReportFieldItemType(FieldItem item, int index1 = 6, int index2 = 7)
        {
            bool feedback1;
            bool feedback2;

            if (item == null)
            {
                feedback1 = false;
                feedback2 = false;
            }
            else if (item.GroupType == FieldItem.PlusOneBox)
            {
                feedback1 = true;
                feedback2 = false;
            }
            else if (item.GroupType == FieldItem.TimesTwoBox)
            {
                feedback1 = true;
                feedback2 = true;
            }
            else
            {
                feedback1 = false;
                feedback2 = true;
            }

            this.robot.FeedbackDigitalVals[index1] = feedback1;
            this.robot.FeedbackDigitalVals[index2] = feedback2;
        }
    }
}
