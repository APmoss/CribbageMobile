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
using CircularFileStream;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;
using Processors;
using Microsoft.Win32;
using System.Reflection;
using System.Drawing;
using System.Drawing.Printing;
using System.Xml;
using System.Configuration;
using System.Runtime.InteropServices;

namespace DebugToolkit
{
   #region AssertStream internal class
   /// <summary>
   /// The assert stream object just monitors debug messages looking for 'Fail: '
   /// </summary>
   internal class AssertStream : Stream, IDisposable
   {
      // a reference to the class that we will call if we see an assert event
      DebugToolkitBase debugToolkit = null;

      #region constructors
      /// <summary>
      /// Constructs a stream to listen to debug messages looking for assert/fail events.
      /// <param name="DebugToolkit">A reference to the object to call when we see an assert.</param>
      /// </summary>
      public AssertStream(DebugToolkitBase DebugToolkit)
      {
         // save the reference
         debugToolkit = DebugToolkit;
      }
      #endregion

      #region IDisposable
      /// <summary>
      /// Dispose of resources used.
      /// </summary>
      public void Dispose()
      {
      }
      #endregion

      #region Stream
      /// <summary>
      /// Close the stream.
      /// </summary>
      public override void Close()
      {
         // Dispose of our resources
         Dispose();
      }
      /// <summary>
      /// Check if we can be read from.
      /// <returns><c>false</c> as you can't read from this stream.</returns>
      /// </summary>
      public override bool CanRead
      {
         get
         {
            return false;
         }
      }

      /// <summary>
      /// Check if we can seek to specific records in this stream.
      /// <returns><c>false</c> as you can't seek to different locations.</returns>
      /// </summary>
      public override bool CanSeek
      {
         get
         {
            return false;
         }
      }

      /// <summary>
      /// Check if we can be written to.
      /// <returns><c>true</c> as we can always be written to.</returns>
      /// </summary>
      public override bool CanWrite
      {
         get
         {
            return true;
         }
      }

      /// <summary>
      /// Flush any unwritten data to disk. This does nothing as data written to the stream is inspected and thrown away.
      /// </summary>
      public override void Flush()
      {
      }

      /// <summary>
      /// Get the length of the file.
      /// <returns><c>0</c> as we never hold onto anything.</returns>
      /// </summary>
      public override long Length
      {
         get
         {
            return 0;
         }
      }

      /// <summary>
      /// Get/set the current slot in the file.
      /// <returns><c>0</c> as there is never any data in the file.</returns>
      /// </summary>
      public override long Position
      {
         get
         {
            return 0;
         }
         set
         {
         }
      }

      /// <summary>
      /// Reads the next record
      /// <param name="buffer">The buffer in which to place the data</param>
      /// <param name="offset">This is ignored</param>
      /// <param name="count">This is ignored</param>
      /// <returns><c>0</c> as there is never any data to read.</returns>
      /// </summary>
      public override int Read(byte[] buffer, int offset, int count)
      {
         // return the length read
         return 0;
      }

      /// <summary>
      /// Accepts a record for writing. This will look to see if the record starts with 
      /// 'Fail: '. IF it does then we raise an asset event back to the debugtoolkit
      /// <param name="buffer">The buffer containing the data to write</param>
      /// <param name="offset">This is ignored</param>
      /// <param name="count">This is ignored</param>
      /// </summary>
      public override void Write(byte[] buffer, int offset, int count)
      {
         // convert it to a string
         MemoryStream ms = new MemoryStream(buffer, offset, count);
         StreamReader sr = new StreamReader(ms);
         string s = sr.ReadToEnd();

         // if it starts with 'Fail: ' then it is an assert
         // it is of course possible that the user just wrote this out but hey what is a man to do?
         if (s.StartsWith("Fail: "))
         {
            // we have a possible assert
            this.debugToolkit.OnInternalAssert(s);
         }
      }

      /// <summary>
      /// Seeks to a slot in stream
      /// <param name="position">The position relative to the <c>origin</c> to seek to</param>
      /// <param name="origin">The origin the position is relative to</param>
      /// <returns><c>0</c> as there is never any data in the stream.</returns>
      /// </summary>
      public override long Seek(long position, SeekOrigin origin)
      {
         return Position;
      }

      /// <summary>
      /// Allows you to specify the length of the file. This is not relevant so it is ignored.
      /// <param name="length">This parameter is ignored</param>
      /// </summary>
      public override void SetLength(long Length)
      {
      }
      #endregion
   }

   #endregion

   /// <summary>
   /// This is the main debug toolkit class. This class is abstract. You must derive 
   /// your own class from it and set the necessary options so it behaves the way you want
   /// it to.
   /// </summary>
   public abstract class DebugToolkitBase 
   {
      #region private member variables
      bool suspendHangDetect = false; // when true the automatic hang will ignore any hangs detected
      bool suspendUserHangDetect = false; // when true the user hang detect thread will ignore any user hangs reported
      Mutex mutexThreadProtect = new Mutex(false); // this mutex is used to protect members accessed from multiple threads
      DateTime timeLastVisit = DateTime.Now; // this remembers the last time a hang detect ping message was received
      string appName = "Undefined"; // this holds the name of the application
      static DebugToolkitBase __TheDebugToolkit = null; // this holds the static reference to the toolkit object
      ArrayList files = new ArrayList(); // this holds the list of files we should dump data on
      ArrayList registryEntries = new ArrayList(); // this holds the list of registry entries we should dump data on
      string startingWorkingDirectory = Environment.CurrentDirectory; // this holds the cwd at the time the toolkit was created
      string version = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location).FileVersion; // this holds the main application assembly version number
      Thread userHangThread = null; // this refers to the user hang detection thread
      Thread hangThread = null; // this refers to the automatic hang detection thread
      HangMonitorForm hangMonitorForm = null; // this refers to the hidden automatic hang detection form
      Thread mainThread = Thread.CurrentThread; // this refers to the applications main thread
      ArrayList threads = new ArrayList(); // this holds a list of registered threads for dumping
      ArrayList perfCounters = new ArrayList(); // list of performance counters to dump
      bool recreateCounters = false; // true if counters are to be deleted and recreated
      #endregion

      #region protected member variables
      // these should be set in the derived class to get the desired dumping behaviour

      #region event dump controls
      // these control what is dumped when an exception/assert/hang event occurs
      protected bool dumpThreads = true; // when true dumps details of this processes threads
      protected bool dumpCallStack  = true; // when true dumps the current threads call stack
      protected bool dumpTasks = true; // when true dumps all the tasks running on the machine
      protected bool dumpResources = true; // when true dumps details of resources available on the machine
      protected bool dumpModulesForAllTasks = false; // when true dumps all the dlls loaded by all tasks on the machine
      protected bool dumpPerfCounters = true; // when true dumps internal and provided performance counters
      #endregion
      
      #region file save controls
      // these control what is placed into a log file when a save is requested
      protected bool dumpFiles = true; // when true dumps the file details
      protected bool dumpRegistry = true; // when true dumps the registry settings
      protected bool dumpCPU = true; // when true dumps the machines CPU details
      protected bool dumpWindowsVersion = true; // when true dumps the OS details
      protected bool dumpCWD = true; // when true dumpes the working directory details
      protected bool dumpCommandLine = true; // when true dumps the command line used to start the application
      protected bool dumpOptions = true; // when true dumps options for this program
      protected bool dumpPrinter = true; // when true dumps details of printers defined on the computer
      protected bool dumpConfig = true; // when true dumps the applications .config appSettings
      protected bool dumpAssemblies = true; // when true dumps details of the assemblies in this process
      protected bool dumpAppDomain = true; // when true dumps the AppDomain object
      protected bool dumpApplication = true; // when true drumps the Application object
      #endregion

      #region what to monitor
      // what to monitor
      protected bool userHangDetect = true; // if true monitors for users telling us that the program has hung
      protected bool hangDetect = true; // if true monitors hangs automatically
      protected bool assertDetect = true; // if true detects assertions
      protected bool exceptionDetect = true; // if true detects exceptions
      protected bool abnormalExitDetect = true; // if true reports prior runs which did not exit normally
      protected bool keepPerfCounters = true; // if true code tracks events with performance counters

      protected bool logTrace = true; // if true keeps a circular log of trace output
      protected int logSize = 500; // number of entries to keep in trace output log file
      protected int hangPingInterval = 30000; // milliseconds between automatic hang checks
      protected int userHangPingInterval = 5000; // milliseconds between user hang checks
      #endregion

      #endregion

      #region overridable functions
      // *******************************************
      // functions to be overridden in derived class
      // *******************************************

      /// <summary>
      /// Called when an assert occurs
      /// </summary>
      /// <param name="assertMessage">The text of the assert message</param>
      virtual protected void OnAssert(string assertMessage)
      {
         // we don't do anything but our derived classes may want to
      }

      /// <summary>
      /// called when an exception occurs
      /// </summary>
      /// <param name="e">The exception that occurred</param>
 
      virtual protected void OnException(Exception e)
      {
         // we don't do anything but our derived classes may want to
      }

      /// <summary>
      /// called to see if you want to handle the hang automatically detected
      /// </summary>
      /// <returns>true if you want the toolkit to dump hang details. false to ignore</returns>
      virtual protected bool OnHang()
      {
         // By default we always handle hangs
         return true;
      }

      /// <summary>
      /// called once the hang has been accepted and handled.
      /// </summary>
      virtual protected void OnEndHang() 
      {
         // we don't do anything but our derived classes may want to
      }

