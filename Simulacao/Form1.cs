using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Simulacao
{
    public partial class Form1 : Form
    {

        private GeradorAleatorio gen;
        private List<Process> list_of_process;              //guarda todos os processos gerados pelos computadores

        public Form1()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(f1_paint);
            gen = new GeradorAleatorio(2921256, 2^89 - 1, 0, 0);

            list_of_process = new List<Process>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void f1_paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black);
            // Get Graphics Object
            Graphics g = e.Graphics;


            //Desenhando as linhas verticais
            g.DrawLine(pen, 85, 355, 85, 250);
            g.DrawLine(pen, 250, 385, 250, 300);
            g.DrawLine(pen, 405, 385, 405, 250);

            //Desenhando a linha horizontal
            g.DrawLine(pen, 85, 250, 405, 250);

            //Desenhando a linha PC->Servidor
            g.DrawLine(pen, 250, 300, 250, 200);

            //Desenhando as linhas Servidor->Impressora
            g.DrawLine(pen, 190, 150, 130, 150);        //Impressora esquerda
            g.DrawLine(pen, 250, 150, 400, 150);        //Impressora direita
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Controla o próximo processo que vai gerar um arquivo
            float next_pc;

            // next_pc = gen.rand();
            next_pc = 0.33f;

            if (next_pc <= 0.33f)           //se o numero gerado for menor ou igual a 33%
            {
                //Entao um processo iniciando no computador da esquerda deve ser gerado



            }else if (next_pc > 0.33f && next_pc <= 0.66f)
            {
                //Caso o numero estiver entre 33% e 66%
                //Será gerado um processo no computador do meio


            }
            else
            {
                //Caso contrário será gerado um processo no computador da direita


            }
        }

   


    }
}
