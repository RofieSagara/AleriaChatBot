using Aleria.Excp;
using Aleria.Holder;
using Aleria.InputOutput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bioskop.BioksopCore;
using Bioskop.Holder;

namespace Aleria.AleriaCore
{
    class ChatBot
    {
        Random rand = new Random();

        public List<String> PrevQuestion { get; set; }

        public DirectoryInfo BaseKnowledgeLocation { get; set; }

        private WordMemoryBot[] WordBot { get; set; }

        private bool IsAvaiable { get; set; }

        private String Answer { get; set; }

        public ChatBot()
        {
            PrevQuestion = new List<String>();
            BaseKnowledgeLocation = new DirectoryInfo(Environment.CurrentDirectory + @"\Data");
        }

        public void ReadDataBaseKnowledge()
        {
            if (BaseKnowledgeLocation != null)
            {
                Console.WriteLine("Log: BaseKnowledgeLocation not null");
                //File.AppendAllText("Log.txt", "Log: BaseKnowledgeLocation not null"+Environment.NewLine);
                List<WordMemoryBot[]> wordBotList = new List<WordMemoryBot[]>();
                Console.WriteLine("Log: BaseKnowledgeLocation.GetFiles");
                //File.AppendAllText("Log.txt", "Log: BaseKnowledgeLocation.GetFiles" + Environment.NewLine);
                FileInfo[] baseKnowledgeFile = BaseKnowledgeLocation.GetFiles("*.bft");
                if (baseKnowledgeFile.Length > 0)
                {
                    foreach (FileInfo file in baseKnowledgeFile)
                    {
                        WordMemoryBot[] temp = Reader.ReadChatDataFromFile(file.FullName);
                        wordBotList.Add(temp);
                        Console.WriteLine("Log: ReadBaseKnowledgeFile " + file.FullName);
                        //File.AppendAllText("Log.txt", "Log: ReadBaseKnowledgeFile " + file.FullName + Environment.NewLine);
                    }
                    int sizeTotal = 0;
                    foreach (WordMemoryBot[] v in wordBotList)
                    {
                        sizeTotal = sizeTotal + v.Length;
                    }
                    WordMemoryBot[] wordBot = new WordMemoryBot[sizeTotal];
                    int lastIndex = 0;
                    foreach (WordMemoryBot[] v in wordBotList)
                    {
                        Array.Copy(v, 0, wordBot, lastIndex, v.Length);
                        lastIndex = lastIndex + v.Length;
                    }
                    WordBot = wordBot;
                }
                else
                {
                    throw new FileBaseKnowledgeNotFound("Tidak dapat menemukan File berformat .bft, Pastikan format file .bft sudah benar!");
                }
            }
            else
            {
                throw new DirectoryBaseKnowledgeNotFound("BaseKnowledgeLocation = null, Perhatikan lokasi folder dengan benar!");
            }
        }

