﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace MediaPlayer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 

        public static Frame RootFrame;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            {
                //First we want to check to see if the application was already running.

                if (args.PreviousExecutionState != ApplicationExecutionState.Running)
                {
                    //If it wasn't, then we want to check to see if it was terminated the last time it was run, 
                    //which we will pass on to our Splash page. 
                    bool loadState = (args.PreviousExecutionState == ApplicationExecutionState.Terminated);
                    //Create a new Splash page object passing th    e SplashScreen object to the constructor. 
                    Splash splashPage = new Splash(args.SplashScreen, loadState);
                    //Set our current app's content = the new Splash page. 
                    Window.Current.Content = splashPage;
                }
                Window.Current.Activate();
                RemoveExtendedSplash();
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }      

        private async void RemoveExtendedSplash()
        {
            await Task.Delay(TimeSpan.FromSeconds(1.5));
            RootFrame = new Frame();
            RootFrame.Style = App.Current.Resources["RootFrameStyle"] as Style;
            RootFrame.Navigate(typeof(MainPage));
            Window.Current.Content = RootFrame;
        }
        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);

            SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
        }
        void onCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs e)
        {
            SettingsCommand defaultsCommand = new SettingsCommand("defaults", "Defaults",
                (handler) =>
                {
                    // SettingsFlyout1 is defined in "SettingsFlyout1.xaml"
                    SettingsFlyout1 sf = new SettingsFlyout1();
                    sf.Show();
                });
            e.Request.ApplicationCommands.Add(defaultsCommand);
            SettingsCommand privacyCommand = new SettingsCommand("privacy policy", "Privacy Policy",
                (handler) =>
                {
                    // SettingsFlyout2 is defined in "SettingsFlyout2.xaml"
                    SettingsFlyout2 sfpp = new SettingsFlyout2();
                    sfpp.Show();
                });
            e.Request.ApplicationCommands.Add(privacyCommand);

        }
    }
}
