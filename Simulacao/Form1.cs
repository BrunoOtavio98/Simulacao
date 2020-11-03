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
using System.Numerics;

namespace Simulacao
{
    public partial class Form1 : Form
    {

        private GeradorAleatorio gen;
        private FileManipulation fm;                       

        private List<Process> list_of_process;              //guarda todos os processos gerados pelos computadores
        private List<Process> waiting_process;

        private double lambda = 1.0/12;                  //Parâmetro da distribuição de Poisson

        private int genId = 1;                              //Ordem que os processs chegam no servidor
        private int genPc = 0;                              //Ordem que os processos são gerados nos computadores

        /*                 (1)
         * Para indexar a fila que se formará
         * 
         */
        private int numberOfProcesswainting = 0;

        /*                 (2)
         * Para contar (timer 3) os quatro segundos de impressão 
         *  
         */
        private int lprinter_time_control = 0;             
        private int rprinter_time_control = 0;          
        
        /*                  (3)
         * A partir do momento que o servidor encaminha uma mensagem, a impressora correspodente fica ocupada
         * as duas variáveis abaixo, controlam a disponibilidade das impressoras
         */
        private bool left_printer_busy = false;             
        private bool right_printer_busy = false;

        /*                  (4)
         *Controla o momento que o processo chega na impressora
         * Para disparar a contagem de quatro segundos
         */
        private bool arrive_left = false;
        private bool arrive_right = false;

        /*                   (5)
         *Variáveis para gerar as estatisticas
         * 
         */
        private int lastIC = 0;                 //Para controlar o intervalo de chegada
        
        public Form1()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(f1_paint);
            gen = new GeradorAleatorio(2921256, Math.Pow(2, 31) - 1, 0, 1);

            list_of_process = new List<Process>();
            waiting_process = new List<Process>();
            fm = new FileManipulation();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void f1_paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black);
            
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

            process.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            process.TabIndex = 5;
            process.TabStop = false;

            if (pc == 1)
            {
                process.Location = new System.Drawing.Point(75, 255);
            }
            else if (pc == 2)
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
            
        /* Timer responsável pela geração dos processos. Quanto menor for seu tempo de interrupção, maior será o numero
         * processos gerados
         */
        private void timer1_Tick(object sender, EventArgs e)
        { 
            double next_pc;
            double poisson_compar;
            double gen_poisson = Poisson(genPc/123.0);
           
            poisson_compar = gen.rand();                               //numero para comparar com a distribuição de poisson

            if (gen_poisson >= poisson_compar)
            {
                next_pc = gen.rand();


                if (next_pc <= 0.33)           //se o numero gerado for menor ou igual a 33%
                {
                    //Entao um processo iniciando no computador da esquerda deve ser gerado
                    list_of_process.Add(assignProcess(1));
                }
                else if (next_pc > 0.33 && next_pc <= 0.66)
                {
                    //Caso o numero estiver entre 33% e 66%
                    //Será gerado um processo no computador do meio
                   
                    list_of_process.Add(assignProcess(2));
                }
                else
                {
                    //Caso contrário será gerado um processo no computador da direita
                    list_of_process.Add(assignProcess(3));
                }
            }
        }

        private Process assignProcess(int father)
        {

            Process p;
            TextBox text = new TextBox();

            p = new Process(generate_process(father), father);
            p.printer = 2;                                                //no inicio o processo não tem alguma impressora atribuida

            this.Controls.Add(text);
            p.localId = text;
            p.localId.Location = new Point(p.process.Location.X + 10, p.process.Location.Y);
            p.localId.Enabled = true;
            p.localId.ReadOnly = true;
            p.localId.Size = new Size(25, 15);
            p.localId.Visible = true;

            if (lastIC == 0)
            {
                p.IC = genPc;
                lastIC = genPc;
            }
            else
            {
                p.IC = genPc - lastIC;
                lastIC = genPc;
            }

            p.TC = genPc;
            return p;
        }

