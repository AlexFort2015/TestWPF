using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace DemoAutoRun.Models
{
    /// <summary>
    /// Перечиление мест откуда 
    /// производится автозапуск программы.
    /// Тип хранилища.
    /// </summary>
    public enum TypeAutoRunContainer
    {
        Registry,  // Реестр
        StartMenu, // Меню пуск
        Scheduler  // Планировщик заданий
         
    }
    /// <summary>
    /// Описываем модель данных, с которой будем работать.
    /// </summary>
    public class AutoRunFileInfoDataModel: INotifyPropertyChanged
    {
        /// <summary>
        /// Свойство - иконка ассоциированная с исполняемым файлом.
        /// </summary>
        private Bitmap FileIcon { get; }

        /// <summary>
        /// Свойства - наименование исполняемого файла.
        /// </summary>
        public string FileName => Path.GetFileName(Path2File);
        
        /// <summary>
        /// Свойство - путь к исполняемому файлу, для обозревателя.
        /// </summary>
        public string FileDirectory => Path.GetDirectoryName(Path2File);

        /// <summary>
        /// Свойство - путь к исполняемому файлу.
        /// </summary>
        public string Path2File { get; }

        /// <summary>
        /// Тип хранилища.
        /// </summary>
        public TypeAutoRunContainer TypeContainer { get; }

        public BitmapSource Icon => Imaging.CreateBitmapSourceFromHBitmap(FileIcon.GetHbitmap(),
           IntPtr.Zero,
           Int32Rect.Empty,
           BitmapSizeOptions.FromEmptyOptions()
           );

        /// <summary>
        /// Аргументы командной строки
        /// </summary>
        public string FileArguments { get; }

        /// <summary>
        /// Свойство - имеет ли файл цифровую подпись
        /// </summary>
        public bool IsSigned { get; }


        /// <summary>
        /// Свойство - дествительна ли цифровая подпись.
        /// </summary>
        public bool IsSignValid { get; }

        /// <summary>
        /// Совйство - компания производитель.
        /// </summary>
        public string FileManufacturer { get; }


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="fileIcon">Иконка</param>
        /// <param name="pathToFile">Имя файла</param>
        /// <param name="arguments">Сомандная строка</param>
        /// <param name="isSigned">Наличие цифровой подписи</param>
        /// <param name="isSignValid">Валидность цифровой подписи</param>
        /// <param name="manufacturer">Производитель</param>
        /// <param name="containerType">Тип хранилища</param>
        public AutoRunFileInfoDataModel(Bitmap fileIcon,
            string pathToFile, 
            string arguments,
            bool isSigned, 
            bool isSignValid,
            string manufacturer,
            TypeAutoRunContainer containerType)
                             {
                                     FileIcon           = fileIcon;
                                     Path2File         = pathToFile;
                                     FileArguments      = arguments;
                                     IsSignValid        = isSignValid;
                                     FileManufacturer   = manufacturer;
                                     IsSigned           = isSigned;
                                     TypeContainer        = containerType;
                             }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
