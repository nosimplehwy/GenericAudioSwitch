
using Crestron.RAD.Common.Transports;
using Crestron.RAD.DeviceTypes.AudioVideoSwitcher;
using Crestron.SimplSharp;

namespace GenericAudioSwitch
{
    public class GenericAudioSwitchProtocol : AAudioVideoSwitcherProtocol
    {
        private const double VolumeStep = 1.0;

        public GenericAudioSwitchProtocol(ISerialTransport transport, byte id)
            : base(transport, id)
        {
            
        }

    


    }

  
}