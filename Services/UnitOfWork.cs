using IP2C_Web_API.Interface;

namespace IP2C_Web_API.Services;

    public class UnitOfWork : IUnitOfWork
    {
        readonly MasterContext _context;

        public IIPDetails IPDetails { get; private set; }
        public IReport Report { get; private set; }

        public UnitOfWork(MasterContext context, IIPDetails ipdetails, IReport report)
        {
            _context = context;
            this.IPDetails = ipdetails;
            this.Report = report;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }        
    }

