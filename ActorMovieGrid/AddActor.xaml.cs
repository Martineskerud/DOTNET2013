using ActorMovieGrid.Data;
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
using System.Data.Services.Client;
using s = ActorMovieGrid.ActorMovieServiceReference;
using Windows.UI.Popups;
using System.Diagnostics;
using System.Text.RegularExpressions;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ActorMovieGrid
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class AddActor : ActorMovieGrid.Common.LayoutAwarePage
    {

        private s.martinbeEntities data;
        //cant comply to CA1823: the field "movies" is used!
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private DataServiceCollection<s.Movie> movies;
        private DataServiceCollection<s.Actor> actors = null;
        private string selectedSex { get; set; }
        public AddActor()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)" /> when this page was initially requested.</param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var serviceRoot = new Uri(ServiceConstants.ServiceRootUrl);
            data = new s.martinbeEntities(serviceRoot);
            movies = new DataServiceCollection<s.Movie>();
            actors = new DataServiceCollection<s.Actor>();
        }



        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }


        /// <summary>
        /// Handles the 1 event of the SubmitButton_Click control. Prompts a message dialog.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// 
        //Example of lambda expression.
        private async void SubmitButton_Click_1(object sender, RoutedEventArgs e)
        {
            var messageDialog = new Windows.UI.Popups.MessageDialog("Are you sure you wish to proceed?", "Confirm your choice");

            messageDialog.Commands.Add(new UICommand("Yes", (command) =>
            {

                ValidateAndAddActorToDB();
            }));


            messageDialog.Commands.Add(new UICommand("No", (command) =>
            {
                output.Text = "Enter information again";

                ResetTextFields();

            }));

            messageDialog.DefaultCommandIndex = 1;
            await messageDialog.ShowAsync();
        }




        /// <summary>
        /// Validate input the and add actor to DB.
        /// </summary>
        public void ValidateAndAddActorToDB()
        {


            string firstName = InputText1.Text;
            string lastName = InputText2.Text;
            string description = InputText3.Text;
            //validating input, description needs more special characters than first- and last name. 
            //string descriptionCleaned, firstNameCleaned, lastNameCleaned;
            try
            {
                //3000 and 50 are the character limits for fields in the DB.
                CleanString(description, 3000);
                CleanStringStrict(firstName, 50);
                CleanStringStrict(lastName, 50);
                output.Text = "Thank you for your entry";
                AddActorToDatabase(new s.Actor { Firstname = firstName, Lastname = lastName, About = description, Image = selectedSex });

            }
            catch (InvalidActorArgumentException exception)
            {
                output.Text = exception.Message;
            }


        }







        /// <summary>
        /// Sets the input text fields as blank.
        /// </summary>
        private void ResetTextFields()
        {

            InputText1.Text = "";

            InputText2.Text = "";

            InputText3.Text = "";

            Male.IsChecked = false;

            Female.IsChecked = false;
        }

        /// <summary>
        /// Adds the actor to database.
        /// </summary>
        /// <param name="actor">The actor.</param>
        private void AddActorToDatabase(s.Actor actor)
        {
            data.AddObject("Actor", actor);
            try
            {
                data.BeginSaveChanges(SaveChangesOptions.Batch, new AsyncCallback(actor_OnDatabaseSaved), data);
            }
            catch (DataServiceRequestException ex)
            {
                Debug.WriteLine(ex.Message + " :: " + ex.InnerException.Message);
            }
        }
        //Example of lambda expression
        private async void actor_OnDatabaseSaved(IAsyncResult ar)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () =>
            {
                data = ar.AsyncState as s.martinbeEntities;
                try
                {
                    data.EndSaveChanges(ar);

                    actors.LoadAsync((DataServiceQuery<s.Actor>)data.Actor.Expand("Actor").OrderBy(m => m.Title));
                }
                catch (DataServiceRequestException ex)
                {
                    Debug.WriteLine(ex.Message + " :: " + ex.InnerException.Message);
                }
            }
            );
        }

        /// <summary>
        /// Sets the string as empty if it's not letters only. maxChars is the amount of letters allowed in the databasecolumn.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="maxChars">The max chars.</param>
        /// <returns></returns>
        public static string CleanStringStrict(string text, int maxChars)
        {

            if (text == null)
                throw new ArgumentNullException("text", "cannot be null");



            Match match = Regex.Match(text, @"^[a-zA-Z]+$", RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new InvalidActorArgumentException("text contains invalid characters");

            //exceeding the varchar(size) of the database column sets the string empty to avoid crashing.
            int textLength = text.Length;

            if (textLength > maxChars)
                throw new InvalidActorArgumentException("text was too long");


            return text;
        }


        /// <summary>
        /// Returns a nice string with no semi-colon. To prevent SQL-injection. Throws InvalidActorArgumentException if input is invalid.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string CleanString(string text, int maxChars)
        {
            if (text == null)
                throw new ArgumentNullException(text + " cannot be null");

            int textLength = text.Length;

            if (textLength > maxChars)
                throw new InvalidActorArgumentException("text" + " was too long");


            return text.Replace(";", "");

        }

        /// <summary>
        /// Handles the Checked event of the Female control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Female_Checked(object sender, RoutedEventArgs e)
        {
            selectedSex = @"\Assets\ActorImages\female.png";
        }

        /// <summary>
        /// Handles the Checked event of the Male control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Male_Checked(object sender, RoutedEventArgs e)
        {
            selectedSex = @"\Assets\ActorImages\male.png";
        }

    }
}
