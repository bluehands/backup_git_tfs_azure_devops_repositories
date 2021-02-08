namespace BackupRepositories.Services.Prtg
{
    public class PrtgRoot
    {
        public BackupRepositories.Services.Prtg.Prtg Prtg { get; }

        public PrtgRoot(BackupRepositories.Services.Prtg.Prtg prtg)
        {
            Prtg = prtg;
        }
    }
}