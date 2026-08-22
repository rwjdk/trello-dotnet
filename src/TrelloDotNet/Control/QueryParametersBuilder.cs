using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using TrelloDotNet.Model;

namespace TrelloDotNet.Control
{
    internal class QueryParametersBuilder
    {
        internal QueryParameter[] GetViaQueryParameterAttributes<T>(T instance)
        {
            Type type = instance.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();
            List<QueryParameter> parameters = new List<QueryParameter>();
            foreach (PropertyInfo updateableProperty in propertyInfos)
            {
                object[] updateableAttributes = updateableProperty.GetCustomAttributes(typeof(QueryParameterAttribute), true);
                if (!updateableAttributes.Any())
                {
                    continue;
                }

                object[] jsonPropertyNameAttributes = updateableProperty.GetCustomAttributes(typeof(JsonPropertyNameAttribute), true);
                if (!jsonPropertyNameAttributes.Any())
                {
                    continue;
                }

                JsonPropertyNameAttribute jsonPropertyName = (JsonPropertyNameAttribute)jsonPropertyNameAttributes.First();
                QueryParameterAttribute updateableAttribute = (QueryParameterAttribute)updateableAttributes.First();
                Type updateablePropertyType = updateableProperty.PropertyType;

                object rawValue = updateableProperty.GetValue(instance);
                if (rawValue == null && !updateableAttribute.IncludeIfNull)
                {
                    continue;
                }

                if (updateablePropertyType == typeof(string))
                {
                    parameters.Add(rawValue == null ? new QueryParameter(jsonPropertyName.Name, string.Empty) : new QueryParameter(jsonPropertyName.Name, (string)rawValue));
                }
                else if (updateablePropertyType == typeof(int) || updateablePropertyType == typeof(int?))
                {
                    parameters.Add(new QueryParameter(jsonPropertyName.Name, (int?)rawValue));
                }
                else if (updateablePropertyType == typeof(decimal) || updateablePropertyType == typeof(decimal?))
                {
                    parameters.Add(new QueryParameter(jsonPropertyName.Name, (decimal?)rawValue));
                }
                else if (updateablePropertyType == typeof(DateTimeOffset) || updateablePropertyType == typeof(DateTimeOffset?))
                {
                    parameters.Add(new QueryParameter(jsonPropertyName.Name, (DateTimeOffset?)rawValue));
                }
                else if (updateablePropertyType == typeof(bool))
                {
                    parameters.Add(new QueryParameter(jsonPropertyName.Name, (bool?)rawValue));
                }
                else if (updateablePropertyType == typeof(List<string>))
                {
                    List<string> list = (List<string>)rawValue;
                    parameters.Add(list == null ? new QueryParameter(jsonPropertyName.Name, string.Empty) : new QueryParameter(jsonPropertyName.Name, list));
                }
                else if (updateablePropertyType.BaseType == typeof(Enum))
                {
                    parameters.Add(new QueryParameter(jsonPropertyName.Name, ((Enum)rawValue).GetJsonPropertyName()));
                }
                else
                {
                    throw new Exception($"Unsupported type of Property: {jsonPropertyName.Name} - Type: {updateablePropertyType}");
                }
            }

            return parameters.ToArray();
        }

        internal void AdjustForNamedPosition(IEnumerable<QueryParameter> parameters, NamedPosition? namedPosition)
        {
            if (!namedPosition.HasValue)
            {
                return;
            }

            QueryParameter parameter = parameters.FirstOrDefault(x => x.Name == "pos");
            if (parameter != null)
            {
                parameter.Type = QueryParameterType.String;
                switch (namedPosition.Value)
                {
                    case NamedPosition.Top:
                        parameter.SetRawValue("top");
                        break;
                    case NamedPosition.Bottom:
                        parameter.SetRawValue("bottom");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}