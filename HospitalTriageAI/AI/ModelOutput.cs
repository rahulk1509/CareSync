using Microsoft.ML.Data;

namespace HospitalTriageAI.AI;

/// <summary>
/// Output data structure from ML.NET model
/// </summary>
public class ModelOutput
{
    [ColumnName("PredictedLabel")]
    public float PredictedTriageLevel { get; set; }
    
    [ColumnName("Score")]
    public float[] Scores { get; set; } = Array.Empty<float>();
}
