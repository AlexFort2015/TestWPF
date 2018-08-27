using DemoAutoRun.Core;
using DemoAutoRun.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using IWshRuntimeLibrary;

namespace DemoAutoRun.ViewModels
{
    /// <summary>
    /// Класс для поиска автоматически запускаемых файлов 
    /// из специальных папок windows, пути описаны в массиве StartMenuFolderArray.
    /// </summary>
    public class StartMenuViewModel : BaseModel
    {
        /// <summary>
        /// Реализация алгоритма поиска автоматически исполняемых файлов
        /// при старте системы из меню Пуск.
        /// </summary>
        /// <returns>Возвращает коллекцию наденных объектов</returns>
        public override IEnumerable<AutoRunFileInfoDataModel> Start()
        {
            var _shell = new WshShell();
            var model_list = new List<AutoRunFileInfoDataModel>();
            try
            {
                foreach (var path in StartMenuFolderArray)
                {
                    try
                    {
                        if (!Directory.Exists(path))
                            throw new Exception($"Не найден путь {path}");

                        var files = Directory.EnumerateFiles(path);

                        foreach (var file in files)
                        {
                            if (file != null && Path.GetExtension(file).ToLower().Equals(".lnk"))
                            {
                                var shortcut = (IWshShortcut)_shell.CreateShortcut(file);
                                var targetPath = Environment.ExpandEnvironmentVariables(shortcut.TargetPath);

                                //Усли файл по указанному пути существует
                                //пытаемся получить всю необходимую информацию о нем.
                                if (System.IO.File.Exists(targetPath))
                                {
                                    var icon = GetIcon(targetPath);

                                    var arguments_command = shortcut.Arguments;

                                    if (string.IsNullOrEmpty(arguments_command))
                                        arguments_command = $"Параметры командной строки отсутствуют";

                                    GetCertificateInfo(targetPath, out var isSigned, out var isSignValid, out var manufacturer);

                                    if (string.IsNullOrEmpty(manufacturer))
                                        manufacturer = GetManufacturer(targetPath);

                                     model_list.Add(new AutoRunFileInfoDataModel(icon, targetPath, arguments_command, isSigned, isSignValid, manufacturer, TypeAutoRunContainer.StartMenu));
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError($"Ошибка при обработке директории {path}: {e} ");
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError($"Ошибка при считывании файлов автозагрузки из меню Пуск: {e}");
            }
            return model_list;
        }
        /// <summary>
        ///  Строковый массив, описывает пути поиска файлов автозагрузки из меню Пуск.
        /// </summary>
        private readonly string[] StartMenuFolderArray =
        {
            Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup),
            Environment.GetFolderPath(Environment.SpecialFolder.Startup)
        };


    }
}