      /// <summary>
      /// called to see if you want to handle the user reported hang 
      /// </summary>
      /// <returns>true if you want the toolkit to dump hang details. false to ignore</returns>
      virtual protected bool OnUserHang()
      {
         // By default we always handle user hangs
         return true;
      }

      /// <summary>
      /// called once the user reported hang has been accepted and handled.
      /// </summary>
      virtual protected void OnEndUserHang() 
      {
         // we don't do anything but our derived classes may want to
      }

      /// <summary>
      /// called to allow you to do some processing when we detect the program did not exit 
      /// normally last time it was run
      /// </summary>
      virtual protected void OnAbnormalExit()
      {
         // we don't do anything but our derived classes may want to
      }

      /// <summary>
      /// called when an assembly could not be loaded.
      /// </summary>
      /// <param name="file">Name of the assembly that could not be loaded</param>
      virtual protected void OnTestAssemblyFail(string file)
      {
         // we don't do anything but our derived classes may want to
      }

      /// <summary>
      /// called when a file could not be opened.
      /// </summary>
      /// <param name="file">Name of file that could not be opened.</param>
      virtual protected void OnTestFileFail(string file)
      {
         // we don't do anything but our derived classes may want to
      }

      /// <summary>
      /// allows you to provide additional data to write to the trace log when 
      /// a hang, exception or assert event occurs. You may wish to dump all the programs objects
      /// </summary>
      /// <returns>The string you want to include when the event occurs in the trace log</returns>
      virtual protected string GetDumpOtherAtEvent() 
      {
         return "";
      }

      /// <summary>
      /// allows you to provide additional data to write to the log file when it is being 
      /// written to disk
      /// </summary>
      /// <returns>The string you want to include in the log file when it is created.</returns>
      virtual protected string GetDumpOtherAtSave() 
      {
         return "";
      }

      /// <summary>
      /// allows you to dump in human readable form the options currently in effect
      /// </summary>
      /// <returns>The options currently in effect.</returns>
      virtual protected string GetOptionDescriptions() 
      {
         return "\r\nNo Options Implemented\r\n";
      }

      /// <summary>
      /// allows you to dump the options in machine readable (but text) form
      /// </summary>
      /// <returns>The options in machine readable form.</returns>
      virtual protected string ExportOptions() 
      {
         // we don't have anything
         return "";
      }
      #endregion

      #region constructor
      protected DebugToolkitBase()
      {
         // check there is only one instance in this executable;
         if (DebugToolkitBase.__TheDebugToolkit != null)
         {
            throw new ApplicationException("Only one debug toolkit can exist");
         }
         else
         {
            DebugToolkitBase.__TheDebugToolkit = this;
         }
      }
      #endregion

      #region Private Functions

      #region Thread Procs

