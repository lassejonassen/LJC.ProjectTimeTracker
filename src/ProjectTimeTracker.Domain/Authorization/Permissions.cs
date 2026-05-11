using System.Reflection;

namespace ProjectTimeTracker.Domain.Authorization;

public static class Permissions
{
    public static class Projects
    {
        public const string Read = "projects.read";
        public const string Create = "projects.create";
        public const string Update = "projects.update";
        public const string Delete = "projects.delete";

        // Used for Close() and Reopen() project status transitions
        public const string ManageStatus = "projects:manage-status";

        public static class ProjectTimeEntries
        {
            public const string Read = "projects.timeentries.read";
            public const string Create = "projects.timeentries.create";
            public const string Update = "projects.timeentries.update";
            public const string Delete = "projects.timeentries.delete";

            // Specifically for the SubmitTimeEntry action
            public const string Submit = "projects:time-entries:submit";

            // Specifically for ApproveTimeEntry, RejectTimeEntry, and ReopenTimeEntry
            public const string Approve = "projects:time-entries:approve";
        }
    }

    public static ICollection<string> GetRegisteredPermissions()
    {
        var permissions = new List<string>();
        RecursiveExtract(typeof(Permissions), permissions);
        return permissions;
    }

    private static void RecursiveExtract(Type type, List<string> permissions)
    {
        // 1. Get all public constants (static fields) at this level
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

        foreach (var field in fields)
        {
            var value = field.GetValue(null)?.ToString();
            if (value is not null)
            {
                permissions.Add(value);
            }
        }

        // 2. Dig into nested classes
        var nestedTypes = type.GetNestedTypes(BindingFlags.Public);
        foreach (var nestedType in nestedTypes)
        {
            RecursiveExtract(nestedType, permissions);
        }
    }
}
