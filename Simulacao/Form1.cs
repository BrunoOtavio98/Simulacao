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
            Process p;
            next_pc = gen.rand();
           
            if (next_pc <= 0.33f)           //se o numero gerado for menor ou igual a 33%
            {
                //Entao um processo iniciando no computador da esquerda deve ser gerado
                p = new Process(generate_process(1), 1);
                p.printer = 0;
               list_of_process.Add(p);


            }else if (next_pc > 0.33f && next_pc <= 0.66f)
            {
                //Caso o numero estiver entre 33% e 66%
                //Será gerado um processo no computador do meio
                p = new Process(generate_process(2), 2);
                p.printer = 1;
                list_of_process.Add(p);

            }
            else
            {
                //Caso contrário será gerado um processo no computador da direita
                p = new Process(generate_process(3), 3);
                p.printer = 0;
                list_of_process.Add(p);

            }
        }

        private PictureBox generate_process(int pc)
        {
            //Os processos quando criados iniciarão nas posições aqui definidas
            PictureBox process = new PictureBox();

            process.Visible = true;
            process.Image = Properties.Resources.Process;
    //        process.Size = new System.Drawing.Size(0, 10);
            process.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            process.TabIndex = 5;
            process.TabStop = false;

            if (pc == 1)
            {
                process.Location = new System.Drawing.Point(75, 255);
            }
            else if(pc == 2)
            {
                process.Location = new System.Drawing.Point(235, 250);
            }
            else
            {
                process.Location = new System.Drawing.Point(390, 255);
            }
            this.Controls.Add(process);
            return process;
        }

        private void finish_process(PictureBox process)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //timer responsável pela movimentação dos processos
            int x;
            int y;



            foreach (Process item in list_of_process)
            {

                if ((item.process.Location.X >= 75 && item.process.Location.X <= 235) && (item.process.Location.Y >= 135 && item.process.Location.Y <= 235) && item.pc_pai == 1)        //canto esquerdo
                {
                    item.process.Location = new Point(item.process.Location.X + 1, item.process.Location.Y);
                }

                else if ((item.process.Location.X >= 235 && item.process.Location.X <= 390) && item.process.Location.Y <= 235 && item.pc_pai == 3)      //canto direito
                {
                     item.process.Location = new Point(item.process.Location.X - 1, item.process.Location.Y);
                }

                else if (item.process.Location.Y < 135)
                {
                    if (item.printer == 0) {
                        if (item.process.Location.X < 75)
                        {

                            item.process.Visible = false;

                        }
                        else {
                            item.process.Location = new Point(item.process.Location.X - 1, item.process.Location.Y);
                        }
                        
                    }
                    else if (item.printer == 1)
                    {
                        if (item.process.Location.X >= 390)
                        {
                            item.process.Visible = false;

                        }
                        else
                        {
                            item.process.Location = new Point(item.process.Location.X + 1, item.process.Location.Y);
                        } 
                    }  
                }
                
                else
                {
                    item.process.Location = new Point(item.process.Location.X, item.process.Location.Y - 1);
                }
            }
        }
    }
}
