
//
// filemap.cs
//    
//    Implementation of a library to use Win32 Memory Mapped
//    Files from within .NET applications
//
// COPYRIGHT (C) 2001, Tomas Restrepo (tomasr@mvps.org)
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//

namespace Winterdom.IO
{
   using System;
   using System.IO;
   using System.Runtime.InteropServices;


/*
   //
   // NativePtr
   //
   /// <remarks>
   ///   Defines a simple structure that can be used in place of 
   ///   IntPtr to handle native pointers, but that also has the 
   ///   overloaded operators needed to make operations on it
   ///   </remarks>
   //
   internal struct NativePtr
   {
      public const NativePtr Zero = NativePtr(IntPtr.Zero);
      private IntPtr m_ptr = IntPtr.Zero;


      //
      // Constructors
      //
      public NativePtr ( IntPtr ptr ) {
         m_ptr = ptr;
      }
      public NativePtr ( int ptrValue ) {
         m_ptr = new IntPtr(ptrValue);
      }
      public NativePtr ( long ptrValue ) {
         m_ptr = new IntPtr(ptrValue);
      }

      //
      // properties
      //
      public int Value32 {
         get { return m_ptr.ToInt32(); }
      }
      public long Value64 
      {
         get { return m_ptr.ToInt64(); }
      }

      //
      // Overloaded operators
      //
      public static NativePtr operator + ( NativePtr lhs, NativePtr rhs ) 
      {
         return (new NativePtr(lhs.Value64 + rhs.Value64)); 
      }
      public static NativePtr operator - ( NativePtr lhs, NativePtr rhs ) 
      {
         return lhs + (-rhs);
      }
      public static NativePtr operator- ( NativePtr rhs ) 
      {
         return new NativePtr(-rhs.Value64);
      }



   } // struct NativePtr

*/


   //
   // Win32MapApis
   //
   /// <remarks>
   ///   Defines the PInvoke functions we use
   ///   to access the FileMapping Win32 APIs
   /// </remarks>
   //
   internal class Win32MapApis
   {
      [ DllImport("kernel32", SetLastError=true, CharSet=CharSet.Auto) ]
      public static extern IntPtr CreateFile ( 
         String lpFileName, int dwDesiredAccess, int dwShareMode,
         IntPtr lpSecurityAttributes, int dwCreationDisposition,
         int dwFlagsAndAttributes, IntPtr hTemplateFile );

      [ DllImport("kernel32", SetLastError=true, CharSet=CharSet.Auto) ]
      public static extern IntPtr CreateFileMapping ( 
         IntPtr hFile, IntPtr lpAttributes, int flProtect, 
         int dwMaximumSizeLow, int dwMaximumSizeHigh,
         String lpName );

      [ DllImport("kernel32", SetLastError=true) ]
      public static extern bool FlushViewOfFile ( 
         IntPtr lpBaseAddress, int dwNumBytesToFlush );

      [ DllImport("kernel32", SetLastError=true) ]
      public static extern IntPtr MapViewOfFile (
         IntPtr hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh,
         int dwFileOffsetLow, int dwNumBytesToMap );

      [ DllImport("kernel32", SetLastError=true, CharSet=CharSet.Auto) ]
      public static extern IntPtr OpenFileMapping (
         int dwDesiredAccess, bool bInheritHandle, String lpName );

      [ DllImport("kernel32", SetLastError=true) ]
      public static extern bool UnmapViewOfFile ( IntPtr lpBaseAddress );

      [ DllImport("kernel32", SetLastError=true) ]
      public static extern bool CloseHandle ( IntPtr handle );

   } // class Win32MapApis





   //
   // FileMapIOException
   //
   ///
   /// <exception cref="System.Exception">
   ///   Represents an exception occured as a result of an
   ///   invalid IO operation on any of the File mapping classes
   ///   It wraps the error message and the underlying Win32 error
   ///   code that caused the error.
   /// </exception>
   //
   public class FileMapIOException : IOException
   {
      //
      // properties
      //
      private int m_win32Error = 0;
      public int Win32ErrorCode
      {
         get { return m_win32Error; }
      }
      public override string Message
      {
         get 
         {
            if ( Win32ErrorCode != 0 )
               return base.Message + " (" + Win32ErrorCode + ")";
            else
               return base.Message;
         }
      }

