// Debug Tooklit

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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace DebugToolkit
{
	/// <summary>
	/// Summary description for HangMonitorForm.
	/// </summary>
	public class HangMonitorForm : System.Windows.Forms.Form
	{
      private System.Windows.Forms.Timer timer1;
      private System.ComponentModel.IContainer components;

		public HangMonitorForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         this.Visible = false;
		}

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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.components = new System.ComponentModel.Container();
         this.timer1 = new System.Windows.Forms.Timer(this.components);
         // 
         // timer1
         // 
         this.timer1.Enabled = true;
         this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
         // 
         // HangMonitorForm
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CausesValidation = false;
         this.ClientSize = new System.Drawing.Size(104, 23);
         this.ControlBox = false;
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "HangMonitorForm";
         this.Opacity = 0;
         this.ShowInTaskbar = false;
         this.Text = "Hang Monitor Form";

      }
		#endregion

      private void timer1_Tick(object sender, System.EventArgs e)
      {
         // reset the last visit time
         DebugToolkitBase.TheDebugToolkit.LastVisitTime = DateTime.Now;
      }
	}
}
