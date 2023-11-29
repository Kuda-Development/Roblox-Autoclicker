using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Blade_Ball_Things
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        DispatcherTimer asignvalue = new DispatcherTimer();        
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
            asignvalue.Tick += Asignvalue_Tick;
            asignvalue.Interval = new TimeSpan(0, 0, 0, 0, 1);
            asignvalue.Start();
        }

        private void Asignvalue_Tick(object sender, EventArgs e)
        {
           ValueSelected.Content = Math.Floor(ClicksPerSecond.Value);
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
            setStatus("Off", true);
            setStatus("");
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            stopped = false;

            IntPtr hWnd = FindWindow(null, "Roblox");

            while (true)
            {
                if (stopped) break;

                bool isKeyPressed = GetAsyncKeyState(KeyInterop.VirtualKeyFromKey(key)) != 0;
                IntPtr foregroundWindow = GetForegroundWindow();


                setStatus(foregroundWindow != hWnd ? "Go to RobloxPlayerBeta" : $"Press {key} to start clicking");
                setStatus(foregroundWindow != hWnd ? "Waiting Roblox" : isKeyPressed ? "Clicking" : "Waiting", true);

                if (foregroundWindow == hWnd && isKeyPressed)
                {

                    SimulateMouseClick();
                    Thread.Sleep((int)(1000.0 / ClicksPerSecond.Value));
                }

                await Task.Delay(10);
            }
        }

        static void SimulateMouseClick()
        {
            mouse_event(0x00000002, 0, 0, 0, 0);
            mouse_event(0x00000004, 0, 0, 0, 0);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
