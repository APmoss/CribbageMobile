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

// ============================================================================

// This file contains the automated unit tests for the classes in the Image 
// assembly (excluding ImageProps as this is UI code).
// These unit tests are by no means perfect but they do provide fairly good
// coverage of non User Interface aspects of the classes.

// To run to unit tests you will need NUnit available from http://www.sourceforge.net

// You will need to have cards32.DLL available in the DLL load path
// You will need to have a bitmap Hearts.BMP available in the bin directory

// ============================================================================

using System;
using NUnit.Framework;
using Image;
using System.Drawing;
using System.IO;

namespace ImageUnitTests
{
   /// <summary>
   /// This class unit tests the Image Class.
   /// </summary>
   [TestFixture]
   public class ImageUnitTests
	{
      [Test]
      public void ResourceDLL()
      {
         ResourceDLL dll = new ResourceDLL("cards32.dll");

         Bitmap b = dll.ExtractBitmap(1);

         Assert.IsNotNull(b);
         Assert.AreEqual(b.Width, 71);
         Assert.AreEqual(b.Height, 96);

         b = dll.ExtractBitmap(1234);
         Assert.IsNull(b);

         dll.Dispose();
      }
      [Test]
      public void ImageFactoryGetImage()
      {
         Bitmap b = ImageFactory.GetImage("Resource", "Cards32.dll", 1, new Size(100,100), 0,0,0,0, "Centre", Color.White);
         Assert.IsNotNull(b);
         Assert.AreEqual(b.Width, 100);
         Assert.AreEqual(b.Height, 100);

         b = ImageFactory.GetImage("Resource", "Cards32.dll", 1, new Size(100,100), 0,0,0,0, "Scale", Color.White);
         Assert.IsNotNull(b);
         Assert.AreEqual(b.Width, 100);
         Assert.AreEqual(b.Height, 100);

         b = ImageFactory.GetImage("Bitmap", "Hearts.BMP", 0, new Size(100,100), 0,0,0,0, "Tile", Color.White);
         Assert.IsNotNull(b);
         Assert.AreEqual(b.Width, 100);
         Assert.AreEqual(b.Height, 100);
      }
      [Test]
      [ExpectedException(typeof(FileNotFoundException))]
      public void ImageFactoryFailure()
      {
         Bitmap b = ImageFactory.GetImage("Bitmap", "Fred.BMP", 0, new Size(100,100), 0,0,0,0, "Tile", Color.White);
      }
      [Test]
      [ExpectedException(typeof(ApplicationException))]
      public void ImageFactoryFailure2()
      {
         Bitmap b = ImageFactory.GetImage("Fred", "Fred.BMP", 0, new Size(100,100), 0,0,0,0, "Tile", Color.White);
      }
      [Test]
      [ExpectedException(typeof(ApplicationException))]
      public void ImageFactoryFailure3()
      {
         Bitmap b = ImageFactory.GetImage("Bitmap", "Hearts.BMP", 0, new Size(100,100), 0,0,0,0, "Bamboozle", Color.White);
      }
      [Test]
      public void ImageFactoryFailure4()
      {
         Bitmap b = ImageFactory.GetImage("Bitmap", "Hearts.BMP", 0, new Size(100,0), 0,0,0,0, "Bamboozle", Color.White);
         Assert.IsNull(b);
      }
      [Test]
      [ExpectedException(typeof(ApplicationException))]
      public void ImageFactoryFailure5()
      {
         Bitmap b = ImageFactory.GetImage("Bitmap", "Hearts.BMP", 0, new Size(100,100), 1000,0,0,0, "Bamboozle", Color.White);
      }
   }
}
