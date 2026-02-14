using Microsoft.ML;
using HospitalTriageAI.Models;
using HospitalTriageAI.Models.Enums;

namespace HospitalTriageAI.AI;

/// <summary>
/// ML.NET prediction engine wrapper for triage risk assessment.
/// Falls back to rule-based prediction if no trained model is available.
/// </summary>
public class TriagePredictionEngine
{
    private readonly MLContext _mlContext;
    private PredictionEngine<ModelInput, ModelOutput>? _predictionEngine;
    private bool _modelLoaded = false;
    
    public TriagePredictionEngine()
    {
        _mlContext = new MLContext(seed: 42);
        TryLoadModel();
    }
    
    private void TryLoadModel()
    {
        try
        {
            var modelPath = Path.Combine(AppContext.BaseDirectory, "AI", "Models", "TriageModel.zip");
            if (File.Exists(modelPath))
            {
                var model = _mlContext.Model.Load(modelPath, out _);
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);
                _modelLoaded = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ML Model not loaded, using rule-based prediction: {ex.Message}");
            _modelLoaded = false;
        }
    }
    
    /// <summary>
    /// Predicts triage level using ML model or rule-based fallback
    /// </summary>
    public RiskPrediction Predict(TriageAssessment assessment, int patientAge)
    {
        var input = ConvertToInput(assessment, patientAge);
        
        if (_modelLoaded && _predictionEngine != null)
        {
            return PredictWithML(input);
        }
        
        // Fallback to rule-based prediction (works without trained model)
        return PredictWithRules(assessment, patientAge);
    }
    
    private ModelInput ConvertToInput(TriageAssessment assessment, int age)
    {
        return new ModelInput
        {
            Age = age,
            HeartRate = assessment.HeartRate,
            BloodPressureSystolic = assessment.BloodPressureSystolic,
            BloodPressureDiastolic = assessment.BloodPressureDiastolic,
            Temperature = assessment.Temperature,
            RespiratoryRate = assessment.RespiratoryRate,
            OxygenSaturation = assessment.OxygenSaturation,
            PainLevel = assessment.PainLevel,
            ChestPain = (float)assessment.ChestPain,
            ShortnessOfBreath = (float)assessment.ShortnessOfBreath,
            AlteredConsciousness = (float)assessment.AlteredConsciousness,
            Bleeding = (float)assessment.Bleeding,
            Fever = (float)assessment.Fever
        };
    }
    
    private RiskPrediction PredictWithML(ModelInput input)
    {
        var output = _predictionEngine!.Predict(input);
        var level = (TriageLevel)(int)Math.Round(output.PredictedTriageLevel);
        
        // Ensure valid range
        if (!Enum.IsDefined(typeof(TriageLevel), level) || level == TriageLevel.Unassessed)
            level = TriageLevel.Standard;
        
        float confidence = output.Scores.Length > 0 ? output.Scores.Max() : 0.8f;
        
        return new RiskPrediction
        {
            PredictedLevel = level,
            RiskScore = CalculateRiskScore(level, confidence),
            Confidence = confidence,
            RiskFactors = IdentifyRiskFactors(input),
            Recommendation = GetRecommendation(level)
        };
    }
    
