using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PreferencePane
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly string _settingsPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "resources", "SleekySnip.props");
        private SleekySnip.Core.SleekySnipSettings _settings;

        public MainWindow()
        {
            InitializeComponent();
            _settings = SleekySnip.Core.SleekySnipSettingsSerializer.Load(_settingsPath);
            OutputFolderTextBox.Text = _settings.OutputFolder;
        }

        private async void BrowseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FolderPicker();
            picker.FileTypeFilter.Add("*");
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                _settings.OutputFolder = folder.Path;
                OutputFolderTextBox.Text = _settings.OutputFolder;
                SleekySnip.Core.SleekySnipSettingsSerializer.Save(_settings, _settingsPath);
            }
        }
    }
}
