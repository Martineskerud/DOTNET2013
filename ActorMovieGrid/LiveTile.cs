using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s = ActorMovieGrid.ActorMovieServiceReference;
using System.Data.Services.Client;
using ActorMovieGrid.Data;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;


namespace ActorMovieGrid
{
    public static class LiveTile
    {
        private static s.martinbeEntities data;
        private static DataServiceCollection<s.Movie> movies;
        private static ActorMovieDataSource dataSource;
        private static string[] movieArray;
        private static XmlDocument xmlDocument;
        private static  XmlNodeList movieTileText;
        private static string movie;



        /// <summary>
        /// Updates this instance.
        /// </summary>
        public static void Update()
        {

            dataSource = new ActorMovieDataSource(); //Live tile lives in its own data world. To not interfere with GroupedItemsPage LoadAsync and add data twice.
            var serviceRoot = new Uri(ServiceConstants.ServiceRootUrl);
            data = new s.martinbeEntities(serviceRoot);
            //Example of lambda expression
            var query = (DataServiceQuery<s.Movie>)data.Movie.Expand("Actor").OrderBy(c => c.Title);
            movies = new DataServiceCollection<s.Movie>();

            //Use of delegate 
            movies.LoadCompleted += movies_LoadCompleted;
            movies.LoadAsync(query);

        }

        /// <summary>
        /// Handles the LoadCompleted event of the movies control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LoadCompletedEventArgs"/> instance containing the event data.</param>
        /// 
        //this method also exists in GroupedItemsPage, this was easier (time constraints) than creating an GroupedItemsPage object and calling it through it to avoid duplicate code.
        private static void movies_LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            foreach (s.Movie c in movies)
            {
                MovieDataGroup movie = new MovieDataGroup("" + c.MovieId, c.Title, string.Empty, c.PosterImage, c.MovieDescription);

                foreach (s.Actor s in c.Actor)
                {
                    movie.Items.Add(new ActorDataItem(c.MovieId.ToString() + s.ActorId.ToString(), s.Firstname + " " + s.Lastname, s.Title, s.Image, string.Empty, s.About, movie));
                }
                dataSource.AllGroups.Add(movie);

            }

            AddDataToArray();
        }


        /// <summary>
        /// Adds Movie data titles to the array.
        /// </summary>
        public static void AddDataToArray()
        {

            int arraySize = 0;
            foreach (MovieDataGroup m in dataSource.AllGroups)
            {
                arraySize += m.Items.Count;
            }

            movieArray = new string[arraySize];

            int i = 0;

            foreach (MovieDataGroup n in dataSource.AllGroups)
            {
                movieArray[i] = n.Title;
                i++;
            }

            UpdateLiveTile();


        }


        //Code metrics: 59, commented in readme.
        /// <summary>
        /// Updates the live tile.
        /// </summary>
        private static void UpdateLiveTile()
        {
            TileUpdater tileUpdater = SetupUpdater();


          
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    movie = movieArray[i];

                    movieTileText = xmlDocument.GetElementsByTagName("text");

                    movieTileText[0].InnerText = movie;

                    TileNotification tileNotification = new TileNotification(xmlDocument);

                    tileNotification.Tag = "tile" + i;

                    tileNotification.ExpirationTime = DateTime.Now.AddSeconds(10 * (i + 1));
                    tileUpdater.Update(tileNotification);
                }
                catch(ArgumentOutOfRangeException exception)
                {
                    movie = exception.Message;
                }
            }

        }

        /// <summary>
        /// Setup for the TileUpdater
        /// </summary>
        /// <returns></returns>
        private static TileUpdater SetupUpdater()
        {
                TileUpdater tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
                tileUpdater.EnableNotificationQueue(true);
                xmlDocument = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText09);
                return tileUpdater;
        }





    }

}
