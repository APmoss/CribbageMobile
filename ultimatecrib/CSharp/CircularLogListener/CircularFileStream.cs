// Circular File Stream

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
using System.IO;
using Winterdom.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CircularFileStream
{
   /// <summary>
   /// Contains utility function used by the circular file classes
   /// </summary>
   internal class Utility
   {
      /// <summary>
      /// serialise an object to a byte array
      /// <param name="obj">Object to serialize.</param>
      /// <returns>A byte array containing the object</returns>
      /// </summary>
      public static byte[] SerialiseObject(object obj)
      {
         // serialise the object to the memory stream
         BinaryFormatter outBinaryFormatter = new BinaryFormatter();
         MemoryStream outMemoryStream = new MemoryStream();
         // create a byte array to put it in
         byte[] buffer;
			
         // Serialize the object into the memoery stream
         outBinaryFormatter.Serialize(outMemoryStream, obj);

         // Create a byte array based on the memory stream
         buffer = new byte[outMemoryStream.Length];

         // rewind the memory stream
         outMemoryStream.Position = 0;

         // fill the byte array
         outMemoryStream.Read(buffer, 0 , (int)outMemoryStream.Length);

         // return the byte array
         return buffer;
      }

      /// <summary>
      /// deserialise an object from a byte array
      /// <param name="buffer">buffer containing serialized object.</param>
      /// <returns>The deserialized object.</returns>
      /// </summary>
      public static object DeserialiseObject(byte[] buffer)
      {
         // create an object of the type specified
         BinaryFormatter binaryFormatter = new BinaryFormatter();
         MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.GetLength(0));

         return binaryFormatter.Deserialize(memoryStream); 
      }
   }
   

   #region File Disk Structures

   /// <summary>
   /// Represents a record header on disk
   /// </summary>
   // It must be serializable as that is how we write it out
   [Serializable()] 
   struct DATARECORDHEADER : ISerializable
   {
      #region static variables

      // we calculate the size of this class at runtime and store it here to make it faster to get to
      static int sizeStore = -1;
      
      #endregion

      #region member variables

      // record identifier tag
      public long id; 

      // number of the record
      public long recNum; 

      // sequence number
      public long seqNum;
 
      // length of the data portion of the record
      public long length; 
      
      // offset of the next record from the start of the file
      public long nextRecordOffset; 
      
      #endregion

      #region ISerializable
      /// <summary>
      /// Serialise my contents
      /// <param name ="info">The serialised info.</param>
      /// <param name ="context">The serialisation context.</param>
      /// </summary>
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         // put all my data in the serialisation info object
         info.AddValue("id", id);
         info.AddValue("recNum", recNum);
         info.AddValue("seqNum", seqNum);
         info.AddValue("length", length);
         info.AddValue("nextRecordOffset", nextRecordOffset);
      }
      /// <summary>
      /// Constructor for deserialisation
      /// <param name ="info">The serialised info.</param>
      /// <param name ="context">The serialisation context.</param>
      /// </summary>
      public DATARECORDHEADER(SerializationInfo info, StreamingContext context) 
      {
         // extract all the data values
         id = (long)info.GetValue("id", typeof(long));
         recNum = (long)info.GetValue("recNum", typeof(long));
         seqNum = (long)info.GetValue("seqNum", typeof(long));
         length = (long)info.GetValue("length", typeof(long));
         nextRecordOffset = (long)info.GetValue("nextRecordOffset", typeof(long));
      }
      #endregion

      #region public static functions
      /// <summary>
      /// Holds the size of this class in serialised form
      /// <returns>The size of this class when serialised.</returns>
      /// </summary>
      public static int Size
      {
         get
         {
            // if we have never calculated our size
            if (sizeStore == -1)
            {
               // calculate our size
               DATARECORDHEADER drh = new DATARECORDHEADER();
               sizeStore = Utility.SerialiseObject(drh).GetLength(0);
            }

            // return our size
            return sizeStore;
         }
      }

      #endregion
   };

   /// <summary>
   /// Represents a file header on disk
   /// </summary>
   // It must be serializable as that is how we write it out
   [Serializable()] 
   struct FILEHEADER : ISerializable
   {
      #region static variables

      // we calculate the size of this class at runtime and store it here to make it faster to get to
      static int sizeStore = -1;
      
      #endregion

      #region member variables

      // number of records in the file
      public long nodes; 
      
      #endregion

      #region ISerializable
      /// <summary>
      /// Serialise my contents
      /// <param name ="info">The serialised info.</param>
      /// <param name ="context">The serialisation context.</param>
      /// </summary>
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         // put all my data in the serialisation info object
         info.AddValue("nodes", nodes);
      }
      /// <summary>
      /// Constructor for deserialisation
      /// <param name ="info">The serialised info.</param>
      /// <param name ="context">The serialisation context.</param>
      /// </summary>
      public FILEHEADER(SerializationInfo info, StreamingContext context) 
      {
         // extract all the data values
         nodes = (long)info.GetValue("nodes", typeof(long));
      }
      #endregion

      #region public static functions
      /// <summary>
      /// Holds the size of this class in serialised form
      /// <returns>The size of this class when serialised.</returns>
      /// </summary>
      public static int Size
      {
         get
         {
            // if we have never calculated our size
            if (sizeStore == -1)
            {
               // calculate our size
               FILEHEADER fh = new FILEHEADER();
               sizeStore = Utility.SerialiseObject(fh).GetLength(0);
            }

            // return our size
            return sizeStore;
         }
      }

      #endregion
   };

   #endregion

   /// <summary>
   /// Controls the memory mapped file based index
   /// </summary>
   internal class Index
   {
      #region index memory structures

      /// <summary>
      /// Represents an item index entry in the memory mapped file
      /// </summary>
      // It must be serializable as that is how we write it out
      [Serializable()] 
      struct INDEXITEM : ISerializable
      {
         #region static variables

         // we calculate the size of this class at runtime and store it here to make it faster to get to
         static int sizeStore = -1;
      
         #endregion

         #region member variables
         
         // while an index really does not need all these data items keeping them in memory eliminates the need
         // for a read before updating a data record header

         // offset from start of file where entry resides
         public long offset;
 
         // size of data portion of file entry
         public long size; 

         // offset from start of file where next entry resides
         public long nextOffset; 
         
         // the sequence number of the record
         public long seqNum; 

         #endregion

         #region Iserializable

         /// <summary>
         /// Serialise my contents
         /// <param name ="info">The serialised info.</param>
         /// <param name ="context">The serialisation context.</param>
         /// </summary>
         public void GetObjectData(SerializationInfo info, StreamingContext context)
         {
            // put all my data in the serialisation info object
            info.AddValue("offset", offset);
            info.AddValue("size", size);
            info.AddValue("nextOffset", nextOffset);
            info.AddValue("seqNum", seqNum);
         }

         /// <summary>
         /// Constructor for deserialisation
         /// <param name ="info">The serialised info.</param>
         /// <param name ="context">The serialisation context.</param>
         /// </summary>
         public INDEXITEM(SerializationInfo info, StreamingContext context) 
         {
            // extract all the data values
            offset = (long)info.GetValue("offset", typeof(long));
            size = (long)info.GetValue("size", typeof(long));
            nextOffset = (long)info.GetValue("nextOffset", typeof(long));
            seqNum = (long)info.GetValue("seqNum", typeof(long));
         }

         #endregion

         #region public static functions
         /// <summary>
         /// Holds the size of this class in serialised form
         /// <returns>The size of this class when serialised.</returns>
         /// </summary>
         public static int Size
         {
            get
            {
               // if we have never calculated our size
               if (sizeStore == -1)
               {
                  // calculate our size
                  INDEXITEM ii = new INDEXITEM();
                  sizeStore = Utility.SerialiseObject(ii).GetLength(0);
               }

               // return our size
               return sizeStore;
            }
         }

      #endregion
      };

      /// <summary>
      /// Represents an index header in the memory mapped file
      /// </summary>
      // It must be serializable as that is how we write it out
      [Serializable()] 
      struct INDEXHEADER : ISerializable
      {
         #region static variables

         // we calculate the size of this class at runtime and store it here to make it faster to get to
         static int sizeStore = -1;
      
         #endregion

         #region member variables
         // number of nodes in the index
         public long nodes;

         // next sequence number that should be used
         public long nextSequence;

         // next slot that should be used
         public long nextSlot;
         #endregion

         #region ISerializable

         /// <summary>
         /// Serialise my contents
         /// <param name ="info">The serialised info.</param>
         /// <param name ="context">The serialisation context.</param>
         /// </summary>
         public void GetObjectData(SerializationInfo info, StreamingContext context)
         {
            // put all my data in the serialisation info object
            info.AddValue("nodes", nodes);
            info.AddValue("nextSequence", nextSequence);
            info.AddValue("nextSlot", nextSlot);
         }

         /// <summary>
         /// Constructor for deserialisation
         /// <param name ="info">The serialised info.</param>
         /// <param name ="context">The serialisation context.</param>
         /// </summary>
         public INDEXHEADER(SerializationInfo info, StreamingContext context) 
         {
            // extract all the data values
            nodes = (long)info.GetValue("nodes", typeof(long));
            nextSequence = (long)info.GetValue("nextSequence", typeof(long));
            nextSlot = (long)info.GetValue("nextSlot", typeof(long));
         }
         #endregion

         #region public static functions
         /// <summary>
         /// Holds the size of this class in serialised form
         /// <returns>The size of this class when serialised.</returns>
         /// </summary>
         public static int Size
         {
            get
            {
               // if we have never calculated our size
               if (sizeStore == -1)
               {
                  // calculate our size
                  INDEXHEADER ih = new INDEXHEADER();
                  sizeStore = Utility.SerialiseObject(ih).GetLength(0);
               }

               // return our size
               return sizeStore;
            }
         }

      #endregion
      }

      #endregion

      /// <summary>
      /// This class is used for all reads and writes to the index memory mapped file
      /// </summary>
      class IndexAccessor : IDisposable
      {
         #region member variables

         // this holds the memory mapped file object
         MemoryMappedFile mapFile = null;

         // this holds the stream which represents the memory mapped file
         Stream strm = null;

         #endregion

         #region IDispose

         public void Dispose()
         {
            // if we have a mapped file
            if (mapFile != null)
            {
               mapFile.Close();
               mapFile = null;
               strm = null;
            }
         }

         #endregion

         #region constructors

         /// <summary>
         /// Gets the memory mapped file ready for use
         /// </summary>
         public IndexAccessor(string fileName, Stream file)
         {
            // if we create the memory mapped file then we are responsible for loading it into memory
            bool owned; 

            // initialise our candicate next vars
            long nextSlotCandidate = 0;
            long nextSequenceCandidate = 0;

            // create a memory mapped file to build the index in
            mapFile = new MemoryMappedFile();

            // try to open an existing memory mapped file
            try
            {
               // open it
               mapFile.Open(MapAccess.FileMapAllAccess, fileName);

               // if we are here then it already exists so we don't own it which means we don't need to initialise the index
               owned = false;
            }
            catch
            {
               // could not open it so lets create it
               mapFile.Create(null, MapProtection.PageReadWrite,0x100000, fileName);

               // we created it so now we own the population step
               owned = true;
            }
   
            // get a stream for the file
            strm = mapFile.MapView(MapAccess.FileMapAllAccess,0, 0x100000);

            // if we own the memory mapped file
            if (owned)
            {
               // now read in file and create index
            
               // start from the beginning of our disk file
               file.Seek(0, SeekOrigin.Begin);

               // read in the file header
               byte[] fhb = new Byte[FILEHEADER.Size];
               long offset = file.Read(fhb, 0, FILEHEADER.Size);
               BinaryFormatter binaryFormatter = new BinaryFormatter();
               MemoryStream memoryStream = new MemoryStream(fhb, 0,FILEHEADER.Size);
               FILEHEADER fh = (FILEHEADER)binaryFormatter.Deserialize(memoryStream); 

               // create a index header first
               WriteEmptyHeader();

               // store the number of nodes
               Nodes = fh.nodes;
               
               // set the next slot to the last that way the first new record will be written to slot 1
               NextSlot = fh.nodes;

               // initialise index
               for (long i = 0; i <= Nodes; i++)
               {
                  INDEXITEM ii = new INDEXITEM();
                  ii.offset = long.MinValue;
                  ii.size = 0;
                  ii.seqNum = 0;
                  ii.nextOffset = long.MinValue;
                  this[i] = ii;
               }

               // allocate some space to load each record into
               byte[] drhb = new Byte[DATARECORDHEADER.Size];
                  
               // now read in each record
               for (long i = 0; i <= Nodes; i++)
               {
                  // read a data record header
                  file.Read(drhb,0, DATARECORDHEADER.Size);
                  memoryStream = new MemoryStream(drhb, 0, DATARECORDHEADER.Size);
                  DATARECORDHEADER drh = (DATARECORDHEADER)binaryFormatter.Deserialize(memoryStream); 

                  // validate data record - id is valid and the next point is big enough so that the record fits in
                  if ((drh.id == long.MinValue) &&
                     (drh.nextRecordOffset >= offset + DATARECORDHEADER.Size + drh.length))
                  {
                     // store index information
                     INDEXITEM ii = this[drh.recNum];
                     ii.offset = offset;
                     ii.size = drh.length;
                     ii.seqNum = drh.seqNum;
                     ii.nextOffset = drh.nextRecordOffset;
                     this[drh.recNum] = ii;

                     INDEXITEM test = this[drh.recNum];
                     Debug.Assert(test.Equals(ii));

                     // if the sequence number is the highest we have seen
                     if (drh.seqNum > nextSequenceCandidate)
                     {
                        // store the high sequence number
                        nextSequenceCandidate = drh.seqNum;

                        // store the record number as possibly where we are up to in the file
                        nextSlotCandidate = drh.recNum;
                     }
                  }
                  else
                  {
                     // file cannot be read
                     throw new ApplicationException("File is corrupt ... data read is not a valid record.");
                  }

                  // seek to the start of the next record skipping over current records data and any free space
                  file.Seek(drh.nextRecordOffset - offset - DATARECORDHEADER.Size, SeekOrigin.Current);

                  // store the offset of the record we are about to read
                  offset = drh.nextRecordOffset;
               }

               // now that we have read all the data in the next sequence number will be one higher than we have seen
               NextSequence = nextSequenceCandidate + 1;

               // if we have no next slot candidate
               if (nextSlotCandidate == 0)
               {
                  // make it the last in the file
                  nextSlotCandidate = Nodes;
               }
               
               // save the next slot number
               NextSlot = nextSlotCandidate;

               // check index loaded all records
               for (long i = 0; i <= Nodes; i++)
               {
                  if (this[i].offset == long.MinValue)
                  {
                     // file cannot be read
                     throw new ApplicationException("File is corrupt ... data read is not a valid record.");
                  }
               }
            }
         }

         #endregion

         #region private functions

         /// <summary>
         /// Writes an empty index header into the memory mapped file
         /// </summary>
         void WriteEmptyHeader()
         {
            // create an index header
            INDEXHEADER ih = new INDEXHEADER();

            // fill it with default values
            ih.nextSequence = -1;
            ih.nextSlot = -1;
            ih.nodes = -1;

            // save it to the file
            IndexHeader = ih;
         }

         /// <summary>
         /// Used internally to read and write the INDEXHEADER from the memory mapped file
         /// </summary>
         INDEXHEADER IndexHeader
         {
            get
            {
               // go to the start of the file
               strm.Seek(0, SeekOrigin.Begin);

               // allocate a buffer for the index header
               byte[] buffer = new Byte[INDEXHEADER.Size];

               // read it in
               strm.Read(buffer, 0, INDEXHEADER.Size);

               // create & return the memory header object
               BinaryFormatter binaryFormatter = new BinaryFormatter();
               MemoryStream memoryStream = new MemoryStream(buffer, 0, INDEXHEADER.Size);
               return (INDEXHEADER)binaryFormatter.Deserialize(memoryStream);
            }
            set
            {
               // go to the start of the file
               strm.Seek(0, SeekOrigin.Begin);

               // write the header to the file
               strm.Write(Utility.SerialiseObject(value), 0, INDEXHEADER.Size);
            }
         }

         #endregion

         #region index header field accessors

         /// <summary>
         /// Used to read/write the nodes entry in the index header
         /// <return>The number of nodes in the index</return>
         /// </summary>
         public long Nodes
         {
            get
            {
               // read the index header and return the nodes element
               return IndexHeader.nodes;
            }
            set
            {
               // retrieve the index header
               INDEXHEADER ih = IndexHeader;

               // update the nodes value
               ih.nodes = value;

               // save the index header back to the memory mapped file
               IndexHeader = ih;
            }
         }

         /// <summary>
         /// Used to read/write the next sequence number entry in the index header
         /// <return>The next sequence number as stored in the index</return>
         /// </summary>
         public long NextSequence
         {
            get
            {
               // read the index header and return the next sequence element
               return IndexHeader.nextSequence;
            }
            set
            {
               // retrieve the index header
               INDEXHEADER ih = IndexHeader;

               // update the next sequence value
               ih.nextSequence = value;

               // save the index header back to the memory mapped file
               IndexHeader = ih;
            }
         }

         /// <summary>
         /// Used to read/write the next slot entry in the index header
         /// <return>The next slot as stored in the index</return>
         /// </summary>
         public long NextSlot
         {
            get
            {
               // read the index header and return the next slot element
               return IndexHeader.nextSlot;
            }
            set
            {
               // retrieve the index header
               INDEXHEADER ih = IndexHeader;

               // update the next slot value
               ih.nextSlot = value;

               // save the index header back to the memory mapped file
               IndexHeader = ih;
            }
         }

         #endregion

         #region index item accessor

         /// <summary>
         /// Used to read/write the slot data in the index header
         /// <param name="index">The record number of the element you want the index data for</param>
         /// <return>The slot data as stored in the index</return>
         /// </summary>
         public INDEXITEM this[long index]
         {
            get
            {
               byte[] buffer = new byte[INDEXITEM.Size];
               
               // move the the location of the record
               strm.Seek(index * INDEXITEM.Size + INDEXHEADER.Size, SeekOrigin.Begin);

               // read in the index data
               strm.Read(buffer, 0, INDEXITEM.Size);

               // create & return the index item object
               BinaryFormatter binaryFormatter = new BinaryFormatter();
               MemoryStream memoryStream = new MemoryStream(buffer, 0, INDEXITEM.Size);
               return (INDEXITEM)binaryFormatter.Deserialize(memoryStream);
            }
            set
            {
               // seek to the correct location in the memory mapped file
               strm.Seek(index * INDEXITEM.Size + INDEXHEADER.Size, SeekOrigin.Begin);
               strm.Write(Utility.SerialiseObject(value), 0, INDEXITEM.Size);
            }
         }

         #endregion

         #region public functions
         /// <summary>
         /// Checks the validity of the Index accessor
         /// <return><c>true</c> if the Index accessor is valid for use</return>
         /// </summary>
         public bool IsValid()
         {
            // valid if we have a mapfile and a stream on it
            return mapFile != null && strm != null;
         }

         #endregion
      };

      #region member variables

      // This object provides access to the index in the memory mapped file
      IndexAccessor indexAccessor = null;

      #endregion

      #region IDisposable
      /// <summary>
      /// Dispose of our resources
      /// </summary>
      public void Dispose()
      {
         if (indexAccessor != null)
         {
            // dispose of our index accessor
            indexAccessor.Dispose();
            indexAccessor = null;
         }
      }
      #endregion

      #region constructors
      /// <summary>
      /// Create the index
      /// <param name="fileName">Name of the file the index is on</param>
      /// <param name="file">A stream on the file being indexed</param>
      /// </summary>
      public Index(string fileName, Stream file)
      {
         // create an index accessor on the file
         indexAccessor = new IndexAccessor(fileName, file);
      }

      #endregion

      #region public functions

      /// <summary>
      /// Gets the number of nodes in the index
      /// </summary>
      public long Nodes
      {
         get
         {
            return indexAccessor.Nodes;
         }
      }
  
      /// <summary>
      /// Checks the validity of the Index
      /// <return><c>true</c> if the Index is valid for use</return>
      /// </summary>
      public bool IsValid()
      {
            return indexAccessor != null && indexAccessor.IsValid();
      }

      /// <summary>
      /// Get the index data for an item
      /// <param name="index">Index of the item to get the index data on.</param>
      /// <param name="offset">Offset of the element in the physical file</param>
      /// <param name="size">Size of the element in the physical file</param>
      /// <param name="nextRecordOffset">Offset of the record physically stored after the given record in the physical file</param>
      /// </summary>
      public void GetIndexData(long index, out long offset, out long size, out long nextRecordOffset)
      {
         // clear the value of all output parameters
         offset = 0;
         size = 0;
         nextRecordOffset = 0;

         if (!IsValid())
         {
            throw new ApplicationException("Index not ready");
         }

         if (index < 0 || index > Nodes)
         {
            throw new ArgumentException("Index is out of bounds.");
         }

         // copy the index data into the return fields
         offset = indexAccessor[index].offset;
         size = indexAccessor[index].size;
         nextRecordOffset = indexAccessor[index].nextOffset;
      }
      
      /// <summary>
      /// Get the record that is physically stored prior to the given record on the disk
      /// <param name="index">Index of the record that we need the physically prior record on disk for.</param>
      /// <param name="priorOffset">Offset of the prior record on the disk</param>
      /// <return>record number of the prior record on the disk</return>
      /// </summary>
      public long GetPriorPhysical(long index, out long priorOffset)
      {
         // set prior physical to be the zero record that is always there
         long RC = 0; 

         if (!IsValid())
         {
            throw new ApplicationException("Index not ready");
         }

         if (index < 1 || index > Nodes)
         {
            throw new ArgumentException("Index is out of bounds.");
         }

         // set prior offset to the known first record in the file which is immediately after the file header
         priorOffset = FILEHEADER.Size; 

         // store the offset of the record we are looking for the prior physical record of
         long indexOffset = indexAccessor[index].offset;

         // now check each node looking for the closest less that offset
         for (long i = 0; i <= indexAccessor.Nodes; i++)
         {
            // if the offset is less than our records offset but greater than the best found so far it is better
            if (indexAccessor[i].offset < indexOffset && indexAccessor[i].offset > priorOffset)
            {
               // store this records details
               RC = i;
               priorOffset = indexAccessor[i].offset;
            }
         }

         // return the found record
         return RC;
      }

      /// <summary>
      /// Update the next record offset of the give record to the newly provided offset
      /// <param name="file">The physical file stream.</param>
      /// <param name="index">Index of the record that we need to update.</param>
      /// <param name="nextOffset">The new nextOffset value</param>
      /// </summary>
      public void SetNextOffset(Stream file, long index, long nextOffset)
      {
         if (!IsValid())
         {
            throw new ApplicationException("Index not ready");
         }

         if (index < 0 || index > Nodes)
         {
            throw new ArgumentException("Index is out of bounds.");
         }

         try
         {

            // if the next offset is really a change
            if (indexAccessor[index].nextOffset != nextOffset)
            {
               // create a data record header for the updated node
               DATARECORDHEADER drh = new DATARECORDHEADER();

               // fill it with values from the index
               drh.nextRecordOffset = nextOffset;
               drh.id = long.MinValue;
               drh.length = indexAccessor[index].size;
               drh.recNum = index;
               drh.seqNum = indexAccessor[index].seqNum;

               // seek to the record location on the disk
               file.Seek(indexAccessor[index].offset, SeekOrigin.Begin);

               // write it out
               file.Write(Utility.SerialiseObject(drh), 0, DATARECORDHEADER.Size);

               // update our in memory index with the new next offset
               INDEXITEM ii = indexAccessor[index];
               ii.nextOffset = nextOffset;
               indexAccessor[index] = ii;
            }
         }
         catch(Exception e)
         {
            throw new ApplicationException("Error writing updated nextOffset to file.", e);
         }
      }

      /// <summary>
      /// Sets the data in an index
      /// <param name="index">Record number to set the index information for</param>
      /// <param name="offset">New offset value.</param>
      /// <param name="size">New size value.</param>
      /// <param name="nextOffset">The new nextOffset value</param>
      /// </summary>
      public void SetIndexData(long index, long offset, long size, long nextOffset)
      {
         // check we are valid
         if (!IsValid())
         {
            throw new ApplicationException("Index not ready");
         }

         // check the index
         if (index < 1 || index > Nodes)
         {
            throw new ArgumentException("Index is out of bounds.");
         }

         // save new index information
         INDEXITEM ii = indexAccessor[index];
         ii.offset = offset;
         ii.size = size;
         ii.nextOffset = nextOffset;
         indexAccessor[index] = ii;

         // the data is not written to disk as this happens with the data for efficiency reasons
      }

      /// <summary>
      /// Finds space in the file for the size given
      /// <param name="size">Size of the free space I need to provide.</param>
      /// <param name="offset">Offset of the found free space.</param>
      /// <param name="nextOffset">The offset of the next record after the space</param>
      /// <return>record number of the record before this record</return>
      /// </summary>
      public long FindSpace(long size, out long offset, out long nextOffset)
      {
         // check we are valid
         if (!IsValid())
         {
            throw new ApplicationException("Index not ready");
         }

         // set return values to NIL
         offset = 0;
         nextOffset = 0;

         // set return value 
         long RC = -1;

         // set variables for last record to zero
         long last = 0;
         long lastOffset = 0;

         // search each node for space & the last node
         for (long i = 0; i <= indexAccessor.Nodes; i++)
         {
            // check for space between this record and the next
            if (indexAccessor[i].nextOffset - indexAccessor[i].offset - DATARECORDHEADER.Size - indexAccessor[i].size >= size)
            {
               // space found!!!

               // return the offset of this record and the offset of the next record and this record number
               offset = indexAccessor[i].offset + DATARECORDHEADER.Size + indexAccessor[i].size;
               nextOffset = indexAccessor[i].nextOffset;
               RC = i;

               // exit the for loop
               break;
            }
            else
            {
               // no space so check if this is the largest offset found to date
               if (indexAccessor[i].nextOffset > lastOffset)
               {
                  // it is so save its details
                  last = i;
                  lastOffset = indexAccessor[i].nextOffset;
               }
            }
         }

         // if no space found
         if (RC == -1)
         {
            // no sapce found so use the last record details and return space at the end of the file
            RC = last;
            offset = lastOffset;
            nextOffset = lastOffset + DATARECORDHEADER.Size + size;
         }
   
         return RC;
      }

      /// <summary>
      /// Gets the next sequence number and increments it
      /// <return>A sequence number to use on a record</return>
      /// </summary>
      public long GetNextSequence()
      {
         // increment the sequence number and return it
         return ++indexAccessor.NextSequence;
      }

      /// <summary>
      /// Gets the current slot after incrementing it by the given increment
      /// <param name="increment">Amount of slots after the current slot to return</param>
      /// <return>The current slot + the increment</return>
      /// </summary>
      public long GetNextSlot(long increment)
      {
         // set the return value to default  
         long RC = 1;

         // check we are valid
         if (!IsValid())
         {
            throw new ApplicationException("Index not ready");
         }

         // get the current slot
         RC = indexAccessor.NextSlot;

         // incrmeent it as requested
         RC = RC + increment;

         // wrap it back to 1 if it is too high
         if (RC > indexAccessor.Nodes || RC <= 0)
         {
            RC = 1;
         }

         // save it as the new next slot
         indexAccessor.NextSlot = RC;

         // return it
         return RC;
      }

      #endregion
   };

   /// <summary>
   /// The CircularFile class suports a slot based file with when written to sequentially 
   /// will loop back over itself writing over the oldest record.
   /// </summary>
   public class CircularFile
   {
      #region member variables
      
      // mutex for preventing multiple threads and processes from simulatenously accessing the index and file
      Mutex       mutex = new Mutex(false, "KWCircularFileMutex");

      // the index for the file
      Index       index = null;

      // the name of the on disk file
      string      fileName = string.Empty;

      // the on disk file object
      Stream      file = null;

      // current position in the file for readfirst/readnext operations
      long        currentRead = 0;

      #endregion

      #region constructors
      /// <summary>
      /// Open an existing circular file.
      /// <param name="fileName">Name of the existing circular file to open</param>
      /// </summary>
      public CircularFile(string fileName)
      {
         // call the Initialise function to actually open the file
         Initialise(fileName, -1);
      }
      /// <summary>
      /// Open/Create a circular file.
      /// <param name="fileName">Name of the new/existing circular file to open</param>
      /// <param name="nodes">Number of nodes to create in a new file. If the file already exists then this parameter is ignored.</param>
      /// </summary>
      public CircularFile(string fileName, long nodes)
      {
         // call the Initialise function to actually open the file
         Initialise(fileName, nodes);
      }
      #endregion

      #region IDisposable
      /// <summary>
      /// Dispose of resources used.
      /// </summary>
      public void Dispose()
      {
         // if we have an index
         if (index != null)
         {
            // get rid of it
            index.Dispose();
            index = null;
         }

         // if we have a file
         if (file != null)
         {
            // close the file
            file.Close();
         }

         // reset values to NIL
         fileName = string.Empty;
      }

      #endregion
      
      #region public methods
      /// <summary>
      /// Sets/gets the current position in the circular file. This only impacts on ReadNext 
      /// operations. The oldest record will still be the one overwritten in a Write operation.
      /// <exception cref="System.ArgumentException">Thrown if the position is out of bounds.</exception>
      /// <return>Current slot in the circular file<return>
      /// </summary>
      public long Position
      {
         get
         {
            // return the current position
            return currentRead;
         }
         set
         {
            // validate the position is valid
            if (value <= 0 || value > index.Nodes)
            {
               throw new ArgumentException("Position out of bounds");
            }

            // set the new position
            currentRead = value;
         }
      }
      /// <summary>
      /// Closes the file.
      /// </summary>
      public void Close()
      {
         // Dispose of our resources
         Dispose();
      }

      /// <summary>
      /// Gets the number of slots in the circular file.
      /// <returns>Number of slots in the circular file.</returns>
      /// </summary>
      public long Length
      {
         get
         {
            return this.index.Nodes;
         }
      }

      /// <summary>
      /// Check the validity of this circular file.
      /// <returns><c>true</c> if the circular file is a valid circular file.</returns>
      /// </summary>
      public bool IsValid()
      {
         // only valid if we have an index, a physical file
         return (index != null && file != null && index.IsValid());
      }

      /// <summary>
      /// Writes an object to the next available slot.
      /// <param name="obj">Object to write.</param>
      /// </summary>
      public void Write(object obj)
      {
         // try to lock the file
         if (mutex.WaitOne())
         {
            try
            {
               // get the next record to write to
               long current = this.index.GetNextSlot(1);

               // now write it out
               WriteItem(current, obj);
            }
            finally
            {
               mutex.ReleaseMutex();
            }
         }
      }

      /// <summary>
      /// Writes a byte array to the nominated slot.
      /// <param name="index">The slot that we should write the byte array to.</param>
      /// <param name="buffer">Byte array to write.</param>
      /// </summary>
      public void WriteItem(long index, byte[] buffer)
      {
         // if we are not valid then reading anything is impossible
         if (!IsValid())
         {
            throw new ApplicationException("File is not ready for writing.");
         }

         // lock the file
         if (mutex.WaitOne())
         {
            try
            {
               // valid the requested slot
               if (index <= 0 || index > this.index.Nodes)
               {
                  throw new ArgumentException("index out of bounds");
               }

               // get info on current location of this record
               long offset;
               long size;
               long nextRecordOffset;
               this.index.GetIndexData(index, out offset, out size, out nextRecordOffset);

               // check the current location. Is it big enough
               if (nextRecordOffset - offset  < DATARECORDHEADER.Size + buffer.GetLength(0))
               {
                  // no it is too small

                  // update prior record to account for extra unused space
                  
                  // get the record stored physically before this one
                  long priorOffset;
                  long prior = this.index.GetPriorPhysical(index, out priorOffset);

                  // update the prior record with the new next record offset
                  this.index.SetNextOffset(file, prior, nextRecordOffset);

                  // if the program crashes between here ========= 
      
                  // find some space to store our new record in
                  long dummy;
                  prior = this.index.FindSpace(DATARECORDHEADER.Size + buffer.GetLength(0), out offset, out nextRecordOffset);

                  // get the record prior to the slot we are about to put our new record in
                  this.index.GetIndexData(prior, out priorOffset, out dummy, out dummy);

                  // tell the new prior record where we start
                  try
                  {
                     this.index.SetNextOffset(file, prior, offset);
                  }
                  catch(Exception e)
                  {
                     // this is very bad as the file is now corrupt
                     throw new ApplicationException("Error writing to file ... file is now corrupt", e);
                  }
               }

               // write new data

               // move to the location where the data is to be written
               file.Seek(offset, SeekOrigin.Begin);

               // build the data record header
               DATARECORDHEADER drh = new DATARECORDHEADER();
               drh.id = long.MinValue;
               drh.recNum = index;
               drh.seqNum = this.index.GetNextSequence();
               drh.length = buffer.GetLength(0);
               drh.nextRecordOffset = nextRecordOffset;

               // write the data record header
               file.Write(Utility.SerialiseObject(drh), 0, DATARECORDHEADER.Size);

               // write the data
               file.Write(buffer, 0, buffer.GetLength(0));
         
               // force it to disk
               file.Flush();

               // update the index
               try
               {
                  this.index.SetIndexData(index, offset, buffer.GetLength(0), nextRecordOffset);
               }
               catch(Exception e)
               {
                  // could not write to index
                  throw new ApplicationException("Error updating index. File should be fine but you need to exit all processes using the file to recover it", e);
               }

               // ====== and here the file will be corrupted
            }
            catch(Exception e)
            {
               throw new ApplicationException("Error writing record to file -- circular file is now almost certainly corrupt.", e);
            }
            finally
            {
               // let go of the file
               mutex.ReleaseMutex();
            }
         }
         else
         {
            throw new ApplicationException("Could not access file ... someone else is locking it.");
         }
      }

      /// <summary>
      /// Writes an object to the nominated slot.
      /// The object must be serialisable.
      /// <param name="index">The slot that we should write the object to.</param>
      /// <param name="obj">Serialisable object to write.</param>
      /// </summary>
      public void WriteItem(long index, object obj)          
      {
         // serialise the object to a byte array and write it
         WriteItem(index, Utility.SerialiseObject(obj));
      }

      /// <summary>
      /// Writes a string to the nominated slot.
      /// <param name="index">The slot that we should write the string to.</param>
      /// <param name="s">String to write.</param>
      /// </summary>
      public void WriteItem(long index, string s)
      {
         // cast it to an object and write it
         WriteItem(index, (object)s);
      }

      /// <summary>
      /// Reads the nominated slot and returns a byte array for it.
      /// <param name="index">The slot that we should read the item from.</param>
      /// <returns>The slots contents as a byte array.</returns>
      /// </summary>
      public byte[] ReadByteArrayItem(long index)
      {
         byte[] buffer = null;

         // if we are not valid then reading anything is impossible
         if (!IsValid())
         {
            return null;
         }

         // lock the file
         if (mutex.WaitOne())
         {
            try
            {
               if (index <= 0 || index > this.index.Nodes)
               {
                  throw new ArgumentException("index out of bounds");
               }

               // get the index info for our record
               long offset;
               long size;
               long dummy;
               this.index.GetIndexData(index, out offset, out size, out dummy);

               // read the data

               // skip over the data record header
               offset = offset + DATARECORDHEADER.Size;

               // seek to the data location
               file.Seek(offset, SeekOrigin.Begin);
      
               // alocate a buffer for the data
               buffer = new byte[size];

               // read the data into the buffer
               file.Read(buffer, 0, (int)size);
            }
            catch(Exception e)
            {
               // something is seriously wrong
               throw new ApplicationException("Could not get index record for requested item.", e);
            }
            finally
            {
               // let go of the file
               mutex.ReleaseMutex();
            }
         }
         else
         {
            throw new ApplicationException("Could not access file ... someone else is locking it.");
         }

         return buffer;
      }

      /// <summary>
      /// Reads the nominated slot and returns the object contained within it. 
      /// The slot must have had a serializable object written to it for this to work.
      /// <param name="index">The slot that we should read the item from.</param>
      /// <returns>The slots contents as an object.</returns>
      /// </summary>
      public object ReadObjectItem(long index)
      {
         // read the item as a byte array
         byte[] buffer = ReadByteArrayItem(index);

         // if buffer is empty
         if (buffer.GetLength(0) == 0)
         {
            // return no object
            return null;
         }
         else
         {
            // read the record and deserialise it into an object
            return Utility.DeserialiseObject(buffer);
         }
      }

      /// <summary>
      /// Reads the nominated slot and returns the string contained within it. 
      /// The slot must have had a string written to it for this to work.
      /// <param name="index">The slot that we should read the item from.</param>
      /// <returns>The slots contents as a string.</returns>
      /// </summary>
      public string ReadItem(long index)
      {
         // read the item as an object
         object obj = ReadObjectItem(index);

         // if there was no object
         if (obj == null)
         {
            // return an empty string
            return string.Empty;
         }
         else
         {
            // read the item as an object and cast it to a string
            return (string)obj;
         }
      }

      /// <summary>
      /// Returns the oldest slot in the circular file as a byte array.
      /// <return>The first byte array in the circular file</return>
      /// </summary>
      public byte[] ReadFirstByteArray()
      {
         byte[] buffer = null;

         // try to lock the file
         if (mutex.WaitOne())
         {
            try
            {
               // get the current record in the file
               long current = this.index.GetNextSlot(0);

               // move to the next slot as this must be the oldest ... first
               current++;
               if (current > this.index.Nodes)
               {
                  current = 1;
               }

               // make that the current read
               currentRead = current;

               buffer = ReadByteArrayItem(currentRead);

               // move to the next
               currentRead++;

               // if we are now past the last node
               if (currentRead > this.index.Nodes)
               {
                  // report end of file
                  throw new EndOfStreamException("End of file");
               }
            }
            finally
            {
               // release our lock
               mutex.ReleaseMutex();
            }
         }
         else
         {
            throw new ApplicationException("Could not access file ... someone else is locking it.");
         }

         return buffer;
      }

      /// <summary>
      /// Returns the oldest slot in the circular file as an object. This slot must have a serialisable object in it.
      /// <return>The first serialisable object in the circular file</return>
      /// </summary>
      public object ReadFirstObject()
      {
         // get the item as a byte array
         byte[] buffer = ReadFirstByteArray();

         // if buffer is empty
         if (buffer.GetLength(0) == 0)
         {
            return null;
         }
         else
         {
            // return the object deserialised from the first byte array
            return Utility.DeserialiseObject(buffer);
         }
      }

      /// <summary>
      /// Returns the oldest slot in the circular file as a string. This slot must have a string in it.
      /// <return>The first string in the circular file</return>
      /// </summary>
      public string ReadFirst()
      {
         // read the item as an object
         object obj = ReadFirstObject();

         // if there was no object
         if (obj == null)
         {
            // return an empty string
            return string.Empty;
         }
         else
         {
            // return the first object cast as a string
            return (string)obj;
         }
      }

      /// <summary>
      /// Returns the next slot in the circular file as a byte array.
      /// <return>The next byte array in the circular file</return>
      /// </summary>
      public byte[] ReadNextByteArray()
      {
         byte[] buffer = null;

         // try to lock the file
         if (mutex.WaitOne())
         {
            try
            {
               if (currentRead != 0)
               {
                  // find the last item in the file
                  long last = this.index.GetNextSlot(0) + 1;
                  if (last > this.index.Nodes)
                  {
                     last = 1;
                  }

                  // if we are up to the last item
                  if (currentRead == last)
                  {
                     // we are at the end of file
                     currentRead = 0;
                     throw new EndOfStreamException("End of file");
                  }
                  else
                  {
                     // read the item
                     buffer = ReadByteArrayItem(currentRead);

                     // move to the next node
                     currentRead++;
                     if (currentRead > this.index.Nodes)
                     {
                        currentRead = 1;
                     }
                  }

               }
               else
               {
                  // we must have a current position before we can read the next
                  throw new EndOfStreamException("File does not have a current position");
               }
            }
            finally
            {
               // release our lock
               mutex.ReleaseMutex();
            }
         }
         else
         {
            throw new ApplicationException("Could not access file ... someone else is locking it.");
         }

         return buffer;
      }

      /// <summary>
      /// Returns the next slot in the circular file as a serialisable object. This slot must have a serialisable object in it.
      /// <return>The next serialisable object in the circular file</return>
      /// </summary>
      public object ReadNextObject()
      {
         // get the item as a byte array
         byte[] buffer = ReadNextByteArray();

         // if buffer is empty
         if (buffer.GetLength(0) == 0)
         {
            return null;
         }
         else
         {
            // return the object deserialised from the first byte array
            return Utility.DeserialiseObject(buffer);
         }
      }

      /// <summary>
      /// Returns the next slot in the circular file as a string. This slot must have a string in it.
      /// <return>The next string in the circular file</return>
      /// </summary>
      public string ReadNext()
      {
         // read the item as an object
         object obj = ReadNextObject();

         // if there was no object
         if (obj == null)
         {
            // return an empty string
            return string.Empty;
         }
         else
         {
            // return the first object cast as a string
            return (string)obj;
         }
      }

      #endregion

      #region Private methods
      /// <summary>
      /// Opens/Creates the file
      /// <param name="fileName">the name of the file to open/create.</param>
      /// <param name="nodes">The number of nodes to put in the created file. If the file exists this parameter is ignored.</param>
      /// </summary>
      void Initialise(string fileName, long nodes)
      {
         if (fileName == string.Empty)
         {
            throw new ArgumentException("fileName must be specified");
         }

         // save the file name
         this.fileName = fileName;

         // initialise values
         file = null;
         index = null;

         // grab the lock
         if (mutex.WaitOne())
         {
            try
            {
               // if file does not exist
               if (!File.Exists(fileName))
               {
                  // check they have specified the number of nodes
                  if (nodes < 1)
                  {
                     throw new ArgumentException("nodes must be specified when file is created.");
                  }

                  // need to create the file
                  file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                  // write an empty file

                  // write the file header
                  FILEHEADER fh = new FILEHEADER();
                  fh.nodes = nodes;
                  byte[] fhb = Utility.SerialiseObject(fh);
                  file.Write(fhb, 0, FILEHEADER.Size);

                  // record where we are up to in the file
                  long offset = FILEHEADER.Size;

                  // create the data record header and initialise the constant value for the empty file
                  DATARECORDHEADER drh = new DATARECORDHEADER();
                  drh.id = long.MinValue;
                  drh.length = 0;

                  // write out the correct number of blank records
                  for (long i = 0; i <= nodes; i++)
                  {
                     // fill in the variable data record header fields
                     drh.recNum = i;
                     drh.seqNum = -1*i - 1;
                     drh.nextRecordOffset = offset + DATARECORDHEADER.Size;

                     byte[] drhb = Utility.SerialiseObject(drh);

                     // write the record to disk
                     file.Write(drhb, 0, DATARECORDHEADER.Size);

                     // increment where we are up to in the file
                     offset = drh.nextRecordOffset;
                  }

                  // force the data to disk
                  file.Flush();
               }
               else
               {
                  // open the file
                  file = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
               }

               // create an index on the file
               index = new Index(fileName, file);
            }
            finally
            {
               mutex.ReleaseMutex();
            }
         }
         else
         {
            throw new ApplicationException("Could not access file ... someone else is locking it.");
         }
      }

      #endregion
   };
   
   /// <summary>
	/// The CircularFileStream class exposes the CircularFileStream object as a stream for use by TraceListeners and the like.
	/// </summary>
	public class CircularFileStream : Stream, IDisposable
	{
      #region member variables
      // log file object the stream will use
      CircularFile circularFile = null;

      // flag to indicate that byte[] writes should be converted to strings before writing them
      bool writeAsStrings = false;

      #endregion

      #region constructors
      /// <summary>
      /// Constructs a stream on an existing circular file.
      /// <param name="fileName">Name of the file to be opened.</param>
      /// </summary>
      public CircularFileStream(string fileName)
		{
         // open an existing circular file.
         circularFile = new CircularFile(fileName);
		}
      /// <summary>
      /// Constructs a stream on a new or existing circular file.
      /// <param name="fileName">Name of the file to be opened or created.</param>
      /// <param name="size">Number of records in the circular file. This is only used if the file needs to be created.</param>
      /// </summary>
      public CircularFileStream(string fileName, long size)
      {
         // open or create a circular file
         circularFile = new CircularFile(fileName, size);
      }
      #endregion

      #region IDisposable
      /// <summary>
      /// Dispose of resources used.
      /// </summary>
      public void Dispose()
      {
         // if we have a circular file
         if (circularFile != null)
         {
            // dispose the circular file
            circularFile.Dispose();
            circularFile = null;
         }
      }
      #endregion

      #region Stream
      /// <summary>
      /// Close the circular file stream.
      /// </summary>
      public override void Close()
      {
         // Dispose of our resources
         Dispose();
      }
      /// <summary>
      /// Check if we can be read from.
      /// <returns><c>true</c> if the stream can be read from.</returns>
      /// </summary>
      public override bool CanRead
      {
         get
         {
            return IsValid;
         }
      }

      /// <summary>
      /// Check if we can seek to specific records in this stream.
      /// <returns><c>true</c> if the stream can be seeked on.</returns>
      /// </summary>
      public override bool CanSeek
      {
         get
         {
            return IsValid;
         }
      }

      /// <summary>
      /// Check if we can be written to.
      /// <returns><c>true</c> if the stream can be written to.</returns>
      /// </summary>
      public override bool CanWrite
      {
         get
         {
            return IsValid;
         }
      }

      /// <summary>
      /// Flush any unwritten data to disk. You do not need to call this as flushing is handled automatically after every write.
      /// </summary>
      public override void Flush()
      {
         // don't need to do anything as this stream always flushes on writes
      }

      /// <summary>
      /// Get the number of slots in the circular file.
      /// <returns>The number of slots in the circular file.</returns>
      /// </summary>
      public override long Length
      {
         get
         {
            // return the value from the circular file.
            return circularFile.Length;
         }
      }

      /// <summary>
      /// Get/set the current slot in the circular file. Slot numbers are numbered 1 - <c>CircularFileStream.Length</c>.
      /// <returns>The slot number of the current position in the circular file.</returns>
      /// </summary>
      public override long Position
      {
         get
         {
            // return the position from the circular file
            return circularFile.Position;
         }
         set
         {
            // set the position on the circular file
            circularFile.Position = value;
         }
      }

      /// <summary>
      /// Reads the next record from the circular file
      /// <param name="buffer">The buffer in which to place the data</param>
      /// <param name="offset">This is ignored</param>
      /// <param name="count">This is ignored</param>
      /// <returns>The length of the read data.</returns>
      /// </summary>
      public override int Read(byte[] buffer, int offset, int count)
      {
         // read it from the circular file
         buffer = circularFile.ReadNextByteArray();

         // return the length read
         return buffer.GetLength(0);
      }

      /// <summary>
      /// Writes a record int the next slot in the circular file
      /// <param name="buffer">The buffer containing the data to write</param>
      /// <param name="offset">This is ignored</param>
      /// <param name="count">This is ignored</param>
      /// <returns>The length of the read data.</returns>
      /// </summary>
      public override void Write(byte[] buffer, int offset, int count)
      {
         if (writeAsStrings == true)
         {
            // convert it to a string
            MemoryStream ms = new MemoryStream(buffer, offset, count);
            StreamReader sr = new StreamReader(ms);
            string s = sr.ReadToEnd();

            // trim off the crap
            // string sTrimmed = s.Substring(0, s.IndexOf((char)0));

            circularFile.Write(s);
         }
         else
         {
            // write it to the circular file.
            circularFile.Write(buffer);
         }
      }

      /// <summary>
      /// Seeks to a slot in the circular file
      /// <param name="position">The position relative to the <c>origin</c> to seek to</param>
      /// <param name="origin">The origin the position is relative to</param>
      /// <returns>The new current position in the file.</returns>
      /// </summary>
      public override long Seek(long position, SeekOrigin origin)
      {
         // depending on the origin
         switch(origin)
         {
            case SeekOrigin.Begin:
               // set the position to the one provided
               Position = position;
               break;
            case SeekOrigin.Current:
               // add the position to the current position
               Position += position;
               break;
            case SeekOrigin.End:
               // subtract the position from the length of the file
               Position = Length - position;
               break;
         }

         return Position;
      }

      /// <summary>
      /// I am required to support this method but it makes no sense in this context.
      /// <param name="length">This parameter is ignored</param>
      /// <exception cref="System.NotSupportedException">This method always throws this exception.</exception>
      /// </summary>
      public override void SetLength(long Length)
      {
         // once created this is not valid so I am hiding this method
         throw new NotSupportedException("SetLength cannot be used on CircularFileStream objects");
      }
      #endregion

      #region Extra public methods

      /// <summary>
      /// Controls whether byte[] write requests are transformed into string objects before writing
      /// <returns><c>true</c> if the byte[] objects are to be transformed.</returns>
      /// </summary>
      public bool WriteAsString
      {
         get
         {
            return writeAsStrings;
         }
         set
         {
            writeAsStrings = value;
         }
      }

      /// <summary>
      /// Check the validity of this stream.
      /// <returns><c>true</c> if the stream is a valid stream.</returns>
      /// </summary>
      public bool IsValid
      {
         get
         {
            // we are valid when we have a valid underlying circular file
            return circularFile != null && circularFile.IsValid();
         }
      }
      /// <summary>
      /// Reads the next string from the circular file
      /// <returns>The string read.</returns>
      /// </summary>
      public string ReadString()
      {
         // read it as an object and cast it to a string
         return (string)ReadObject();
      }

      /// <summary>
      /// Reads the next serializable object from the circular file
      /// <returns>The object read.</returns>
      /// </summary>
      public object ReadObject()
      {
         // read it as an object and cast it to a string
         return circularFile.ReadNextObject();
      }

      /// <summary>
      /// Writes a string into the next slot in the circular file.
      /// <param>The string to write.</param>
      /// </summary>
      public void Write(string s)
      {
         // use the object version of this method
         Write((object)s);
      }

      /// <summary>
      /// Writes a serializable object into the next slot in the circular file.
      /// <param name="obj">The serializable object to write.</param>
      /// </summary>
      public void Write(object obj)
      {
         // write it to the circular file
         circularFile.Write(obj);
      }
      #endregion

   }
}
