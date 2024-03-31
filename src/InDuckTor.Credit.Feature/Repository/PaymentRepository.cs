using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Infrastructure.Config.Database;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Repository;

public class PaymentRepository : IPaymentRepository
{
    public PaymentRepository(LoanDbContext context)
    {
        _context = context;
    }

    private readonly LoanDbContext _context;

    public async Task<List<Payment>> GetAllNonDistributedPayments(long loanId)
    {
        return await _context.Payments
            .Include(p => p.BillingsPayoffs)
            .ThenInclude(bp => bp.PeriodBilling)
            .Where(payment => !payment.IsDistributed && payment.LoanId == loanId).ToListAsync();
    }
}