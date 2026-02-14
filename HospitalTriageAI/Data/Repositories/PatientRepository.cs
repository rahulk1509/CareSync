using Microsoft.EntityFrameworkCore;
using HospitalTriageAI.Models;

namespace HospitalTriageAI.Data.Repositories;

/// <summary>
/// SQLite repository implementation for Patient
/// </summary>
public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _context;
    
    public PatientRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Patient>> GetAllAsync()
    {
        return await _context.Patients
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<Patient?> GetByIdAsync(int id)
    {
        return await _context.Patients.FindAsync(id);
    }
    
    public async Task<Patient?> GetByIdWithAssessmentsAsync(int id)
    {
        return await _context.Patients
            .Include(p => p.Assessments)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<List<Patient>> GetRecentPatientsAsync(int count = 10)
    {
        return await _context.Patients
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
    
    public async Task<Patient> AddAsync(Patient patient)
    {
        patient.CreatedAt = DateTime.Now;
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient;
    }
    
    public async Task UpdateAsync(Patient patient)
    {
        patient.LastUpdated = DateTime.Now;
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient != null)
        {
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<List<Patient>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _context.Patients
            .Where(p => p.FirstName.ToLower().Contains(term) 
                     || p.LastName.ToLower().Contains(term)
                     || (p.MedicalRecordNumber != null && p.MedicalRecordNumber.Contains(term)))
            .ToListAsync();
    }
}
