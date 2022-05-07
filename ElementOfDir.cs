using System;
using System.IO;

namespace KR_WinForm_FileManager
{
    /// <summary>
    /// Класс каталог/файла
    /// </summary>
    class ElementOfDir
    {
        /// <summary>
        /// Полное наименование каталога/файла
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Имя каталога/файла
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Размер файла (в б, Кб, Мб, Гб)
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// Тип элемента, расширение файла (name_element.type)
        /// </summary>
        public string TypeEl { get; set; }
        /// <summary>
        /// Дата последнего изменения каталога/файла
        /// </summary>
        public DateTime LastTime { get; set; }
        /// <summary>
        /// Ссылка на сам каталог/файла
        /// </summary>
        public object Element { get; }
        /// <summary>
        /// Конструктор класса элемента каталога
        /// </summary>
        /// <param name="path">Полный путь к файлу, тип string</param>
        /// <param name="DirEl">Каталог или файл. true - Каталог;false - файл. Тип bool</param>
        public ElementOfDir(string path, bool DirEl)
        {
            FullName = path;
            if (DirEl)
                Element = new DirectoryInfo(path);
            else
                Element = new FileInfo(path);
            AttributesInit(DirEl);
        }
        /// <summary>
        /// Инициализация полей класса, установление информации о каталоге/файле
        /// </summary>
        /// <param name="DirEl">Каталог или файл. true - Каталог;false - файл. Тип bool</param>
        private void AttributesInit(bool DirEl)
        {
            if (DirEl)
            {
                Name = ((DirectoryInfo)Element).Name;
                Size = "";
                TypeEl = "Папка с файлами";
                LastTime = ((DirectoryInfo)Element).LastWriteTime;
            }
            else
            {
                Name = ((FileInfo)Element).Name;
                Size = CheckAndSetSizeStr(((FileInfo)Element).Length);
                TypeEl = "Файл " + ((FileInfo)Element).Extension.ToString();
                LastTime = ((FileInfo)Element).LastWriteTime;
            }
        }
        /// <summary>
        /// Проверка и приведение размера файла к читаемому виду (Б, КБ, МБ, ГБ) - Валидация данных
        /// </summary>
        /// <param name="sizeB">Значение размера файла в Байтах. Тип long int</param>
        /// <returns>Приведенное значение в виде: размер_файла_*Б. Тип string</returns>
        public static string CheckAndSetSizeStr(long sizeB)
        {
            string strSize;
            if (sizeB < 10000)
            {
                strSize = sizeB.ToString() + " Б";
            }
            else if ((sizeB /= 1000) < 10000)
            {
                strSize = sizeB.ToString() + " КБ";
            }
            else if ((sizeB /= 1000) < 10000)
            {
                strSize = sizeB.ToString() + " МБ";
            }
            else
            {
                sizeB /= 1000;
                strSize = sizeB.ToString() + " ГБ";
            }
            return strSize;
        }
    }
    /// <summary>
    /// Информация о дисках. Вспомогательный класс.
    /// </summary>
    class DriveInfoUser
    {
        /// <summary>
        /// Предоставляет доступ к сведениям на диске.
        /// </summary>
        private DriveInfo drive;
        /// <summary>
        /// Размер диска в байтах
        /// </summary>
        public string SizeB { set; get; }
        /// <summary>
        /// Наименование диска
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Определяет константы для типов дисков, включая CDRom, Fixed, сети, NoRootDirectory,
        /// ОЗУ, съемных и Unknown.
        /// </summary>
        public DriveType DriveType { get; set; }
        /// <summary>
        /// Конструктор класса информации о диске
        /// </summary>
        /// <param name="driveN">Наименование диска</param>
        public DriveInfoUser(string driveN)
        {
            drive = new DriveInfo(driveN);
            SizeB = ElementOfDir.CheckAndSetSizeStr(drive.AvailableFreeSpace) + " / " + ElementOfDir.CheckAndSetSizeStr(drive.TotalSize);
            Name = drive.Name;
            DriveType = drive.DriveType;
        }
    }
}
