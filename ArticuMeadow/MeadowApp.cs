using ArticuMeadow.ControlInterfaces;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using System;
using System.Threading;

namespace ArticuMeadow
{
    // Change F7MicroV2 to F7Micro for V1.x boards
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        private ArmControl arm;
        private readonly bool doTest = true;

        public MeadowApp()
        {
            Initialize();
            
        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");
            arm = new ArmControl();
            arm.Init();
            Console.WriteLine("Init Complete");
            if (doTest)
            {
                arm.Test();
            }
        }

    }
}
