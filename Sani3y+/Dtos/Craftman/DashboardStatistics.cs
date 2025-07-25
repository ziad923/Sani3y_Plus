﻿namespace Sani3y_.Dtos.Craftman
{
    public class DashboardStatistics
    {
        public string ProfileImage {  get; set; }
        public string FullName { get; set; }
        public double AverageRating { get; set; }
        public int AreasWorkedIn { get; set; }
        public int ProjectsCompleted { get; set; }
        public int TotalIncomingRequests { get; set; }
        public int RequestsCompleted { get; set; }
        public int RequestsUnderImplementation { get; set; }
        public int RequestsRejected { get; set; }
        public double AverageResponseTime { get; set; }
        public double AverageExecutionTime { get; set; }
        public int AverageMonthlyProjects { get; set; }
    }
}