        /*
         * Timer responsável pela movimentação dos processos
         */
        private void timer2_Tick(object sender, EventArgs e)
        {
            
            int x;
            int y;

            foreach (Process item in list_of_process)
            {
                if (item.removeItem == 0)
                {

                    if ((item.process.Location.X >= 75 && item.process.Location.X <= 235) && (item.process.Location.Y >= 210 && item.process.Location.Y <= 235) && item.pc_pai == 1)        //canto esquerdo
                    {
                        item.process.Location = new Point(item.process.Location.X + 1, item.process.Location.Y);
                        item.localId.Location = new Point(item.process.Location.X, item.process.Location.Y - 30);
                    }

                    else if ((item.process.Location.X >= 235 && item.process.Location.X <= 390) && (item.process.Location.Y >= 210 && item.process.Location.Y <= 235) && item.pc_pai == 3)      //canto direito
                    {
                        item.process.Location = new Point(item.process.Location.X - 1, item.process.Location.Y);
                        item.localId.Location = new Point(item.process.Location.X, item.process.Location.Y - 30);
                    }

                    else if (item.process.Location.Y <= 200)                                 //chegou na posição do servidor
                    {
                        if (item.processID == 0)
                        {
                            item.processID = genId;
                            genId += 1;

                            item.localId.Text = item.processID.ToString();
                        }

                        if (left_printer_busy == false && item.printer == 2 && check_minimum_waiting_process(item) == item.processID)                              //se a impressora da esquerda estiver livre, este processo será atribuido a ela desde que ele não esteja atribuido já
                        {
                            item.printer = 0;
                            left_printer_busy = true;                               //e então a impressora será marcada como ocupada

                            item.process.Location = new Point(240, 135);
                            item.localId.Location = new Point(item.process.Location.X, item.process.Location.Y + 30);

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

                            item.process.Location = new Point(240, 135);
                            item.localId.Location = new Point(item.process.Location.X, item.process.Location.Y + 30);

                            item.motionDisabled = false;

                            waiting_process.Remove(item);

                            if (numberOfProcesswainting > 0)
                            {
                                numberOfProcesswainting -= 1;
                            }
                        }
                        else if (left_printer_busy == true && right_printer_busy == true && item.printer == 2 && (!waiting_process.Contains(item) || waiting_process.Count == 0))
                        {
                            //se as duas impressoras estiverem sendo usadas, uma fila deve ser formada
                            item.process.Location = new Point(280 + 40 * numberOfProcesswainting, 200);
                            item.localId.Location = new Point(item.process.Location.X, item.process.Location.Y + 30);

                            item.motionDisabled = true;

                            item.EF = genPc;                //Guardo o instante que esse processo entrou na fila

                            numberOfProcesswainting += 1;
                            waiting_process.Add(item.DeepCopy());
                        }

                        if (item.printer == 0)
                        {                                                              //se a impressora é a da esquerda
                            if (item.process.Location.X < 75)                          //se chegou na impressora, desabilita o processo
                            {
                                arrive_left = true;

                                if (item.IA == 0)
                                {
                                    item.IA = genPc;                                        //Momento que o processo chega na impressora é o momento que ele começa a ser atendido
                                }

                                item.process.Visible = false;
                                item.localId.Visible = false;
                                //   list_of_process.Remove(item);
                                if (lprinter_time_control >= 4)                              //4 segundos é o tempo de impressão, depois disso ela esta liberada
                                {
                                    arrive_left = false;
                                    left_printer_busy = false;

                                    item.TA = lprinter_time_control;
                                    item.FA = item.TA + item.IA;
                                    item.PS = item.FA - item.TC;

                                    lprinter_time_control = 0;
                                    item.removeItem = 1;
                                    pictureBox7.Image = Simulacao.Properties.Resources.Accept__2_;
                                }
                            }
                            else
                            {
                                if (item.EF > 0 && item.exitPrinter == false)                                        //Caso o processo tenha entrado na fila
                                {
                                    item.exitPrinter = true;
                                    item.EF = genPc - item.EF;                          //Subtraio do instante atual o momento que ele entrou na fila 
                                }

                                item.process.Location = new Point(item.process.Location.X - 1, item.process.Location.Y);    //movimenta o processo até ele chegar na impressora
                                item.localId.Location = new Point(item.process.Location.X, item.process.Location.Y + 30);
                            }
                        }
                        else if (item.printer == 1)
                        {
                            if (item.process.Location.X >= 390)                         // se chegou na impressora da direita
                            {
                                arrive_right = true;

                                if (item.IA == 0)
                                {
                                    item.IA = genPc;                                        //Momento em que o processo começa a ser atendido na impressora               
                                }

                                item.process.Visible = false;                           //desabilita o processo
                                item.localId.Visible = false;

                                if (rprinter_time_control >= 4)
                                {
                                    arrive_right = false;
                                    right_printer_busy = false;

                                    item.TA = rprinter_time_control;
                                    item.FA = item.TA + item.IA;
                                    item.PS = item.FA - item.TC;

                                    rprinter_time_control = 0;
                                    item.removeItem = 1;
                                    pictureBox8.Image = Simulacao.Properties.Resources.Accept__2_;
                                }
                            }
                            else
                            {
                                if (item.EF > 0 && item.exitPrinter == false)                                        //Caso o processo tenha entrado na fila
                                {
                                    item.exitPrinter = true;
                                    item.EF = genPc - item.EF;                          //Subtraio do instante atual o momento que ele entrou na fila 
                                }

                                item.process.Location = new Point(item.process.Location.X + 1, item.process.Location.Y);         //movimenta o processo até ele chegar na impressora da direita
                                item.localId.Location = new Point(item.process.Location.X, item.process.Location.Y + 30);
                            }
                        }
                    }

                    else if (item.motionDisabled == false)
                    {
                        item.process.Location = new Point(item.process.Location.X, item.process.Location.Y - 1);
                        item.localId.Location = new Point(item.process.Location.X + 30, item.process.Location.Y);
                    }
                }
            }
     //       finish_process();
        }

