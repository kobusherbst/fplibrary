using System;
using System.Collections.Generic;
using GriauleFingerprintLibrary.DataTypes;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;

namespace FPLibrary.DataAccessLayer
{
  public class FPDal : IFPDal, IDisposable
  {
    #region Fields
    private Dictionary<Fingers, byte> _FingerValueTable = new Dictionary<Fingers, byte> 
    { {Fingers.RightThumb,1},
      {Fingers.RightIndex,2},
      {Fingers.RightMiddle,3},
      {Fingers.RightRing,4},
      {Fingers.RightLittle,5},
      {Fingers.LeftThumb,6},
      {Fingers.LeftIndex,7},
      {Fingers.LeftMiddle,8},
      {Fingers.LeftRing,9},
      {Fingers.LeftLittle,10},
      {Fingers.RightHeel,11},
      {Fingers.LeftHeel,12}
    };
    private string _QualityFieldName = FPLibrary.Properties.Resources.QualityFieldNameValue;
    private string _TemplateFieldName = FPLibrary.Properties.Resources.TemplateFieldNameValue;
    private string _FingerFieldName = FPLibrary.Properties.Resources.FingerFieldNameValue;
    private string _IndividualFieldName = FPLibrary.Properties.Resources.IndividualFieldNameValue;
    private string _TemplateTable = FPLibrary.Properties.Resources.TemplateTableValue;
    #endregion
    #region Properties
    /// <summary>
    /// Table containing fingerprint templates, it should have the form:
    /// - IndividualID int or GUID (uniqueidentifier)
    /// - FingerID smallint
    /// - FPTemplate varbinary or image 8000 bytes max
    /// - FPQuality smallint
    /// Default name : "IndividualFinger"
    /// </summary>
    public string TemplateTable
    {
      get { return _TemplateTable; }
      set
      {
        _TemplateTable = value;
      }
    }
    /// <summary>
    /// Field name of the field that uniquely identifes an individual
    /// Default name : "IndividualID"
    /// </summary>
    public string IndividualFieldName
    {
      get { return _IndividualFieldName; }
      set
      {
        _IndividualFieldName = value;
      }
    }
    /// <summary>
    /// Field name of the field that identifies a particular finger
    /// Default name : "FingerID"
    /// </summary>
    public string FingerFieldName
    {
      get { return _FingerFieldName; }
      set
      {
        _FingerFieldName = value;
      }
    }
    /// <summary>
    /// Field name of the field that contains the fingerprint template
    /// Default name : "FPTemplate"
    /// </summary>
    public string TemplateFieldName
    {
      get { return _TemplateFieldName; }
      set
      {
        _TemplateFieldName = value;
      }
    }
    /// <summary>
    /// Field name of the field that contains the finger print quality value
    /// Default name : "FPQuality"
    /// </summary>
    public string QualityFieldName
    {
      get { return _QualityFieldName; }
      set
      {
        _QualityFieldName = value;
      }
    }
    /// <summary>
    /// Dictionary to translate between Fingers enumeration and FingerID byte code
    /// Default translation based on ISO19794_2 with special codes 11 + 12 for right + left heel
    /// </summary>
    public Dictionary<Fingers, byte> FingerValueTable
    {
      get { return _FingerValueTable; }
      set { _FingerValueTable = value; }
    }
    /// <summary>
    /// Connection to the underlying database
    /// </summary>
    public DbConnection Connection { get; set; }
    /// <summary>
    /// The key value for the active individual
    /// Single template operations relate to this individual
    /// </summary>
    public object ActiveIndividual { get; set; }
    #endregion
    #region Public Methods
    /// <summary>
    /// Opens the connection to the underlying database
    /// </summary>
    public virtual void Open()
    {
      if (Connection != null)
      {
        try
        {
          if (Connection.State == ConnectionState.Closed)
            Connection.Open();
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
      }
      else
      {
        throw new FPLibraryException(FPLibraryException.LibraryDalError, "Connection is null");
      }
    }
    /// <summary>
    /// Closes the connection to the underlying database
    /// </summary>
    public virtual void Close()
    {
      if (Connection != null)
      {
        try
        {
          if (Connection.State != ConnectionState.Closed)
            Connection.Close();
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
      }
    }
    #endregion
    #region IFPDal Members
    public System.Drawing.Bitmap GetFingerprintImage(Fingers finger)
    {
      return InternalGetFingerprintImage(finger);
    }
    /// <summary>
    /// Save a fingerprint template for the current individual
    /// </summary>
    /// <param name="finger">The finger to which the template belongs</param>
    /// <param name="fingerPrintTemplate">The template to be saved</param>
    public void SaveTemplate(Fingers finger, FingerprintTemplate fingerPrintTemplate)
    {
      if (Connection != null && Connection.State == ConnectionState.Open)
        if (ActiveIndividual != null)
          InternalSaveTemplate(FingerValueTable[finger], fingerPrintTemplate);
        else throw new FPLibraryException(FPLibraryException.LibraryDalError, "Active individual not set");
      else
        throw new FPLibraryException(FPLibraryException.LibraryDalError, "Connection is null or closed");
    }
    /// <summary>
    /// Returns a datareader with access to all recorded templates
    /// </summary>
    /// <returns></returns>
    public IDataReader GetTemplates()
    {
      return InternalGetTemplates();
    }
    /// <summary>
    /// Returns the template for the specified finger of the current individual
    /// </summary>
    /// <param name="finger">The finger to which the template belongs</param>
    /// <returns>A fingerprint template</returns>
    public FingerprintTemplate GetTemplate(Fingers finger)
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Returns all the fingerprint templates belonging to the current individual
    /// </summary>
    /// <returns>A hash table of fingers and correxponding templates</returns>
    public Dictionary<Fingers, FingerprintTemplate> GetFingerTemplates()
    {
      if (Connection != null && Connection.State == ConnectionState.Open)
        if (ActiveIndividual != null)
        {
          Dictionary<Fingers, FingerprintTemplate> templates = new Dictionary<Fingers, FingerprintTemplate>();
          using (IDataReader rdr = InternalGetFingerTemplates())
          {
            try
            {
              while (rdr.Read())
              {
                FingerprintTemplate template;
                Fingers finger;
                ReadTemplate(rdr, out finger, out template);
                templates.Add(finger, template);
              }
            }
            finally
            {
              rdr.Close();
            }
          }
          return templates;
        }
        else throw new FPLibraryException(FPLibraryException.LibraryDalError, "Active individual not set");
      else
        throw new FPLibraryException(FPLibraryException.LibraryDalError, "Connection is null or closed");
    }
    /// <summary>
    /// Steps through all available temmplates and match fingerprint templates
    /// </summary>
    /// <param name="comparer">Delegate function that does the actual matching in the context desgnated by a particular finger</param>
    /// <param name="matchAny">If true any one finger match is sufficient for matching, if false all fingers must match</param>
    /// <param name="fingerContexts">List of matching contexts indexed by finger</param>
    /// <param name="matchFingers">Flags of fingers available for matching</param>
    /// <returns></returns>
    public object Identify(CompareFingerDelegate comparer, bool matchAny, Dictionary<Fingers, int> fingerContexts, Fingers matchFingers)
    {
      if (Connection != null && Connection.State == ConnectionState.Open)
      {
        using (IDataReader rdr = InternalGetTemplates()) //Reader must be sorted by individual
        {
          Fingers matchedFingers = 0; //fingers that have been matched
          FingerprintTemplate template;
          Fingers finger;
          while (rdr.Read())
          {
            ReadTemplate(rdr, out finger, out template);
            if ((matchFingers & finger) != 0)
            {
              if (comparer(fingerContexts[finger], template))
                matchedFingers |= finger;
            }
            if ((matchAny & ((matchFingers & matchedFingers) != 0)) | (matchFingers == matchedFingers)) //individual matched
              return rdr[IndividualFieldName];
          } //while
          return null; //No match found
        }
      }
      else
      {
        throw new FPLibraryException(FPLibraryException.LibraryDalError, "Connection is null or closed");
      }
    }
    #endregion
    #region Protected Methods
    protected virtual System.Drawing.Bitmap InternalGetFingerprintImage(Fingers finger)
    {
      throw new NotImplementedException();
    }
    protected virtual void InternalSaveTemplate(byte fingerCode, FingerprintTemplate fingerPrintTemplate)
    {
    }
    protected virtual IDataReader InternalGetFingerTemplates()
    {
      throw new NotImplementedException();
    }
    protected virtual IDataReader InternalGetTemplates()
    {
      throw new NotImplementedException();
    }
    protected void ReadTemplate(IDataReader rdr, out Fingers finger, out FingerprintTemplate template)
    {
      byte[] buff = (byte[])rdr[TemplateFieldName];
      int quality = Convert.ToInt16(rdr[QualityFieldName]);
      template = new FingerprintTemplate { Size = buff.Length, Buffer = buff, Quality = quality };
      Byte fingerCode = Convert.ToByte(rdr[FingerFieldName]);
      finger = MatchFingerCode(fingerCode);
    }
    protected virtual bool IndividualsIsEqual(object individualA, object individualB)
    {
      return individualA == individualB;
    }
    private Fingers MatchFingerCode(byte fingerCode)
    {
      foreach (var kvp in FingerValueTable)
      {
        if (kvp.Value==fingerCode)
        {
        	return kvp.Key;
        }
      }
      return (byte)0;
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
        if (Connection != null)
          if (Connection.State == ConnectionState.Open)
            Connection.Close();
          else if (Connection.State != ConnectionState.Closed)
            throw new FPLibraryException(FPLibraryException.LibraryDalError, "Unable to close FPDal connection");
      if (Connection != null)
        Connection.Dispose();
    }
    ~FPDal()
    {
      Dispose(false);
    }
    #endregion
  }
}
