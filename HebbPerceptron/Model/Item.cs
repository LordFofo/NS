using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HebbPerceptron.Model
{
    public class Item
    {
        private Boolean controled=false;
        public Boolean Controled
        {
            get { return controled; }
            set { controled = value; }
        }

        private double x;

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        private double y;

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        private double output;

        public double OutPut
        {
            get { return output; }
            set { output = value; }
        }

        private double calculateOutPut;

        public double CalculateOutPut
        {
            get { return calculateOutPut; }
            set { calculateOutPut = value; }
        }

    }
}
