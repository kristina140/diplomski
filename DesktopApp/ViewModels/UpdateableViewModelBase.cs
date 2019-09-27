using DesktopApp.Commands;
using DesktopApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace DesktopApp.ViewModels
{
    public abstract class UpdateableViewModelBase<UModel, CModel> : AddOnlyViewModelBase<UModel, CModel>
         where UModel : UpdateableModelBase
         where CModel : new()
    {
        public UpdateableViewModelBase()
        {
            _editItemCommand = new DelegateCommand(OnEditItem, CanEditItem);
            _discardEditCommand = new DelegateCommand(OnDiscardEdit, CanDiscardEdit);
            _saveEditCommand = new DelegateCommand(OnSaveEdit, CanSaveEdit);
        }
        
        #region shared commands
        private readonly DelegateCommand _editItemCommand;
        public ICommand EditItemCommand => _editItemCommand;

        private readonly DelegateCommand _discardEditCommand;
        public ICommand DiscardEditCommand => _discardEditCommand;

        private readonly DelegateCommand _saveEditCommand;
        public ICommand SaveEditCommand => _saveEditCommand;
        #endregion

        #region command implementations
        protected virtual void OnEditItem(object commandParameter)
        {
            if (SelectedItemIndex.HasValue && SelectedItemIndex >= 0 && SelectedItemIndex < ItemsObservable.Count)
            {
                ItemsObservable[SelectedItemIndex.Value].InEditMode = true;
            }
        }

        protected virtual bool CanEditItem(object commandParameter)
        {
            return true;
        }

        protected abstract void OnDiscardEdit(object commandParameter);

        protected virtual bool CanDiscardEdit(object commandParameter)
        {
            return true;
        }

        protected abstract void OnSaveEdit(object commandParameter);

        protected virtual bool CanSaveEdit(object commandParameter)
        {
            return true;
        }
        #endregion

    }
}
