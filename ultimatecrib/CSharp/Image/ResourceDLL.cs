// Ultimate Cribbage
// Image Assembly

// Copyright (C) 2003 - Keith Westley <keithsw1111@hotmail.com>

// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Runtime.InteropServices; 
using System.Drawing;

namespace Image
{
   /// <summary>
	/// ResourceDLL is used to read resources out of application DLLs.
	/// This code uses native Win32 APIs
	/// </summary>
	public class ResourceDLL : IDisposable
	{
      #region Windows Function Declarations
      // declare Win32 functions I need
      [DllImport("kernel32.dll")]
      static extern IntPtr LoadLibrary(string c);
      [DllImport("kernel32.dll")]
      static extern IntPtr FreeLibrary(IntPtr hLib);
      [DllImport("user32.dll")]
      static extern IntPtr LoadBitmap(IntPtr hInstance, IntPtr i);
      [DllImport("gdi32.dll")]
      static extern IntPtr DeleteObject (IntPtr hBmp);
      #endregion

      #region Member Variables
      IntPtr _dllHandle; // handle to DLL
      #endregion

      #region Constructors
      /// <summary>
      /// constructor which also loads the dll
      /// </summary>
      /// <param name="DLL">DLL to load</param>
		public ResourceDLL(string DLL)
		{
         // load the dll.
         _dllHandle = LoadLibrary(DLL);

         // if the dll failed to load
         if (_dllHandle == new IntPtr(0))
         {
            // throw an exception
            throw new ApplicationException("Could not load library " + DLL);
         }
		}
      #endregion

      #region IDisposable
      /// <summary>
      /// Dispose resources used by this object
      /// </summary>
      public void Dispose()
      {
         // if we have a handle to the library
         if (_dllHandle != new IntPtr(0))
         {
            // free it
            FreeLibrary(_dllHandle);
            _dllHandle = new IntPtr(0);
         }

         // No need to call the destructor now
         GC.SuppressFinalize(this);
      }
      #endregion

      #region Destructor
      // destructor which unloads the library
      ~ResourceDLL()
      {
         Dispose();
      }
      #endregion

      #region Public Member Functions

      /// <summary>
      /// Extract a bitmap resource
      /// </summary>
      /// <param name="ResourceId">Resource to extract</param>
      /// <returns></returns>
      public Bitmap ExtractBitmap(int ResourceId)
      {
         // if we have a library handle
         if (_dllHandle != new IntPtr(0))
         {
            // convert the integer parameter to a resource pointer
            IntPtr ip = new IntPtr(ResourceId);

            // load the bitmap
            IntPtr bitmapHandle = LoadBitmap(_dllHandle, ip);  

            // if we got a bitmap
            if (bitmapHandle != new IntPtr(0))
            {
               // extract it to a managed bitmap
               Bitmap RC = Bitmap.FromHbitmap(bitmapHandle);

               // release the bitmap
               DeleteObject(bitmapHandle);

               // return the bitmap
               return RC;
            }
         }

         // return null if we cannot load it
         return null;
      }
      #endregion
	}
}
