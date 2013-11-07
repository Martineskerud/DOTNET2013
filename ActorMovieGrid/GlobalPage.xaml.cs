using System;
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
using Windows.ApplicationModel.Search;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ActorMovieGrid
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GlobalPage : Page
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalPage"/> class.
        /// </summary>
        public GlobalPage()
        {
            this.InitializeComponent();
        }






/// <summary>
/// Hides the appBars.
/// </summary>
        private void HideAppBars()
        {
          BottomAppBar.IsOpen = false;
          TopAppBar.IsOpen = false;    
        }
                /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="navigationEvent">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        /// 

        protected override void OnNavigatedTo(NavigationEventArgs navigationEvent)
        {
            if (navigationEvent == null)
                throw new ArgumentNullException("navigationEvent", "+ cannot be null");


            if (navigationEvent.Parameter.ToString().Split(":".ToCharArray()).Length > 1)
            {

                LocalFrame.Navigate(typeof(GroupedItemsPage), "AllGroups"); //To have the correct Back-stack
                LocalFrame.Navigate(typeof(GroupDetailPage), navigationEvent.Parameter.ToString().Split(":".ToCharArray())[1]);
            }
            else
            {
                LocalFrame.Navigate(typeof(GroupedItemsPage), navigationEvent.Parameter);
                HideAppBars();
            }
           
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            LocalFrame.Navigate(typeof(HelpPage), "AllGroups");
            HideAppBars();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            LocalFrame.Navigate(typeof(AddActor), "AllGroups");
            HideAppBars();
        }

        private void MoviesButton_Click(object sender, RoutedEventArgs e)
        {
            LocalFrame.Navigate(typeof(GroupedItemsPage), "AllGroups");
            HideAppBars();
        }




    }
}
