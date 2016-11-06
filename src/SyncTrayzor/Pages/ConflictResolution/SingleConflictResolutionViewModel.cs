using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet;
using SyncTrayzor.Services;

namespace SyncTrayzor.Pages.ConflictResolution
{
    public class SingleConflictResolutionViewModel : Screen
    {
        private readonly IProcessStartProvider processStartProvider;

        public ConflictViewModel SelectedConflict { get; set; }

        public SingleConflictResolutionViewModel(IProcessStartProvider processStartProvider)
        {
            this.processStartProvider = processStartProvider;
        }

        public void ShowFileInFolder()
        {
            this.processStartProvider.ShowInExplorer(this.SelectedConflict.FilePath);
        }

        public void ChooseOriginal(ConflictViewModel conflict)
        {
            if (!this.ResolveConflict(this.SelectedConflict.ConflictSet, conflict.ConflictSet.File.FilePath))
                return;

            // The conflict will no longer exist, so remove it
            this.Conflicts.Remove(conflict);
        }

        public void ChooseConflictFile(ConflictOptionViewModel conflictOption)
        {
            if (!this.ResolveConflict(this.SelectedConflict.ConflictSet, conflictOption.ConflictOption.FilePath))
                return;

            // The conflict will no longer exist, so remove it
            var correspondingVm = this.Conflicts.First(x => x.ConflictOptions.Contains(conflictOption));
            this.Conflicts.Remove(correspondingVm);
        }

        private bool ResolveConflict(ConflictSet conflictSet, string filePath)
        {
            // This can happen e.g. if the file chosen no longer exists
            try
            {
                this.conflictFileManager.ResolveConflict(conflictSet, filePath, this.DeleteToRecycleBin);
                return true;
            }
            catch (IOException e)
            {
                this.windowManager.ShowMessageBox(
                    Localizer.F(Resources.ConflictResolutionView_Dialog_Failed_Message, e.Message),
                    Resources.ConflictResolutionView_Dialog_Failed_Title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return false;
            }
        }
    }
}
