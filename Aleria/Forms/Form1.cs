using Aleria.AleriaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aleria
{
    public partial class FormChat : Form
    {
        public FormChat()
        {
            InitializeComponent();
        }
        List<String> prevQuestion = new List<string>();
        ChatBot chatBot = new ChatBot();
        private void button1_Click(object sender, EventArgs e)
        {
            label2.Text = chatBot.AnswerMe(textBox1.Text, prevQuestion);
            if (chatBot.PrevQuestion != null)
            {
                prevQuestion = chatBot.PrevQuestion;
            }
            else
            {
                prevQuestion.Add(textBox1.Text);
            }
            textBox1.Text = "";
            textBox1.Focus();
        }

        private void FormChat_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Create Chatbot Object");
            File.AppendAllText("Log.txt", "Create Chatbot Object" + Environment.NewLine);
            Console.WriteLine("Chatbot BaseKnowledgeLocation :" + chatBot.BaseKnowledgeLocation );
            File.AppendAllText("Log.txt", "Chatbot BaseKnowledgeLocation :" + chatBot.BaseKnowledgeLocation + Environment.NewLine);
            Console.WriteLine("Chatbot Start ReadDataBaseKnowledge");
            File.AppendAllText("Log.txt", "Chatbot Start ReadDataBaseKnowledge" + Environment.NewLine);
            chatBot.ReadDataBaseKnowledge();      
        }
    }
}

