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

        public int processID { get; set; }

        public int printer { get; set; }        //0 vai para esquerda, 1 vai para a direita

        public int pc_pai { get; set; }

        public int removeItem { get; set; }

        public bool motionDisabled { get; set; }

        public TextBox localId { get; set; }


        //Inserção das estatisticas de cada processo

        public int IC { get; set; }

        public int TA { get; set; }

        public int TC { get; set; }

        public int IA { get; set; }

        public int FA { get; set; }

        public int FA1 { get; set; }

        public int FA2 { get; set; }

        public int EF { get; set; }

        public int PS { get; set; }

        public int TO { get; set; }

        public Process(PictureBox pictureBox, int pc_pai)
        {
            process = pictureBox;
            this.pc_pai = pc_pai;
        }

        public Process DeepCopy()
        {
            Process other = (Process)this.MemberwiseClone();

            other.process = this.process;
            other.processID = this.processID;
            other.printer = this.printer;
            other.pc_pai = this.pc_pai;
            other.removeItem = this.removeItem;
            other.removeItem = this.removeItem;
            other.motionDisabled = this.motionDisabled;
         
            return other;
        }
               
        public override bool Equals(object obj)
        {
            Process p = (Process)obj;

            return (p.processID == this.processID);
        }
    }
}
