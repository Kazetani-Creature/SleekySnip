# SleekySnip

SleekySnip is a collection of small projects that together provide a simple screenshot utility.

## Solution Layout

- `src/SleekySnip.Core` – .NET 8 class library containing shared logic and settings handling.
- `src/interfaces/PreferencePane` – WinUI 3 application that exposes configuration options.
- `src/services` – capture service and other low‑level components.
- `resources` – default configuration and other shared assets.

## Prerequisites

- **Windows** 10 or later.
- **.NET 8 SDK** – required to build all projects.
- **Windows App SDK** – required for the WinUI based preference pane.

Ensure that the Windows App SDK is installed via Visual Studio Installer or by adding the `Microsoft.WindowsAppSDK` workload.

## Building

Restore dependencies and build the entire solution:

```bash
dotnet restore

dotnet build
```

## Running

Each project can be started directly from the command line. The capture service runs first, then the preference pane:

```bash
# Start the capture service (replace <CaptureService.csproj> with the actual path once implemented)
dotnet run --project src/services/<CaptureService.csproj>

# Launch the preference pane UI
 dotnet run --project src/interfaces/PreferencePane/PreferencePane.csproj
```

Opening `SleekySnip.sln` in Visual Studio provides the same start options through the IDE.
