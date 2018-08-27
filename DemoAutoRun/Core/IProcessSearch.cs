using System.Collections.Generic;


using DemoAutoRun.Models;

namespace DemoAutoRun.Core
{
    interface IProcessSearch
    {
        IEnumerable<AutoRunFileInfoDataModel> Start();
    }
}
