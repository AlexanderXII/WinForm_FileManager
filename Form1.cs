using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KR_WinForm_FileManager
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Объект файлового менеджера для левого окна
        /// </summary>
        private ManagerFS ManagerLeft;

        /// <summary>
        /// Объект файлового менеджера для правого окна
        /// </summary>
        private ManagerFS ManagerRight;

        /// <summary>
        /// Список названий выделенных элементов для копирования
        /// </summary>
        public List<string> arrCopyElementPath { set; get; }

        /// <summary>
        /// Инициализация основной формы (создание объектов файловых менеджеров)
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            ManagerLeft = new ManagerFS(ref NameListBoxL, ref SizeListBoxL, ref TypeListBoxL, ref DateListBoxL, ref PathLabelL, ref SearchBoxL, ref SplitContainerL, this);
            ManagerRight = new ManagerFS(ref NameListBoxR, ref SizeListBoxR, ref TypeListBoxR, ref DateListBoxR, ref PathLabelR, ref SearchBoxR, ref SplitContainerR, this);
        }

        /// <summary>
        /// Активация опции вставки элемента при копирование элемента
        /// <para>то есть пока пользователь не скопировал какой-либо элемент, ему не будет отображатся опция вставить в ... (правую или
        /// левую панель</para>
        /// </summary>
        public void EnabledPastMenuItem()
        {
            ManagerLeft.managerContextMenu.Items[2].Enabled = true;
            ManagerRight.managerContextMenu.Items[2].Enabled = true;
        }

        /// <summary>
        /// Обработчик включчения/выключения опции параллельный режим, в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void ParallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CollapsedManagerRight();
        }

        /// <summary>
        /// Скрытие/отображения правого окна менеджера
        /// </summary>
        private void CollapsedManagerRight()
        { 
            if (ParallToolStripMenuItem.Checked)
                MaintSplitC.Panel2Collapsed = true;
            else
                MaintSplitC.Panel2Collapsed = false;
            ParallToolStripMenuItem.Checked = !ParallToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Обработчик включения/выключения функции множественного выделения элементов
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void MultiCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!MultiCopyToolStripMenuItem.Checked)
            {
                MultiCopyToolStripMenuItem.Checked = true;
                MultiCopyToolStripMenuItem.Text = "&Одиночный выбор";
                NameListBoxL.SelectionMode = SelectionMode.MultiExtended;
                NameListBoxR.SelectionMode = SelectionMode.MultiExtended;
            }
            else
            {
                MultiCopyToolStripMenuItem.Checked = false;
                MultiCopyToolStripMenuItem.Text = "&Множественный выбор";
                NameListBoxL.SelectionMode = SelectionMode.One;
                NameListBoxR.SelectionMode = SelectionMode.One;
            }
        }

        /// <summary>
        /// Обработчик выбора опции открытие элемента правого окна в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void OpenRPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagerRight.OpenElementManager();
        }

        /// <summary>
        /// Обработчик выбора опции открытие элемента левого окна в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void OpenLPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagerLeft.OpenElementManager();
        }

        /// <summary>
        /// Обработчик выбора опции обновить элементы в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void UpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateAll();
        }

        /// <summary>
        /// Обнавление элементов отображаемых в окнах менеджера
        /// </summary>
        public void UpdateAll()
        {
            ManagerRight.UpdateList();
            ManagerLeft.UpdateList();
        }

        /// <summary>
        /// Обработчик выбора опции копирования выделенного элемента правого окна в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void CopyRPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagerRight.CopyElementBox();
        }

        /// <summary>
        /// Обработчик выбора опции копирования выделенного элемента левого окна в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void CopyLPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagerLeft.CopyElementBox();
        }

        /// <summary>
        /// Обработчик выбора опции вставки скопированного элемента в правое окно в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void PastFightPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagerRight.PastElementsBox();
            ManagerLeft.UpdateList();
        }

        /// <summary>
        /// Обработчик выбора опции вставки скопированного элемента в левое окно в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void PastLeftPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagerLeft.PastElementsBox();
            ManagerRight.UpdateList();
        }

        /// <summary>
        /// Обработчик выбора опции создания каталога в правом окне в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void CreateDirRPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagerRight.CreateDir(PathLabelR.Text + '\\');
            ManagerLeft.UpdateList();
        }

        /// <summary>
        /// Обработчик выбора опции создания каталога в левом окне в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void CreateDirLPToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ManagerLeft.CreateDir(PathLabelL.Text + '\\');
            ManagerRight.UpdateList();
        }

        /// <summary>
        /// Обработчик выбора опции удаления выделенного элемента в правом окне в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void DeleteRPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagerRight.DeleteElementOfManager();
            ManagerLeft.UpdateList();
        }

        /// <summary>
        /// Обработчик выбора опции удаления выделенного элемента в левом окне в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void DeleteLPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagerLeft.DeleteElementOfManager();
            ManagerRight.UpdateList();
        }

        /// <summary>
        /// Обработчик выбора опции отмены выделенных элементов окон в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void CancelSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagerLeft.CancelSelecteItemBoxes();
            ManagerRight.CancelSelecteItemBoxes();
        }

        /// <summary>
        /// Строка справочник
        /// </summary>
        private string about = "В данной программе доступны следующие опции:\n1) Открытие каталогов.\n2) Запуск файлов\n3) Удаление файлов\n4) Копирование и вставка файлов и каталогов\n5) Создание папки\n6) Обновление отображаемого содержимого каталогов или дисков\n7) Отмена выделенных элементов\n8) Двухпанельный/однопанельный режим\n9) Множесвтенный/одиночный выбор элементов.\n10) Копировать из правой/левой панели в ...\n\nДанные операции выполняются, либо при помощи вызова контекстного меню в списке элементов каждой панели, либо при помощи раздела 'Операции' на панели меню.\nОтмену выделенных элементов можно произвести при помощи нажатия кнопки ESCAPE.\nПоиск по шаблону производится в открытом каталоге.\nЧтобы осуществить поиск введите в текстовое поле над списком нужный для поиска текст и нажмите ENTER, после этого в списке отобразяться все элементы текущего каталого удовлетворяющие шаблону.";

        /// <summary>
        /// Обработчик выбора опции справка в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void InfoAboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            MessageBox.Show(about);
        }

        /// <summary>
        /// Обработчик выбора опции копирования выделенного(-ых) элемента(-ов) из правого окна в левое окно менеджера в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void CoppyToRPВToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagerRight.CopyToSD();
            ManagerLeft.UpdateList();
        }

        /// <summary>
        /// Обработчик выбора опции копирования выделенного(-ых) элемента(-ов) из левого окна в правое окно менеджера в верхнем меню (ToolBar)
        /// </summary>
        /// <param name="sender">Объект с которым произошло событие</param>
        /// <param name="e">Событие</param>
        private void CopyToLPВToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManagerLeft.CopyToSD();
            ManagerRight.UpdateList();
        }
    }
}
