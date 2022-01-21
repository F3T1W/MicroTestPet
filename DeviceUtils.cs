using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture.Frames;

namespace MicroTestPet
{
    internal class DeviceUtils
    {
        //Find all VideoCapture device on machine
        public static async Task<DeviceInformationCollection> GetListVideoCaptureDeviceAsync()
        {
            DeviceInformationCollection arg = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            return arg;
        }

        //Find All AudioCaptureDevice
        public static async Task<DeviceInformationCollection> GetListAudioCaptureDeviceAsync()
        {
            DeviceInformationCollection arg = await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture);
            return arg;
        }

        //Get all device, that we can make a MediaFrame devices
        public static async Task<IReadOnlyList<MediaFrameSourceGroup>> GetListMediaFrameSource()
        {
            IReadOnlyList<MediaFrameSourceGroup> arg = await MediaFrameSourceGroup.FindAllAsync();
            return arg;
        }

        public static async Task<MediaFrameSourceGroup> GetMediaFrameSourceFromId(string device_id)
        {
            MediaFrameSourceGroup mediaFrameDevice = await MediaFrameSourceGroup.FromIdAsync(device_id);
            return mediaFrameDevice;
        }
    }
}
