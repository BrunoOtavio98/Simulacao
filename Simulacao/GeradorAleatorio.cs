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

        public int m { get; set; }

        public int c { get; set; }

        public int seed { get; set; }

        public GeradorAleatorio(int a, int m, int c, int seed)
        {
            this.a = a;
            this.m = m;
            this.c = c;
            this.seed = seed;
        }

        public float rand()
        {
            float to_return;
            List<int> vetor = new List<int>();



            this.seed += 1;
            return to_return;
        }
    }
}
