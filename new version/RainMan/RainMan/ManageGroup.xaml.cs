using RainMan.Common;
using RainMan.DataModels;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using RainMan.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RainMan
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageGroup : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private PathGroup group = null;
        private IMobileServiceTable<PathGroup> groupsTable = App.mobileClient.GetTable<PathGroup>();
        private IMobileServiceTable<DataModels.Path> pathTable = App.mobileClient.GetTable<DataModels.Path>();

        public ManageGroup()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

            if(Frame.BackStack.ElementAt(Frame.BackStackDepth-1).GetType() == typeof(RouteBuilder))
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }


            this.appBarDeletePath.Visibility = Visibility.Collapsed;
            group = e.NavigationParameter as PathGroup;
            this.GeneralInformationGrid.Visibility = Visibility.Collapsed;
            this.defaultViewModel["Group"] = group;
            await refreshPaths();


        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void appBarDetails_Click(object sender, RoutedEventArgs e)
        {

        }

        private void appBarDetails_Checked(object sender, RoutedEventArgs e)
        {
            this.GeneralInformationGrid.Visibility = Visibility.Visible;
        }

        private void appBarDetails_Unchecked(object sender, RoutedEventArgs e)
        {
            this.GeneralInformationGrid.Visibility = Visibility.Collapsed;
        }

        private async void appBarDeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            ModalWindow win = new ModalWindow(String.Format("Removing {0} ...", this.selectedPath.PathName), "Please wait ... ", "");
            win.Dialog.ShowAsync();
            await this.pathTable.DeleteAsync(selectedPath);
            this.selectedPath = null;
            this.appBarDeletePath.Visibility = Visibility.Collapsed;
            await this.refreshPaths();
            win.Dialog.Hide();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if (this.groupName.Text == "" || this.startName.Text == "" || this.endName.Text == "")
            {
                MessageDialog diag = new MessageDialog("One of the fields is empty, please fill the information");
                diag.ShowAsync();
                return;
            }

            group.GroupName = this.groupName.Text;
            group.StartName = this.startName.Text;
            group.FinishName = this.endName.Text;

            await groupsTable.UpdateAsync(group);

            MessageDialog dialog = new MessageDialog("Changes were succesfully saved!");
            await dialog.ShowAsync();
            this.GeneralInformationGrid.Visibility = Visibility.Collapsed;
            this.appBarDetails.IsChecked = false;

        }


        private async Task refreshPaths()
        {

            var paths = await pathTable.Where(item => item.UserId == App.userId).ToCollectionAsync<DataModels.Path>();
            var groupPaths = paths.Where(item => item.groupId == group.Id).ToList();

            // set my paths
            this.defaultViewModel["Paths"] = groupPaths;

        }

        private void addPathButton_Click(object sender, RoutedEventArgs e)
        {

            this.appBarDetails.IsChecked = false;
            this.GeneralInformationGrid.Visibility = Visibility.Collapsed;

            Frame.Navigate(typeof(RouteBuilder), this.group);

        }

        private void pathList_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private DataModels.Path selectedPath = null;

        private void pathList_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.selectedPath = e.ClickedItem as DataModels.Path;
            this.appBarDeletePath.Visibility = Visibility.Visible;
        }

        private async void DeleteGroup_Click(object sender, RoutedEventArgs e)
        {
                ModalWindow modalWindow = new ModalWindow("Group is being deleted", "Please wait ... ", "");
                modalWindow.Dialog.ShowAsync();

                await this.groupsTable.DeleteAsync(this.group);
                //this.defaultViewModel["PathGroups"] = await this.groupsTable.ToCollectionAsync<PathGroup>();

               // this.selectedGroup = null;
               // this.appBarRemoveGroup.Visibility = Visibility.Collapsed;
               // this.selectGroup.Visibility = Visibility.Collapsed;

                modalWindow.Dialog.Hide();

                Frame.GoBack();
        }
    }
}
