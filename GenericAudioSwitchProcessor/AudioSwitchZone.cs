using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using GenericAudioSwitchProcessor;

namespace AudioSwitchProcessor
{
    public class AudioSwitchZone
    {
        private string _zone;
        private ushort _route;
        private ushort _mute;
        private ushort _vol;

        public void Initialize(ushort zoneNum)
        {
            _zone = Convert.ToString(zoneNum);
            _route = 0;
            _mute = 0;
            _vol = 0;
            AudioSwitch.RegisterZone(_zone, this);
        }

        public RouteDelegate Route { get; set; }
        public VolDelegate Vol { get; set; }
        public MuteDelegate Mute { get; set; }


        internal void ProcessCommand(string cmd, string value)
        {
            switch (cmd)
            {
                case "ROUTE":
                    {
                        try
                        {
                            var route = int.Parse(value);
                            Route.Invoke((ushort)route);
                            _route = (ushort)route;
                            Logger.Log(LogMethod.Console, "ProcessCommand", " OUTPUT input=" + route);
                        }
                        catch (Exception e)
                        {
                            Logger.Log(LogMethod.ConsoleAndError, "ProcessCommand", " ROUTE input could not be parsed, " + e);
                        }
                        break;
                    }
                    case "MUTE":
                    {
                        switch (value)
                        {
                            case "?":
                                {

                                    SendMute();
                                    break;
                                }
                            case "1":
                                {
                                    UpdateMute(1);
                                    break;
                                }
                            case "0":
                                {
                                    UpdateMute(0);
                                    break;
                                }
                            default:
                                {
                                    Logger.Log(LogMethod.ConsoleAndError, "ProcessCommand", " MUTE could not be parsed.");
                                    break;
                                }
                        }
                        break;

                     }
                    case "VOL":
                    {
                        switch (value)
                        {
                            case "?":
                                {
                                    SendVolume(_vol);
                                    break;
                                }
                            default:
                                {
                                    try
                                    {
                                        var vol = int.Parse(value);
                                        if (vol >= 0 & vol <= 100)
                                        {
                                                UpdateVolume((ushort)vol);

                                        }
                                        else
                                        {
                                            Logger.Log(LogMethod.Console, "ProcessCommand", "VOLSET is out of range");
                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Log(LogMethod.ConsoleAndError, "ProcessCommand", " VOL could not be parsed, " + e);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                        default:
                        {
                            Logger.Log(LogMethod.ConsoleAndError, "ProcessCommand", " Command could not be parsed");
                            break;
                        }  
                    }
            }

        private void UpdateVolume(ushort level)
        {
            Logger.Log(LogMethod.ConsoleAndError, "UpdateVolume - Level", level.ToString());
           
                _vol = level;
                Vol.Invoke(_vol);
                SendVolume(_vol);
                Logger.Log(LogMethod.ConsoleAndError, "UpdateVolume - New Level", level.ToString());

        }


        private void SendVolume(int level)
        {
            Logger.Log(LogMethod.ConsoleAndError, "SendVolume", level.ToString());
            AudioSwitch.SendData("^OUTPUT" + _zone + ":VOL" + level + "\r");
        }


        private void UpdateMute(ushort state)
        {
            if (_mute != state)
            {
                _mute = state;
                Mute.Invoke(state);
            }
            SendMute();

        }

        private void SendMute()
        {
            Logger.Log(LogMethod.ConsoleAndError, "SendMute", _mute.ToString());
            AudioSwitch.SendData("^OUTPUT" + _zone + ":MUTE" + _mute + "\r");
        }

        public delegate void RouteDelegate(ushort value);
        public delegate void VolDelegate(ushort value);
        public delegate void MuteDelegate(ushort value);
    }

}