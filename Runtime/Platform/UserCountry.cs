using System.Globalization;

namespace Unity.Services.Analytics.Internal.Platform
{
    public static class UserCountry
    {
        public static string Name()
        {
            var culture = Locale.CurrentCulture();
            var region = new RegionInfo(culture.LCID);
            return region.TwoLetterISORegionName;
        }
    }
}
