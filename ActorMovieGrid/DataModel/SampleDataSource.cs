using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using s = ActorMovieGrid.ActorMovieServiceReference;
using System.Data.Services.Client;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace ActorMovieGrid.Data
{
    /// <summary>
    /// Base class for <see cref="ActorDataItem"/> and <see cref="MovieDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    //abstract was default, removed to comply to codemetrics. Keeping this comment in there incase it needs to be changed and for easier debugging. 
    public /*abstract */ class ActorMovieDataCommon : ActorMovieGrid.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "description"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "subtitle")]
        //Keeping the constructor as is for now, if it's not used by April 30th, it'll be removed.
        public ActorMovieDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._imagePath = imagePath;
            this._description = description;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(ActorMovieDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class ActorDataItem : ActorMovieDataCommon
    {
        public ActorDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, MovieDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
            this._description = description;
        }

        private string _description = string.Empty;
        //Casing is consistant with template code, only complains because it was refactored. 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "description")]
        public string description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }


        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private MovieDataGroup _group;
        public MovieDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }



    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class MovieDataGroup : ActorMovieDataCommon
    {
        public MovieDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }
        //use of generics
        private ObservableCollection<ActorDataItem> _items = new ObservableCollection<ActorDataItem>();
        public ObservableCollection<ActorDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<ActorDataItem> _topItem = new ObservableCollection<ActorDataItem>();
        public ObservableCollection<ActorDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// ActorMovieDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class ActorMovieDataSource
    {

        private ObservableCollection<MovieDataGroup> _allGroups = new ObservableCollection<MovieDataGroup>();
        public ObservableCollection<MovieDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public IEnumerable<MovieDataGroup> GetGroups(string uniqueId)
        {
            if (uniqueId == null)
            {
                throw new ArgumentNullException("uniqueId");
            }

            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return this.AllGroups;
        }

        public MovieDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = this.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public ActorDataItem GetItem(string uniqueId)
        {

            // Simple linear search is acceptable for small data sets
            var matches = this.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public ActorMovieDataSource()
        {
            _allGroups= new ObservableCollection<MovieDataGroup>();
   
        }
        /// <summary>
        /// Gets the searched item by first or last name. NYI: no time.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <returns></returns>
        /// 
        public static ObservableCollection<ActorDataItem> SearchActorByLastName(/*String searchQuery*/)
        {
            throw new NotImplementedException("Method has not been implemented");
        }


        /// <summary>
        /// Searches the movies by title.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public ObservableCollection<MovieDataGroup> SearchMoviesByTitle(string query)
        {
            ObservableCollection<MovieDataGroup> result = new ObservableCollection<MovieDataGroup>();
            foreach(MovieDataGroup mdg in this.AllGroups.Where(m => m.Title.Contains(query)))
            {
                result.Add(mdg);
            }
            
            return result;
        }
        



        }
    }
