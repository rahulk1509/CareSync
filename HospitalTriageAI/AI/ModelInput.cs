using Microsoft.ML.Data;

namespace HospitalTriageAI.AI;

/// <summary>
/// Input data structure for ML.NET model
/// </summary>
public class ModelInput
{
    [LoadColumn(0)]
    public float Age { get; set; }
    
    [LoadColumn(1)]
    public float HeartRate { get; set; }
    
    [LoadColumn(2)]
    public float BloodPressureSystolic { get; set; }
    
    [LoadColumn(3)]
    public float BloodPressureDiastolic { get; set; }
    
    [LoadColumn(4)]
    public float Temperature { get; set; }
    
    [LoadColumn(5)]
    public float RespiratoryRate { get; set; }
    
    [LoadColumn(6)]
    public float OxygenSaturation { get; set; }
    
    [LoadColumn(7)]
    public float PainLevel { get; set; }
    
    [LoadColumn(8)]
    public float ChestPain { get; set; }
    
    [LoadColumn(9)]
    public float ShortnessOfBreath { get; set; }
    
    [LoadColumn(10)]
    public float AlteredConsciousness { get; set; }
    
    [LoadColumn(11)]
    public float Bleeding { get; set; }
    
    [LoadColumn(12)]
    public float Fever { get; set; }
    
    // Label for training (1-4 = TriageLevel)
    [LoadColumn(13)]
    [ColumnName("Label")]
    public float TriageLevel { get; set; }
}
