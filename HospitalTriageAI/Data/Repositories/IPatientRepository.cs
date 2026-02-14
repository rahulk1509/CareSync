using HospitalTriageAI.Models;

namespace HospitalTriageAI.Data.Repositories;

/// <summary>
/// Repository interface for Patient operations
/// </summary>
public interface IPatientRepository
{
    Task<List<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(int id);
    Task<Patient?> GetByIdWithAssessmentsAsync(int id);
    Task<List<Patient>> GetRecentPatientsAsync(int count = 10);
    Task<Patient> AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(int id);
    Task<List<Patient>> SearchAsync(string searchTerm);
}
