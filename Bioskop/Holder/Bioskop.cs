using System;
using System.Collections.Generic;

namespace Bioskop.Holder
{
    public class Movie
    {
        public String Nama { get; set; }
        public String Poster { get; set; }
        public String Genre { get; set; }
        public String Duration { get; set; }
        public List<Jadwal> Jadwal { get; set; }
    }

    public class Jadwal
    {
        public String Bioskop { get; set; }
        public List<DateTime> Date { get; set; }
        public String Harga { get; set; }
    }

    public class LocationBioskop
    {
        public String Id { get; set; }
        public String Nama { get; set; }
    }

}
