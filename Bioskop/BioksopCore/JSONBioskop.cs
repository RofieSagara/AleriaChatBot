using Bioskop.Holder;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace Bioskop.BioksopCore
{
    public class JSONBioskop
    {
        public String NameBioskop { get; set; }
        public String TitleMovie { get; set; }
        public String Location { get; set; }
        private List<LocationBioskop> LocBis { get; set; }

        public JSONBioskop()
        {
            LocBis = GetDataLocation();
        }

        private List<LocationBioskop> GetDataLocation()
        {
            List<LocationBioskop> data = new List<LocationBioskop>();
            WebClient wc = new WebClient();
            String response = wc.DownloadString(@"http://ibacor.com/api/jadwal-bioskop");
            JObject responseJson = JObject.Parse(response);
            String status = responseJson.SelectToken("$.status").ToString();

            JToken jsonT = responseJson.SelectToken("$.data");

            JArray jsonArray = JArray.FromObject(jsonT);
            foreach(Object d in jsonArray)
            {
                JObject da = (JObject)d;
                LocationBioskop lb = new LocationBioskop();
                lb.Id = da.SelectToken("$.id").ToString();
                lb.Nama = da.SelectToken("$.kota").ToString();
                data.Add(lb);
            }

            return data;
        }

        private String GetLocationName(int Location)
        {
            foreach(LocationBioskop lc in LocBis)
            {
                if (lc.Id.Equals(Location.ToString()))
                {
                    return lc.Nama;
                }
            }

            return "";
        }
        public List<Movie> CheckMovieToday(int location)
        {
            Location = GetLocationName(location);
            WebClient wc = new WebClient();
            String response = wc.DownloadString(@"http://ibacor.com/api/jadwal-bioskop?id=" + location);

            JObject responseJson = JObject.Parse(response);

            JToken jsonTokenData = responseJson.SelectToken("$.data");

            JArray jsonArray = JArray.FromObject(jsonTokenData);
            List<Movie> dataMovie = new List<Movie>();
            foreach (Object data in jsonArray)
            {
                JObject da = (JObject)data;
                Movie m = new Movie();
                m.Nama = da.SelectToken("$.movie").ToString();
                m.Poster = da.SelectToken("$.poster").ToString();
                m.Genre = da.SelectToken("$.genre").ToString();
                m.Duration = da.SelectToken("$.duration").ToString();
                JArray jsonArrayJadwal = JArray.FromObject(da.SelectToken("$.jadwal"));
                List<Jadwal> dataJadwal = new List<Jadwal>();

                foreach (Object datajadwalObject in jsonArrayJadwal)
                {
                    JObject djo = (JObject)datajadwalObject;
                    Jadwal jd = new Jadwal();
                    jd.Bioskop = djo.SelectToken("$.bioskop").ToString();

                    String tempDate = djo.SelectToken("$.jam").ToString();
                    tempDate = tempDate.Replace("[\r\n  \"", "");
                    tempDate = tempDate.Replace("\"\r\n]", "");
                    tempDate = tempDate.Replace("\r\n", "");
                    tempDate = tempDate.Replace("\"", "");
                    String[] date = tempDate.Split(',');
                    List<DateTime> dataDate = new List<DateTime>();
                    foreach (String a in date)
                    {
                        DateTime dateNow = DateTime.Now;
                        String temp = dateNow.ToShortDateString();
                        temp = temp + " " + a;
                        dateNow = DateTime.Parse(temp);
                        dataDate.Add(dateNow);
                    }
                    jd.Harga = djo.SelectToken("$.harga").ToString();
                    jd.Date = dataDate;
                    dataJadwal.Add(jd);
                }
                m.Jadwal = dataJadwal;
                dataMovie.Add(m);
            }
            return dataMovie;
        }

        public Movie CheckMovieTodayFromTitle(int location, String title)
        {
            Location = GetLocationName(location);
            List<Movie> dataMovie = CheckMovieToday(location);
            Movie dataResult = new Movie();
            foreach (Movie m in dataMovie)
            {
                if (m.Nama.ToLower().Equals(title.ToLower()))
                {
                    dataResult = m;
                    break;
                }
            }

            return dataResult;
        }

        public List<Movie> CheckMovieTodayFromBioskop(int location, String nameBioskop)
        {
            Location = GetLocationName(location);
            List<Movie> dataMovie = CheckMovieToday(location);
            List<Movie> dataResult = new List<Movie>();
            foreach (Movie m in dataMovie)
            {
                foreach (Jadwal j in m.Jadwal)
                {
                    if (j.Bioskop.Equals(nameBioskop))
                    {
                        Movie mv = new Movie();
                        mv.Jadwal.Add(j);
                        mv.Duration = m.Duration;
                        mv.Genre = m.Genre;
                        mv.Nama = m.Nama;
                        mv.Poster = mv.Poster;
                        dataResult.Add(mv);
                    }
                }
            }

            return dataResult;
        }

        public Movie CheckMovieTodayFromTitleandBioskop(int location, String nameBioskop, String title)
        {
            Location = GetLocationName(location);
            List<Movie> dataMovie = CheckMovieTodayFromBioskop(location, nameBioskop);
            Movie dataResult = new Movie();
            foreach (Movie m in dataMovie)
            {
                if (m.Nama.ToLower().Equals(title.ToLower()))
                {
                    dataResult = m;
                    break;
                }
            }

            return dataResult;
        }
    }
}
