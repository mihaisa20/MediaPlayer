﻿using MediaPlayer.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Data.Xml.Dom;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaPlayer
{
    class TopTrackByTag
    {
        public String Tag
        {
            get;
            set;
        }
        public TopTrackByTag(String tag)
        {
            Tag = tag;
        }
        public async Task get(FrameworkElement frameElement , GridView contentHolder , int no = 50)
        {

            String url = "http://ws.audioscrobbler.com/2.0/?method=tag.gettoptracks&tag=" +
             Tag + 
             "&limit=" + 
             no + 
             "&api_key=30e44ae9c1e227a2f44f410e16e56586";

            String urlEncoded = Uri.EscapeUriString(url);

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(urlEncoded);

            System.Net.WebResponse response;
            try
            {
                response = await request.GetResponseAsync();
            }
            catch
            {
                throw new Exception("No internet connection or bad request! in getTopTrackByTag");
            }

            String resp = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
            

            XmlDocument fullXML = new XmlDocument();
            fullXML.LoadXml(resp);            
            XmlNodeList tracks = fullXML.GetElementsByTagName("track");


            for (int i = 0; i < tracks.Length; i++)
            {
                String xml = tracks[i].GetXml();
                XmlDocument new_xml = new XmlDocument();
                new_xml.LoadXml(xml);
                XmlNodeList names = new_xml.GetElementsByTagName("name");
                XmlNodeList duration = new_xml.GetElementsByTagName("duration");
                XmlNodeList music_url = new_xml.GetElementsByTagName("url");
                XmlNodeList images = new_xml.GetElementsByTagName("image");
                String trackName = names[0].InnerText;
                String artistName = names[1].InnerText;
                String musicLink = music_url[0].InnerText;

                Uri imageUri = new Uri("ms-appx:///Assets/blue.png");
                bool gotImageUri = false;
                String videoID = "NONE";
                Int32 durationNumber = 0;
                YoutubeStats stats = null;

                LastFMPageScrapper scpr = new LastFMPageScrapper(new Uri(musicLink));
                videoID = await scpr.getYoutubeId();

                if (videoID == "" || videoID == "s=") continue;

                stats = new YoutubeStats(videoID);

                var length = Convert.ToInt32(images.Length) - 1;
                if (length > 0)
                {
                    imageUri = new Uri(images[Convert.ToInt32(images.Length - 1)].InnerText);
                    gotImageUri = true;
                }

                durationNumber = Math.Max(stats.DurationInSeconds, Convert.ToInt32(duration[0].InnerText));

                try
                {
                    if (!gotImageUri)
                    {
                        await stats.getData();
                        imageUri = new Uri(stats.VideoImageURL);
                    }
                }
                catch (Exception er)
                {
                    new MessageDialog("A");
                }

                videoID = stats.VideoID;
                frameElement.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    Track new_track = new Track(artistName, trackName, musicLink, durationNumber, imageUri, videoID);
                    contentHolder.Items.Add(new_track);
                    GlobalArray.list.Add(new_track);
                });
            }

        }
    }
}
