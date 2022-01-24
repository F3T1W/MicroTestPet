﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
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
        private InMemoryRandomAccessStream _memoryBuffer;
        public bool IsRecording { get; set; }
        private const string DEFAULT_AUDIO_FILENAME = "Pisechka.mp3";
        private string _fileName;

        public async void Record()
        {
            _memoryBuffer = new InMemoryRandomAccessStream();
            if (IsRecording)
            {
                StopRecording();
            }
            //await Initialize();
            //await DeleteExistingFile();
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

        public async void StopRecording()
        {
            await _mediaCapture.StopRecordAsync();
            IsRecording = false;
            SaveAudioToFile();
        }

        private async void SaveAudioToFile()
        {
            IRandomAccessStream audioStream = _memoryBuffer.CloneStream();
            StorageFolder storageFolder = ApplicationData.Current.LocalCacheFolder;
            StorageFile storageFile = await storageFolder.CreateFileAsync(
              DEFAULT_AUDIO_FILENAME, CreationCollisionOption.GenerateUniqueName);
            this._fileName = storageFile.Name;
            using (IRandomAccessStream fileStream =
              await storageFile.OpenAsync(FileAccessMode.ReadWrite))
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