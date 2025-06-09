# llsvc - low level services

This folder contains background components for SleekySnip.

## Capture Service (`llsvc.capture`)

The capture service registers `Ctrl+Shift+S` as a global hotkey. When pressed,
it opens a small WPF window with options to capture an area, window or the full
screen. The current window is a placeholder for future capture logic.
