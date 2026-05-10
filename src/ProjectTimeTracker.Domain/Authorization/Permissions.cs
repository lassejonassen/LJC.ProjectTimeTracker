namespace ProjectTimeTracker.Domain.Authorization;

public static class Permissions
{
    public static class Projects
    {
        public const string Read = "projects.read";
        public const string Create = "projects.create";
        public const string Update = "projects.update";
        public const string Delete = "projects.delete";

        public static class ProjectTimeEntries
        {
            public const string Read = "projects.timeentries.read";
            public const string Create = "projects.timeentries.create";
            public const string Update = "projects.timeentries.update";
            public const string Delete = "projects.timeentries.delete";
        }
    }
}
