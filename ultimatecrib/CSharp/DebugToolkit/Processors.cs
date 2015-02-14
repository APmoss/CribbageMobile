// Debug Toolkit

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
using System.Collections;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Processors
{
	/// <summary>
	/// Provides access to CPU details
	/// </summary>
   public class Processor
   {
      #region SDK definitions
      // defines possible cpu architecure constants as provided by Platform SDK
      public enum ARCHITECTURES
      {
         Unknown = 0xFFFF,
         Intel = 0,
         MIPS = 1,
         Alpha = 2,
         PPC = 3,
         SHX = 4,
         ARM = 5,
         IA64 = 6,
         Alpha64 = 7,
         MSIL = 8,
         AMD64 = 9,
         IA32_on_WIN64 = 10,

         Intel386 = 386,
         Intel486 = 486,
         Pentium = 586,
         IntelIA64 = 2200,
         MIPS_R4000 = 4000,    // incl R4101 & R3910 for Windows CE
         ALPHA_21064 = 21064,
         PPC601 = 601,
         PPC603 = 603,
         PPC604 = 604,
         PPC620 = 620,
         HITACHI_SH3 = 10003,   // Windows CE
         HITACHI_SH3E = 10004,   // Windows CE
         HITACHI_SH4 = 10005,   // Windows CE
         MOTOROLA_821 = 821,     // Windows CE
         SHx_SH3 = 103,     // Windows CE
         SHx_SH4 = 104,     // Windows CE
         STRONGARM = 2577,    // Windows CE - 0xA11
         ARM720 = 1824,    // Windows CE - 0x720
         ARM820 = 2080,    // Windows CE - 0x820
         ARM920 = 2336,    // Windows CE - 0x920
         ARM_7TDMI = 70001,   // Windows CE
         OPTIL = 0x494f  // MSIL
      };

      // defines the possible Intel processor levels as defined by the platform SDK
      public enum INTELLEVELS
      {
         i80386 = 3,
         i80486 = 4,
         Pentium = 5,
         PentiumII_or_PentiumPro = 6,
         Unknown = 0xFF
      };

      // defines the possible Alpha processor levels as defined by the platform SDK
      public enum ALPHALEVELS
      {
         a21064 = 21064,
         a21066 = 21066,
         a21164 = 21164
      };

      // defines the possible MIPS processor levels as defined by the platform SDK
      public enum MIPSLEVELS
      {
         MIPS_R4000 = 0004
      };

      // defines the possible Power PC processor levels as defined by the platform SDK
      public enum PPCLEVELS
      {
         PPC601 = 1,
         PPC603 = 3,
         PPC604 = 4,
         PPC603_Plus = 6,
         PPC604_Plus = 9,
         PPC620 = 20
      };

      // structure for getting system info
      struct SYSTEM_INFO
      {
         public Int16 processorArchitecture;
         public Int16 reserved;
         public Int32 pageSize;
         public Int32 pMinimumApplicationAddress;
         public Int32 pMaximumApplicationAddress;
         public Int32 pActiveProcessorMask;
         public Int32 numberOfProcessors;
         public Int32 processorType;
         public Int32 allocationGranularity;
         public Int16 processorLevel;
         public Int16 processorRevision;
      };

      // get system info api definition
      [ DllImport("kernel32", SetLastError=true) ]
      static extern void GetSystemInfo(ref SYSTEM_INFO lpSystemInfo);

      [ DllImport("kernel32", SetLastError=true) ]
      static extern int IsProcessorFeaturePresent(Int32 ProcessorFeature);

      #endregion

      #region public static functions

      /// <summary>
      /// Gets a collection of processor objects for each processor in the machine
      /// </summary>
      /// <returns>Collection of processor objects</returns>
      public static ArrayList GetProcessors()
      {
         // create a list to return
         ArrayList processors = new ArrayList();

         // call the get system info api
         SYSTEM_INFO si = new SYSTEM_INFO();
         GetSystemInfo(ref si);

         // for each processor found
         for (int i = 0; i < si.numberOfProcessors; i++)
         {
            // add a new processor object
            processors.Add(new Processor(i, si));
         }

         // return the list
         return processors;
      }
      #endregion

      #region member variables
      int number = -1; // the processor number
      long speed = 0; // the processor speed
      string vendor = "Unknown"; // the processor vendor
      ARCHITECTURES architecture = ARCHITECTURES.Unknown; // the processor architecture
      int level = (int)INTELLEVELS.Unknown; // the processor level
      string stepping = "Unkown"; // the processor stepping
      bool mmx = false; // mmx feature flag

      #endregion

      #region Constructors

      /// <summary>
      /// Creates a processor object based on the provided system info
      /// </summary>
      /// <param name="i">The processor number</param>
      /// <param name="si">The system info</param>
		Processor(int i, SYSTEM_INFO si)
		{
         // save the processor number
         number = i;

         // get the speed from the registry
         RegistryKey processorKey = 
Registry.LocalMachine.OpenSubKey("Hardware\\Description\\System\\CentralProcessor\\" 
+ i.ToString());
         speed = (Int32)processorKey.GetValue("~MHz");

         // get the vendor from the registry
         try
         {
            vendor = (string)processorKey.GetValue("VendorIdentifier");
         }
         catch
         {
            // leave vendor as it was
         }

         // if we are running on NT
         if (Environment.OSVersion.Platform == PlatformID.Win32NT)
         {
            // save the architecture and level
            architecture = (ARCHITECTURES)si.processorArchitecture;
            level = si.processorLevel;

            int iModel;
            int iSteppingLevel;
            int iStepping;

            // the rest depends on the architecture
            switch(architecture)
            {
               case ARCHITECTURES.Intel:

                  if (level == 3 || level == 4)
                  {
                     iSteppingLevel = (int)si.processorRevision / 100;
                     iStepping = (int)si.processorRevision % 100;
                     stepping = (char)iSteppingLevel + iStepping.ToString();
                  }
                  else
                  {
                     iModel = (int)si.processorRevision / 100;
                     iStepping = (int)si.processorRevision % 100;
                     stepping = iModel.ToString() + "-" + 
iStepping.ToString();
                  }

                  if (level == 5)
                  {
                     if (IsProcessorFeaturePresent(3) != 0)
                     {
                        mmx = true;
                     }
                  }
                  break;

               case ARCHITECTURES.MIPS:

                  stepping = "00" + si.processorRevision;
                  break;

               case ARCHITECTURES.Alpha:

                  iModel = (int)si.processorRevision / 100;
                  iStepping = (int)si.processorRevision % 100;
                  stepping = (char)iModel + iStepping.ToString();
                  break;

               case ARCHITECTURES.PPC:
                  iModel = (int)si.processorRevision / 100;
                  iStepping = (int)si.processorRevision % 100;
                  stepping = iModel.ToString() + "." + iStepping.ToString();
                  break;

               case ARCHITECTURES.Unknown:
                  iModel = (int)si.processorRevision / 100;
                  iStepping = (int)si.processorRevision % 100;
                  stepping = iModel.ToString() + "-" + iStepping.ToString();
                  break;

               default:
                  iModel = (int)si.processorRevision / 100;
                  iStepping = (int)si.processorRevision % 100;
                  stepping = iModel.ToString() + "-" + iStepping.ToString();
                  break;
            }

         }
         else
         {
            // Win9x uses a different mechanism
            architecture = (ARCHITECTURES)si.processorType;
         }
		}

      #endregion

      #region public functions

      #region Accessors

      /// <summary>
      /// Get the processor speed int MHz
      /// </summary>
      public long SpeedMHz
      {
         get
         {
            return speed;
         }
      }

      /// <summary>
      /// Get the processor vendor name
      /// </summary>
      public string Vendor
      {
         get
         {
            return vendor;
         }
      }

      /// <summary>
      /// Get the processors architecture
      /// </summary>
      public ARCHITECTURES Architecture
      {
         get
         {
            return architecture;
         }
      }

      /// <summary>
      /// Get the processor level
      /// </summary>
      public int Level
      {
         get
         {
            return level;
         }
      }

      /// <summary>
      /// check the presence of mmx
      /// </summary>
      public bool IsMMX
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      /// <summary>
      /// Get the processor level
      /// </summary>
      public string LevelName
      {
         get
         {
            switch(architecture)
            {
               case ARCHITECTURES.Intel:
                  return ((INTELLEVELS)level).ToString();
               case ARCHITECTURES.MIPS:
                  return ((MIPSLEVELS)level).ToString();
               case ARCHITECTURES.Alpha:
                  return ((ALPHALEVELS)level).ToString();
               case ARCHITECTURES.PPC:
                  return ((PPCLEVELS)level).ToString();
               default:
                  return level.ToString();
            }
         }
      }

      /// <summary>
      /// Get the processor stepping
      /// </summary>
      public string Stepping
      {
         get
         {
            return stepping;
         }
      }

      #endregion

      /// <summary>
      /// Get a description of the processor
      /// </summary>
      /// <returns>The processor description</returns>
      public override string ToString()
      {
         return "Processor #" + number.ToString() +
                ", Vendor " + vendor +
                ", Architecture " + architecture.ToString() +
                ", Level " + LevelName + (mmx?"MMX":"") +
                ", Stepping " + stepping +
                ", Speed " + speed.ToString()+ "MHz";
      }
      #endregion
	};
}

