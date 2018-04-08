using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HebbPerceptron.Model
{
    public class Input
    {

        public Input(int v1, int v2, string v3)
        {
            this.X = v1;
            this.Y = v2;
            this.title = v3;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public string title { get; set; }

    }
}
