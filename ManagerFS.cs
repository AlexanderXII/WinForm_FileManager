using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace KR_WinForm_FileManager
{
    /// <summary>
    /// Файловый менеджер
    /// </summary>
    class ManagerFS
    {
        /// <summary>
        /// Путь корневого каталога
        /// </summary>
        private string rootDir = "";

        /// <summary>
        /// Список дисков
        /// </summary>
        private List<DriveInfoUser> drivesU;

        /// <summary>
        /// Список текущих каталогов и файлов
        /// </summary>
        private List<ElementOfDir> cuurFileAndDirs;

        /// <summary>
        /// Список элементов для отображения данных
        /// </summary>
        private List<ListBox> ListBoxes;

        /// <summary>
        /// Элемент отображения текущего полного пути (тип: C:\Dir_1\SubDir_2\Dir_3)
        /// </summary>
        private Label PathLab;

        /// <summary>
        /// Элемент поиска (файлов и каталогов)
        /// </summary>
        private TextBox SearchBox;

        /// <summary>
        /// Экземляр контейнера содержащего визуальные элементы
        /// </summary>
        private SplitContainer SplitC;

        /// <summary>
        /// Экземпляр контексного меню
        /// </summary>
        public ContextMenuStrip managerContextMenu;

        /// <summary>
        /// Экземпляр родительского элемента отображения
        /// </summary>
        private Form1 parentForm;

        /// <summary>
        /// Диалоговое окно для выбора каталога
        /// </summary>
        private FolderBrowserDialog folderBrowserDialog1;

        /// <summary>
        /// Конструктор фалового менеджера
        /// </summary>
        /// <param name="NameListB">Ссылка на визуальный список в котором отображаются названия файлов, каталогов и дисков</param>
        /// <param name="SizeListB">Ссылка на визуальный список в котором отображаются размеры файлов или дисков</param>
        /// <param name="TypeListB">Ссылка на визуальный список в котором отображаются названия типов элемента (html, png и т.п.)</param>
        /// <param name="DateListB">Ссылка на визуальный список в котором отображаются даты последнего изменения элемента</param>
        /// <param name="Path">Ссылка на визуальный элемент для отображения текущего пути</param>
        /// <param name="SearchB">Ссылка на визуальный элемент для отображения строки поиска</param>
        /// <param name="SplitCont">Ссылка на визуальный элемент для отображения основных элементов</param>
        /// <param name="pForm">Ссылка на основную форму</param>
        public ManagerFS(ref ListBox NameListB, ref ListBox SizeListB, ref ListBox TypeListB, ref ListBox DateListB, ref Label Path, ref TextBox SearchB, ref SplitContainer SplitCont, Form1 pForm)
        {
            parentForm = pForm;
            ListBoxes = new List<ListBox>();
            ListBoxes.Add(NameListB);
            ListBoxes.Add(SizeListB);
            ListBoxes.Add(TypeListB);
            ListBoxes.Add(DateListB);
            SplitC = SplitCont;
            PathLab = Path;
            SearchBox = SearchB;
            SearchBox.KeyDown += SearchBox_KeyDown;
            OutDrivesInfo();
            ListBoxes[0].DoubleClick += NameListBoxL_DoubleClick;
            managerContextMenu = new ContextMenuStrip();
            //--
            ToolStripMenuItem copyElement = new ToolStripMenuItem("Копировать");
            copyElement.Click += CopyElement_Click;
            ToolStripMenuItem pastElement = new ToolStripMenuItem("Вставить");
            pastElement.Click += PastElement_Click;
            ToolStripMenuItem deleteElement = new ToolStripMenuItem("Удалить");
            deleteElement.Click += DeleteElement_Click;
            ToolStripMenuItem createDir = new ToolStripMenuItem("Создать папку");
            createDir.Click += CreateDir_Click;
            ToolStripMenuItem openElement = new ToolStripMenuItem("Открыть");
            openElement.Click += OpenElement_Click;
            ToolStripMenuItem updatelistElements = new ToolStripMenuItem("Обновить");
            updatelistElements.Click += UpdatelistElements_Click;
            ToolStripMenuItem CancelSelect = new ToolStripMenuItem("Отменить выбор");
            CancelSelect.Click += CancelSelect_Click;
            ToolStripMenuItem CopyToSelectD = new ToolStripMenuItem("Копировать в...");
            CopyToSelectD.Click += CopyToSelectD_Click;
            //--
            //                                             0            1           2           3               4               5               6               7
            managerContextMenu.Items.AddRange(new[] { openElement, copyElement, pastElement, createDir, deleteElement, updatelistElements, CancelSelect, CopyToSelectD});
            managerContextMenu.Items[2].Enabled = false;
            ListBoxes[0].ContextMenuStrip = managerContextMenu;
            ListBoxes[0].KeyDown += ManagerFS_KeyDown;
            managerContextMenu.Opening += ManagerContextMenu_Opening;
        }

        /// <summary>
        /// Обработчик события выбора в контекстном меню функии копирования
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void CopyToSelectD_Click(object sender, EventArgs e)
        {
            CopyToSD();
            parentForm.UpdateAll();
        }

        /// <summary>
        /// Копироване файла или каталога в выбранную дерикторию
        /// </summary>
        public void CopyToSD()
        {
            CopyElementBox();
            folderBrowserDialog1 = new FolderBrowserDialog();
            DirectoryInfo targetDir;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                targetDir = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
            }
            else 
                return;
            foreach (string pathCopyEl in parentForm.arrCopyElementPath)
            {
                //targetDir
                if (Directory.Exists(pathCopyEl))                               // Dir
                {
                    DirectoryInfo sourceDir = new DirectoryInfo(pathCopyEl);
                    //targetDir = targetDir.CreateSubdirectory(sourceDir.Name + "_copy");
                    PastElementInTarget(sourceDir, targetDir.CreateSubdirectory(sourceDir.Name + "_copy"));
                }
                else if (File.Exists(pathCopyEl))                                // File
                {
                    FileInfo sourceFile = new FileInfo(pathCopyEl);
                    sourceFile.CopyTo(Path.Combine(targetDir.ToString(), "Copy_" + sourceFile.Name), true);
                }
            }
            //UpdateList();
        }

        /// <summary>
        /// Отмена выбора нексольких файлов (при включенном режиме Множественный выбор)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void CancelSelect_Click(object sender, EventArgs e)
        {
            CancelSelecteItemBoxes();
        }

        /// <summary>
        /// Визуальная очистка нескольких выделенных элементов
        /// </summary>
        public void CancelSelecteItemBoxes()
        {
            ListBoxes[0].ClearSelected();
            ListBoxes[1].ClearSelected();
            ListBoxes[2].ClearSelected();
            ListBoxes[3].ClearSelected();
        }

        /// <summary>
        /// Обработчик события выбора функции "Обновить"
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void UpdatelistElements_Click(object sender, EventArgs e)
        {
            UpdateList();
        }

        /// <summary>
        /// Обработчик события нажатия клавиши Enter при вводе в поле для поиска (запуск процедуры поиска элемента)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (SearchBox.Text == "")
                    return;
                string searchPath;
                switch (CheckTypeItem(ListBoxes[0].SelectedItem))
                {
                    case 0:
                        searchPath = ((DriveInfoUser)ListBoxes[0].SelectedItem).Name;        // Root_Dir
                        break;
                    case 1:
                        searchPath = PathLab.Text;                                           // Parent_Dir
                        break;
                    case 2:                                                                  // Dir
                        searchPath = ((ElementOfDir)ListBoxes[0].SelectedItem).FullName;
                        break;
                    default:
                        MessageBox.Show("Error");
                        return;
                }
                SeacrhElementDir(searchPath);
                SearchBox.Clear();
            }
        }

        /// <summary>
        /// Поиск элемента по названию
        /// </summary>
        /// <param name="pathSearch">Часть текста для поиска элемена</param>
        private void SeacrhElementDir(string pathSearch)
        {
            if (SearchBox.Text.Contains("\r\n"))
                SearchBox.Text = SearchBox.Text.Replace("\r\n", "");
            cuurFileAndDirs = new List<ElementOfDir>();
            cuurFileAndDirs.Add(new ElementOfDir(rootDir, true));
            cuurFileAndDirs[0].Name = "...";
            cuurFileAndDirs[0].TypeEl = "Корневой каталог";
            PathLab.Text = rootDir;
            string[] dirEntries = Directory.GetDirectories(pathSearch, '*' + SearchBox.Text + '*');
            foreach (string fileName in dirEntries)
                cuurFileAndDirs.Add(new ElementOfDir(fileName, true));
            string[] fileEntries = Directory.GetFiles(pathSearch, '*' + SearchBox.Text + '*');
            foreach (string fileName in fileEntries)
                cuurFileAndDirs.Add(new ElementOfDir(fileName, false));

            ListBoxes[0].DataSource = cuurFileAndDirs;
            ListBoxes[0].DisplayMember = "Name";
            SplitC.Panel2Collapsed = false;
            ListBoxes[3].DataSource = cuurFileAndDirs;
            ListBoxes[3].DisplayMember = "LastTime";
            ListBoxes[1].DataSource = cuurFileAndDirs;
            ListBoxes[1].DisplayMember = "Size";
            ListBoxes[2].DataSource = cuurFileAndDirs;
            ListBoxes[2].DisplayMember = "TypeEl";
        }

        /// <summary>
        /// Обработчик события выбора функции создать каталог
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void CreateDir_Click(object sender, EventArgs e)
        {
            CreateDir(PathLab.Text +'\\');
            parentForm.UpdateAll();
        }

        /// <summary>
        /// Создание каталога
        /// </summary>
        /// <param name="path">Путь где создается каталог</param>
        public void CreateDir(string path)
        {
            DirectoryInfo createDir;
            FormCreateEl formCreateEl = new FormCreateEl();
            if (formCreateEl.ShowDialog(parentForm) == DialogResult.OK)
            {
                path += formCreateEl.NameElementTextBox.Text + '\\';
                createDir = new DirectoryInfo(path);
                if (!createDir.Exists)
                    createDir.Create();
                else
                {
                    MessageBox.Show("Каталог уже существует");
                    return;
                }
            }
            UpdateList();
        }

        /// <summary>
        /// Обработчик события выбора функции удалить выбранный элемент
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void DeleteElement_Click(object sender, EventArgs e)
        {
            DeleteElementOfManager();
            parentForm.UpdateAll();
        }

        /// <summary>
        /// Удаление выделенного элемента
        /// </summary>
        public void DeleteElementOfManager()
        {
            for (int i = 0; i < ListBoxes[0].Items.Count; i++)
            {
                if (ListBoxes[0].GetSelected(i))
                {
                    switch (CheckTypeItem(ListBoxes[0].Items[i]))
                    {
                        case 1:
                            break;
                        case 2:
                            DirectoryInfo tmpDelEl = new DirectoryInfo(((ElementOfDir)ListBoxes[0].Items[i]).FullName);
                            try
                            {
                                tmpDelEl.Delete();
                            }
                            catch (IOException)
                            {
                                DialogResult result;
                                result = MessageBox.Show("Удалить каталог вместе с его содержимым?", "Каталог не пуст", MessageBoxButtons.YesNo);
                                if (result == DialogResult.Yes)
                                    try
                                    {
                                        tmpDelEl.Delete(true);
                                    }
                                    catch (IOException)
                                    {
                                        MessageBox.Show("Каталог доступен только для чтения или является текущим рабочим каталогом приложения");
                                    }
                            }
                            break;
                        case 3:
                            FileInfo tmpDelF = new FileInfo(((ElementOfDir)ListBoxes[0].Items[i]).FullName);
                            try
                            {
                                tmpDelF.Delete();
                            }
                            catch (IOException)
                            {
                                MessageBox.Show("Целевой файл открыт");
                            }
                            break;
                        default:
                            MessageBox.Show("Error");
                            return;
                    }
                }
            }
            UpdateList();
        }


        /// <summary>
        /// Обработчик события выбора функции вставить элемент
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void PastElement_Click(object sender, EventArgs e)
        {
            PastElementsBox();
            parentForm.UpdateAll();
        }

        /// <summary>
        /// Вставка скопиравонного элемента в выделенное место
        /// <para>При вставке скопированного элемента происходит проверка содержится ли файл с таким же названием, если содержится
        /// то к названию вставляемого файла добавить префикс "Copy_" для каталога и постфикс для файла "_copy"</para>
        /// </summary>
        public void PastElementsBox()
        {
            if (parentForm.arrCopyElementPath != null)
            {
                bool insert = false;
                for (int i = 0; i < ListBoxes[0].Items.Count; i++)
                {
                    if (ListBoxes[0].GetSelected(i))
                    {
                        string tmpPathEl;
                        switch (CheckTypeItem(ListBoxes[0].Items[i]))
                        {
                            case 2:                                                                     // Select Dir in ListBox
                                tmpPathEl = ((ElementOfDir)ListBoxes[0].Items[i]).FullName;
                                insert = true;
                                break;
                            /*case 4:                                                                     // Current Dir
                                tmpPathEl = PathLab.Text;
                                insert = true;
                                break;*/
                            case 1:
                                tmpPathEl = PathLab.Text;
                                insert = true;
                                break;
                            default:
                                MessageBox.Show("Error");
                                return;
                        }
                        DirectoryInfo targetDir = new DirectoryInfo(tmpPathEl);
                        foreach (string pathCopyEl in parentForm.arrCopyElementPath)
                        {
                            if (Directory.Exists(pathCopyEl))                               // Dir
                            {
                                DirectoryInfo sourceDir = new DirectoryInfo(pathCopyEl);
                                //targetDir = targetDir.CreateSubdirectory(sourceDir.Name + "_copy");
                                PastElementInTarget(sourceDir, targetDir.CreateSubdirectory(sourceDir.Name + "_copy"));
                            }
                            else if (File.Exists(pathCopyEl))                                // File
                            {
                                FileInfo sourceFile = new FileInfo(pathCopyEl);
                                sourceFile.CopyTo(Path.Combine(targetDir.ToString(), "Copy_" + sourceFile.Name), true);
                            }
                        }
                        UpdateList();
                    }
                }
                if (!insert && CheckTypeItem(ListBoxes[0].SelectedItem) == 4)
                {
                    //DirectoryInfo tmpDir = new DirectoryInfo(PathLab.Text);
                    DirectoryInfo targetDir = new DirectoryInfo(PathLab.Text);
                    foreach (string pathCopyEl in parentForm.arrCopyElementPath)
                    {
                        //targetDir
                        if (Directory.Exists(pathCopyEl))                               // Dir
                        {
                            DirectoryInfo sourceDir = new DirectoryInfo(pathCopyEl);
                            //targetDir = targetDir.CreateSubdirectory(sourceDir.Name + "_copy");
                            PastElementInTarget(sourceDir, targetDir.CreateSubdirectory(sourceDir.Name + "_copy"));
                        }
                        else if (File.Exists(pathCopyEl))                                // File
                        {
                            FileInfo sourceFile = new FileInfo(pathCopyEl);
                            sourceFile.CopyTo(Path.Combine(targetDir.ToString(), "Copy_" + sourceFile.Name), true);
                        }
                    }
                    UpdateList();
                }
                else if (!insert)
                    MessageBox.Show("Вставка не произошла\n неизвестаня ошибка");
            }
        }

        /// <summary>
        /// Вставка элемента в указанную дирексторию
        /// </summary>
        /// <param name="source">Исходный каталог расположения</param>
        /// <param name="target">Каталог куда планируется вставка элемента</param>
        private void PastElementInTarget(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                PastElementInTarget(diSourceSubDir, nextTargetSubDir);
            }
        }

        /// <summary>
        /// Обработчик события выбора функции копировать элемент в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void CopyElement_Click(object sender, EventArgs e)
        {
            CopyElementBox();
        }

        /// <summary>
        /// Копирование элемента в выбранную пнель (правую или левую при параллельном режиме)
        /// </summary>
        public void CopyElementBox()
        {
            parentForm.arrCopyElementPath = new List<string>();
            for (int i = 0; i < ListBoxes[0].Items.Count; i++)
            {
                if (ListBoxes[0].GetSelected(i))
                    switch (CheckTypeItem(ListBoxes[0].Items[i]))
                    {
                        case 2:
                            parentForm.arrCopyElementPath.Add(((ElementOfDir)ListBoxes[0].Items[i]).FullName);
                            break;
                        case 3:
                            parentForm.arrCopyElementPath.Add(((ElementOfDir)ListBoxes[0].Items[i]).FullName);
                            break;
                        case 1:
                            continue;
                        default:
                            MessageBox.Show("Error");
                            break;
                    }
            }
            parentForm.EnabledPastMenuItem();
        }

        /// <summary>
        /// Обработчик двойного щелчка для открытия выделенного элемента
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void OpenElement_Click(object sender, EventArgs e)
        {
            OpenElementManager();
        }

        /// <summary>
        /// Обработчик события нажатия на клавишу Escape, сброс всех выбранных элементов, при режиме "Множественного выбора"
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void ManagerFS_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                ListBoxes[0].SelectedItem = null;
                ListBoxes[1].SelectedItem = null;
                ListBoxes[2].SelectedItem = null;
                ListBoxes[3].SelectedItem = null;
            }
        }

        /// <summary>
        /// Обработчик события открытия контекстного меню (нажатие на ПКМ)
        /// <para>В зависимости от того какой элемент является выделенным при вызове контекстного меню, отображаются разные
        /// возможные функции (например для элемента Диск возможно только открытие и обновление списка элементо, 
        /// а для файла копирование, удаление и т.д.)</para>
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void ManagerContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            switch (CheckTypeItem(ListBoxes[0].SelectedItem))
            {
                case 0:
                    managerContextMenu.Items[0].Visible = true;         // Open
                    managerContextMenu.Items[1].Visible = false;
                    managerContextMenu.Items[2].Visible = false;
                    managerContextMenu.Items[3].Visible = false;
                    managerContextMenu.Items[4].Visible = false;
                    managerContextMenu.Items[5].Visible = true;         // Update
                    managerContextMenu.Items[7].Visible = false;
                    break;
                case 1:
                    managerContextMenu.Items[0].Visible = true;         // Open
                    managerContextMenu.Items[1].Visible = false;
                    managerContextMenu.Items[2].Visible = true;         // Past;
                    managerContextMenu.Items[3].Visible = true;         // Create
                    managerContextMenu.Items[4].Visible = true;         // Update
                    managerContextMenu.Items[7].Visible = false;
                    break;
                case 2:                                         // Dir
                    managerContextMenu.Items[0].Visible = true;         // Open
                    managerContextMenu.Items[1].Visible = true;         // Copy
                    managerContextMenu.Items[2].Visible = true;         // Past
                    managerContextMenu.Items[3].Visible = false;
                    managerContextMenu.Items[4].Visible = true;         // Del
                    managerContextMenu.Items[5].Visible = true;         // Update
                    managerContextMenu.Items[7].Visible = true;         // Copy to
                    break;
                case 3:                                         // File
                    managerContextMenu.Items[0].Visible = true;         // Open
                    managerContextMenu.Items[1].Visible = true;         // Copy
                    managerContextMenu.Items[2].Visible = false;
                    managerContextMenu.Items[3].Visible = false;
                    managerContextMenu.Items[4].Visible = true;         // Del
                    managerContextMenu.Items[5].Visible = true;         // Update
                    managerContextMenu.Items[7].Visible = true;         // Copy to
                    break;
                case 4:
                    managerContextMenu.Items[0].Visible = false;
                    managerContextMenu.Items[1].Visible = false;
                    managerContextMenu.Items[2].Visible = true;         // Past
                    managerContextMenu.Items[3].Visible = true;         // Create
                    managerContextMenu.Items[4].Visible = false;
                    managerContextMenu.Items[5].Visible = true;         // Update
                    managerContextMenu.Items[7].Visible = false;
                    break;
                case 5:
                    managerContextMenu.Items[0].Visible = false;
                    managerContextMenu.Items[1].Visible = false;
                    managerContextMenu.Items[2].Visible = false;
                    managerContextMenu.Items[3].Visible = false;
                    managerContextMenu.Items[4].Visible = false;
                    managerContextMenu.Items[5].Visible = true;         // Update
                    managerContextMenu.Items[7].Visible = false;
                    break;
                default:
                    MessageBox.Show("Error");
                    break;
            }
        }

        /// <summary>
        /// Вывод информации о элементе Диск (специальный вывод так как отсутсвует поле дата изменения)
        /// </summary>
        private void OutDrivesInfo()
        {
            drivesU = new List<DriveInfoUser>();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                    drivesU.Add(new DriveInfoUser(drive.Name));
            }
            ListBoxes[0].DataSource = drivesU;
            ListBoxes[0].DisplayMember = "Name";
            ListBoxes[1].DataSource = drivesU;
            ListBoxes[1].DisplayMember = "SizeB";
            SplitC.Panel2Collapsed = true;
            ListBoxes[2].DataSource = drivesU;
            ListBoxes[2].DisplayMember = "DriveType";
            PathLab.Text = "Этот компьютер";
        }

        /// <summary>
        /// Обработчик события двойного нажатия на элемент (открытие)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void NameListBoxL_DoubleClick(object sender, EventArgs e)
        {
            OpenElementManager();
        }

        /// <summary>
        /// Открытие выделенного элемента
        /// <para>Для разных объектов отличается функция открытия, так для каталога происходит погружение внутрь каталога, а
        /// при открытии файла запускается процесс запуска программы для конкретного файла (например для .html - браузер)</para>
        /// </summary>
        public void OpenElementManager()
        {
            for (int i = 0; i < ListBoxes[0].Items.Count; i++)
            {
                if (ListBoxes[0].GetSelected(i))
                {
                    string pathD;
                    switch (CheckTypeItem(ListBoxes[0].Items[i]))
                    {
                        case 0:
                            pathD = ((DriveInfoUser)ListBoxes[0].Items[i]).Name;        // Root_Dir
                            CheckAndOutputDir(pathD);
                            break;
                        case 1:
                            pathD = PathLab.Text;                                           // Parent_Dir
                            CheckAndOutputDir(pathD);
                            break;
                        case 2:                                                             // Dir
                            pathD = ((ElementOfDir)ListBoxes[0].Items[i]).FullName;
                            CheckAndOutputDir(pathD);
                            break;
                        case 3:                                                             // File
                            pathD = ((ElementOfDir)ListBoxes[0].Items[i]).FullName;
                            Process.Start(pathD);
                            break;
                        case 4:
                        case 5:
                            break;
                        default:
                            MessageBox.Show("Error");
                            return;
                    }
                }
            }
        }

        /// <summary>
        /// Проверка и вывод информации о текущей директории (происходит при открытии директории)
        /// </summary>
        /// <param name="pathDir">Текущий путь открытого элемента</param>
        private void CheckAndOutputDir(string pathDir)
        {
            if (pathDir == rootDir)
            {
                for (int i = 0; i < drivesU.Count; i++)
                {
                    if (pathDir == drivesU[i].Name)
                    {
                        rootDir = "";
                        OutDrivesInfo();
                        return;
                    }
                }
                if (pathDir != Directory.GetDirectoryRoot(pathDir) && pathDir.IndexOf('\\') != pathDir.LastIndexOf('\\'))
                    rootDir = pathDir.Substring(0, pathDir.Length - (pathDir.Length - pathDir.LastIndexOf('\\')));
                else
                    rootDir = Directory.GetDirectoryRoot(pathDir);
                pathDir = rootDir;
            }
            else
                rootDir = pathDir;

            cuurFileAndDirs = new List<ElementOfDir>();
            cuurFileAndDirs.Add(new ElementOfDir(rootDir, true));
            cuurFileAndDirs[0].Name = "...";
            cuurFileAndDirs[0].TypeEl = "Корневой каталог";
            PathLab.Text = pathDir;

            if (Directory.Exists(pathDir))
            {
                rootDir = pathDir;
                //Подкаталоги
                foreach (string strD in Directory.GetDirectories(pathDir))
                {
                    cuurFileAndDirs.Add(new ElementOfDir(strD, true));
                }
                //Файлы
                foreach (string strF in Directory.GetFiles(pathDir))
                {
                    cuurFileAndDirs.Add(new ElementOfDir(strF, false));
                }
            }
            ListBoxes[0].DataSource = cuurFileAndDirs;
            ListBoxes[0].DisplayMember = "Name";
            SplitC.Panel2Collapsed = false;
            ListBoxes[3].DataSource = cuurFileAndDirs;
            ListBoxes[3].DisplayMember = "LastTime";
            ListBoxes[1].DataSource = cuurFileAndDirs;
            ListBoxes[1].DisplayMember = "Size";
            ListBoxes[2].DataSource = cuurFileAndDirs;
            ListBoxes[2].DisplayMember = "TypeEl";
        }

        /// <summary>
        /// Соотнисение выбранного элемента и его кода
        /// <list type="table">
        ///     <listheader>
        ///         <term>Коды элемента</term>
        ///         <description>Тип элемента</description>
        ///     </listheader>
        ///     <item>
        ///         <term>0</term>
        ///         <description>Этот компьютер</description>
        ///     </item>
        ///     <item>
        ///         <term>1</term>
        ///         <description>... (текущий каталог)</description>
        ///     </item>
        ///     <item>
        ///         <term>2</term>
        ///         <description>Каталог (папка)</description>
        ///     </item>
        ///     <item>
        ///         <term>3</term>
        ///         <description>Файл</description>
        ///     </item>
        ///     <item>
        ///         <term>4,5</term>
        ///         <description>null</description>
        ///     </item>
        ///     <item>
        ///         <term>-1,-2</term>
        ///         <description>error</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="selectItem">Выделенный элемент</param>
        /// <returns>Целочисленного представление элемента</returns>
        private int CheckTypeItem(object selectItem)
        {
            if (PathLab.Text == "Этот компьютер")
            {
                if(selectItem == null)
                    return 5;
                return 0;
            }
            else if (selectItem is ElementOfDir)
            {
                ElementOfDir tmpEl = (ElementOfDir)selectItem;
                if (tmpEl.Name == "...")
                    return 1;
                else if (tmpEl.Element is DirectoryInfo)
                    return 2;
                else if (tmpEl.Element is FileInfo)
                    return 3;
                else
                    return -1;
            }
            else if (selectItem == null)
                return 4;
            return -2;
        }

        /// <summary>
        /// Обновление списка отображаемых элементов открытого каталога (вызывается при изменении содержимого или при помощи опции)
        /// </summary>
        public void UpdateList()
        {
            if(PathLab.Text == "Этот компьютер")
            {
                OutDrivesInfo();
            }
            else
            {
                cuurFileAndDirs = new List<ElementOfDir>();
                cuurFileAndDirs.Add(new ElementOfDir(rootDir, true));
                cuurFileAndDirs[0].Name = "...";
                cuurFileAndDirs[0].TypeEl = "Корневой каталог";
                PathLab.Text = rootDir;

                if (Directory.Exists(rootDir))
                {
                    //Подкаталоги
                    foreach (string strD in Directory.GetDirectories(rootDir))
                    {
                        cuurFileAndDirs.Add(new ElementOfDir(strD, true));
                    }
                    //Файлы
                    foreach (string strF in Directory.GetFiles(rootDir))
                    {
                        cuurFileAndDirs.Add(new ElementOfDir(strF, false));
                    }
                }
                ListBoxes[0].DataSource = cuurFileAndDirs;
                ListBoxes[0].DisplayMember = "Name";
                SplitC.Panel2Collapsed = false;
                ListBoxes[3].DataSource = cuurFileAndDirs;
                ListBoxes[3].DisplayMember = "LastTime";
                ListBoxes[1].DataSource = cuurFileAndDirs;
                ListBoxes[1].DisplayMember = "Size";
                ListBoxes[2].DataSource = cuurFileAndDirs;
                ListBoxes[2].DisplayMember = "TypeEl";
            }
        }
    }
}
