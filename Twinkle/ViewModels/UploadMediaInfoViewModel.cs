using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Twinkle.ViewModels
{
    public class UploadMediaInfoViewModel
    {
        private IList<UploadMediaInfoViewModel> _parent;
        private byte[] _data;

        private BitmapImage _thumbnailImage;
        public BitmapImage ThumbnailImage
        {
            get
            {
                if (_thumbnailImage == null)
                {
                    using (MemoryStream ms = new MemoryStream(_data))
                    {
                        ms.Seek(0, SeekOrigin.Begin);

                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = ms;
                        bi.EndInit();
                        bi.Freeze();
                        _thumbnailImage = bi;
                    }
                }
                return _thumbnailImage;
            }
        }

        public IEnumerable<byte> GetBytes()
        {
            return _data;
        }

        public void RemoveFromParent()
        {
            _parent.Remove(this);
        }

        private UploadMediaInfoViewModel(IList<UploadMediaInfoViewModel> parent)
        {
            _parent = parent;
        }

        public UploadMediaInfoViewModel(IList<UploadMediaInfoViewModel> parent, byte[] data) : this(parent)
        {
            _data = data;
        }

        public UploadMediaInfoViewModel(IList<UploadMediaInfoViewModel> parent, string path) : this(parent)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                using (var br = new BinaryReader(fs))
                {
                    _data = br.ReadBytes((int)fs.Length);
                }
            }
        }
    }
}