      // construction
      public FileMapIOException ( int error ) : base()
      {
         m_win32Error = error;
      }
      public FileMapIOException ( string message ) : base(message)
      {
      }
      public FileMapIOException ( string message, Exception innerException ) 
               : base(message, innerException)
      {
      }
   } // class FileMapIOException();



   //
   // MapProtection
   // 
   /// <remarks>
   ///   Specifies page protection for the mapped file
   ///   These correspond to the PAGE_XXX set of flags
   ///   passed to CreateFileMapping()
   /// </remarks>
   //
   [ Flags ]
   public enum MapProtection 
   {
      PageNone       = 0x00000000,
      // protection
      PageReadOnly   = 0x00000002,
      PageReadWrite  = 0x00000004,
      PageWriteCopy  = 0x00000008,
      // attributes
      SecImage       = 0x01000000,
      SecReserve     = 0x04000000,
      SecCommit      = 0x08000000,
      SecNoCache     = 0x10000000,
   }


   //
   // MapViewStream
   //
   /// <remarks>
   ///   Allows you to read/write from/to
   ///   a view of a memory mapped file.
   /// </remarks>
   //
   public class MapViewStream : Stream
   {
      // 
      // data members
      //

      //! What's our access?
      MapProtection m_protection = MapProtection.PageNone;
      //! base address of our buffer
      IntPtr m_base = IntPtr.Zero;
      //! our current buffer length
      long m_length = 0;
      //! our current position in the stream buffer
      long m_position = 0;
      //! are we open?
      bool m_isOpen = false;

      //
      // Construction / Destruction
      //
      internal MapViewStream ( IntPtr baseAddress, long length, 
                               MapProtection protection )
      {
         m_base = baseAddress;
         m_length = length;
         m_protection = protection;
         m_position = 0;
         m_isOpen = (baseAddress.ToInt64() > 0);
      }
      ~MapViewStream()
      {
         this.Close();
      }


      // 
      // Stream properties
      //

      public override bool CanRead 
      {
         get { return true; }
      }
      public override bool CanSeek 
      {
         get { return true; }
      }
      public override bool CanWrite 
      {
         get { return (((int)m_protection) & 0x000000C) != 0; }
      }
      public override long Length 
      {
         get { return m_length; }
      }
      public override long Position 
      {
         get { return m_position; }
         set {
            // does it fit within our buffer limits?
            // according to stream docs, we should allow
            // a position set to at least Length+1
            if ( value < this.Length  ) 
               m_position = value;
            else 
               throw new FileMapIOException ( "Invalid Position" );
         }
      }
      private bool IsOpen 
      {
         get { return m_isOpen; }
         set { m_isOpen = value; }
      }


      //
      // Stream members
      //

      public override void Flush() 
      {
         if ( !IsOpen ) 
            throw new FileMapIOException ( "Stream is closed" );
         // flush the view but leave the buffer intact
         // FIX: get rid of cast
         Win32MapApis.FlushViewOfFile ( m_base, (int)m_length );
      }

      public override int Read ( byte[] buffer, int offset, int count )
      {
         if ( !IsOpen ) 
            throw new FileMapIOException ( "Stream is closed" );

         if ( buffer.Length - offset < count )
            throw new ArgumentException ( "Invalid Offset" );
      
         int i;
         for ( i=0; (i < (Length-m_position)) && (i < count); i++ )
         {

            buffer[offset+i] = Marshal.ReadByte ( (IntPtr)(m_base.ToInt64()+m_position+i) );
         }
         m_position += i;
         return i;
      }
    
