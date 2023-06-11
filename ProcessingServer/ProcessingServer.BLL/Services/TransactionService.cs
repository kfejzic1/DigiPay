using AutoMapper;
using Org.BouncyCastle.Crypto.Agreement.JPake;
using Org.BouncyCastle.Tls;
using ProcessingServer.BLL.DTO;
using ProcessingServer.BLL.Interfaces;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Interfaces;
using ProcessingServer.DAL.Other;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Authentication;

namespace ProcessingServer.BLL.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAdministrationService _administrationService;

        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper, IAdministrationService administrationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _administrationService = administrationService;
        }

        #region Validation Functions
        public bool ValidateAmountFilterRange(TransactionsFilter filter)
        {
            if ((filter.AmountStartFilter != null && filter.AmountEndFilter == null) || (filter.AmountEndFilter != null && filter.AmountStartFilter == null))
                throw new ArgumentException("Invalid amount range filter!");
            bool amountFilterActive = filter.AmountEndFilter != null && filter.AmountStartFilter != null;
            if (amountFilterActive && double.Parse(filter.AmountStartFilter) > double.Parse(filter.AmountEndFilter))
                throw new ArgumentException("Invalid amount range filter!");
            return amountFilterActive;
        }

        private bool ValidateCreatedAtFilterRange(TransactionsFilter filter)
        {
            if ((filter.CreatedAtStartFilter != null && filter.CreatedAtEndFilter == null) || (filter.CreatedAtEndFilter != null && filter.CreatedAtStartFilter == null))
                throw new ArgumentException("Invalid date range filter!");
            bool createdOnFilterActive = filter.CreatedAtEndFilter != null && filter.CreatedAtStartFilter != null;
            if (createdOnFilterActive && Convert.ToDateTime(filter.CreatedAtStartFilter) > Convert.ToDateTime(filter.CreatedAtEndFilter))
                throw new ArgumentException("Invalid date range filter!");
            return createdOnFilterActive;
        }

        private bool ValidateSortingOrder(string sortingOrder)
        {
            string[] validValues =
            {
                "default", "amountasc", "amountdesc", "currencyasc", "currencydesc", "transactiontypeasc", "transactiontypedesc", "createdatasc", "createdatdesc",
                "categoryasc", "categorydesc", "recipientnameasc", "recipientnamedesc"
            };
            return validValues.Contains(sortingOrder.ToLower());
        }

        private void ValidateTransactionType(string transactionType, string senderType, string recipientType)
        {
            var validValues = new List<(string SenderType, string RecipientType, string TransactionType)>
            {
                ("person", "person", "c2c"),
                ("person", "company", "c2b"),
                ("company", "company", "b2b"),
            };
            if (!validValues.Contains((senderType.ToLower(), recipientType.ToLower(), transactionType.ToLower())))
                throw new Exception("Invalid sender type, recipient type or transaction type!");
        }

        private async Task<double> ValidateExchangeRate(string token, UserBankAccount senderAccount, UserBankAccount recipientAccount)
        {
            if (senderAccount.Currency.ToLower().Equals(recipientAccount.Currency.ToLower()))
                return 1;
            try
            {
                var exchangeRates = await _administrationService.GetExchangeRatesFromAdministrationApi(token);
                foreach (var exchangeRate in exchangeRates)
                {
                    if (exchangeRate.StartDate > DateTime.Now || (exchangeRate.EndDate != null && exchangeRate.EndDate < DateTime.Now))
                        continue;
                    var inputCurrency = exchangeRate.InputCurrency.Substring(exchangeRate.InputCurrency.IndexOf('(') + 1, 3).ToLower();
                    var outputCurrency = exchangeRate.OutputCurrency.Substring(exchangeRate.OutputCurrency.IndexOf('(') + 1, 3).ToLower();
                    if (inputCurrency.Equals(senderAccount.Currency.ToLower()) && outputCurrency.Equals(recipientAccount.Currency.ToLower()))
                        return exchangeRate.Rate;
                }
                throw new Exception("Exhange rate from " + senderAccount.Currency.ToUpper() + " to " + recipientAccount.Currency.ToUpper() + " is not available!");
            }
            catch (NullReferenceException)
            {
                throw new Exception("Exhange rate from " + senderAccount.Currency.ToUpper() + " to " + recipientAccount.Currency.ToUpper() + " is not available!");
            }
        }

        private void ValidateAccounts(UserBankAccount senderAccount, UserBankAccount recipientAccount)
        {
            if (senderAccount.AccountNumber.Equals(recipientAccount.AccountNumber))
                throw new Exception("Unable to make transaction to the same account!");
        }
        #endregion

        #region Mapping Functions
        private async Task<UserWithBankAccount> MapToUserWithAccount(UserBankAccount account)
        {
            var user = await _unitOfWork.UserRepository.GetUserById(account.OwnerId);
            var userWithAccount = _mapper.Map<UserWithBankAccount>(user);
            userWithAccount.AccountNumber = account.AccountNumber;
            userWithAccount.BankName = account.BankName;
            return userWithAccount;
        }

        private async Task<TransactionResponse> MapToTransactionReturn(Transaction transaction)
        {
            var transactionReturn = _mapper.Map<TransactionResponse>(transaction);
            transactionReturn.Sender = await MapToUserWithAccount(transaction.SenderAccount);
            transactionReturn.Recipient = await MapToUserWithAccount(transaction.RecipientAccount);
            return transactionReturn;
        }

        private async Task<List<TransactionResponse>> MapToTransactionReturn(List<Transaction> transactions)
        {
            var result = new List<TransactionResponse>();
            foreach (var transaction in transactions)
                result.Add(await MapToTransactionReturn(transaction));
            return result;
        }

        private Transaction MapToTransaction(TransactionRequest transactionDto, UserBankAccount senderAccount, UserBankAccount recipientAccount)
        {
            var transaction = _mapper.Map<Transaction>(transactionDto);
            transaction.SenderAccountId = senderAccount.UserBankAccountId;
            transaction.SenderAccount = senderAccount;
            transaction.RecipientAccountId = recipientAccount.UserBankAccountId;
            transaction.RecipientAccount = recipientAccount;
            transaction.CreatedAt = DateTime.Now;
            transaction.TransactionType = transaction.TransactionType.ToUpper();
            return transaction;
        }

        private async Task<List<TransactionsGroupResponse>> MapToTransactionsGroupReturn(List<TransactionsGroup> transactions)
        {
            List<TransactionsGroupResponse> result = new List<TransactionsGroupResponse>();
            foreach (var transaction in transactions)
            {
                var transactionsReturn = await MapToTransactionReturn(transaction.Transactions);
                result.Add(new TransactionsGroupResponse(transaction.KeyValue, transactionsReturn, transaction.TotalAmount, transaction.NumberOfTransactions));
            }
            return result;
        }
        #endregion

        #region Transactions List Manipulating Functions
        private Tuple<int, int> SetPageStartAndPageEnd(int pageNumber, int pageSize, int listSize)
        {
            if (listSize == 0)
                throw new NullReferenceException("No matching transactions!");
            int pageStart = (pageNumber - 1) * pageSize;
            if (pageNumber < 1 || pageStart > listSize - 1)
                throw new ArgumentOutOfRangeException("Invalid page number!");
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("Invalid page size!");
            int pageEnd = pageStart + pageSize > listSize ? listSize - pageStart : pageSize;
            return Tuple.Create(pageStart, pageEnd);
        }

        private List<TransactionResponse> FilterTransactions(List<TransactionResponse> transactions, TransactionsFilter filter)
        {
            return transactions.Where(transaction => (ValidateAmountFilterRange(filter) ? transaction.Amount >= double.Parse(filter.AmountStartFilter) && transaction.Amount <= double.Parse(filter.AmountEndFilter) : true) &&
                                     (filter.CurrencyFilter != null ? transaction.Currency.ToLower().Equals(filter.CurrencyFilter.ToLower()) : true) &&
                                     (filter.TransactionTypeFilter != null ? transaction.TransactionType.ToLower().Equals(filter.TransactionTypeFilter.ToLower()) : true) &&
                                     (filter.RecipientNameFilter != null ? transaction.Recipient.Name.ToLower().Equals(filter.RecipientNameFilter.ToLower()) : true) &&
                                     (filter.RecipientAccountNumberFilter != null ? transaction.Recipient.AccountNumber.ToLower().Equals(filter.RecipientAccountNumberFilter.ToLower()) : true) &&
                                     (ValidateCreatedAtFilterRange(filter) ? transaction.CreatedAt >= Convert.ToDateTime(filter.CreatedAtStartFilter) && transaction.CreatedAt <= Convert.ToDateTime(filter.CreatedAtEndFilter) : true) &&
                                     (filter.SenderNameFilter != null ? transaction.Sender.Name.ToLower().Equals(filter.SenderNameFilter.ToLower()) : true) &&
                                     (filter.CategoryFilter != null ? transaction.Category.ToLower().Equals(filter.CategoryFilter.ToLower()) : true))
                               .ToList();
        }

        private List<TransactionResponse> SortTransactions(List<TransactionResponse> transactions, string sortingOrder)
        {
            sortingOrder = sortingOrder.ToLower();
            if (!ValidateSortingOrder(sortingOrder))
                throw new Exception(); // Catch will send message invalid parameter format
            if (sortingOrder.Equals("default"))
                return transactions.OrderBy(transaction => transaction.TransactionId)
                                   .ToList();
            if (sortingOrder.Equals("recipientnameasc"))
                return transactions.OrderBy(transaction => transaction.Recipient.Name)
                                   .ToList();
            if (sortingOrder.Equals("recipientnamedesc"))
                return transactions.OrderByDescending(transaction => transaction.Recipient.Name)
                                   .ToList();
            if (sortingOrder.Equals("amountasc"))
                return transactions.OrderBy(transaction => transaction.Amount)
                                   .ToList();
            if (sortingOrder.Equals("amountdesc"))
                return transactions.OrderByDescending(transaction => transaction.Amount)
                                   .ToList();
            if (sortingOrder.Equals("currencyasc"))
                return transactions.OrderBy(transaction => transaction.Currency)
                                   .ToList();
            if (sortingOrder.Equals("currencydesc"))
                return transactions.OrderByDescending(transaction => transaction.Currency)
                                   .ToList();
            if (sortingOrder.Equals("transactiontypeasc"))
                return transactions.OrderBy(transaction => transaction.TransactionType)
                                   .ToList();
            if (sortingOrder.Equals("transactiontypedesc"))
                return transactions.OrderByDescending(transaction => transaction.TransactionType)
                                   .ToList();
            if (sortingOrder.Equals("createdatasc"))
                return transactions.OrderBy(transaction => transaction.CreatedAt)
                                   .ToList();
            if (sortingOrder.Equals("createdatdesc"))
                return transactions.OrderByDescending(transaction => transaction.CreatedAt)
                                   .ToList();
            if (sortingOrder.Equals("categoryasc"))
                return transactions.OrderBy(transaction => transaction.Category)
                                   .ToList();
            return transactions.OrderByDescending(transaction => transaction.Category)
                                   .ToList();
        }

        private async Task<List<TransactionsGroup>> GroupTransactions(string token, Func<User, Task<List<TransactionsGroup>>> GroupingFunction)
        {
            var user = await _administrationService.GetUserFromAdministrationApi(token, _mapper);
            var transactions = await GroupingFunction(user);
            if (transactions.Count == 0)
                throw new NullReferenceException("No matching transactions!");
            return transactions;
        }
        #endregion

        private async Task<UserBankAccount> GetAccountForSender(string token, string accountNumber)
        {
            var sender = await _administrationService.GetUserFromAdministrationApi(token, _mapper); // Returns type User!
            var account = await _unitOfWork.UserBankAccountRepository.GetAccountByAccountNumber(accountNumber);
            if (account == null || !account.OwnerId.Equals(sender.UserId))
                throw new Exception("Invalid sender account number!");
            return account;
        }

        private async Task<UserBankAccount> GetAccountForRecipient(string token, RecipientRequest recipientDto)
        {
            var recipient = await _administrationService.GetRecipientFromAdministrationApi(token, recipientDto.Name, _mapper); // Returns type User!
            var account = await _unitOfWork.UserBankAccountRepository.GetAccountByAccountNumber(recipientDto.AccountNumber);
            if (account == null || !account.OwnerId.Equals(recipient.UserId))
                throw new Exception("Invalid recipient name or account number!");
            return account;
        }

        private void IncreaseMoney(UserBankAccount account, double amount)
        {
            account.Credit += amount;
            account.Total += amount;
        }

        private void DecreaseMoney(UserBankAccount account, string transactionCurrency, double amount)
        {
            if (!account.Currency.ToLower().Equals(transactionCurrency.ToLower()))
                throw new Exception("Sender account currency and transaction currency must match!");
            account.Debit -= amount; // Debit should always be negative.
            account.Total -= amount; // Total = Credit - |Debit|
        }

        #region Main Routing Functions
        public async Task<List<TransactionResponse>> GetTransactionsForUser(string token, string pageNumber, string pageSize, TransactionsFilter filter, string sortingOrder)
        {
            var sender = await _administrationService.GetUserFromAdministrationApi(token, _mapper);
            var usersTransactions = await _unitOfWork.TransactionRepository.GetTransactionsForUser(sender);
            var usersTransactionsReturn = await MapToTransactionReturn(usersTransactions);
            usersTransactionsReturn = SortTransactions(FilterTransactions(usersTransactionsReturn, filter), sortingOrder);
            var range = SetPageStartAndPageEnd(int.Parse(pageNumber), int.Parse(pageSize), usersTransactionsReturn.Count);
            return usersTransactionsReturn.GetRange(range.Item1, range.Item2);
        }

        public async Task<TransactionResponse> CreateTransaction(string token, TransactionRequest transactionDto)
        {
            var senderAccount = await GetAccountForSender(token, transactionDto.Sender.AccountNumber);
            var recipientAccount = await GetAccountForRecipient(token, transactionDto.Recipient);
            ValidateAccounts(senderAccount, recipientAccount);
            var rate = await ValidateExchangeRate(token, senderAccount, recipientAccount);
            ValidateTransactionType(transactionDto.TransactionType, senderAccount.Owner.Type, recipientAccount.Owner.Type);
            if (transactionDto.Amount <= 0)
                throw new Exception("Amount must be positive number!");
            IncreaseMoney(recipientAccount, transactionDto.Amount * rate);
            DecreaseMoney(senderAccount, transactionDto.Currency, transactionDto.Amount);
            var transaction = MapToTransaction(transactionDto, senderAccount, recipientAccount);
            var createdTransaction = await _unitOfWork.TransactionRepository.CreateTransaction(transaction);
            await _unitOfWork.SaveAsync();
            var result = await MapToTransactionReturn(createdTransaction);
            return result;
        }

        public async Task<List<TransactionsGroupResponse>> GroupTransactionsByCurrency(string token)
        {
            var transactions = await GroupTransactions(token, _unitOfWork.TransactionRepository.GroupTransactionsByCurrency);
            var transactionsReturn = await MapToTransactionsGroupReturn(transactions);
            return transactionsReturn;
        }

        public async Task<List<TransactionsGroupResponse>> GroupTransactionsByType(string token)
        {
            var transactions = await GroupTransactions(token, _unitOfWork.TransactionRepository.GroupTransactionsByType);
            var transactionsReturn = await MapToTransactionsGroupReturn(transactions);
            return transactionsReturn;
        }

        public async Task<TransactionResponse> GetTransactionById(string token, string transactionId)
        {
            var user = await _administrationService.GetUserFromAdministrationApi(token, _mapper);
            var transaction = await _unitOfWork.TransactionRepository.GetTransactionById(int.Parse(transactionId));
            if (transaction == null)
                throw new NullReferenceException("Transaction with provided ID does not exist!");
            if (!(transaction.SenderAccount.OwnerId.Equals(user.UserId) || transaction.RecipientAccount.OwnerId.Equals(user.UserId)))
                throw new AuthenticationException("The user cannot see transactions in which he is not involved!");
            var transactionResponse = await MapToTransactionReturn(transaction);
            return transactionResponse;
        }
        #endregion
    }
}
