# XBoxController
.NET library for quickly using XBox Controllers as input devices.

XBox Controllers make awesome input devices for Windows applications. Here's a library to make XBox Controller input simple and trivial, in about 1 minute.

To get started, install NuGet Package **XBoxController** (see https://www.nuget.org/packages/XBoxController for more info).

Code Samples:

**Get Connected XBox Controllers**

```cs
var connectedControllers = BrandonPotter.XBox.XBoxController.GetConnectedControllers();
```

**Receive events when controllers are connected or disconnected**

```cs
BrandonPotter.XBox.XBoxControllerWatcher watcher = new BrandonPotter.XBox.XBoxControllerWatcher();
watcher.ControllerConnected += (c) => { Console.WriteLine("Controller " + c.PlayerIndex.ToString() + " connected"); };
watcher.ControllerDisconnected += (c) => { Console.WriteLine("Controller " + c.PlayerIndex.ToString() + " disconnected"); };
```

**Find out if Button A is pressed on the first connected controller**

```cs
var isPressed = BrandonPotter.XBox.XBoxController.GetConnectedControllers().FirstOrDefault().ButtonAPressed;
```