      public override void Write ( byte[] buffer, int offset, int count ) 
      {
         if ( !this.IsOpen || !this.CanWrite ) 
            throw new FileMapIOException ( "Stream cannot be written to" );

         if ( buffer.Length - offset < count )
            throw new ArgumentException ( "Invalid Offset" );
      
         int i;
         for ( i=0; (i < (Length-m_position)) && (i < count); i++ )
         {
            //! FIX: get rid of cast
            Marshal.WriteByte ( (IntPtr)(m_base.ToInt64()+m_position+i), buffer[offset+i] );
         }
         m_position += i;
      }

      public override long Seek ( long offset, SeekOrigin origin )
      {
         if ( !IsOpen ) 
            throw new FileMapIOException ( "Stream is closed" );

         long newpos = 0;
         switch ( origin )
         {
         case SeekOrigin.Begin:    newpos = offset;             break;
         case SeekOrigin.Current:  newpos = Position + offset;  break;
         case SeekOrigin.End:      newpos = Length - offset;    break;
         }
         // sanity check
         if ( newpos < 0 || newpos > Length )
            throw new FileMapIOException ( "Invalid Seek Offset" );
         m_position = newpos;

         return newpos;
      }

      public override void SetLength ( long value )
      {
         // not supported!
         throw new NotSupportedException ( "Can't change View Length" );
      }

      public override void Close()
      {
         if ( !IsOpen ) 
         {
            Flush();
            // FIX: eliminate cast
            Win32MapApis.UnmapViewOfFile ( m_base );
            IsOpen = false;
         }
      }

   } // class MapViewStream



   //
   // MapAccess
   // 
   /// <remarks>
   ///   Specifies access for the mapped file.
   ///   These correspond to the FILE_MAP_XXX
   ///   constants used by MapViewOfFile[Ex]()
   /// </remarks>
   //
   public enum MapAccess 
   {
      FileMapCopy       = 0x0001,
      FileMapWrite      = 0x0002,
      FileMapRead       = 0x0004,
      FileMapAllAccess  = 0x001f,
   }
  
 

   //
   // MemoryMappedFile
   //
   /// <remarks>
   ///    Allows you to easily use memory mapped files on
   ///    .NET applications.
   ///    Currently, not all functionality provided by 
   ///    the Win32 system is avaliable. Things that are not 
   ///    supported include:
   ///    <list>
   ///       <item>You can't specify security descriptors</item>
   ///       <item>You can't build the memory mapped file
   ///           on top of a System.IO.File already opened</item>
   ///    </list>
   ///    The class is currently MarshalByRefObject, but I would
   ///    be careful about possible interactions!
   /// </remarks>
   //
   public class MemoryMappedFile : MarshalByRefObject
   {
      //
      // variables
      //

      //! handle to MemoryMappedFile object
      private IntPtr m_hMap = IntPtr.Zero;

      //
      // Some constants needed
      //
      private const int GENERIC_READ  = unchecked((int)0x80000000);
      private const int GENERIC_WRITE = unchecked((int)0x40000000);
      private const int OPEN_ALWAYS   = 4;
      private readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
      private readonly IntPtr NULL_HANDLE = IntPtr.Zero;


      //
      // Construction/destruction
      // It would probably be interesting to implement
      // IDisposable!
      //
      public MemoryMappedFile()
      {
      }
      ~MemoryMappedFile()
      {
         this.Close();
      }


      //
      // Create()
      //
      /// <summary>
      ///   Create an unnamed map object 
      /// </summary>
      /// <param name="fileName">name of backing file, or 
      ///            null for a pagefile-backed map</param>
      /// <param name="protection">desired access to the 
      ///            mapping object</param>
      /// <param name="maxSize">maximum size of filemap 
      ///            object, or -1 for size of file</param>
      //
      public void Create ( String fileName, MapProtection protection, 
                           long maxSize ) 
      {
         Create ( fileName, protection, maxSize, null );
      }

