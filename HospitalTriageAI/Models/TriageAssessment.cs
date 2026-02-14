using HospitalTriageAI.Models.Enums;

namespace HospitalTriageAI.Models;

/// <summary>
/// Triage assessment data including vital signs and symptoms
/// </summary>
public class TriageAssessment
{
    public int Id { get; set; }
    
    public int PatientId { get; set; }
    
    // Vital Signs
    public float HeartRate { get; set; }           // bpm (normal: 60-100)
    
    public float BloodPressureSystolic { get; set; }  // mmHg (normal: 90-120)
    
    public float BloodPressureDiastolic { get; set; } // mmHg (normal: 60-80)
    
    public float Temperature { get; set; }          // Celsius (normal: 36.1-37.2)
    
    public float RespiratoryRate { get; set; }      // breaths/min (normal: 12-20)
    
    public float OxygenSaturation { get; set; }     // % (normal: 95-100)
    
    public int PainLevel { get; set; }              // 0-10 scale
    
    // Symptoms (simplified for hackathon)
    public SymptomSeverity ChestPain { get; set; }
    
    public SymptomSeverity ShortnessOfBreath { get; set; }
    
    public SymptomSeverity AlteredConsciousness { get; set; }
    
    public SymptomSeverity Bleeding { get; set; }
    
    public SymptomSeverity Fever { get; set; }
    
    public string? AdditionalSymptoms { get; set; }
    
    public string? Notes { get; set; }
    
    // Result
    public TriageLevel AssignedLevel { get; set; } = TriageLevel.Unassessed;
    
    public float? AiRiskScore { get; set; }
    
    public DateTime AssessedAt { get; set; } = DateTime.Now;
    
    public string? AssessedBy { get; set; }
    
    // Navigation property
    public Patient? Patient { get; set; }
}
