﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MediaPlayer
{
    //https://www.youtube.com/embed/1VnSzOzL0UM?autoplay=1
    //https://gdata.youtube.com/feeds/api/videos/-biwNmWLFa5Q?v=2

    public sealed partial class MainPage : Page
    {
        private MediaPlayer mediaPlayer;
        public static MainPage current;
        private DataLayer searchLayer;

        public MainPage()
        { 
            
            this.InitializeComponent();
            MusicPlayer.AudioCategory = AudioCategory.BackgroundCapableMedia;
            mediaPlayer = new MediaPlayer(this, MusicPlayer, PlayPause, ProgressSlider);
            mediaPlayer.OnMediaFailed += MediaEnds;
            mediaPlayer.OnMediaEnded += MediaEnds;            

            MediaControl.NextTrackPressed += MediaControl_NextTrackPressed;
            MediaControl.PreviousTrackPressed += MediaControl_PreviousTrackPressed;

            list.ItemClick += Grid_ItemClick;
            current = this;

            searchLayer = new DataLayer();
            Task.Run(()  =>searchLayer.getTracksByPreferences(this, list));
        }

        private async void MediaControl_PreviousTrackPressed(object sender, object e)
        {
             await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => prevTrack());
        }

        private async void MediaControl_NextTrackPressed(object sender, object e)
        {
             await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => nextTrack());
        }

        private void MediaEnds(object sender, object e)
        {
            nextTrack();
        }
        private async Task LoadTrack(Track track)
        {
            try
            {
                VideoTitleHolder.Text = track.Name + " - " + track.Artist;
                VideoImageHolder.Source = new BitmapImage(track.ImageUri);
                mediaPlayer.CurrentTrack = track;
                mediaPlayer.play(); 
            }
            catch (Exception er)
            {
                new MessageDialog("Error", er.Message).ShowAsync();
            }

        }

        public static BitmapImage ImageFromRelativePath(FrameworkElement parent, string path)
        {
            var uri = new Uri(parent.BaseUri, path);
            BitmapImage result = new BitmapImage();
            result.UriSource = uri;
            return result;
        } 
        public async void nextTrack()
        {
            mediaPlayer.stop();
            mediaPlayer.MediaIndex += 1;
            mediaPlayer.MediaIndex %= GlobalArray.list.Count;           
            Track new_item = GlobalArray.list[mediaPlayer.MediaIndex];    
            await LoadTrack(new_item);           
        }
     
        public async void prevTrack()
        {
            mediaPlayer.stop();
            mediaPlayer.MediaIndex -= 1;
            if (mediaPlayer.MediaIndex < 0) mediaPlayer.MediaIndex = GlobalArray.list.Count - 1;   

            Track new_item = GlobalArray.list[mediaPlayer.MediaIndex];
            await LoadTrack(new_item);
            
        }

        public async void Grid_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            Track new_item = ((Track)e.ClickedItem);
            if (mediaPlayer.MediaIndex == list.Items.IndexOf(new_item))
            {
               return;
            }

            mediaPlayer.stop();
            mediaPlayer.MediaIndex = list.Items.IndexOf(new_item);
            await LoadTrack(new_item);            
        }


        private void PlayPause_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mediaPlayer.playPause();
        }

        private async void FeelLucky_Tapped(object sender, TappedRoutedEventArgs e)
        {
            list.Items.Clear();
            GlobalArray.list.Clear();
            searchLayer.cancelSearch();
            searchLayer = new DataLayer();
            Task.Run(() => searchLayer.getTracksByPreferences(this, list));
        }
        private void Prev_track_Tapped(object sender, TappedRoutedEventArgs e)
        {
            prevTrack();
        }      

        private void Next_track_Tapped(object sender, TappedRoutedEventArgs e)
        {
            nextTrack();
        }

        private async void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            if (SettingsFlyout1.Queue == false)
            {
                list.Items.Clear();
                GlobalArray.list.Clear();
            }
            
            try
            {
                searchLayer.cancelSearch();
                searchLayer = new DataLayer();
                Preferences.addTag(args.QueryText);
                string txt = args.QueryText;
                Task.Run(() => searchLayer.getTrackByTag(this, list, txt));  
            }
            catch (Exception exp)
            {
                new MessageDialog("Error", exp.Message).ShowAsync();
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                SettingsFlyout1 sf = new SettingsFlyout1();
                sf.ShowIndependent();
            }
            
        }

        private void Playlist_Click(object sender, RoutedEventArgs e)
        {
            var page = new PlaylistPage();
            Window.Current.Content = page;
        }


        private void SearchBox1_Loaded(object sender, RoutedEventArgs e)
        {
            SearchBox1.SearchHistoryEnabled = SettingsFlyout1.History;
        }
      
        private async void AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            int length = list.SelectedItems.Count;
            for (int i = 0; i < length; i++)
               await PlayList.addToPlayList((Track)list.SelectedItems[i]);
        }      

    }
}
