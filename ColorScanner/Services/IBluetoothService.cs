using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColorScanner.Services
{
    public interface IBluetoothService
    {
        void Start(string name, int sleepTime, Func<string, Task> resultCallback);
        void Cancel();
        Task<IEnumerable<string>> GetPairedDevices();
    }
}
