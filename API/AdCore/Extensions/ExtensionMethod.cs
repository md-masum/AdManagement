using AdCore.Entity;

namespace AdCore.Extensions
{
    public static class ExtensionMethod
    {
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static void UpdateObjectWithoutId<TFirst, TSecond>(this TFirst obj1, TSecond obj2)
        {
            foreach (var property in obj2?.GetType().GetProperties()!)
            {
                if (property.Name == nameof(BaseEntity.Id)) continue;
                obj1?.GetType().GetProperty(property.Name)?.SetValue(obj1, property.GetValue(obj2));
            }

        }
        public static void UpdateObject<TFirst, TSecond>(this TFirst obj1, TSecond obj2)
        {
            foreach (var property in obj2?.GetType().GetProperties()!)
                obj1?.GetType().GetProperty(property.Name)?.SetValue(obj1, property.GetValue(obj2));
        }
    }
}
