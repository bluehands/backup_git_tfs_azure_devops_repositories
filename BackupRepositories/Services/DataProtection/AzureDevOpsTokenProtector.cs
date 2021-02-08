using Microsoft.AspNetCore.DataProtection;

namespace BackupRepositories.Services.DataProtection
{
    public class AzureDevOpsTokenProtector : IDataProtector
    {
        private readonly IDataProtector dataProtector;

        public AzureDevOpsTokenProtector(IDataProtectionProvider dataProtectionProvider)
        {
            dataProtector = dataProtectionProvider.CreateProtector("AzureDevOpsTokenProtection");
        }

        public IDataProtector CreateProtector(string purpose)
        {
            return dataProtector.CreateProtector(purpose);
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return dataProtector.Unprotect(protectedData);
        }

        public byte[] Protect(byte[] plaintext)
        {
            return dataProtector.Protect(plaintext);
        }
    }
}