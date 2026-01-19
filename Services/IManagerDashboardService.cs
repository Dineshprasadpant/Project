using System.Threading.Tasks;
using WorkTrack.App.Dto;

namespace WorkTrack.App.Services
{
    public interface IManagerDashboardService
    {
        Task<ManagerDashboardVM> GetDashboardData(string userId);
    }
}
