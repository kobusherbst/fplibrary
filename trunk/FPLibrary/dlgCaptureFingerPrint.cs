using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using GriauleFingerprintLibrary;
using GriauleFingerprintLibrary.Exceptions;
using GriauleFingerprintLibrary.DataTypes;
using System.Threading;

namespace FPLibrary
{
  public partial class dlgCaptureFingerPrint : Form
  {
    public dlgCaptureFingerPrint()
    {
      InitializeComponent();
      CapturedFingers = new Dictionary<Fingers, FingerprintTemplate>(4); //4=typical maximum amount of fingerprints captured at one time
      ImageFinger = 0;
    }
    public dlgCaptureFingerPrint(string sensor, FingerPrints controller, FingerprintCore fpCore)
      : this()
    {
      Sensor = sensor;
      Controller = controller;
      FPCore = fpCore;
    }
    #region Fields
    bool capturing = false;
    private FingerprintTemplate template;
    private enum EnrollQuality { Unspecified, Poor, Sufficient, Good, VeryGood }
    #endregion
    #region Properties
    public string Sensor { get; set; }
    public FingerPrints Controller { get; set; }
    public FingerprintCore FPCore { get; set; }
    public Fingers ImageFinger { get; set; }
    public bool CloseOnCapture
    {
      get { return chkCaptureClose.Checked; }
      set
      {
        chkCaptureClose.Checked = value;
        SetButtons(value);
      }
    }
    public bool CaptureMultiple
    {
      get { return chkCaptureMultiple.Checked; }
      set
      {
        chkCaptureMultiple.Checked = value;
      }
    }
    public Dictionary<Fingers, FingerprintTemplate> CapturedFingers { get; set; }
    public Fingers RegisteredFingers
    {
      get { return hands1.RegisteredFingers; }
      set { hands1.RegisteredFingers = value; }
    }
    #endregion
    #region Events
    private void dlgCaptureFingerPrint_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing && this.DialogResult != DialogResult.OK && CapturedFingers.Count > 0)
      {
        if (MessageBox.Show("Are you sure you wish to close the windows and discard the captured fingerprints?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.OK)
        {
          e.Cancel = true;
        };
      }
    }
    private void btnCapture_Click(object sender, EventArgs e)
    {
      if (hands1.SelectedFinger == ImageFinger)
      {
        if (!CapturedFingers.ContainsKey(ImageFinger))
        {
          if (!CaptureMultiple)
          {
            CapturedFingers.Clear();
            hands1.RegisteredFingers = 0;
          }
          CapturedFingers.Add(ImageFinger, template);
          hands1.RegisteredFingers |= ImageFinger;
        }
        else
        {
          CapturedFingers[ImageFinger] = template;
        }
        SetFingerLabel(String.Format("Template captured, quality={0}", template.Quality), true);
        btnClose.Enabled = true;
        if (CloseOnCapture)
        {
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
      }
      else
      {
        MessageBox.Show(String.Format("Fingerprint image ({0}) does not correspond to selected finger ({1}).",
                                      ImageFinger.ToString(), hands1.SelectedFinger.ToString()));
      }
    }
    private void hands1_FingerFocused(object sender, FingerFocusedEventArgs e)
    {
      btnCapture.Enabled = false;
      if (!capturing)
      {
        try
        {
          FPCore.StartCapture(Sensor);
          capturing = true;
        }
        catch (FingerprintException ex)
        {
          MessageBox.Show(String.Format("StartCapture Error : {0} {1}", ex.ErrorCode, ex.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
      switch (e.FocusedFinger)
      {
        case Fingers.RightThumb:
          lblPrompt.Text = "Place right thumb on fingerprint sensor";
          picFinger.Image = Properties.Resources.RThumb;
          break;
        case Fingers.RightIndex:
          lblPrompt.Text = "Place right index finger on fingerprint sensor";
          picFinger.Image = Properties.Resources.RIndex;
          break;
        case Fingers.RightMiddle:
          lblPrompt.Text = "Place right middle finger on fingerprint sensor";
          picFinger.Image = Properties.Resources.RMiddle;
          break;
        case Fingers.RightRing:
          lblPrompt.Text = "Place right ring finger on fingerprint sensor";
          picFinger.Image = Properties.Resources.RRing;
          break;
        case Fingers.RightLittle:
          lblPrompt.Text = "Place right little finger on fingerprint sensor";
          picFinger.Image = Properties.Resources.RLittle;
          break;
        case Fingers.LeftThumb:
          lblPrompt.Text = "Place left thumb on fingerprint sensor";
          picFinger.Image = Properties.Resources.LThumb;
          break;
        case Fingers.LeftIndex:
          lblPrompt.Text = "Place left index finger on fingerprint sensor";
          picFinger.Image = Properties.Resources.LIndex;
          break;
        case Fingers.LeftMiddle:
          lblPrompt.Text = "Place left middle finger on fingerprint sensor";
          picFinger.Image = Properties.Resources.LMiddle;
          break;
        case Fingers.LeftRing:
          lblPrompt.Text = "Place left ring finger on fingerprint sensor";
          picFinger.Image = Properties.Resources.LRing;
          break;
        case Fingers.LeftLittle:
          lblPrompt.Text = "Place left little finger on fingerprint sensor";
          picFinger.Image = Properties.Resources.LLittle;
          break;
        case Fingers.RightHeel:
          lblPrompt.Text = "Place right heel on fingerprint sensor";
          picFinger.Image = Properties.Resources.RHeel;
          break;
        case Fingers.LeftHeel:
          lblPrompt.Text = "Place left heel on fingerprint sensor";
          picFinger.Image = Properties.Resources.LHeel;
          break;
        default:
          lblPrompt.Text = "";
          break;
      }
      SetFingerLabel("", false);
      ShowQuality(EnrollQuality.Unspecified);
      try
      {
        FPCore.StartEnroll();
      }
      catch (FingerprintException ex)
      {
        MessageBox.Show(String.Format("StartEnroll Error : {0} {1}", ex.ErrorCode, ex.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    private void chkCaptureClose_CheckedChanged(object sender, EventArgs e)
    {
      if (chkCaptureClose.Checked & chkCaptureMultiple.Checked)
      {
        MessageBox.Show("Close on capture cannot be selected if capture multiple fingers has been specified.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        chkCaptureClose.Checked = false;
      }
      else
      {
        SetButtons(chkCaptureClose.Checked);
      }
    }
    private void chkCaptureMultiple_CheckedChanged(object sender, EventArgs e)
    {
      if (chkCaptureMultiple.Checked)
      {
        chkCaptureClose.Checked = false;
      }
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      if (capturing)
      {
        try
        {
          FPCore.StopCapture(Sensor);
          capturing = false;
        }
        catch (FingerprintException ex)
        {
          MessageBox.Show(String.Format("StopCapture Error : {0} {1}", ex.ErrorCode, ex.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
      if (CloseOnCapture)
        this.DialogResult = DialogResult.Cancel;
      else if (CaptureMultiple && CapturedFingers.Count > 0)
        this.DialogResult = DialogResult.OK;
      else
        this.DialogResult = DialogResult.Cancel;
      this.Close();
    }
    private void dlgCaptureFingerPrint_Load(object sender, EventArgs e)
    {
      FPCore.onFinger += new FingerEventHandler(FPCore_onFinger);
      FPCore.onImage += new ImageEventHandler(FPCore_onImage);
      btnClose.Enabled = CloseOnCapture;
    }
    void FPCore_onImage(object source, GriauleFingerprintLibrary.Events.ImageEventArgs ie)
    {
      try
      {
        if (ie.RawImage != null)
        {
          SetImage(ie.RawImage.Image);
          template = new FingerprintTemplate();
          int ret = (int)FPCore.Enroll(ie.RawImage, ref template, (GrTemplateFormat)Controller.DefaultTemplateFormat, FingerprintConstants.GR_DEFAULT_CONTEXT);
          if (ret >= FingerprintConstants.GR_ENROLL_SUFFICIENT)
          {
            DisplayImage(template, ie.RawImage);
            EnableCaptureButton(true);
            if (ret == FingerprintConstants.GR_ENROLL_SUFFICIENT)
            {
              ShowQuality(EnrollQuality.Sufficient);
              SetFingerLabel("Place finger again to improve quality", true);
            }
            else if (ret == FingerprintConstants.GR_ENROLL_GOOD)
            {
              ShowQuality(EnrollQuality.Good);
              SetFingerLabel("Click 'Capture' to record template", true);
            }
            else if (ret == FingerprintConstants.GR_ENROLL_VERY_GOOD)
            {
              ShowQuality(EnrollQuality.VeryGood);
              SetFingerLabel("Click 'Capture' to record template", true);
            }
            else if (ret == FingerprintConstants.GR_ENROLL_MAX_LIMIT_REACHED)
            {
              SetFingerLabel("Enrollment limit reached", true);
            }
          }
          else
          {
            SetFingerLabel("Place finger again", true);
            ShowQuality(EnrollQuality.Poor);
          }
          Thread.Sleep(100);
        }
      }
      catch (FingerprintException ex)
      {
        ShowFPError(ex);
      }
      catch (Exception e)
      {
        ShowError(e);
      }

    }
    void FPCore_onFinger(object source, GriauleFingerprintLibrary.Events.FingerEventArgs fe)
    {
      switch (fe.EventType)
      {
        case GriauleFingerprintLibrary.Events.FingerEventType.FINGER_DOWN:
          SetFingerLabel("Hold finger until image appears", true);
          break;
        case GriauleFingerprintLibrary.Events.FingerEventType.FINGER_UP:
          SetFingerLabel("", false);
          break;
        default:
          break;
      }
    }
    #endregion
    #region Methods
    private void SetButtons(bool value)
    {
      if (value) //Close on capture
      {
        btnCancel.Visible = false;
        btnClose.Width = 104;
        btnClose.Text = "Close";
        btnClose.Enabled = true;
      }
      else
      {
        btnClose.Width = 50;
        btnCancel.Visible = true;
        btnClose.Text = "Done";
        btnClose.Enabled = false;
      }
    }
    private delegate void DelegateShowQuality(EnrollQuality quality);
    private void ShowQuality(EnrollQuality quality)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new DelegateShowQuality(ShowQuality), new object[] { quality });
      }
      else
      {
        switch (quality)
        {
          case EnrollQuality.Unspecified:
            paQuality.BackColor = Color.White;
            lblQuality.Text = "Quality";
            break;
          case EnrollQuality.Poor:
            paQuality.BackColor = Color.LightCoral;
            lblQuality.Text = "Poor";
            break;
          case EnrollQuality.Sufficient:
            paQuality.BackColor = Color.YellowGreen;
            lblQuality.Text = "Acceptable";
            break;
          case EnrollQuality.Good:
            paQuality.BackColor = Color.MediumSeaGreen;
            lblQuality.Text = "Good";
            break;
          case EnrollQuality.VeryGood:
            paQuality.BackColor = Color.LimeGreen;
            lblQuality.Text = "Very Good";
            break;
          default:
            break;
        }
      }
    }
    private delegate void DelegateSetImage(Image img);
    private void SetImage(Image img)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new DelegateSetImage(SetImage), new object[] { img });
      }
      else
      {
        picFinger.Image = img;
        ImageFinger = hands1.SelectedFinger;
      }
    }
    private delegate void DelegateSetFingerLablel(string text, bool visible);
    private void SetFingerLabel(string text, bool visible)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new DelegateSetFingerLablel(SetFingerLabel), new object[] { text, visible });
      }
      else
      {
        lblFinger.Visible = visible;
        lblFinger.Text = text;
      }
    }
    private delegate void DelegateShowFPError(FingerprintException ex);
    private void ShowFPError(FingerprintException ex)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new DelegateShowFPError(ShowFPError), new object[] { ex });
      }
      else
      {
        MessageBox.Show(String.Format("OnImage Error : {0} {1}", ex.ErrorCode, ex.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    private delegate void DelegateShowError(Exception e);
    private void ShowError(Exception e)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new DelegateShowError(ShowError), new object[] { e });
      }
      else
      {
        MessageBox.Show(String.Format("OnImage Error : {0} ", e.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    private delegate void DelegateSetBMapImage(Image img);
    private void SetBMapImage(Image img)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new DelegateSetBMapImage(SetBMapImage), new object[] { img });
      }
      else
      {
        Bitmap bmp = new Bitmap(img, picFinger.Width, picFinger.Height);
        picFinger.Image = bmp;
      }
    }
    private void DisplayImage(FingerprintTemplate template, FingerprintRawImage rawImage)
    {
      IntPtr hdc = FingerprintCore.GetDC();

      IntPtr image = new IntPtr();

      FPCore.GetBiometricDisplay(template, rawImage, hdc, ref image, FingerprintConstants.GR_NO_CONTEXT);

      SetBMapImage(Bitmap.FromHbitmap(image));

      FingerprintCore.ReleaseDC(hdc);
    }
    private delegate void DelegateEnableCaptureButton(bool enabled);
    private void EnableCaptureButton(bool enabled)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new DelegateEnableCaptureButton(EnableCaptureButton), new object[] { enabled });
      }
      else
      {
        btnCapture.Enabled = enabled;
      }
    }
    #endregion
  }
}
