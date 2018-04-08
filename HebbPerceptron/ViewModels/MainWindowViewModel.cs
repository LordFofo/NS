using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Drawing;
using System.Collections.Generic;
using Prism.Commands;
using System;
using OxyPlot;
using OxyPlot.Series;
using HebbPerceptron.Model;
using OxyPlot.Axes;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Xml;

namespace HebbPerceptron.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Property

        public Weight w { get; set; }

        private System.Windows.Media.Brush _foregraound;
        public System.Windows.Media.Brush Foreground
        {
            get { return _foregraound; }
            set { SetProperty(ref _foregraound, value); }
        }

        private DispatcherTimer timer;

        private string learn;
        public string Learn
        {
            get { return learn; }
            set { SetProperty(ref learn, value); }
        }

        private int selectedItem;
        public int SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }

        private double learning = 0.2;
        public double Learning
        {
            get { return learning; }
            set { SetProperty(ref learning, value); Perceptron.learningRate = value; }
        }

        private int iteration;
        public int Iteration
        {
            get { return iteration; }
            set { SetProperty(ref iteration, value); }
        }

        private PlotModel model;
        public PlotModel Model
        {
            get { return model; }
            set { SetProperty(ref model, value); }
        }

        private ObservableCollection<Weight> weight;
        public ObservableCollection<Weight> Weight
        {
            get { return weight; }
            set { SetProperty(ref weight, value); }
        }

        private ObservableCollection<Item> treningdata;
        public ObservableCollection<Item> TreningData
        {
            get { return treningdata; }
            set { SetProperty(ref treningdata, value); }
        }

        private ObservableCollection<Item> testingdata;
        public ObservableCollection<Item> TestingData
        {
            get { return testingdata; }
            set { SetProperty(ref testingdata, value); }
        }

        #endregion

        public Input ax,ay;

        public List<DataPoint> Controled { get; set; }
        public List<DataPoint> Positive;
        public List<DataPoint> Negative;
        public List<DataPoint> Lines { get; set; }

        public DelegateCommand OneStepCommand { get; set; }
        public DelegateCommand FastCommand { get; set; }
        public DelegateCommand ResetCommand { get; set; }
        public DelegateCommand AutoCommand { get; set; }
        public DelegateCommand LoadCommand { get; set; }

        public MainWindowViewModel()
        {
            
            OneStepCommand = new DelegateCommand(OneStep,CanStep).ObservesProperty(() => Learn);
            FastCommand = new DelegateCommand(Fast,CanStep).ObservesProperty(() => Learn);
            ResetCommand = new DelegateCommand(PrepereToStart,Can);
            AutoCommand = new DelegateCommand(Auto,CanStep).ObservesProperty(()=>Learn);
            LoadCommand = new DelegateCommand(LoadData);

           
            InitialData();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += new EventHandler(AutoEvent);
            PrepereToStart();  
        }

        private bool Can()
        {
            w = new Weight();
            return true;
        }

        private void InitialData()
        {
            TreningData = new ObservableCollection<Item>();
            TestingData = new ObservableCollection<Item>();

            TreningData.Add(new Item() { X = 1, Y = 1, OutPut = 1 });
            TreningData.Add(new Item() { X = 2, Y = -2, OutPut = 0 });
            TreningData.Add(new Item() { X = -1, Y = -1.5, OutPut = 0 });
            TreningData.Add(new Item() { X = -2, Y = -1, OutPut = 0 });
            TreningData.Add(new Item() { X = -2, Y = 1, OutPut = 1 });
            TreningData.Add(new Item() { X = 1.5, Y = -0.5, OutPut = 1 });

            TestingData.Add(new Item() { X = 1, Y = 0 });
            TestingData.Add(new Item() { X = 0, Y = 0.5 });
            TestingData.Add(new Item() { X = -1, Y = -0.5 });

            ax = new Input(-3, 3,"X");
            ay = new Input(-3, 3,"Y");
            w = new Weight();        

        }

        private void LoadData()
        {
            XmlDocument reader= new XmlDocument();
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml";
            if (ofd.ShowDialog() == true)
                reader.Load(ofd.FileName);
            else return;

            TreningData = new ObservableCollection<Item>();
            TestingData = new ObservableCollection<Item>();

            foreach (XmlNode node in reader.DocumentElement.ChildNodes)
            {
                switch (node.Name)
                {
                    case "perceptron":
                        {
                            double x = double.Parse((node.FirstChild.FirstChild.InnerText).Replace('.', ','));
                            double y = double.Parse(node.FirstChild.ChildNodes[1].InnerText.Replace('.', ','));
                            string title = (node.FirstChild.LastChild.InnerText);
                            ax.X = y - 5; ax.Y = x + 5;

                            x = double.Parse((node.ChildNodes[1].FirstChild.InnerText).Replace('.', ','));
                            y = double.Parse(node.ChildNodes[1].ChildNodes[1].InnerText.Replace('.', ','));
                            title = (node.ChildNodes[1].LastChild.InnerText);
                            ay.X = y - 5; ay.Y = x + 5;

                            Learning = double.Parse(node.ChildNodes[2].InnerText.Replace('.', ','));

                            double w1 = double.Parse(node.ChildNodes[4].FirstChild.InnerText.Replace('.', ','));
                            double w2 = double.Parse(node.ChildNodes[4].ChildNodes[1].InnerText.Replace('.', ','));
                            double w3 = double.Parse(node.ChildNodes[4].ChildNodes[2].InnerText.Replace('.', ','));
                            w = new Weight(w1, w2, w3);
                         
                        }
                        break;

                    case "TestSet":
                        {
                            foreach (XmlNode element in node)
                            {
                                foreach (XmlNode input in element)
                                {
                                    double x = double.Parse(input.FirstChild.InnerText.Replace('.', ','));
                                    double y = double.Parse(input.LastChild.InnerText.Replace('.', ','));
                                    Item p = new Item() { X = x, Y = y };
                                    TestingData.Add(p);
                                }
                            }
                        }
                        break;
                    case "TrainSet":
                        {
                            foreach (XmlNode element in node)
                            {
                                double x = double.Parse(element.FirstChild.FirstChild.InnerText.Replace('.', ','));
                                double y = double.Parse(element.FirstChild.LastChild.InnerText.Replace('.', ','));
                                double o = double.Parse(element.LastChild.FirstChild.InnerText.Replace('.', ','));
                                Item p = new Item() { X = x, Y = y, OutPut = o };
                                TreningData.Add(p);
                            }
                        }
                        break;
                }

            }


            PrepereToStart();

        }

        #region Timer
        private void AutoEvent(object sender, EventArgs e)
            {
                if (Perceptron.Finish())
                { timer.Stop(); Foreground = System.Windows.Media.Brushes.Green; Learn = "Learned"; return; }
                OneStep();
            }

            private void Auto()
            {
                timer.Start();
            }
        #endregion

        private void Fast()
        {
            while (!Perceptron.Finish())
            {
                OneStep();
                if (Iteration > 500) { MessageBox.Show("Stagnation","Information",MessageBoxButtons.OK,MessageBoxIcon.Information); return; }
            }
            Foreground = System.Windows.Media.Brushes.Green;
            Learn = "Learned";

        }

        private bool CanStep()
        {
            if (!Perceptron.Finish())
                return true;
            else
            {
                Learn = "Learned";
                Foreground = System.Windows.Media.Brushes.Green;
                return false;
            }
        }

        private void OneStep()
        {
            TreningData = new ObservableCollection<Item>(Perceptron.OneStep());
            Weight= new ObservableCollection<Weight>(Perceptron.GetWeight()); 
            Iteration++;
            UpdateLine();
            SelectedItem = Perceptron.actual;
            Actualize();
        }

        private void UpdateLine()
        {
            Controled.Clear();
            Controled.AddRange(Perceptron.GetControledItem());
            Lines = Perceptron.GetLine();
            LineSeries line = new LineSeries() { Color = OxyColors.Green, StrokeThickness = 2 };

            foreach (var l in Lines)
            {
                line.Points.Add(new DataPoint(l.X,l.Y));
            }
            Model.Series[model.Series.Count() - 1] = line;
            Model.InvalidatePlot(true);
        }

        private void PrepereToStart()
        {
            Foreground = System.Windows.Media.Brushes.Red;
            Iteration = 0;
            Perceptron.actual = 0;
            timer.Stop();
            Learn = "Not Learned";
            Weight = new ObservableCollection<Weight>();
            Model = new PlotModel();
            Positive = new List<DataPoint>();
            Negative = new List<DataPoint>();
            Controled = new List<DataPoint>();
            Load();
            Actualize();
        }

        private void Load()
        {

            #region TreningData

            Model.Series.Add(new LineSeries() { Title = "Trening", MarkerSize = 11, MarkerType = MarkerType.Circle, LineStyle = LineStyle.None, MarkerFill = OxyColors.Red, ItemsSource = Controled });

            #endregion

           var axisx = new LinearAxis()
            {
                Minimum =ax.X,
                Maximum = ax.Y,
                PositionAtZeroCrossing = true,
                AxislineStyle = LineStyle.Solid,

            };

           var  axisy = new LinearAxis()
            {
                Minimum = ay.X,
                Maximum = ay.Y,
                PositionAtZeroCrossing = true,
                AxislineStyle = LineStyle.Solid,
                Position = AxisPosition.Top
            };

            Model.Axes.Add(axisx);
            Model.Axes.Add(axisy);


            #region TestingData

            List<DataPoint> Tested = new List<DataPoint>();
            foreach (var item in TestingData)
                Tested.Add(new DataPoint(item.X,item.Y));
            Model.Series.Add(new LineSeries() { Title = "Tested", MarkerSize = 11, MarkerType = MarkerType.Square, LineStyle = LineStyle.None, MarkerFill = OxyColors.Blue, ItemsSource = Tested});
            #endregion
           
            LineSeries Pos = new LineSeries() { Title = "1", MarkerSize = 8, MarkerType = MarkerType.Triangle, LineStyle = LineStyle.None, MarkerFill = OxyColors.GreenYellow, ItemsSource = Positive };
            LineSeries Neg = new LineSeries() { Title = "0", MarkerSize = 8, MarkerType = MarkerType.Diamond, LineStyle = LineStyle.None, MarkerFill = OxyColors.BlueViolet, ItemsSource = Negative };


            Model.Series.Add(Pos);
            Model.Series.Add(Neg);

            Model.Series.Add(new LineSeries());
            model.InvalidatePlot(true);

            Perceptron.Generate(TreningData,w);
            UpdateLine();

            Weight.Clear();
            Weight.AddRange(Perceptron.GetWeight());
            UpdateLine();

        }

        private void Actualize()
        {
        
            Positive = new List<DataPoint>();
            Negative = new List<DataPoint>();
            foreach (var a in TreningData)
            {
                if (a.OutPut == 1) Positive.Add(new DataPoint(a.X, a.Y));
                else Negative.Add(new DataPoint(a.X, a.Y));
            }
            TestingData = new ObservableCollection<Item>(Perceptron.TestedData(TestingData));
            foreach (var a in TestingData)
            {
                if(a.CalculateOutPut==1) Positive.Add(new DataPoint(a.X, a.Y));
                else Negative.Add(new DataPoint(a.X, a.Y));
            }

            ((LineSeries)(Model.Series[2])).ItemsSource = Positive;
            ((LineSeries)(Model.Series[3])).ItemsSource = Negative;
            Model.InvalidatePlot(true);
        }

        

    }
}
