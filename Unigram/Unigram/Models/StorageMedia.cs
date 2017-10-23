﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Unigram.Core.Helpers;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Unigram.Models
{
    public class StorageMedia : BindableBase
    {
        public StorageMedia(StorageFile file)
        {
            File = file;
        }

        public StorageFile File { get; private set; }

        protected BitmapImage _thumbnail;
        public BitmapImage Thumbnail
        {
            get
            {
                if (_thumbnail == null)
                    LoadThumbnail();

                return _thumbnail;
            }
        }

        protected ImageSource _preview;
        public ImageSource Preview
        {
            get
            {
                if (_preview == null)
                    Refresh();

                return _preview;
            }
        }

        protected string _caption;
        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                Set(ref _caption, value);
            }
        }

        protected int? _ttlSeconds;
        public int? TTLSeconds
        {
            get
            {
                return _ttlSeconds;
            }
            set
            {
                Set(ref _ttlSeconds, value);
            }
        }

        protected bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                Set(ref _isSelected, value);
            }
        }

        protected Rect? _fullRectangle;
        protected Rect? _cropRectangle;
        public Rect? CropRectangle
        {
            get
            {
                return _cropRectangle;
            }
            set
            {
                Set(ref _cropRectangle, value == _fullRectangle ? null : value);
            }
        }

        public bool IsPhoto => this is StoragePhoto;
        public bool IsVideo => this is StorageVideo;
        public bool IsCropped => CropRectangle.HasValue;

        private async void LoadThumbnail()
        {
            try
            {
                if (!File.Attributes.HasFlag(FileAttributes.Temporary))
                {
                    using (var thumbnail = await File.GetThumbnailAsync(ThumbnailMode.ListView, 96, ThumbnailOptions.UseCurrentScale))
                    {
                        if (thumbnail != null)
                        {
                            var bitmapImage = new BitmapImage();
                            await bitmapImage.SetSourceAsync(thumbnail);

                            _thumbnail = bitmapImage;
                        }
                    }

                    RaisePropertyChanged(() => Thumbnail);
                }
            }
            catch { }
        }

        public virtual async void Refresh()
        {
            if (CropRectangle.HasValue)
            {
                _preview = await ImageHelper.CropAndPreviewAsync(File, CropRectangle.Value);
            }
            else
            {
                _preview = await ImageHelper.GetPreviewBitmapAsync(File);
            }

            RaisePropertyChanged(() => Preview);
        }

        public virtual StorageMedia Clone()
        {
            var item = new StorageMedia(File);
            item._thumbnail = _thumbnail;

            return item;
        }

        public virtual void Reset()
        {
            IsSelected = false;
            Caption = null;
            CropRectangle = null;

            //_thumbnail = null;
            _preview = null;
        }

        public static async Task<StorageMedia> CreateAsync(StorageFile file, bool selected)
        {
            if (file.ContentType.Equals("video/mp4"))
            {
                return await StorageVideo.CreateAsync(file, selected);
            }
            else
            {
                return await StoragePhoto.CreateAsync(file, selected);
            }
        }
    }
}
