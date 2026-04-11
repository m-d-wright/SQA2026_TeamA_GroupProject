using Bank4Us.Domain.DomainObjects;
using Bank4Us.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Bank4Us.Domain.Repositories
{

    public interface ITransferRepository
    {
        int CreateTransfer(Transfer transfer, BankingContext bankingContext);

        Transfer? GetTransferById(int id, BankingContext bankingContext);
    }


    public class TransferRepository : ITransferRepository
    {
        public int CreateTransfer(Transfer transfer, BankingContext bankingContext)
        {
            // Create transfer
            bankingContext.Transfers.Add(transfer);
            bankingContext.SaveChanges();

            // Id should be set on enttiy
            return transfer.Id;
        }

        public Transfer? GetTransferById(int id, BankingContext bankingContext)
        {
            Transfer? transfer = bankingContext.Transfers.Find(id);
            return transfer;
        }
    }
}
