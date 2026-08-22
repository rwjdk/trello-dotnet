using System;
using System.Reflection;
using System.Text.Json.Serialization;
using TrelloDotNet.Model;

namespace TrelloDotNet.Control
{
    internal static class EnumHelper
    {
        internal static string GetJsonPropertyName(this Enum enumVal)
        {
            Type type = enumVal.GetType();
            MemberInfo[] memInfo = type.GetMember(enumVal.ToString());
            object[] attributes = memInfo[0].GetCustomAttributes(typeof(JsonPropertyNameAttribute), false);
            return attributes.Length > 0 ? ((JsonPropertyNameAttribute)attributes[0]).Name : null;
        }

        internal static LabelColorInfo GetColorInfo(this Enum enumVal)
        {
            Type type = enumVal.GetType();
            MemberInfo[] memInfo = type.GetMember(enumVal.ToString());
            object[] attributes = memInfo[0].GetCustomAttributes(typeof(LabelColorInfo), false);
            return attributes.Length > 0 ? (LabelColorInfo)attributes[0] : null;
        }
    }
}