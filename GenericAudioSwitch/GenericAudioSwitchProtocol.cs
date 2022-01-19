
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
                {"1",new SoundUnitedVolumeController(Zone1IsMuted,ChangeZone1Volume, VolumeStep, TimeBetweenCommands)},
                {"2",new SoundUnitedVolumeController(Zone2IsMuted,ChangeZone2Volume, VolumeStep, TimeBetweenCommands)},
                {"3",new SoundUnitedVolumeController(Zone3IsMuted,ChangeZone3Volume, VolumeStep, TimeBetweenCommands)},
                {"4",new SoundUnitedVolumeController(Zone4IsMuted,ChangeZone4Volume, VolumeStep, TimeBetweenCommands)},
                {"5",new SoundUnitedVolumeController(Zone5IsMuted,ChangeZone5Volume, VolumeStep, TimeBetweenCommands)},
                {"6",new SoundUnitedVolumeController(Zone6IsMuted,ChangeZone6Volume, VolumeStep, TimeBetweenCommands)},
                {"7",new SoundUnitedVolumeController(Zone7IsMuted,ChangeZone7Volume, VolumeStep, TimeBetweenCommands)},
                {"8",new SoundUnitedVolumeController(Zone8IsMuted,ChangeZone8Volume, VolumeStep, TimeBetweenCommands)},
                {"9",new SoundUnitedVolumeController(Zone9IsMuted,ChangeZone9Volume, VolumeStep, TimeBetweenCommands)},
                {"10",new SoundUnitedVolumeController(Zone10IsMuted,ChangeZone10Volume, VolumeStep, TimeBetweenCommands)},
                {"11",new SoundUnitedVolumeController(Zone11IsMuted,ChangeZone11Volume, VolumeStep, TimeBetweenCommands)},
                {"12",new SoundUnitedVolumeController(Zone12IsMuted,ChangeZone12Volume, VolumeStep, TimeBetweenCommands)},
                {"13",new SoundUnitedVolumeController(Zone13IsMuted,ChangeZone13Volume, VolumeStep, TimeBetweenCommands)},
                {"14",new SoundUnitedVolumeController(Zone14IsMuted,ChangeZone14Volume, VolumeStep, TimeBetweenCommands)},
                {"15",new SoundUnitedVolumeController(Zone15IsMuted,ChangeZone15Volume, VolumeStep, TimeBetweenCommands)},
                {"16",new SoundUnitedVolumeController(Zone16IsMuted,ChangeZone16Volume, VolumeStep, TimeBetweenCommands)},
                {"17",new SoundUnitedVolumeController(Zone17IsMuted,ChangeZone17Volume, VolumeStep, TimeBetweenCommands)},
                {"18",new SoundUnitedVolumeController(Zone18IsMuted,ChangeZone18Volume, VolumeStep, TimeBetweenCommands)},
                {"19",new SoundUnitedVolumeController(Zone19IsMuted,ChangeZone19Volume, VolumeStep, TimeBetweenCommands)},
                {"20",new SoundUnitedVolumeController(Zone20IsMuted,ChangeZone20Volume, VolumeStep, TimeBetweenCommands)},
                {"21",new SoundUnitedVolumeController(Zone21IsMuted,ChangeZone21Volume, VolumeStep, TimeBetweenCommands)},
                {"22",new SoundUnitedVolumeController(Zone22IsMuted,ChangeZone22Volume, VolumeStep, TimeBetweenCommands)},
                {"23",new SoundUnitedVolumeController(Zone23IsMuted,ChangeZone23Volume, VolumeStep, TimeBetweenCommands)},
                {"24",new SoundUnitedVolumeController(Zone24IsMuted,ChangeZone24Volume, VolumeStep, TimeBetweenCommands)},
                {"25",new SoundUnitedVolumeController(Zone25IsMuted,ChangeZone25Volume, VolumeStep, TimeBetweenCommands)},
                {"26",new SoundUnitedVolumeController(Zone26IsMuted,ChangeZone26Volume, VolumeStep, TimeBetweenCommands)},
                {"27",new SoundUnitedVolumeController(Zone27IsMuted,ChangeZone27Volume, VolumeStep, TimeBetweenCommands)},
                {"28",new SoundUnitedVolumeController(Zone28IsMuted,ChangeZone28Volume, VolumeStep, TimeBetweenCommands)},
                {"29",new SoundUnitedVolumeController(Zone29IsMuted,ChangeZone29Volume, VolumeStep, TimeBetweenCommands)},
                {"30",new SoundUnitedVolumeController(Zone30IsMuted,ChangeZone30Volume, VolumeStep, TimeBetweenCommands)},
                {"31",new SoundUnitedVolumeController(Zone31IsMuted,ChangeZone31Volume, VolumeStep, TimeBetweenCommands)},
                {"32",new SoundUnitedVolumeController(Zone32IsMuted,ChangeZone32Volume, VolumeStep, TimeBetweenCommands)},
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

        public override void ExtenderSetVolume(AudioVideoExtender extender, uint volume)
        {
            DriverLog.Log(EnableLogging, Log, LoggingLevel.Debug, "ExtenderSetVolume", String.Format($"{extender.ApiIdentifier}, vol {volume}"));
            VolumeControllerForCommand(extender.ApiIdentifier).VolumeLevel.Percent = volume;
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
                false,
                CommandPriority.Normal,
                StandardCommandsEnum.Vol);

            // Notify volume controller that we're sending a volume command
            // which means it needs to ensure we poll later even if this
            // command is not queued. Otherwise, fake feedback will leave the
            // application with the wrong volume value.
            MuteVolControllerForCommand(zone).StartControllingVolume();

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