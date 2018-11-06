using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace ISBS_New.Methods
{
    class Validation : IDisposable
    {
        public bool ValValidasi = true;

        public System.Windows.Forms.Control Validasi(System.Windows.Forms.Control TmpControl)
        {
            TmpControl.BackColor = Color.White;
            if (TmpControl.GetType() == typeof(System.Windows.Forms.TextBox) && TmpControl.Text == String.Empty)
            {
                ValValidasi = false;
                TmpControl.BackColor = Color.LightPink;
                return TmpControl;
            }
            return TmpControl;
        }

        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
    }
}
