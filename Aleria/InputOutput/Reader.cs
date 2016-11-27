using Aleria.Holder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Aleria.InputOutput
{
    class Reader
    {
        public static WordMemoryBot[] ReadChatDataFromFile(String data)
        {
            String response = "";
            response = File.ReadAllText(data);

            MatchCollection mathcer = Regex.Matches(response, "<bft>.*</bft>");
            List<WordMemoryBot> wordText = new List<WordMemoryBot>();
            foreach (Match m in mathcer)
            {
                Match ma = Regex.Match(m.Value, "<qt>.*</qt>");
                String temp = ma.Value;
                temp = temp.Replace("<qt>", "");
                temp = temp.Replace("</qt>", "");
                String textQuestion = temp;

                ma = Regex.Match(m.Value, "<ans>.*</ans>");
                temp = ma.Value;
                temp = temp.Replace("<ans>", "");
                temp = temp.Replace("</ans>", "");
                String[] SplitString = { "<i>" };
                String[] textAnswer = new String[100];
                if (temp.Contains("<i>"))
                {
                    textAnswer = temp.Split(SplitString, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    textAnswer[0] = temp;
                }

                String[] textPrevQuest = new String[100];
                ma = Regex.Match(m.Value, "<pq>.*</pq>");
                if (ma.Success)
                {
                    temp = ma.Value;
                    temp = temp.Replace("<pq>", "");
                    temp = temp.Replace("</pq>", "");
                    if (temp.Contains("<i>"))
                    {
                        String[] SpiltString = { "<i>" };
                        textPrevQuest = temp.Split(SpiltString, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                    {
                        textPrevQuest[0] = temp;
                    }
                }
                else
                {
                    textPrevQuest = null;
                }

                WordMemoryBot dataWordText = new WordMemoryBot();
                dataWordText.QuestionText = textQuestion;
                dataWordText.AnswerText = RemoveNullArray(textAnswer);
                dataWordText.PrevQuestion = RemoveNullArray(textPrevQuest);
                wordText.Add(dataWordText);
            }

            return wordText.ToArray();
        }

        private static int GetSizeRemoveNull(String[] data)
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

        private static String[] RemoveNullArray(String[] data)
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
