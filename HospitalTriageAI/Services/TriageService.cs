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
        
        // Assign department based on symptoms and diagnosis
        patient.AssignedDepartment = DetermineDepartment(assessment, patient);
        
        // Save to database
        _context.Assessments.Add(assessment);
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
        
        return prediction;
    }
    
    /// <summary>
    /// Determines the appropriate department based on symptoms and assessment
    /// </summary>
    private Department DetermineDepartment(TriageAssessment assessment, Patient patient)
    {
        // Priority 1: Emergency/Trauma for severe altered consciousness or severe bleeding
        if (assessment.AlteredConsciousness == SymptomSeverity.Severe ||
            assessment.Bleeding == SymptomSeverity.Severe)
        {
            return Department.EmergencyTrauma;
        }
        
        // Priority 2: Cardiology for chest pain or cardiac indicators
        if (assessment.ChestPain >= SymptomSeverity.Moderate ||
            (assessment.HeartRate > 120 || assessment.HeartRate < 50) ||
            assessment.BloodPressureSystolic > 180 || assessment.BloodPressureSystolic < 90)
        {
            return Department.Cardiology;
        }
        
        // Priority 3: Pulmonology for respiratory issues
        if (assessment.ShortnessOfBreath >= SymptomSeverity.Moderate ||
            assessment.OxygenSaturation < 92 ||
            assessment.RespiratoryRate > 25 || assessment.RespiratoryRate < 10)
        {
            return Department.Pulmonology;
        }
        
        // Priority 4: Neurology for altered consciousness
        if (assessment.AlteredConsciousness >= SymptomSeverity.Mild)
        {
            return Department.Neurology;
        }
        
        // Priority 5: Infectious Disease for fever
        if (assessment.Fever >= SymptomSeverity.Moderate ||
            assessment.Temperature > 38.5f)
        {
            return Department.InfectiousDisease;
        }
        
        // Priority 6: Surgery/Orthopedics for bleeding with trauma indicators
        if (assessment.Bleeding >= SymptomSeverity.Mild)
        {
            // Check chief complaint for trauma/injury keywords
            var complaint = patient.ChiefComplaint?.ToLower() ?? "";
            if (complaint.Contains("fall") || complaint.Contains("accident") ||
                complaint.Contains("fracture") || complaint.Contains("bone") ||
                complaint.Contains("joint") || complaint.Contains("sprain"))
            {
                return Department.Orthopedics;
            }
            return Department.Surgery;
        }
        
        // Priority 7: Pain Management for high pain without other indicators
        if (assessment.PainLevel >= 7)
        {
            return Department.PainManagement;
        }
        
        // Default: General Medicine
        return Department.GeneralMedicine;
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
