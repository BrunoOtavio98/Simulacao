using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulacao
{
    class Process
    {

        public PictureBox process { get; set; }

        public int printer { get; set; }        //0 vai para esquerda, 1 vai para a direita

        public int pc_pai { get; set; }

        public Process(PictureBox pictureBox, int pc_pai)
        {
            process = pictureBox;
            this.pc_pai = pc_pai;
        }  
    }
}
