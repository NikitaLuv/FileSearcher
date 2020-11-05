using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace FileSearcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<TreeViewItem> _directoriesTree;
        private List<TreeViewItem> _folderNodeList;
        private TreeViewItem _mainNode;

        public MainWindow()
        {
            InitializeComponent();

            _directoriesTree = new ObservableCollection<TreeViewItem>();
            _folderNodeList = new List<TreeViewItem>();
            FilesTree.ItemsSource = _directoriesTree;


        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            _directoriesTree.Clear();
            _mainNode = null;
            _folderNodeList.Clear();

            _directoriesTree = new ObservableCollection<TreeViewItem>();
            FilesTree.ItemsSource = _directoriesTree;
            string startDirectory = StartDirectory.Text;
            string filePattern = FilePattern.Text;

            if (string.IsNullOrEmpty(startDirectory) || string.IsNullOrEmpty(filePattern))
            {
                MessageBox.Show("Все поля должны быть заполнены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var wathc = Stopwatch.StartNew();
            FindFilePattern(startDirectory, filePattern);
            wathc.Stop();
            Seconds.Text = ((decimal)wathc.ElapsedMilliseconds / 1000).ToString();
        }
        private void FindFilePattern(string startDirectory, string filePattern)
        {
  
            TreeViewItem currentNode;
            CurrentDirectory.Text = startDirectory;
            if(_mainNode == null)
            {
                _mainNode = new TreeViewItem();
                _mainNode.Header = startDirectory;
                _mainNode.Tag = startDirectory;

                currentNode = _mainNode;
                _folderNodeList.Add(_mainNode);
                _directoriesTree.Add(_mainNode);
            }
            else
            {
                var child = new TreeViewItem();
                int start = startDirectory.LastIndexOf('\\') + 1;
                child.Header = startDirectory.Substring(start, startDirectory.Length - start);
                child.Tag = startDirectory;
                var baseDirectory = startDirectory.Substring(0, startDirectory.LastIndexOf('\\'));

                currentNode = child;
                var ancesstorNode = _folderNodeList.FirstOrDefault(a => (string)a.Tag == baseDirectory);
                ancesstorNode.Items.Add(child);
                _folderNodeList.Add(child);
            }

            string[] dirs = Directory.GetDirectories(startDirectory);
            string[] files = Directory.GetFiles(startDirectory);

            if (files.Count() > 0)
            {
                foreach (string file in files)
                {
                    int lastSlashPos = file.LastIndexOf('\\');
                    string fileName = file.Substring(lastSlashPos + 1, file.Length - lastSlashPos - 1);
                    if (Regex.IsMatch(fileName, filePattern))
                    {
                        var fileTreeItem = new TreeViewItem();
                        fileTreeItem.Header = fileName;
                        fileTreeItem.Tag = file;
                        currentNode.Items.Add(fileTreeItem);
                    }
                }
            }

            if(dirs.Count() > 0)
            {
                foreach(string dir in dirs)
                {
                    FindFilePattern(dir, filePattern);
                }
            }


            return;
        }
    }
}
