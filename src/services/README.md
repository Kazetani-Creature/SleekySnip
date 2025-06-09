# llsvc - low level services

This folder contains background components for SleekySnip.

## Capture Service (`llsvc.capture`)

The capture service provides several global shortcuts for taking screenshots on Windows:

| Shortcut | Mode |
| --- | --- |
| `Alt+Shift+Num4` | Capture the active window with optional DWM shadow |
| `Alt+Shift+Num1` | Capture the entire screen (all monitors) |
| `Alt+Shift+3` | Capture a user selected area |
| `Alt+Shift+Num5` | Open a flyout to choose a capture mode |

Captured images are placed on the clipboard and optionally written to a user defined path from the preference pane.
Actual capture implementation is based on Windows APIs and the AeroShotCRE project but is stubbed in this repo.
