using System;
using System.Collections.Generic;
using GriauleFingerprintLibrary;
using GriauleFingerprintLibrary.Exceptions;
using GriauleFingerprintLibrary.DataTypes;
using System.Windows.Forms;
using GriauleFingerprintLibrary.Events;
using FPLibrary.DataAccessLayer;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FPLibrary
{
  public class FingerPrints : IDisposable
  {
    public FingerPrints()
    {
      FingerprintSensors = new List<string>();
      DefaultTemplateFormat = TemplateFormat.ISO19794_2;
    }
    #region Fields
    private int _IdentificationRotationTolerance = 180;
    private int _IdentificationThreshold = (int)MatchThreshold.Low_FRR;
    private int _VerificationRotationTolerance = 180;
    private int _VerificationThreshold = (int)MatchThreshold.VeryLow_FRR;
    private FingerprintCore griauleLibrary;  //Griaule fingerprint library
    private bool closed = true;
    #endregion
    #region Properties
    /// <summary>
    /// List of detected fingerprint sensors
    /// </summary>
    public List<String> FingerprintSensors { get; set; }
    /// <summary>
    /// The format in which a fingerprint template will be saved
    /// </summary>
    public TemplateFormat DefaultTemplateFormat { get; set; }
    /// <summary>
    /// Data access layer used to interact with saved templates
    /// </summary>
    public IFPDal DataAccessLayer { get; set; }
    /// <summary>
    /// Fingerprint dialog closes automatically once the selected fingerprint has been successfully captured
    /// </summary>
    public bool CloseOnCapture { get; set; }
    /// <summary>
    /// Fingerprints for multiple fingers can be captured in fingerprint dialog
    /// </summary>
    public bool CaptureMultiple { get; set; }
    /// <summary>
    /// End user can change fingerprint dialog options (CloseOnCapture, CaptureMultiple)
    /// </summary>
    public bool AllowOptionChange { get; set; }
    public int VerificationThreshold
    {
      get
      {
        if (!closed)
          griauleLibrary.GetVerifyParameters(out _VerificationThreshold, out _VerificationRotationTolerance);
        return _VerificationThreshold;
      }
      set
      {
        _VerificationThreshold = Math.Min(200, Math.Max(10, value));
        if (!closed)
          griauleLibrary.SetVerifyParameters(_VerificationThreshold, _VerificationRotationTolerance);
      }
    }
    public int VerificationRotationTolerance
    {
      get
      {
        if (!closed)
          griauleLibrary.GetVerifyParameters(out _VerificationThreshold, out _VerificationRotationTolerance);
        return _VerificationRotationTolerance;
      }
      set
      {
        _VerificationRotationTolerance = Math.Min(180, Math.Max(0, value));
        if (!closed)
          griauleLibrary.SetVerifyParameters(_VerificationThreshold, _VerificationRotationTolerance);
      }
    }
    public int IdentificationThreshold
    {
      get
      {
        if (!closed)
          griauleLibrary.GetIdentifyParameters(out _IdentificationThreshold, out _IdentificationRotationTolerance);
        return _IdentificationThreshold;

      }
      set
      {
        _IdentificationThreshold = Math.Min(200, Math.Max(10, value));
        if (!closed)
          griauleLibrary.SetIdentifyParameters(_IdentificationThreshold, _IdentificationRotationTolerance);
      }
    }
    public int IdentificationRotationTolerance
    {
      get
      {
        if (!closed)
          griauleLibrary.GetIdentifyParameters(out _IdentificationThreshold, out _IdentificationRotationTolerance);
        return _IdentificationRotationTolerance;
      }
      set
      {
        _IdentificationRotationTolerance = Math.Min(180, Math.Max(0, value));
        if (!closed)
          griauleLibrary.SetIdentifyParameters(_IdentificationThreshold, _IdentificationRotationTolerance);
      }
    }
    #endregion
    #region Public Methods
    /// <summary>
    /// Initialize the finger print library as wel as the underlying Griaule core library
    /// </summary>
    /// <param name="dal">Data access layer interface to interact with underlying data store, User responsible to manage opening and disposing object</param>
    public void Initialize(IFPDal dal)
    {
      DataAccessLayer = dal;
      if (!closed)
      {
        throw new FPLibraryException(FPLibraryException.LibraryInitialisationError, "Library has already been initialized.");
      }
      griauleLibrary = new FingerprintCore();
      griauleLibrary.onStatus += new StatusEventHandler(griauleLibrary_onStatus);
      //griauleLibrary.onFinger - not yet linked - only once we want to be notified
      //griauleLibrary.onImage - not yet linked - only once we want to be notified
      try
      {
        griauleLibrary.Initialize();
        griauleLibrary.CaptureInitialize();
        griauleLibrary.SetIdentifyParameters(IdentificationThreshold, IdentificationRotationTolerance);
        griauleLibrary.SetVerifyParameters(VerificationThreshold, VerificationRotationTolerance);
        closed = false;
      }
      catch (FingerprintException ex)
      {
        MessageBox.Show(String.Format("Initialize Error : {0} {1}", ex.ErrorCode, ex.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      catch (Exception e)
      {
        MessageBox.Show(String.Format("Initialize Error : {0}", e.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    /// <summary>
    /// Start capturing finger prints on the indicated fingersprint sensor
    /// </summary>
    /// <param name="sensor">The sensor to capture the finger print from</param>
    public bool CaptureFingerprint(string sensor)
    {
      try
      {
        Dictionary<Fingers, FingerprintTemplate> capturedFingers = Capture(sensor, true);
        foreach (var kvp in capturedFingers)
        {
          DataAccessLayer.SaveTemplate(kvp.Key, kvp.Value);
        }
        if (capturedFingers.Count > 0)
          return true;
        else
          return false;
      }
      catch (FingerprintException ex)
      {
        MessageBox.Show(String.Format("CaptureFingerprint Error : {0} {1}", ex.ErrorCode, ex.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }
      catch (Exception e)
      {
        MessageBox.Show(String.Format("CaptureFingerprint Error : {0}", e.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }
    }
    /// <summary>
    /// Capture fingerprint and verify against stored template(s) belonging to the active individual
    /// If CaptureMultiple is true, verification will be done across multiple fingers
    /// </summary>
    /// <param name="sensor">The name of the sensor to capture the fingerprint template on</param>
    /// <returns>If any finger matched Verify returns true</returns>
    public bool Verify(string sensor)
    {
      return Verify(sensor, true);
    }
    /// <summary>
    /// Capture fingerprint and verify against stored template(s) belonging to the active individual
    /// If CaptureMultiple is true, verification will be done for multiple fingers
    /// </summary>
    /// <param name="sensor">The sensor to capture the finger print from</param>
    /// <param name="anyMatch">Any one finger needs to match for verification</param>
    /// <returns>Returns true if finger prints verified</returns>
    public bool Verify(string sensor, bool anyMatch)
    {
      try
      {
        griauleLibrary.SetVerifyParameters(VerificationThreshold, VerificationRotationTolerance);
        Dictionary<Fingers, FingerprintTemplate> capturedFingers = Capture(sensor, true);
        Dictionary<Fingers, FingerprintTemplate> storedFingers = DataAccessLayer.GetFingerTemplates();
        bool result = !anyMatch;
        int verifyScore;
        FingerprintTemplate referenceTemplate;
        foreach (var kvp in capturedFingers)
        {
          if (storedFingers.TryGetValue(kvp.Key, out referenceTemplate))
          {
            if (anyMatch)
            {
              result = result | (griauleLibrary.Verify(kvp.Value, referenceTemplate, out verifyScore) == FingerprintConstants.GR_MATCH);
              if (result)
              {
                return true;
              };
            }
            else
            {
              result = result && (griauleLibrary.Verify(kvp.Value, referenceTemplate, out verifyScore) == FingerprintConstants.GR_MATCH);
            }
          }
        }
        return result;
      }
      catch (FingerprintException ex)
      {
        MessageBox.Show(String.Format("Verification Error : {0} {1}", ex.ErrorCode, ex.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }
      catch (Exception e)
      {
        MessageBox.Show(String.Format("Verification Error : {0}", e.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }
    }
    /// <summary>
    /// Search through all templates for a fingerprint match
    /// If CaptureMultiple is true, match will be done on all captured templates
    /// Any one finger will be sufficient to match
    /// </summary>
    /// <param name="sensor">The sensor to capture the finger print from</param>
    /// <returns></returns>
    public object Identify(string sensor)
    {
      return Identify(sensor, true);
    }
    /// <summary>
    /// Search through all templates for a fingerprint match
    /// If CaptureMultiple is true, match will be done on all captured templates
    /// </summary>
    /// <param name="sensor">The sensor to capture the finger print from</param>
    /// <param name="anyMatch">Any one finger needs to match for identification</param>
    /// <returns>The key (IndividualID) of the matched individual</returns>
    public object Identify(string sensor, bool anyMatch)
    {
      try
      {
        Dictionary<Fingers, FingerprintTemplate> capturedFingers = Capture(sensor, false);
        if (capturedFingers == null || capturedFingers.Count == 0) return null;
        else return Identify(anyMatch, capturedFingers);
      }
      catch (FingerprintException ex)
      {
        MessageBox.Show(String.Format("Identification Error : {0} {1}", ex.ErrorCode, ex.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return null;
      }
      catch (Exception e)
      {
        MessageBox.Show(String.Format("Identification Error : {0}", e.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return null;
      }
    }
    /// <summary>
    /// Search though all available fingerprint templates for a match
    /// </summary>
    /// <param name="anyMatch">Any one finger needs to match for identification</param>
    /// <param name="capturedFingers">The fingerprints to search for</param>
    /// <returns></returns>
    public object Identify(bool anyMatch, Dictionary<Fingers, FingerprintTemplate> capturedFingers)
    {
      try
      {
        object result = null;
        griauleLibrary.SetIdentifyParameters(IdentificationThreshold, IdentificationRotationTolerance);
        //Set up and prepare an identification context for each captured finger
        Dictionary<Fingers, int> fingerContexts = new Dictionary<Fingers, int>(capturedFingers.Count);
        Fingers matchFingers = 0; //Fingers to match
        try
        {
          foreach (var kvp in capturedFingers)
          {
            int context;
            griauleLibrary.CreateContext(out context);
            fingerContexts.Add(kvp.Key, context);
            griauleLibrary.IdentifyPrepare(capturedFingers[kvp.Key], context);
            matchFingers |= kvp.Key; //add finger to fingers to be matched
          }
          result = DataAccessLayer.Identify(CompareTemplate, anyMatch, fingerContexts, matchFingers);
        }
        finally
        {
          foreach (var kvp in fingerContexts)
          {
            griauleLibrary.DestroyContext(kvp.Value);
          }
        }
        return result;
      }
      catch (FingerprintException ex)
      {
        MessageBox.Show(String.Format("Identification Error : {0} {1}", ex.ErrorCode, ex.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return null;
      }
      catch (Exception e)
      {
        MessageBox.Show(String.Format("Identification Error : {0}", e.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return null;
      }
    }
    /// <summary>
    /// Fingerprints are obtained, first tested if they exist in the database, and if not then saved to the database
    /// The individual to which the captured fingerprints must be saved must be set as the ActiveIndividual in the DataAccesLayer
    /// </summary>
    /// <param name="sensor">The sensor to capture the finger print from</param>
    /// <returns>The individual ID of the individual identified, null if none was identified</returns>
    public object IdentifyAndCapture(string sensor)
    {
      try
      {
        object result = null;
        griauleLibrary.SetIdentifyParameters(IdentificationThreshold, IdentificationRotationTolerance);
        Dictionary<Fingers, FingerprintTemplate> capturedFingers = Capture(sensor, DataAccessLayer.ActiveIndividual != null);
        if (capturedFingers != null && capturedFingers.Count > 0)
        {
          //Set up and prepare an identification context for each captured finger
          Dictionary<Fingers, int> fingerContexts = new Dictionary<Fingers, int>(capturedFingers.Count);
          Fingers matchFingers = 0; //Fingers to match
          try
          {
            foreach (var kvp in capturedFingers)
            {
              int context;
              griauleLibrary.CreateContext(out context);
              fingerContexts.Add(kvp.Key, context);
              griauleLibrary.IdentifyPrepare(capturedFingers[kvp.Key], context);
              matchFingers |= kvp.Key; //add finger to fingers to be matched
            }
            CompareFingerDelegate comparer = CompareTemplate;
            result = DataAccessLayer.Identify(CompareTemplate, true, fingerContexts, matchFingers);
            if (result == null)  //No one identified
            {
              foreach (var kvp in capturedFingers)
              {
                DataAccessLayer.SaveTemplate(kvp.Key, kvp.Value);
              }
            }
          }
          finally
          {
            foreach (var kvp in fingerContexts)
            {
              griauleLibrary.DestroyContext(kvp.Value);
            }
          }
        }
        return result;
      }
      catch (FingerprintException ex)
      {
        MessageBox.Show(String.Format("Identification Error : {0} {1}", ex.ErrorCode, ex.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return null;
      }
      catch (Exception e)
      {
        MessageBox.Show(String.Format("Identification Error : {0}", e.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return null;
      }
    }
    /// <summary>
    /// Extract a fingerprint template from a raw fingerprint image
    /// </summary>
    /// <param name="image">The image to extract the template from</param>
    /// <returns></returns>
    public FingerprintTemplate ExtractTemplate(FingerprintRawImage image)
    {
      var template = new FingerprintTemplate();
      griauleLibrary.ExtractEx(image, ref template, (GrTemplateFormat)DefaultTemplateFormat);
      return template;
    }
    #endregion
    #region Private Methods
    private bool CompareTemplate(int context, FingerprintTemplate template)
    {
      int verifyScore;
      return griauleLibrary.Identify(template, out verifyScore, context) == FingerprintConstants.GR_MATCH;
    }
    private Fingers GetRegisteredFingers()
    {
      Fingers fingers = 0;
      Dictionary<Fingers, FingerprintTemplate> savedFingers = DataAccessLayer.GetFingerTemplates();
      foreach (var kvp in savedFingers)
      {
        fingers |= kvp.Key;
      }
      return fingers;
    }
    private Dictionary<Fingers, FingerprintTemplate> Capture(string sensor, bool showRegisteredFingers)
    {
      using (var dlg = new dlgCaptureFingerPrint(sensor, this, griauleLibrary))
      {
        //Set options
        if (CaptureMultiple)
          CloseOnCapture = false;
        dlg.CloseOnCapture = CloseOnCapture;
        dlg.CaptureMultiple = CaptureMultiple;
        if (AllowOptionChange)
          dlg.Height = 298;
        else
          dlg.Height = 276;
        //Get fingers previously captured for the active individual
        if (showRegisteredFingers) dlg.RegisteredFingers = GetRegisteredFingers();
        //Show fingerprint capture dialog
        if (dlg.ShowDialog() == DialogResult.OK)
        {
          return dlg.CapturedFingers;
        }
        else
        {
          return new Dictionary<Fingers, FingerprintTemplate>(); //Return empty list
        }
      }
    }
    #endregion
    #region Events
    public event EventHandler<SensorChangedEventArgs> SensorChanged;
    protected virtual void OnSensorChanged(SensorChangedEventArgs e)
    {
      if (SensorChanged != null)
      {
        SensorChanged(this, e);
      }
    }
    #endregion
    #region Event Handlers

    void griauleLibrary_onStatus(object source, StatusEventArgs se)
    {
      switch (se.StatusEventType)
      {
        case StatusEventType.SENSOR_PLUG:
          FingerprintSensors.Add(se.Source);
          OnSensorChanged(new SensorChangedEventArgs(se.Source, false));
          break;
        case StatusEventType.SENSOR_UNPLUG:
          FingerprintSensors.Remove(se.Source);
          OnSensorChanged(new SensorChangedEventArgs(se.Source, true));
          break;
      }
    }
    #endregion
    #region IDisposable Members
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        FingerprintSensors.Clear();
        if (!closed)
        {
          try
          {
            try
            {
              griauleLibrary.CaptureFinalize();
            }
            catch (FingerprintException ex)
            {
              MessageBox.Show(String.Format("CaptureFinalize Error : {0} {1}", ex.ErrorCode, ex.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          }
          catch (Exception e)
          {
            MessageBox.Show(String.Format("Dispose Error : {0}", e.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }
      }
      if (griauleLibrary != null)
      {
        try
        {
          griauleLibrary.Finalizer();
          griauleLibrary = null;
        }
        catch (FingerprintException ex)
        {
          MessageBox.Show(String.Format("Finalizer Error : {0} {1}", ex.ErrorCode, ex.Message), "FPLibrary Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }
    ~FingerPrints()
    {
      Dispose(false);
    }
    #endregion
  }
  #region Support Classes
  public class SensorChangedEventArgs : EventArgs
  {
    public readonly string Sensor;
    public readonly bool Removed;
    public SensorChangedEventArgs(string sensor, bool removed)
    {
      Sensor = sensor;
      Removed = removed;
    }
  }
  public delegate bool CompareFingerDelegate(int context, FingerprintTemplate template);
  #endregion
  #region Enums
  [Flags]
  public enum Fingers
  {
    RightThumb = 1, RightIndex = 2, RightMiddle = 4, RightRing = 8, RightLittle = 16,
    LeftThumb = 32, LeftIndex = 64, LeftMiddle = 128, LeftRing = 256, LeftLittle = 512, RightHeel = 1024, LeftHeel = 2048
  }
  public enum TemplateFormat
  {
    GriauleDefault = GrTemplateFormat.GR_FORMAT_DEFAULT,
    ANSI378_2004 = GrTemplateFormat.GR_FORMAT_ANSI,
    ISO19794_2 = GrTemplateFormat.GR_FORMAT_ISO
  }
  public enum MatchThreshold
  {
    Minimum = FingerprintConstants.GR_MIN_THRESHOLD,
    VeryLow_FRR = FingerprintConstants.GR_VERYLOW_FRR,  // Threshold value for a very low FRR (1:1000) FAR (1:30000)
    Low_FRR = FingerprintConstants.GR_LOW_FRR,          // Threshold value for a low FRR (1:100) FAR (1:100000)
    Low_FAR = FingerprintConstants.GR_LOW_FAR,          // Threshold value for a low FAR (1 false acceptance in 300000)
    VeryLow_FAR = FingerprintConstants.GR_VERYLOW_FAR,  // Threshold value for a very low FAR (1 false acceptance in 3000000)
    Maximum = FingerprintConstants.GR_MAX_THRESHOLD
  }
  #endregion
}
