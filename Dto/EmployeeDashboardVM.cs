using WorkTrack.App.Models;

namespace WorkTrack.App.Dto
{
    public class EmployeeDashboardVM
    {
        public List<EmployeeProjectVM> Projects { get; set; } = new();
        public List<EmployeeTaskVM> Tasks { get; set; } = new();
    }

    public class EmployeeProjectVM
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectCode { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public DateTime? EndDate { get; set; }
        public decimal Progress { get; set; }
    }

    public class EmployeeTaskVM
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public TasksStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
