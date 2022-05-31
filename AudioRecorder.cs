using System;
using System.Collections.Generic;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace MicroTestPet
{
    internal class AudioRecorder
    {
        private MediaCapture _mediaCapture;
        private readonly InMemoryRandomAccessStream _memoryBuffer;
        public bool IsRecording { get; set; }

        public AudioRecorder()
        {
            IsRecording = false;
            _memoryBuffer = new InMemoryRandomAccessStream();
        }

        public async void Record()
        {
            if (IsRecording)
            {
                StopRecording();
                await _memoryBuffer.FlushAsync();
            }
            else
            {
                MediaCaptureInitializationSettings settings =
                new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Audio
                };
                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync(settings);
                await _mediaCapture.StartRecordToStreamAsync(
                  MediaEncodingProfile.CreateMp3(AudioEncodingQuality.High), _memoryBuffer);
                IsRecording = true;
            }
        }

        public async void StopRecording()
        {
            await _mediaCapture.StopRecordAsync();
            IsRecording = false;
            SaveAudioToFile();
        }

        private async void SaveAudioToFile()
        {
            IRandomAccessStream audioStream = _memoryBuffer.CloneStream();
            Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop
            };
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".mp3" });
            savePicker.SuggestedFileName = "Pisechka";
            StorageFile file = await savePicker.PickSaveFileAsync();

            using (IRandomAccessStream fileStream =
            await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await RandomAccessStream.CopyAndCloseAsync(
                audioStream.GetInputStreamAt(0), fileStream.GetOutputStreamAt(0));
                await audioStream.FlushAsync();
                audioStream.Dispose();
            }
        }

        public void Play()
        {
            MediaElement playbackMediaElement = new MediaElement();
            playbackMediaElement.SetSource(_memoryBuffer, "MP3");
            playbackMediaElement.Play();
        }
    }
}
