using AdCore.Helpers;
using Microsoft.Graph;

namespace AdRepository.Authentication
{
    public static class ExtensionMethod
    { 
        public static bool IsInRole(this User user, string extensionCLintId, string role)
        {
            B2CCustomAttributeHelper helper = new B2CCustomAttributeHelper(extensionCLintId);
            var roleAttributeName = helper.GetCompleteAttributeName("Role");
            if (user.AdditionalData.TryGetValue(roleAttributeName, out var roleValue))
            {
                if (roleValue.ToString() == role)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
