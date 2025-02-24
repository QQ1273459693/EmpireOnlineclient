﻿using UnityEngine;

namespace TEngine
{
    public class ErrorLogger
    {
        public ErrorLogger()
        {
            Application.logMessageReceived += LogHandler;
        }

        ~ErrorLogger()
        {
            Application.logMessageReceived -= LogHandler;
        }

        private void LogHandler(string condition, string stacktrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                string des = $"客户端报错, \n#内容#：---{condition} \n#位置#：---{stacktrace}";
                Log.Info(des);
                //GameModule.UI.ShowUI<LogUI>(des);
            }
        }
    }
}