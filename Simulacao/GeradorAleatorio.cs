using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulacao
{
    class GeradorAleatorio
    {


        public int a { get; set; }

        public double m { get; set; }

        public int c { get; set; }

        public double seed { get; set; }

        public GeradorAleatorio(int a, double m, int c, double seed)
        {
            this.a = a;
            this.m = m;
            this.c = c;
            this.seed = seed;
        }

        public double rand()
        {
            double to_return;

            this.seed = ((this.seed * this.a + this.c) % this.m);
            to_return = this.seed / this.m ;
           
            
            return to_return;
        }
    }
}
