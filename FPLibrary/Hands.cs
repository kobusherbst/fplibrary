using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace FPLibrary
{
  public partial class Hands : UserControl
  {
    #region "Fields"
    private Fingers selectedFinger = 0;
    #endregion
    #region "Properties"
    public Fingers RegisteredFingers { get; set; }
    public Fingers SelectedFinger
    {
      get { return selectedFinger; }
      set
      {
        selectedFinger = value;
        FocusFinger(selectedFinger);
      }
    }
    public bool IsReadOnly { get; set; }
    public Hands()
    {
      InitializeComponent();
      RegisteredFingers = 0;
      selectedFinger = 0;
      ClearFingers();
    }
    #endregion
    #region "Events"
    private void Hands_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (!IsReadOnly)
      {
        ClearFingers();
        switch (e.KeyChar)
        {
          case 'q':
          case 'Q':
            FocusFinger(Fingers.LeftLittle);
            break;
          case 'w':
          case 'W':
            FocusFinger(Fingers.LeftRing);
            break;
          case 'e':
          case 'E':
            FocusFinger(Fingers.LeftMiddle);
            break;
          case 'r':
          case 'R':
            FocusFinger(Fingers.LeftIndex);
            break;
          case 't':
          case 'T':
            FocusFinger(Fingers.LeftThumb);
            break;
          case 'g':
          case 'G':
            FocusFinger(Fingers.LeftHeel);
            break;
          case 'h':
          case 'H':
            FocusFinger(Fingers.RightHeel);
            break;
          case 'y':
          case 'Y':
            FocusFinger(Fingers.RightThumb);
            break;
          case 'u':
          case 'U':
            FocusFinger(Fingers.RightIndex);
            break;
          case 'i':
          case 'I':
            FocusFinger(Fingers.RightMiddle);
            break;
          case 'o':
          case 'O':
            FocusFinger(Fingers.RightRing);
            break;
          case 'p':
          case 'P':
            FocusFinger(Fingers.RightLittle);
            break;
          default:
            FocusFinger(SelectedFinger);
            break;
        }
      }
    }
    protected override void OnLoad(EventArgs e)
    {
      FocusFinger(selectedFinger);
      base.OnLoad(e);
    }
    protected override void OnPaint(PaintEventArgs e)
    {
      pbLPalm.Image = Properties.Resources.palm;
      pbRPalm.Image = Properties.Resources.palm;
      if (IsReadOnly)
      {
        pbLeftRing.Cursor = Cursors.Default;
        pbLeftIndex.Cursor = Cursors.Default;
        pbLeftLittle.Cursor = Cursors.Default;
        pbLeftMiddle.Cursor = Cursors.Default;
        pbLeftThumb.Cursor = Cursors.Default;
        pbLeftHeel.Cursor = Cursors.Default;

        pbRightRing.Cursor = Cursors.Default;
        pbRightIndex.Cursor = Cursors.Default;
        pbRightLittle.Cursor = Cursors.Default;
        pbRightMiddle.Cursor = Cursors.Default;
        pbRightThumb.Cursor = Cursors.Default;
        pbRightHeel.Cursor = Cursors.Default;
      }
      else
      {
        pbLeftRing.Cursor = Cursors.Hand;
        pbLeftIndex.Cursor = Cursors.Hand;
        pbLeftLittle.Cursor = Cursors.Hand;
        pbLeftMiddle.Cursor = Cursors.Hand;
        pbLeftThumb.Cursor = Cursors.Hand;
        pbLeftHeel.Cursor = Cursors.Hand;

        pbRightRing.Cursor = Cursors.Hand;
        pbRightIndex.Cursor = Cursors.Hand;
        pbRightLittle.Cursor = Cursors.Hand;
        pbRightMiddle.Cursor = Cursors.Hand;
        pbRightThumb.Cursor = Cursors.Hand;
        pbRightHeel.Cursor = Cursors.Hand;
      }
      DisplayFingers();
      base.OnPaint(e);
    }
    private void pbLPinkie_Click(object sender, EventArgs e)
    {
      if (!IsReadOnly)
      {
        ClearFingers();
        FocusFinger(Fingers.LeftLittle);
      }
    }
    private void pbLRing_Click(object sender, EventArgs e)
    {
      if (!IsReadOnly)
      {
        ClearFingers();
        FocusFinger(Fingers.LeftRing);
      }
    }
    private void pbLMiddle_Click(object sender, EventArgs e)
    {
      if (!IsReadOnly)
      {
        ClearFingers();
        FocusFinger(Fingers.LeftMiddle);
      }
    }
    private void pbLIndex_Click(object sender, EventArgs e)
    {
      if (!IsReadOnly)
      {
        ClearFingers();
        FocusFinger(Fingers.LeftIndex);
      }
    }
    private void pbLThumb_Click(object sender, EventArgs e)
    {
      if (!IsReadOnly)
      {
        ClearFingers();
        FocusFinger(Fingers.LeftThumb);
      }
    }
    private void pbRThumb_Click(object sender, EventArgs e)
    {
      if (!IsReadOnly)
      {
        ClearFingers();
        FocusFinger(Fingers.RightThumb);
      }
    }
    private void pbRIndex_Click(object sender, EventArgs e)
    {
      if (!IsReadOnly)
      {
        ClearFingers();
        FocusFinger(Fingers.RightIndex);
      }
    }
    private void pbRMiddle_Click(object sender, EventArgs e)
    {
      if (!IsReadOnly)
      {
        ClearFingers();
        FocusFinger(Fingers.RightMiddle);
      }
    }
    private void pbRRing_Click(object sender, EventArgs e)
    {
      if (!IsReadOnly)
      {
        ClearFingers();
        FocusFinger(Fingers.RightRing);
      }
    }
    private void pbRPinkie_Click(object sender, EventArgs e)
    {
      if (!IsReadOnly)
      {
        ClearFingers();
        FocusFinger(Fingers.RightLittle);
      }
    }
    private void pbHeelL_Click(object sender, EventArgs e)
    {
      if (!IsReadOnly)
      {
        ClearFingers();
        FocusFinger(Fingers.LeftHeel);
      }
    }
    private void pbHeelR_Click(object sender, EventArgs e)
    {
      if (!IsReadOnly)
      {
        ClearFingers();
        FocusFinger(Fingers.RightHeel);
      }
    }
    #endregion
    #region "Methods"
    /// <summary>
    /// Set a flag in RegisteredFingers that the given finger is registered
    /// </summary>
    /// <param name="finger">Finger to register, if <c>Fingers.Unknown</c> then all fingers are unregistered</param>
    public void RegisterFinger(Fingers finger)
    {
      RegisteredFingers |= finger;
    }
    /// <summary>
    /// Clear a flag in RegisteredFingers indicating that the given finger is not registered
    /// </summary>
    /// <param name="finger">Finger to register, if <c>Fingers.Unknown</c> then all fingers are registered</param>
    public void UnRegisterFinger(Fingers finger)
    {
      Fingers allFingers = ~(Fingers)0;
      RegisteredFingers &= (finger ^ allFingers);
    }
    private void ClearFingers()
    {
      pbLeftLittle.Image = ((RegisteredFingers & Fingers.LeftLittle) != 0) ? Properties.Resources.sel_finger : Properties.Resources.unsel_finger;
      pbLeftRing.Image = ((RegisteredFingers & Fingers.LeftRing) != 0) ? Properties.Resources.sel_finger : Properties.Resources.unsel_finger;
      pbLeftMiddle.Image = ((RegisteredFingers & Fingers.LeftMiddle) != 0) ? Properties.Resources.sel_finger : Properties.Resources.unsel_finger;
      pbLeftIndex.Image = ((RegisteredFingers & Fingers.LeftIndex) != 0) ? Properties.Resources.sel_finger : Properties.Resources.unsel_finger;
      pbLeftThumb.Image = ((RegisteredFingers & Fingers.LeftThumb) != 0) ? Properties.Resources.sel_finger : Properties.Resources.unsel_finger;
      pbRightThumb.Image = ((RegisteredFingers & Fingers.RightThumb) != 0) ? Properties.Resources.sel_finger : Properties.Resources.unsel_finger;
      pbRightIndex.Image = ((RegisteredFingers & Fingers.RightIndex) != 0) ? Properties.Resources.sel_finger : Properties.Resources.unsel_finger;
      pbRightMiddle.Image = ((RegisteredFingers & Fingers.RightMiddle) != 0) ? Properties.Resources.sel_finger : Properties.Resources.unsel_finger;
      pbRightRing.Image = ((RegisteredFingers & Fingers.RightRing) != 0) ? Properties.Resources.sel_finger : Properties.Resources.unsel_finger;
      pbRightLittle.Image = ((RegisteredFingers & Fingers.RightLittle) != 0) ? Properties.Resources.sel_finger : Properties.Resources.unsel_finger;
      pbLeftHeel.Image = ((RegisteredFingers & Fingers.LeftHeel) != 0) ? Properties.Resources.sel_foot_l : Properties.Resources.unsel_foot_l;
      pbRightHeel.Image = ((RegisteredFingers & Fingers.RightHeel) != 0) ? Properties.Resources.sel_foot_r : Properties.Resources.unsel_foot_r;
    }
    private void DisplayFingers()
    {
      ClearFingers();
      switch (SelectedFinger)
      {
        case Fingers.RightThumb:
          pbRightThumb.Image = ((RegisteredFingers & Fingers.RightThumb) != 0) ? Properties.Resources.focus_sel_finger : Properties.Resources.focus_finger;
          break;
        case Fingers.RightIndex:
          pbRightIndex.Image = ((RegisteredFingers & Fingers.RightIndex) != 0) ? Properties.Resources.focus_sel_finger : Properties.Resources.focus_finger;
          break;
        case Fingers.RightMiddle:
          pbRightMiddle.Image = ((RegisteredFingers & Fingers.RightMiddle) != 0) ? Properties.Resources.focus_sel_finger : Properties.Resources.focus_finger;
          break;
        case Fingers.RightRing:
          pbRightRing.Image = ((RegisteredFingers & Fingers.RightRing) != 0) ? Properties.Resources.focus_sel_finger : Properties.Resources.focus_finger;
          break;
        case Fingers.RightLittle:
          pbRightLittle.Image = ((RegisteredFingers & Fingers.RightLittle) != 0) ? Properties.Resources.focus_sel_finger : Properties.Resources.focus_finger;
          break;
        case Fingers.LeftThumb:
          pbLeftThumb.Image = ((RegisteredFingers & Fingers.LeftThumb) != 0) ? Properties.Resources.focus_sel_finger : Properties.Resources.focus_finger;
          break;
        case Fingers.LeftIndex:
          pbLeftIndex.Image = ((RegisteredFingers & Fingers.LeftIndex) != 0) ? Properties.Resources.focus_sel_finger : Properties.Resources.focus_finger;
          break;
        case Fingers.LeftMiddle:
          pbLeftMiddle.Image = ((RegisteredFingers & Fingers.LeftMiddle) != 0) ? Properties.Resources.focus_sel_finger : Properties.Resources.focus_finger;
          break;
        case Fingers.LeftRing:
          pbLeftRing.Image = ((RegisteredFingers & Fingers.LeftRing) != 0) ? Properties.Resources.focus_sel_finger : Properties.Resources.focus_finger;
          break;
        case Fingers.LeftLittle:
          pbLeftLittle.Image = ((RegisteredFingers & Fingers.LeftLittle) != 0) ? Properties.Resources.focus_sel_finger : Properties.Resources.focus_finger;
          break;
        case Fingers.RightHeel:
          pbRightHeel.Image = ((RegisteredFingers & Fingers.RightHeel) != 0) ? Properties.Resources.focus_sel_foot_r : Properties.Resources.focus_foot_r;
          break;
        case Fingers.LeftHeel:
          pbLeftHeel.Image = ((RegisteredFingers & Fingers.LeftHeel) != 0) ? Properties.Resources.focus_sel_foot_l : Properties.Resources.focus_foot_l;
          break;
      }
    }
    private void FocusFinger(Fingers finger)
    {
      if (SelectedFinger != finger)
      {
        selectedFinger = finger;
        DisplayFingers();
        OnFingerFocused(new FingerFocusedEventArgs(finger));
      }
    }
    public override void Refresh()
    {
      FocusFinger(selectedFinger);
      base.Refresh();
    }
    #endregion
    #region Delegates & Events
    [BrowsableAttribute(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public event EventHandler<FingerFocusedEventArgs> FingerFocused;
    protected virtual void OnFingerFocused(FingerFocusedEventArgs e)
    {
      if (FingerFocused != null)
      {
        FingerFocused(this, e);
      }
    }
    #endregion
  }
  public class FingerFocusedEventArgs : System.EventArgs
  {
    public readonly Fingers FocusedFinger;
    public FingerFocusedEventArgs(Fingers finger)
    {
      FocusedFinger = finger;
    }
  }
}
