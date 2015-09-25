using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandonPotter.XBox.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            XBoxControllerWatcher xbcw = new XBoxControllerWatcher();
            xbcw.ControllerConnected += OnControllerConnected;
            xbcw.ControllerDisconnected += OnControllerDisconnected;

            Console.WriteLine("Press any key to exit.");

            while (!Console.KeyAvailable)
            {
                System.Threading.Thread.Sleep(1000);
                foreach (var c in XBoxController.GetConnectedControllers())
                {
                    Console.WriteLine("Controller " + c.PlayerIndex.ToString() + " Thumb Left X = " + c.ThumbLeftX.ToString() + ", Y = " + c.ThumbLeftY.ToString() + ", A = " + c.ButtonAPressed.ToString());
                }
            }
        }

        private static void OnControllerDisconnected(XBoxController controller)
        {
            Console.WriteLine("Controller Disconnected: Player " + controller.PlayerIndex.ToString());
        }

        private static void OnControllerConnected(XBoxController controller)
        {
            Console.WriteLine("Controller Connected: Player " + controller.PlayerIndex.ToString());
        }
    }
}
 