using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Player;
using Cards;

namespace PlayerUnitTests
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
      Box b = null;
      private System.Windows.Forms.Button button1;
      private System.Windows.Forms.Button button2;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

         HumanPlayer p = new HumanPlayer(1);
			b = new Box(p);

         Deck d = new Deck();
         b.AddCard((DisplayableCard)d.DealOne());
         b.AddCard((DisplayableCard)d.DealOne());
         b.AddCard((DisplayableCard)d.DealOne());
         b.AddCard((DisplayableCard)d.DealOne());

         b.Show();
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
         this.button1 = new System.Windows.Forms.Button();
         this.button2 = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // button1
         // 
         this.button1.Location = new System.Drawing.Point(8, 8);
         this.button1.Name = "button1";
         this.button1.TabIndex = 0;
         this.button1.Text = "Ok";
         this.button1.Click += new System.EventHandler(this.button1_Click);
         // 
         // button2
         // 
         this.button2.BackColor = System.Drawing.Color.Green;
         this.button2.Location = new System.Drawing.Point(88, 8);
         this.button2.Name = "button2";
         this.button2.Size = new System.Drawing.Size(80, 24);
         this.button2.TabIndex = 1;
         this.button2.Text = "Cancel";
         this.button2.Click += new System.EventHandler(this.button2_Click);
         // 
         // Form1
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.BackColor = System.Drawing.Color.Green;
         this.ClientSize = new System.Drawing.Size(544, 344);
         this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                      this.button2,
                                                                      this.button1});
         this.Name = "Form1";
         this.Text = "Form1";
         this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
         this.ResumeLayout(false);

      }
		#endregion

      private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         b.Display(e.Graphics);
      }

      private void button1_Click(object sender, System.EventArgs e)
      {
         DialogResult = DialogResult.OK;
         Close();
      }

      private void button2_Click(object sender, System.EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         Close();
      }
	}
}
