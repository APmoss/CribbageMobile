using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CribbageBoardUnitTests
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
   public class Form1 : System.Windows.Forms.Form
   {
      bool f = true;
      
      private System.Windows.Forms.Button Ok;
      private System.Windows.Forms.Button button1;
      private CribbageBoard.CribbageBoard cribbageBoard1;
      private System.Windows.Forms.Timer timer1;
      private System.Windows.Forms.TextBox textBox1;
      private System.ComponentModel.IContainer components;

      public Form1()
      {
         //
         // Required for Windows Form Designer support
         //
         InitializeComponent();

         cribbageBoard1.SetPlayerName(1, "Player1");
         cribbageBoard1.SetPlayerName(2, "Player2");
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
         this.Ok = new System.Windows.Forms.Button();
         this.button1 = new System.Windows.Forms.Button();
         this.cribbageBoard1 = new CribbageBoard.CribbageBoard();
         this.timer1 = new System.Windows.Forms.Timer(this.components);
         this.textBox1 = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // Ok
         // 
         this.Ok.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
         this.Ok.Location = new System.Drawing.Point(64, 492);
         this.Ok.Name = "Ok";
         this.Ok.TabIndex = 0;
         this.Ok.Text = "&Ok";
         this.Ok.Click += new System.EventHandler(this.Ok_Click);
         // 
         // button1
         // 
         this.button1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
         this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.button1.Location = new System.Drawing.Point(144, 492);
         this.button1.Name = "button1";
         this.button1.TabIndex = 1;
         this.button1.Text = "&Cancel";
         this.button1.Click += new System.EventHandler(this.button1_Click);
         // 
         // cribbageBoard1
         // 
         this.cribbageBoard1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right);
         this.cribbageBoard1.BoardName = "David 1";
         this.cribbageBoard1.DockPadding.All = 5;
         this.cribbageBoard1.Location = new System.Drawing.Point(8, 8);
         this.cribbageBoard1.MaxScore = CribbageBoard.MAXSCORE.ONETWENTY;
         this.cribbageBoard1.Name = "cribbageBoard1";
         this.cribbageBoard1.Offset = new System.Drawing.Size(0, 0);
         this.cribbageBoard1.ShowToolTip = true;
         this.cribbageBoard1.Size = new System.Drawing.Size(228, 476);
         this.cribbageBoard1.TabIndex = 2;
         this.cribbageBoard1.Click += new System.EventHandler(this.cribbageBoard1_Click);
         // 
         // timer1
         // 
         this.timer1.Enabled = true;
         this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
         // 
         // textBox1
         // 
         this.textBox1.Location = new System.Drawing.Point(8, 496);
         this.textBox1.MaxLength = 3;
         this.textBox1.Name = "textBox1";
         this.textBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
         this.textBox1.Size = new System.Drawing.Size(48, 20);
         this.textBox1.TabIndex = 3;
         this.textBox1.Text = "0";
         this.textBox1.WordWrap = false;
         this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
         // 
         // Form1
         // 
         this.AcceptButton = this.Ok;
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.CancelButton = this.button1;
         this.ClientSize = new System.Drawing.Size(240, 520);
         this.ControlBox = false;
         this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                      this.textBox1,
                                                                      this.cribbageBoard1,
                                                                      this.button1,
                                                                      this.Ok});
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "Form1";
         this.Text = "Click Ok if board acts ok";
         this.ResumeLayout(false);

      }
		#endregion

      private void Ok_Click(object sender, System.EventArgs e)
      {
         DialogResult = DialogResult.OK;
         Close();
      }

      private void button1_Click(object sender, System.EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         Close();
      }

      void IncrementScore()
      {
         if (cribbageBoard1.GetPlayerScore(1) == Convert.ToInt32(cribbageBoard1.MaxScore))
         {
            cribbageBoard1.SetScore(1, 0, 0);
            cribbageBoard1.SetScore(2, 0, 0);
         }
         else
         {
            cribbageBoard1.AddToScore(1, 1);
            cribbageBoard1.AddToScore(2, 1);
         }
      }

      void DecrementScore()
      {
         if (cribbageBoard1.GetPlayerScore(1) == 0)
         {
            cribbageBoard1.SetScore(1, Convert.ToInt32(cribbageBoard1.MaxScore), Convert.ToInt32(cribbageBoard1.MaxScore)-1);
            cribbageBoard1.SetScore(2, Convert.ToInt32(cribbageBoard1.MaxScore), Convert.ToInt32(cribbageBoard1.MaxScore)-1);
         }
         else
         {
            cribbageBoard1.SetScore(1, cribbageBoard1.GetPlayerScore(1)-2, cribbageBoard1.GetPlayerScore(1)-1);
            cribbageBoard1.SetScore(2, cribbageBoard1.GetPlayerScore(2)-2, cribbageBoard1.GetPlayerScore(2)-1);
         }
      }

      private void cribbageBoard1_Click(object sender, System.EventArgs e)
      {
         timer1.Enabled = false;
         if (f)
         {
            IncrementScore();
            IncrementScore();
         }
         else
         {
            IncrementScore();
         }
         f = true;
         textBox1.Text = cribbageBoard1.GetPlayerScore(1).ToString();
         timer1.Enabled = true;
      }

      private void timer1_Tick(object sender, System.EventArgs e)
      {
         if (f)
         {
            IncrementScore();
            f = false;
         }
         else
         {
            DecrementScore();
            f = true;
         }
      }

      private void textBox1_TextChanged(object sender, System.EventArgs e)
      {
         int i = 0;
         try
         {
            i = Convert.ToInt32(textBox1.Text);
         }
         catch
         {
         }

         if (i == 0)
         {
            cribbageBoard1.SetScore(1, Convert.ToInt32(cribbageBoard1.MaxScore), i);
            cribbageBoard1.SetScore(2, Convert.ToInt32(cribbageBoard1.MaxScore), i);
         }
         else
         {
            cribbageBoard1.SetScore(1, i-1, i);
            cribbageBoard1.SetScore(2, i-1, i);
         }
      }
	}
}
