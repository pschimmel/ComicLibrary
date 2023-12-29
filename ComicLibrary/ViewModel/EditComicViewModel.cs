using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ComicLibrary.Model;
using ComicLibrary.Model.Entities;
using ES.Tools.Core.MVVM;
using Microsoft.Win32;

namespace ComicLibrary.ViewModel
{
  public class EditComicViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    #region Fields

    private ActionCommand _addImageCommand;
    private ActionCommand<ComicImageViewModel> _removeImageCommand;
    private ActionCommand<ComicImageViewModel> _exportImageCommand;
    private ActionCommand _selectGradeCommand;
    private readonly Comic _comic;

    #endregion

    #region Constructor

    public EditComicViewModel(Comic comic, string library)
    {
      Publishers =
      [
        new EmptyOptionItemViewModel<Publisher>(),
        .. Globals.Instance.Publishers.OrderBy(x => x.Name).Select(x => new OptionItemViewModel<Publisher>(x))
      ];

      Countries =
      [
        new EmptyOptionItemViewModel<Country>(),
        .. Globals.Instance.Countries.OrderBy(x => x.Name).Select(x => new OptionItemViewModel<Country>(x))
      ];

      _comic = comic;
      Series = comic.Series;
      Year = comic.Year;
      IssueNumber = comic.IssueNumber;
      Title = comic.Title;
      Condition = comic.Condition;
      Publisher = Publishers.FirstOrDefault(x => x.Option == comic.Publisher);
      Country = Countries.FirstOrDefault(x => x.Option == comic.Country);
      Comment = comic.Comment;
      LimitedEdition = comic.LimitedEdition;
      CollectorsEdition = comic.CollectorsEdition;
      Library = library;

      foreach (var imageAsString in comic.ImagesAsString)
      {
        ComicImages.Add(new ComicImageViewModel(imageAsString));
      }
    }

    #endregion

    #region Properties

    public string Library { get; set; }

    public string Series { get; set; }

    public int? Year { get; set; }

    public string Title { get; set; }

    public int? IssueNumber { get; set; }

    public string Comment { get; set; }

    public Grade Condition { get; set; }

    public IOptionItemViewModel<Publisher> Publisher { get; set; }

    public IOptionItemViewModel<Country> Country { get; set; }

    public bool CollectorsEdition { get; set; }

    public bool LimitedEdition { get; set; }

    public IEnumerable<IOptionItemViewModel<Publisher>> Publishers { get; }

    public IEnumerable<IOptionItemViewModel<Country>> Countries { get; }

    public ObservableCollection<ComicImageViewModel> ComicImages { get; } = [];

    #endregion

    #region Commands

    #region Select Grade

    public ICommand SelectGradeCommand => _selectGradeCommand ??= new ActionCommand(SelectGrade);

    private void SelectGrade()
    {
      var vm = new GradePickerViewModel(Condition);
      var view = ViewFactory.Instance.CreateView(vm);
      if (view.ShowDialog() == true)
      {
        Condition = vm.SelectedGrade;
        OnPropertyChanged(nameof(Condition));
      }
    }

    #endregion

    #region Add Image

    public ICommand AddImageCommand => _addImageCommand ??= new ActionCommand(AddImage);

    private void AddImage()
    {
      var dialog = new OpenFileDialog
      {
        Filter = $"{Properties.Resources.JpegFiles}|*.jpg|{Properties.Resources.PngFiles}|*.png",
        CheckFileExists = true
      };

      if (dialog.ShowDialog() == true && File.Exists(dialog.FileName))
      {
        var imageAsByteArray = ImageHelpers.LoadImage(dialog.FileName);
        if (imageAsByteArray != null)
        {
          ComicImages.Add(new ComicImageViewModel(imageAsByteArray));
        }
      }
    }

    #endregion

    #region Remove Image

    public ICommand RemoveImageCommand => _removeImageCommand ??= new ActionCommand<ComicImageViewModel>(RemoveImage);

    private void RemoveImage(ComicImageViewModel vm)
    {
      ComicImages.Remove(vm);
    }

    #endregion

    #region Export Image

    public ICommand ExportImageCommand => _exportImageCommand ??= new ActionCommand<ComicImageViewModel>(ExportImage, CanExportImage);

    private void ExportImage(ComicImageViewModel vm)
    {
      var dialog = new SaveFileDialog
      {
        Filter = $"{Properties.Resources.JpegFiles}|*.jpg|{Properties.Resources.PngFiles}|*.png",
        FileName = FileHelper.GetValidFileName(Path.ChangeExtension(Title, "jpg"))
      };

      if (dialog.ShowDialog() == true)
      {
        ImageHelpers.SaveImage((BitmapSource)vm.Image, FileHelper.GetValidFileName(dialog.FileName), Path.GetExtension(dialog.FileName) == ".png");
      }
    }

    private bool CanExportImage(ComicImageViewModel vm)
    {
      return vm?.Image != null;
    }

    #endregion

    #endregion

    #region Public Methods

    public void ApplyChanges()
    {
      _comic.Series = Series;
      _comic.Year = Year;
      _comic.IssueNumber = IssueNumber;
      _comic.Title = Title;
      _comic.Publisher = Publisher.Option;
      _comic.Condition = Condition;
      _comic.Country = Country.Option;
      _comic.Comment = Comment;
      _comic.CollectorsEdition = CollectorsEdition;
      _comic.LimitedEdition = LimitedEdition;

      _comic.ImagesAsString.Clear();

      foreach (var image in ComicImages)
      {
        _comic.ImagesAsString.Add(image.ToString());
      }
    }

    #endregion
  }
}
