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
        AudioRecorder _audioRecorder;

        public MainPage()
        {
            this.InitializeComponent();
            this._audioRecorder = new AudioRecorder();
        }

        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {
            this._audioRecorder.Record();
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            this._audioRecorder.Play();
        }
    }
}
