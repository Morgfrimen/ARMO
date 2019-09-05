using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;


namespace ТЗ_АРМО
{
    public partial class Form1 : Form
    {
        private List<Thread> threads = new List<Thread>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] vs = Bin.CheckNullXml();
            if (vs != null)
            {
                StartDirectory.Text = vs[0];
                textBox1.Text = vs[1];
                textBox2.Text = vs[2];
            }
        }

        [Obsolete]
        private void Run_Click(object sender, EventArgs e)
        {
            //Task task = Task.Factory.StartNew(()=> Bin.SearchAndTreeViewAdd(StartDirectory.Text,textBox1.Text,textBox2.Text));
            if (threads.Count == 0)
            {
                treeView1.Nodes.Clear();
                //Thread parrents = Thread.CurrentThread;
                Thread thread = new Thread(() =>
                {
                    Bin.SearchAndTreeViewAdd(StartDirectory.Text, textBox1.Text, textBox2.Text);
                });
                thread.Name = "Search";
                thread.Priority = ThreadPriority.Highest;
                threads.Add(thread);
                thread.Start();
                

            }
            else
            {
                if (((Thread)threads.First()).IsAlive && ((Thread)threads.First()).ThreadState==System.Threading.ThreadState.Suspended)
                {
                    ((Thread)threads.First()).Resume();
                }
                else
                {
                    Console.Text = "Поток закрывается...";
                    ((Thread)threads.First()).Abort();
                    threads.Clear();
                    treeView1.Nodes.Clear();
                }
            }
        }

        [Obsolete]
        private void Pause_Click(object sender, EventArgs e)
        {
            if (threads.Count == 0)
            {
                Console.Text = "Поиск не был произведен, нечего приостонавливать";
                return;
            }
            Thread thread = (Thread)threads.First();
            if (((Thread)threads.First()).Name== "Search")
            {
                if (thread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    Console.Text = "Поток закрыт";
                    threads.Clear();
                }
                else
                {
                    thread.Suspend();
                    Console.Text = "Поток поиска приостоновлен";
                    Run.Text = "Продолжить";
                    Run.Width += 50;
                }
            }
            

        }

        [Obsolete]
        private void Stop_Click(object sender, EventArgs e)
        {
            if (threads.Count == 0)
            {
                Console.Text = "Поток не был запущен";
                return;
            }
            if (threads.First().IsAlive && threads.First().ThreadState!=System.Threading.ThreadState.Suspended)
            {
                threads.First().Abort();
                threads.Clear();
                Run.Text = "Начать";
                Run.Width -= 50;
                Console.Text = "Поток остановлен";
            }
            else if(threads.First().IsAlive & threads.First().ThreadState == System.Threading.ThreadState.Suspended)
            {
                threads.First().Resume();
                threads.First().Abort();
                threads.Clear();
                Run.Text = "Начать";
                Run.Width -= 50;
                Console.Text = "Поток остановлен";
            }
            else
            {
                Console.Text = "Поток остановлен";
            }
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Bin.XML(StartDirectory.Text, textBox1.Text, textBox2.Text);
        }
    }
}


