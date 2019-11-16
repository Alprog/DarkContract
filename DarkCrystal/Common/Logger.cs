
using System;
using UnityEngine;

namespace DarkCrystal
{
    public class Logger : Singleton<Logger>
    {
        public void Print(string message)
        {
            Debug.Log(message);
        }

        public void Print(string message, params object[] args)
        {
            Debug.Log(String.Format(message, args));
        }

        public void Warning(string message)
        {
            Debug.LogWarning(message);
        }

        public void Warning(string message, params object[] args)
        {
            Debug.LogWarning(String.Format(message, args));
        }

        public void Error(string message)
        {
            Debug.LogError(message);
        }

        public void Error(string message, params object[] args)
        {
            Debug.LogError(String.Format(message, args));
        }
    }
}
