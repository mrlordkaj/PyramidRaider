using OpenitvnGame.Helpers;
using OpenitvnGame.MathExtend;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace OpenitvnGame
{
    class Leaderboard
    {
        const string RECORD_USERNAME = "leaderboard_username";
        const string RECORD_DEVICEID = "leaderboard_deviceid";
        const string RECORD_SCORE = "leaderboard_score";

        const string URI_PREFIX = "http://mrlordkaj.byethost13.com/leaderboard/v1/";
        const short GAMEID = 2;
        const byte DEVICEID_LENGTH = 32;

        public static int GameId
        {
            get
            {
                return GAMEID;
            }
        }

        public static string UserName
        {
            get
            {
                return SettingHelper.GetSetting<string>(RECORD_USERNAME, "");
            }
            private set
            {
                SettingHelper.StoreSetting(RECORD_USERNAME, value, true);
            }
        }

        public static int Score
        {
            get
            {
                return SettingHelper.GetSetting<int>(RECORD_SCORE, 0);
            }
            private set
            {
                SettingHelper.StoreSetting(RECORD_SCORE, value, true);
            }
        }

        public static string DeviceId
        {
            get
            {
                string _deviceId = SettingHelper.GetSetting<string>(RECORD_DEVICEID, "");
                if (_deviceId.Length != DEVICEID_LENGTH)
                {
                    _deviceId = genDeviceId();
                    SettingHelper.StoreSetting(RECORD_DEVICEID, _deviceId, true);
                }
                return _deviceId;
            }
        }

        static ILeaderboardCaller caller;

        public static void SubmitScore(int score, string username, ILeaderboardCaller caller)
        {
            Leaderboard.caller = caller;
            UserName = username;
            if(Score < score) Score = score;
            string hash = MD5Core.GetHashString(string.Format("{0}.{1}.{2}", GameId, DeviceId, score));
            string uri = string.Format(URI_PREFIX + "?act=submit&gameid={0}&deviceid={1}&score={2}&hash={3}&name={4}", GameId, DeviceId, Score, hash, UserName);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.BeginGetResponse(submitScoreCallback, request);
            SettingHelper.SaveSetting();
        }

        private static void submitScoreCallback(IAsyncResult result)
        {
            Thread.Sleep(5000);
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            if (request != null)
            {
                try
                {
                    WebResponse response = request.EndGetResponse(result);
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    string resultCode = reader.ReadLine();
                    if (resultCode == "1")
                    {
                        //gui diem thanh cong
                        caller.OnSubmitSuccess();
                        reader.Close();
                        stream.Close();
                        response.Close();
                        return;
                    }
                }
                catch { }
            }
            //gui diem that bai, xu ly loi tai day
            caller.OnSubmitFailed();
        }

        public static void GetRank(ILeaderboardCaller caller)
        {
            Leaderboard.caller = caller;
            if (Score == 0)
            {
                caller.OnGetRankSuccess(-1);
                return;
            }
            string hash = MD5Core.GetHashString(string.Format("{0}.{1}.{2}", GameId, DeviceId, Score));
            string uri = string.Format(URI_PREFIX + "?act=myrank&gameid={0}&deviceid={1}&score={2}&hash={3}", GameId, DeviceId, Score, hash);
            HttpWebRequest rankRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            rankRequest.BeginGetResponse(getRankCallback, rankRequest);
        }

        private static void getRankCallback(IAsyncResult result)
        {
            Thread.Sleep(1000);
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            if (request != null)
            {
                try
                {
                    WebResponse response = request.EndGetResponse(result);
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    string resultCode = reader.ReadLine();
                    if (resultCode == "1")
                    {
                        //xu ly ket qua tai day
                        caller.OnGetRankSuccess(int.Parse(reader.ReadLine()));
                        reader.Close();
                        stream.Close();
                        response.Close();
                        return;
                    }
                }
                catch { }
            }
            //xu ly loi tai day
            caller.OnGetRankFailed();
        }

        public static void View7(ILeaderboardCaller caller)
        {
            Leaderboard.caller = caller;
            string uri = string.Format(URI_PREFIX + "?act=view7&gameid={0}", GameId);
            HttpWebRequest view7Request = (HttpWebRequest)HttpWebRequest.Create(uri);
            view7Request.BeginGetResponse(view7Callback, view7Request);
        }

        private static void view7Callback(IAsyncResult result)
        {
            Thread.Sleep(2000);
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            if (request != null)
            {
                try
                {
                    WebResponse response = request.EndGetResponse(result);
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    string resultCode = reader.ReadLine();
                    if (resultCode == "1")
                    {
                        //xu ly ket qua tai day
                        caller.OnView7Success(reader);
                        reader.Close();
                        stream.Close();
                        response.Close();
                        return;
                    }
                }
                catch { }
            }
            //xu ly loi tai day
            caller.OnView7Failed();
        }

        public static void ViewAll(ILeaderboardCaller caller)
        {
            Leaderboard.caller = caller;
            string uri = string.Format(URI_PREFIX + "?act=viewall&gameid={0}", GameId);
            HttpWebRequest viewAllRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            viewAllRequest.BeginGetResponse(viewAllCallback, viewAllRequest);
        }

        private static void viewAllCallback(IAsyncResult result)
        {
            Thread.Sleep(2000);
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            if (request != null)
            {
                try
                {
                    WebResponse response = request.EndGetResponse(result);
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    string resultCode = reader.ReadLine();
                    if (resultCode == "1")
                    {
                        //xu ly ket qua tai day
                        caller.OnViewAllSuccess(reader);
                        reader.Close();
                        stream.Close();
                        response.Close();
                        return;
                    }
                }
                catch { }
            }
            //xu ly loi tai day
            caller.OnViewAllFailed();
        }

        public static void RemoveScore(ILeaderboardCaller caller)
        {
            Leaderboard.caller = caller;
            string hash = MD5Core.GetHashString(string.Format("{0}.{1}.{2}", GameId, DeviceId, 0));
            string uri = string.Format(URI_PREFIX + "?act=remove&gameid={0}&deviceid={1}&score={2}&hash={3}", GameId, DeviceId, 0, hash);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.BeginGetResponse(removeScoreCallback, request);
        }

        private static void removeScoreCallback(IAsyncResult result)
        {
            Thread.Sleep(5000);
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            if (request != null)
            {
                try
                {
                    WebResponse response = request.EndGetResponse(result);
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    string resultCode = reader.ReadLine();
                    if (resultCode == "1")
                    {
                        //gui diem thanh cong
                        Score = 0;
                        SettingHelper.SaveSetting();
                        caller.OnRemoveSuccess();
                        reader.Close();
                        stream.Close();
                        response.Close();
                        return;
                    }
                }
                catch { }
            }
            //gui diem that bai, xu ly loi tai day
            caller.OnRemoveFailed();
        }

        private static string genDeviceId()
        {
            string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string code = "";
            for (int i = 0; i < DEVICEID_LENGTH; i++)
            {
                code += chars[GameScene.Random.Next(chars.Length - 1)];
            }
            return code;
        }
    }
}
