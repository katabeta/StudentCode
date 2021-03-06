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
        /// The motor controlling the position of the hopper, on connector M2
        /// </summary>
        private GrizzlyBear hopperMotor;

        /// <summary>
        /// The motor controlling the converyor belt for the pick-up mechanism, on connector M3
        /// </summary>
        private GrizzlyBear conveyorBeltMotor;


        /// <summary>
        /// A flag to toggle RFID usage in the code
        /// </summary>
        private bool useRfid;

        /// <summary>
        /// The rfid sensor
        /// </summary>
        private Rfid rfid;

        /// <summary>
        /// This is your I2CEncoder
        /// </summary>
        private GrizzlyEncoder leftEncoder;
        private GrizzlyEncoder rightEncoder;

        /// <summary>
        /// Step to use with the ^^ encoder
        /// </summary>
        private float Step = 0.0125f;

        /// <summary>
        /// The limit switch for testing if the hopper is open, on connector D1
        /// </summary>
        private DigitalLimitSwitch servoSwitch;

        /// <summary>
        /// The limit switch for testing if the hopper is closed, on connector D2
        /// </summary>
        private DigitalLimitSwitch autonomousSwitch;

        /// <summary>
        /// Create a new MicroMaestro and servos
        /// </summary>
        private MicroMaestro mm;
        private ServoMotor servo0;

        //For autonomous
        private Boolean StartTime;

        //for hopper delay
        //private Boolean dump;
        //private Boolean notDump;

        //For stage 3 autonomous
        //private int n;

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
            this.useRfid = false;
            /*if (this.useRfid)
            {
                this.rfid = new Rfid(robot);
            }*/
            this.leftMotor = new GrizzlyBear(robot, Watson.Motor.M0);
            this.rightMotor = new GrizzlyBear(robot, Watson.Motor.M1);
            this.conveyorBeltMotor = new GrizzlyBear(robot, Watson.Motor.M3);
            this.hopperMotor = new GrizzlyBear(robot, Watson.Motor.M2);
            this.sonar = new AnalogSonarDistanceSensor(robot, Watson.Analog.A5);
            this.leftEncoder = new GrizzlyEncoder(Step, this.leftMotor, this.robot);
            this.rightEncoder = new GrizzlyEncoder(Step, this.rightMotor, this.robot);
            this.servoSwitch = new DigitalLimitSwitch(robot, Watson.Digital.D1);
            this.autonomousSwitch = new DigitalLimitSwitch(robot, Watson.Digital.D3);
            this.StartTime = true;
            this.mm = new MicroMaestro(robot, 12);   
            this.servo0 = new ServoMotor(robot, mm, 0, 500, 2000);
            //this.dump = true;
            //this.notDump = false;
            //this.n = 0;
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


        /// <summary>
        /// Determines throttle value based on the activation of precision mode
        /// </summary>
        /// <param name="baseThrottle">The input value from the controller. The analog value of the left and right sticks.</param>
        /// <param name="slow">Whether slow mode is initiated or not</param>
        /// <param name="deadpan">The margin for error in the released throttle</param>
        /// <returns>The adjusted throttle value</returns>
        /// Not being used
        public int GetTrueThrottle(int baseThrottle, Boolean slow, int deadpan)
        {
            if (slow)
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


        /// <summary>
        /// Uses the position of the hopper to determine the speed at which the motor should rotate when the driver elects to move the door
        /// </summary>
        /// <returns>A value of throttle for the hopper motor</returns>
        public int getTrueHopperThrottle()
        {
            //Open hopper - Press once to open RightThumbB, press again to close
            if (this.robot.PiEMOSDigitalVals[7] == true)
            {
                //this.dump = this.notDump;
                return -20;
            }

            else
            {
                //this.notDump = this.dump;
                return 60;
            }

        }


        public void TeleoperatedCode()
        {
            //Turn on/off slow mode - Press Left Stick
            this.rightMotor.Throttle = GetTrueThrottle(this.robot.PiEMOSAnalogVals[1], this.robot.PiEMOSDigitalVals[6], 5);
            this.leftMotor.Throttle = -1 * GetTrueThrottle(this.robot.PiEMOSAnalogVals[3], this.robot.PiEMOSDigitalVals[6], 5);


            //key bindings for wheel throttle
            //RightMotor - Right Stick y-axis
            //LeftMotor - Right Stick x-axis
            this.robot.FeedbackAnalogVals[0] = this.rightMotor.Throttle;
            this.robot.FeedbackAnalogVals[1] = this.leftMotor.Throttle;


            //Turn on conveyor belt - Press and Hold Left Trigger
            this.conveyorBeltMotor.Throttle = this.robot.PiEMOSAnalogVals[5] + -1 * this.robot.PiEMOSAnalogVals[4];




            //Open hopper - Press once to open RightThumbB, press again to close
            this.hopperMotor.Throttle = getTrueHopperThrottle();        
            

            //Start Flashing Dispensor and compare release code - Press and Hold RB
            /*if (this.robot.PiEMOSDigitalVals[5] == true)
            {
                this.useRfid = true;
                if (this.useRfid && (this.rfid.LastItemScanned != null))
                {
                    FieldItem last = this.rfid.LastItemScanned;
                    int groupTypeLast = last.GroupType;
                    this.robot.SendConsoleMessage("Scanned");

                    if (groupTypeLast == 3 || groupTypeLast == 4)
                    {
                        this.robot.StartFlashingDispenser(last);
                        this.robot.SendConsoleMessage("Flashing");

                        if (this.robot.PiEMOSDigitalVals[4] == true)
                        {
                            this.robot.SendReleaseCode(last);
                            this.robot.SendConsoleMessage("Releasing");
                        }
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                this.useRfid = false;
            }*/
                /*this.useRfid = true;
                this.robot.SendConsoleMessage("Test");
                this.robot.SendConsoleMessage("Last item scanned type: " + this.rfid.LastItemScanned.GroupType.ToString());
                
                //Check if lastItemScanned was a false release code
                int groupType = this.rfid.LastItemScanned.GroupType;
                
                //Make sure that releasing the code will not disable robot
                if (this.rfid.LastItemScanned.GroupType != PiEAPI.FieldItem.FalseReleaseCode && this.rfid.LastItemScanned != null)
                {
                    this.robot.SendConsoleMessage("Test 2");
                    //Find which dispenser this release code corresponds to
                    this.robot.StartFlashingDispenser(this.rfid.LastItemScanned);
                    //this.robot.SendConsoleMessage(this.rfid.LastItemScanned.ToString());

                    //Release code - Press and Hold Left Bumper while holding RB
                    if (this.robot.FeedbackDigitalVals[4] == true)
                    {
                        this.robot.SendConsoleMessage("test 3");
                        this.robot.SendReleaseCode(this.rfid.LastItemScanned);
                        this.robot.SendConsoleMessage("Code releasing");
                        this.robot.StopFlashingDispenser(this.rfid.LastItemScanned);
                    }
                }
            }*/



            //Turn on servo - Press and Hold Right Bumper
            /*this.servo0.TargetRotation = this.robot.FeedbackAnalogVals[4];
            if (this.robot.PiEMOSDigitalVals[1] == true && this.servo0.TargetRotation == 0)
            {
                this.servo0.TargetRotation = 50;
                //this.servo0.AngularSpeed = 0;
                //this.robot.SendConsoleMessage(this.servo0.TargetRotation.ToString());
                this.servo0.Write();
            }
            else if (this.robot.PiEMOSDigitalVals[1] == false && this.servo0.TargetRotation == 50)
            {
                this.servo0.TargetRotation = 0;
                //this.servo0.AngularSpeed = 0;
                //this.robot.SendConsoleMessage(this.servo0.TargetRotation.ToString());
                this.servo0.Write();
            }
            else if (this.robot.PiEMOSDigitalVals[1] == true && (this.servo0.TargetRotation != 0 && this.servo0.TargetRotation != 90))
            {
                this.servo0.TargetRotation = 0;
                //this.servo0.AngularSpeed = 0;
                this.robot.SendConsoleMessage(this.servo0.TargetRotation.ToString());
                //this.servo0.Write();
            }*/


            /*//Tank Mode - Press Right Stick
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
            }*/
        }


        /// <summary>
        /// The robot will call this method every time it needs to run the
        /// autonomous student code
        /// The StudentCode should basically treat this as a chance to change motorsw
        /// and servos based on non user-controlled input like sensors. But you
        /// don't need sensors, as this example demonstrates.
        /// </summary>
        public void AutonomousCode()
        {
            Debug.Print("Autonomous");
            //this.robot.SendConsoleMessage(n.ToString());//
            float leftDisplacement = this.leftEncoder.Displacement;
            float rightDisplacement = this.rightEncoder.Displacement;
            float TotalDisplacement = Math.Abs((int)this.leftEncoder.Displacement) + Math.Abs((int)this.rightEncoder.Displacement);
            
              
              
            this.rightMotor.Throttle = GetTrueThrottle(this.robot.PiEMOSAnalogVals[1], this.robot.PiEMOSDigitalVals[6], 5);
            this.leftMotor.Throttle = -1 * GetTrueThrottle(this.robot.PiEMOSAnalogVals[3], this.robot.PiEMOSDigitalVals[6], 5); 
            



            this.robot.SendConsoleMessage("TotalDisplacement = " + (Math.Abs((int)this.leftEncoder.Displacement) + Math.Abs((int)this.rightEncoder.Displacement)));
            
            //this.hopperMotor.Throttle = 60;


            //stage 3 autonomous - getting ball into goal
            /*switch (n)
            {
                case 0:
                    this.robot.SendConsoleMessage("reached 0");
                    if (TotalDisplacement < 600.0)
                    {
                        this.robot.SendConsoleMessage("Forward");
                        this.rightMotor.Throttle = -100;
                        this.leftMotor.Throttle = 100;
                    }
                    else
                    {
                        n = 1;
                    }
            
                    break;
                case 1:
                    this.robot.SendConsoleMessage("reached 1");
                    if (TotalDisplacement < 1400.0)
                    {
                        this.robot.SendConsoleMessage("veer Left");
                        this.rightMotor.Throttle = 100;
                        this.leftMotor.Throttle = 95;
                    }
                    else
                    {
                        n = 2;
                    }
                    break;
                case 2:
                    this.robot.SendConsoleMessage("reached 2");
                    if (TotalDisplacement < 2000.0)
                    {
                        this.robot.SendConsoleMessage("Turn");
                        this.rightMotor.Throttle = 100;
                        this.leftMotor.Throttle = 0;
                    }
                    break;
                case 3:
                    this.robot.SendConsoleMessage("reached 2");
                    if (TotalDisplacement < 1600.0)
                    {
                        this.robot.SendConsoleMessage("Staigheten");
                        this.rightMotor.Throttle = 100;
                        this.leftMotor.Throttle = -100;
                    }
                    else
                    {
                        n = 3;
                    }
                    break;
                case 4:
                    this.robot.SendConsoleMessage("reached 3");
                    if (TotalDisplacement < 2300.0)
                    {
                        this.robot.SendConsoleMessage("Forward");
                        this.rightMotor.Throttle = -100;
                        this.leftMotor.Throttle = 100;
                    }
                    else
                    {
                        n = 4;
                    }
                    break;
                default:
                    this.robot.SendConsoleMessage("reached default");
                    this.rightMotor.Throttle = 0;
                    this.leftMotor.Throttle = 0;
                    break;
                }*/
                
       
            //move forward initially
            /*if (TotalDisplacement < 20.0)
            {
                this.hopperMotor.Throttle = -20;
                this.robot.SendConsoleMessage("Forward");
                this.rightMotor.Throttle = -100;
                this.leftMotor.Throttle = 100;
            }
            //turn left (to face the goal)
            else if (TotalDisplacement < 25.0)
            {
                this.robot.SendConsoleMessage("Turn Left");
                this.rightMotor.Throttle = 100;
                this.leftMotor.Throttle = 95;
            }
            //straighten against wall
            else if (TotalDisplacement < 29.3)
            {
                this.robot.SendConsoleMessage("Backwards");
                this.rightMotor.Throttle = 100;
                this.leftMotor.Throttle = -100;
            }
            //move forward (get to goal
            else if (TotalDisplacement < 60.0)
            {
                this.robot.SendConsoleMessage("Forward 2");
                this.rightMotor.Throttle = -100;
                this.leftMotor.Throttle = 100;
            }
            //stop when reached location
            else
            {
                this.conveyorBeltMotor.Throttle = 100;
                this.rightMotor.Throttle = 0;
                this.leftMotor.Throttle = 0;
            }*/

            //Stage 2 autonomous - getting ball over wall
    if (this.autonomousSwitch.IsPressed == true)
            {
                //veers to right
                if (StartTime)
                {
                    this.stopwatch.Start();
                    StartTime = false;
                }
                this.robot.SendConsoleMessage(this.stopwatch.ElapsedTime.ToString());
                this.rightMotor.Throttle = -100;
                this.leftMotor.Throttle = 85;
                this.hopperMotor.Throttle = -20;
                if (this.stopwatch.ElapsedTime >= 5.0)
                {
                    this.conveyorBeltMotor.Throttle = 100;

                }
            }

            if (this.autonomousSwitch.IsPressed == false)
            {
                //veers to left
                if (StartTime)
                {
                    this.stopwatch.Start();
                    StartTime = false;
                }
                this.robot.SendConsoleMessage(this.stopwatch.ElapsedTime.ToString());
                this.rightMotor.Throttle = -78;
                this.leftMotor.Throttle = 100;
                this.hopperMotor.Throttle = -20;
                if (this.stopwatch.ElapsedTime >= 5.0)
                {
                    this.conveyorBeltMotor.Throttle = 100;

                }
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

            //reset encoder displacement values
            this.leftEncoder.Displacement = 0;
            this.rightEncoder.Displacement = 0;
            this.rightMotor.Throttle = GetTrueThrottle(this.robot.PiEMOSAnalogVals[1], this.robot.PiEMOSDigitalVals[6], 5);
            this.leftMotor.Throttle = -1 * GetTrueThrottle(this.robot.PiEMOSAnalogVals[3], this.robot.PiEMOSDigitalVals[6], 5);
            this.stopwatch.Reset();
            this.StartTime = true;
        }

        /// <summary>
        /// The robot will call this method periodically while it is disabled
        /// during the user controlled period. Actuators will not be updated
        /// during this time.
        /// </summary>
        public void DisabledTeleoperatedCode()
        {
            this.stopwatch.Reset();
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
