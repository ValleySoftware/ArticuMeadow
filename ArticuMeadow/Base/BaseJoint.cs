using Meadow.Devices;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Motors.Stepper;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArticuMeadow.Base
{
    public enum TravelDirection { Elevation, Rotation, Depth }
    public enum PresetPositions { StowedPosition, ReadyPosition, MinimumPosition, MaximumPosition, CentrePosition }

    public class JointInfoPacket
    {
        public string Name { get; set; }
        public IDigitalOutputPort PinOne { get; set; }
        public IDigitalOutputPort PinTwo { get; set; }
        public IDigitalOutputPort PinThree { get; set; }
        public IDigitalOutputPort PinFour { get; set; }
        public TravelDirection JointDirection { get; set; }
        public PushButton PositiveStopSwitch { get; set; }
        public PushButton NegativeStopSwitch { get; set; }
        public Mcp23x08 ioExpander { get; set; }
        public int StowedPosition { get; set; }
        public int ReadyPosition { get; set; }
    }

    public class BaseJoint
    {
        private JointInfoPacket _informationPacket;
        private Uln2003 _stepper;
        private bool _isReady;
        private bool _useStopSensors = true;
        private int _currentStepPosition = -1;
        private int _maxStepPosition = -1;

        public BaseJoint()
        {

        }

        public bool IsReady
        {
            get => _isReady;
            set => _isReady = value;
        }

        public bool Init(JointInfoPacket info, bool goToReady = false)
        {
            var result = false;

            if (info == null)
            {
                return false;
            }

            if (
                string.IsNullOrEmpty(info.Name) ||
                info.PinOne == null ||
                info.PinTwo == null ||
                info.PinThree == null || 
                info.PinFour == null
                )
            {
                return false;
            }

            if (info.PositiveStopSwitch == null ||
                info.NegativeStopSwitch == null)
            {
                _useStopSensors = false;
                Console.WriteLine("No stop sensors defined for " + info.Name + ". Auto stop will be disabled.");
            }
            else
            {
                _useStopSensors = true;
                info.PositiveStopSwitch.Clicked += PositiveStopSwitch_Clicked;
                info.NegativeStopSwitch.Clicked += NegativeStopSwitch_Clicked; 
            }

            try
            {
                _informationPacket = info;

                _stepper = new Uln2003(
                    _informationPacket.ioExpander,
                     _informationPacket.PinOne.Pin,
                     _informationPacket.PinTwo.Pin,
                     _informationPacket.PinThree.Pin,
                     _informationPacket.PinFour.Pin);

                _stepper.Mode = 
                    Uln2003.StepperMode.HalfStep;

                _stepper.AngularVelocity = 
                    new Meadow.Units.AngularVelocity(
                        50, 
                        Meadow.Units.AngularVelocity.UnitType.RevolutionsPerSecond);

                if (goToReady)
                {
                    GoToPresetPosition(PresetPositions.ReadyPosition);
                }

                result = true;

            }
            catch (Exception smEx)
            {
                Console.WriteLine(info.Name + " init faied " + smEx.Message);

                _stepper = null;
                _informationPacket = null;
            }

            IsReady = result;
            return result;
        }

        private void NegativeStopSwitch_Clicked(object sender, EventArgs e)
        {
            Stop();
            _currentStepPosition = 0;
        }

        private void PositiveStopSwitch_Clicked(object sender, EventArgs e)
        {
            Stop();
            _maxStepPosition = _currentStepPosition;
        }

        public void LocateMinAndMaxPositions()
        {
            if (IsReady && 
                _useStopSensors &&
                _informationPacket.NegativeStopSwitch != null &&
                _informationPacket.PositiveStopSwitch != null )
            {
                var stop = false;
                int increment = -10;

                while (!stop)
                {
                    _stepper.Step(increment);

                    //Check both in case stepper is wired backwards.
                    if (_informationPacket.NegativeStopSwitch.State ||
                        _informationPacket.PositiveStopSwitch.State)
                    {
                        stop = true;
                        break;
                    }
                }

                stop = false;
                increment = 10;

                while (!stop)
                {
                    Step(increment);

                    //Check both in case stepper is wired backwards.
                    if (_informationPacket.NegativeStopSwitch.State ||
                        _informationPacket.PositiveStopSwitch.State)
                    {
                        stop = true;
                        break;
                    }
                }

                GoToPresetPosition(PresetPositions.CentrePosition);
            }
        }

        public void GoToPresetPosition(PresetPositions preset)
        {

            if (IsReady &&
                _useStopSensors)
            {
                int moveTo = -1;

                if (_maxStepPosition < 0)
                {
                    LocateMinAndMaxPositions();
                }

                switch (preset)
                {
                    case PresetPositions.StowedPosition: moveTo = _informationPacket.StowedPosition; break;
                    case PresetPositions.ReadyPosition: moveTo = _informationPacket.ReadyPosition; break;
                    case PresetPositions.MinimumPosition: moveTo = 0; break;
                    case PresetPositions.MaximumPosition: moveTo = _maxStepPosition; break;
                    case PresetPositions.CentrePosition: moveTo = CalculateCentrePosition(); break;
                    default: moveTo = _currentStepPosition; break;
                }

                if (moveTo < 0)
                {
                    moveTo = CalculateCentrePosition();
                }

                if (moveTo > -1 &&
                    _currentStepPosition > -1 &&
                    _currentStepPosition < _maxStepPosition)
                {
                    int moveBy = moveTo - _currentStepPosition;

                    Step(moveBy);
                }
            }
        }

        private int CalculateCentrePosition()
        {
            if (_maxStepPosition > 0)
            {
                double halfway = (double)_maxStepPosition / (double)2;
                return Convert.ToInt32(Math.Round(halfway) * -1);
            }
            else
            {
                return -1;
            }
        }

        public void Step(int noOfSteps)
        {
            if (IsReady)
            {
                _stepper.Step(noOfSteps);
                _currentStepPosition = _currentStepPosition + noOfSteps;
            }
        }

        public void Stop()
        {
            if (_stepper != null)
            {
                _stepper.Stop();
            }
        }
        
        public void RandomStep(int qty = 0)
        {
            if (qty == 0)
            {
                Random r = new Random();
                qty = r.Next(200, 800);
                    
                if (r.Next(0,2) == 1)
                {
                    qty = qty * - 1;
                }

            }

            Console.WriteLine(_informationPacket.Name + " Test Dance step by " + qty);
            _stepper.Step(qty);

        }
    }
}
