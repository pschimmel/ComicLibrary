using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
      Conditions = Enum.GetValues(typeof(Condition)).Cast<Condition>().ToList();
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

    public Condition Condition { get; set; }

    public IOptionItemViewModel<Publisher> Publisher { get; set; }

    public IOptionItemViewModel<Country> Country { get; set; }

    public bool CollectorsEdition { get; set; }

    public bool LimitedEdition { get; set; }

    public List<IOptionItemViewModel<Publisher>> Publishers { get; }

    public List<IOptionItemViewModel<Country>> Countries { get; }

    public List<Condition> Conditions { get; }

    public ObservableCollection<ComicImageViewModel> ComicImages { get; } = [];

    #endregion

    #region Commands

    #region Add Image

    public ICommand AddImageCommand => _addImageCommand ??= new ActionCommand(AddImage);

    private void AddImage()
    {
      var dialog = new OpenFileDialog
      {
        Filter = "Jpeg Image Files (*.jpg)|*.jpg|Portable Network Graphics Files (*.png)|*.png"
      };

      if (dialog.ShowDialog() == true && File.Exists(dialog.FileName))
      {
        bool isPng = Path.GetExtension(dialog.FileName).Equals(".png", StringComparison.CurrentCultureIgnoreCase);
        var bitmap = new BitmapImage(new Uri(dialog.FileName, UriKind.Absolute));
        ComicImages.Add(new ComicImageViewModel(ImageHelpers.ToArray(bitmap, isPng)));
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
