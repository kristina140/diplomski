using DesktopApp.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace DesktopApp.ViewModels
{
    public abstract class AddOnlyViewModelBase<LModel,CModel> : ViewModelBase
        where CModel : new()
    {
        public AddOnlyViewModelBase()
        {
            _deleteItemCommand = new DelegateCommand(OnDeleteItem, CanDeleteItem);
            _startAddCommand = new DelegateCommand(OnStartAdd, CanStartAdd);
            _discardAddCommand = new DelegateCommand(OnDiscardAdd, CanDiscardAdd);
            _saveAddCommand = new DelegateCommand(OnSaveAdd, CanSaveAdd);
        }

        #region shared properties
        private ObservableCollection<LModel> _itemsObservable;
        public ObservableCollection<LModel> ItemsObservable
        {
            get => _itemsObservable;
            set => SetProperty(ref _itemsObservable, value);
        }

        private CModel _newItem;
        public CModel NewItem
        {
            get => _newItem;
            set => SetProperty(ref _newItem, value);
        }
        
        private bool _inAddMode = false;
        public bool InAddMode
        {
            get => _inAddMode;
            set => SetProperty(ref _inAddMode, value);
        }

        private LModel _selectedItem;
        public LModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
            }
        }
        private int? _selectedItemIndex;
        public int? SelectedItemIndex
        {
            get => _selectedItemIndex;
            set => SetProperty(ref _selectedItemIndex, value);
        }
        #endregion

        #region shared commands

        private readonly DelegateCommand _deleteItemCommand;
        public ICommand DeleteItemCommand => _deleteItemCommand;
        
        private readonly DelegateCommand _startAddCommand;
        public ICommand StartAddCommand => _startAddCommand;

        private readonly DelegateCommand _discardAddCommand;
        public ICommand DiscardAddCommand => _discardAddCommand;

        private readonly DelegateCommand _saveAddCommand;
        public ICommand SaveAddCommand => _saveAddCommand;
        #endregion

        #region command implementations
        protected abstract void OnDeleteItem(object commandParameter);

        protected virtual bool CanDeleteItem(object commandParameter)
        {
            return true;
        }

        protected virtual void OnStartAdd(object commandParameter)
        {
            InAddMode = true;
            NewItem = new CModel();
        }

        protected virtual bool CanStartAdd(object commandParameter)
        {
            return true;
        }

        protected virtual void OnDiscardAdd(object commandParameter = null)
        {
            ValidationErrors = null;
            InAddMode = false;
            NewItem = default(CModel); //set null
        }

        protected virtual bool CanDiscardAdd(object commandParameter)
        {
            return true;
        }

        protected abstract void OnSaveAdd(object commandParameter);

        protected virtual bool CanSaveAdd(object commandParameter)
        {
            return true;
        }
        #endregion
    }
}
