using Meadow.Foundation.Motors.Stepper;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArticuMeadow.Base
{
    public enum TravelDirection { Elevation, Rotation, Depth }

    public class JointInfoPacket
    {
        public string Name { get; set; }
        public IPin PinOne { get; set; }
        public IPin PinTwo { get; set; }
        public IPin PinThree { get; set; }
        public IPin PinFour { get; set; }
        public TravelDirection JointDirection { get; set; }
    }

    public class BaseJoint
    {
        private JointInfoPacket _informationPacket;
        private Uln2003 _stepper;
        private bool _isReady;

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

            try
            {
                _informationPacket = info;

                _stepper = new Uln2003(MeadowApp.Device, info.PinOne, info.PinTwo, info.PinThree, info.PinFour);

                GoToReadyPosition();

                result = true;

            }
            catch (Exception) 
            {
                _stepper = null;
                _informationPacket = null;
            }

            IsReady = result;
            return result;
        }

        public void GoToReadyPosition()
        {
            if (IsReady)
            {

            }
        }

        public void GoToStowedPosition()
        {
            if (IsReady)
            {

            }
        }

        public void Step(int noOfSteps)
        {
            if (IsReady)
            {
                _stepper.Step(noOfSteps);
            }
        }

        public void Stop()
        {
            if (IsReady)
            {
                _stepper.Stop();
            }
        }
    }
}
