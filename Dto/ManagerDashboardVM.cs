namespace WorkTrack.App.Dto
{
    public class ManagerDashboardVM
    {
        public int OngoingProjects { get; set; }
        public int TeamMembers { get; set; }
        public int ActiveTasks { get; set; }
        public int PendingTasks { get; set; }
        public double CompletionRate { get; set; }

        public List<ProjectCardVM> Projects { get; set; } = new();
        public List<TeamMemberVM> TeamMembersList { get; set; } = new();
    }

    public class ProjectCardVM
    {
        public int ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int MembersCount { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Progress { get; set; }
    }

    public class TeamMemberVM
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateTime AssignedDate { get; set; }
        public int TaskCount { get; set; }
        public string? Role {  get; set; }
    }
}
