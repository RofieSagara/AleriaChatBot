using Bioskop.Holder;
using Bioskop.InputOutput;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Bioskop.BioksopCore
{
    public class CoreBioskop
    {
        public String PathKnowledge { get; set; }
        private CommandMemoryBot[] DataKnowledge { get; set; }
        public CommandMemoryBot WinnerMemory { get; set; }
        public List<String> PrevQuestion { get; set; }
        private bool IsAvaiable { get; set; }
        public bool Success { get; set; }

        public CoreBioskop(String Path)
        {
            this.PathKnowledge = Path;
            DataKnowledge = Reader.ReadChatDataFromFile(Path);
            WinnerMemory = new CommandMemoryBot();
            Success = false;
        }

        public void IsCommand(String word, List<String> prevQuestion)
        {
            PrevQuestion = prevQuestion;
            IsAvaiable = false;
            String wordWoSpace = word.Replace(" ", "");
            List<CommandMemoryBot> commdMemBotWinner = new List<CommandMemoryBot>();
            bool isFound = false;
            foreach (CommandMemoryBot m in DataKnowledge)
            {
                String qstWoSpace = m.QuestionText.Replace(" ", "");
                //Espace Params di taruh di sini.
                Match match = Regex.Match(wordWoSpace, qstWoSpace);
                if (match.Success)
                {
                    commdMemBotWinner.Add(m);
                    isFound = true;
                }
            }

            if (isFound)
            {
                List<CommandMemoryBot> listWordWithPQ = new List<CommandMemoryBot>();
                List<CommandMemoryBot> listWordWithOutPQ = new List<CommandMemoryBot>();
                foreach (CommandMemoryBot m in commdMemBotWinner)
                {
                    if (m.PrevQuestion != null)
                    {
                        listWordWithPQ.Add(m);
                    }
                }

                foreach (CommandMemoryBot m in commdMemBotWinner)
                {
                    if (m.PrevQuestion == null)
                    {
                        listWordWithOutPQ.Add(m);
                    }
                }

                foreach (CommandMemoryBot m in listWordWithPQ)
                {
                    String[] pq = prevQuestion.ToArray();
                    if (m.AnswerText != null && (m.PrevQuestion.Count() == pq.Count()))
                    {
                        if (m.PrevQuestion.Count() > 1)
                        {
                            var intersetMprev = m.PrevQuestion.Intersect(pq);
                            var intersetPQprev = pq.Intersect(m.PrevQuestion);
                            int mtopq = intersetMprev.Count();
                            int pqtom = intersetPQprev.Count();

                            double valuePrev = (
                                Convert.ToDouble(mtopq) + Convert.ToDouble(pqtom)) /
                                (Convert.ToDouble(m.PrevQuestion.Length) + Convert.ToDouble(pq.Length));

                            if (valuePrev == 1.0)
                            {
                                PrevQuestion.Add(m.QuestionText);
                                WinnerMemory = m;
                                IsAvaiable = true;
                                Success = true;
                                break;
                            }

                        }
                        else if (m.AnswerText != null && m.PrevQuestion[0].Equals(pq[0]))
                        {
                            PrevQuestion.Add(m.QuestionText);
                            WinnerMemory = m;
                            IsAvaiable = true;
                            Success = true;
                            break;
                        }
                    }
                }

                if (!IsAvaiable)
                {
                    foreach (CommandMemoryBot m in listWordWithOutPQ)
                    {
                        if (m.AnswerText != null && m.AnswerText[0].Contains("<qta>") && m.AnswerText[0].Contains("</qta>"))
                        {
                            String temp = m.AnswerText[0];
                            temp = temp.Replace("<qta>", "");
                            temp = temp.Replace("</qta>", "");
                            IsAvaiable = false;
                            IsCommand(temp, PrevQuestion);
                        }
                        else if (m.AnswerText != null && m.PrevQuestion == null)
                        {
                            PrevQuestion.Clear();
                            PrevQuestion.Add(m.QuestionText);
                            WinnerMemory = m;
                            Success = true;
                            IsAvaiable = true;
                        }
                    }
                }
            }
        }

        private int GetSizeRemoveNull(String[] data)
        {
            int i = 0;
            while (i <= data.Length - 1)
            {
                if (data[i] != null)
                {
                    i++;
                }
                else
                {
                    break;
                }
            }
            return i;
        }

        private String[] RemoveNullArray(String[] data)
        {
            if (data != null)
            {
                int index = GetSizeRemoveNull(data);
                String[] temp = new String[index];
                int i = 0;
                while (i <= data.Length - 1)
                {
                    if (data[i] != null)
                    {
                        temp[i] = data[i];
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                return temp;
            }
            return null;
        }
    }
}
