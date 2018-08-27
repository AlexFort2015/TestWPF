using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using DemoAutoRun.Models;

namespace DemoAutoRun.Core
{
    /// <summary>
    /// Абстрактный класс, с помощью него будем
    /// будем порождать производные классы для поиска информации,
    /// для каждого из которых  реализуем свой метод RUN() для получения
    /// необходимых нам по заданию данных.
    /// </summary>
    abstract public class BaseModel : IProcessSearch
    {
        /// <summary>
        /// Этот метод необходимо реализовать в производных классах,
        /// наполнив его необходимой бизнес-логикой.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<AutoRunFileInfoDataModel> Start();

        /// <summary>
        /// Разбирает командную строку, для получения 
        /// параметров запуска программ.
        /// </summary>
        /// <param name="commandLine">Командная строка</param>
        /// <param name="path">Путь к файлу</param>
        /// <param name="arguments">аргументы командной строки</param>
        public void ParseCommandLine(string commandLine, out string path, out string arguments)
        {
            try
            {
                commandLine = commandLine.Replace("\"", "");
                commandLine = Environment.ExpandEnvironmentVariables(commandLine);

                var name = Path.GetFileName(commandLine);
                var match = Regex.Match(commandLine,
                    @"([A-Za-z0-9\-+=\s\.!%&$@{}\[\]'_])+\.([A-Za-z0-9\-+=!%&$@{}\[\]'_])+\s",
                    RegexOptions.IgnoreCase);

                var shortname = match.Value;
             
                if (shortname == "")
                    shortname = name;
                else
                    shortname = shortname.Substring(0, shortname.Length - 1);


                var substrs = commandLine.Split(new[] { shortname }, StringSplitOptions.RemoveEmptyEntries);
                path = Path.Combine(substrs.First(), shortname);
               
                if (substrs.Length == 2)
                {
                    arguments = substrs.LastOrDefault()?.Trim();
                }
                else
                {
                    arguments = $"Параметры командной строки отстсутствуют.";
                }

            }
            catch (Exception e)
            {
                Trace.TraceError($"Ошибка при разборе командной строки - {e}");
                path = "";
                arguments = string.Empty;
            }
        }
        /// <summary>
        /// Считывает иконку файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Возвращает изображение</returns>
        protected Bitmap GetIcon(string path)
        {
            try
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException($"Не верный путь к фалу {path}");
                using (var icon = Icon.ExtractAssociatedIcon(path))
                    return icon?.ToBitmap();
            }
            catch (Exception e)
            {
                Trace.TraceError($"Ошибка получения иконки из файла {path}: {e}");
                return null;
            }
        }

        /// <summary>
        /// Проверяет имеет ли файл цифровую подпись и ее валидность
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="isSigned">Если имеется цифровая подпись, возвращает TRUE</param>
        /// <param name="isSignValid">Если цифравая подпись действительна, возвращает TRUE</param>
        /// <param name="manufacturer"></param>
        protected void GetCertificateInfo(string path, out bool isSigned, out bool isSignValid,
            out string manufacturer)
        {
            try
            {
                var certificate = new X509Certificate2(path);
                isSigned = true;
                var certificateChain = new X509Chain {
                        ChainPolicy =
                        {
                            RevocationMode = X509RevocationMode.NoCheck
                        }
                    };

                isSignValid = certificateChain.Build(certificate);

                manufacturer = certificate.Subject
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault(s => s.StartsWith("CN="))?
                    .Replace("CN=", "").Trim(',').Trim().Trim('"');

            }
            catch (Exception e)
            {
                Trace.TraceError($"Ошибка при получении цифровой подписи для файла {path}: {e}");
                isSigned = false;
                isSignValid = false;
                manufacturer = string.Empty;
            }
        }

        /// <summary>
        /// Считывает информацию о производителе файла.
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Возвращает строку с именем производителя исполняемого файла.</returns>
        protected string GetManufacturer(string path)
        {
            try
            {
                return FileVersionInfo.GetVersionInfo(path).CompanyName;
            }
            catch (Exception e)
            {
                Trace.TraceError($"Ошибка при получении производителя из ресурсов файла {path}: {e}");
                return string.Empty;
            }
        }
    }
}
