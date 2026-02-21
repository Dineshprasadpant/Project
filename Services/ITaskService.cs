using WorkTrack.App.Dto;

namespace WorkTrack.App.Services
{
    public interface ITaskService
    {
        Task<(bool Success, string Message)> CreateTaskAsync(TaskCreateDto dto, string assignedById);
    }
}
