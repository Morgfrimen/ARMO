using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;

namespace ТЗ_АРМО
{
    public static class Bin
   {
        private static void BeginInvoker(Action action)
        {
            Form1.ActiveForm.BeginInvoke(action);
        }
        private static void Invoker(Action action)
        {
            Form1.ActiveForm.Invoke(action);
        }

        /// <summary>
        /// Создание XML файла
        /// </summary>
        private static void XmlCreate(string startDitectory,string patternText,string text)
        {
            XDocument xdoc = new XDocument();

            XElement _search = new XElement("HistorySearch");

            XAttribute _searchNameAttr = new XAttribute("name", "SearchLoad");
            XElement str1= new XElement("StartDirectory", startDitectory);
            XElement str2 = new XElement("PatternName", patternText);
            XElement str3 = new XElement("PatternText", text);

            _search.Add(_searchNameAttr);
            _search.Add(str1);
            _search.Add(str2);
            _search.Add(str3);

            xdoc.Add(_search);

            xdoc.Save("HistorySearch.xml");
        }
        /// <summary>
        /// Метод для поиска и созадния массива строк из файла XML
        /// </summary>
        /// <returns>Возвращает null, если файла нет, иначе мамссив строк</returns>
        public static string[] CheckNullXml()
        {
            if(File.Exists(Environment.CurrentDirectory+ @"\HistorySearch.xml"))
            {
                string[] atributeXml = new string[3];
                XDocument xDocument = XDocument.Load("HistorySearch.xml");

                XElement elementPatents = xDocument.Element("HistorySearch");
                XElement strElement1 = elementPatents.Elements().First();
                XElement strElement2 = elementPatents.Elements().ElementAt(1);
                XElement strElement3 = elementPatents.Elements().Last();

                if (strElement1.Value != null && strElement2.Value != null && strElement3.Value != null)
                {
                    atributeXml[0] = strElement1.Value;
                    atributeXml[1] = strElement2.Value;
                    atributeXml[2] = strElement3.Value;
                    return atributeXml;
                }
                return null;
            }
            return null;

        }
        /// <summary>
        /// Если XML файл есть, то редактирует его, иначе создаст XML с этими параметрами
        /// </summary>
        /// <param name="startDitectory">Стартовая директория</param>
        /// <param name="patternText">Шаблон имени файла</param>
        /// <param name="text">Шаблон поиска текста в файле</param>
        public static void XML(string startDitectory, string patternText, string text)
        {
            if (File.Exists(Environment.CurrentDirectory + @"\HistorySearch.xml"))
            {
                XDocument xDocument = XDocument.Load("HistorySearch.xml");

                XElement nameAttribute = xDocument.Element("HistorySearch");
                XElement strElement1 = nameAttribute.Element("StartDirectory");
                XElement strElement2 = nameAttribute.Element("PatternName");
                XElement strElement3 = nameAttribute.Element("PatternText");

                strElement1.Value = startDitectory;
                strElement2.Value = patternText;
                strElement3.Value = text;

                xDocument.Save("HistorySearch.xml");
            }
            else
            {
                XmlCreate(startDitectory, patternText, text);
            }
        }

        /// <summary>
        /// Метод начинает поиск,создает дерево и редактирует его
        /// </summary>
        /// <param name="directory">Директория, в которой нужно искать</param>
        /// <param name="patternName">Шаблон имени текстового файла</param>
        /// <param name="patternText">Шаблон текста в файле</param>
        [Obsolete]
        public static void SearchAndTreeViewAdd(string directory,string patternName,string patternText)
        {
            Form1.ActiveForm.BeginInvoke(new Action(()=> 
            { 
                Label label = (Label)Form1.ActiveForm.Controls.Find("Console", false).First();
                label.Text = "Запуск потока поиска";
                TreeView treeView = (TreeView)Form1.ActiveForm.Controls.Find("treeView1", false).First();
                treeView.Nodes.Add(directory);
                
            }));
            Search(directory, patternName, patternText);
        }

        /// <summary>
        /// Алгоритм поиска
        /// </summary>
        /// <param name="directory">Директория </param>
        /// <param name="pateetnName">Шаблон названия файла</param>
        /// <param name="patternText">шаблон текста</param>
        [Obsolete]
        private static void Search(string directory,string pateetnName,string patternText)
        {
            DateTime time = DateTime.Now;
            Regex fileName = new Regex($@"{pateetnName}.*\.txt$");
            Regex fileText = new Regex($@".*{patternText}.*");
            int shet = 0;
            foreach (string item in Directory.GetFiles(directory))
            {                 
                MatchCollection mathFileSearch = fileName.Matches(item);
                if (mathFileSearch.Count > 0)
                {
                    StreamReader reader = new StreamReader(item,System.Text.Encoding.Default);
                    MatchCollection mathFileText = fileText.Matches(reader.ReadToEnd());
                    reader.Close();
                    if (mathFileText.Count > 0)
                    {
                        shet += 1;
                        for (int i = 0; i < mathFileSearch.Count; i++)
                        {
                            Invoker(() =>
                            {
                                //((TreeView)Form1.ActiveForm.Controls.Find("treeView1", false).First()).Nodes[0].Expand();
                                ((TreeView)Form1.ActiveForm.Controls.Find("treeView1", false).First()).Nodes[0].Nodes.Add(mathFileSearch[i].Value);
                                Label label = (Label)Form1.ActiveForm.Controls.Find("Console", false).First();
                                label.Text = $"Всего подходящих файло:{shet} {mathFileSearch[i].Value}";
                            });
                            if (i == 0)
                            {
                                Invoker(() =>
                                {
                                    ((TreeView)Form1.ActiveForm.Controls.Find("treeView1", false).First()).Nodes[0].Expand();
                                });
                            }
                        }
                        
                        
                    }
                }
                else
                {
                    Invoker(() => 
                    {
                        Label label = (Label)Form1.ActiveForm.Controls.Find("Console", false).First();
                        label.Text = $"Похожего файла в директории не найдено";
                        Button button = (Button)Form1.ActiveForm.Controls.Find("Run", false).First();
                        if (button.Text != "Начать")
                        {
                            button.Text = "Начать";
                            button.Width -= 50;
                        }
                    });
                    return;
                }
            }
            Invoker(() => 
            {
                Label label = (Label)Form1.ActiveForm.Controls.Find("Console", false).First();
                label.Text += $" Время потрачено: {(time-DateTime.Now).Minutes}min:{(time - DateTime.Now).Seconds}sec:{(time - DateTime.Now).Milliseconds}ms";
                Button button=(Button)Form1.ActiveForm.Controls.Find("Run", false).First();
                if(button.Text != "Начать")
                {
                    button.Text = "Начать";
                    button.Width -= 50;
                }
               
            });
        }
   }
}
