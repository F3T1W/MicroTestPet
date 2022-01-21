using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace MicroTestPet
{
    public class Microphone
    {


        MediaCapture mediaCapture;
        //LowLagMediaRecording _mediaRecording;
        InMemoryRandomAccessStream buffer;
        public bool recording;
        private string filename;
        private const string audioFile = "audio.m4a";

        public async Task<bool> Init()
        {
            if (buffer != null)
            {
                buffer.Dispose();
            }
            buffer = new InMemoryRandomAccessStream();
            if(mediaCapture != null)
            {
                mediaCapture.Dispose();
            }
            try
            {
                MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Audio
                };
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync(settings);
                mediaCapture.RecordLimitationExceeded += (MediaCapture sender) =>
                {
                    Stop();
                    throw new Exception("Exceed record limitation");
                };
                mediaCapture.Failed += (MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs) =>
                {
                    recording = false;
                    throw new Exception("Exceed recording");
                };
            }

            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(UnauthorizedAccessException))
                {
                    throw ex.InnerException;
                }
                throw;
            }
            return true;
        }

        public async void Record()
        {
            await Init();
            try
            {
                await mediaCapture.StartRecordToStreamAsync(MediaEncodingProfile.CreateM4a(AudioEncodingQuality.High), buffer);
            }
            catch(Exception ex)
            {
                MessageDialog messageDialog = new MessageDialog(ex.Message);
                await messageDialog.ShowAsync();
            }
            if(recording) throw new InvalidOperationException("Cant execute two records");
            recording = true;
        }

        public async void Stop()
        {
            await mediaCapture.StopRecordAsync();
            recording = false;
        }

        public async Task Play(CoreDispatcher dispatcher)
        {
            MediaElement playback = new MediaElement();
            IRandomAccessStream audio = buffer.CloneStream();
            if (audio == null) throw new ArgumentNullException("buffer");
            StorageFolder storageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            if (!string.IsNullOrEmpty(filename))
            {
                StorageFile original = await storageFolder.GetFileAsync(filename);
                await original.DeleteAsync();
            }
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                StorageFile storageFile = await storageFolder.CreateFileAsync(audioFile, CreationCollisionOption.GenerateUniqueName);
                filename = storageFile.Name;
                using (IRandomAccessStream fileStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await RandomAccessStream.CopyAndCloseAsync(audio.GetInputStreamAt(0), fileStream.GetOutputStreamAt(0));
                    await audio.FlushAsync();
                    audio.Dispose();
                };
                IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read);
                playback.SetSource(stream, storageFile.FileType);
                playback.Play();
            });
        }
    }
}
