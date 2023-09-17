using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private string _sourceDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public MainWindow()
        {
            InitializeComponent();

            ReplaceButton.Click += OnReplaceButtonClick;
            CheckEmojiDoubleBackslashes.IsChecked = true;
            FindBtn.Click += OnFindButtonClick;
            ClearButtonForInputField.Click += OnClearButtonForInputFieldClick;

            CheckFilesExisting();
        }

        private void OnReplaceButtonClick(object sender, RoutedEventArgs e)
        {
            ClearText();
            MessageRichHex("Выполнено\n\n");
            MessageRichHex2("Выполнено\n\n");

            IEnumerable<string> jsonFiles = Directory.EnumerateFiles(_sourceDirectory, "*.json");

            if (jsonFiles.Count() > 0)
            {
                foreach (string currentFile in jsonFiles)
                {
                    MessageRichHex(currentFile + "\n");
                    MessageRichHex2(currentFile + "\n");
                    ReplaceInFile(currentFile);
                }
            }
            else
            {
                MessageRichHex("No JSONs");
                MessageRichHex2("No JSONs");
            }
        }

        private void ReplaceInFile(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            string content1 = reader.ReadToEnd();
            reader.Close();

            string content2 = "";
            string searchText1 = @"\\\\n";
            string replaceText1 = @"\n";
            string searchText2 = @"\\\\u";
            string replaceText2 = @"\u";

            if (CheckNewLineDoubleBackslashes.IsChecked == true && CheckEmojiDoubleBackslashes.IsChecked == true)
            {
                content2 = Regex.Replace(content1, searchText1, replaceText1);
                content2 = Regex.Replace(content2, searchText2, replaceText2);
            }
            else if (CheckEmojiDoubleBackslashes.IsChecked == true)
            {
                content2 = Regex.Replace(content1, searchText2, replaceText2);
            }
            else
            {
                content2 = Regex.Replace(content1, searchText1, replaceText1);
            }

            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content2);
            writer.Close();

            ShowChangedLines(content1, content2);
        }

        private void ShowChangedLines(string content1, string content2)
        {
            var lines1 = content1.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var lines2 = content2.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            if (lines1.Length < lines2.Length)
            {
                for (int i = 0; i < lines1.Length; i++)
                {
                    if (lines1[i] != lines2[i])
                    {
                        //MessageRichHex(lines1[i] + "\n", "#000000", "#f7adad");
                        //MessageRichHex2(lines2[i] + "\n", "#000000", "#d5f7ad");
                        MessageRichHex(lines1[i] + "\n");
                        MessageRichHex2(lines2[i] + "\n");
                    }
                }
            }
            else
            {
                for (int i = 0; i < lines2.Length; i++)
                {
                    if (lines1[i] != lines2[i])
                    {
                        MessageRichHex(lines1[i] + "\n");
                        MessageRichHex2(lines2[i] + "\n");
                    }
                }
            }
        }

        private void CheckFilesExisting()
        {
            ClearText();
            MessageRichHex("Найденные JSON\n\n");
            MessageRichHex2("Найденные JSON\n\n");

            IEnumerable<string> jsonFiles = Directory.EnumerateFiles(_sourceDirectory, "*.json");

            if (jsonFiles.Count() > 0)
            {
                foreach (string currentFile in jsonFiles)
                {
                    MessageRichHex(currentFile + "\n");
                    MessageRichHex2(currentFile + "\n");
                }
            }
            else
            {
                MessageRichHex("No JSONs");
                MessageRichHex2("No JSONs");
            }
        }

        private void ClearText()
        {
            OutputText1.Document.Blocks.Clear();
            OutputText2.Document.Blocks.Clear();
        }

        private void MessageRichHex(string text, string fontColor = "#000000", string bgColor = "#ffffff", int fontWeight = 0)
        {
            BrushConverter brushConverter = new BrushConverter();

            TextRange textRange = new TextRange(OutputText1.Document.ContentEnd, OutputText1.Document.ContentEnd);
            textRange.Text = text;
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, brushConverter.ConvertFromString(fontColor));
            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, brushConverter.ConvertFromString(bgColor));
            if (fontWeight == 1)
                textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            else
                textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Regular);
        }

        private void MessageRichHex2(string text, string fontColor = "#000000", string bgColor = "#ffffff", int fontWeight = 0)
        {
            BrushConverter brushConverter = new BrushConverter();

            TextRange textRange = new TextRange(OutputText2.Document.ContentEnd, OutputText2.Document.ContentEnd);
            textRange.Text = text;
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, brushConverter.ConvertFromString(fontColor));
            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, brushConverter.ConvertFromString(bgColor));
            if (fontWeight == 1)
                textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            else
                textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Regular);
        }

        private void RichTextBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var textToSync = (sender == OutputText1) ? OutputText2 : OutputText1;

            textToSync.ScrollToVerticalOffset(e.VerticalOffset);
            textToSync.ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        private void RichTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ResetAllFormat();

            Point pos0 = e.GetPosition(OutputText1);
            Point pos1 = new Point();
            Point pos2 = new Point();

            pos1.X = 10;
            pos2.X = 10;
            pos1.Y = pos0.Y;
            pos2.Y = pos0.Y;

            TextPointer tp1 = OutputText1.GetPositionFromPoint(pos1, true);
            TextPointer start1 = tp1.GetLineStartPosition(0);
            TextPointer end1 = (tp1.GetLineStartPosition(1) != null ? tp1.GetLineStartPosition(1) : tp1.DocumentEnd);
            TextRange tr1 = new TextRange(start1, end1);
            tr1.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.PaleGoldenrod);

            TextPointer tp2 = OutputText2.GetPositionFromPoint(pos2, true);
            TextPointer start2 = tp2.GetLineStartPosition(0);
            TextPointer end2 = (tp2.GetLineStartPosition(1) != null ? tp2.GetLineStartPosition(1) : tp2.DocumentEnd);
            TextRange tr2 = new TextRange(start2, end2);
            tr2.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.PaleGoldenrod);
        }

        private void ResetAllFormat()
        {
            TextRange tr1 = new TextRange(OutputText1.Document.ContentStart, OutputText1.Document.ContentEnd);
            tr1.ClearAllProperties();
            TextRange tr2 = new TextRange(OutputText2.Document.ContentStart, OutputText2.Document.ContentEnd);
            tr2.ClearAllProperties();
        }

        private void OnFindButtonClick(object sender, RoutedEventArgs e)
        {
            FindText();
        }

        private void FindText()
        {
            ResetAllFormat();

            TextRange textRange = new TextRange(OutputText1.Document.ContentStart, OutputText1.Document.ContentEnd);
            string allText = textRange.Text;
            string searchText = FindInput.Text;

            if (string.IsNullOrWhiteSpace(allText) || string.IsNullOrWhiteSpace(searchText))
            {
                SearchStatus.Content = "Поле пустое";
            }
            else
            {
                //using regex to get the search count
                //this will include search word even it is part of another word
                //say we are searching "hi" in "hi, how are you Mahi?" --> match count will be 2 (hi in 'Mahi' also)

                Regex regex = new Regex(searchText);
                int count_MatchFound = Regex.Matches(allText, regex.ToString()).Count;

                for (TextPointer startPointer = OutputText1.Document.ContentStart;
                            startPointer.CompareTo(OutputText1.Document.ContentEnd) <= 0;
                                startPointer = startPointer.GetNextContextPosition(LogicalDirection.Forward))
                {
                    //check if end of text
                    if (startPointer.CompareTo(OutputText1.Document.ContentEnd) == 0)
                    {
                        break;
                    }

                    //get the adjacent string
                    string parsedString = startPointer.GetTextInRun(LogicalDirection.Forward);

                    //check if the search string present here
                    int indexOfParseString = parsedString.IndexOf(searchText);

                    if (indexOfParseString >= 0) //present
                    {
                        //setting up the pointer here at this matched index
                        startPointer = startPointer.GetPositionAtOffset(indexOfParseString);

                        if (startPointer != null)
                        {
                            //next pointer will be the length of the search string
                            TextPointer nextPointer = startPointer.GetPositionAtOffset(searchText.Length);

                            //create the text range
                            TextRange searchedTextRange = new TextRange(startPointer, nextPointer);

                            //color up 
                            searchedTextRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Red));

                            //add other setting property

                        }
                    }
                }

                //update the label text with count
                if (count_MatchFound > 0)
                {
                    SearchStatus.Content = "Total Match Found : " + count_MatchFound;
                }
                else
                {
                    SearchStatus.Content = "No Match Found!";
                }
            }
        }

        private void OnClearButtonForInputFieldClick(object sender, RoutedEventArgs e)
        {
            FindInput.Text = "";
        }
    }
}