      /// <summary>
      /// user hang thread function. Detects when the user presses the hang keys
      /// </summary>
      void UserHangThreadProc()
      {
         // register this thread for dumping
         DebugToolkitBase.TheDebugToolkit.RegisterThread(Thread.CurrentThread, "DebugToolkit User Hang Detection Thread");

         // get the check frequency
         int sleep = DebugToolkitBase.TheDebugToolkit.UserHangPingInterval;

         // try loop catches the exception when the thread is aborted
         try
         {
            // forever
            while (true)
            {
               // sleep for the required period
               Thread.Sleep(sleep);

               // if user hang detection is not suspended and <CTRL><SHIFT><ALT> is depressed 
               // simultaneously then the user thinks it has hung
               if (!DebugToolkitBase.TheDebugToolkit.SuspendUserHang && (Control.ModifierKeys & Keys.Control) == Keys.Control && (Control.ModifierKeys & Keys.Alt) == Keys.Alt && (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
               {
                  // suspend the main thread
                  DebugToolkitBase.TheDebugToolkit.Thread.Suspend();

                  // call our method for handling the hang
                  DebugToolkitBase.TheDebugToolkit.HandleUserHang();

                  // resume the main thread
                  DebugToolkitBase.TheDebugToolkit.Thread.Resume();
               }
            }
         }
            // catch when this thread is being aborted
         catch(System.Threading.ThreadAbortException e)
         {
            // ignore this event
         }
         catch(Exception e)
         {
            // a different exception occured. This is bad so pass it on
            throw new ApplicationException("Exception occurred in user hang monitor thread", e);
         }
      }

      /// <summary>
      /// Automatic hang detection thread
      /// </summary>
      void HangThreadProc()
      {
         // register this thread so it can be dumped
         DebugToolkitBase.TheDebugToolkit.RegisterThread(Thread.CurrentThread, "DebugToolkit Automatic Hang Detection Thread");

         // get the check frequency
         int sleep = DebugToolkitBase.TheDebugToolkit.HangPingInterval;

         // try loop catches the exception when the thread is aborted
         try
         {
            // forever
            while (true)
            {
               // sleep for the required amount of time
               Thread.Sleep(sleep);
      
               // calculate the amount of time since we last updated our visit time
               TimeSpan delay = DateTime.Now - DebugToolkitBase.TheDebugToolkit.LastVisitTime;

               // if it has been too long (2 times the sleep time) and hand detection has not been suspended
               if ((long)delay.TotalMilliseconds > sleep * 2 && !DebugToolkitBase.TheDebugToolkit.SuspendHang)
               {
                  // ask the user to confirm that a hang has occured
                  if (MessageBox.Show("It appears as if " + DebugToolkitBase.TheDebugToolkit.AppName + " has hung. If you are confident this is the case then please press the YES button and I will kill it for you. If you don't think it has hung or if you are not sure press NO.", "Hung?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                  {
                     // suspend the main thread
                     DebugToolkitBase.TheDebugToolkit.Thread.Suspend();

                     // call our method for handling the hang
                     DebugToolkitBase.TheDebugToolkit.HandleHang();

                     // resume the main thread
                     DebugToolkitBase.TheDebugToolkit.Thread.Resume();

                     // now kill the entire task
                     Process.GetCurrentProcess().Kill();
                  }
               }
            }
         }
            // catch the exception generated when the thread is aborted
         catch(System.Threading.ThreadAbortException e)
         {
            // ignore this
         }
         catch(Exception e)
         {
            // this is bad so pass it on
            throw new ApplicationException("Exception occurred in automatic hang monitor thread", e);
         }
      }
      #endregion

      #region Event Handlers

      /// <summary>
      /// called when a thread exception occurs
      /// </summary>
      /// <param name="sender">sending object</param>
      /// <param name="e">thread exception parameters</param>
      void OnThreadingExceptionEvent(object sender, ThreadExceptionEventArgs e)
      {
         // pass it on to our generic exception handler
         HandleException(e.Exception, true);
      }

      /// <summary>
      /// called when an unhandled exception occurs
      /// </summary>
      /// <param name="sender">sending object</param>
      /// <param name="e">unhandled exception parameters</param>
      void OnUnhandledExceptionEvent(object sender, UnhandledExceptionEventArgs e)
      {
         // pass it on to our generic exception handler
         HandleException((Exception)e.ExceptionObject, true);
      }

      /// <summary>
      /// Dumps the appropriate data when an exception occurs
      /// </summary>
      /// <param name="e">The exception</param>
      /// <param name="tellOwner"><c>true</c> if we are to tell our derived class</param>
      void HandleException(Exception e, bool tellOwner)
      {
         // if we are to keep performance counters
         if (keepPerfCounters)
         {
            // increment the executions counter
            IncrementCounter("Exceptions");
         }


         // write out a header
         Trace.WriteLine("*** Exception Caught ... Exception Details Start");
         Trace.WriteLine("");

         // write out the exception data
         Trace.WriteLine("*** Exception data");
         Trace.WriteLine(DumpException(e, ""));

         // Dump the call stack now
         Trace.WriteLine(DumpCallStack());

         // Dump all executing tasks
         Trace.WriteLine(DumpTasks());

         // dump the threads
         Trace.WriteLine(DumpThreads());

         // dump resources
         Trace.WriteLine(DumpResources());

         // dump perfance counters
         Trace.WriteLine(DumpPerfCounters());

         // ask our derived class if they want to dump anything
         Trace.WriteLine("*** Application specific data on the exception");
         Trace.WriteLine(GetDumpOtherAtEvent());

         // if we are supposed to tell the owner
         if (tellOwner)
         {
            // call our derived class
            OnException(e);
         }

         // write the exception footer
         Trace.WriteLine("*** Exception Caught ... Exception Details End");
         Trace.WriteLine("");
      }

      /// <summary>
      /// Dump the appropriate data data when a user hang is detected.
      /// </summary>
      void HandleUserHang()
      {
         // ask our derived class if we should do this
         if (OnUserHang())
         {
            // if we are to keep performance counters
            if (keepPerfCounters)
            {
               // increment the executions counter
               IncrementCounter("User Hangs");
            }

            // write the user hang header record
            Trace.WriteLine("*** User Hang Detected ... Details Start");
            Trace.WriteLine("");

            // Dump the call stack now
            Trace.WriteLine(DumpCallStack());

            // Dump all executing tasks
            Trace.WriteLine(DumpTasks());

            // dump the threads
            Trace.WriteLine(DumpThreads());

            // dump resources
            Trace.WriteLine(DumpResources());

            // dump perfance counters
            Trace.WriteLine(DumpPerfCounters());

            // ask our derived class if they want to dump anything
            Trace.WriteLine("*** Application specific data on the user hang");
            Trace.WriteLine(GetDumpOtherAtEvent());

            // call our derived class so they can do stuff
            OnEndUserHang();

            // write the user hang footer record
            Trace.WriteLine("");
            Trace.WriteLine("*** User Hang Detected ... Details End");
         }
      }

      /// <summary>
      /// handles an auto detected hang
      /// </summary>
      void HandleHang()
      {
         // ask our derived class if we should do this
         if (OnHang())
         {
            // if we are to keep performance counters
            if (keepPerfCounters)
            {
               // increment the executions counter
               IncrementCounter("Automatic Hangs");
            }

            // write the automatic hang header record
            Trace.WriteLine("*** Automatic Hang Detected ... Details Start");
            Trace.WriteLine("");

            // Dump the call stack now
            Trace.WriteLine(DumpCallStack());

            // Dump all executing tasks
            Trace.WriteLine(DumpTasks());

            // dump the threads
            Trace.WriteLine(DumpThreads());

            // dump resources
            Trace.WriteLine(DumpResources());

            // dump perfance counters
            Trace.WriteLine(DumpPerfCounters());

            // ask our derived class if they want to dump anything
            Trace.WriteLine("*** Application specific data on the automatic hang");
            Trace.WriteLine(GetDumpOtherAtEvent());

            // call our derived class in case they want to dump anything
            OnEndHang();

            // write the automatic hang footer record
            Trace.WriteLine("");
            Trace.WriteLine("*** Automatic Hang Detected ... Details End");
         }
      }
      /// <summary>
      /// called when an assert occurs
      /// </summary>
      /// <param name="assertMessage">The message fished out of the trace messages</param>
      internal void OnInternalAssert(string assertMessage)
      {
         // if we are to keep performance counters
         if (keepPerfCounters)
         {
            // increment the executions counter
            IncrementCounter("Asserts");
         }

         // write the assert header record
         Trace.WriteLine("*** Assert Possibly Detected ... Details Start");
         Trace.WriteLine("");

         // re-write the message that makes us think we have an assert
         Trace.WriteLine("*** " + assertMessage);
         Trace.WriteLine("");

         // Dump the call stack now
         Trace.WriteLine(DumpCallStack());

         // Dump all executing tasks
         Trace.WriteLine(DumpTasks());

         // dump the threads
         Trace.WriteLine(DumpThreads());

         // dump resources
         Trace.WriteLine(DumpResources());

         // ask our derived class if they want to dump anything
         Trace.WriteLine("*** Application specific data on assert");
         Trace.WriteLine(GetDumpOtherAtEvent());

         // ask our derived class if they want to dump anything
         OnAssert(assertMessage);

         // write the assert footer record
         Trace.WriteLine("");
         Trace.WriteLine("*** Assert Possibly Detected ... Details End");
      }

      #endregion

      #region Dump Data

      /// <summary>
      /// Dump an exception
      /// </summary>
      /// <param name="e">The exception to dump</param>
      /// <param name="indent">The indentation to use when displaying the exception</param>
      /// <returns>The dumped data.</returns>
      string DumpException(Exception e, string indent)
      {
         // dump the exception data
         string s = "Exception " + e.Message + 
            ", Source " + e.Source + 
            ", Stack Trace " + e.StackTrace +
            ", Target Site " + e.TargetSite + "\r\n";

         // if there is an inner exception
         if (e.InnerException != null)
         {
            // recursively dump the exception
            s = s + DumpException(e.InnerException, indent + "   ");
         }

         // return the result
         return s;
      }

      /// <summary>
      /// Dump the value of internal and provided performance counters
      /// </summary>
      /// <returns>dumped data</returns>
      string DumpPerfCounters()
      {
         string s  = string.Empty;

         // if we are to dump performance counters
         if (dumpPerfCounters)
         {
            s = s + "=== Debug Log Statistics\r\n\r\n";

            if (keepPerfCounters)
            {
               s = s + "Exceptions " + GetCounter("Exceptions").ToString() + "\r\n";
               s = s + "Asserts " + GetCounter("Asserts").ToString() + "\r\n";
               s = s + "Abnormal Exits " + GetCounter("Abnormal Exits").ToString() + "\r\n";
               s = s + "Normal Exits " + GetCounter("Normal Exits").ToString() + "\r\n";
               s = s + "Excecutions " + GetCounter("Executions").ToString() + "\r\n";
               s = s + "User Hangs " + GetCounter("User Hangs").ToString() + "\r\n";
               s = s + "Automatic Hangs " + GetCounter("Automatic Hangs").ToString() + "\r\n";
            }

            s = s + "\r\n=== Performance Counters\r\n\r\n";

            foreach(string sc in perfCounters)
            {
               string category = sc.Substring(0, sc.IndexOf("##"));
               string counter = sc.Substring(sc.IndexOf("##")+2, sc.IndexOf("$$") - sc.IndexOf("##")-2);
               string instance = sc.Substring(sc.IndexOf("$$")+2);
               try
               {
                  PerformanceCounter pc = new PerformanceCounter(category, counter, true);
                  pc.InstanceName = instance;
                  pc.NextValue();
                  s = s + category + "." + counter + "." + instance + " = " + pc.NextValue().ToString() + "\r\n";
               }
               catch
               {
                  s = s + category + "." + counter + " NOT FOUND.\r\n";
               }
            }
         }

         return s;
      }

      /// <summary>
      /// dump the programs optins
      /// </summary>
      /// <returns>The options in a string</returns>
      string DumpOptions()
      {
         string s = string.Empty;

         // if we need to dump options
         if (dumpOptions)
         {
            s = "=== Program Options\r\n\r\n";

            // call our virtual method to get the option descriptions
            // write out the option descriptions
            s = s + GetOptionDescriptions();
         }

         return s;
      }

      /// <summary>
      /// Dump the details on the files
      /// </summary>
      /// <returns>string containing the file details</returns>
      string DumpFiles()
      {
         string s = string.Empty;

         // if we are to dump files
         if (dumpFiles)
         {
            s = "=== File Details\r\n\r\n";

            // add a header
            s = s + "Name\tCreated\tUpdated\tSize\tDirectory\tAttributes\tVersion\r\n\r\n";

            // for each file
            foreach (string file in files)
            {
               try
               {
                  // get the file info
                  FileInfo fi = new FileInfo(file);


                  string fileVersion = "";
                  try
                  {
                     fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(file).FileVersion;
                  }
                  catch
                  {
                  }

                  // write out the file info
                  s = s + fi.Name + "\t" + 
                     fi.CreationTime.ToString() + "\t" + 
                     fi.LastWriteTime.ToString() + "\t" + 
                     fi.Length.ToString() + "\t" + 
                     fi.DirectoryName + "\t" + 
                     fi.Attributes.ToString() + "\t" +
                     fileVersion + "\r\n";
               }
               catch
               {
                  // add a line with just the file name
                  s = s + file + "\r\n";
               }
            }
         }

         return s;
      }

      /// <summary>
      /// Dump the registry keys
      /// </summary>
      /// <returns>The dumped registry keys</returns>
      string DumpRegistry()
      {
         string s = string.Empty;

         // if we are to dump registry
         if (dumpRegistry)
         {
            s = "=== Registry Details\r\n\r\n";

            // list to put the keys we are to dump into
            ArrayList entries = new ArrayList();

            // expand registry list if there are wildcards
            foreach (string entry in registryEntries)
            {
               // if there is a wild card
               if (entry.IndexOf("\\*\\") != -1)
               {
                  // add it to an empty list
                  ArrayList work = new ArrayList();
                  work.Add(entry);

                  // expand it
                  ExpandRegistryWildcard(ref work);

                  // add it to our list of keys to dump
                  entries.AddRange(work);
               }
               else
               {
                  // add it to our list of keys to dump
                  entries.Add(entry);
               }
            }

            // dump each entry
            foreach (string entry in entries)
            {
               // dump it
               s = s + entry + "\r\n";
               s = s + DumpRegistryEntry(entry, "---");
            }
         }

         return s;
      }

      /// <summary>
      /// Dump a specific registry entry
      /// </summary>
      /// <param name="entry">The name of the key to dump</param>
      /// <param name="indent">The indent to use</param>
      /// <returns>The dumped data</returns>
      string DumpRegistryEntry(string entry, string indent)
      {
         // the return value
         string s = string.Empty;

         // get the top key
         RegistryKey key = GetTopKey(entry);

         // try loop ... we will assume it is a key. If it is a value we will get an exception
         try
         {

            // extract the sub key name
            string subKeyName = entry.Substring(entry.IndexOf("\\") +1);

            // open it
            RegistryKey subKey = key.OpenSubKey(subKeyName);

            // get the value names
            string[] valueNames = subKey.GetValueNames();

            // for each value found
            foreach(string valueName in valueNames)
            {
               // add it to our return value
               s = s + DumpRegistryEntry(entry + "\\" + valueName, indent);
            }

            // now get the sub keynames
            string[] keyNames = subKey.GetSubKeyNames();

            // for each subkey found
            foreach(string keyName in keyNames)
            {
               // add the key name & dump it
               s = s + indent + keyName + "\r\n";
               s = s + DumpRegistryEntry(entry + "\\" + keyName, indent + "---");
            }
         }
         catch
         {
            // must be a value object

            // extract the key name
            string keyName = entry.Substring(entry.IndexOf("\\")+1, entry.LastIndexOf("\\") - entry.IndexOf("\\") - 1);
            string valueName = entry.Substring(entry.LastIndexOf("\\")+1);

            // open the key
            RegistryKey subKey = key.OpenSubKey(keyName);
            
            // get the value
            object keyValue = null;
            try
            {
               keyValue = subKey.GetValue(valueName);
            }
            catch
            {
            }

            // dump the value
            s = s + indent + valueName + " : " + keyValue.GetType().ToString() + " : " + keyValue.ToString() + "\r\n";
         }

         return s;
      }

      /// <summary>
      /// Dump the circular trace log
      /// </summary>
      /// <returns>The trace log contents</returns>
      string DumpLog()
      {
         string s  = string.Empty;
         CircularFile cl = null;

         try
         {
            // open the circular log
            cl = new CircularFile(appName + ".dat");
            
            // read the first record
            string line = cl.ReadFirst();

            // read until I get an exception
            while (true)
            {
               // add it to our return string
               s = s + line;

               // read next line
               line = cl.ReadNext();
            }
         }
         catch
         {
            // file not there or end of file either way ignore it
         }

         // if file there
         if (cl != null)
         {
            // close it
            cl.Close();
         }

         return s;
      }

      /// <summary>
      /// Dump details of all printers
      /// </summary>
      /// <returns>dumped data</returns>
      string DumpPrinter()
      {
         string s  = string.Empty;

         // if we are to dump printers
         if (dumpPrinter)
         {
            s = "=== Printer Details\r\n\r\n";

            // for each installed printer
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
               // get the printer settings for the printer
               PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
               ps.PrinterName = printer;

               // dump it
               s = s + "Printer " + printer + 
                  ", Protrait " + (!ps.DefaultPageSettings.Landscape).ToString() + 
                  ", Default " + ps.IsDefaultPrinter.ToString() + 
                  ", Supports Colour "  + ps.SupportsColor.ToString() + 
                  ", Can Duplex " + ps.CanDuplex.ToString() + 
                  ", Collated " + ps.Collate.ToString() +
                  ", Copies " + ps.Copies.ToString() +
                  ", Duplex " + ps.Duplex.ToString() +
                  ", Plotter " + ps.IsPlotter.ToString() +
                  ", Print To File " + ps.PrintToFile.ToString() +
                  ", Print In Colour " + ps.DefaultPageSettings.Color.ToString() +
                  ", Margins " + ps.DefaultPageSettings.Margins.ToString() +
                  ", Bounds " + ps.DefaultPageSettings.Bounds.ToString() +
                  ", Paper Height Inches " + (ps.DefaultPageSettings.PaperSize.Height * 100).ToString() +
                  ", Paper Width Inches " + (ps.DefaultPageSettings.PaperSize.Width * 100).ToString() +
                  ", Paper Name " + ps.DefaultPageSettings.PaperSize.PaperName.ToString() +
                  ", Paper Source " + ps.DefaultPageSettings.PaperSource.SourceName +
                  ", Resolution DPI " + ps.DefaultPageSettings.PrinterResolution.X.ToString() + "x" + ps.DefaultPageSettings.PrinterResolution.Y.ToString() +
                  "\r\n";
            }
         }

         return s;
      }

      /// <summary>
      /// Dump the Application object
      /// </summary>
      /// <returns>dumped data</returns>
      string DumpApplication()
      {
         string s = "";

         // if we are to dump the application object
         if (dumpApplication)
         {
            s = "=== .NET Application Details\r\n\r\n";

            // dump the interesting application fields
            s = s + "Application Common App Data Path " + Application.CommonAppDataPath + "\r\n";
            s = s + "Application Common App Data Registry " + Application.CommonAppDataRegistry + "\r\n";
            s = s + "Application Company Name " + Application.CompanyName + "\r\n";
            s = s + "Application Current Culture " + Application.CurrentCulture + "\r\n";
            s = s + "Application Current Input Language " + Application.CurrentInputLanguage + "\r\n";
            s = s + "Application Executable Path " + Application.ExecutablePath + "\r\n";
            s = s + "Application Local User App Data Path " + Application.LocalUserAppDataPath + "\r\n";
            s = s + "Application Product Name " + Application.ProductName + "\r\n";
            s = s + "Application Product Version " + Application.ProductVersion + "\r\n";
            s = s + "Application Safe Top Level Caption Format " + Application.SafeTopLevelCaptionFormat + "\r\n";
            s = s + "Application Startup Path " + Application.StartupPath + "\r\n";
            s = s + "Application User App Data Path " + Application.UserAppDataPath + "\r\n";
            s = s + "Application User App Data Registry " + Application.UserAppDataRegistry + "\r\n";
         }

         return s;
      }

      /// <summary>
      /// Dump the AppDomain object
      /// </summary>
      /// <returns>Dumped data</returns>
      string DumpAppDomain()
      {
         string s = "";

         // if we are to dump the app domain
         if (dumpAppDomain)
         {
            s = "=== .NET Domain Details\r\n\r\n";

            // dump the interesting app domain fields
            s = s + "Base Directory " + AppDomain.CurrentDomain.BaseDirectory + "\r\n";
            s = s + "Dynamic Directory " + AppDomain.CurrentDomain.DynamicDirectory + "\r\n";
            s = s + "Friendly Name " + AppDomain.CurrentDomain.FriendlyName + "\r\n";
            s = s + "Relative Search Path " + AppDomain.CurrentDomain.RelativeSearchPath + "\r\n";
            s = s + "Setup Application Base " + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\r\n";
            s = s + "Setup Application Name " + AppDomain.CurrentDomain.SetupInformation.ApplicationName + "\r\n";
            s = s + "Setup Configuration File " + AppDomain.CurrentDomain.SetupInformation.ConfigurationFile + "\r\n";
            s = s + "Setup Dynamic Base " + AppDomain.CurrentDomain.SetupInformation.DynamicBase + "\r\n";
            s = s + "Setup License File " + AppDomain.CurrentDomain.SetupInformation.LicenseFile + "\r\n";
            s = s + "Setup Private Bin Path " + AppDomain.CurrentDomain.SetupInformation.PrivateBinPath + "\r\n";
            s = s + "Setup Private Bin Path Probe " + AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe + "\r\n";
            s = s + "Setup Shadow Copy Directories " + AppDomain.CurrentDomain.SetupInformation.ShadowCopyDirectories + "\r\n";
            s = s + "Setup Shadow Copy Files " + AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles + "\r\n";
         }

         return s;
      }

      /// <summary>
      /// Dump the AppSettings data from the .config file
      /// </summary>
      /// <returns>dumped data</returns>
      string DumpConfig()
      {
         string s = "";

         // if we are to dump the config data
         if (dumpConfig)
         {
            s = "=== .config\r\n\r\n";

            // for each app setting
            for (int i = 0; i < System.Configuration.ConfigurationSettings.AppSettings.Count; i++)
            {
               // dump the data
               s = s + System.Configuration.ConfigurationSettings.AppSettings.GetKey(i) + "=" +System.Configuration.ConfigurationSettings.AppSettings[i]+"\r\n";
            }
         }

         return s;
      }

      /// <summary>
      /// Dump CPU data
      /// </summary>
      /// <returns>sumped data</returns>
      string DumpCPU()
      {
         string s = string.Empty;

         // if we are to dump CPU data
         if (dumpCPU)
         {
            s = "=== CPU Details\r\n\r\n";

            // for each processor
            foreach (Processor processor in Processor.GetProcessors())
            {
               // dump the processor
               s = s + processor.ToString() + "\r\n";
            }
         }

         return s;
      }

      /// <summary>
      /// Dump the program options in a machine readable form
      /// </summary>
      /// <returns>dumped data</returns>
      string DumpLoadableOptions()
      {
         string s = "=== Loadable Options\r\n\r\n";

         // call our derived class to get the data
         s = s + ExportOptions() + "\r\n";

         return s;
      }

      /// <summary>
      /// Dump the current working directory
      /// </summary>
      /// <returns>dumped data</returns>
      string DumpCWD()
      {
         string s =  string.Empty;

         // if we are to dump the working directory data
         if (dumpCWD)
         {
            s = "=== Current Working Directory Details\r\n\r\n";

            // dump what it was when we were created
            s = s + "Current Working Directory When Started : " + startingWorkingDirectory + "\r\n";

            // dump what it is now
            s = s + "Current Working Directory Now : " + Environment.CurrentDirectory + "\r\n";
         }

         return s;
      }

      /// <summary>
      /// Dump the OS data
      /// </summary>
      /// <returns>dumped data</returns>
      string DumpWindowsVersion()
      {
         string s = string.Empty;

         // if we are to dump windows version data
         if (dumpWindowsVersion)
         {
            s = "=== Windows Version Details\r\n\r\n";

            // dump the os version
            s = s + "Platform " + Environment.OSVersion.Platform.ToString() + "\r\n";
            s = s + "Version " + Environment.OSVersion.Version.ToString() + "\r\n";

            // dump the machine name
            s = s + "Machine Name " + Environment.MachineName + "\r\n";
            s = s + "Network Domain Name " + Environment.UserDomainName + "\r\n";
         }

         return s;
      }

      /// <summary>
      /// dump the programs command line
      /// </summary>
      /// <returns>dumped data</returns>
      string DumpCommandLine()
      {
         string s = string.Empty;

         // if we are to dump the command line
         if (dumpCommandLine)
         {
            // dump the command line
            s = "=== Command Line Details\r\n\r\n";
            s = s + "Command Line : " + Environment.CommandLine + "\r\n";
         }

         return s;
      }

      /// <summary>
      /// Dump the call stack right now on this thread
      /// </summary>
      /// <returns>dumped data</returns>
      string DumpCallStack()
      {
         string s = "";

         // if we are to dump the call stack
         if (dumpCallStack)
         {
            // dump the call stack
            s = "*** Call stack now\r\n";
            s = s + Environment.StackTrace;
         }

         return s;
      }

      /// <summary>
      /// Dump the programs threads
      /// </summary>
      /// <returns>dumped data</returns>
      string DumpThreads()
      {
         string s = "";

         // if we are to dump threads
         if (dumpThreads)
         {
            s = "*** Thread dump\r\n";

            // get this thread
            int thisThread = AppDomain.GetCurrentThreadId();

            // get all the os threads in this process
            foreach(ProcessThread thread in Process.GetCurrentProcess().Threads)
            {
               // dump the thread data
               s = s + "Thread " + thread.Id.ToString() + ", State" + thread.ThreadState.ToString() + ", Start Address " + thread.StartAddress + (thread.Id == thisThread?"(Active Thread)":"") + "\r\n";
            }

            s = s + "\r\n";

            // now dump all our registered managed threads.
            // we need to do this as managed threads are not necessarily the same as os threads and
            // you cant get a thread object from a process thread object. You need a thread
            // object so you can suspend it and dump the stack ... bugger.
            foreach (Thread thread in threads)
            {
               // if this thread is us
               if (thread == Thread.CurrentThread)
               {
                  // we need to do us differently as we cant suspend ourselves
                  s = s + "Thread " + thread.Name + ", Thread State " + thread.ThreadState.ToString() + "\r\n";
                  s = s + new StackTrace(true).ToString() + "\r\n";
               }
               else
               {
                  // check it is still alive
                  if (thread.IsAlive)
                  {
                     // dump the thread
                     s = s + "Thread " + thread.Name + ", Thread State " + thread.ThreadState.ToString() + "\r\n";

                     // if this is the main thread this will already be suspended so dont do the suspend
                     if (thread != DebugToolkitBase.TheDebugToolkit.Thread)
                     {
                        // suspend the thread
                        thread.Suspend();
                     }

                     // it may have died
                     if (thread.IsAlive)
                     {
                        // do the stack trace
                        s = s + new StackTrace(thread, true).ToString() + "\r\n";
                     }

                     // if this is not the main thread
                     if (thread != DebugToolkitBase.TheDebugToolkit.Thread)
                     {
                        // resume it
                        thread.Resume();
                     }
                  }
                  else
                  {
                     // this thread is dead so ignore it
                  }
               }
            }

            // go though all the threads to remove dead ones
            for (int i = 0; i < threads.Count; i++)
            {
               // if it is dead
               if (!((Thread)threads[i]).IsAlive)
               {
                  // remove it
                  threads.RemoveAt(i);

                  // move back one
                  i--;
               }
            }
         }

         return s;
      }

      /// <summary>
      /// Dump a single assembly
      /// </summary>
      /// <param name="startAssembly">The assembly to dump</param>
      /// <param name="indent">The indent to use when displaying the assembly</param>
      /// <param name="dumpedAssemblies">List of Assemblies already dumped</param>
      /// <returns>dumped data</returns>
      string DumpAssembly(Assembly startAssembly, string indent,ref ArrayList dumpedAssemblies)
      {
         // add this assembly to the list of dumped assemblies
         dumpedAssemblies.Add(startAssembly);

         // dump the assembly data
         string s = indent + "Assembly " + startAssembly.FullName + 
            ", Location " + startAssembly.Location + 
            ", From GAC " + startAssembly.GlobalAssemblyCache + "\r\n";


         // for each module in this assembly
         foreach(Module module in startAssembly.GetModules(true))
         {
            // dump the module
            s = s + indent + "  - Module " + module.Name + 
               ", File " + module.FullyQualifiedName + 
               ", Version " + FileVersionInfo.GetVersionInfo(module.FullyQualifiedName).FileVersion + "\r\n";

            // if the assembly for that module has not already been dumped
            if (!dumpedAssemblies.Contains(module.Assembly))
            {
               // dump the modules assembly as well
               s = s + DumpAssembly(module.Assembly, indent + "   ", ref dumpedAssemblies);
            }
         }

         // now dump the referenced assemblies
         s = s + indent + "  - Referenced Assemblies\r\n";

         // for each referenced assembly
         foreach (AssemblyName assembly in startAssembly.GetReferencedAssemblies())
         {
            // dump the assembly
            s = s + indent + "   o " + assembly.FullName + 
               ", Version " + assembly.Version.ToString() +
               ", Version Compatability " + assembly.VersionCompatibility.ToString() + "\r\n";

            // now load it
            Assembly x = System.Reflection.Assembly.LoadWithPartialName(assembly.Name);

            // if we have not already dumped it
            if (!dumpedAssemblies.Contains(x))
            {
               // dump it
               s = s + DumpAssembly(x, indent + "   ",ref dumpedAssemblies);
            }
         }

         return s;
      }

      /// <summary>
      /// Dump all the assemblies for this program
      /// </summary>
      /// <returns>dumped data</returns>
      string DumpAssemblies()
      {
         string s = "";

         // if we are to dump the assemblies
         if (dumpAssemblies)
         {
            s = "=== Assembly Details\r\n\r\n";

            // get the root assembly
            Assembly rootAssembly = System.Reflection.Assembly.GetEntryAssembly();

            // create a list to track what we have already dumped else we will get in
            // never ending recursive loops and blow the stack and die
            ArrayList dumpedAssemblies = new ArrayList();

            // dump the root assembly
            s = s + DumpAssembly(rootAssembly , "", ref dumpedAssemblies);
         }

         return s;
      }

      /// <summary>
      /// Dump all the debug toolkit options
      /// </summary>
      /// <returns>Dumped data</returns>
      string DumpDebugToolkitOptions()
      {
         string s = "=== DebugToolkit Options\r\n\r\n";

         s = s + "Dump Threads " + dumpThreads.ToString() + "\r\n";
         s = s + "Dump Call Stack " + dumpCallStack.ToString() + "\r\n";
         s = s + "Dump Tasks " + dumpTasks.ToString() + "\r\n";
         s = s + "Dump Modules For All Tasks " + dumpModulesForAllTasks.ToString() + "\r\n";
         s = s + "Dump Memory/Disk " + dumpResources.ToString() + "\r\n";
         s = s + "Dump Performance Counters " + dumpPerfCounters.ToString() + "\r\n";

         s = s + "Dump Files " + dumpFiles.ToString() + "\r\n";
         s = s + "Dump Registry " + dumpRegistry.ToString() + "\r\n";
         s = s + "Dump CPU " + dumpCPU.ToString() + "\r\n";
         s = s + "Dump OS Info " + dumpWindowsVersion.ToString() + "\r\n";
         s = s + "Dump Current Working Directory " + dumpCWD.ToString() + "\r\n";
         s = s + "Dump Command Line " + dumpCommandLine.ToString() + "\r\n";
         s = s + "Dump Options " + dumpOptions.ToString() + "\r\n";
         s = s + "Dump Printer " + dumpPrinter.ToString() + "\r\n";
         s = s + "Dump Config " + dumpConfig.ToString() + "\r\n";
         s = s + "Dump Assemblies " + dumpAssemblies.ToString() + "\r\n";
         s = s + "Dump AppDomain " + dumpAppDomain.ToString() + "\r\n";
         s = s + "Dump Application " + dumpApplication.ToString() + "\r\n";

         s = s + "User Hang Detect " + userHangDetect.ToString() + "\r\n";
         s = s + "Hang Detect " + hangDetect.ToString() + "\r\n";
         s = s + "Assert Detect " + assertDetect.ToString() + "\r\n";
         s = s + "Exception Detect " + exceptionDetect.ToString() + "\r\n";
         s = s + "Abnormal Exit Detect " + abnormalExitDetect.ToString() + "\r\n";
         s = s + "Keep Performance Counters " + keepPerfCounters.ToString() + "\r\n";

         s = s + "Log Trace Output " + logTrace.ToString() + "\r\n";
         s = s + "Log Size " + logSize.ToString() + "\r\n";
         s = s + "Hang Ping Interval " + hangPingInterval.ToString() + "\r\n";
         s = s + "User Hang Ping Interval " + userHangPingInterval.ToString() + "\r\n";

         return s;
      }

      /// <summary>
      /// Dump all tasks running on the machine
      /// </summary>
      /// <returns>dumped data</returns>
      string DumpTasks()
      {
         string sTasks = "";

         // if we are to dump the tasks
         if (dumpTasks)
         {
            sTasks = "*** Task dump\r\n";

            // get all the running processes
            Process[] processes = Process.GetProcesses();

            // for each process running
            foreach (Process process in processes)
            {
               string sProcess = "";

               // get the main module name
               string moduleName = "N/A";
               try
               {
                  moduleName = process.MainModule.ModuleName;
               }
               catch
               {
               }

               // get the process handle
               string processHandle = "N/A";
               try
               {
                  processHandle = process.Handle.ToString();
               }
               catch
               {
               }

               // dump the process
               sProcess = "Process " + process.ProcessName + 
                  ", Id " + process.Id.ToString() + 
                  ", Handle " + processHandle + 
                  ", Main Module " + moduleName + 
                  ", Started " + process.StartTime.ToString() + 
                  ", Threads " + process.Threads.Count.ToString() + "\r\n";

               try
               {
                  // if this is the current process or we are to dump modules for all tasks
                  if (process.Id == Process.GetCurrentProcess().Id || dumpModulesForAllTasks)
                  {
                     // for each module in the process
                     foreach (ProcessModule module in process.Modules)
                     {

                        // get the modules version number
                        string version = "N/A";
                        try
                        {
                           version = module.FileVersionInfo.FileVersion;
                        }
                        catch
                        {
                        }

                        // dump the module
                        sProcess = sProcess + "   Module " + module.ModuleName + 
                           ", File " + module.FileName + 
                           ", Version " + version + 
                           ", Address " + module.BaseAddress.ToString() + "\r\n";
                     }
                  }
               }
               catch
               {
               }

               // add the process to the dumped data
               sTasks = sTasks + sProcess;
            }
         }

         return sTasks;
      }

      // structure for getting memory and disk status details
      struct MEMORYSTATUSEX 
      {
         public Int32 iLength; 
         public Int32 iMemoryLoad; 
         public Int64 ullTotalPhys; 
         public Int64 ullAvailPhys; 
         public Int64 ullTotalPageFile; 
         public Int64 ullAvailPageFile; 
         public Int64 ullTotalVirtual; 
         public Int64 ullAvailVirtual; 
         public Int64 ullAvailExtendedVirtual;
      };
      [ DllImport("kernel32", SetLastError=true) ]
      static extern void GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpMemoryStatusEx);
      [ DllImport("kernel32", SetLastError=true) ]
      static extern int GetDiskFreeSpaceEx(string lpDirectoryName, out Int64 lpFreeBytesAvailable, out Int64 lpTotalNumberOfBytes, out Int64 lpTotalNumberOfFreeBytes);

      /// <summary>
      /// Dumps computer resources such as disk and memory
      /// </summary>
      /// <returns>dumped data</returns>
      string DumpResources()
      {
         string s = "";

         // if we are to dump resources
         if (dumpResources)
         {
            s = "*** Resource dump\r\n";

            // Get all the disks
            string[] disks = Environment.GetLogicalDrives();

            // for each disk found
            Int64 bytesLeftICanUse;
            Int64 bytesOnDiskAvailableToUser;
            Int64 bytesFreeSpace;
            foreach (string disk in disks)
            {

               // get the disk free space stats
               if (GetDiskFreeSpaceEx(disk, out bytesLeftICanUse, out bytesOnDiskAvailableToUser, out bytesFreeSpace) != 0)
               {
                  // dump it out
                  s = s + "Disk " + disk + " Disk free space " + bytesFreeSpace.ToString() + " User allocation " + bytesOnDiskAvailableToUser.ToString() + " User allocation free " + bytesLeftICanUse.ToString() + "\r\n";
               }
            }

            // get memory data
//            MEMORYSTATUSEX memoryStatusEx = new MEMORYSTATUSEX();
//            memoryStatusEx.iLength = 2*4 + 7*8;
//            GlobalMemoryStatusEx(ref memoryStatusEx);

            // dump it out
//            s = s + "Memory Utilisation " + memoryStatusEx.iMemoryLoad.ToString() + "%\r\n";
//            s = s + "Physical Memory Available " + memoryStatusEx.ullAvailPhys.ToString() + "\r\n";
//            s = s + "Physical Memory Size " + memoryStatusEx.ullTotalPhys.ToString() + "\r\n";
//            s = s + "Physical Memory In Use " + Environment.WorkingSet.ToString() + "\r\n";
         }
         
         return s;
      }
      
      #endregion

      #region Utility Functions

      /// <summary>
      /// Given a registry key returns the top registry key of the hive
      /// </summary>
      /// <param name="entry">The registry key name</param>
      /// <returns>The registry key object for the hive root</returns>
      RegistryKey GetTopKey(string entry)
      {
         // get the hive root name
         string top = entry.Substring(0, entry.IndexOf("\\"));

         // default the return value
         RegistryKey key = null;

         // switch on the string
         switch(top)
         {
            case "HKEY_CLASSES_ROOT":
               key = Registry.ClassesRoot;
               break;
            case "HKEY_CURRENT_CONFIG":
               key = Registry.CurrentConfig;
               break;
            case "HKEY_CURRENT_USER":
               key = Registry.CurrentUser;
               break;
            case "HKEY_LOCAL_MACHINE":
               key = Registry.LocalMachine;
               break;
            case "HKEY_USERS":
               key = Registry.Users;
               break;
            case "HKEY_PERFORMANCE_DATA":
               key = Registry.PerformanceData;
               break;
            case "HKEY_DYN_DATA":
               key = Registry.DynData;
               break;
            default:
               throw new ApplicationException("Unknown registry root key " + top);
         }

         // return the key
         return key;
      }

      /// <summary>
      /// Expand the wildcards in the registry key
      /// </summary>
      /// <param name="wildcards">list of wildcards</param>
      void ExpandRegistryWildcard(ref ArrayList wildcards)
      {
         // go though each wildcard
         for(int i = 0; i < wildcards.Count; i++)
         {
            // if there is a wildcard in the current entry
            if (((string)wildcards[i]).IndexOf("\\*\\") != -1)
            {
               // extract it
               string entry = (string)wildcards[i];

               // remove it & move back to prior
               wildcards.RemoveAt(i);
               i--;

               // extract top level
               RegistryKey key = GetTopKey(entry);

               // split into before and after wild card
               string before = entry.Substring(0, entry.IndexOf("\\*\\"));
               string after = entry.Substring(entry.IndexOf("\\*\\")+3);

               // open the before key
               RegistryKey subKey = key.OpenSubKey(before.Substring(before.IndexOf("\\")+1));
               if (subKey != null)
               {
                  string[] subKeyNames = subKey.GetSubKeyNames();

                  // for each sub key
                  foreach(string subKeyName in subKeyNames)
                  {
                     // if there is still a wildcard
                     if (after.IndexOf("\\*\\") != -1)
                     {
                        // if there are still wildcards recursively call
                        ArrayList work = new ArrayList();
                        work.Add(before + "\\" + subKeyName + "\\" + after);
                        ExpandRegistryWildcard(ref work);
                        wildcards.AddRange(work);
                     }
                     else
                     {
                        // else add the value
                        wildcards.Add(before + "\\" + subKeyName + "\\" + after);
                     }
                  }
               }
               else
               {
                  // key not found
               }
            }
         }
      }

      /// <summary>
      /// Flags the process as exiting normally
      /// </summary>
      void NormalExit()
      {
         // create the mutex that protected the exit file and grab it
         Mutex mutex = new Mutex(false, "ExitTracking");
         mutex.WaitOne();

         // load the exit tracking xml file
         XmlDataDocument exit = new XmlDataDocument();
         try
         {
            exit.Load(appName + "ExitTracker.xml");
         }
         catch
         {
            // not good but we can fix it next time
            mutex.ReleaseMutex();
            mutex.Close();
            return;
         }

         // find our node
         XmlNode node = exit.SelectSingleNode("/Runs/Run[@id='" + Process.GetCurrentProcess().Id+"']");

         // if we found ourselves
         if (node != null)
         {
            // remove us
            node.ParentNode.RemoveChild(node);

            // save the xml file
            exit.Save(appName + "ExitTracker.xml");
         }
         else
         {
            // strange we are not there but can't be helped
            mutex.ReleaseMutex();
            mutex.Close();
            return;
         }

         // release our lock
         mutex.ReleaseMutex();
         mutex.Close();
      }

      /// <summary>
      /// Increments one of our named statistic counters
      /// </summary>
      /// <param name="counter">Name of counter to increment</param>
      void IncrementCounter(string counter)
      {
         // create our mutex to protect access to the exit tracking file and grab it
         Mutex mutex = new Mutex(false, "Counters");
         mutex.WaitOne();

         XmlDataDocument counters = new XmlDataDocument();
         try
         {
            // load the counters document
            counters.Load("Counters.xml");

            // get the counter node
            XmlNode node = counters.SelectSingleNode("Counters/Counter[@name='"+counter+"']");

            // if node was found
            if (node != null)
            {
               // increment the value
               node.InnerText = ((Convert.ToInt32(node.InnerText))+1).ToString();
            }
            else
            {
               // find the parent node
               XmlNode nodeCounters = counters.SelectSingleNode("Counters");

               // create a counter node
               XmlNode newNode = counters.CreateElement("Counter");

               // add the name attribute
               XmlAttribute attr = counters.CreateAttribute("name");
               attr.Value = counter;
               newNode.Attributes.Append(attr);

               // initialise value to 1
               newNode.InnerText = "1";

               // add it to its parent
               nodeCounters.AppendChild(newNode);
            }
         }
         catch
         {
            // create a root level node
            XmlNode nodeCounters = counters.CreateElement("Counters");
            counters.AppendChild(nodeCounters);

            // create a counter node
            XmlNode newNode = counters.CreateElement("Counter");

            // add the name attribute
            XmlAttribute attr = counters.CreateAttribute("name");
            attr.Value = counter;
            newNode.Attributes.Append(attr);

            // initialise value to 1
            newNode.InnerText = "1";

            // add it to its parent
            nodeCounters.AppendChild(newNode);
         }

         // save the updated xml file
         counters.Save("Counters.xml");
         counters = null;

         // release our lock
         mutex.ReleaseMutex();
         mutex.Close();
      }

      int GetCounter(string counter)
      {
         int rc = 0;

         // create our mutex to protect access to the exit tracking file and grab it
         Mutex mutex = new Mutex(false, "Counters");
         mutex.WaitOne();

         // load the counters file
         XmlDataDocument counters = new XmlDataDocument();
         counters.Load("Counters.xml");

         // release our lock
         mutex.ReleaseMutex();
         mutex.Close();

         // find the counter node
         XmlNode node = counters.SelectSingleNode("Counters/Counter[@name='"+counter+"']");

         // if we found it
         if (node != null)
         {
            // extract the value
            rc = Convert.ToInt32(node.InnerText);
         }
         else
         {
            // default to 0
            rc = 0;
         }

         counters = null;

         return rc;
      }

      /// <summary>
      /// Checks if there are any processes in the exit file that are not still running
      /// </summary>
      void CheckAbnormalExit()
      {
         // create our mutex to protect access to the exit tracking file and grab it
         Mutex mutex = new Mutex(false, "ExitTracking");
         mutex.WaitOne();

         // try to load the file
         XmlDataDocument exit = new XmlDataDocument();
         try
         {
            exit.Load(appName + "ExitTracker.xml");
         }
         catch
         {
            // file did not exist so create an empty one and return
            XmlNode runs = exit.CreateElement("Runs");
            exit.AppendChild(runs);

            // add a node for this process
            XmlNode run = exit.CreateElement("Run");
            run.Attributes.Append(exit.CreateAttribute("id"));
            run.Attributes["id"].Value = Process.GetCurrentProcess().Id.ToString();
            runs.AppendChild(run);

            // save the file
            exit.Save(appName + "ExitTracker.xml");

            // release our lock on it
            mutex.ReleaseMutex();
            mutex.Close();
            return;
         }

         // find our nodes
         XmlNodeList nodes = exit.SelectNodes("/Runs/Run");

         // start off assuming there were no abnormal exits
         bool abnormal = false;

         // go through the entire list
         foreach (XmlNode node in nodes)
         {
            // get the process id
            int id = Convert.ToInt32(node.Attributes["id"].Value);

            try
            {
               // if there is not process with that id or the process with that id does not share the same main module then we have a problem
               if (Process.GetProcessById(id) == null || (Process.GetProcessById(id) != null && Process.GetProcessById(id).MainModule != Process.GetCurrentProcess().MainModule))
               {
                  // the process is not running
                  abnormal = true;

                  // remove the node
                  node.ParentNode.RemoveChild(node);
               }
            }
            catch
            {
               // the process is not running
               abnormal = true;

               // remove the node
               node.ParentNode.RemoveChild(node);
            }
         }

      {
         // get the runs node
         XmlNode runs = exit.SelectSingleNode("/Runs");

         // add an element for this new process
         XmlNode run = exit.CreateElement("Run");
         run.Attributes.Append(exit.CreateAttribute("id"));
         run.Attributes["id"].Value = Process.GetCurrentProcess().Id.ToString();
         runs.AppendChild(run);
      }

         // save the xml file
         exit.Save(appName + "ExitTracker.xml");

         // release our lock
         mutex.ReleaseMutex();
         mutex.Close();

         // if the abnormal flag was set
         if (abnormal)
         {
            // if we are to keep performance counters
            if (keepPerfCounters)
            {
               // increment the executions counter
               IncrementCounter("Abnormal Exits");
            }

            // call our derive class in case they want to do something
            OnAbnormalExit();
         }
      }

      /// <summary>
      /// Test a file can be read
      /// </summary>
      /// <param name="file">name of file to test</param>
      /// <returns><c>true</c> if file can be opened</returns>
      bool TestFile(string file)
      {
         try
         {
            // open the file
            FileStream fs = File.OpenRead(file);

            // close the file
            fs.Close();

            // it worked
            return true;
         }
         catch
         {
            // call the derived class to tell them this failed
            OnTestFileFail(file);

            // return false
            return false;
         }
      }

      /// <summary>
      /// Test that an assembly can be loaded
      /// </summary>
      /// <param name="file">file name of assembly to load</param>
      /// <returns><c>false</c> if assembly cannot be loaded.</returns>
      bool TestAssembly(string file)
      {
         try
         {
            // load the assembly
            Assembly assembly = Assembly.LoadFrom(file);

            // it loaded ok
            return true;
         }
         catch
         {
            // call our derived class to tell them it could not be loaded
            OnTestAssemblyFail(file);

            // it failed
            return false;
         }
      }
      /// <summary>
      /// Gets/Sets the last time the hang thread successfully got a message processed.
      /// </summary>
      internal DateTime LastVisitTime
      {
         get
         {
            // protect this from multi thread access
            mutexThreadProtect.WaitOne();

            //extract the time
            DateTime rc = timeLastVisit;

            // release the lock
            mutexThreadProtect.ReleaseMutex();

            // return it
            return rc;
         }
         set
         {
            // protect this from multi thread access
            mutexThreadProtect.WaitOne();

            // update the last visit time
            timeLastVisit = value;

            // release the lock
            mutexThreadProtect.ReleaseMutex();
         }
      }

      /// <summary>
      /// Get the automatic hang detection interval
      /// </summary>
      internal int HangPingInterval
      {
         get
         {
            return hangPingInterval;
         }
      }

      /// <summary>
      /// Get the user hang detection interval
      /// </summary>
      internal int UserHangPingInterval
      {
         get
         {
            return userHangPingInterval;
         }
      }

      /// <summary>
      /// Get the main thread object
      /// </summary>
      /// <returns>The main thread object</returns>
      Thread Thread
      {
         get
         {
            // return the main thread object
            return mainThread;
         }
      }

      /// <summary>
      /// Gets the application name
      /// </summary>
      string AppName
      {
         get
         {
            // return the application name
            return appName;
         }
      }
      #endregion

      #endregion                                                                               

      #region protected functions

      /// <summary>
      /// Try to read all settings from the application config file.
      /// </summary>
      protected void LoadSettingsFromConfigFile()
      {
         System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();

         try
         {
            dumpThreads = ((bool)(configurationAppSettings.GetValue("DumpThreads", typeof(bool))));
         }
         catch
         {
         }
         try
         {
            recreateCounters = ((bool)(configurationAppSettings.GetValue("RecreateCounters", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpCallStack = ((bool)(configurationAppSettings.GetValue("DumpCallStack", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpTasks = ((bool)(configurationAppSettings.GetValue("DumpTasks", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpResources = ((bool)(configurationAppSettings.GetValue("DumpResources", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpModulesForAllTasks = ((bool)(configurationAppSettings.GetValue("DumpModulesForAllTasks", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpFiles = ((bool)(configurationAppSettings.GetValue("DumpFiles", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpRegistry = ((bool)(configurationAppSettings.GetValue("DumpRegistry", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpCPU = ((bool)(configurationAppSettings.GetValue("DumpCPU", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpWindowsVersion = ((bool)(configurationAppSettings.GetValue("DumpWindowsVersion", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpCWD = ((bool)(configurationAppSettings.GetValue("DumpCWD", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpCommandLine = ((bool)(configurationAppSettings.GetValue("DumpCommandLine", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpOptions = ((bool)(configurationAppSettings.GetValue("DumpOptions", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpPrinter = ((bool)(configurationAppSettings.GetValue("DumpPrinters", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpConfig = ((bool)(configurationAppSettings.GetValue("DumpConfig", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpAssemblies = ((bool)(configurationAppSettings.GetValue("DumpAssemblies", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpAppDomain = ((bool)(configurationAppSettings.GetValue("DumpAppDomain", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         dumpApplication = ((bool)(configurationAppSettings.GetValue("DumpApplication", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         userHangDetect = ((bool)(configurationAppSettings.GetValue("UserHangDetect", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         hangDetect = ((bool)(configurationAppSettings.GetValue("HangDetect", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         assertDetect = ((bool)(configurationAppSettings.GetValue("AssertDetect", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         exceptionDetect = ((bool)(configurationAppSettings.GetValue("ExceptionDetect", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         abnormalExitDetect = ((bool)(configurationAppSettings.GetValue("AbnormalExitDetect", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         logTrace = ((bool)(configurationAppSettings.GetValue("LogTrace", typeof(bool))));
         }
         catch
         {
         }
         try
         {
         logSize = ((int)(configurationAppSettings.GetValue("LogSize", typeof(int))));
         }
         catch
         {
         }
         try
         {
         hangPingInterval = ((int)(configurationAppSettings.GetValue("HangPingInterval", typeof(int))));
         }
         catch
         {
         }
         try
         {
         userHangPingInterval = ((int)(configurationAppSettings.GetValue("UserHangPingInterval", typeof(int))));
         }
         catch
         {
         }
         try
         {
            dumpPerfCounters = ((bool)(configurationAppSettings.GetValue("DumpPerfCounters", typeof(bool))));
         }
         catch
         {
         }
         try
         {
            keepPerfCounters = ((bool)(configurationAppSettings.GetValue("KeepPerfCounters", typeof(bool))));
         }
         catch
         {
         }

         configurationAppSettings = null;
      }

      /// <summary>
      /// Adds a performance counter to the list to dump
      /// </summary>
      /// <param name="category">counter category</param>
      /// <param name="counter">counter name</param>
      protected void AddCounter(string category, string counter, string instance)
      {
         // add it to our list
         perfCounters.Add(category + "##" + counter + "$$" + instance);
      }

      /// <summary>
      /// Add a file to check. testAssembly/testFile are mutually exclusive.
      /// </summary>
      /// <param name="file">File to check</param>
      /// <param name="testAssembly"><c>true</c> if the file is an assembly that we want to verify can be loaded.</param>
      /// <param name="testFile"><c>true</c> if we want to check the file can be opened.</param>
      /// <returns>true if file checking succeeded.</returns>
      protected bool AddFile(string file, bool testAssembly, bool testFile)
      {
         // check we were not asked to testFile and testAssembly
         if (testAssembly && testFile)
         {
            throw new ApplicationException("Can only check ASSEMBLY or FILE not both");
         }

         // add the file to our file array
         files.Add(file); 

         // test assembly if asked
         if (testAssembly)
         {
            return TestAssembly(file);
         }
         // test file open if asked
         else if (testFile)
         {
            return TestFile(file);
         }
         // in all other cases this works
         else
         {
            return true;
         }
      }

      /// <summary>
      /// Add a registry key to the list of keys to dump
      /// </summary>
      /// <param name="entry">Key to dump.</param>
      protected void AddRegistry(string entry)
      {
         // add the file to our file array
         registryEntries.Add(entry); 
      }

      #endregion

      #region public functions
      
      public delegate void Main2(string[] args);

      /// <summary>
      /// Starts the debug toolkit.
      /// </summary>
      /// <param name="AppName">The name of the application to display</param>
      /// <param name="subMain">The 2nd main in the calling application.</param>
      /// <param name="args">The arguments passed into the main function.</param>
      public void Start(string AppName, Main2 subMain, string[] args)
      {
         // save the application name
         appName = AppName;

         // if we are to keep performance counters
         if (keepPerfCounters)
         {
            // increment the executions counter
            IncrementCounter("Executions");
         }

         // register the main thread
         RegisterThread(Thread.CurrentThread, "Main Thread");

         // install the circular log trace listener if required
         CircularFileStream.CircularFileStream cfs = null;
         TextWriterTraceListener tl = null;
         if (logTrace)
         {
            cfs = new CircularFileStream.CircularFileStream(appName + ".dat", logSize);
            cfs.WriteAsString = true;
            tl = new TextWriterTraceListener(cfs, "DebugToolKitListener");
            Trace.AutoFlush = true; // required so data is written immediately
            Trace.Listeners.Add(tl);
         }

         // insert the assert trace listener if required
         AssertStream assertStream = null;
         TextWriterTraceListener tl2 = null;
         if (assertDetect)
         {
            assertStream = new AssertStream(this);
            tl2 = new TextWriterTraceListener(assertStream, "DebugToolKitAssertListener");
            Trace.Listeners.Add(tl2);
         }

         // if we are to check for abnormal exits then do it now
         if (abnormalExitDetect)
         {
            CheckAbnormalExit();
         }

         // if we are to check for exceptions then install some handlers
         if (exceptionDetect)
         {
            // install the handlers
            Application.ThreadException += new ThreadExceptionEventHandler(OnThreadingExceptionEvent);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledExceptionEvent);
         }

         // if we are to do automatic hang detection
         if (hangDetect)
         {
            // initialise the last visit time
            LastVisitTime = DateTime.Now;

            // create the timer window
            hangMonitorForm = new HangMonitorForm();
            hangMonitorForm.Show();

            // Create the thread
            hangThread = new Thread(new ThreadStart(HangThreadProc));
            hangThread.Start();
         }

         // if we are to detect user hangs
         if (userHangDetect)
         {
            // Create the user hang detect thread
            userHangThread = new Thread(new ThreadStart(UserHangThreadProc));
            userHangThread.Start();
         }

         // run the form based application
         try
         {
            subMain(args);
         }
         catch(Exception e)
         {
            // if we are to process exceptions caught
            if (exceptionDetect)
            {
               // call our exception handler
               HandleException(e, true);
            }
            else
            {
               // if we are not handling exceptions then rethrow this exception
               throw e;
            }
         }

         // if we are doing abnormal exit detection
         if (abnormalExitDetect)
         {
            // record this as a normal exit
            NormalExit();
         }

         // if we are to keep performance counters
         if (keepPerfCounters)
         {
            // increment the normal exits counter
            IncrementCounter("Normal Exits");
         }

         // if we are doing exception detection
         if (exceptionDetect)
         {
            // remove our handlers
            Application.ThreadException -= new ThreadExceptionEventHandler(OnThreadingExceptionEvent);
            AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(OnUnhandledExceptionEvent);
         }

         // if we are doing assertion detection
         if (assertDetect)
         {
            // remove the assertion trace listener
            Trace.Listeners.Remove("DebugToolKitAssertListener");
            tl2.Close();
            assertStream.Close();
         }

         // if we are doing circular log tracing
         if (logTrace)
         {
            // remove the cicular log listener
            Trace.Listeners.Remove("DebugToolKitListener");
            tl.Close();
            cfs.Close();
         }

         // if we have a user hang thread
         if (userHangThread != null && userHangThread.IsAlive)
         {
            // kill it
            userHangThread.Abort();
            userHangThread.Join(6000);
            userHangThread = null;
         }

         // if we have a automatic detecting hang thread
         if (hangThread != null && hangThread.IsAlive)
         {
            // kill it
            hangThread.Abort();
            hangThread.Join(6000);
            hangThread = null;
         }

         // if we have a hang monitor form
         if (hangMonitorForm != null)
         {
            // close it
            hangMonitorForm.Close();
            hangMonitorForm = null;
         }
      }

      /// <summary>
      /// Save the log etc to a file.
      /// </summary>
      /// <param name="file">File to write everything to</param>
      /// <param name="prefix">A string you would like to put at the top of the saved file.</param>
      public void DumpToFile(string file, string prefix)
      {
         // open the file
         TextWriter tw = new StreamWriter(file);

         // do the dump
         DumpToFile(tw, prefix);

         // close the file
         tw.Close();
      }

      /// <summary>
      /// Save the log etc to a file.
      /// </summary>
      /// <param name="tw">Text writer to write the data to</param>
      /// <param name="prefix">A string you would like to put at the top of the saved file.</param>
      public void DumpToFile(TextWriter tw, string prefix)
      {
         // write the provided prefix data
         tw.WriteLine(prefix);
         tw.WriteLine("");
         tw.WriteLine("");

         // now dump the data
         tw.WriteLine("=================== Debug information follows ===================");
         tw.WriteLine("");

         tw.WriteLine(appName);
         tw.WriteLine("Version " + version);
         tw.WriteLine("");

         // suspend hang detection as the next steps can take some time
         SuspendHang = true;
         SuspendUserHang = true;

         tw.WriteLine(DumpDebugToolkitOptions());
         tw.WriteLine(DumpOptions());
         tw.WriteLine(DumpLoadableOptions());
         tw.WriteLine(DumpConfig());
         tw.WriteLine(DumpPrinter());
         tw.WriteLine(DumpCWD());
         tw.WriteLine(DumpWindowsVersion());
         tw.WriteLine(DumpAssemblies());
         tw.WriteLine(DumpAppDomain());
         tw.WriteLine(DumpApplication());
         tw.WriteLine(DumpCPU());
         tw.WriteLine(DumpCommandLine());
         tw.WriteLine(DumpRegistry());
         tw.WriteLine(DumpFiles());
         tw.WriteLine(DumpPerfCounters());
         tw.WriteLine("");
         tw.WriteLine("=== Trace Log Details");
         tw.WriteLine("");
         tw.WriteLine(DumpLog());

         tw.WriteLine("");
         tw.WriteLine("=== Application Provided Details");
         tw.WriteLine("");
         tw.WriteLine(GetDumpOtherAtSave());
         tw.WriteLine("");
         tw.WriteLine("=== Done");

         // resume hang detection
         SuspendHang = false;
         SuspendUserHang = false;
      }

      /// <summary>
      /// Set to <c>true</c> when automatic hang detection is suspended.
      /// This prevents false hang detections when the code is very busy
      /// </summary>
      public bool SuspendHang
      {
         get
         {
            // protect from multi thread access
            mutexThreadProtect.WaitOne();

            // get the value
            bool rc = suspendHangDetect;

            // release the lock
            mutexThreadProtect.ReleaseMutex();

            // return it
            return rc;
         }
         set
         {
            // protect from multi thread access
            mutexThreadProtect.WaitOne();

            // set the value
            suspendHangDetect = value;

            // if hang detection was turned off
            if (!suspendHangDetect)
            {
               // if we are turning suspend off then reset the last visit time so we don't
               // falsly detect a hang
               LastVisitTime = DateTime.Now;
            }

            // release the lock
            mutexThreadProtect.ReleaseMutex();
         }
      }

      /// <summary>
      /// Set to <c>true</c> when user hang detection is suspended.
      /// This prevents false user hang detections when the code is very busy
      /// </summary>
      public bool SuspendUserHang
      {
         get
         {
            // protect from multi thread access
            mutexThreadProtect.WaitOne();

            // get the value
            bool rc = suspendUserHangDetect;

            // release the lock
            mutexThreadProtect.ReleaseMutex();

            // return it
            return rc;
         }
         set
         {
            // protect from multi thread access
            mutexThreadProtect.WaitOne();

            // set the value
            suspendUserHangDetect = value;

            // release the lock
            mutexThreadProtect.ReleaseMutex();
         }
      }

      /// <summary>
      /// Called by programs which have caught an exception that they want reported like
      /// the exceptions we normally catch
      /// </summary>
      /// <param name="e">The exception to report</param>
      /// <param name="callOnException"><c>true</c> of you want the derived classes <c>OnException</c> method called.</param>
      public void ReportException(Exception e, bool callOnException)
      {
         // call our exception handler
         HandleException(e, callOnException);
      }

      /// <summary>
      /// Registers a managed thread for dumping.
      /// </summary>
      /// <param name="thread">Thread to register</param>
      /// <param name="name">Name to give the thread</param>
      public void RegisterThread(System.Threading.Thread thread, string name)
      {
         // if the thread is not already register
         if (!threads.Contains(thread))
         {
            // set the thread name
            thread.Name = name;

            // add it to the list
            threads.Add(thread);
         }

         // check the threads
         for(int i = 0; i < threads.Count; i++)
         {
            // if the thread is dead
            if (!((Thread)threads[i]).IsAlive)
            {
               // remove it
               threads.RemoveAt(i);

               // move back one
               i--;
            }
         }
      }

      /// <summary>
      /// Static method to get the one instance of the debug toolkit
      /// </summary>
      public static DebugToolkitBase TheDebugToolkit
      {
         get
         {
            // return the debug toolkit
            return __TheDebugToolkit;
         }
      }

      #endregion
                                                                               
   }
}