        /*  Controla o tempo de impressão
         *  
         */
        private void timer3_Tick(object sender, EventArgs e)
        {

            genPc += 1;

            if (arrive_left == true)
            { 
                lprinter_time_control += 1;
            }
            if (arrive_right == true)
            {
                rprinter_time_control += 1;
            }
        }

        /* Verifica qual processo de menor id que está na fila, esse será escolhido
         * 
         */
        private int check_minimum_waiting_process(Process p)
        {
            
            int minimum_process;

            foreach (Process item in waiting_process)
            {
                if (item.processID < p.processID)        //se há um processo na fila de processos que estão esperando com id menor que aquele avaliado
                {
                    minimum_process = item.processID;
                    return minimum_process;
                }
            }
            return p.processID;                                   //caso não haja, este processo mesmo será impresso
        }

        private double Factorial(int number)
        {

            double to_return = 1;
            
            for (int i = number; i > 0; i--)
                to_return *= i;

            return to_return;
        }

        private double Poisson(double k)
        {
            double numerador;
            double denumerador;

            /* De acordo com a distribuição feita previamente em MATLAB, se k > 40, a probabilidade estará muito            
             * próximo de 0 (0.0002535) então para ganhar tempo, retornamos 0, pois é estatisticamente improvavel que um
             * processo com essa probabilidade seja gerado.
             */
            if ((int)k < 40) {                         
                numerador = Math.Pow(Math.Exp(1), -lambda) * Math.Pow(lambda, k);
                denumerador = Factorial((int)k);

                return (numerador / denumerador);
            }
            else
                return 0;
        }

        /*
         * Timer que interrompe o programa e salva os resultados nos arquivos
         */
        private void timer4_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer2.Enabled = false;

