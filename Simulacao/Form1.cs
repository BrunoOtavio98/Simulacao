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
        private List<Process> waiting_process;

        private int genId = 0;
        private int numberOfProcesswainting = 0;

        private int lprinter_time_control = 0;
        private int rprinter_time_control = 0;
        private bool left_printer_busy = false;
        private bool right_printer_busy = false;
        
        public Form1()
        {   

            InitializeComponent();
            this.Paint += new PaintEventHandler(f1_paint);
            gen = new GeradorAleatorio(2921256, 2^89 - 1, 0, 0);

            list_of_process = new List<Process>();
            waiting_process = new List<Process>();
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

        private void finish_process()
        {
            foreach (Process item in list_of_process.Reverse<Process>())                    //retira os processos que já foram impressos
            {
                if (item.removeItem == 1)
                {
                    list_of_process.Remove(item);
                }
            }
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
                p.printer = 2;                                                       //no inicio o processo não tem alguma impressora atribuida
             
                list_of_process.Add(p);
            }
            else if (next_pc > 0.33f && next_pc <= 0.66f)
            {
                //Caso o numero estiver entre 33% e 66%
                //Será gerado um processo no computador do meio
                p = new Process(generate_process(2), 2);
                p.printer = 2;                                                   //no inicio o processo não tem alguma impressora atribuida
               

                list_of_process.Add(p);
            }
            else
            {
                //Caso contrário será gerado um processo no computador da direita
                p = new Process(generate_process(3), 3);
                p.printer = 2;                                                //no inicio o processo não tem alguma impressora atribuida
               

                list_of_process.Add(p);
            }
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //timer responsável pela movimentação dos processos
            int x;
            int y;
        
            foreach (Process item in list_of_process)
            {

                if ((item.process.Location.X >= 75 && item.process.Location.X <= 235) && (item.process.Location.Y >= 210 && item.process.Location.Y <= 235) && item.pc_pai == 1)        //canto esquerdo
                {
                    item.process.Location = new Point(item.process.Location.X + 1, item.process.Location.Y);
                }

                else if ((item.process.Location.X >= 235 && item.process.Location.X <= 390) && (item.process.Location.Y >= 210 && item.process.Location.Y <= 235) && item.pc_pai == 3)      //canto direito
                {
                     item.process.Location = new Point(item.process.Location.X - 1, item.process.Location.Y);
                }

                else if (item.process.Location.Y <= 200)                                 //chegou na posição do servidor
                {
                    if (item.processID == 0)
                    {
                        item.processID = genId;
                        genId += 1;
                    }

                    if (left_printer_busy == false && item.printer == 2 && check_minimum_waiting_process(item) == item.processID)                              //se a impressora da esquerda estiver livre, este processo será atribuido a ela desde que ele não esteja atribuido já
                    {
                        item.printer = 0;
                        left_printer_busy = true;                               //e então a impressora será marcada como ocupada
                        item.process.Location = new Point(240, 135);
                        item.motionDisabled = false;
                        pictureBox7.Image = Simulacao.Properties.Resources.Error__2_;
                        waiting_process.Remove(item);

                        if (numberOfProcesswainting > 0)
                        {
                          numberOfProcesswainting -= 1;
                        }

                    }
                    else if (right_printer_busy == false && item.printer == 2 && check_minimum_waiting_process(item) == item.processID)
                    {
                        item.printer = 1;
                        right_printer_busy = true;
                        pictureBox8.Image = Simulacao.Properties.Resources.Error__2_;
                        item.process.Location = new Point(240,135);
                        item.motionDisabled = false;
                       
                        waiting_process.Remove(item);

                        if (numberOfProcesswainting > 0)
                        {
                            numberOfProcesswainting -= 1;
                        }
                    }
                    else if (left_printer_busy == true && right_printer_busy == true && item.printer == 2 && (!waiting_process.Contains(item) || waiting_process.Count == 0) ) 
                    {
                        //se as duas impressoras estiverem sendo usadas, uma fila deve ser formada
                          item.process.Location = new Point(280 + 40*numberOfProcesswainting, 200);
                          item.motionDisabled = true;
                          numberOfProcesswainting += 1;
                          waiting_process.Add(item.DeepCopy());
                        
                    }

                    if (item.printer == 0) {                                                              //se a impressora é a da esquerda

                        if (item.process.Location.X < 75)                               //se chegou na impressora, desabilita o processo
                        {
                          
                            item.process.Visible = false;
                         //   list_of_process.Remove(item);
                            if(lprinter_time_control >= 4)                              //4 segundos é o tempo de impressão, depois disso ela esta liberada
                            {
                                left_printer_busy = false;
                                lprinter_time_control = 0;
                                item.removeItem = 1;
                                pictureBox7.Image = Simulacao.Properties.Resources.Accept__2_;
                            }

                        }
                        else {
                            item.process.Location = new Point(item.process.Location.X - 1, item.process.Location.Y);    //movimenta o processo até ele chegar na impressora
                        }
                        
                    }
                    else if (item.printer == 1)
                    {
                        if (item.process.Location.X >= 390)                         // se chegou na impressora da direita
                        {
                          
                            item.process.Visible = false;                           //desabilita o processo
               
                            if(rprinter_time_control >= 4)
                            {
                                right_printer_busy = false;
                                rprinter_time_control = 0;
                                item.removeItem = 1;
                                pictureBox8.Image = Simulacao.Properties.Resources.Accept__2_;
                            }
                        }

                        else
                        {
                            item.process.Location = new Point(item.process.Location.X + 1, item.process.Location.Y);         //movimenta o processo até ele chegar na impressora da direita                          
                        } 
                    }  
                }
               
                else if(item.motionDisabled == false)
                {
                    item.process.Location = new Point(item.process.Location.X, item.process.Location.Y - 1);
                }
                
            }

            finish_process();
       
        }

        private int check_minimum_waiting_process(Process p) 
        {

            //verifica qual processo de menor id que está na fila, esse será escolhido.

            int exclude;

            foreach (Process item in waiting_process)
            {

                if(item.processID < p.processID)        //se há um processo na fila de processos que estão esperando com id menor que aquele avaliado
                {
                    exclude = item.processID;
                    return exclude;
                }
            }

                return p.processID;                                   //caso não haja, este processo mesmo será impresso
            
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
           //controla o tempo de impressão

            if (left_printer_busy == true)
            {
                lprinter_time_control += 1;
            }
            if (right_printer_busy == true)
            {
                rprinter_time_control += 1;
            }

        }
    }
}
