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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Blade_Ball_Things
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
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
        bool IsRunning = false;
        DispatcherTimer asignvalue = new DispatcherTimer();

        public Main()
        {
            InitializeComponent();
            LoadSettings();
            asignvalue.Tick += Asignvalue_Tick;
            asignvalue.Interval = new TimeSpan(0, 0, 0, 0, 100);
            asignvalue.Start();
        }

        private void Asignvalue_Tick(object sender, EventArgs e)
        {
            SelectedValue.Content = Math.Floor(ValueSlider.Value);
            SaveSettings();

        }

        private void MoveApp(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeApp(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
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

        private void MakeAppTopmost(object sender, MouseButtonEventArgs e)
        {
            if (!Topmost)
            {
                Topmost = true;
                ((Storyboard)TryFindResource("TopmostOn")).Begin();
            }
            else
            {
                Topmost = false;
                ((Storyboard)TryFindResource("TopmostOff")).Begin();
            }
        }

        private async void StartAC(object sender, RoutedEventArgs e)
        {
            IsRunning = !IsRunning; 
            start_button.Content = IsRunning ? "Stop" : "Start";
            setStatus(!IsRunning ? "Stopped" : null, true);
            setStatus(!IsRunning ? "Start AC" : null );

            IntPtr hWnd = FindWindow(null, "Roblox");

            while (true)
            {
                if (!IsRunning) break;

                bool isKeyPressed = GetAsyncKeyState(KeyInterop.VirtualKeyFromKey(key)) != 0;
                IntPtr foregroundWindow = GetForegroundWindow();


                setStatus(foregroundWindow != hWnd ? "Go to Roblox" : "Waiting key");

                setStatus(foregroundWindow != hWnd ? "Started" : isKeyPressed ? "Clicking" : "idle", true);
                setStatus(foregroundWindow != hWnd ? "Open Roblox" : isKeyPressed ? "Enjoy" : "Waiting key");

                if (foregroundWindow == hWnd && isKeyPressed)
                {
                    SimulateMouseClick();
                    Thread.Sleep((int)(1000.0 / ValueSlider.Value));
                }

                await Task.Delay(10);
            }
        }
        static void SimulateMouseClick()
        {
            mouse_event(0x00000002, 0, 0, 0, 0);
            mouse_event(0x00000004, 0, 0, 0, 0);
        }
        private void setStatus(string input, [Optional] bool indication)
        {
            if (!indication) Indication.Content ="Indication: " + input;
            else Status.Content = $"Status: {input}";
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.Keybind = key.ToString();
            Properties.Settings.Default.CPSValue = ValueSlider.Value;
            Properties.Settings.Default.Topmost = Topmost;
            Properties.Settings.Default.Save();
        }

        private void LoadSettings()
        {
            KeyConverter k = new KeyConverter();
            Keybind.Text = Properties.Settings.Default.Keybind;
            key = (Key)k.ConvertFromString(Properties.Settings.Default.Keybind);
            ValueSlider.Value = Properties.Settings.Default.CPSValue;
            Topmost = Properties.Settings.Default.Topmost;
            if(Topmost)
            {
                ((Storyboard)TryFindResource("TopmostOn")).Begin();
            }
           
        }

        private void AssignSliderValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
