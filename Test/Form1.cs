using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Threading;

// System.IO.Directory.GetCurrentDirectory()


namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void testButton_Click(object sender, EventArgs e)
        {

            Jack cunt = new Jack("the simpsons", 2, 15,35);
            cunt.StartTheThreadening();


            //InputText.Text = cunt.GenerateLink(19, 10);
            //cunt.PopulateSeasons();
            //cunt.CollectLinksPerSeason(2);
            //cunt.SaveSeason(2);
            //cunt.SplitEvenly();


            //MessageBox.Show(html.DocumentNode.SelectSingleNode("//head/title").InnerText);


        }
    }

    public class Jack
    {
        string baseURL = "http://the-watch-series.to/episode/";
        string SeriesName = "";
        int FromSeason = 0;
        int ToSeason = 0;
        int EpisodesPerSeason = 0;
        List<Season> Seasons;
        string basePath = Directory.GetCurrentDirectory() + "\\Downloads\\";

        public Jack(string SeriesName, int FromSeason, int ToSeason, int EpisodesPerSeason)
        {
            this.SeriesName = SeriesName.Replace(" ", "_");
            this.FromSeason = FromSeason;
            this.ToSeason = ToSeason;
            this.EpisodesPerSeason = EpisodesPerSeason;
            //MessageBox.Show(this.SeriesName);
            this.Seasons = new List<Season>(ToSeason - FromSeason + 1);
            this.PopulateSeasons();
            Directory.CreateDirectory(basePath);
        
        }

        public string GenerateLink(int SeasonNr, int EpisodeNr)
        {
            return this.baseURL + this.SeriesName + "_s" + SeasonNr.ToString() + "_e" + EpisodeNr.ToString() + ".html";
        }

        public void PopulateSeasons()
        {
            for (int i = 0; i <= this.ToSeason; i++)
            {
                Seasons.Add(new Season(i, this.EpisodesPerSeason));
                var tempSeason = new Season(i, this.EpisodesPerSeason);
                for (int k = 1; k <= this.EpisodesPerSeason; k++)
                {
                    var tempEpisode = new Episode(this.GenerateLink(i, k), this.SeriesName, "add this later faggot", i, k);
                    tempSeason.AddEpisode(tempEpisode);
                }
                if (i >= this.FromSeason)
                {
                    Seasons[i] = tempSeason;
                }
                
            }
        }

        public void CollectLinksPerSeason(int SeasonNr)
        {
            var currentEpisodes = Seasons[SeasonNr].GetEpisode();
            foreach (var epi in currentEpisodes)
            {
                var html = new HtmlAgilityPack.HtmlDocument();

                try
                {
                    html.LoadHtml(new WebClient().DownloadString((epi.GetRawURL())));
                }
                catch (System.Exception ex)
                {
                    return;
                }

                var testNode = html.DocumentNode.Descendants().Where(n => n.GetAttributeValue("class", "").Contains("download_link_"))
                    .Where(k => k.InnerHtml.Contains("/cale.html?r="));

                foreach (var node in testNode)
                {
                    var aTag = node.Descendants("a").Where(j => j.GetAttributeValue("class", "").Equals("buttonlink")).First();
                    string tempLink = aTag.GetAttributeValue("href", "").Substring(13);

                    byte[] data = Convert.FromBase64String(tempLink);
                    string decodedString = Encoding.UTF8.GetString(data);

                    try
                    {
                        epi.AddRealURL(aTag.GetAttributeValue("title", ""), decodedString);
                    }
                    catch (ArgumentException)
                    {

                    }
                }

            }
        }

        public void SaveSeason(int SeasonNr)
        {
            string slightlymoreadvancedpath = basePath + this.SeriesName + "\\" + "Season " + SeasonNr.ToString() + "\\";

            //Seasons[SeasonNr].trimEmptyEpisodes();
            var tempEpisodes = Seasons[SeasonNr].GetEpisode();
            foreach (var epi in tempEpisodes)
            {
                if(epi.GetUrls().Count == 0) { continue; }
                Directory.CreateDirectory(slightlymoreadvancedpath);
                string fullPath = slightlymoreadvancedpath + epi.GetEpisodeName() + " (" + epi.GetEpisodeNr().ToString() + ").txt";
                string writeString = JsonConvert.SerializeObject(epi.GetUrls());
                File.WriteAllText(fullPath, writeString);
            }
        }

        // after this comment lies deeply burried the disasters of attempted mutlithreading by me. If you dont wish your sould eternal suffering, do not venture further


        List<List<int>> evenSplit = new List<List<int>> {new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>(), new List<int>()};

        public void SplitEvenly()
        {
            int positionInList = 0;
            foreach (var season in this.Seasons)
            {
                evenSplit[positionInList].Add(season.GetSeasonNr());
                positionInList++;
                if (positionInList == 6) { positionInList = 0; }
            }
        }

        public void StartTheThreadening()
        {
            this.SplitEvenly();

            Thread thread1 = new Thread(new ThreadStart(MTFunc1));
            Thread thread2 = new Thread(new ThreadStart(MTFunc2));
            Thread thread3 = new Thread(new ThreadStart(MTFunc3));
            Thread thread4 = new Thread(new ThreadStart(MTFunc4));
            Thread thread5 = new Thread(new ThreadStart(MTFunc5));
            Thread thread6 = new Thread(new ThreadStart(MTFunc6));

            thread1.Start();

            thread2.Start();

            thread3.Start();

            thread4.Start();

            thread5.Start();

            thread6.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();
            thread5.Join();
            thread6.Join();

        }


        private void MTFunc1()
        {
            foreach (int i in evenSplit[0])
            {
                Thread.Sleep(10);

                this.CollectLinksPerSeason(i);
                this.SaveSeason(i);
            }
        }

        private void MTFunc2()
        {
            foreach (int i in evenSplit[1])
            {
                Thread.Sleep(10);

                this.CollectLinksPerSeason(i);
                this.SaveSeason(i);
            }
        }

        private void MTFunc3()
        {
            foreach (int i in evenSplit[2])
            {
                Thread.Sleep(10);

                this.CollectLinksPerSeason(i);
                this.SaveSeason(i);
            }
        }

        private void MTFunc4()
        {
            foreach (int i in evenSplit[3])
            {
                Thread.Sleep(10);

                this.CollectLinksPerSeason(i);
                this.SaveSeason(i);
            }
        }

        private void MTFunc5()
        {
            foreach (int i in evenSplit[4])
            {
                Thread.Sleep(10);

                this.CollectLinksPerSeason(i);
                this.SaveSeason(i);
            }
        }

        private void MTFunc6()
        {
            foreach (int i in evenSplit[5])
            {
                Thread.Sleep(10);

                this.CollectLinksPerSeason(i);
                this.SaveSeason(i);
            }
        }


    }

    public class Season
    {
        int SeasonNr = 0;
        List<Episode> Episodes;

        public Season(int SeasonNr,int EpisodesPerSeason)
        {
            this.SeasonNr = SeasonNr;
            this.Episodes = new List<Episode>(EpisodesPerSeason);
        }

        public void AddEpisode(Episode newEpisode)
        {
            Episodes.Add(newEpisode);
        }

        public List<Episode> GetEpisode()
        {
            return Episodes;
        }

        public int GetSeasonNr()
        {
            return SeasonNr;
        }

        public void trimEmptyEpisodes()
        {
            for (int i = Episodes.Count; i >= 1; i--)
            {
                if(Episodes[i-1].GetUrls().Count == 0)
                {
                    Episodes.RemoveAt(i-1);
                }
            }
        }

    }
    
    public class Episode
    {
        string rawURL;
        Dictionary<string, string> URLs;
        string SeriesName;
        string EpisodeName;
        int Season;
        int EpisodeNumber;

        public Episode(string URL, string SeriesName, string EpisodeName, int Season, int EpisodeNumber)
        {
            this.rawURL = URL;
            this.SeriesName = SeriesName;
            this.EpisodeName = EpisodeName;
            this.Season = Season;
            this.EpisodeName = EpisodeName;
            this.EpisodeNumber = EpisodeNumber;
            this.URLs = new Dictionary<string, string>();
        }

        public string GetRawURL()
        {
            return rawURL;
        }

        public void AddRealURL(string Hoster, string URL)
        {
            this.URLs.Add(Hoster, URL);
        }

        public Dictionary<string,string> GetUrls()
        {
            return URLs;
        }

        public int GetEpisodeNr()
        {
            return this.EpisodeNumber;
        }

        public string GetEpisodeName()
        {
            return this.EpisodeName;
        }
    }
}
