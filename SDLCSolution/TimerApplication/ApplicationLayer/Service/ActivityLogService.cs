using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Enums;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Service;

public class ActivityLogService
{
    private readonly IActivityLogRepository _activityLogRepository;

    private readonly ActivityService _activityService;

    public ActivityLogService(IActivityLogRepository activityLogRepository, ActivityService activityService)
    {
        this._activityLogRepository = activityLogRepository;
        this._activityService = activityService;
    }

    public void StartTimer(Guid activityId)
    {
        HashSet<ActivityLog> logs = this.GetLogsByActivityId(activityId);
        string taskName = this._activityService.GetAllActivities().FirstOrDefault(activity => activity.ActivityId == activityId).ActivityName;
        string logName = $"{taskName}-Log{logs.Count() + 1}";
        this.CreateActivityLog(logName, activityId);
        this._activityService.EditTaskStatus(activityId, TimerStatus.Running);
    }

    public void CreateActivityLog(string logName, Guid activityId)
    {
        ActivityLog activityLog = new ActivityLog(logName, DateTime.Now, null, TimeSpan.Zero, TimerStatus.Running, activityId, CurrentSessionManager.CurrentSessionUser.UserId);
        this._activityLogRepository.CreateActivityLog(activityLog);
    }

    private HashSet<ActivityLog> GetLogsByActivityId(Guid activityId)
    {
        return this.GetAllActivityLogs().Where(activity => activity.ActivityId == activityId).ToHashSet();
    }
    public void StopTimer(Guid activityLogId)
    {
        ActivityLog activityLog = this._activityLogRepository.FetchAllActivityLogs().FirstOrDefault(activityLog => activityLog.ActivityLogId == activityLogId);
        activityLog.LogStatus = TimerStatus.Stopped;
        activityLog.EndTime = DateTime.Now;
        activityLog.Duration = (TimeSpan)(activityLog.EndTime - activityLog.StartTime);
        this._activityLogRepository.EditActivityLog(activityLogId, activityLog);
        this._activityService.EditTaskDuration(activityLog);
    }

        public List<ActivityLog> GetAllActivityLogs()
        {
            return this._activityLogRepository.FetchAllActivityLogs().Where(a => a.UserId == CurrentSessionManager.CurrentSessionUser.UserId).ToList();
        }

        public void DeleteActivityLogsByTaskId(Guid activityId)
        {
            var activityLogs = this.GetAllActivityLogs().Where(a => a.ActivityId == activityId).ToList();
            foreach (var log in activityLogs)
            {
                this._activityLogRepository.DeleteActivityLog(log.ActivityLogId);
            }
        }

        public bool DeleteActivityLogByLogId(Guid activityLogId)
        {
            var activityLogToDelete = this.GetAllActivityLogs().FirstOrDefault(a => a.ActivityLogId == activityLogId);
            if (activityLogToDelete.LogStatus == TimerStatus.Running)
            {
                return false;
            }

            this._activityLogRepository.DeleteActivityLog(activityLogId);
            return true;
        }

        public void EditTaskLog(ActivityLog activityLogToBeEdited)
        {
            this._activityLogRepository.EditActivityLog(activityLogToBeEdited.ActivityLogId, activityLogToBeEdited);
        }

        public Result ExportLogs(int indexOfActivity)
        {
            var activities = this._activityService.GetAllActivities();
            if (indexOfActivity >= activities.Count)
            {
                return new Result(false, "Index Out Of Range");
            }

            var selectedActivity = activities[indexOfActivity];
            var logsToExport = this.GetAllActivityLogs().Where(log => log.TaskId == selectedActivity.ActivityId)
                .OrderBy(log => log.StartTime)
                .ToList();
            if (!logsToExport.Any())
            {
                return new Result(false, "No logs recorded for the activity to export");
            }

            this._activityLogRepository.ExportActivityLogsToCsv($"{selectedActivity.ActivityName}.csv", logsToExport);
            return new Result(true, $"Logs exported Successfully");
        }


        private Result ImportLogs(string filePath, int index)
        {
            try
            {
                var activities = this._activityService.GetAllActivities();
                if (index >= activities.Count)
                {
                    return new Result(false, "Index Out Of Range");
                }

                var activitySelected = activities[index];
                var importedLogs = this._activityLogRepository.ImportActivityLogsFromCsv(filePath, activitySelected.ActivityId);
                var existingActivityLogs = this.GetAllActivityLogs().Where(a => a.TaskId == activitySelected.ActivityId).ToList();
                var logsToValidateAgainst = new List<ActivityLog>(existingActivityLogs);
                TimeSpan totalTaskDuration = TimeSpan.Zero;
                int importedCount = 0;
                var overlapEntries = new List<ActivityLog>();
                foreach (var importedLog in importedLogs)
                {
                    bool hasOverlap = logsToValidateAgainst.Any(existing => importedLog.StartTime < existing.EndTime && importedLog.EndTime > existing.StartTime);
                    if (hasOverlap)
                    {
                        overlapEntries.Add(importedLog);
                        continue;
                    }

                    var newLog = new ActivityLog(Guid.NewGuid(), importedLog.LogName, importedLog.StartTime, importedLog.EndTime, importedLog.Duration, importedLog.LogStatus, importedLog.TaskId, CurrentSessionManager.CurrentSessionUser.UserId);
                    this._activityLogRepository.CreateActivityLog(newLog);
                    logsToValidateAgainst.Add(newLog);
                    totalTaskDuration += newLog.Duration;
                    importedCount++;
                }

                if (importedCount > 0)
                {
                    this._activityService.UpdateTaskTotalDuration(activitySelected, totalTaskDuration);
                    if (!overlapEntries.Any())
                    {
                        return new Result(true, "Logs imported successfully!");
                    }

                    string skippedEntries = string.Join("\n", overlapEntries.Select(o => $"-{o.LogName} ({o.StartTime:yyyy-MM-dd HH:mm:ss} to {o.EndTime:yyyy-MM-dd HH:mm:ss})"));
                    return new Result(true, $"{importedCount} logs imported, Overlapping log entries were found and skipped+\n");
                }
                else
                {
                    return new Result(false, "No logs were imported because time overlaps with existing activity logs");
                }
            }
            catch (FormatException ex)
            {
                return new Result(false, ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                return new Result(false, ex.Message);
            }
        }
    }

