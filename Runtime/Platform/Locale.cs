﻿using System.Globalization;
using UnityEngine;

#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace Unity.Services.Analytics.Internal
{
    public static class Locale
    {
        
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string unity_services_current_culture();
        [DllImport("__Internal")]
        private static extern string unity_services_system_culture();

        /// <summary>
        /// Returns the current culture info. Invokes native method on Android and iOS.
        /// </summary>
        /// <returns></returns>
        public static CultureInfo CurrentCulture()
        {
            return new CultureInfo(unity_services_current_culture());
        }

        /// <summary>
        /// Returns the current culture info. Invokes native method on iOS.
        /// </summary>
        /// <returns></returns>
        public static CultureInfo SystemCulture()
        {
            return new CultureInfo(unity_services_system_culture()); 
        }
        
#elif UNITY_ANDROID && !UNITY_EDITOR
        
        /// <summary>
        /// Returns the current culture info. Invokes native method on Android and iOS.
        /// </summary>
        /// <returns></returns>
        public static CultureInfo CurrentCulture()
        {
            var localeClass = new AndroidJavaClass("java.util.Locale");
            var defaultLocale = localeClass.CallStatic<AndroidJavaObject>("getDefault");
            var language = defaultLocale.Call<string>("getLanguage");
            var country = defaultLocale.Call<string>("getCountry");
            
            var ret = new CultureInfo($"{language}-{country}", false);
            return ret;
        }

        /// <summary>
        /// Returns the current culture info. Invokes native method on iOS.
        /// </summary>
        /// <returns></returns>
        public static CultureInfo SystemCulture()
        {
            return CultureInfo.InvariantCulture;
        }
        
#else

        /// <summary>
        /// Returns the current culture info. Invokes native method on Android and iOS.
        /// </summary>
        /// <returns></returns>
        public static CultureInfo CurrentCulture()
        {
            return CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Returns the current culture info. Invokes native method on iOS.
        /// </summary>
        /// <returns></returns>
        public static CultureInfo SystemCulture()
        {
            return CultureInfo.InvariantCulture;
        }
        
#endif
        
    }
}