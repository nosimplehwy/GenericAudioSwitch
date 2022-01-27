using System;
using System.Collections.Generic;
using Crestron.DeviceDrivers.SDK.Controllers.NumericController.Scaling;
using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Logging;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.DeviceTypes.AudioVideoSwitcher;
using Crestron.RAD.DeviceTypes.AudioVideoSwitcher.Extender;
using Crestron.RAD.DeviceTypes.RADAVReceiver;

namespace GenericAudioSwitch
{
    public class GenericAudioSwitchProtocol : AAudioVideoSwitcherProtocol
    {

        public GenericAudioSwitchProtocol(ISerialTransport transport, byte id)
            : base(transport, id)
        {
            EnableLogging = true;
        }

        protected override void ConnectionChanged(bool connection)
        {
            base.ConnectionChanged(connection);
            DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "ConnectionChanged", $"connection = {connection} IsConnected = {IsConnected}");
           // if (connection) SendAllRoutes();
        }


        private void SendAllRoutes()
        {
            foreach (var extender in GetRoutableOutputs())
            {
                DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "SendAllRoutes", $"extender = {extender.Id} Source = {extender.AudioSourceExtenderId}");
                if(extender.AudioSourceExtenderId == null) continue;
                RouteAudioInput(extender.AudioSourceExtenderId, extender.Id);
            }

        }

        protected override bool PrepareStringThenSend(CommandSet commandSet)
        {
            commandSet.Command = $"{commandSet.Command}\r";
            DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "PrepareStringThenSend", commandSet.Command);
            return base.PrepareStringThenSend(commandSet);
        }

        public override void DataHandler(string rx)
        {
            DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "DataHandler", rx);
            base.DataHandler(rx);
        }
    }


}