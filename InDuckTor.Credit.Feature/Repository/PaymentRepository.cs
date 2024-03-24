using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Infrastructure.Config.Database;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Repository;

public class PaymentRepository(LoanDbContext context) : IPaymentRepository
{
    public async Task<List<Payment>> GetAllNonDistributedPayments(long loanId)
    {
        return await context.Payments.Where(payment => !payment.PaymentDistribution.IsDistributed).ToListAsync();
    }
}