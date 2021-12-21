using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoSeederSvc
{
    public static class InputChecker
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(int KeyAsciiId);

        public static Task<bool> CheckIfKeysAreBeingPressed()
        {
            for (int i = 32; i < 127; i++)
            {
                int keyState = InputChecker.GetAsyncKeyState(i);
                if (keyState != 0)
                {
                    return Task.FromResult(true);
                }

            }

            return Task.FromResult(false);
        }

  
    }
}
