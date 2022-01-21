using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace MicroTestPet
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        readonly Microphone micro = new Microphone();
        //private DeviceInformation current;
        //private DeviceInformationCollection devices;

        public MainPage()
        {
            this.InitializeComponent();
            //Record.IsEnabled = false;
            //_ = FindDevicesAsync();
            Record.Click += Record_Click;
            Play.Click += Play_Click;
        }

        /*private void ListMic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListMic.SelectedIndex == -1)
            {
                Record.IsEnabled = false;
                return;
            }
            current = devices[ListMic.SelectedIndex];
            Record.IsEnabled = true;
        }

        public async Task FindDevicesAsync()
        {
            // Finds all video capture devices

            devices = await DeviceUtils.GetListAudioCaptureDeviceAsync();
            foreach (DeviceInformation dev in devices)
            {
                ListMic.Items.Add(dev.Name);
            }
        }*/

        private void Record_Click(object sender, RoutedEventArgs e)
        {
            if (micro.recording)
            {
                micro.Stop();
            }
            else
            {
                micro.Record();
            }
        }

        private async void Play_Click(object sende, RoutedEventArgs e)
        {
            await micro.Play(Dispatcher);
        }
    }
}
