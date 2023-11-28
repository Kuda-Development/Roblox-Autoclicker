using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Blade_Ball_Things
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        Key key = Key.E;
        bool stopped = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void kebind_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textbox = (TextBox)sender;

            if (!textbox.IsKeyboardFocusWithin)
            {
                textbox.Clear();
                textbox.Focus();
            }
        }

        private void kebind_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textbox = (TextBox)sender;

            if (char.IsLetter(e.Key.ToString(), 0))
            {
                var newText = e.Key.ToString();
                Enum.TryParse(newText, out key);
                textbox.Text = newText;

                e.Handled = true;
            }
        }

        private void setStatus(string input, [Optional]bool indication)
        {
            if (!indication) text.Content = input;
            else status.Content = $"Status: {input}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            stopped = true;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IntPtr hWnd = FindWindow(null, "Roblox");

            while (true)
            {
                if (stopped) break;

                bool isKeyPressed = GetAsyncKeyState(int.Parse($"0x{(int)key}")) != 0; // ESTA PUTA COJONERIA NO SIRVE
                IntPtr foregroundWindow = GetForegroundWindow();


                setStatus(foregroundWindow != hWnd ? "Go to RobloxPlayerBeta" : $"Press {key} to start clicking");
                setStatus(foregroundWindow != hWnd ? "Waiting Roblox" : isKeyPressed ? "Clicking" : "Waiting", true);

                if (foregroundWindow == hWnd && isKeyPressed)
                {
                    SimulateMouseClick();
                    //await Task.Delay((int)(1000.0 / ClicksPerSecond.Value));
                }

                await Task.Delay(10);
            }
        }

        static void SimulateMouseClick()
        {
            mouse_event(0x00000002, 0, 0, 0, 0);
            mouse_event(0x00000004, 0, 0, 0, 0);
        }
    }
}