      //
      // Create()
      //
      /// <summary>
      ///   Create a named map object 
      /// </summary>
      /// <param name="fileName">name of backing file, or null 
      ///            for a pagefile-backed map</param>
      /// <param name="protection">desired access to the mapping 
      ///            object</param>
      /// <param name="maxSize">maximum size of filemap object, or 0 
      ///            for size of file</param>
      /// <param name="name">name of file mapping object</param>
      //
      public void Create ( String fileName, MapProtection protection, 
                           long maxSize, String name ) 
      {
         // open file first
         IntPtr hFile = INVALID_HANDLE_VALUE;

         if ( fileName != null )
         {
            // determine file access needed
            // we'll always need generic read access
            int desiredAccess = GENERIC_READ;
            if  ( (protection == MapProtection.PageReadWrite) ||
                  (protection == MapProtection.PageWriteCopy) )
            {
               desiredAccess |= GENERIC_WRITE; 
            }

            // open or create the file
            // if it doesn't exist, it gets created
            hFile = Win32MapApis.CreateFile ( 
                        fileName, desiredAccess, 0, 
                        IntPtr.Zero, OPEN_ALWAYS, 0, IntPtr.Zero  
                     );
            if ( hFile == INVALID_HANDLE_VALUE )
               throw new FileMapIOException ( Marshal.GetHRForLastWin32Error() );

            // FIX: Is support needefor zero-length files!?!
         }

         m_hMap = Win32MapApis.CreateFileMapping (
                     hFile, IntPtr.Zero, (int)protection, 
                     (int)((maxSize >> 32) & 0xFFFFFFFF),
                     (int)(maxSize & 0xFFFFFFFF), name 
                  );

         // close file handle, we don't need it
         if ( !(hFile == NULL_HANDLE) ) Win32MapApis.CloseHandle(hFile);
         if ( m_hMap == NULL_HANDLE )
            throw new FileMapIOException ( Marshal.GetHRForLastWin32Error() );
      }

      //
      // Open()
      //
      /// <summary>
      ///   Open an existing named File Mapping object
      /// </summary>
      /// <param name="access">desired access to the map</param>
      /// <param name="name">name of object</param>
      //
      public void Open ( MapAccess access, String name )
      {
         m_hMap = Win32MapApis.OpenFileMapping ( (int)access, false, name );
         if ( m_hMap == NULL_HANDLE )
            throw new FileMapIOException ( Marshal.GetHRForLastWin32Error() );
      }

    

      //
      // Close()
      //
      /// <summary>
      ///   Close this File Mapping object
      ///   From here on, You can't do anything with it
      ///   but the open views remain valid.
      /// </summary>
      //
      public void Close()
      {
         if ( m_hMap != NULL_HANDLE )
            Win32MapApis.CloseHandle ( m_hMap );
         m_hMap = NULL_HANDLE;
      }


      //
      // MapView()
      //
      /// <summary>
      ///   Map a view of the file mapping object
      ///   This returns a stream, giving you easy access to the memory,
      ///   as you can use StreamReaders and writes on top of it
      /// </summary>
      /// <param name="access">desired access to the view</param>
      /// <param name="offset">offset of the file mapping object to 
      ///            start view at</param>
      /// <param name="size">size of the view</param>
      //
      public Stream MapView ( MapAccess access, long offset, int size )
      {
         IntPtr baseAddress = IntPtr.Zero;
         baseAddress = Win32MapApis.MapViewOfFile (
                           m_hMap, (int)access, 
                           (int)((offset >> 32) & 0xFFFFFFFF),
                           (int)(offset & 0xFFFFFFFF), 0);

         if ( baseAddress == IntPtr.Zero )
            throw new FileMapIOException ( Marshal.GetHRForLastWin32Error() );

         // Find out what MapProtection to use
         // based on the MapAccess flags...
         MapProtection protection;
         if ( access == MapAccess.FileMapRead )
            protection = MapProtection.PageReadOnly;
         else
            protection = MapProtection.PageReadWrite;

         return new MapViewStream ( baseAddress, size, protection );
      }
      
   }  // class MemoryMappedFile


} // namespace Winterdom.IO
