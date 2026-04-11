using Bank4Us.Domain.DomainObjects;
using Bank4Us.Infrastructure;

namespace Bank4Us.Domain.Repositories
{
    public interface IAccountRepository
    {
        int CreateAccount(Account account, BankingContext bankingContext);

        Account? GetAccountById(int id, BankingContext bankingContext);

        void Update(Account account, BankingContext bankingContext);
    }

    public class AccountRepository
    {
        public int CreateAccount(Account account, BankingContext bankingContext)
        {
            // Create account
            bankingContext.Accounts.Add(account);
            bankingContext.SaveChanges();

            // Id should be set on entity
            return account.Id;
        }

        public Account? GetAccountById(int id, BankingContext bankingContext)
        {
            Account? account = bankingContext.Accounts.Find(id);
            return account;
        }

        public void Update(Account account, BankingContext bankingContext)
        {
            bankingContext.Update(account);
            return;
        }
    }
}
