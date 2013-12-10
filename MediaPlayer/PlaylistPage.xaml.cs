﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MediaPlayer
{
    public sealed partial class PlaylistPage : Page
    {
        private MediaPlayer mediaPlayer;

        public PlaylistPage()
        {
            this.InitializeComponent();
            MusicPlayer.AudioCategory = AudioCategory.BackgroundCapableMedia;
            mediaPlayer = new MediaPlayer(this, MusicPlayer , PlayPause, ProgressSlider);
            mediaPlayer.OnMediaFailed += MediaEnds;
            mediaPlayer.OnMediaEnded += MediaEnds;

            MediaControl.NextTrackPressed += MediaControl_NextTrackPressed;
            MediaControl.PreviousTrackPressed += MediaControl_PreviousTrackPressed;
            MediaControl.PlayPressed += MediaControl_PlayPressed;
            MediaControl.PausePressed += MediaControl_PausePressed;
            MediaControl.PlayPauseTogglePressed += MediaControl_PlayPauseTogglePressed;

            list.ItemClick += Grid_ItemClick;
            Task.Run(()=>PlayList.readPlayList(list));
        }

        private async void MediaControl_PlayPauseTogglePressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                PlayPause_Tapped(PlayPause, null);
            });
        }
        private async void MediaControl_PausePressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => PlayPause_Tapped(PlayPause, null));
        }

        private async void MediaControl_PlayPressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => PlayPause_Tapped(PlayPause, null));
        }

        private async void MediaControl_PreviousTrackPressed(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => prevTrack());
        }

        private async void MediaControl_NextTrackPressed(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => nextTrack());
        }

        private void MediaEnds(object sender, EventArgs e)
        {
            nextTrack();
        }

        private async Task LoadTrack(Track track)
        {
            YoutubeDecoder decoder = new YoutubeDecoder();
            decoder.VideoID = track.VideoID;
            try
            {
                track.CacheUriString = await decoder.fetchURL();
            }
            catch (Exception error)
            {
                if (error.Message == ExceptionMessages.CONNECTION_FAILED)
                {
                    Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
                    {
                        new MessageDialog("Unable to load track due to internet problems!", "Error").ShowAsync();
                    });
                }
                return;
            }
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
            {
                mediaPlayer.CurrentTrack = track;
                mediaPlayer.play();
                VideoImageHolder.Source = new BitmapImage(track.ImageUri);
                VideoTitleHolder.Text = track.Name + " - " + track.Artist;
            });
        }

        private void Grid_ItemClick(object sender, ItemClickEventArgs e)
        {
            Track new_item = ((Track)e.ClickedItem);
            mediaPlayer.MediaIndex = PlayList.getIndex(new_item);
            mediaPlayer.stop();
            Task.Run(() => LoadTrack(new_item));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }
        private async void PlayPause_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (mediaPlayer.CurrentTrack == null)
            {
                if (PlayList.getSize() > 0)
                {
                    mediaPlayer.MediaIndex = 0;
                    await Task.Run(() => LoadTrack(PlayList.getElement(0)));
                    
                }
            }
            else
            { 
                mediaPlayer.playPause();
            }
        }

        public async void nextTrack()
        {
            if (PlayList.getSize() > 0)
            {
                mediaPlayer.stop();
                mediaPlayer.MediaIndex += 1;
                mediaPlayer.MediaIndex %= PlayList.getSize();
                Task.Run(() => LoadTrack(PlayList.getElement(mediaPlayer.MediaIndex)));
            }
        }

        public async void prevTrack()
        {
            if (PlayList.getSize() > 0)
            {
                mediaPlayer.stop();
                mediaPlayer.MediaIndex -= 1;
                if (mediaPlayer.MediaIndex < 0) mediaPlayer.MediaIndex = PlayList.getSize() - 1;
                Task.Run(() => LoadTrack(PlayList.getElement(mediaPlayer.MediaIndex)));
            }
        }

        private void Prev_track_Tapped(object sender, TappedRoutedEventArgs e)
        {
            prevTrack();
        }

        private void Next_track_Tapped(object sender, TappedRoutedEventArgs e)
        {
            nextTrack();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Window.Current.Content = MainPage.current;
        }
        


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // The code to remove!
            Button sender_button = (Button)sender;
            sender_button.IsEnabled = false;            
            int length = list.SelectedItems.Count;
            for (int i = 0; i < length; i++)
            {
                Track track_to_delete = (Track)list.SelectedItems[list.SelectedItems.Count - 1];
                await Task.Run(()=>PlayList.removeFromPlayList(track_to_delete, list));
            }
            list.SelectedIndex = -1;
            if (length != 0) new MessageDialog(length + " tracks were removed from playlist!", "Info").ShowAsync();           

            sender_button.IsEnabled = true;
        }
    }
}
