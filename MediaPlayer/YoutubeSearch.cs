﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace MediaPlayer
{
    class YoutubeSearch
    {
        public String TrackName
        {
            get;set;
        }
        public String ArtistName
        {
            get;set;
        }
        private static readonly Regex youtubeVideoRegex = new Regex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
        private HttpClient mClient;
        private HttpResponseMessage mResponse;
        private YoutubeDecoder mDecoder;
        private bool mIsRunning;


        public YoutubeSearch(String trackName, String artistName)
        {
            TrackName = trackName;
            ArtistName = artistName;
            mClient = new HttpClient();
            mDecoder = new YoutubeDecoder();
            mIsRunning = false;
        }

        public YoutubeSearch()
        {
            mClient = new HttpClient();
            mDecoder = new YoutubeDecoder();
            mIsRunning = false;
            TrackName = "";
            ArtistName = "";
        }
        
        public void cancel()
        {
            mIsRunning = false;
            mClient.CancelPendingRequests();
        }
        public async Task<Pair<string,string>> getAVideoCacheUri()
        {
            mClient.CancelPendingRequests();
            // example https://gdata.youtube.com/feeds/api/videos?q=Lady+Gaga+Alejandro&orderby=relevance
            mIsRunning = true;
            string search_url = "https://gdata.youtube.com/feeds/api/videos?q=" + ArtistName + " " + TrackName + "&orderby=relevance";        
            string contents;
            try
            {
                mResponse = await mClient.GetAsync(search_url);
                contents = await mResponse.Content.ReadAsStringAsync();
            }
            catch (Exception error)
            {
                mIsRunning = false;
                throw new Exception(ExceptionMessages.CONNECTION_FAILED);
            }
            
            string string_to_search = "media:player url=";
            string youtubeVideo = "";
            string videoId = "";

            for (int index = 0; index <= contents.Length && mIsRunning; index += string_to_search.Length)
            {
                index = contents.IndexOf(string_to_search, index);
                if (index == -1) return new Pair<string,string>("http://127.0.0.1","NONE");
                int copy = index;
                copy += string_to_search.Length + 1;
                youtubeVideo = "";
                while (contents[copy] != "'"[0])
                {
                    youtubeVideo += contents[copy];
                    copy++;
                }
                Match youtubeMatch = youtubeVideoRegex.Match(youtubeVideo);
                if (youtubeMatch.Success)
                {
                    videoId = youtubeMatch.Groups[1].Value;
                    mDecoder.VideoID = videoId;
                    string directVideoURL = "";
                    try
                    {
                        directVideoURL = await mDecoder.fetchURL();
                    }
                    catch (Exception)
                    {
                        directVideoURL = "";
                    }
                    if (directVideoURL.Contains("&signature="))
                    {
                        mIsRunning = false;
                        return new Pair<string,string>(directVideoURL,mDecoder.VideoID);
                    }
                }
            }
            mIsRunning = false;
            return new Pair<string, string>("http://127.0.0.1", "NONE");
        }
    }
}
