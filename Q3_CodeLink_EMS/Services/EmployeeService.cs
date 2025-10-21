using Microsoft.EntityFrameworkCore;
using Q3_CodeLink_EMS.Data;
using Q3_CodeLink_EMS.Models;

namespace Q3_CodeLink_EMS.Services
{
    public class EmployeeService
    {
        private readonly CodeLinkEmsDbContext dbContext;
        private readonly ILogger<EmployeeService> logger;

        public EmployeeService(CodeLinkEmsDbContext dbContext, ILogger<EmployeeService> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        // Create
        public async Task<bool> CreateEmployee(Employee employee)
        {
            try
            {
                // Checking if email already exists
                if (await dbContext.Employees.AnyAsync(e => e.EmailAddress.ToLower() == employee.EmailAddress.ToLower()))
                {
                    return false;
                }

                // Create employee record
                employee.Id = Guid.NewGuid(); // Generating new unique Guid Id
                dbContext.Employees.Add(employee);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) // Catching and logging any unexpected error
            {
                logger.LogError(ex, "Unexpected error creating employee record: {Id} - {FullName}.", employee.Id, employee.FullName);
                return false;
            }
        }

        // Read (all)
        public async Task<List<Employee>> GetAllEmployees()
        {
            try
            {
                return await dbContext.Employees.ToListAsync();
            }
            catch (Exception ex) // Catching and logging any unexpected error
            {
                logger.LogError(ex, "Unexpected error retrieving all employee records.");
                return new List<Employee>();
            }
        }

        // Read (specified)
        public async Task<List<Employee>> SearchById(Guid id)
        {
            try
            {
                return await dbContext.Employees.Where(e => e.Id == id).ToListAsync(); // Search needs to match exaclty
            }
            catch (Exception ex) // Catching and logging any unexpected error
            {
                logger.LogError(ex, "Unexpected error searching employee record(s) with Id: {id}.", id);
                return new List<Employee>();
            }
        }

        public async Task<List<Employee>> SearchByFullName(string fullName)
        {
            try
            {
                return await dbContext.Employees.Where(e => e.FullName.ToLower().Contains(fullName.ToLower())).ToListAsync(); // Search can be partial
            }
            catch (Exception ex) // Catching and logging Exception of SingleOrDefault
            {
                logger.LogError(ex, "Unexpected error searching employee record(s) with FullName: {fullName}.", fullName);
                return new List<Employee>();
            }
        }

        public async Task<List<Employee>> SearchByEmailAddress(string emailAddress)
        {
            try
            {
                return await dbContext.Employees.Where(e => e.EmailAddress.ToLower().Contains(emailAddress.ToLower())).ToListAsync(); // Search can be partial
            }
            catch (Exception ex) // Catching and logging any unexpected error
            {
                logger.LogError(ex, "Unexpected error searching employee record(s) with EmailAddress: {emailAddress}.", emailAddress);
                return new List<Employee>();
            }
        }

        public async Task<List<Employee>> SearchByJobTitle(string? jobTitle)
        {
            try
            {
                IQueryable<Employee> query = dbContext.Employees; // LINQ query varaible

                // Search can be for nulled field in model object
                if (string.IsNullOrEmpty(jobTitle) || jobTitle.ToLower().Equals("not specified"))
                {
                    query = query.Where(e => e.JobTitle == null);
                }
                // Search can be partial
                else
                {
                    query = query.Where(e => e.JobTitle != null && e.JobTitle.ToLower().Contains(jobTitle.ToLower()));
                }
                return await query.ToListAsync();
            }
            catch (Exception ex) // Catching and logging any unexpected errort
            {
                logger.LogError(ex, "Unexpected error searching employee record(s) with JobTitle: {jobTitle}.", jobTitle);
                return new List<Employee>();
            }
        }

        public async Task<List<Employee>> SearchByDepartment(string? department)
        {
            try
            {
                IQueryable<Employee> query = dbContext.Employees; // LINQ query varaible

                // Search can be for nulled field in model object
                if (string.IsNullOrEmpty(department) || department.ToLower().Equals("not specified"))
                {
                    query = query.Where(e => e.Department == null);
                }
                // Search can be partial
                else
                {
                    query = query.Where(e => e.Department != null && e.Department.ToLower().Contains(department.ToLower()));
                }
                return await query.ToListAsync();
            }
            catch (Exception ex) // Catching and logging any unexpected error
            {
                logger.LogError(ex, "Unexpected error searching employee record(s) withy Department: {department}.", department);
                return new List<Employee>();
            }
        }

        // Update
        public async Task<bool> UpdateEmployee(Employee updatedEmployee)
        {
            try
            {
                // Checking if the id exists or if there are duplicates
                var employee = await dbContext.Employees.SingleOrDefaultAsync(e => e.Id == updatedEmployee.Id);
                if (employee == null)
                {
                    return false;
                }

                // Update employee record
                employee.FullName = updatedEmployee.FullName;
                employee.EmailAddress = updatedEmployee.EmailAddress;
                employee.JobTitle = updatedEmployee.JobTitle;
                employee.Department = updatedEmployee.Department;

                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (InvalidOperationException ex) // Catching and logging Exception of SingleOrDefault
            {
                logger.LogError(ex, "Multiple employee records found with Id: {Id}.", updatedEmployee.Id);
                return false;
            }
            catch (Exception ex) // Catching and logging any unexpected error
            {
                logger.LogError(ex, "Unexpected error updating employee record with Id: {Id}.", updatedEmployee.Id);
                return false;
            }
        }

        // Delete
        public async Task<bool> DeleteEmployee(Guid id)
        {
            try
            {
                // Checking if the id exists or if there are duplicates
                var employee = await dbContext.Employees.SingleOrDefaultAsync(e => e.Id == id);
                if (employee == null)
                {
                    return false;
                }

                dbContext.Employees.Remove(employee);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (InvalidOperationException ex) // Catching and logging Exception of SingleOrDefault
            {
                logger.LogError(ex, "Multiple employee records found with Id: {id}.", id);
                return false;
            }
            catch (Exception ex) // Catching and logging any unexpected error
            {
                logger.LogError(ex, "Unexpected error deleting employee record with Id: {id}.", id);
                return false;
            }
        }

        // Load dummy employee records
        public async Task<bool> PreloadDummyData()
        {
            try
            {
                // Hardcoded dummy employee records
                var dummyEmployees = new List<Employee>
                {
                    new Employee { FullName = "Alice Johnson", EmailAddress = "alice.johnson@example.com", JobTitle = "Software Developer", Department = "Development" },
                    new Employee { FullName = "Bob Smith", EmailAddress = "bob.smith@example.com", JobTitle = "QA Engineer", Department = "Quality Assurance" },
                    new Employee { FullName = "Charlie Brown", EmailAddress = "charlie.brown@example.com", JobTitle = null, Department = "Support" },
                    new Employee { FullName = "Diana Prince", EmailAddress = "diana.prince@example.com", JobTitle = "Project Manager", Department = null },
                    new Employee { FullName = "Ethan Hunt", EmailAddress = "ethan.hunt@example.com", JobTitle = "DevOps Engineer", Department = "Operations" },
                    new Employee { FullName = "Fiona Gallagher", EmailAddress = "fiona.gallagher@example.com", JobTitle = null, Department = null },
                    new Employee { FullName = "George Michaels", EmailAddress = "george.michaels@example.com", JobTitle = "Business Analyst", Department = "Analysis" },
                    new Employee { FullName = "Hannah White", EmailAddress = "hannah.white@example.com", JobTitle = "Software Developer", Department = "Development" },
                    new Employee { FullName = "Ian Curtis", EmailAddress = "ian.curtis@example.com", JobTitle = "QA Engineer", Department = null },
                    new Employee { FullName = "Julia Roberts", EmailAddress = "julia.roberts@example.com", JobTitle = "HR Manager", Department = "Human Resources" },
                    new Employee { FullName = "Kevin Durant", EmailAddress = "kevin.durant@example.com", JobTitle = "Software Developer", Department = "Development" },
                    new Employee { FullName = "Laura Palmer", EmailAddress = "laura.palmer@example.com", JobTitle = "Support Engineer", Department = "Support" },
                    new Employee { FullName = "Michael Scott", EmailAddress = "michael.scott@example.com", JobTitle = null, Department = "Management" },
                    new Employee { FullName = "Nancy Drew", EmailAddress = "nancy.drew@example.com", JobTitle = "QA Engineer", Department = "Quality Assurance" },
                    new Employee { FullName = "Oscar Wilde", EmailAddress = "oscar.wilde@example.com", JobTitle = "Business Analyst", Department = null },
                    new Employee { FullName = "Pam Beesly", EmailAddress = "pam.beesly@example.com", JobTitle = "HR Assistant", Department = "Human Resources" },
                    new Employee { FullName = "Quentin Tarantino", EmailAddress = "quentin.tarantino@example.com", JobTitle = null, Department = "Creative" },
                    new Employee { FullName = "Rachel Green", EmailAddress = "rachel.green@example.com", JobTitle = "Marketing Specialist", Department = "Marketing" },
                    new Employee { FullName = "Steve Rogers", EmailAddress = "steve.rogers@example.com", JobTitle = "Project Manager", Department = "Development" },
                    new Employee { FullName = "Tina Turner", EmailAddress = "tina.turner@example.com", JobTitle = "Support Engineer", Department = null },
                };

                foreach (var employee in dummyEmployees)
                {
                    // Checking if email already exists
                    if (!await dbContext.Employees.AnyAsync(e => e.EmailAddress.ToLower() == employee.EmailAddress.ToLower()))
                    {
                        employee.Id = Guid.NewGuid();
                        dbContext.Employees.Add(employee);
                    }
                }

                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) // Catching and logging any unexpected error
            {
                logger.LogError(ex, "Error loading dummy employee records.");
                return false;
            }
        }

    }
}
