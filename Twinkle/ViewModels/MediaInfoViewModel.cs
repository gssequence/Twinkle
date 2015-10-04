using Livet.Messaging;
using Livet.Messaging.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinkle.Models;
using Twinkle.Mvvm;

namespace Twinkle.ViewModels
{
    public class MediaInfoViewModel : BindableViewModel
    {
        private MediaInfo _model;

        public Uri ThumbnailUri { get { return _model.ThumbnailUri; } }

        public Uri Uri { get { return _model.Uri; } }

        public string FileName
        {
            get
            {
                var path = Uri.AbsoluteUri;
                if (path.EndsWith(":orig"))
                    path = path.Substring(0, path.Length - ":orig".Length);
                return Path.GetFileName(path);
            }
        }

        public string SaveDialogFilter
        {
            get
            {
                var ext = Path.GetExtension(FileName).ToLower();
                if (ext.Length > 0) ext = ext.Substring(1);
                return ext.ToUpper() + " (*." + ext + ")|*." + ext + "|" + GlobalizationService.Instance.GetString("FilterAllFiles") + " (*.*)|*.*";
            }
        }

        public void Open()
        {
            var msg = new TransitionMessage(new ImageWindowViewModel(this), "OpenImageWindowMessageKey");
            Messenger.Raise(msg);
        }

        public MediaInfoViewModel(MediaInfo model)
        {
            _model = model;
        }

        public void Save()
        {
            if (Settings.Current.CustomImageSaveFolder)
            {
                _model.Save(Path.Combine(Settings.Current.CustomImageSaveFolderPath, FileName).NextFilePath());
            }
            else
            {
                var msg = new SavingFileSelectionMessage("SaveDialogMessageKey");
                Messenger.Raise(msg);
            }
        }

        public void SaveFileDialogCallback(SavingFileSelectionMessage msg)
        {
            if (msg.Response == null || msg.Response.Count() == 0) return;
            _model.Save(msg.Response.First());
        }

        public void OpenInBrowser()
        {
            _model.OpenInBrowser();
        }
    }
}
