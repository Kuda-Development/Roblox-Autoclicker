using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Autoclicker
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(
            int dwFlags,
            int dx,
            int dy,
            int dwData,
            int dwExtraInfo
        );

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private Key _key = Key.E;
        private bool _isRunning;
        private readonly DispatcherTimer _assignedValue = new DispatcherTimer();
        private bool textBoxClicked = false;

        public Main()
        {
            InitializeComponent();
            LoadSettings();
            _assignedValue.Tick += AssignedValueTick;
            _assignedValue.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _assignedValue.Start();
        }

        private void AssignedValueTick(object sender, EventArgs e)
        {
            SelectedValue.Content = Math.Floor(ValueSlider.Value);
            SaveSettings();
        }

        private void MoveApp(object sender, MouseButtonEventArgs e) => DragMove();

        private void CloseApp(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void MinimizeApp(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void kebind_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textbox = (TextBox)sender;

            if (!textbox.IsKeyboardFocusWithin)
            {
                textbox.Clear();
                textbox.Focus();
            }

            textBoxClicked = true;
        }

        private void kebind_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textbox = (TextBox)sender;

            if (!textBoxClicked || !char.IsLetter(e.Key.ToString(), 0))
                return;

            var newText = e.Key.ToString();
            Enum.TryParse(newText, out _key);
            textbox.Text = newText;
            e.Handled = true;

            textBoxClicked = false;
        }

        private void MakeAppTopmost(object sender, MouseButtonEventArgs e)
        {
            if (Topmost)
            {
                Topmost = false;
            }
            else
            {
                Topmost = true;
            }
        }

        private async void StartAc(object sender, RoutedEventArgs e)
        {
            _isRunning = !_isRunning;
            StartButton.Content = _isRunning ? "Stop" : "Start";
            SetStatus(!_isRunning ? "Stopped" : null, true);
            SetStatus(!_isRunning ? "Start AC" : null);

            var hWnd = FindWindow(null, "Roblox");

            while (true)
            {
                if (!_isRunning)
                    break;

                var isKeyPressed = GetAsyncKeyState(KeyInterop.VirtualKeyFromKey(_key)) != 0;
                var foregroundWindow = GetForegroundWindow();

                SetStatus(foregroundWindow != hWnd ? "Go to Roblox" : "Waiting key");

                SetStatus(
                    foregroundWindow != hWnd
                        ? "Started"
                        : isKeyPressed
                            ? "Clicking"
                            : "idle",
                    true
                );
                SetStatus(
                    foregroundWindow != hWnd
                        ? "Open Roblox"
                        : isKeyPressed
                            ? "Enjoy"
                            : "Waiting key"
                );
                if (foregroundWindow == hWnd)
                {
                    ((Storyboard)TryFindResource("UnFocus")).Begin();
                    Opacity = 0.5;
                }
                else
                {
                    ((Storyboard)TryFindResource("OnFocus")).Begin();

                    Opacity = 1;
                }
                if (foregroundWindow == hWnd && isKeyPressed)
                {
                    SimulateMouseClick();
                    Thread.Sleep((int)(1000.0 / ValueSlider.Value));
                }

                await Task.Delay(10);
            }
        }

        private static void SimulateMouseClick()
        {
            mouse_event(0x00000002, 0, 0, 0, 0);
            mouse_event(0x00000004, 0, 0, 0, 0);
        }

        private void SetStatus(string input, [Optional] bool indication)
        {
            if (!indication)
                Indication.Content = "Indication: " + input;
            else
                Status.Content = $"Status: {input}";
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.Keybind = _key.ToString();
            Properties.Settings.Default.CPSValue = ValueSlider.Value;
            Properties.Settings.Default.Topmost = Topmost;
            Properties.Settings.Default.Save();
        }

        private void LoadSettings()
        {
            var k = new KeyConverter();
            Keybind.Text = Properties.Settings.Default.Keybind;
            _key = (Key)k.ConvertFromString(Properties.Settings.Default.Keybind);
            ValueSlider.Value = Properties.Settings.Default.CPSValue;
            Topmost = Properties.Settings.Default.Topmost;
            topmost_checkbox.IsChecked = Topmost;
        }

        private void ToggleTopmost(object sender, RoutedEventArgs e)
        {
            var checkbox = (CheckBox)sender;
            Topmost = checkbox.IsChecked == true ? true : false;
        }
    }
}
