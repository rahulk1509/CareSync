namespace HospitalTriageAI.Models.Enums;

/// <summary>
/// Hospital departments for patient routing based on diagnosis
/// </summary>
public enum Department
{
    /// <summary>Not yet assigned</summary>
    Unassigned = 0,
    
    /// <summary>Heart and cardiovascular issues</summary>
    Cardiology = 1,
    
    /// <summary>Breathing and lung issues</summary>
    Pulmonology = 2,
    
    /// <summary>Brain and nervous system</summary>
    Neurology = 3,
    
    /// <summary>General surgery needs</summary>
    Surgery = 4,
    
    /// <summary>Bone and muscle issues</summary>
    Orthopedics = 5,
    
    /// <summary>General medical care</summary>
    GeneralMedicine = 6,
    
    /// <summary>Infectious diseases and fever</summary>
    InfectiousDisease = 7,
    
    /// <summary>Emergency and trauma</summary>
    EmergencyTrauma = 8,
    
    /// <summary>Pain management</summary>
    PainManagement = 9
}
