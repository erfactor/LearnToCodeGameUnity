using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    public static class Timing
    {
        public static float WinWindowSceneChangeDelay
        {
            get
            {
                if (Config.Debug.SkipWaitingIfPossible) return 0.0f;
                else return 0.5f;
            }
        }

        public static float WinWindowOnLevelCompletionDelay
        {
            get
            {
                if (Config.Debug.SkipWaitingIfPossible) return 0.0f;
                else return 0.5f;
            }
        }

        public static float CommandExecutionTimeInSeconds
        {
            get
            {
                return 1.2f;
            }
        }

        public static float StartupWordsShowInterval
        {
            get
            {
                if (Config.Debug.SkipWaitingIfPossible) return 0.0f;
                else return 0.3f;
            }
        }

        public static float StartupWordsHideInterval
        {
            get
            {
                if (Config.Debug.SkipWaitingIfPossible) return 0.0f;
                else return 0.1f;
            }
        }

        public static float StartupWordsStayDuration
        {
            get
            {
                if (Config.Debug.SkipWaitingIfPossible) return 0.0f;
                else return 1.0f;
            }
        }

        public static float StartupAfterHideDuration
        {
            get
            {
                if (Config.Debug.SkipWaitingIfPossible) return 0.0f;
                else return 0.5f;
            }
        }
    }
}
