using HospitalTriageAI.Models;

namespace HospitalTriageAI.Services;

/// <summary>
/// Service interface for patient operations
/// </summary>
public interface IPatientService
{
    Task<List<Patient>> GetAllPatientsAsync();
    Task<Patient?> GetPatientAsync(int id);
    Task<Patient> CreatePatientAsync(Patient patient);
    Task UpdatePatientAsync(Patient patient);
    Task DeletePatientAsync(int id);
    Task<List<Patient>> SearchPatientsAsync(string searchTerm);
}
