
using System;
using System.Collections.Generic;
using AudioSwitchZektorProAudio;
using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Logging;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.DeviceTypes.AudioVideoSwitcher;
using Crestron.RAD.DeviceTypes.AudioVideoSwitcher.Extender;
using Crestron.RAD.Drivers.AVReceivers.SoundUnited;
using Crestron.SimplSharp;

namespace GenericAudioSwitch
{
    public class GenericAudioSwitchProtocol : AAudioVideoSwitcherProtocol
    {
        private const double VolumeStep = 1.0;
        private Dictionary<string, SoundUnitedVolumeController> _controllers;
        private bool _disposed;

        public GenericAudioSwitchProtocol(ISerialTransport transport, byte id)
            : base(transport, id)
        {
            
        }

        public override void Initialize(object driverData)
        {
            base.Initialize(driverData);

            _controllers = new Dictionary<string, SoundUnitedVolumeController>()
            {
                {"1",new SoundUnitedVolumeController(ChangeZone1Volume, VolumeStep, TimeBetweenCommands)},
                {"2",new SoundUnitedVolumeController(ChangeZone2Volume, VolumeStep, TimeBetweenCommands)},
                {"3",new SoundUnitedVolumeController(ChangeZone3Volume, VolumeStep, TimeBetweenCommands)},
                {"4",new SoundUnitedVolumeController(ChangeZone4Volume, VolumeStep, TimeBetweenCommands)},
                {"5",new SoundUnitedVolumeController(ChangeZone5Volume, VolumeStep, TimeBetweenCommands)},
                {"6",new SoundUnitedVolumeController(ChangeZone6Volume, VolumeStep, TimeBetweenCommands)},
                {"7",new SoundUnitedVolumeController(ChangeZone7Volume, VolumeStep, TimeBetweenCommands)},
                {"8",new SoundUnitedVolumeController(ChangeZone8Volume, VolumeStep, TimeBetweenCommands)},
            };

            foreach (var controller in _controllers)
            {
                _controllers[controller.Key].VolumeLevel.PercentChanged += VolumeLevel_PercentChanged;
            }


        }

        protected override void ConnectionChanged(bool connection)
        {
            base.ConnectionChanged(connection);

            if (!IsConnected)
                return;

            PollExtenderVolume();
            ClearExtenderRoutes();
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                foreach (var controller in _controllers)
                {
                    if (_controllers[controller.Key] != null)
                    {
                        _controllers[controller.Key].VolumeLevel.PercentChanged -= VolumeLevel_PercentChanged;
                        _controllers[controller.Key].Dispose();

                    }
                }

                _disposed = true;
            }

            base.Dispose();
        }

        //public override void ExtenderSetVolume(AudioVideoExtender extender, uint volume)
        //{
        //    DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "ExtenderSetVolume", String.Format($"{extender.ApiIdentifier}, vol {volume}"));
        //   // VolumeControllerForCommand(extender.ApiIdentifier).VolumeLevel.Percent = volume;
          
        //}
        public override void ExtenderSetVolume(AudioVideoExtender extender, uint volume)
        { 
            DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "ExtenderSetVolume", String.Format($"{extender.ApiIdentifier}, vol {volume}"));

