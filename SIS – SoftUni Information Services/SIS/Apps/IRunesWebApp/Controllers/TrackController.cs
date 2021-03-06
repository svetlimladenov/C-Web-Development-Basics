﻿using System;
using System.Linq;
using System.Net;
using IRunesWebApp.Models;
using IRunesWebApp.ViewModels.Tracks;
using SIS.Http.Responses.Contracts;
using SIS.MvcFramework;

namespace IRunesWebApp.Controller
{
    public class TrackController : BaseController
    {
        [HttpGet("/Albums/Tracks/Create")]
        public IHttpResponse CreateTrack(CreateTrackViewModel model)
        {
            //var albumId = this.Request.QueryData["albumId"].ToString();
            var albumId = model.AlbumId;

            var viewModel = new CreateTrackViewModel()
            {
                AlbumId = albumId,
            };

            return this.View("CreateTrack", viewModel);
        }

        [HttpPost("/Albums/Tracks/Create")]
        public IHttpResponse AddTrack(AddTrackInputModel model)
        {
            var trackName = model.TrackName;
            var trackLink = model.TrackLink.Trim();

            var trackPriceString = model.TrackPrice;
            var albumId = model.AlbumId;

            if (string.IsNullOrWhiteSpace(trackName) || string.IsNullOrWhiteSpace(trackLink) || string.IsNullOrWhiteSpace(trackPriceString))
            {
                return BadRequestError("Fill in all the blanks.");
            }

            if (!Uri.IsWellFormedUriString(trackLink, UriKind.RelativeOrAbsolute))
            {
                return BadRequestError("Invalid URL.");
            }

            var trackPrice = decimal.Parse(trackPriceString);

            var track = new Track
            {
                Name = trackName,
                Link = trackLink,
                Price = trackPrice,
            };

            bool trackExists = this.Db.Tracks.Any(x => x.Name == track.Name);

            string trackId;
            if (!trackExists)
            {
                this.Db.Tracks.Add(track);
                trackId = this.Db.Tracks.Local.First(x => x.Name == track.Name).Id;
            }
            else
            {
                trackId = this.Db.Tracks.First(x => x.Name == track.Name).Id;
            }

            var trackAlbum = new TrackAlbum()
            {
                AlbumId = this.Db.Albums.FirstOrDefault(a => a.Id == albumId)?.Id,
                TrackId = trackId
            };

            bool trackAlbumExist =
                this.Db.TracksAlbums.Any(x => x.AlbumId == trackAlbum.AlbumId && x.TrackId == trackAlbum.TrackId);

            if (!trackAlbumExist)
            {
                this.Db.Add(trackAlbum);
                this.Db.SaveChanges();
            }

            return Redirect($"/Albums/Details?id={albumId}");
        }

        [HttpGet("/Albums/Tracks/Details")]
        public IHttpResponse TrackInfo(TrackInfoViewModel model)
        {
            var albumId = model.AlbumId;
            var trackId = model.TrackId;
            var track = this.Db.Tracks.FirstOrDefault(x => x.Id == trackId);

            if (track != null && this.Db.Albums.Any(a => a.Id == albumId))
            {
                var youtubeLink = WebUtility.UrlDecode(track.Link);
                var trackName = track.Name;
                var price = track.Price;

                var viewModel = new TrackInfoViewModel()
                {
                    AlbumId = albumId,
                    TrackUrl = youtubeLink,
                    TrackName = trackName,
                    TrackPrice = price,
                };

                return this.View("TrackInfo", viewModel);
            }

            return BadRequestError("Track or Album unavaivable");

        }
    }
}
