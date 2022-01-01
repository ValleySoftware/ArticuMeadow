using ArticuMeadow.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArticuMeadow.ControlInterfaces
{
    public class ArmControl
    {

        private BaseJoint _pivot;
        private BaseJoint _shoulder;
        private BaseJoint _elbow;
        private BaseJoint _wrist;

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

            try
            {
                result = InitJoints();
            }
            catch (Exception ex)
            {
                result = false;
            }

            IsReady = result;
            return result;
        }

        private bool InitJoints()
        {
            var result = false;

            try
            {
                _pivot = new BaseJoint();
                var pivotInfo = new JointInfoPacket()
                {
                    JointDirection = TravelDirection.Rotation,
                    Name = "Pivot",
                    PinOne = MeadowApp.Device.Pins.D15,
                    PinTwo = MeadowApp.Device.Pins.D14,
                    PinThree = MeadowApp.Device.Pins.D13,
                    PinFour = MeadowApp.Device.Pins.D12
                };
                _pivot.Init(pivotInfo);

                _shoulder = new BaseJoint();
                var shoulderInfo = new JointInfoPacket()
                {
                    JointDirection = TravelDirection.Elevation,
                    Name = "Shoulder",
                    PinOne = MeadowApp.Device.Pins.D15,
                    PinTwo = MeadowApp.Device.Pins.D14,
                    PinThree = MeadowApp.Device.Pins.D13,
                    PinFour = MeadowApp.Device.Pins.D12
                };
                _shoulder.Init(shoulderInfo);

                _elbow = new BaseJoint();
                var elbowInfo = new JointInfoPacket()
                {
                    JointDirection = TravelDirection.Elevation,
                    Name = "Elbow",
                    PinOne = MeadowApp.Device.Pins.D11,
                    PinTwo = MeadowApp.Device.Pins.D10,
                    PinThree = MeadowApp.Device.Pins.D09,
                    PinFour = MeadowApp.Device.Pins.D08
                };
                _elbow.Init(elbowInfo);

                _wrist = new BaseJoint();
                var wristInfo = new JointInfoPacket()
                {
                    JointDirection = TravelDirection.Elevation,
                    Name = "Wrist",
                    PinOne = MeadowApp.Device.Pins.D07,
                    PinTwo = MeadowApp.Device.Pins.D06,
                    PinThree = MeadowApp.Device.Pins.D05,
                    PinFour = MeadowApp.Device.Pins.D04
                };
                _wrist.Init(wristInfo);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }

            IsReady = result;
            return result;
        }

        public void PrepareForShutdown()
        {
            StowArm();
        }

        public void TestDance()
        {
            if (IsReady)
            {
                GoToReadyPosition();

                Stop = false;
                while (!Stop)
                {

                }
            }
        }

        private void GoToReadyPosition()
        {
            if (IsReady)
            {
                _pivot.GoToReadyPosition();
                _shoulder.GoToReadyPosition();
                _elbow.GoToReadyPosition();
                _wrist.GoToReadyPosition();
            }
        }

        private void StowArm()
        {
            if (IsReady)
            {
                _pivot.GoToStowedPosition();
                _shoulder.GoToStowedPosition();
                _elbow.GoToStowedPosition();
                _wrist.GoToStowedPosition();
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

        public void Point(int qty)
        {
            if (IsReady)
            {
                _pivot.Step(qty);
            }
        }

    }
}
