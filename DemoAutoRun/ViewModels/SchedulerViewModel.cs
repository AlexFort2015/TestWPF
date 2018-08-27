
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using TaskScheduler;
using DemoAutoRun.Core;
using DemoAutoRun.Models;

namespace DemoAutoRun.ViewModels
{
    /// <summary>
    /// Класс для поиска автоматически запускаемых файлов 
    /// из задач стандартного планировщика заданий windows.
    /// </summary>
    public class SchedulerViewModel : BaseModel
    {
        /// <summary>
        /// Реализация алгоритма поиска автоматически исполняемых файлов
        /// при старте системы из планировщика заданий.
        /// </summary>
        /// <param name="taskFolder"></param>
        /// <param name="model">Список объектов</param>
        private void ScanSchedulerTask(ITaskFolder taskFolder, List<AutoRunFileInfoDataModel> model)
        {
            try
            {
                var tasks = taskFolder.GetTasks((int)_TASK_ENUM_FLAGS.TASK_ENUM_HIDDEN);
                var countTask = tasks.Count;
                foreach (IRegisteredTask task in tasks)
                {
                    try
                    {
                        if (!task.Enabled)
                            continue;
                        var taskTriggers = task.Definition.Triggers;
                        foreach (ITrigger taskTrigger in taskTriggers)
                        {
                            if (!taskTrigger.Enabled ||
                               taskTrigger.Type != _TASK_TRIGGER_TYPE2.TASK_TRIGGER_BOOT &&
                               taskTrigger.Type != _TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON)
                                continue;
                            var actions = task.Definition.Actions;
                            foreach (IAction action in actions)
                            {
                                if (action.Type != _TASK_ACTION_TYPE.TASK_ACTION_EXEC ||
                                    !(action is IExecAction execAction)) continue;

                                var path = Environment.ExpandEnvironmentVariables(execAction.Path);
                               
                                //Если файл по указанному пути существует
                                //пытаемся получить всю необходимую информацию о нем.
                                if (File.Exists(path))
                                {
                                    var icon = GetIcon(path);

                                    var startArguments = execAction.Arguments;

                                    if(string.IsNullOrEmpty(startArguments))
                                        startArguments=$"Параметры командной строки отсутствуют";

                                    GetCertificateInfo(path, out var isSigned, out var isSignValid, out var manufacturer);

                                    if (string.IsNullOrEmpty(manufacturer))
                                        manufacturer = GetManufacturer(path);

                                    model.Add(new AutoRunFileInfoDataModel(icon, path, startArguments, isSigned, isSignValid, manufacturer, TypeAutoRunContainer.Scheduler));
                                }

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError($"Ошибка при считывании файлов из планировщика, задание {task?.Name}: {e}");
                    }
                }

                var folders = taskFolder.GetFolders((int)_TASK_ENUM_FLAGS.TASK_ENUM_HIDDEN);
                foreach (ITaskFolder folder in folders)
                    ScanSchedulerTask(folder, model);
            }
            catch (Exception e)
            {
                Trace.TraceError($"Ошибка при считывании файлов из планировщика, папка {taskFolder?.Name}: {e}");
            }
        }

        /// <summary>
        /// Реализация алгоритма поиска автоматически исполняемых файлов
        /// при старте системы из планировщика заданий.
        /// </summary>
        /// <returns>Возвращает коллекцию наденных объектов</returns>
        public override IEnumerable<AutoRunFileInfoDataModel> Start()
        {
            var model_list = new List<AutoRunFileInfoDataModel>();
            try
            {
                ITaskService taskService = new TaskScheduler.TaskScheduler();
                taskService.Connect();
                var rootTaskFolder = taskService.GetFolder("\\");
                ScanSchedulerTask(rootTaskFolder, model_list);
            }
            catch (Exception e)
            {
                Trace.TraceError($"Ошибка при считывании файлов из планировщика: {e}");
            }
            return model_list;
        }
    }
}
