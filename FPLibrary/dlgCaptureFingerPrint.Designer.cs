using GriauleFingerprintLibrary.Exceptions;
using System.Windows.Forms;
using System;
namespace FPLibrary
{
  partial class dlgCaptureFingerPrint
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        if (capturing)
        {
          try
          {
            FPCore.StopCapture(Sensor);
          }
          catch (FingerprintException ex)
          {
            MessageBox.Show(String.Format("StopCapture Error : {0} {1}", ex.ErrorCode, ex.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.btnCapture = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.paQuality = new System.Windows.Forms.Panel();
      this.lblQuality = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.chkCaptureClose = new System.Windows.Forms.CheckBox();
      this.chkCaptureMultiple = new System.Windows.Forms.CheckBox();
      this.picFinger = new System.Windows.Forms.PictureBox();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.hands1 = new FPLibrary.Hands();
      this.btnCancel = new System.Windows.Forms.Button();
      this.lblPrompt = new System.Windows.Forms.TextBox();
      this.lblFinger = new System.Windows.Forms.TextBox();
      this.paQuality.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picFinger)).BeginInit();
      this.SuspendLayout();
      // 
      // btnCapture
      // 
      this.btnCapture.Enabled = false;
      this.btnCapture.Location = new System.Drawing.Point(12, 218);
      this.btnCapture.Name = "btnCapture";
      this.btnCapture.Size = new System.Drawing.Size(104, 26);
      this.btnCapture.TabIndex = 1;
      this.btnCapture.Text = "Capture";
      this.toolTip1.SetToolTip(this.btnCapture, "Capture template for currently displayed fingerprint");
      this.btnCapture.UseVisualStyleBackColor = true;
      this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
      // 
      // btnClose
      // 
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.Enabled = false;
      this.btnClose.Location = new System.Drawing.Point(120, 218);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(50, 26);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "Close";
      this.toolTip1.SetToolTip(this.btnClose, "Close dialog");
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // paQuality
      // 
      this.paQuality.BackColor = System.Drawing.Color.White;
      this.paQuality.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.paQuality.Controls.Add(this.lblQuality);
      this.paQuality.Location = new System.Drawing.Point(239, 218);
      this.paQuality.Name = "paQuality";
      this.paQuality.Size = new System.Drawing.Size(151, 24);
      this.paQuality.TabIndex = 5;
      // 
      // lblQuality
      // 
      this.lblQuality.BackColor = System.Drawing.Color.Transparent;
      this.lblQuality.ForeColor = System.Drawing.SystemColors.ControlText;
      this.lblQuality.Location = new System.Drawing.Point(-2, 0);
      this.lblQuality.Name = "lblQuality";
      this.lblQuality.Size = new System.Drawing.Size(149, 20);
      this.lblQuality.TabIndex = 0;
      this.lblQuality.Text = "Quality";
      this.lblQuality.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.toolTip1.SetToolTip(this.lblQuality, "Quality of captured fingerprint template");
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 254);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(48, 13);
      this.label1.TabIndex = 7;
      this.label1.Text = "Options:";
      // 
      // chkCaptureClose
      // 
      this.chkCaptureClose.AutoSize = true;
      this.chkCaptureClose.Location = new System.Drawing.Point(66, 253);
      this.chkCaptureClose.Name = "chkCaptureClose";
      this.chkCaptureClose.Size = new System.Drawing.Size(109, 17);
      this.chkCaptureClose.TabIndex = 8;
      this.chkCaptureClose.Text = "Close on Capture";
      this.toolTip1.SetToolTip(this.chkCaptureClose, "Close dialog as soon as a template has been captured successfully");
      this.chkCaptureClose.UseVisualStyleBackColor = true;
      this.chkCaptureClose.CheckedChanged += new System.EventHandler(this.chkCaptureClose_CheckedChanged);
      // 
      // chkCaptureMultiple
      // 
      this.chkCaptureMultiple.AutoSize = true;
      this.chkCaptureMultiple.Location = new System.Drawing.Point(181, 253);
      this.chkCaptureMultiple.Name = "chkCaptureMultiple";
      this.chkCaptureMultiple.Size = new System.Drawing.Size(142, 17);
      this.chkCaptureMultiple.TabIndex = 9;
      this.chkCaptureMultiple.Text = "Capture Multiple Fingers";
      this.toolTip1.SetToolTip(this.chkCaptureMultiple, "Capture templates sequentially for multiple fingers");
      this.chkCaptureMultiple.UseVisualStyleBackColor = true;
      this.chkCaptureMultiple.CheckedChanged += new System.EventHandler(this.chkCaptureMultiple_CheckedChanged);
      // 
      // picFinger
      // 
      this.picFinger.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.picFinger.Location = new System.Drawing.Point(239, 29);
      this.picFinger.Name = "picFinger";
      this.picFinger.Size = new System.Drawing.Size(151, 183);
      this.picFinger.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.picFinger.TabIndex = 4;
      this.picFinger.TabStop = false;
      this.toolTip1.SetToolTip(this.picFinger, "Fingerprint image");
      // 
      // hands1
      // 
      this.hands1.BackColor = System.Drawing.Color.White;
      this.hands1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.hands1.IsReadOnly = false;
      this.hands1.Location = new System.Drawing.Point(12, 29);
      this.hands1.Name = "hands1";
      this.hands1.Size = new System.Drawing.Size(212, 183);
      this.hands1.TabIndex = 0;
      this.toolTip1.SetToolTip(this.hands1, "Click on finger to capture");
      this.hands1.FingerFocused += new System.EventHandler<FPLibrary.FingerFocusedEventArgs>(this.hands1_FingerFocused);
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(174, 218);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(50, 26);
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "Cancel";
      this.toolTip1.SetToolTip(this.btnCancel, "Discard captured fingerprints and close dialog");
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // lblPrompt
      // 
      this.lblPrompt.BackColor = System.Drawing.SystemColors.Control;
      this.lblPrompt.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.lblPrompt.Location = new System.Drawing.Point(12, 1);
      this.lblPrompt.Multiline = true;
      this.lblPrompt.Name = "lblPrompt";
      this.lblPrompt.ReadOnly = true;
      this.lblPrompt.Size = new System.Drawing.Size(212, 28);
      this.lblPrompt.TabIndex = 11;
      this.lblPrompt.TabStop = false;
      // 
      // lblFinger
      // 
      this.lblFinger.BackColor = System.Drawing.SystemColors.Control;
      this.lblFinger.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.lblFinger.Location = new System.Drawing.Point(239, 1);
      this.lblFinger.Multiline = true;
      this.lblFinger.Name = "lblFinger";
      this.lblFinger.ReadOnly = true;
      this.lblFinger.Size = new System.Drawing.Size(151, 28);
      this.lblFinger.TabIndex = 12;
      this.lblFinger.TabStop = false;
      // 
      // dlgCaptureFingerPrint
      // 
      this.AcceptButton = this.btnCapture;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnClose;
      this.ClientSize = new System.Drawing.Size(402, 251);
      this.ControlBox = false;
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.lblFinger);
      this.Controls.Add(this.lblPrompt);
      this.Controls.Add(this.chkCaptureMultiple);
      this.Controls.Add(this.chkCaptureClose);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.paQuality);
      this.Controls.Add(this.picFinger);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.btnCapture);
      this.Controls.Add(this.hands1);
      this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "dlgCaptureFingerPrint";
      this.ShowIcon = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Capture Fingerprint";
      this.Load += new System.EventHandler(this.dlgCaptureFingerPrint_Load);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.dlgCaptureFingerPrint_FormClosing);
      this.paQuality.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.picFinger)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Hands hands1;
    private System.Windows.Forms.Button btnCapture;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.PictureBox picFinger;
    private System.Windows.Forms.Panel paQuality;
    private System.Windows.Forms.Label lblQuality;
    private Label label1;
    private CheckBox chkCaptureClose;
    private CheckBox chkCaptureMultiple;
    private ToolTip toolTip1;
    private TextBox lblPrompt;
    private TextBox lblFinger;
    private Button btnCancel;
  }
}