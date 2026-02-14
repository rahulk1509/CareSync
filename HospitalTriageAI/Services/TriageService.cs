using Microsoft.EntityFrameworkCore;
using HospitalTriageAI.AI;
using HospitalTriageAI.Data;
using HospitalTriageAI.Models;
using HospitalTriageAI.Models.Enums;

namespace HospitalTriageAI.Services;

/// <summary>
/// Triage service implementation with AI integration
/// </summary>
public class TriageService : ITriageService
{
    private readonly AppDbContext _context;
    private readonly TriagePredictionEngine _predictionEngine;
    
    public TriageService(AppDbContext context, TriagePredictionEngine predictionEngine)
    {
        _context = context;
        _predictionEngine = predictionEngine;
    }
    
    public async Task<RiskPrediction> AssessPatientAsync(Patient patient, TriageAssessment assessment)
    {
        // Get AI prediction
        var prediction = _predictionEngine.Predict(assessment, patient.Age);
        
        // Update assessment with AI results
        assessment.AiRiskScore = prediction.RiskScore;
        assessment.AssignedLevel = prediction.PredictedLevel;
        assessment.AssessedAt = DateTime.Now;
        
        // Update patient's current triage level
        patient.CurrentTriageLevel = prediction.PredictedLevel;
        patient.LastUpdated = DateTime.Now;
        
        // Save to database
        _context.Assessments.Add(assessment);
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
        
        return prediction;
    }
    
    public async Task<List<TriageAssessment>> GetPatientAssessmentsAsync(int patientId)
    {
        return await _context.Assessments
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AssessedAt)
            .ToListAsync();
    }
    
    public async Task SaveAssessmentAsync(TriageAssessment assessment)
    {
        _context.Assessments.Add(assessment);
        await _context.SaveChangesAsync();
    }
    
    public async Task<Dictionary<TriageLevel, List<Patient>>> GetPatientsByTriageLevelAsync()
    {
        var patients = await _context.Patients
            .Where(p => p.CurrentTriageLevel != TriageLevel.Unassessed)
            .ToListAsync();
        
        return patients
            .GroupBy(p => p.CurrentTriageLevel)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}
