using HospitalTriageAI.Models;

namespace HospitalTriageAI.Services;

/// <summary>
/// Service interface for triage operations
/// </summary>
public interface ITriageService
{
    /// <summary>
    /// Performs AI-powered triage assessment
    /// </summary>
    Task<RiskPrediction> AssessPatientAsync(Patient patient, TriageAssessment assessment);
    
    /// <summary>
    /// Gets assessment history for a patient
    /// </summary>
    Task<List<TriageAssessment>> GetPatientAssessmentsAsync(int patientId);
    
    /// <summary>
    /// Saves a completed assessment
    /// </summary>
    Task SaveAssessmentAsync(TriageAssessment assessment);
    
    /// <summary>
    /// Gets all patients grouped by triage level for dashboard
    /// </summary>
    Task<Dictionary<Models.Enums.TriageLevel, List<Patient>>> GetPatientsByTriageLevelAsync();
}
