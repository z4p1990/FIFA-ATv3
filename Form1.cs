using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace FIFA_Anti_Trainer
{
    public partial class Form1 : Form
    {
        private const int WmNchittest = 0x84;
        private const int HtCaption = 0x2;
        private Timer _tm1;
        private Timer _tm2;
        private Timer _tm3;
        private Timer _tm4;


        public Form1()
        {
            InitializeComponent();
            button2.Click += fecharToolStripMenuItem_Click;
            Closed += Form1_FormClosed;
            Load += Form1_Load;
            button1.Enabled = false;
        }
        
        public void Tm1()
        {
            _tm1 = new Timer();
            _tm1.Tick += timer1_Tick;
            _tm1.Interval = 180000;
            _tm1.Start();
        }

        public void Tm2()
        {
            _tm2 = new Timer();
            _tm2.Tick += timer2_Tick;
            _tm2.Interval = 300;
            _tm2.Start();
        }
        public void Tm3()
        {
            _tm3 = new Timer();
            _tm3.Interval = 480002;
            _tm3.Tick += timer3_Tick;
            _tm3.Start();
        }

        public void Tm4()
        {
            _tm4 = new Timer();
            _tm4.Tick += timer4_Tick;
            _tm4.Interval = 500000;
            _tm4.Start();

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            ProcSusp(Process.GetProcesses());
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            VerificarBashShell(Process.GetCurrentProcess().SessionId);
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            var processes = Process.GetProcesses();
            EnviarEmailReport2("smtp.gmail.com", processes, textBox1.Text);
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            _tm3.Stop();
        }
        
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WmNchittest)
                m.Result = (IntPtr) HtCaption;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Tm1();
            Tm2();
            Tm3();
            Tm4();
            var currentUser = Process.GetCurrentProcess().SessionId;
            Process.GetProcesses();
            Process.GetProcessesByName("FIFA22").Where(p => p.SessionId.Equals(currentUser)).ToList()
                .ForEach(p => p.Kill());
            timer1.Start();
            label1.Text = DateTime.Now.ToLongTimeString();
            label2.Text = DateTime.Now.ToShortDateString();
        }

        private void Form1_FormClosed(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
               
            }
            else
            {
                var currentUser = Process.GetCurrentProcess().SessionId;
                var processes = Process.GetProcesses();
                var list = Process.GetProcessesByName("FIFA22").Where(p => p.SessionId.Equals(currentUser)).ToList();
                EnviarEmailReport("smtp.gmail.com", processes, textBox1.Text, false);
                list.ForEach(p => p.Kill());
            }
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var unused = Process.GetCurrentProcess().SessionId;
            EnviarEmailReport("smtp.gmail.com", Process.GetProcesses(), textBox1.Text, true);
            textBox1.Enabled = false;
            if (button1.Text == @"ATIVAR")
            {
                button1.Text = @"ATIVADO";
                button1.ForeColor = Color.SpringGreen;
                pictureBox1.Visible = true;
                pictureBox1.Refresh();
                pictureBox2.Visible = true;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;

            }
            else if (button1.Text == @"ATIVADO")
            {
                button1.Text = @"DESATIVADO";
                button1.ForeColor = Color.Red;
                pictureBox1.Visible = false;
                pictureBox1.Refresh();
                pictureBox2.Visible = false;
                label1.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
            }
            else if (button1.Text == @"DESATIVADO")
            {
                button1.Text = @"ATIVADO";
                button1.ForeColor = Color.SpringGreen;
                pictureBox1.Visible = true;
                pictureBox1.Refresh();
                pictureBox2.Visible = true;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
            }
        }


        public static void EnviarEmailReport(
            string server,
            Process[] processos,
            string nick,
            bool abertura)
        {
            var to = "anti.trainer.arena @gmail.com";
            var hostName = Dns.GetHostName();
            var userName = Environment.UserName;
            var str = !abertura ? "OFF" : "ON";
            var message = new MailMessage("anti.trainer.arena@gmail.com", to);
            message.Subject ="["+ str +"]" + " Jogador: " + nick;
            message.IsBodyHtml = true;
            message.Body += "<b>Jogador:</b> " + nick + "";
            var mailMessage1 = message;
            mailMessage1.Body = mailMessage1.Body + "<br> <b>Versao:</b> " + "v3.0.2.6";
            var mailMessage2 = message;
            mailMessage2.Body = mailMessage2.Body + "<br> <b>Hostname:</b> " + hostName;
            var mailMessage3 = message;
            mailMessage3.Body = mailMessage3.Body + "<br> <b>Usuario:</b> " + userName;
            message.Body += "<br><br> <b>Lista de Processos:</b>";
            foreach (var processo in processos)
                try
                {
                    var mailMessage5 = message;
                    if (processo.MainModule != null)
                        mailMessage5.Body = mailMessage5.Body + "<br /> - " + processo.ProcessName + " <b>ID:</b> " +
                                            processo.Id + " <b>Caminho:</b> " + processo.MainModule.FileName + " <br><b>Descricao:</b> " + processo.MainModule.FileVersionInfo.FileDescription + "<br>";
                }
                catch (Exception)
                {
                    /* You will get access denied exception for system processes, We are skiping the system processes here */
                }

            var smtpClient = new SmtpClient(server, 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("anti.trainer.arena@gmail.com", "Arena@2022");
            try
            {
                smtpClient.Send(message);
            }
            catch (Exception)
            {
                // ignored
            }
        }

         public static void EnviarEmailReport2(
            string server,
            Process[] processos,
            string nick)
        {
            var to = "anti.trainer.arena @gmail.com";
            var hostName = Dns.GetHostName();
            var userName = Environment.UserName;
            var message = new MailMessage("anti.trainer.arena@gmail.com", to);
            message.Subject ="[SCAN]" + " Jogador: " + nick;
            message.IsBodyHtml = true;
            message.Body += "<b>Jogador:</b> " + nick + "";
            var mailMessage1 = message;
            mailMessage1.Body = mailMessage1.Body + "<br> <b>Versao:</b> " + "v3.0.2.6";
            var mailMessage2 = message;
            mailMessage2.Body = mailMessage2.Body + "<br> <b>Hostname:</b> " + hostName;
            var mailMessage3 = message;
            mailMessage3.Body = mailMessage3.Body + "<br> <b>Usuario:</b> " + userName;
            message.Body += "<br><br> <b>Lista de Processos:</b>";
            foreach (var processo in processos)
                try
                {
                    var mailMessage5 = message;
                    if (processo.MainModule != null)
                        mailMessage5.Body = mailMessage5.Body + "<br /> - " + processo.ProcessName + " <b>ID:</b> " +
                                            processo.Id + " <b>Caminho:</b> " + processo.MainModule.FileName + " <br><b>Descricao:</b> " + processo.MainModule.FileVersionInfo.FileDescription + "<br>";
                }
                catch (Exception)
                {
                    /* You will get access denied exception for system processes, We are skiping the system processes here */
                }

            var smtpClient = new SmtpClient(server, 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("anti.trainer.arena@gmail.com", "Arena@2022");
            try
            {
                smtpClient.Send(message);
            }
            catch (Exception)
            {
                // ignored
            }
            
        }

        public static void EnviarEmailSuspeito(
            string server,
            Process[] processos,
            List<string> processosSuspeitos,
            string nick)
        {
            var to = "anti.trainer.arena@gmail.com";
            var hostName = Dns.GetHostName();
            var userName = Environment.UserName;
            var message = new MailMessage("anti.trainer.arena@gmail.com", to);
            message.Subject = "[DETECTADO] - Usuario: " + nick + "";
            message.IsBodyHtml = true;
            message.Body += "<b>Jogador:</b> " + nick + "";
            var mailMessage1 = message;
            mailMessage1.Body = mailMessage1.Body + "<br> <b>Hostname:</b> " + hostName;
            var mailMessage2 = message;
            mailMessage2.Body = mailMessage2.Body + "<br> <b>Usuario:</b> " + userName;
            message.Body += "<br> Processo Identificado: ";
            foreach (var processosSuspeito in processosSuspeitos)
            {
                var mailMessage5 = message;
                mailMessage5.Body = mailMessage5.Body + "<br /> Processo: " + processosSuspeito;
            }

            var smtpClient = new SmtpClient(server, 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("anti.trainer.arena@gmail.com", "Arena@2022");
            try
            {
                smtpClient.Send(message);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        
        private void ProcSusp(Process[] processos)
        {
            var stringList = new List<string>();
            string[] strArray =
            {
                "fifatrainer",
                "trainer",
                "fifa20trainer",
                "fifa21trainer",
                "fifa22trainer",
                "fifa19trainer",
                "cheatengine",
                "cheat",
                "dark",
                "darktrainer",
                "auto",
                "bot",
                "coins",
                "macro",
                "frosty",
                "xiter",
                "cheattable",
                "cheatable",
                "unknowncheats",
                "unknown",
                "finishshot",
                "timeshot"
            };
            foreach (var processo in processos)
            foreach (var str in strArray)
                if (processo.ProcessName.ToLower().Contains(str) && processo.ProcessName != "FIFA ATv3.0.2.5")
                    stringList.Add(processo.ProcessName);
            if (stringList.Any())
            {
                EnviarEmailSuspeito("smtp.gmail.com", processos, stringList, textBox1.Text);
            }
        }

        public void VerificarBashShell(int currentUser)
        {
            var processes = Process.GetProcesses();
            var unused = new List<string>();
            string[] strArray =
            {
                "bash",
                "shell",
                "prompt",
                "console",
                "taskmgr",
                "cmd",
                "procexp",
                "procexp64",
                "procexp64a"
            };
            foreach (var process in processes)
            foreach (var str in strArray)
                if (process.ProcessName.ToLower().Contains(str))
                    process.Kill();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var f2 = new AboutBox1();
            f2.ShowDialog();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToLongTimeString();
            timer1.Start();
        }

        private void fecharToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }
        }
    }
}