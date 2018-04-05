using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BorasGrappling2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer timer;
        private int secondsLeft;
        private int secondsRestLeft;
        private int minutesLeft;
        private int minutesRestLeft;
        private int secondsDefault;
        private int minutesDefault;
        private bool play = true;
        private float volume = 0.9f;
        private int secondsRestDefault;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;

            // time
            secondsLeft = 0;
            minutesLeft = 5;
            minutesRestLeft = 0;
            secondsRestLeft = 30;

            roundCounter.Text = "1";
            timeLeft.Text = FormatTime(minutesLeft, secondsLeft);
            restTime.Text = FormatTime(minutesRestLeft, secondsRestLeft);
        }

        void timer_Tick(object sender, object e)
        {
            timeLeft.Text = FormatTime(minutesLeft, secondsLeft);

            if (secondsLeft == 0 && minutesLeft == 0)
            {
                if (play)
                {
                    PlaySoundAsync("startbell.wav"); // play end of round
                    play = false;
                }
                // reset everything
                //timer.Stop();
                //btnIncreaseTime.IsEnabled = true;
                //btnDecreaseTime.IsEnabled = true;
                if (secondsRestLeft == 0 && minutesRestLeft == 0)
                {
                    // add round 
                    var nextRound = 1 + Int32.Parse(roundCounter.Text);
                    roundCounter.Text = nextRound.ToString();

                    secondsRestLeft = secondsRestDefault;
                    secondsLeft = secondsDefault;
                    minutesLeft = minutesDefault;
                    timeLeft.Text = FormatTime(minutesLeft, secondsLeft);
                    //start next round
                    PlaySoundAsync("startbell.wav");
                    play = true;
                }
                else if (secondsRestLeft == 0 && minutesRestLeft != 0)
                {
                    // decrease minutes and reset seconds to 59
                    minutesRestLeft = minutesRestLeft - 1;
                    secondsRestLeft = 59;
                }
                else
                {
                    secondsRestLeft = secondsRestLeft - 1;
                }

                //btnStart.Content = "START";
                restTime.Text = FormatTime(minutesRestLeft, secondsRestLeft);
            }
            else if(secondsLeft == 0 && minutesLeft != 0)
            {
                // decrease minutes and reset seconds to 59
                minutesLeft = minutesLeft - 1;
                secondsLeft = 59;
            }
            else
            {
                secondsLeft = secondsLeft - 1;

            }
        }

        private void BtnIncreaseTime_Click(object sender, RoutedEventArgs e)
        {

            if(secondsLeft >= 30)
            {
                minutesLeft += 1;
                secondsLeft = 0;
            }
            else
            {
                secondsLeft += 30;
            }

            timeLeft.Text = FormatTime(minutesLeft, secondsLeft);

            // if the timer was at 0 make the button available again when there
            // is more than 10 seconds
            if (secondsLeft >= 30)
            {
                btnDecreaseTime.IsEnabled = true;
                return;
            }
        }

        private void BtnDecreaseTime_Click(object sender, RoutedEventArgs e)
        {
            // We dont like negative rounds
            if (secondsLeft == 0 && minutesLeft == 0)
            {
                btnDecreaseTime.IsEnabled = false;
                return;
            }

            if (minutesLeft != 0 && secondsLeft == 0)
            {
                minutesLeft -= 1;
                secondsLeft = 30;
            }
            else
            {
                secondsLeft -= 30;
            }

            timeLeft.Text = FormatTime(minutesLeft, secondsLeft);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

            btnIncreaseTime.IsEnabled = false;
            btnDecreaseTime.IsEnabled = false;
            btnIncreaseRestTime.IsEnabled = false;
            btnDecreaseRestTime.IsEnabled = false;

            // Do this only if there is timeleft and start is displayed!
            if (btnStart.Content.ToString() == "START" && (secondsLeft != 0 || minutesLeft != 0))
            {
                // Start round first time
                PlaySoundAsync("startbell.wav");

                //timeLeft.Text = FormatTime(minutesLeft, secondsLeft);

                secondsDefault = secondsLeft;
                minutesDefault = minutesLeft;
                secondsRestDefault = secondsRestLeft;
                timer.Start();
                btnStart.Content = "PAUSE";

            } else if (btnStart.Content.ToString() == "PAUSE")
            {
                timer.Stop();
                btnStart.Content = "CONTINUE";

            } else if (btnStart.Content.ToString() == "CONTINUE")
            {
                timer.Start();
                btnStart.Content = "PAUSE";
            }

        }

        private string FormatTime(int minutes, int seconds)
        {
            string minStr = minutes.ToString();
            string secStr = seconds.ToString();

            if(minutes < 10)
            {
                minStr = $"0{minutes}";
            } 

            if(seconds < 10)
            {
                secStr = $"0{seconds}";
            }

            return $"{minStr}:{secStr}";
        }

        private void BtnIncreaseRestTime_Click(object sender, RoutedEventArgs e)
        {
            if (secondsRestLeft == 60)
            {
                minutesRestLeft += 1;
                secondsRestLeft = 0;
            }
            else
            {
                secondsRestLeft += 10;
            }

            restTime.Text = FormatTime(minutesRestLeft, secondsRestLeft);

            // if the timer was at 0 make the button available again when there
            // is more than 10 seconds
            if (secondsRestLeft >= 10)
            {
                btnDecreaseRestTime.IsEnabled = true;
                return;
            }
        }
        private void BtnDecreaseRestTime_Click(object sender, RoutedEventArgs e)
        {
            // We dont like negative rest
            if (secondsRestLeft == 0 && minutesRestLeft == 0)
            {
                btnDecreaseRestTime.IsEnabled = false;
                return;
            }

            if (minutesRestLeft != 0 && secondsRestLeft == 0)
            {
                minutesRestLeft -= 1;
                secondsRestLeft = 50;
            }
            else
            {
                secondsRestLeft -= 10;
            }

            restTime.Text = FormatTime(minutesRestLeft, secondsRestLeft);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            //CoreApplication.Exit();
            // replaced in package manifest:
            // IgnorableNamespaces="uap mp"
            // Shutdowns the device immediately:

            //ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, TimeSpan.FromSeconds(0));
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            btnStart.Content = "START";

            // time
            secondsLeft = 0;
            minutesLeft = 5;
            minutesRestLeft = 0;
            secondsRestLeft = 30;

            btnIncreaseTime.IsEnabled = true;
            btnDecreaseTime.IsEnabled = true;
            btnIncreaseRestTime.IsEnabled = true;
            btnDecreaseRestTime.IsEnabled = true;

            roundCounter.Text = "1";
            timeLeft.Text = FormatTime(minutesLeft, secondsLeft);
            restTime.Text = FormatTime(minutesRestLeft, secondsRestLeft);
        }

        private async void PlaySoundAsync(string soundfile)
        {
            MediaElement mysong = new MediaElement();


            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync(soundfile);
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mysong.SetSource(stream, file.ContentType);
            mysong.Volume = volume;
            mysong.Play();
        }

        //private void VolumeDown_Click(object sender, RoutedEventArgs e)
        //{
        //    if(volume > 0.1f)
        //    {
        //        volume = volume - 0.1f;
        //        volumeBox.Text = $"volume: {volume}";
        //    }

        //}

        //private void VolumeUp_Click(object sender, RoutedEventArgs e)
        //{
        //    if (volume < 0.9f)
        //    {
        //        volume = volume + 0.1f;
        //        volumeBox.Text = $"volume: {volume}";
        //    }
        //}
    }
}
