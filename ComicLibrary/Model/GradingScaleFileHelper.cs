using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;
using ComicLibrary.Model.Entities;

namespace ComicLibrary.Model
{
  public static class GradingScaleFileHelper
  {
    #region Constants

    private const string GradingScaleFolder = "Grading Scales";
    private const string GradingScaleKey = "GradingScale";
    private const string GradesKey = "Grades";
    private const string GradeKey = "Grade";
    private const string NameKey = "Name";
    private const string NumberKey = "Number";
    private const string ShortNameKey = "ShortName";

    #endregion

    #region Loading

    public static IEnumerable<GradingScale> GetScales()
    {
      var location = Assembly.GetEntryAssembly().Location;
      var dir = Path.GetDirectoryName(location);
      var path = Path.Combine(dir, GradingScaleFolder);

      var files = Directory.GetFiles(path, "*.xml");
      List<GradingScale> scales = new(files.Length);

      foreach (var file in files)
      {
        scales.Add(LoadGradingScale(file));
      }

      return scales;
    }

    public static GradingScale LoadGradingScale(string filePath)
    {
      if (!File.Exists(filePath))
        return null;

      try
      {
        var xml = new XmlDocument();
        xml.Load(filePath);

        if (xml.FirstChild.Name == GradingScaleKey)
        {
          var scaleNode = xml.FirstChild;
          var scale = new GradingScale(scaleNode.Attributes.GetNamedItem(NameKey).Value);

          foreach (XmlNode scaleChildNode in scaleNode.ChildNodes)
          {
            if (scaleChildNode.Name == GradesKey)
            {
              var gradesNode = scaleChildNode;
              foreach (XmlNode gradeNode in gradesNode.ChildNodes)
              {
                if (gradeNode.Name == GradeKey)
                {
                  Grade grade = TryReadGradeAttribute(gradeNode.Attributes, scale.Name);

                  if (grade != null)
                    scale.Grades.Add(grade);
                }
              }
            }
          }

          return scale;
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format(Properties.Resources.CannotLoadFileMessage, filePath) + Environment.NewLine + ex.Message);
      }

      return null;
    }

    private static Grade TryReadGradeAttribute(XmlAttributeCollection attributes, string scale)
    {
      string name = attributes.GetNamedItem(NameKey)?.Value;
      string shortName = attributes.GetNamedItem(ShortNameKey)?.Value;
      string numberAsString = attributes.GetNamedItem(NumberKey)?.Value;
      return !string.IsNullOrWhiteSpace(name) && double.TryParse(numberAsString, CultureInfo.InvariantCulture, out double number)
        ? new Grade(number, name, shortName, scale)
        : null;
    }

    #endregion
  }
}