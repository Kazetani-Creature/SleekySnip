# SleekySnip

SleekySnip is a simple example project that targets Windows 10 and later using .NET 8 and WinUI.

The repository includes a low level keyboard listener service (`llsvc.keyboardlistener`) used for capturing global hotkeys.  The preference pane starts this listener at launch and lets you configure hotkeys for screen, window, and region capture.

## Prerequisites

- **Windows 10 or later** – this project requires the Windows 10 SDK and features available on Windows 10 and newer.
- **Windows App SDK** – required for WinUI 3 development. You can install it from Visual Studio's installer or by downloading the standalone installer. See [Set up your development environment](https://learn.microsoft.com/windows/apps/windows-app-sdk/set-up-your-development-environment) for instructions.

Once these prerequisites are installed, open `SleekySnip.sln` in Visual Studio 2022 and build the solution.

