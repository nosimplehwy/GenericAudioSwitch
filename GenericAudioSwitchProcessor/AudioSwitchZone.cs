using System;
using System.Collections.Generic;
using Crestron.SimplSharp;

namespace AudioSwitchBridge
{
    public class AudioSwitchZone
    {
        private string _zone;
        private ushort _route;
        private ushort _mute;
        private ushort _vol;
        private const ushort _startup = 32769;
        CTimer Timer;
        private const uint _repeatDelay = 25;
        private const uint _repeatTime = 25;

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
                            if (_route == 0 & route > 0)
                            {
                                UpdateVolume(_startup);
                            }
                            else if(route == 0)
                                UpdateVolume(0);
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
                            case "+":
                                {
                                    VolumeUp();
                                    break;
                                }
                            case "-":
                                {
                                    VolumeDown();
                                    break;
                                }
                            default:
                                {
                                    try
                                    {
                                        var vol = int.Parse(value);
                                        if (vol >= 0 & vol <= 65535)
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
            var change = 0;

            if (level > _vol)
                change = 655;
            else if (level < _vol)
                change = -655;
            else
            {
                Stop();
                return;
            }

            int newLevel = _vol + change;

            var atLimit = false;
            if (newLevel > 65535)
            {
                newLevel = 65535;
                atLimit = true;
            }
            else if (newLevel < 0)
            {
                newLevel = 0;
                atLimit = true;
            }

            _vol = (ushort)newLevel;
                Vol.Invoke(_vol);
                SendVolume(_vol);
                Logger.Log(LogMethod.ConsoleAndError, "UpdateVolume - New Level", newLevel.ToString());

            if (atLimit) // Don't go past end
               Stop();
            else if (Timer == null)
                Timer = new CTimer(o => { UpdateVolume(level); }, null, _repeatDelay,_repeatTime);
        }

        private void VolumeUp()
        {
            if (Timer != null) return;
             UpdateVolume(_vol++);
        }

        private void VolumeDown()
        {
            if (Timer != null) return;
                UpdateVolume(_vol--);
        }

        public void Stop()
        {
            Logger.Log(LogMethod.ConsoleAndError, "Stop", "Timer");
            if (Timer != null)
                Timer.Stop();
            Timer = null;
        }

        private void SendVolume(int level)
        {
            AudioSwitch.SendData("^OUTPUT" + _zone + ":VOL" + level + "\r");
        }


        private void UpdateMute(ushort state)
        {
            if (_mute != state)
            {
                _mute = state;
                Mute.Invoke(state);
                SendMute();
            }
        }

        private void SendMute()
        {
            AudioSwitch.SendData("^OUTPUT" + _zone + ":MUTE" + _mute + "\r");
        }

        public delegate void RouteDelegate(ushort value);
        public delegate void VolDelegate(ushort value);
        public delegate void MuteDelegate(ushort value);
    }

}