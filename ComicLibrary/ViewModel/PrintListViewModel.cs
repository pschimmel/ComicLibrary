﻿using System.Windows.Documents;
using ComicLibrary.Model.Entities;
using ComicLibrary.View.Comparers;
using ComicLibrary.ViewModel.Helpers;

namespace ComicLibrary.ViewModel
{
  public class PrintListViewModel : PrintActiveLibraryViewModel
  {
    private readonly bool _reduceIssues;
    private readonly bool _excludeLowGrades;
    private readonly double _excludedGradeThreshold;

    public PrintListViewModel(bool reduceIssues, bool excludeLowGrades, Grade excludedGradeThreshold)
      : base()
    {
      _reduceIssues = reduceIssues;
      _excludeLowGrades = excludeLowGrades;
      _excludedGradeThreshold = excludedGradeThreshold.Number;
    }

    protected override FlowDocument CreateReport(ActiveLibraryViewModel library)
    {
      var document = new FlowDocument();

      // Add header
      document.AddHeader1(library.Name);

      // Add series 
      var series = library.Comics.Where(x => !string.IsNullOrWhiteSpace(x.IssueNumber) && !string.IsNullOrWhiteSpace(x.Series))
                                 .Select(x => x.Series)
                                 .Distinct()
                                 .Order();

      foreach (var serie in series)
      {
        document.AddHeader2(serie);
        var listOfIssues = library.Comics.Where(x => !string.IsNullOrWhiteSpace(x.IssueNumber) && x.Series == serie)
                                         .OrderBy(x => x.IssueNumber, new StringAsNumberComparer())
                                         .ToList();
        int? lastIssue = null;
        string text = "";

        for (int i = 0; i < listOfIssues.Count; i++)
        {
          var issue = listOfIssues[i];

          if (_excludeLowGrades && issue.Condition.Number < _excludedGradeThreshold)
            continue;

          if (_reduceIssues && int.TryParse(issue.IssueNumber, out int currentIssueNumber))
          {
            // Reduce a run of issues into a range
            if (currentIssueNumber != lastIssue) // In case there are duplicates
            {
              if (lastIssue.HasValue && lastIssue + 1 == currentIssueNumber)
              {
                if (!text.EndsWith('-'))
                  text += "-";

                if (i == listOfIssues.Count - 1)
                  text += currentIssueNumber;
              }
              else
              {
                if (text.EndsWith('-'))
                  text += lastIssue;

                if (text.Length != 0)
                  text += ", ";

                text += currentIssueNumber;
              }
            }

            lastIssue = currentIssueNumber;
          }
          else
          {
            // List all items individually.
            if (text.Length != 0)
              text += ", ";

            text += issue.IssueNumber;
          }
        }

        if (text.EndsWith('-') && lastIssue != null)
          text += lastIssue;

        document.AddParagraph(text);
      }

      return document;
    }
  }
}
