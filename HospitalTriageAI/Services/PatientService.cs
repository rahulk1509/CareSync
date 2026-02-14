using HospitalTriageAI.Data.Repositories;
using HospitalTriageAI.Models;

namespace HospitalTriageAI.Services;

/// <summary>
/// Patient service implementation
/// </summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;
    
    public PatientService(IPatientRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<List<Patient>> GetAllPatientsAsync()
    {
        return await _repository.GetAllAsync();
    }
    
    public async Task<Patient?> GetPatientAsync(int id)
    {
        return await _repository.GetByIdWithAssessmentsAsync(id);
    }
    
    public async Task<Patient> CreatePatientAsync(Patient patient)
    {
        // Generate medical record number if not provided
        if (string.IsNullOrEmpty(patient.MedicalRecordNumber))
        {
            patient.MedicalRecordNumber = GenerateMRN();
        }
        
        return await _repository.AddAsync(patient);
    }
    
    public async Task UpdatePatientAsync(Patient patient)
    {
        await _repository.UpdateAsync(patient);
    }
    
    public async Task DeletePatientAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
    
    public async Task<List<Patient>> SearchPatientsAsync(string searchTerm)
    {
        return await _repository.SearchAsync(searchTerm);
    }
    
    private string GenerateMRN()
    {
        return $"MRN{DateTime.Now:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
    }
}
