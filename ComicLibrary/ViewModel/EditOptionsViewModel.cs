using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ComicLibrary.Model.Entities;
using ES.Tools.Core.MVVM;

namespace ComicLibrary.ViewModel
{
  public class EditOptionsViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    #region Fields 

    private IOption _selectedOption;
    private readonly Type _type;
    private ActionCommand _addOptionCommand;
    private ActionCommand _removeOptionCommand;
    private ActionCommand _editOptionCommand;
    private readonly string _singleItemHeader;

    #endregion

    #region Constructor

    public EditOptionsViewModel(IEnumerable<IOption> options, Type type, string header, string singleItemHeader)
    {
      Options = new ObservableCollection<IOption>(options);
      _type = type;
      Header = header;
      _singleItemHeader = singleItemHeader;
    }

    #endregion

    #region Properties

    public ObservableCollection<IOption> Options { get; }

    public IOption SelectedOption
    {
      get => _selectedOption;
      set
      {
        if (!Equals(_selectedOption, value))
        {
          _selectedOption = value;
          OnPropertyChanged(nameof(SelectedOption));
        }
      }
    }

    public string Header { get; }

    #endregion

    #region Commands

    #region Add Option

    public ICommand AddOptionCommand => _addOptionCommand ??= new ActionCommand(AddOption);

    private void AddOption()
    {
      using var settingVM = new EditOptionViewModel((IOption)Activator.CreateInstance(_type), _singleItemHeader);
      var view = ViewFactory.Instance.CreateView(settingVM);

      if (view.ShowDialog() == true && !Options.Contains(settingVM.Option))
      {
        Options.Add(settingVM.Option);
        SelectedOption = settingVM.Option;
      }
    }

    #endregion

    #region Remove Option

    public ICommand RemoveOptionCommand => _removeOptionCommand ??= new ActionCommand(RemoveOption, CanRemoveOption);

    private void RemoveOption()
    {
      if (MessageBox.Show(Properties.Resources.RemoveOptionQuestion, Properties.Resources.Warning, MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
      {
        Options.Remove(SelectedOption);
      }
    }

    private bool CanRemoveOption()
    {
      return SelectedOption != null;
    }

    #endregion  

    #region Edit Option

    public ICommand EditOptionCommand => _editOptionCommand ??= new ActionCommand(EditOption, CanEditOption);

    private void EditOption()
    {
      using var settingVM = new EditOptionViewModel(SelectedOption, _singleItemHeader);
      var view = ViewFactory.Instance.CreateView(settingVM);

      if (view.ShowDialog() == true &&
          !Options.Contains(settingVM.Option) &&
          !Equals(SelectedOption, settingVM.Option))
      {
        var index = Options.IndexOf(SelectedOption);
        Options.Insert(index, settingVM.Option);
        Options.Remove(SelectedOption);
      }
    }

    private bool CanEditOption()
    {
      return SelectedOption != null;
    }

    #endregion

    #endregion
  }
}
