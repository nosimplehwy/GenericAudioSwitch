using System;
using System.Collections.Generic;
using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Logging;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.DeviceTypes.AudioVideoSwitcher;

namespace GenericAudioSwitch
{
    public class GenericAudioSwitchProtocol : AAudioVideoSwitcherProtocol
    {

        public GenericAudioSwitchProtocol(ISerialTransport transport, byte id)
            : base(transport, id)
        {
            
        }

        protected override void ConnectionChanged(bool connection)
        {
            base.ConnectionChanged(connection);

            if (!IsConnected)
                return;

            Poll();
        }


    }


}