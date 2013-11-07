using ActorMovieGrid.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Data.Services.Client;
using s = ActorMovieGrid.ActorMovieServiceReference;
using Windows.ApplicationModel.Search;
using System.Diagnostics;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace ActorMovieGrid
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedItemsPage : ActorMovieGrid.Common.LayoutAwarePage
    {
        private ActorMovieDataSource dataSource;
        private s.martinbeEntities data;
        private DataServiceCollection<s.Movie> movies;

        public GroupedItemsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {

            dataSource = (ActorMovieDataSource)App.Current.Resources["ActorMovieDataSource"];
            if (dataSource.AllGroups.Count > 0)
            {
                this.DefaultViewModel["Groups"] = dataSource.AllGroups;
            }
            else
            {
                var sampleDataGroups = dataSource.GetGroups((String)navigationParameter);
                this.DefaultViewModel["Groups"] = sampleDataGroups;
                SetupQuery();
            }
        }


        /// <summary>
        /// Sets up query and handles it.
        /// </summary>
        public void SetupQuery()
        {

            var serviceRoot = new Uri(ServiceConstants.ServiceRootUrl);
            data = new s.martinbeEntities(serviceRoot);
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
        private void movies_LoadCompleted(object sender, LoadCompletedEventArgs e)
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
            this.DefaultViewModel["Groups"] = dataSource.AllGroups;
        }
        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="navigationEvent">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            this.Frame.Navigate(typeof(GroupDetailPage), ((MovieDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="navigationEvent">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((ActorDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
        }

    }
}
