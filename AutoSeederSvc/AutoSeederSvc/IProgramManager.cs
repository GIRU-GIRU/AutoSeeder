using System.Collections.Generic;

namespace AutoSeederSvc
{
    public interface IProgramManager
    {
        bool CheckIfActiveProcesses();
        void ClosePrograms();
        void OpenPrograms(List<string> paths);
    }
}