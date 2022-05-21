// ReSharper disable InconsistentNaming
namespace Comforix.Shared;

public enum PhysicalOrEmotional
{
    Physical = 0,
    Emotional = 1,
    Both = 2
}

public enum PhysicalIssue
{
    Broken_Leg = 0,
    Broken_Arm = 1,
    Cancer = 2,
    Something_Else = 65534,
    Prefer_Not_To_Say = 65535
}

public enum EmotionalIssue
{
    Depression = 0,
    Anxiety = 1,
    PTSD = 2,
    Stress = 3,
    Something_Else = 65534,
    Prefer_Not_To_Say = 65535
}

public enum Issue
{
    Depression = 0,
    Anxiety = 1,
    PTSD = 2,
    Stress = 3,
    Broken_Leg = 4,
    Broken_Arm = 5,
    Cancer = 6,
    Something_Else = 65534,
    Prefer_Not_To_Say = 65535
}