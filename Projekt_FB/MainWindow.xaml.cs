using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Projekt_FB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            // Rozpoczęcie odtwarzania i uruchomienie odliczania czasu
            mediaPlayer.Play();
            timer.Start();


            Slider newSlider = new Slider();
            newSlider.Width = 710;
            
            newSlider.ValueChanged += (s, ev) =>
            {
                // Ustawienie nowej pozycji na podstawie wartości suwaka
                TimeSpan newTime = TimeSpan.FromSeconds(newSlider.Value);
                mediaPlayer.Position = newTime;
            };

            StackPanel stackPanel = (StackPanel)lesnySamochodzikButton.Parent;
            stackPanel.Children.Add(newSlider);







            newSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }


        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            // Odtwarzanie zostało zakończone, usuń Slider
            Slider sliderToRemove = fairyTalesMenu.Children.OfType<Slider>().FirstOrDefault();
            if (sliderToRemove != null)
            {
                fairyTalesMenu.Children.Remove(sliderToRemove);
            }
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            // Aktualizacja wskaźnika czasu odtwarzania
            foreach (Slider slider in fairyTalesMenu.Children.OfType<Slider>())
            {
                if (mediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    // Ustawienie wartości slidera na aktualną pozycję odtwarzania
                    slider.Value = mediaPlayer.Position.TotalSeconds;
                }
            }
        }

        private void paintingButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void fairyTalesButton_Click(object sender, RoutedEventArgs e)
        {
            // Ukryj menu główne (mainMenu)
            mainMenu.Visibility = Visibility.Collapsed;

            // Pokaż menu bajek (fairyTalesMenu)
            fairyTalesMenu.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (mediaPlayer.Source == null)
            {
                // Wstaw właściwą ścieżkę do pliku MP3
                mediaPlayer.Open(new Uri("C:\\STUDIA\\ProjektWPF\\Projekt_FB\\Projekt_FB\\NowyDomekMyszki.mp3"));
            }

            
        }


        private void lesnySamochodzikButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.Source == null)
            {
                // Wstaw właściwą ścieżkę do pliku MP3
                mediaPlayer.Open(new Uri("C:\\STUDIA\\ProjektWPF\\Projekt_FB\\Projekt_FB\\lesnySamochodzik.mp3"));
            }
        }
    }
}