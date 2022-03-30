namespace AdCore.Helpers
{
    public class B2CCustomAttributeHelper
    {
        internal readonly string B2CExtensionAppClientId;

        public B2CCustomAttributeHelper(string b2CExtensionAppClientId)
        {
            B2CExtensionAppClientId = b2CExtensionAppClientId.Replace("-", "");
        }

        public string GetCompleteAttributeName(string attributeName)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new ArgumentException("Parameter cannot be null", nameof(attributeName));
            }

            return $"extension_{B2CExtensionAppClientId}_{attributeName}";
        }
    }
}
