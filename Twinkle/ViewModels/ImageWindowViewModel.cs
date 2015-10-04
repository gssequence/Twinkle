using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinkle.Mvvm;

namespace Twinkle.ViewModels
{
    public class ImageWindowViewModel : BindableViewModel
    {
        public MediaInfoViewModel MediaInfoViewModel { get; private set; }

        private int _angle = 0;
        public int Angle
        {
            get { return _angle; }
            set { SetProperty(ref _angle, value % 360); }
        }

        public ImageWindowViewModel(MediaInfoViewModel model)
        {
            MediaInfoViewModel = model;
        }

        public void RotateCounterClockwise()
        {
            Angle -= 90;
        }

        public void ButtonRotateClockwise()
        {
            Angle += 90;
        }
    }
}
