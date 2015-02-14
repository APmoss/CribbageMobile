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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Image
{
   /// <summary>
   /// Class for contstructing images
   /// </summary>
   public class ImageFactory
   {
      #region Public Static Functions

      /// <summary>
      /// Gets a bitmap object for an image using the defined parameters
      /// </summary>
      /// <param name="Type">Resource or image file</param>
      /// <param name="FileName">File containing the source image or if it is a resource type this is the file containing the resource</param>
      /// <param name="Resource">If Type is 'Resource' then this is the reosurce id. Otherwise ignored</param>
      /// <param name="ImageSize">New image size</param>
      /// <param name="CropLeft">Amount to chop off the left of the source image</param>
      /// <param name="CropRight">Amount to chop off the right of the source image</param>
      /// <param name="CropTop">Amount to chop off the top of the source image</param>
      /// <param name="CropBottom">Amount to chop off the bottom of the source image</param>
      /// <param name="Render">How to render the source image onto the target image</param>
      /// <param name="BackColour">Background colour to use if source image does not fill new image</param>
      /// <returns>The created Image</returns>
      static public Bitmap GetImage(string Type, string FileName, int Resource, Size ImageSize, int CropLeft, int CropRight, int CropTop, int CropBottom, string Render, Color BackColour)
      {
         // If we are asked to create a too small image return no image
         if (ImageSize.Width < 1 || ImageSize.Height < 1)
         {
            return null;
         }

         // create an empty bitmap
         Bitmap RC = new Bitmap(ImageSize.Width, ImageSize.Height);

         // raw bitmap
         Bitmap rawBitmap = null;

         // if the type is bitmap
         if (Type == "Bitmap")
         {
            // if there is no file
            if (FileName == "")
            {
               // create a blank bitmap
               rawBitmap = new Bitmap(ImageSize.Width, ImageSize.Height);
            }
            else
            {
               // load the bitmap from the file
               rawBitmap = (Bitmap)Bitmap.FromFile(FileName);
            }
         }
         else
         {
            // load the resource dll
            ResourceDLL dll = new ResourceDLL(FileName);

            // extract the bitmap
            rawBitmap = dll.ExtractBitmap(Resource);

            // throw the dll away
            dll.Dispose();
         }

         // start it off with the backcolour
         Graphics graphics = Graphics.FromImage(RC);
         graphics.FillRectangle(new SolidBrush(BackColour), new Rectangle(new Point(0,0), ImageSize));

         // based on render apply the raw bitmap
         switch(Render)
         {
            case "Scale":
               // scale it to the target ImageSize
               graphics.DrawImage(rawBitmap, new Rectangle(0,0,ImageSize.Width,ImageSize.Height), CropLeft, CropTop, rawBitmap.Width - CropLeft - CropRight, rawBitmap.Height - CropTop - CropBottom, GraphicsUnit.Pixel);
               break;
            case "Tile":
            {
               // tile it over the target ImageSize
               int iY = 0;
               while (iY < ImageSize.Height)
               {
                  int iX = 0;
                  while (iX < ImageSize.Width)
                  {
                     graphics.DrawImage(rawBitmap, new Rectangle(iX,iY,rawBitmap.Width-CropLeft-CropRight,rawBitmap.Height-CropTop-CropBottom), CropLeft, CropTop, rawBitmap.Width - CropRight, rawBitmap.Height - CropBottom, GraphicsUnit.Pixel);
                     iX = iX + rawBitmap.Width;
                  }
                  iY = iY + rawBitmap.Height;
               }
            }
               break;
            case "Centre":
            case "Center":
            {
               // draw it in the middle
               int iX = (ImageSize.Width - (rawBitmap.Width-CropLeft-CropRight))/2;
               int iY = (ImageSize.Height - (rawBitmap.Height-CropTop-CropBottom))/2;
               graphics.DrawImage(rawBitmap, new Rectangle(iX, iY,rawBitmap.Width-CropLeft-CropRight,rawBitmap.Height-CropTop-CropBottom), CropLeft, CropTop, rawBitmap.Width - CropRight - CropLeft, rawBitmap.Height - CropBottom - CropTop, GraphicsUnit.Pixel);
            }
               break;
            default:
               throw new ApplicationException("Unrecognised render type");
         }

         // delete what we don't need
         rawBitmap.Dispose();
         graphics.Dispose();

         return RC;
      }
      #endregion
   }
}
