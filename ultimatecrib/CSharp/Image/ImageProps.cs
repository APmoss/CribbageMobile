using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Image
{
	/// <summary>
	/// This form is used by applications which want users to be able to easily change
	/// the way an image is rendered by the image factory
	/// </summary>
   public class ImageProps : System.Windows.Forms.Form
   {
      #region Member Variables
      string _fileName;
      #endregion

      #region Generated Member Variables
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Button Ok;
      private System.Windows.Forms.Button Cancel;
      private System.Windows.Forms.PictureBox pictureBox;
      private System.Windows.Forms.TrackBar ctlCropLeft;
      private System.Windows.Forms.ComboBox ctlRender;
      private System.Windows.Forms.TrackBar ctlCropRight;
      private System.Windows.Forms.TrackBar ctlCropBottom;
      private System.Windows.Forms.TrackBar ctlCropTop;
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.Container components = null;
      #endregion

      #region Constructors
      /// <summary>
      /// Creates the form class
      /// </summary>
      /// <param name="FileName">File on which Image Properties Apply</param>
      /// <param name="Render">How the image will be rendered</param>
      /// <param name="CropLeft">Amount to chop off the left edge of the image</param>
      /// <param name="CropRight">Amount to chop off the right edge of the image</param>
      /// <param name="CropTop">Amount to chop off the top edge of the image</param>
      /// <param name="CropBottom">Amount to chop off the bottom edge of the image</param>
      public ImageProps(string FileName, string Render, int CropLeft, int CropRight, int CropTop, int CropBottom)
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         // save the file name
         _fileName = FileName;

         // load the image
         Bitmap bitmap = new Bitmap(FileName);

         // Set the maximums to be the image size
         ctlCropLeft.Maximum = bitmap.Size.Width;
         ctlCropRight.Maximum = bitmap.Size.Width;
         ctlCropTop.Maximum = bitmap.Size.Height;
         ctlCropBottom.Maximum = bitmap.Size.Height;
         
         // Set the initial values to those passed in
         ctlRender.Text = Render;
         ctlCropLeft.Value = CropLeft;
         ctlCropRight.Value = ctlCropRight.Maximum - CropRight;
         ctlCropTop.Value = CropTop;
         ctlCropBottom.Value = ctlCropBottom.Maximum - CropBottom;
      }
      #endregion

      #region Public Member Functions
      /// <summary>
      /// Get the users Render option
      /// </summary>
      public string Render
      {
         get
         {
            return ctlRender.Text;
         }
      }

      /// <summary>
      /// Get the users crop left
      /// </summary>
      public int CropLeft
      {
         get
         {
            return ctlCropLeft.Value;
         }
      }

      /// <summary>
      /// Get the users crop right
      /// </summary>
      public int CropRight
      {
         get
         {
            return ctlCropRight.Maximum - ctlCropRight.Value;
         }
      }

      /// <summary>
      /// Get the users crop top
      /// </summary>
      public int CropTop
      {
         get
         {
            return ctlCropTop.Value;
         }
      }

      /// <summary>
      /// Get the users crop bottom
      /// </summary>
      public int CropBottom
      {
         get
         {
            return ctlCropBottom.Maximum - ctlCropBottom.Value;
         }
      }

      /// <summary>
      /// Update the image on the screen to reflect the current user settings
      /// </summary>
      void UpdateImage()
      {
         pictureBox.Image = ImageFactory.GetImage("Bitmap", _fileName, 0, pictureBox.Size, CropLeft, CropRight, CropTop, CropBottom, Render, Color.Transparent);
      }
      #endregion

      #region IDisposable
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
      #endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.label1 = new System.Windows.Forms.Label();
         this.ctlCropLeft = new System.Windows.Forms.TrackBar();
         this.ctlRender = new System.Windows.Forms.ComboBox();
         this.ctlCropRight = new System.Windows.Forms.TrackBar();
         this.ctlCropBottom = new System.Windows.Forms.TrackBar();
         this.ctlCropTop = new System.Windows.Forms.TrackBar();
         this.pictureBox = new System.Windows.Forms.PictureBox();
         this.Cancel = new System.Windows.Forms.Button();
         this.Ok = new System.Windows.Forms.Button();
         ((System.ComponentModel.ISupportInitialize)(this.ctlCropLeft)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.ctlCropRight)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.ctlCropBottom)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.ctlCropTop)).BeginInit();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(8, 232);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(48, 24);
         this.label1.TabIndex = 5;
         this.label1.Text = "Render";
         this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // ctlCropLeft
         // 
         this.ctlCropLeft.AutoSize = false;
         this.ctlCropLeft.Location = new System.Drawing.Point(0, 192);
         this.ctlCropLeft.Name = "ctlCropLeft";
         this.ctlCropLeft.Size = new System.Drawing.Size(184, 16);
         this.ctlCropLeft.TabIndex = 1;
         this.ctlCropLeft.TickStyle = System.Windows.Forms.TickStyle.None;
         this.ctlCropLeft.ValueChanged += new System.EventHandler(this.CropLeft_ValueChanged);
         // 
         // ctlRender
         // 
         this.ctlRender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.ctlRender.DropDownWidth = 112;
         this.ctlRender.Items.AddRange(new object[] {
                                                       "Tile",
                                                       "Scale",
                                                       "Centre"});
         this.ctlRender.Location = new System.Drawing.Point(64, 232);
         this.ctlRender.Name = "ctlRender";
         this.ctlRender.Size = new System.Drawing.Size(112, 21);
         this.ctlRender.TabIndex = 6;
         this.ctlRender.SelectedIndexChanged += new System.EventHandler(this.Render_SelectedIndexChanged);
         // 
         // ctlCropRight
         // 
         this.ctlCropRight.AutoSize = false;
         this.ctlCropRight.Location = new System.Drawing.Point(0, 208);
         this.ctlCropRight.Name = "ctlCropRight";
         this.ctlCropRight.Size = new System.Drawing.Size(184, 16);
         this.ctlCropRight.TabIndex = 2;
         this.ctlCropRight.TickStyle = System.Windows.Forms.TickStyle.None;
         this.ctlCropRight.Scroll += new System.EventHandler(this.CropRight_Scroll);
         // 
         // ctlCropBottom
         // 
         this.ctlCropBottom.AutoSize = false;
         this.ctlCropBottom.Location = new System.Drawing.Point(208, 0);
         this.ctlCropBottom.Name = "ctlCropBottom";
         this.ctlCropBottom.Orientation = System.Windows.Forms.Orientation.Vertical;
         this.ctlCropBottom.Size = new System.Drawing.Size(16, 184);
         this.ctlCropBottom.TabIndex = 4;
         this.ctlCropBottom.TickStyle = System.Windows.Forms.TickStyle.None;
         this.ctlCropBottom.Scroll += new System.EventHandler(this.CropBottom_Scroll);
         // 
         // ctlCropTop
         // 
         this.ctlCropTop.AutoSize = false;
         this.ctlCropTop.Location = new System.Drawing.Point(192, 0);
         this.ctlCropTop.Name = "ctlCropTop";
         this.ctlCropTop.Orientation = System.Windows.Forms.Orientation.Vertical;
         this.ctlCropTop.Size = new System.Drawing.Size(16, 184);
         this.ctlCropTop.TabIndex = 3;
         this.ctlCropTop.TickStyle = System.Windows.Forms.TickStyle.None;
         this.ctlCropTop.Scroll += new System.EventHandler(this.CropTop_Scroll);
         // 
         // pictureBox
         // 
         this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.pictureBox.Location = new System.Drawing.Point(8, 8);
         this.pictureBox.Name = "pictureBox";
         this.pictureBox.Size = new System.Drawing.Size(176, 168);
         this.pictureBox.TabIndex = 0;
         this.pictureBox.TabStop = false;
         // 
         // Cancel
         // 
         this.Cancel.Location = new System.Drawing.Point(136, 264);
         this.Cancel.Name = "Cancel";
         this.Cancel.Size = new System.Drawing.Size(80, 24);
         this.Cancel.TabIndex = 7;
         this.Cancel.Text = "Cancel";
         // 
         // Ok
         // 
         this.Ok.Location = new System.Drawing.Point(48, 264);
         this.Ok.Name = "Ok";
         this.Ok.Size = new System.Drawing.Size(80, 24);
         this.Ok.TabIndex = 7;
         this.Ok.Text = "Ok";
         // 
         // ImageProps
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(224, 297);
         this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                      this.Cancel,
                                                                      this.Ok,
                                                                      this.ctlRender,
                                                                      this.label1,
                                                                      this.ctlCropBottom,
                                                                      this.ctlCropTop,
                                                                      this.ctlCropRight,
                                                                      this.ctlCropLeft,
                                                                      this.pictureBox});
         this.Name = "ImageProps";
         this.Text = "Image Properties";
         ((System.ComponentModel.ISupportInitialize)(this.ctlCropLeft)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.ctlCropRight)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.ctlCropBottom)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.ctlCropTop)).EndInit();
         this.ResumeLayout(false);

      }
		#endregion

      #region Event Handlers
      /// <summary>
      /// Handle render option changed
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void Render_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         // update the image
         UpdateImage();

         // ask for a redraw
         pictureBox.Invalidate();
      }

      /// <summary>
      /// Handle crop left option changed
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void CropLeft_ValueChanged(object sender, System.EventArgs e)
      {
         // update the image
         UpdateImage();

         // ask for a redraw
         pictureBox.Invalidate();
      }

      /// <summary>
      /// Handle crop right option changed
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void CropRight_Scroll(object sender, System.EventArgs e)
      {
         // update the image
         UpdateImage();

         // ask for a redraw
         pictureBox.Invalidate();
      }

      /// <summary>
      /// Handle crop top option changed
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void CropTop_Scroll(object sender, System.EventArgs e)
      {
         // update the image
         UpdateImage();

         // ask for a redraw
         pictureBox.Invalidate();
      }

      /// <summary>
      /// Handle crop bottom option changed
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void CropBottom_Scroll(object sender, System.EventArgs e)
      {
         // update the image
         UpdateImage();

         // ask for a redraw
         pictureBox.Invalidate();
      }
      #endregion
	}
}
