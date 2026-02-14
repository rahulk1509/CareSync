using HospitalTriageAI.Models.Enums;

namespace HospitalTriageAI.Models;

/// <summary>
/// AI prediction result from ML.NET model
/// </summary>
public class RiskPrediction
{
    /// <summary>
    /// Recommended triage level from AI
    /// </summary>
    public TriageLevel PredictedLevel { get; set; }
    
    /// <summary>
    /// Risk score from 0 to 1 (higher = more urgent)
    /// </summary>
    public float RiskScore { get; set; }
    
    /// <summary>
    /// Confidence of the prediction (0-1)
    /// </summary>
    public float Confidence { get; set; }
    
    /// <summary>
    /// Key factors that influenced the prediction
    /// </summary>
    public List<string> RiskFactors { get; set; } = new();
    
    /// <summary>
    /// Recommendation for healthcare staff
    /// </summary>
    public string Recommendation { get; set; } = string.Empty;
    
    /// <summary>
    /// When the prediction was made
    /// </summary>
    public DateTime PredictedAt { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Display color for UI
    /// </summary>
    public string GetLevelColor() => PredictedLevel switch
    {
        TriageLevel.Emergency => "#DC2626",    // Red
        TriageLevel.Urgent => "#F97316",       // Orange
        TriageLevel.Standard => "#FBBF24",     // Yellow
        TriageLevel.NonUrgent => "#22C55E",    // Green
        _ => "#6B7280"                          // Gray
    };
    
    /// <summary>
    /// Display text for triage level
    /// </summary>
    public string GetLevelText() => PredictedLevel switch
    {
        TriageLevel.Emergency => "EMERGENCY - Immediate",
        TriageLevel.Urgent => "URGENT - Within 15 min",
        TriageLevel.Standard => "STANDARD - Within 1 hour",
        TriageLevel.NonUrgent => "NON-URGENT - Can wait",
        _ => "Not Assessed"
    };
}
