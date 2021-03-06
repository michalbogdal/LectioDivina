using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using LectioDivina.Model;
using LectioDivina.Service;
using LectioDivinaWydawca;

using MvvmLight.Extensions;
using WordAutomation;

namespace LectioDivina.Wydawca.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// 
        private LectioDivinaWeek lectioDivinaWeek;
        private ILectioDataService dataService;
        private IDialogService dialogService;
        private ICredentialsService credentialsService;
        private bool isDirty;

        public MainViewModel(ILectioDataService dataService, IDialogService dialogService, ICredentialsService credentialsService)
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            this.dataService = dataService;
            this.dialogService = dialogService;
            this.credentialsService = credentialsService;
            //this.loggingService = loggingService;

            CreateCommands();
            InitiateData();
        }

        private void Log(string msg)
        {
            if (LoggingService != null)
                LoggingService.Log(msg);
        }
        private void InitiateData()
        {
            lectioDivinaWeek = dataService.Load();

            TitlePage = new TitlePageVM(lectioDivinaWeek.Title);

            Sunday = new OneDayContemplationVM(lectioDivinaWeek.Sunday);
            Monday = new OneDayContemplationVM(lectioDivinaWeek.Monday);
            Tuesday = new OneDayContemplationVM(lectioDivinaWeek.Tuesday);
            Wednesday = new OneDayContemplationVM(lectioDivinaWeek.Wednesday);
            Thursday = new OneDayContemplationVM(lectioDivinaWeek.Thursday);
            Friday = new OneDayContemplationVM(lectioDivinaWeek.Friday);
            Saturday = new OneDayContemplationVM(lectioDivinaWeek.Saturday);

            RefreshTargetFileProperty();

            IsDirty = false;
        }

        public ILoggingService LoggingService { get; set; }

        #region VM properties
        private TitlePageVM titlePage;
        public TitlePageVM TitlePage
        {
            get { return titlePage; }
            set
            {
                titlePage = value;
                RaisePropertyChanged(() => this.TitlePage);

                titlePage.PropertyChanged += titlePage_PropertyChanged;
            }
        }

        void titlePage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsDirty = true;
            if (e.PropertyName.Equals("WeekInvocation") ||
                e.PropertyName.Equals("WeekDescription") ||
                e.PropertyName.Equals("LectioTargetFolder")) // todo replace string with expression
            {
                RefreshTargetFileProperty();
            }
        }

        private OneDayContemplationVM sunday;
        public OneDayContemplationVM Sunday
        {
            get { return sunday; }
            set
            {
                sunday = value;
                RaisePropertyChanged(() => this.Sunday);

                sunday.PropertyChanged += oneDay_PropertyChanged;
            }
        }

        private OneDayContemplationVM monday;
        public OneDayContemplationVM Monday
        {
            get { return monday; }
            set
            {
                monday = value;
                RaisePropertyChanged(() => this.Monday);

                monday.PropertyChanged += oneDay_PropertyChanged;
            }
        }

        private OneDayContemplationVM tuesday;
        public OneDayContemplationVM Tuesday
        {
            get { return tuesday; }
            set
            {
                tuesday = value;
                RaisePropertyChanged(() => this.Tuesday);

                tuesday.PropertyChanged += oneDay_PropertyChanged;
            }
        }

        private OneDayContemplationVM wednesday;
        public OneDayContemplationVM Wednesday
        {
            get { return wednesday; }
            set
            {
                wednesday = value;
                RaisePropertyChanged(() => this.Wednesday);

                wednesday.PropertyChanged += oneDay_PropertyChanged;
            }
        }

        private OneDayContemplationVM thursday;
        public OneDayContemplationVM Thursday
        {
            get { return thursday; }
            set
            {
                thursday = value;
                RaisePropertyChanged(() => this.Thursday);

                thursday.PropertyChanged += oneDay_PropertyChanged;
            }
        }

        private OneDayContemplationVM friday;
        public OneDayContemplationVM Friday
        {
            get { return friday; }
            set
            {
                friday = value;
                RaisePropertyChanged(() => this.Friday);

                friday.PropertyChanged += oneDay_PropertyChanged;
            }
        }

        private OneDayContemplationVM saturday;
        public OneDayContemplationVM Saturday
        {
            get { return saturday; }
            set
            {
                saturday = value;
                RaisePropertyChanged(() => this.Saturday);

                saturday.PropertyChanged += oneDay_PropertyChanged;
            }
        }


        void oneDay_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsDirty = true;
        }
        #endregion

        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    RaisePropertyChanged(() => IsDirty);
                    Save.RaiseCanExecuteChanged();
                }
            }
        }

        #region Commands
        public RelayCommand Save { get; set; }
        public RelayCommand Send { get; set; }
        public RelayCommand Receive { get; set; }
        public RelayCommand CloseApp { get; set; }
        public RelayCommand Clear { get; set; }
        public RelayCommand OpenLectioTarget { get; set; }
        public RelayCommand GenerateLectioTarget { get; set; }
        public RelayCommand SelectTemplate { get; set; }
        public RelayCommand SelectTarget { get; set; }
        public RelayCommand SelectEbookSource { get; set; }
        public RelayCommand SelectPicture { get; set; }
        public RelayCommand SelectShortContemplation { get; set; }

        #endregion

        #region private methods

        private void CreateCommands()
        {
            Save = new RelayCommand(SaveLectio);
            Send = new RelayCommand(SendLectioToServer);
            Receive = new RelayCommand(ReceiveLectiosFromServer);
            Clear = new RelayCommand(ClearLectio);
            CloseApp = new RelayCommand(Close);
            OpenLectioTarget = new RelayCommand(OpenLectioTargetInWord);
            GenerateLectioTarget = new RelayCommand(GenerateLectioTargetDoc);
            SelectTemplate = new RelayCommand(SelectLectioTemplate);
            SelectTarget = new RelayCommand(SelectLectioTarget);
            SelectEbookSource = new RelayCommand(SelectLectioEbookSource);
            SelectPicture = new RelayCommand(SelectPictureFile);
            SelectShortContemplation = new RelayCommand(SelectShortContemplationFile);
        }

        private void SelectShortContemplationFile()
        {
            string template = dialogService.SelectFile("Wybierz pliku Rozwa�a� kr�tkich", "*.doc;*.docx");

            if (!String.IsNullOrEmpty(template))
                TitlePage.WeekShortContemplationName = template;
        }

        private void SelectPictureFile()
        {
            string picture = dialogService.SelectFile("Wybierz obrazek do bie��cego Lectio Divina", "*.jpg;*.png");

            if (!String.IsNullOrEmpty(picture))
                TitlePage.WeekPictureName = picture;
        }

        private void SelectLectioTemplate()
        {
            string template = dialogService.SelectFile("Wybierz szablon pliku Lectio Divina", "*.doc;*.docx");

            if (!String.IsNullOrEmpty(template))
                TitlePage.LectioTemplateFile = template;
        }

        private void SelectLectioTarget()
        {
            string folder = dialogService.SelectFolder("Wybierz katalog, w kt�rym umieszczony zostanie bie��cy plik Lectio Divina", TitlePage.LectioTargetFolder);
            if (!String.IsNullOrEmpty(folder))
                TitlePage.LectioTargetFolder = folder;
        }

        private void SelectLectioEbookSource()
        {
            string folder = dialogService.SelectFolder("Wybierz katalog zawierajacy pliki html do tworzenia ebooka Lectio Divina", TitlePage.LectioEbookSourceFolder);
            if (!String.IsNullOrEmpty(folder))
                TitlePage.LectioEbookSourceFolder = folder;
        }

        private void RefreshTargetFileProperty()
        {
            TitlePage.LectioTargetFile = dataService.ProposeLectioTargetName(TitlePage.LectioTargetFolder, TitlePage.WeekInvocation, TitlePage.WeekDescription);

        }
        private void OpenLectioTargetInWord()
        {
            try
            {
                System.Diagnostics.Process.Start(TitlePage.LectioTargetFile);
            }
            catch (Exception ex)
            {
                string msg = "Nie uda�o si� otworzy� Lectio:\r\n" + ex.Message;
                Log(msg);
                dialogService.ShowError(msg, "B��d", "OK", null);
            }
        }

        private void GenerateLectioTargetDoc()
        {
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            bool showFinishInfo = true;

            System.Threading.Tasks.Task.Factory
                /* in fact synchronously - as we use current sync context */
            .StartNew(() =>
            {
                List<string> issues = lectioDivinaWeek.Validate();

                if (issues.Count > 0)
                {
                    Log("Braki w Lectio\r\n" + issues.Aggregate((a, b) => a + "\r\n" + b));

                    dialogService.ShowMessage("W rozwa�aniach s� braki. Tworzy� Lectio mimo wszystko?",
                                            "Potwierdzenie",
                                            buttonConfirmText: "Tak", buttonCancelText: "Nie",
                                            afterHideCallback: (confirmed) =>
                                            {
                                                if (confirmed)
                                                    GenerateLectio();
                                                else
                                                {
                                                    Log("Lectio nie zosta�o utworzone ze wzgl�du na braki.");
                                                    showFinishInfo = false;
                                                    return;
                                                }


                                            });
                }
                else
                    GenerateLectio();

            }, CancellationToken.None, TaskCreationOptions.LongRunning, scheduler)
                /* when completed, display response */
            .ContinueWith((t) =>
            {
                dialogService.SetNormal();
                if (t.Exception != null)
                {
                    string msg = "Nie uda�o si� utworzy� Lectio:\r\n" + t.Exception.InnerException.Message;
                    Log(msg);
                    dialogService.ShowError(msg, "B��d", "OK", null);
                }
                else if (showFinishInfo)
                {
                    Log("Zako�czono tworzenie Lectio");
                    dialogService.ShowMessage("Zako�czono tworzenie Lectio", "Informacja");
                }
            }, CancellationToken.None, TaskContinuationOptions.None, scheduler);

        }

        private void GenerateLectio()
        {
            dialogService.SetBusy();
            Log("Rozpocz�to tworzenie Lectio. Czekaj...");

            if (TitlePage.IsPictureFromShortContemplation)
                ExtractPictureFromShortContemplation();

            try
            {
                if (!String.IsNullOrEmpty(TitlePage.LectioEbookSourceFolder))
                {
                    var ebookLectioGenerator = new OnJestEbookMaker(TitlePage.LectioEbookSourceFolder, lectioDivinaWeek);
                    string ebookOutputName = OnJestEbookMaker.GetEbookName(lectioDivinaWeek);
                    ebookLectioGenerator.Notification += Progress_Notification;
                    ebookLectioGenerator.GenerateEbook(ebookOutputName);
                }
            }
            catch (Exception exception)
            {
                Log("B��d podczas generowania ebook-a\r\n" + exception.Message);
            }
            
            var lectioGenerator = new LectioDivinaGenerator();
            lectioGenerator.Notification += Progress_Notification;
            lectioGenerator.GenerateLectio(TitlePage.LectioTemplateFile, TitlePage.WeekPictureName, TitlePage.LectioTargetFile,
                lectioDivinaWeek,
                Properties.Settings.Default.ShowWord);
                

        }

        void Progress_Notification(object sender, NotificationEventArgs e)
        {
            Log(e.Notification);
        }

        private void Close()
        {
            if (IsDirty)
            {
                dialogService.ShowMessage("Rozwa�ania by�y zmienione. Zapisa� przed zako�czeniem?",
                    "Potwierdzenie",
                    buttonConfirmText: "Tak", buttonCancelText: "Nie",
                    afterHideCallback: (confirmed) =>
                    {
                        if (confirmed)
                            SaveLectio();
                        App.Current.Shutdown();
                    });
            }
            else
                App.Current.Shutdown();
        }

        private void ClearLectio()
        {
            dialogService.ShowMessage("Wyczy�ci� wszystkie pola?",
                "Uwaga",
                buttonConfirmText: "Tak", buttonCancelText: "Nie",
                afterHideCallback: (confirmed) =>
                {
                    if (confirmed)
                    {
                        Log("Wszystke pola wyczyszczone.");
                        TitlePage.SundayDate = dataService.GetNearestSunday();
                        TitlePage.WeekDescription = "";
                        ReplaceDay(Sunday, new OneDayContemplation());
                        ReplaceDay(Monday, new OneDayContemplation());
                        ReplaceDay(Tuesday, new OneDayContemplation());
                        ReplaceDay(Wednesday, new OneDayContemplation());
                        ReplaceDay(Thursday, new OneDayContemplation());
                        ReplaceDay(Friday, new OneDayContemplation());
                        ReplaceDay(Saturday, new OneDayContemplation());
                    }
                });
        }

        private void SendLectioToServer()
        {
            System.Threading.Tasks.Task.Factory
                /* in fact synchronously - as we use current sync context */
            .StartNew(() =>
            {
                List<string> issues = lectioDivinaWeek.Validate();

                if (issues.Count > 0)
                {
                    Log("Braki w Lectio\r\n" + issues.Aggregate((a, b) => a + "\r\n" + b));

                    dialogService.ShowMessage("W rozwa�aniach s� braki. Wys�a� Lectio mimo wszystko?",
                                            "Potwierdzenie",
                                            buttonConfirmText: "Tak", buttonCancelText: "Nie",
                                            afterHideCallback: (confirmed) =>
                                            {
                                                if (confirmed)
                                                    SendLectio();
                                                else
                                                {
                                                    Log("Lectio nie zosta�o wys�ane ze wzgl�du na braki.");
                                                    return;
                                                }


                                            });
                }
                else
                    SendLectio();

            })
                /* when completed, display response */
            .ContinueWith((t) =>
            {
                if (t.Exception != null)
                {
                    string msg = "Co� posz�o �le przy wysy�aniu Lectio:\r\n" + t.Exception.InnerException.Message;
                    Log(msg);
                    dialogService.ShowError(msg, "B��d", "OK", null);
                }
                else
                {
                    Log("Zako�czono wysy�anie Lectio");
                    dialogService.ShowMessage("Zako�czono wysy�anie Lectio", "Informacja");
                }
            });
        }

        private void SendLectio()
        {
            var poster = new OnJestPostSender(credentialsService);

            poster.Notification += Progress_Notification;

            poster.SendLectio(lectioDivinaWeek);
        }

        private void ReceiveLectiosFromServer()
        {
            int count = 0;
            System.Threading.Tasks.Task.Factory
                .StartNew(() =>
                {
                    Log("Odbieram  z serwera Lectio od autor�w");
                    Credentials emailPwd = credentialsService.Load();
                    emailPwd = CredentialsValidator.UpdateEmailPwdIfMissing(emailPwd, credentialsService);
                    MailTransport transport = new MailTransport(emailPwd);
                    transport.Notification += Progress_Notification;
                    List<OneDayContemplation> contemplations = transport.RetrieveContemplations();
                    foreach (var contemplation in contemplations)
                    {
                        AddContemplationToWeek(contemplation);
                        count++;
                    }

                })
                .ContinueWith((t) =>
                {
                    if (t.Exception != null)
                    {
                        string msg = "Nie uda�o si� odebra� Lectio:\r\n" + t.Exception.InnerException.Message;
                        Log(msg);
                        dialogService.ShowError(msg, "B��d", "OK", null);
                    }
                    else
                    {
                        Log(String.Format("Odebrano {0} rozwa�a�", count));
                        dialogService.ShowMessage(String.Format("Zako�czono odbieranie Lectio, odebrano {0}. ", count), "Informacja");
                    }
                }
                );
        }

        private void AddContemplationToWeek(OneDayContemplation contemplation)
        {
            switch (contemplation.Day.DayOfWeek)
            {
                case DayOfWeek.Sunday: { ReplaceDay(Sunday, contemplation); break; }
                case DayOfWeek.Monday: { ReplaceDay(Monday, contemplation); break; }
                case DayOfWeek.Tuesday: { ReplaceDay(Tuesday, contemplation); break; }
                case DayOfWeek.Wednesday: { ReplaceDay(Wednesday, contemplation); break; }
                case DayOfWeek.Thursday: { ReplaceDay(Thursday, contemplation); break; }
                case DayOfWeek.Friday: { ReplaceDay(Friday, contemplation); break; }
                case DayOfWeek.Saturday: { ReplaceDay(Saturday, contemplation); break; }
            }
        }

        private void ReplaceDay(OneDayContemplationVM target, OneDayContemplation source)
        {
            target.Day = source.Day;
            target.DayDescription = source.DayDescription;
            target.Title = source.Title;
            target.ReadingReference = source.ReadingReference;
            target.ReadingText = source.ReadingText;
            target.Contemplation1 = source.Contemplation1;
            target.Contemplation2 = source.Contemplation2;
            target.Contemplation3 = source.Contemplation3;
            target.Contemplation4 = source.Contemplation4;
            target.Contemplation5 = source.Contemplation5;
            target.Contemplation6 = source.Contemplation6;
            target.Prayer = source.Prayer;
        }

        private void SaveLectio()
        {
            try
            {
                dataService.Save(lectioDivinaWeek);
                IsDirty = false;
                Log("Lectio zapisane");
            }
            catch (Exception ex)
            {
                string msg = "Nie uda�o si� zapisa� Lectio:\r\n" + ex.Message;
                Log(msg);
                dialogService.ShowError(msg, "B��d", "OK", null);
            }
        }


        private void ExtractPictureFromShortContemplation()
        {
            // we will save the picture under the name from WeekPictureName
            // if it is ewmpty, we must create it

            if (String.IsNullOrEmpty(TitlePage.WeekPictureName))
                TitlePage.WeekPictureName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(TitlePage.WeekShortContemplationName),
                                            "obrazek.jpg");
            Log("Bior� obrazek z "+ TitlePage.WeekShortContemplationName);
            Log("zapisuj� jako "+TitlePage.WeekPictureName);

            WordDocument word = new WordDocument();
            word.Open(TitlePage.WeekShortContemplationName, false);
            word.ExtractImage(Properties.Settings.Default.ShortContemplationPictureNumber, TitlePage.WeekPictureName, ImageFormats.Jpg);
            word.Close();
        }


        #endregion
    }
}