            if (left_printer_busy == false && right_printer_busy == false)
            {

                timer4.Enabled = false;

                List<string> IC = new List<string>();
                List<string> TA = new List<string>();
                List<string> TC = new List<string>();
                List<string> IA = new List<string>();
                List<string> FA = new List<string>();
                List<string> FA1 = new List<string>();
                List<string> FA2 = new List<string>();
                List<string> EF = new List<string>();
                List<string> PS = new List<string>();
                List<string> TO = new List<string>();

                Process LastProcessR = null;
                Process LastProcessL = null;

              
                foreach (Process item in list_of_process)
                {

                    if (item.processID == 1 || item.processID == 2)
                    {
                        item.FA1 = item.FA;
                        item.FA2 = item.FA;

                        item.TO = item.IA;

                        if(item.printer == 0)
                        {
                            LastProcessL = item.DeepCopy();
                        }
                        else
                        {
                            LastProcessR = item.DeepCopy();
                        }
                    }
                    else if (item.processID > 2)
                    {

                        if(item.printer == 0)
                        {
                            if (LastProcessL.FA1 <= LastProcessL.FA2)
                            {
                                item.FA1 = item.FA;
                                item.FA2 = LastProcessL.FA1;
                            }
                            else
                            {
                                item.FA1 = LastProcessL.FA1;
                                item.FA2 = item.FA;
                            }

                            if (item.FA == item.FA1)
                            {
                                item.TO = item.IA - LastProcessL.FA1;
                            }
                            else
                            {
                                item.TO = item.IA - LastProcessL.FA2;
                            }

                            LastProcessL = item.DeepCopy();
                        }
                        else
                        {
                            if (LastProcessR.FA1 <= LastProcessR.FA2)
                            {
                                item.FA1 = item.FA;
                                item.FA2 = LastProcessR.FA1;
                            }
                            else
                            {
                                item.FA1 = LastProcessR.FA1;
                                item.FA2 = item.FA;
                            }

                            if (item.FA == item.FA1)
                            {
                                item.TO = item.IA - LastProcessR.FA1;
                            }
                            else
                            {
                                item.TO = item.IA - LastProcessR.FA2;
                            }

                            LastProcessR = item.DeepCopy();
                        }
                    }

                    if (item.processID > 0)                             //Se o programa interrompeu antes de um processo receber seu ID, tal processo será descartado
                    {
                        IC.Add(item.processID.ToString() + " -> " + item.IC.ToString());
                        TA.Add(item.processID.ToString() + " -> " + item.TA.ToString());
                        TC.Add(item.processID.ToString() + " -> " + item.TC.ToString());
                        IA.Add(item.processID.ToString() + " -> " + item.IA.ToString());
                        FA.Add(item.processID.ToString() + " -> " + item.FA.ToString());
                        FA1.Add(item.processID.ToString() + " -> " + item.FA1.ToString());
                        FA2.Add(item.processID.ToString() + " -> " + item.FA2.ToString());
                        EF.Add(item.processID.ToString() + " -> " + item.EF.ToString());
                        PS.Add(item.processID.ToString() + " -> " + item.PS.ToString());
                        TO.Add(item.processID.ToString() + " -> " + item.TO.ToString());
                    }
                }
               
                fm.WriteFile(IC, "IC");
                fm.WriteFile(TA, "TA");
                fm.WriteFile(TC, "TC");
                fm.WriteFile(IA, "IA");
                fm.WriteFile(FA, "FA");
                fm.WriteFile(FA1, "FA1");
                fm.WriteFile(FA2, "FA2");
                fm.WriteFile(EF, "EF");
                fm.WriteFile(PS, "PS");
                fm.WriteFile(TO, "TO");

                MessageBox.Show("Acesse os resultados em: " + AppDomain.CurrentDomain.BaseDirectory + "Dados", "Programa terminado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
                timer2.Enabled = true;
        }

        private void accept_Click(object sender, EventArgs e)
        {

            if(textBox1.TextLength > 0)
            {
                timer4.Interval = int.Parse(textBox1.Text)*1000;
                textBox1.Text = "\0";
                timer1.Enabled = true;
                timer3.Enabled = true;
                timer4.Enabled = true;
                timer2.Enabled = true;
                
            }
        }
    }
}