            base.ExtenderSetVolume(extender, volume);
        }

        public override void ExtenderMuteOff(AudioVideoExtender extender)
        {
            DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "ExtenderMuteOff", String.Format($"{extender.ApiIdentifier}"));

            base.ExtenderMuteOff(extender);
        }

        private void VolumeLevel_PercentChanged(object sender, Crestron.RAD.Ext.Util.Scaling.LevelChangedEventArgs<uint> e)
        {
            DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "VolumePercentChanged", String.Format(e.ToString()));
        }


        #region ChangeVolumeHelpers
        private void ChangeZone1Volume(double volume)
        {
            ZoneChangeVolume("1", volume);
        }
        private void ChangeZone2Volume(double volume)
        {
            ZoneChangeVolume("2", volume);
        }
        private void ChangeZone3Volume(double volume)
        {
            ZoneChangeVolume("3", volume);
        }
        private void ChangeZone4Volume(double volume)
        {
            ZoneChangeVolume("4", volume);
        }
        private void ChangeZone5Volume(double volume)
        {
            ZoneChangeVolume("5", volume);
        }
        private void ChangeZone6Volume(double volume)
        {
            ZoneChangeVolume("6", volume);
        }
        private void ChangeZone7Volume(double volume)
        {
            ZoneChangeVolume("7", volume);
        }
        private void ChangeZone8Volume(double volume)
        {
            ZoneChangeVolume("8", volume);
        }
        private void ChangeZone9Volume(double volume)
        {
            ZoneChangeVolume("9", volume);
        }
        private void ChangeZone10Volume(double volume)
        {
            ZoneChangeVolume("10", volume);
        }
        private void ChangeZone11Volume(double volume)
        {
            ZoneChangeVolume("11", volume);
        }
        private void ChangeZone12Volume(double volume)
        {
            ZoneChangeVolume("12", volume);
        }
        private void ChangeZone13Volume(double volume)
        {
            ZoneChangeVolume("13", volume);
        }
        private void ChangeZone14Volume(double volume)
        {
            ZoneChangeVolume("14", volume);
        }
        private void ChangeZone15Volume(double volume)
        {
            ZoneChangeVolume("15", volume);
        }
        private void ChangeZone16Volume(double volume)
        {
            ZoneChangeVolume("16", volume);
        }
        private void ChangeZone17Volume(double volume)
        {
            ZoneChangeVolume("17", volume);
        }
        private void ChangeZone18Volume(double volume)
        {
            ZoneChangeVolume("18", volume);
        }
        private void ChangeZone19Volume(double volume)
        {
            ZoneChangeVolume("19", volume);
        }
        private void ChangeZone20Volume(double volume) => ZoneChangeVolume("20", volume);
        private void ChangeZone21Volume(double volume) => ZoneChangeVolume("21", volume);
        private void ChangeZone22Volume(double volume) => ZoneChangeVolume("22", volume);
        private void ChangeZone23Volume(double volume) => ZoneChangeVolume("23", volume);
        private void ChangeZone24Volume(double volume) => ZoneChangeVolume("24", volume);
        private void ChangeZone25Volume(double volume) => ZoneChangeVolume("25", volume);
        private void ChangeZone26Volume(double volume) => ZoneChangeVolume("26", volume);

        private void ChangeZone27Volume(double volume) => ZoneChangeVolume("27", volume);

        private void ChangeZone28Volume(double volume) => ZoneChangeVolume("28", volume);
        private void ChangeZone29Volume(double volume) => ZoneChangeVolume("29", volume);
        private void ChangeZone30Volume(double volume) => ZoneChangeVolume("30", volume);
        private void ChangeZone31Volume(double volume) => ZoneChangeVolume("31", volume);
        private void ChangeZone32Volume(double volume) => ZoneChangeVolume("32", volume);

        #endregion

        #region IsMuted Helpers

        private bool Zone1IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("1");
            return extender.Muted;
        }
        private bool Zone2IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("2");
            return extender.Muted;
        }
        private bool Zone3IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("3");
            return extender.Muted;
        }
        private bool Zone4IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("4");
            return extender.Muted;
        }
        private bool Zone5IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("5");
            return extender.Muted;
        }
        private bool Zone6IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("6");
            return extender.Muted;
        }
        private bool Zone7IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("7");
            return extender.Muted;
        }
        private bool Zone8IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("8");
            return extender.Muted;
        }
        private bool Zone9IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("9");
            return extender.Muted;
        }
        private bool Zone10IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("10");
            return extender.Muted;
        }
        private bool Zone11IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("11");
            return extender.Muted;
        }
        private bool Zone12IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("12");
            return extender.Muted;
        }
        private bool Zone13IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("13");
            return extender.Muted;
        }
        private bool Zone14IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("14");
            return extender.Muted;
        }
        private bool Zone15IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("15");
            return extender.Muted;
        }
        private bool Zone16IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("16");
            return extender.Muted;
        }
        private bool Zone17IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("17");
            return extender.Muted;
        }
        private bool Zone18IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("18");
            return extender.Muted;
        }
        private bool Zone19IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("19");
            return extender.Muted;
        }
        private bool Zone20IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("20");
            return extender.Muted;
        }
        private bool Zone21IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("21");
            return extender.Muted;
        }
        private bool Zone22IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("22");
            return extender.Muted;
        }
        private bool Zone23IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("23");
            return extender.Muted;
        }
        private bool Zone24IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("24");
            return extender.Muted;
        }
        private bool Zone25IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("25");
            return extender.Muted;
        }
        private bool Zone26IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("26");
            return extender.Muted;
        }
        private bool Zone27IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("27");
            return extender.Muted;
        }
        private bool Zone28IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("28");
            return extender.Muted;
        }
        private bool Zone29IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("29");
            return extender.Muted;
        }
        private bool Zone30IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("30");
            return extender.Muted;
        }
        private bool Zone31IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("31");
            return extender.Muted;
        }
        private bool Zone32IsMuted()
        {
            var extender = GetExtenderByApiIdentifier("32");
            return extender.Muted;
        }

        #endregion

        private void ZoneChangeVolume(string zone, double volume)
        {
            DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "ChangeZoneVolume", String.Format($"zone {zone},vol {volume}"));

            // Input volume is already in correct scale, just need to cast it
            uint vol = (uint)volume;
            var command = new CommandSet(
                zone,
                string.Format($"OUTPUT{zone}:VOL{vol}"),
                CommonCommandGroupType.AudioVideoExtender,
                null,
                true,
                CommandPriority.Normal,
                StandardCommandsEnum.Vol);

            // Notify volume controller that we're sending a volume command
            // which means it needs to ensure we poll later even if this
            // command is not queued. Otherwise, fake feedback will leave the
            // application with the wrong volume value.
           // MuteVolControllerForCommand(zone).StartControllingVolume();

            SendCommand(command);
        }

        private SoundUnitedMuteVolController MuteVolControllerForCommand(string zone)
        {
            DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "MuteVolControllerForCommand", "");
            return VolumeControllerForCommand(zone).MuteVol;
        }

        private SoundUnitedVolumeController VolumeControllerForCommand(string zone)
        {
            // Main zone commands use the CommonCommandGroupType for the command
            // Ones for zones use the zone-specific group, so default to the main
            // zone and return the other zone controllers only for those specific
            // command groups
            DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "VolControllerForCommand", "");
            SoundUnitedVolumeController controller = _controllers[zone];
            //TODO need a list of volume controllers

            return controller;
        }


        private void ClearExtenderRoutes()
        {
            DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "ClearExtenderRoutes", "");

            //var command = new CommandSet(
            //    String.Format($"Clear zone routes"),
            //    string.Format($"^SZ @1:32,0$"),
            //    CommonCommandGroupType.AudioVideoExtender,
            //    null,
            //    true,
            //    CommandPriority.Normal,
            //    StandardCommandsEnum.AudioVideoExtenderRoute);

            //SendCommand(command);

        }

        private void PollExtenderVolume()
        {
            DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "PollVolume", string.Format($"zone"));

            //var command = new CommandSet(
            //    string.Format($"Poll zone volume"),
            //    string.Format($"^VPZ @1:32,?$"),
            //    CommonCommandGroupType.AudioVideoExtender,
            //    null,
            //    true,
            //    CommandPriority.High,
            //    StandardCommandsEnum.Vol);

            //SendCommand(command);
        }

    }


}