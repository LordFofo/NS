using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HebbPerceptron.Model
{
    public class Weight
    {
        private double x0;

        public double X0
        {
            get { return x0; }
            set { x0 = value; }
        }

        private double x1;

        public double X1
        {
            get { return x1; }
            set { x1 = value; }
        }

        private double x2;

        public double X2
        {
            get { return x2; }
            set { x2 = value; }
        }

        public Weight(double x0,double x1,double x2)
        {
            this.X0 = x0;
            this.X1 = x1;
            this.X2 = x2;
        }

        public Weight()
        {
            Random r = new Random();
            double pom = r.Next(-1000000, 1000000) / 1000000.0;
            X0 = pom;
            pom = r.Next(-1000000, 1000000) / 1000000.0;
            X1 = pom;
            pom = r.Next(-1000000, 1000000) / 1000000.0;
            X2 = pom;
        }
    }
}
