using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Globalization;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.IO;
using System.Data;
using System.Xml.Linq;

namespace Projekt_FB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MediaPlayer mediaPlayer = new MediaPlayer();
        private DispatcherTimer timer = new DispatcherTimer();
        //private SpeechRecognitionEngine speechRecognitionEngine;
        SpeechRecognitionEngine speechRecognitionEngine = new SpeechRecognitionEngine();
        private SpeechSynthesizer speechSynthesizer;
        private bool isSpeechRecognitionEnabled = false;
        private List<string> words = new List<string> {"Książka",   "Dom",  "Drzewo",   "Kot",  "Pies", "Kwiat",    "Mama", "Tata", "Słońce",   "Księżyc",  "Góra", "Rzeka",    "Ryba", "Samochód", "Kolor",    "Zamek",    "Most", "Chmurka",  "Dziecko",  "Babcia",   "Dziadek",  "Serce",    "Noga", "Ręka", "Oko",  "Ucho", "Nos",  "Bułka",    "Mleko",    "Jajko",    "Ząb",  "Dół",  "Góra", "Długi",    "Krótki",   "Gruby",    "Cienki",   "Duży", "Mały", "Nowy", "Stary",    "Mokry",    "Suchy",    "Gorący",   "Zimny",    "Szybki",   "Wolny",    "Jasny",    "Ciemny",   "Łatwy",    "Trudny",   "Dobry",    "Zły",  "Słodki",   "Kwaśny",   "Szybki",   "Wolny",    "Wielki",   "Mały", "Głośny",   "Cichy",    "Brat", "Siostra",  "Przyjaciel",   "Szkoła",   "Zegar",    "Telewizor",    "Kuchnia",  "Lampa",    "Kredka",   "Farba",    "Piłka",    "Lalka",    "Samolot",  "Statek",   "Praca",    "Śmieci",   "Las",  "Woda", "Ogień",    "Ziemia",   "Piasek",   "Góra", "Wiatr",    "Deszcz",   "Śnieg",    "Ptak", "Motyl",    "Pszczoła", "Owoc", "Warzywo",  "Miasto",   "Wieś", "Gospodarstwo", "Król", "Królowa",  "Książę",   "Księżniczka",  "Dziękuję", "Proszę"
 };
        private StringBuilder input = new StringBuilder(); // Przechowuje wprowadzone przez użytkownika liczby i operatory
        private bool isNewInput = true; // Określa, czy aktualny ciąg znaków jest nowym wprowadzeniem
        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpenedOld;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            TestDatabaseToXmlConversion();
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            // Rozpoczęcie odtwarzania i uruchomienie odliczania czasu
            mediaPlayer.Play();

            if (!timer.IsEnabled) // Dodaj warunek sprawdzający czy timer jest już uruchomiony
            {
                timer.Start();
            }

            Slider existingSlider = FindSliderInGrid(fairyTalesMenu);


            if (existingSlider == null)
            {
                Slider newSlider = new Slider();
                newSlider.Width = 700;

                // Tworzenie obrazu z pliku
                ImageBrush nyanCatBrush = new ImageBrush();
                nyanCatBrush.ImageSource = new BitmapImage(new Uri("C:\\STUDIA\\ProjektWPF\\Projekt_FB\\Projekt_FB\\nyan_cat.png"));

                // Ustawienie obrazu jako kciuk/slider thumb
                ControlTemplate sliderTemplate = new ControlTemplate(typeof(Slider));
                FrameworkElementFactory thumb = new FrameworkElementFactory(typeof(Thumb));
                thumb.Name = "PART_Thumb";
                thumb.SetValue(WidthProperty, 30.0);
                thumb.SetValue(HeightProperty, 30.0);
                thumb.SetValue(BackgroundProperty, nyanCatBrush);
                sliderTemplate.VisualTree = thumb;
                newSlider.Template = sliderTemplate;

                newSlider.ValueChanged += (s, ev) =>
                {
                    // Ustawienie nowej pozycji na podstawie wartości suwaka
                    TimeSpan newTime = TimeSpan.FromSeconds(newSlider.Value);
                    mediaPlayer.Position = newTime;
                };

                fairyTalesMenu.Children.Add(newSlider);
                newSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }



        private void MediaPlayer_MediaOpenedOld(object sender, EventArgs e)
        {
            // Rozpoczęcie odtwarzania i uruchomienie odliczania czasu
            mediaPlayer.Play();

            if (!timer.IsEnabled) // Dodaj warunek sprawdzający czy timer jest już uruchomiony
            {
                timer.Start();
            }

            Slider existingSlider = fairyTalesMenu.Children.OfType<Slider>().FirstOrDefault();
            if (existingSlider == null)
            {
                Slider newSlider = new Slider();
                newSlider.Width = 750;
                Grid.SetRow(newSlider, 3);
                newSlider.Style = (Style)Application.Current.FindResource("FancySliderStyle");
                newSlider.ValueChanged += (s, ev) =>
                {
                    // Ustawienie nowej pozycji na podstawie wartości suwaka
                    TimeSpan newTime = TimeSpan.FromSeconds(newSlider.Value);
                    mediaPlayer.Position = newTime;
                };

                fairyTalesMenu.Children.Add(newSlider);
                newSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            // Odtwarzanie zostało zakończone, zatrzymaj odtwarzanie i usuń Slider
            mediaPlayer.Stop();
            timer.Stop();

            Slider sliderToRemove = fairyTalesMenu.Children.OfType<Slider>().FirstOrDefault();
            if (sliderToRemove != null)
            {
                fairyTalesMenu.Children.Remove(sliderToRemove);
            }

            mediaPlayer.Close();

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



        // Funkcja rekurencyjna do znalezienia Slidera w siatce
        // Funkcja rekurencyjna do znalezienia Slidera w siatce
        // Funkcja rekurencyjna do znalezienia slidera w siatce
        private Slider FindSliderInGrid(Grid grid)
        {
            foreach (var element in grid.Children)
            {
                if (element is Slider slider)
                {
                    return slider; // Znaleziono slider
                }
                else if (element is Grid nestedGrid)
                {
                    var nestedResult = FindSliderInGrid(nestedGrid); // Rekurencyjne szukanie w zagnieżdżonym Grid
                    if (nestedResult != null)
                    {
                        return nestedResult;
                    }
                }
            }
            return null; // Nie znaleziono slidera w siatce
        }



        private void paintingButton_Click(object sender, RoutedEventArgs e)
        {
            mainMenu.Visibility = Visibility.Collapsed;

            // Pokaż menu bajek (fairyTalesMenu)
            calculatorMenu.Visibility = Visibility.Visible;
            backToMenuButton.Visibility = Visibility.Visible;
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
            backToMenuButton.Visibility = Visibility.Visible;
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

        private void backToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            mainMenu.Visibility = Visibility.Visible;
            fairyTalesMenu.Visibility = Visibility.Collapsed;
            backToMenuButton.Visibility = Visibility.Hidden;
            wordsMenu.Visibility = Visibility.Collapsed;
            calculatorMenu.Visibility = Visibility.Collapsed;
            if (mediaPlayer.Source != null) { 
                mediaPlayer.Stop();
                
                Slider sliderToRemove = fairyTalesMenu.Children.OfType<Slider>().FirstOrDefault();
                if (sliderToRemove != null)
                {
                    fairyTalesMenu.Children.Remove(sliderToRemove);
                }


                mediaPlayer.Close();
            }
            wordInputTextBox.Clear();
            randomWordTextBlock.Text = "";
            speechRecognitionEngine.RecognizeAsyncCancel();
            isSpeechRecognitionEnabled = false;
        }

        //Nauka slow
        private void wordsButton_Click(object sender, RoutedEventArgs e)
        {
            // Ukryj menu główne (mainMenu)
            mainMenu.Visibility = Visibility.Collapsed;

            // Pokaż menu bajek (fairyTalesMenu)
            wordsMenu.Visibility = Visibility.Visible;
            backToMenuButton.Visibility = Visibility.Visible;

            DisplayRandomWord();
            InitializeSpeechRecognition();
            InitializeSpeechSynthesizer();
            isSpeechRecognitionEnabled = true;
        }


        



        private void InitializeSpeechRecognition()
        {
            CultureInfo cultureInfo = new CultureInfo("pl-PL"); // np. "en-US" dla angielskiego, "pl-PL" dla polskiego


            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string grammarPath = System.IO.Path.Combine(basePath, "test.grxml");
            Grammar grammar = LoadSrgsGrammar(grammarPath);


            speechRecognitionEngine.LoadGrammar(grammar);
            speechRecognitionEngine.SetInputToDefaultAudioDevice();
            speechRecognitionEngine.SpeechRecognized += SpeechRecognized;
            speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);

        }


        private Grammar LoadSrgsGrammar(string filePath)
        {
            try
            {
                // Tworzenie gramatyki z pliku SRGS
                Grammar grammar = new Grammar(filePath);

                return grammar;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas ładowania gramatyki: {ex.Message}");
                return null;
            }
        }



        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string recognizedText = e.Result.Text;

            if (Keyboard.FocusedElement is TextBox textBox && recognizedText != "Menu")
            {
                textBox.SelectedText += recognizedText;
            }
            /**
            if (recognizedText == "Menu")
            {
                SaveButton_Click(this, new RoutedEventArgs());
            }**/

        }

        private void DisplayRandomWord()
        {
            // Randomly pick a word from the list
            Random random = new Random();
            string randomWord = words[random.Next(words.Count)];

            // Display the selected word in the TextBlock
            randomWordTextBlock.Text = randomWord;
        }

        private void SubmitWordButton_Click(object sender, RoutedEventArgs e)
        {

            if (randomWordTextBlock.Text == wordInputTextBox.Text)
            {
                speechSynthesizer.Speak("Brawo! Dobrze przeczytałeś słowo!");
            }
            else 
            { 
                speechSynthesizer.Speak("Niestety, nie udało ci się."); 
            }

            DisplayRandomWord();
            wordInputTextBox.Clear();

        }

        private void InitializeSpeechSynthesizer()
        {
            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SetOutputToDefaultAudioDevice();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void HandleInput(string value)
        {
            if (isNewInput)
            {
                ResultTextBox.Text = ""; // Czyści pole tekstowe, gdy zaczynamy nowe wprowadzenie
                isNewInput = false;
            }

            input.Append(value); // Dodaje wartość (liczbę lub operator) do aktualnego wprowadzenia
            ResultTextBox.Text += value; // Aktualizuje pole tekstowe o dodaną wartość
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("1");
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("2");
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("3");
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("4");
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("5");
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("6");

        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("7");
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("8");
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("9");
        }

        private void button0_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("0");
        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("+");
        }
        private void buttonMinus_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("-");
        }
        private void buttonTimes_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("*");
        }

        private void buttonDivide_Click(object sender, RoutedEventArgs e)
        {
            HandleInput("/");
        }

        private void buttonCount_Click(object sender, RoutedEventArgs e)
        {
            string expression = ResultTextBox.Text;
            DataTable dt = new DataTable();

            try
            {
                var result = dt.Compute(expression, ""); // Obliczanie wyrażenia przy użyciu DataTable

                ResultTextBox.Text = result.ToString(); // Wyświetlenie wyniku w polu tekstowym
                isNewInput = true; // Ustawienie flagi na nowe wprowadzenie
                input.Clear(); // Wyczyszczenie bufora wprowadzania
            }
            catch (Exception ex)
            {
                ResultTextBox.Text = "Błąd"; // Informacja o błędzie w przypadku niepowodzenia obliczeń
            }
        }


        private void TestDatabaseToXmlConversion()
        {
            try
            {
                var converter = new DatabaseToXmlConverter("Host=localhost;Port=5433;Database=postgres;User Id=postgres;Password=postgres;\r\n");
                XDocument xmlDocument = converter.CreateXmlFromDatabase();
                // Optionally, save to a file or process the document as needed
                xmlDocument.Save("C:\\STUDIA\\ProjektWPF\\Projekt_FB\\Projekt_FB\\test.grxml");
            }
            catch (Exception ex)
            {
                // Handle any exceptions, possibly display an error message
                MessageBox.Show("Error: " + ex.Message);
            }
        }




    }
}