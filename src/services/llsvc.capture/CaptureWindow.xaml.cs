using System.Windows;

namespace LLSvc.Capture
{
    public partial class CaptureWindow : Window
    {
        public CaptureWindow()
        {
            InitializeComponent();
        }

        private void Window_Click(object sender, RoutedEventArgs e)
        {
            CaptureManager.CaptureWindow(includeShadow: true);
            Close();
        }

        private void Screen_Click(object sender, RoutedEventArgs e)
        {
            CaptureManager.CaptureFullScreen();
            Close();
        }

        private void Area_Click(object sender, RoutedEventArgs e)
        {
            CaptureManager.CaptureArea(new Rect());
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
