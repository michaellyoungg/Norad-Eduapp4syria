using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * English Tracing Book Package
 *
 * @license		    Unity Asset Store EULA https://unity3d.com/legal/as_terms
 * @author		    Indie Studio - Baraa Nasser
 * @Website		    https://indiestd.com
 * @Asset Store     https://www.assetstore.unity3d.com/en/#!/publisher/9268
 * @Unity Connect   https://connect.unity.com/u/5822191d090915001dbaf653/column
 * @email		    info@indiestd.com
 *
 */

namespace IndieStudio.EnglishTracingBook.Utility
{
    [Serializable]
    public class AdPackage
    {
        public bool isEnabled = true;
        public List<AdEvent> adEvents = new List<AdEvent>();
        public Package package;

        [Serializable]
        public class AdEvent
        {
            public Event evt;
            public Type type = Type.BANNER;
#if GOOGLE_MOBILE_ADS
				public GoogleMobileAds.Api.AdPosition adPostion = GoogleMobileAds.Api.AdPosition.Bottom;
#endif
            public bool isEnabled = false;

            public enum Event
            {
                ON_LOAD_LOGO_SCENE,
                ON_LOAD_MAIN_SCENE,
                ON_LOAD_ALBUM_SCENE,
                ON_LOAD_GAME_SCENE,
                ON_SHOW_WIN_DIALOG,
                ON_RENEW_HELP_COUNT
            }

            public enum Type
            {
                BANNER,
                INTERSTITIAL,
                RewardBasedVideo,
            }
        }

        public enum Package
        {
            ADMOB,
            CHARTBOOST,
            UNITY
        }

        /// <summary>
        /// Build the ad events.
        /// </summary>
        public void BuildAdEvents()
        {
            Array eventsEnum = Enum.GetValues(typeof(AdEvent.Event));

            if (NeedsToResetAdEventsList(eventsEnum, adEvents))
            {
                adEvents.Clear();
            }

            foreach (AdEvent.Event e in eventsEnum)
            {
                if (!InAdEventsList(adEvents, e))
                {
                    adEvents.Add(new AdEvent() { evt = e });
                }
            }
        }

        /// <summary>
        /// Whether the given event in the adEvents list.
        /// </summary>
        /// <returns><c>true</c>, if evt was found, <c>false</c> otherwise.</returns>
        /// <param name="adEvents">Ad events.</param>
        /// <param name="evt">Evt.</param>
        public bool InAdEventsList(List<AdEvent> adEvents, AdEvent.Event evt)
        {
            if (adEvents == null)
            {
                return false;
            }

            foreach (AdEvent adEvent in adEvents)
            {
                if (adEvent.evt == evt)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Whether to reset ad events list or not.
        /// </summary>
        /// <returns><c>true</c>, if reset ad events list was needed, <c>false</c> otherwise.</returns>
        /// <param name="eventsEnum">Events enum.</param>
        /// <param name="adEvents">Ad events.</param>
        public bool NeedsToResetAdEventsList(Array eventsEnum, List<AdEvent> adEvents)
        {
            if (eventsEnum.Length != adEvents.Count)
            {
                return true;
            }

            return false;
        }
    }
}