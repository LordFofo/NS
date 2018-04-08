using HebbPerceptron.Model;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HebbPerceptron.ViewModels.MainWindowViewModel;

namespace HebbPerceptron.ViewModels
{
    public static class Perceptron
    {
        private static Random r = new Random();
        public static double learningRate = 0.2;
        private static ObservableCollection<Item> data;
        public static Weight weight;
        public static int actual = 0;

        public static void Generate(ObservableCollection<Item> treningdata,Weight w)
        {          
            weight = w;
            data = new ObservableCollection<Item>();
            data.AddRange(treningdata);
            Check();
            SetFalse();
        }

        public static Boolean Finish()
        {
            //foreach (var a in Change)
            //    if (!a) return false;
            return data.Select(y=>y.Controled).All(x => x == true);
        }

        private static void Check()
        {
            //for (int i = 0; i < data.Count(); i++)
            //{
            //    double ret = (data[i].X * weight.X1) + (data[i].Y * weight.X2) + weight.X0;
            //    ret = (ret > 0) ? 1 : 0;
            //    data[i].CalculateOutPut = ret;
            //}

            double[] d = data.Select(x =>x.CalculateOutPut = ((x.X * weight.X1) + (x.Y * weight.X2) + weight.X0) > 0 ? 1 : 0).ToArray();

            for (int i = 0; i < data.Count(); i++)
            {
                data[i].CalculateOutPut = d[i];
            }
        }

        public static ObservableCollection<Item> OneStep()
        {
            double delta = data[actual].OutPut - data[actual].CalculateOutPut;

            if (delta != 0)
            {
                weight.X0 += learningRate * 1 * delta;
                weight.X1 += learningRate * data[actual].X * delta;
                weight.X2 += learningRate * data[actual].Y * delta;
                SetFalse(); Check();
            }
            else
            {
                data[actual].Controled = true;
            }

            #region 2 Possible way
            //if (data[actual].CalculateOutPut > data[actual].OutPut)
            //{
            //    weight.X0 -= learningRate * 1;
            //    weight.X1 -= learningRate * data[actual].X;
            //    weight.X2 -= learningRate * data[actual].Y;

            //    SetFalse(); Check();
            //}
            //else if (data[actual].CalculateOutPut < data[actual].OutPut)
            //{
            //    weight.X0 += learningRate * 1;
            //    weight.X1 += learningRate * data[actual].X;
            //    weight.X2 += learningRate * data[actual].Y;

            //    SetFalse(); Check();
            //}
            //else
            //{
            //    data[actual].Controled = true;
            //}
            #endregion

            actual++;
            actual %= data.Count();

            return data;

        }

        public static void SetFalse()
        {
          
            for (int i = 0; i < data.Count(); i++)
            {
                data[i].Controled = false;
            }
        }

        public static List<DataPoint> GetLine()
        {
            List<DataPoint> points = new List<DataPoint>();

            int xmax = (int)data.Max(xx => xx.X) + 10;
           int xmin = (int)data.Min(xx => xx.X) - 10;

           points.Add(new DataPoint(xmax, x2(xmax)));
            points.Add(new DataPoint(xmin, x2(xmin)));
            return points;
        }

        private static double x2(double x1)
        {
            return ((-weight.X0 - (x1 * weight.X1)) / weight.X2);
        }

        public static ObservableCollection<Weight> GetWeight()
        {
            ObservableCollection<Weight> w = new ObservableCollection<Weight>();
            w.Add(weight);
            return w;
        }

        public static List<DataPoint> GetControledItem()
        {
           return data.Where(x => x.Controled == true).Select(y=>new DataPoint(y.X,y.Y)).ToList();
        }

        public static ObservableCollection<Item> TestedData(ObservableCollection<Item> dat)
        {
            ObservableCollection<Item> ret = new ObservableCollection<Item>(dat);

            for (int i = 0; i < ret.Count(); i++)
            {
                double r = (ret[i].X * weight.X1) + (ret[i].Y * weight.X2) + weight.X0;
                r = (r > 0) ? 1 : 0;
                ret[i].CalculateOutPut = r;
            }

            return ret;
        }


    }


}
