namespace HospitalTriageAI.Models.Enums;

/// <summary>
/// Standard emergency department triage levels
/// </summary>
public enum TriageLevel
{
    /// <summary>Life-threatening - Immediate attention required</summary>
    Emergency = 1,
    
    /// <summary>Potentially life-threatening - Seen within 15 minutes</summary>
    Urgent = 2,
    
    /// <summary>Serious - Seen within 30-60 minutes</summary>
    Standard = 3,
    
    /// <summary>Minor - Can wait 1-2 hours</summary>
    NonUrgent = 4,
    
    /// <summary>Not yet assessed</summary>
    Unassessed = 0
}
