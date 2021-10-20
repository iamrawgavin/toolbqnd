using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace SetlistLoader
{
    class Program
    {
        static List<Concert> concertList = new List<Concert>();
        static CultureInfo enUs = new CultureInfo("en-Us");
        static string artist;
        static string mbid;
        static string connectionString;
        static int numberOfLists = 13;
        static HashSet<DateTime> bestshows = new HashSet<DateTime>();

        static void Main(string[] args)
        {
            if (args.Length == 0) mbid = "66fc5bf8-daa4-4241-b378-9bc9077939d2";
            else mbid = args[0];

            Console.WriteLine("Welcome to Setlist Loader!");
            Console.WriteLine("Loading list of best shows");
            LoadHashSet();
            Console.WriteLine($"Loaded {bestshows.Count} best shows");
            Console.WriteLine("Pulling setlists from Setlist.fm API");
            GetConcerts(mbid);
            Console.WriteLine($"Pulled {concertList.Count} setlists");
            Console.WriteLine("Saving to the database");
            SaveConcerts();
            Console.WriteLine($"Saved records to database... done.");
        }

        static void LoadHashSet()
        {
            foreach (string line in File.ReadLines("bestshows.txt"))
            {
                var dt = DateTime.Parse(line);
                bestshows.Add(dt);
            }
        }

        static void SaveConcerts()
        {
            var listOfConcertLists = concertList.GetListOfLists(concertList.Count / numberOfLists);
            _ = Parallel.For(0, listOfConcertLists.Count,
                i =>
                {
                    SaveConcerts(listOfConcertLists[i]);
                });
        }

        static void SaveConcerts(List<Concert> concerts)
        {
            connectionString = ConfigurationManager.ConnectionStrings["PGDB"].ConnectionString;
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                int bandId;
                var command = $"select insert_band('{artist}','{mbid}')";
                using (NpgsqlCommand comm = new NpgsqlCommand(command, conn))
                {
                    comm.Parameters.Add(new NpgsqlParameter("bid", DbType.Int32) { Direction = ParameterDirection.Output });
                    comm.ExecuteNonQuery();
                    bandId = (int)comm.Parameters[0].Value;
                }

                foreach (var c in concerts)
                {
                    if (!bestshows.Contains(c.Date)) continue;
                    var v = c.Venue;
                    int venueId;
                    command = $"select insert_venue('{v.Name.SetSafeQuotes()}','{v.City}','{v.State}','{v.StateCode}','{v.Country}','{v.CountryCode}',{Math.Round(v.Latitude, 8)}, {Math.Round(v.Longitude, 8)})";
                    using (NpgsqlCommand comm = new NpgsqlCommand(command, conn))
                    {
                        comm.Parameters.Add(new NpgsqlParameter("vid", DbType.Int32) { Direction = ParameterDirection.Output });
                        comm.ExecuteNonQuery();
                        venueId = (int)comm.Parameters[0].Value;
                    }

                    int concertId;
                    command = $"select insert_concert({bandId}, '{c.Date.ToString("yyyy-MM-dd")}',{venueId})";
                    using (NpgsqlCommand comm = new NpgsqlCommand(command, conn))
                    {
                        comm.Parameters.Add(new NpgsqlParameter("cid", DbType.Int32) { Direction = ParameterDirection.Output });
                        comm.ExecuteNonQuery();
                        concertId = (int)comm.Parameters[0].Value;
                    }

                    int i = 1;
                    foreach (var song in c.Setlist)
                    {
                        command = $"select insert_song({concertId}, {i}, '{song.SetSafeQuotes()}')";
                        using (NpgsqlCommand comm = new NpgsqlCommand(command, conn))
                        {
                            comm.ExecuteNonQuery();
                        }
                        i++;
                    }
                }
            }
        }

        static void GetConcerts(string mbid)
       {
            var data = GetConcertsFromApi(mbid, 1);

            artist = data["setlist"][0]["artist"]["name"].ToString();
            LoadConcertsIntoList(data); //load first page of data

            var itemsPerPage = (int)data["itemsPerPage"];
            var total = (int)data["total"];
            var totalPages = (int)Math.Ceiling((decimal)total / (decimal)itemsPerPage);

            _ = Parallel.For(2, totalPages, //load the remaining pages 
                i =>
                {
                    data = GetConcertsFromApi(mbid, i);
                    LoadConcertsIntoList(data);
                });
        }

        static JObject GetConcertsFromApi(string musicBrainzId, int pageNumber)
        {
            var content = string.Empty;
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.setlist.fm/rest/1.0/artist/{musicBrainzId}/setlists?p={pageNumber}");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("x-api-key", File.ReadAllText("setlist-fm-key.txt").Trim());
                var response = client.SendAsync(request).Result;
                content = response.Content.ReadAsStringAsync().Result;
            }
            return JObject.Parse(content);
        }

        static void LoadConcertsIntoList(JObject json)
        {
            var concerts = json["setlist"] as JArray;
            for (var i = 0; i < concerts.Count; i++)
            {
                decimal lat, lng;
                DateTime eventDate;

                var v = new Venue();
                v.Name = concerts[i]["venue"]["name"]?.ToString() ?? "";
                v.City = concerts[i]["venue"]["city"]["name"]?.ToString() ?? "";
                v.State = concerts[i]["venue"]["city"]["state"]?.ToString() ?? "";
                v.StateCode = concerts[i]["venue"]["city"]["stateCode"]?.ToString() ?? "";
                v.Country = concerts[i]["venue"]["city"]["country"]["name"]?.ToString() ?? "";
                v.CountryCode = concerts[i]["venue"]["city"]["country"]["code"]?.ToString() ?? "";
                v.Latitude = Decimal.TryParse(concerts[i]["venue"]["city"]["coords"]["lat"]?.ToString(), out lat) ? lat : 0m;
                v.Longitude = Decimal.TryParse(concerts[i]["venue"]["city"]["coords"]["long"]?.ToString(), out lng) ? lng : 0m; ;

                var c = new Concert();
                c.Artist = artist;
                c.Date = DateTime.TryParseExact(concerts[i]["eventDate"]?.ToString(),"dd-MM-yyyy",enUs, DateTimeStyles.None, out eventDate) ? eventDate : DateTime.MinValue;
                c.Venue = v;

                c.Setlist = new List<string>();
                var sets = concerts[i]["sets"]["set"] as JArray;

                for (var j = 0; j < sets.Count; j++)
                {
                    var songs = sets[j]["song"] as JArray;
                    for (var k = 0; k < songs.Count; k++)
                    {
                        if (songs[k]["tape"] == null) c.Setlist.Add(songs[k]["name"].ToString());
                    }
                }
                concertList.Add(c);
            }
        }
    }
}
