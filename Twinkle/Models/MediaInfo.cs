using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace Twinkle.Models
{
    public class MediaInfo
    {
        public Uri ThumbnailUri { get; private set; }

        public Uri Uri { get; private set; }

        public MediaInfo(Uri thumbnailUri, Uri uri)
        {
            ThumbnailUri = thumbnailUri;
            Uri = uri;
        }

        public void Save(string path)
        {
            new WebClient().DownloadDataTaskAsync(Uri).ToObservable()
                .Subscribe(data =>
                {
                    try
                    {
                        File.WriteAllBytes(path, data);
                        ApplicationMessageService.Instance.Messages.Add(new ApplicationMessage(ApplicationMessage.MessageType.Info, "MessageImageSaved", "Format", null, new[] { path }));
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex));
                    }
                },
                ex => ApplicationMessageService.Instance.Messages.Add(ApplicationMessage.CreateExceptionMessage(ex)));
        }

        public void OpenInBrowser()
        {
            Process.Start(Uri.AbsoluteUri);
        }
    }
}
