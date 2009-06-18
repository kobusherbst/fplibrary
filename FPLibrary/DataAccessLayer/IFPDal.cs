using System;
using System.Collections.Generic;
using GriauleFingerprintLibrary.DataTypes;
using System.Data;
using System.Drawing;

namespace FPLibrary.DataAccessLayer
{
  public interface IFPDal
  {
    /// <summary>
    /// Save a fingerprint template for the current individual
    /// </summary>
    /// <param name="finger">The finger to which the template belongs</param>
    /// <param name="fingerPrintTemplate">The template to be saved</param>
    void SaveTemplate(Fingers finger, FingerprintTemplate fingerPrintTemplate);
    /// <summary>
    /// Return all templates in the database, used for identification
    /// </summary>
    /// <returns>A datareader interface</returns>
    IDataReader GetTemplates();
    /// <summary>
    /// Returns the template for the specified finger of the current individual
    /// </summary>
    /// <param name="finger">The finger to which the template belongs</param>
    /// <returns>A fingerprint template</returns>
    FingerprintTemplate GetTemplate(Fingers finger);
    /// <summary>
    /// Returns all the fingerprint templates belonging to the current individual
    /// </summary>
    /// <returns>A hash table of fingers and correxponding templates</returns>
    Dictionary<Fingers,FingerprintTemplate> GetFingerTemplates();
    /// <summary>
    /// Process all fingerprint templates in the database
    /// </summary>
    /// <param name="comparer">Delegate function to compare finger templates</param>
    /// <param name="matchAny">True if any one finger needs to match, false if all fingers must match</param>
    /// <param name="fingerContexts">The contexts in which a particular finger should be matched</param>
    /// <param name="matchFingers">The fingers to be matched</param>
    /// <returns>Object key of identified individual or null if no match</returns>
    object Identify(CompareFingerDelegate comparer, bool matchAny, Dictionary<Fingers, int> fingerContexts, Fingers matchFingers);
    /// <summary>
    /// Read a fingerprint bitmap from the database for the current individual
    /// </summary>
    /// <param name="finger">The finger to which the image belongs</param>
    /// <returns>A finger print bitmap</returns>
    Bitmap GetFingerprintImage(Fingers finger);
  }
}
