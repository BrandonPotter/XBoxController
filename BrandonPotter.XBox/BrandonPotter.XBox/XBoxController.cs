using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrandonPotter.XBox
{
    public class XBoxController
    {
        private SharpDX.XInput.Controller dxC;
        private SharpDX.XInput.State lastGamepadState = new SharpDX.XInput.State();
        private int lastLeftMotorRawValue;
        private long lastMsRefreshed;
        private int lastRightMotorRawValue;
        private int pIndex;
        private long ticksPerMs = TimeSpan.TicksPerMillisecond;
        private bool connected = false;

        private static Dictionary<int, XBoxController> _controllers = null;
        public static IEnumerable<XBoxController> GetConnectedControllers()
        {
            InitControllers();

            return _controllers.Values.Where(c => c.IsConnected).ToList();
        }

        internal static IEnumerable<XBoxController> GetAllControllers()
        {
            InitControllers();

            return _controllers.Values.ToList();
        }

        private static void InitControllers()
        {
            if (_controllers == null)
            {
                _controllers = new Dictionary<int, XBoxController>();

                _controllers[0] = new XBoxController(new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.One));
                _controllers[1] = new XBoxController(new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.Two));
                _controllers[2] = new XBoxController(new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.Three));
                _controllers[3] = new XBoxController(new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.Four));
            }
        }

        internal XBoxController(SharpDX.XInput.Controller dxController)
        {
            this.dxC = dxController;
            this.pIndex = (int)this.dxC.UserIndex;
            this.SnapDeadZoneCenter = 50.0;
            this.SnapDeadZoneTolerance = 10.0;
            this.TriggerLeftPressThreshold = 10.0;
            this.TriggerRightPressThreshold = 10.0;
            this.RefreshIntervalMilliseconds = 30;
        }

        private void EnsureRefresh()
        {
            long num = Environment.TickCount * this.ticksPerMs;

            if (!connected)
            {
                if ((num - this.lastMsRefreshed) < 1000)
                {
                    return;
                }
            }
            
            if ((num - this.lastMsRefreshed) > this.RefreshIntervalMilliseconds)
            {
                this.RefreshControllerState();
            }
        }

        private double GetPercentageValue(double min, double max, double value)
        {
            if (value > max)
            {
                return 100.0;
            }
            double num = max - min;
            if (num < 0.0)
            {
                return 0.0;
            }
            double num2 = 0.0 - min;
            double num3 = max + num2;
            double num4 = value + num2;
            if (num4 == 0.0)
            {
                return 0.0;
            }
            return ((num4 / num3) * 100.0);
        }

        private void RefreshControllerState()
        {
            this.lastMsRefreshed = Environment.TickCount * this.ticksPerMs;

            try
            {
                var state = this.dxC.GetState();
                this.lastGamepadState = state;
                connected = true;
            } catch (SharpDX.SharpDXException ex)
            {
                if (ex.Message.ToUpper().Contains("THE DEVICE IS NOT CONNECTED"))
                {
                    connected = false;
                }
            }
        }

        private double RoundToSnap(double value)
        {
            double num = this.SnapDeadZoneCenter - this.SnapDeadZoneTolerance;
            double num2 = this.SnapDeadZoneCenter + this.SnapDeadZoneTolerance;
            if ((value >= num) && (value <= num2))
            {
                return this.SnapDeadZoneCenter;
            }
            return value;
        }

        public void SetLeftMotorVibrationSpeed(double percentageSpeed)
        {
            int num = Convert.ToInt32((double)(65000.0 * (percentageSpeed / 100.0)));
            this.lastLeftMotorRawValue = num;
            this.SetMotorSpeeds();
        }

        private void SetMotorSpeeds()
        {
            this.dxC.SetVibration(new SharpDX.XInput.Vibration() { LeftMotorSpeed = Convert.ToUInt16(this.lastLeftMotorRawValue), RightMotorSpeed = Convert.ToUInt16(this.lastRightMotorRawValue) });
        }

        public void SetRightMotorVibrationSpeed(double percentageSpeed)
        {
            int num = Convert.ToInt32((double)(65000.0 * (percentageSpeed / 100.0)));
            this.lastRightMotorRawValue = num;
            this.SetMotorSpeeds();
        }

        public override string ToString()
        {
            int num = this.pIndex + 1;
            return ("XBox Controller " + num.ToString());
        }

        // Properties
        public bool ButtonAPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.A);
            }
        }

        public bool ButtonBackPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.Back);
            }
        }

        public bool ButtonBPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.B);
            }
        }

        public bool ButtonDownPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.DPadDown);
            }
        }

        public bool ButtonLeftPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.DPadLeft);
            }
        }

        public bool ButtonRightPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.DPadRight);
            }
        }

        public bool ButtonShoulderLeftPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.LeftShoulder);
            }
        }

        public bool ButtonShoulderRightPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.RightShoulder);
            }
        }

        public bool ButtonStartPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.Start);
            }
        }

        public bool ButtonUpPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.DPadUp);
            }
        }

        public bool ButtonXPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.X);
            }
        }

        public bool ButtonYPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.Y);
            }
        }

        public bool IsConnected
        {
            get
            {
                this.EnsureRefresh();
                return this.connected;
            }
        }

        public double LeftMotorVibrationSpeed
        {
            get
            {
                this.EnsureRefresh();
                return this.GetPercentageValue(0.0, 65000.0, (double)this.lastLeftMotorRawValue);
            }
        }

        public int PlayerIndex
        {
            get
            {
                return this.pIndex;
            }
        }

        public int RefreshIntervalMilliseconds { get; set; }

        public double RightMotorVibrationSpeed
        {
            get
            {
                this.EnsureRefresh();
                return this.GetPercentageValue(0.0, 65000.0, (double)this.lastRightMotorRawValue);
            }
        }

        public double SnapDeadZoneCenter { get; set; }

        public double SnapDeadZoneTolerance { get; set; }

        public double ThumbLeftX
        {
            get
            {
                this.EnsureRefresh();
                return this.RoundToSnap(this.GetPercentageValue(-32768.0, 32767.0, (double)this.lastGamepadState.Gamepad.LeftThumbX));
            }
        }

        public double ThumbLeftY
        {
            get
            {
                this.EnsureRefresh();
                return this.RoundToSnap(this.GetPercentageValue(-32768.0, 32767.0, (double)this.lastGamepadState.Gamepad.LeftThumbY));
            }
        }

        public bool ThumbpadLeftPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.LeftThumb);
            }
        }

        public bool ThumbpadRightPressed
        {
            get
            {
                this.EnsureRefresh();
                return lastGamepadState.Gamepad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.RightThumb);
            }
        }

        public double ThumbRightX
        {
            get
            {
                this.EnsureRefresh();
                return this.RoundToSnap(this.GetPercentageValue(-32768.0, 32767.0, (double)this.lastGamepadState.Gamepad.RightThumbX));
            }
        }

        public double ThumbRightY
        {
            get
            {
                this.EnsureRefresh();
                return this.RoundToSnap(this.GetPercentageValue(-32768.0, 32767.0, (double)this.lastGamepadState.Gamepad.RightThumbY));
            }
        }

        public double TriggerLeftPosition
        {
            get
            {
                this.EnsureRefresh();
                return this.GetPercentageValue(0.0, 255.0, (double)this.lastGamepadState.Gamepad.LeftTrigger);
            }
        }

        public bool TriggerLeftPressed
        {
            get
            {
                this.EnsureRefresh();
                return (this.TriggerLeftPosition > this.TriggerLeftPressThreshold);
            }
        }

        public double TriggerLeftPressThreshold { get; set; }

        public double TriggerRightPosition
        {
            get
            {
                this.EnsureRefresh();
                return this.GetPercentageValue(0.0, 255.0, (double)this.lastGamepadState.Gamepad.RightTrigger);
            }
        }

        public bool TriggerRightPressed
        {
            get
            {
                this.EnsureRefresh();
                return (this.TriggerRightPosition > this.TriggerRightPressThreshold);
            }
        }

        public double TriggerRightPressThreshold { get; set; }
    }
}
