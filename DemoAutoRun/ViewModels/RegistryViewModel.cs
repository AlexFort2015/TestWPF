using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using DemoAutoRun.Core;
using DemoAutoRun.Models;
namespace DemoAutoRun.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class RegistryViewModel : BaseModel
    {
        /// <summary>
        /// Реализация поиска информации, автозапуска программ
        /// из ветвей реестра.
        /// </summary>
        /// <returns>Возвращает коллекцию наденных объектов</returns>
        public override IEnumerable<AutoRunFileInfoDataModel> Start()
        {
            var model_list = new List<AutoRunFileInfoDataModel>();
            try
            {
                //Проход по веткам реестра для получения списка ключей
                foreach (var key in RegistryPathArray)
                {
                    if (key == null)
                        continue;

                    var valueNames = key.GetValueNames();
                    
                    foreach (var valueName in valueNames)
                    {
                        var command = key.GetValue(valueName).ToString();

                        if (string.IsNullOrWhiteSpace(command))
                            continue;

                        ParseCommandLine(command, out var pathToFile, out var arguments);
                       
                        //Если файл по указанному пути существует
                        //пытаемся получить всю необходимую информацию о нем.
                        if (File.Exists(pathToFile))
                        {
                            var icon = GetIcon(pathToFile);
                            GetCertificateInfo(pathToFile, out var isSigned, out var isSignValid, out var manufacturer);
                            if (string.IsNullOrEmpty(manufacturer))
                                manufacturer = GetManufacturer(pathToFile);

                            model_list.Add(new AutoRunFileInfoDataModel(icon, pathToFile, arguments, isSigned, isSignValid, manufacturer, TypeAutoRunContainer.Registry));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError($"Ошибка при считывании файлов из реестра: {e}");

            }

            return model_list;
        }
        /// <summary>
        /// Массив ветвей реестра для 32 и 64 -битных версий WIndows
        /// по которым будет произведен поиск программ.
        /// </summary>
        private readonly RegistryKey[] RegistryPathArray = {
            /// Ветви реестра для 32-битной версии Windows
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                .OpenSubKey("SOFTWARE")?
                .OpenSubKey("Microsoft")?
                .OpenSubKey("Windows")?
                .OpenSubKey("CurrentVersion")?
                .OpenSubKey("RUN"),
            RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32)
                .OpenSubKey("SOFTWARE")?
                .OpenSubKey("Microsoft")?
                .OpenSubKey("Windows")?
                .OpenSubKey("CurrentVersion")?
                .OpenSubKey("RUN"),
            RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry32)
                .OpenSubKey(".DEFAULT")?
                .OpenSubKey("SOFTWARE")?
                .OpenSubKey("Microsoft")?
                .OpenSubKey("Windows")?
                .OpenSubKey("CurrentVersion")?
                .OpenSubKey("RUN"),
            // Ветви реестра для 64-битной версии Windows
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                .OpenSubKey("SOFTWARE")?
                .OpenSubKey("Microsoft")?
                .OpenSubKey("Windows")?
                .OpenSubKey("CurrentVersion")?
                .OpenSubKey("RUN"),
            RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
                .OpenSubKey("SOFTWARE")?
                .OpenSubKey("Microsoft")?
                .OpenSubKey("Windows")?
                .OpenSubKey("CurrentVersion")?
                .OpenSubKey("RUN"),
            RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64)
                .OpenSubKey(".DEFAULT")?
                .OpenSubKey("SOFTWARE")?
                .OpenSubKey("Microsoft")?
                .OpenSubKey("Windows")?
                .OpenSubKey("CurrentVersion")?
                .OpenSubKey("RUN")

        };
    }
}
