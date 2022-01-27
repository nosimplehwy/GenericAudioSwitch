using System;
using System.Text.RegularExpressions;
using AudioSwitchProcessor;

namespace GenericAudioSwitchProcessor
{
    public class AudioSwitchZone
    {
        private string _zone;
        private ushort _route;
        private ushort _mute;
        private ushort _vol;

        private string _regex = @"OUTPUT(\d+):([A-Z]+)(\d+|\?|\+|\-)";


        public delegate void RouteDelegate(ushort value);
        public delegate void VolDelegate(ushort value);
        public delegate void MuteDelegate(ushort value);

        public delegate ushort RouteFeedbackDelegate();

        public delegate ushort VolFeedbackDelegate();

        public delegate ushort MuteFeedbackDelegate();

        public RouteDelegate Route { get; set; }
        public VolDelegate Vol { get; set; }
        public MuteDelegate Mute { get; set; }
        public RouteFeedbackDelegate RouteFeedback { get; set; }
        public VolFeedbackDelegate VolFeedback { get; set; }
        public MuteFeedbackDelegate MuteFeedback { get; set; }

        public void Initialize(ushort zoneNum)
        {
            _zone = Convert.ToString(zoneNum);
            _route = 0;
            _mute = 0;
            _vol = 0;
            //AudioSwitch.RegisterZone(_zone, this);
            AudioSwitch.MessageReceived += AudioSwitch_MessageReceived;
        }

        private void AudioSwitch_MessageReceived(object sender, string message)
        {
            Logger.Log(LogMethod.Console, "ProcessMessage", $"Zone {_zone}: {message}");

            var match = Regex.Match(message, _regex);
            if (!match.Success)
            {
                Logger.Log(LogMethod.Console, "ProcessMessage", $"Zone {_zone}: OUTPUT could not be parsed.");
                return;
            }
            while (match.Success)
            {

                if(_zone == match.Groups[1].Value) 
                    ProcessCommand(match.Groups[2].Value, match.Groups[3].Value);
                
                match = match.NextMatch();
            }
        }


       private void ProcessCommand(string cmd, string value)
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
                                    SendVolume();
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
                Logger.Log(LogMethod.ConsoleAndError, "UpdateVolume - New Level", level.ToString());

        }


        public void SendVolume()
        {
            var level = VolFeedback.Invoke();
            Logger.Log(LogMethod.ConsoleAndError, "SendVolume", level.ToString());
            AudioSwitch.SendData("^OUTPUT" + _zone + ":VOL" + level + "\r");
        }


        private void UpdateMute(ushort state)
        {
            Logger.Log(LogMethod.ConsoleAndError, "UpdateMute", state.ToString());
            if (_mute != state)
            {
                _mute = state;
                Mute.Invoke(state);
            }
            else
            {
                //respond with feedback anyway
                SendMute();
            }
            
        }

        public void SendMute()
        {
            var mute = MuteFeedback.Invoke();
            Logger.Log(LogMethod.ConsoleAndError, "SendMute", mute.ToString());
            AudioSwitch.SendData("^OUTPUT" + _zone + ":MUTE" + mute + "\r");
        }

    }

}