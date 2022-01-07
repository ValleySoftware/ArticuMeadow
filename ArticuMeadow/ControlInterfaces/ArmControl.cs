using ArticuMeadow.Base;
using Meadow.Foundation.ICs.IOExpanders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Meadow.Hardware;

namespace ArticuMeadow.ControlInterfaces
{
    public class ArmControl
    {

        private BaseJoint _pivot;
        private BaseJoint _shoulder;
        private BaseJoint _elbow;
        private BaseJoint _wrist;

        private II2cBus _ii2cBus;
        private Mcp23x08 _portExpanderOne;
        private Mcp23x08 _portExpanderTwo;

        private bool _stop;

        private bool _isReady;

        public ArmControl()
        {
        }

        public bool IsReady
        {
            get => _isReady;
            set => _isReady = value;
        }

        public bool Stop
        {
            get => _stop;
            set
            {
                _stop = value;
            }
        }

        public bool Init()
        {
            var result = false;
            Console.WriteLine("Arm Init Starerd");

            try
            {
                _ii2cBus = MeadowApp.Device.CreateI2cBus(I2cBusSpeed.Standard);
                _portExpanderOne = new Mcp23x08(_ii2cBus, false, false, false);
                _portExpanderTwo = new Mcp23x08(_ii2cBus, false, false, true);
            }
            catch (Exception)
            {
                Console.WriteLine("Expander Init Fail");
                IsReady = false;
                return false;
            }

            try
            {
                result = InitJoints();
            }
            catch (Exception)
            {
                Console.WriteLine("Joint Init Fail");
                result = false;
            }

            IsReady = result;
            return result;
        }

        private bool InitJoints()
        {
            var result = false;
            Console.WriteLine("joint Inits Starerd");

            if (_portExpanderOne != null && 
                _portExpanderTwo != null)
            {

                try
                {
                    _pivot = new BaseJoint();
                    var pivotInfo = new JointInfoPacket()
                    {
                        JointDirection = TravelDirection.Rotation,
                        Name = "Pivot",
                        PinOne = _portExpanderOne.Pins.GP0,
                        PinTwo = _portExpanderOne.Pins.GP1,
                        PinThree = _portExpanderOne.Pins.GP2,
                        PinFour = _portExpanderOne.Pins.GP3,
                        ReadyPosition = -1,
                        StowedPosition = -1
                    };
                    Console.WriteLine("joint " + pivotInfo.Name + " Init complete: success = " + _pivot.Init(pivotInfo));

                    _shoulder = new BaseJoint();
                    var shoulderInfo = new JointInfoPacket()
                    {
                        JointDirection = TravelDirection.Elevation,
                        Name = "Shoulder",
                        PinOne = _portExpanderOne.Pins.GP4,
                        PinTwo = _portExpanderOne.Pins.GP5,
                        PinThree = _portExpanderOne.Pins.GP6,
                        PinFour = _portExpanderOne.Pins.GP7,
                        ReadyPosition = -1,
                        StowedPosition = -1
                    };
                    Console.WriteLine("joint " + shoulderInfo.Name + " Init complete: success = " + _shoulder.Init(shoulderInfo));

                    _elbow = new BaseJoint();
                    var elbowInfo = new JointInfoPacket()
                    {
                        JointDirection = TravelDirection.Elevation,
                        Name = "Elbow",
                        PinOne = _portExpanderTwo.Pins.GP0,
                        PinTwo = _portExpanderTwo.Pins.GP1,
                        PinThree = _portExpanderTwo.Pins.GP2,
                        PinFour = _portExpanderTwo.Pins.GP3,
                        ReadyPosition = -1,
                        StowedPosition = -1
                    };
                    Console.WriteLine("joint " + elbowInfo.Name + " Init complete: success = " + _elbow.Init(elbowInfo));

                    _wrist = new BaseJoint();
                    var wristInfo = new JointInfoPacket()
                    {
                        JointDirection = TravelDirection.Elevation,
                        Name = "Wrist",
                        PinOne = _portExpanderTwo.Pins.GP4,
                        PinTwo = _portExpanderTwo.Pins.GP5,
                        PinThree = _portExpanderTwo.Pins.GP6,
                        PinFour = _portExpanderTwo.Pins.GP7,
                        ReadyPosition = -1,
                        StowedPosition = -1
                    };
                    Console.WriteLine("joint " + wristInfo.Name + " Init complete: success = " + _wrist.Init(wristInfo));

                    result = true;

                    GoToReadyPosition();

                    Console.WriteLine("joint Inits complete: success = " + result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("**Joint Init Error** " + ex.Message);
                    result = false;
                }
            }
            IsReady = result;
            return result;
        }

        public void PrepareForShutdown()
        {
            StowArm();
        }

        public async void Test()
        {
            if (IsReady)
            {
                Console.WriteLine("Test Boogie");

                GoToReadyPosition();

                Stop = false;

                while (!Stop)
                {
                    Console.WriteLine("Test Boogie Loop");

                    _wrist.RandomStep();
                    _elbow.RandomStep();
                    _shoulder.RandomStep();
                    _pivot.RandomStep();

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }

        public void GoToReadyPosition()
        {
            if (IsReady)
            {
                _wrist.GoToPresetPosition(PresetPositions.ReadyPosition);
                _elbow.GoToPresetPosition(PresetPositions.ReadyPosition);
                _shoulder.GoToPresetPosition(PresetPositions.ReadyPosition);
                _pivot.GoToPresetPosition(PresetPositions.ReadyPosition);
            }
        }

        public void StowArm()
        {
            if (IsReady)
            {
                _wrist.GoToPresetPosition(PresetPositions.StowedPosition);
                _elbow.GoToPresetPosition(PresetPositions.StowedPosition);
                _shoulder.GoToPresetPosition(PresetPositions.StowedPosition);
                _pivot.GoToPresetPosition(PresetPositions.StowedPosition);
            }
        }

        private void rotate(int stepsToRotate)
        {
            if (IsReady)
            {
                _pivot.Step(stepsToRotate);
            }
        }

        public void Lean(int qty, bool keepLevel = true)
        {
            if (IsReady)
            {
                if (keepLevel)
                {

                }
                else
                {

                }
                _pivot.Step(qty);
            }
        }

        public void Turn(int qty, bool keepLevel = true)
        {
            if (IsReady)
            {
                if (keepLevel)
                {

                }
                else
                {

                }
                _pivot.Step(qty);
            }
        }

        public void Elevate(int qty, bool keepLevel = true)
        {
            if (IsReady)
            {
                if (keepLevel)
                {

                }
                else
                {

                }
                _pivot.Step(qty);
            }
        }

    }
}
