using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulacao
{
    class Process
    {
        public float x { get; set; }

        public float y { get; set; }

        public Process(float x, float y)
        {
            this.x = x;
            this.y = y;
        }  
    }
}