    /// <summary>
    /// Rule-based prediction when ML model is not available
    /// </summary>
    private RiskPrediction PredictWithRules(TriageAssessment a, int age)
    {
        var riskFactors = new List<string>();
        int urgencyScore = 0;
        
        // === EMERGENCY CONDITIONS (Score 90-100) ===
        
        // Altered consciousness is always emergency
        if (a.AlteredConsciousness >= SymptomSeverity.Moderate)
        {
            urgencyScore += 40;
            riskFactors.Add("Altered consciousness");
        }
        
        // Severe chest pain
        if (a.ChestPain >= SymptomSeverity.Severe)
        {
            urgencyScore += 35;
            riskFactors.Add("Severe chest pain");
        }
        
        // Critically low oxygen
        if (a.OxygenSaturation < 90)
        {
            urgencyScore += 35;
            riskFactors.Add($"Critical O2 saturation: {a.OxygenSaturation}%");
        }
        
        // Severe breathing difficulty
        if (a.ShortnessOfBreath >= SymptomSeverity.Severe)
        {
            urgencyScore += 30;
            riskFactors.Add("Severe breathing difficulty");
        }
        
        // === URGENT CONDITIONS (Score 60-89) ===
        
        // Abnormal heart rate
        if (a.HeartRate > 120 || a.HeartRate < 50)
        {
            urgencyScore += 20;
            riskFactors.Add($"Abnormal heart rate: {a.HeartRate} bpm");
        }
        
        // High blood pressure
        if (a.BloodPressureSystolic > 180 || a.BloodPressureSystolic < 90)
        {
            urgencyScore += 18;
            riskFactors.Add($"Abnormal BP: {a.BloodPressureSystolic}/{a.BloodPressureDiastolic}");
        }
        
        // High fever
        if (a.Temperature > 39.5f)
        {
            urgencyScore += 15;
            riskFactors.Add($"High fever: {a.Temperature}°C");
        }
        
        // Moderate bleeding
        if (a.Bleeding >= SymptomSeverity.Moderate)
        {
            urgencyScore += 18;
            riskFactors.Add("Significant bleeding");
        }
        
        // Severe pain
        if (a.PainLevel >= 8)
        {
            urgencyScore += 15;
            riskFactors.Add($"Severe pain level: {a.PainLevel}/10");
        }
        
        // Low oxygen (but not critical)
        if (a.OxygenSaturation >= 90 && a.OxygenSaturation < 94)
        {
            urgencyScore += 12;
            riskFactors.Add($"Low O2 saturation: {a.OxygenSaturation}%");
        }
        
        // === STANDARD CONDITIONS (Score 30-59) ===
        
        // Mild chest pain
        if (a.ChestPain == SymptomSeverity.Mild || a.ChestPain == SymptomSeverity.Moderate)
        {
            urgencyScore += 12;
            riskFactors.Add("Chest discomfort");
        }
        
        // Moderate fever
        if (a.Temperature >= 38.5f && a.Temperature <= 39.5f)
        {
            urgencyScore += 8;
            riskFactors.Add($"Fever: {a.Temperature}°C");
        }
        
        // Moderate pain
        if (a.PainLevel >= 5 && a.PainLevel < 8)
        {
            urgencyScore += 8;
            riskFactors.Add($"Moderate pain: {a.PainLevel}/10");
        }
        
        // === AGE ADJUSTMENTS ===
        if (age > 65)
        {
            urgencyScore += 10;
            riskFactors.Add("Elderly patient (>65 years)");
        }
        else if (age < 5)
        {
            urgencyScore += 8;
            riskFactors.Add("Pediatric patient (<5 years)");
        }
        
        // Normalize score to 0-1
        float riskScore = Math.Min(urgencyScore / 100f, 1f);
        
        // Determine triage level
        TriageLevel level = urgencyScore switch
        {
            >= 70 => TriageLevel.Emergency,
            >= 45 => TriageLevel.Urgent,
            >= 20 => TriageLevel.Standard,
            _ => TriageLevel.NonUrgent
        };
        
        // If no risk factors identified, add a default
        if (riskFactors.Count == 0)
        {
            riskFactors.Add("Vitals within normal range");
        }
        
        return new RiskPrediction
        {
            PredictedLevel = level,
            RiskScore = riskScore,
            Confidence = 0.85f, // Rule-based has fixed confidence
            RiskFactors = riskFactors,
            Recommendation = GetRecommendation(level)
        };
    }
    
    private List<string> IdentifyRiskFactors(ModelInput input)
    {
        var factors = new List<string>();
        
        if (input.OxygenSaturation < 94) factors.Add("Low oxygen saturation");
        if (input.HeartRate > 100 || input.HeartRate < 60) factors.Add("Abnormal heart rate");
        if (input.Temperature > 38.5f) factors.Add("Elevated temperature");
        if (input.ChestPain >= 2) factors.Add("Chest pain reported");
        if (input.ShortnessOfBreath >= 2) factors.Add("Breathing difficulty");
        if (input.PainLevel >= 7) factors.Add("High pain level");
        if (input.Age > 65) factors.Add("Elderly patient");
        
        return factors;
    }
    
    private float CalculateRiskScore(TriageLevel level, float confidence)
    {
        float baseScore = level switch
        {
            TriageLevel.Emergency => 0.9f,
            TriageLevel.Urgent => 0.7f,
            TriageLevel.Standard => 0.4f,
            TriageLevel.NonUrgent => 0.2f,
            _ => 0.5f
        };
        
        return baseScore * confidence;
    }
    
    private string GetRecommendation(TriageLevel level)
    {
        return level switch
        {
            TriageLevel.Emergency => "IMMEDIATE medical attention required. Alert trauma/resuscitation team.",
            TriageLevel.Urgent => "Patient needs to be seen within 15 minutes by a physician.",
            TriageLevel.Standard => "Patient can wait up to 1 hour. Monitor for any changes.",
            TriageLevel.NonUrgent => "Patient can safely wait. Consider walk-in clinic if available.",
            _ => "Please complete assessment to determine triage level."
        };
    }
}