        public String AnswerMe(String word, List<String> prevQuest)       
        {

            //WordMemoryBot[] wordCommend = InputOutputClass.ReadChatDataFromFile(CommendType.CommandBioskop);
            String path = Environment.CurrentDirectory + @"\Command\Bioskop.bft";
            CoreBioskop core = new CoreBioskop(path);
            core.IsCommand(word, prevQuest);
            if (core.Success)
            {
                CommandMemoryBot commnd = core.WinnerMemory;
            }
            
            Console.WriteLine("Log: AnswerMe Word : " + word);
            IsAvaiable = false;
            Answer = "Sepertinya aku tidak mengerti.";
            String[] textToArrayInputString = ConvertStringToStringArray(word);
            int i = 0;
            double value = 0;
            String[] winner = null;
            WordMemoryBot wordBotWinner = new WordMemoryBot();
            List<WordMemoryBot> listWordBotWinner = new List<WordMemoryBot>();
            Console.WriteLine("Log: =================FindAnswer====================");
            while (i <= WordBot.Length - 1)
            {
                String[] textToArrayQuestString = ConvertStringToStringArray(WordBot[i].QuestionText);
                var intersectb1tob2 = textToArrayInputString.Intersect(textToArrayQuestString);
                var intersectb2tob1 = textToArrayQuestString.Intersect(textToArrayInputString);
                int b1tob2 = intersectb1tob2.Count();
                int b2tob1 = intersectb2tob1.Count();

                double t = (Convert.ToDouble(b1tob2) + Convert.ToDouble(b2tob1)) / (Convert.ToDouble(textToArrayInputString.Length) + Convert.ToDouble(textToArrayQuestString.Length));
                Console.WriteLine("Log: "+WordBot[i].QuestionText+" >< "+word +" = "+t);
                Console.WriteLine("Log: (" + Convert.ToDouble(b1tob2) + " + " + Convert.ToDouble(b2tob1) + ") / (" + Convert.ToDouble(textToArrayInputString.Length) +" + "+ Convert.ToDouble(textToArrayQuestString.Length) + ") = " + t);
                if (t == value)
                {
                    value = t;                                                 
                    winner = textToArrayQuestString;       
                    wordBotWinner = WordBot[i];
                    listWordBotWinner.Add(wordBotWinner);
                }
                else if (t > value)
                {
                    value = t;
                    winner = textToArrayQuestString;
                    wordBotWinner = WordBot[i];
                    listWordBotWinner.Clear();
                    listWordBotWinner.Add(wordBotWinner);
                    Console.WriteLine("Log: THE WINNER " + wordBotWinner.QuestionText + " >< " + word + " = " + t +" ==========WINNER==========");
                }
                i++;
            }

            if (value > 0)
            {
                List<WordMemoryBot> listWordWithPQ = new List<WordMemoryBot>();
                List<WordMemoryBot> listWordWithOutPQ = new List<WordMemoryBot>();
                foreach (WordMemoryBot a in listWordBotWinner)
                {
                    if (a.PrevQuestion != null)
                    {
                        listWordWithPQ.Add(a);
                    }
                }

                foreach (WordMemoryBot a in listWordBotWinner)
                {
                    if (a.PrevQuestion == null)
                    {
                        listWordWithOutPQ.Add(a);
                    }
                }

                foreach (WordMemoryBot m in listWordWithPQ)
                {
                    String[] ab = PrevQuestion.ToArray();
                    if (m.AnswerText != null && (m.PrevQuestion.Count() == ab.Count()))
                    {
                        if (m.PrevQuestion.Count() > 1)
                        {
                            var intersetMPrev = m.PrevQuestion.Intersect(ab);
                            var intersetAbPrev = ab.Intersect(m.PrevQuestion);
                            int mToab = intersetMPrev.Count();
                            int abTom = intersetAbPrev.Count();

                            double valuePrev = (Convert.ToDouble(mToab) + Convert.ToDouble(abTom)) / (Convert.ToDouble(m.PrevQuestion.Length) + Convert.ToDouble(ab.Length));
                            if (valuePrev == 1.0)
                            {
                                Answer = SelectAnswer(m);
                                PrevQuestion.Add(m.QuestionText);
                                IsAvaiable = true;
                                break;
                            }
                        }
                        else if (m.AnswerText != null && m.PrevQuestion[0].Equals(ab[0]))
                        {
                            Answer = SelectAnswer(m);
                            PrevQuestion.Add(m.QuestionText);
                            IsAvaiable = true;
                            break;
                        }
                    }
                }

                if (!IsAvaiable)
                {
                    foreach (WordMemoryBot s in listWordWithOutPQ)
                    {
                        if (s.AnswerText != null && s.AnswerText[0].Contains("<qta>") && s.AnswerText[0].Contains("</qta>"))
                        {
                            String temp = s.AnswerText[0];
                            temp = temp.Replace("<qta>", "");
                            temp = temp.Replace("</qta>", "");
                            IsAvaiable = false;
                            return AnswerMe(temp, PrevQuestion);
                        }
                        else if (s.AnswerText != null && s.PrevQuestion == null)
                        {
                            Answer = SelectAnswer(s);
                            PrevQuestion.Clear();
                            PrevQuestion.Add(s.QuestionText);
                            IsAvaiable = true;
                        }
                    }
                }
            }
            else
            {
                return Answer;
            }

            return Answer;
        }

        private String SelectAnswer(WordMemoryBot dataWinner)
        {
            String[] dataAnswer = dataWinner.AnswerText;
            String resAnswer = "";
            if (dataAnswer.Length > 1)
            {
                int i = rand.Next(0, dataAnswer.Length);
                resAnswer = dataAnswer[i];
            }
            else
            {
                resAnswer = dataAnswer[0];
            }
            return resAnswer;
        }

        private String[] ConvertStringToStringArray(String data)
        {
            char[] textCharArray = data.ToCharArray();
            int j = 0;
            int index = 0;
            String[] textStringArray = new String[1000];
            while (j <= textCharArray.Length - 1)
            {
                if (j != textCharArray.Length - 1)
                {
                    textStringArray[index] = textCharArray[j].ToString() + textCharArray[j+1].ToString();
                }
                else if (j == textCharArray.Length - 1)
                {
                    textStringArray[index] = textCharArray[j].ToString();
                }
                j++;
                index++;
            }
            return RemoveNullArray(textStringArray);
        }

        private int GetSizeRemoveNull(String[] data)
        {
            int i = 0;
            while (data[i] != null)
            {
                i++;
            }
            return i;
        }

        private String[] RemoveNullArray(String[] data)
        {
            int index = GetSizeRemoveNull(data);
            String[] temp = new String[index];
            int i = 0;
            while (data[i] != null)
            {
                temp[i] = data[i];
                i++;
            }
            return temp;
        }
    }
}
