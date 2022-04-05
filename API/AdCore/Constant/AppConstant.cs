namespace AdCore.Constant
{
    public static class AppConstant
    {
        public static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        public static string AdB2CRoleClaimsName = "extension_Role";
    }
}
