using System;
using System.Globalization;

namespace TrelloDotNet.Control
{
    internal static class IdToCreatedHelper
    {
        internal static DateTimeOffset? GetCreatedFromId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            string hexDecimaltimestapThatIsPartOfId = id.Substring(0, 8);
            int decimalVersionInEpoch = int.Parse(hexDecimaltimestapThatIsPartOfId, NumberStyles.HexNumber);
            return DateTimeOffset.FromUnixTimeSeconds(decimalVersionInEpoch);
        }
    }
}