using KeeperApp.Authentication;
using KeeperApp.Database;
using KeeperApp.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.ViewModels
{
    public class AddCardCredentialsViewModel : AddRecordViewModel
    {
        private string cardNumber;
        private string expiryDate;
        private string cvv;

        public AddCardCredentialsViewModel(KeeperDbContext dbContext, SignInManager signInManager) : base(dbContext, signInManager)
        {
        }

        public string CardNumber 
        { 
            get => cardNumber;
            set => SetProperty(ref cardNumber, value, nameof(CardNumber));
        }

        public string ExpiryDate 
        { 
            get => expiryDate;
            set => SetProperty(ref expiryDate, value, nameof(ExpiryDate));
        }

        public string Cvv 
        { 
            get => cvv; 
            set => SetProperty(ref cvv, value, nameof(Cvv));
        }

        public string CardNumberPlaceholder => resourceLoader.GetString("CardNumber");
        public string ExpiryDatePlaceholder => resourceLoader.GetString("ExpiryDate");
        public string CvvPlaceholder => resourceLoader.GetString("Cvv");

        protected override async Task SaveRecordAsync()
        {
            Record = new CardCredentialsRecord
            {
                CardNumber = CardNumber,
                ExpiryDate = ExpiryDate,
                Cvv = Cvv
            };
            await base.SaveRecordAsync();
        }

        public override bool IsInputValid()
        {
            return !string.IsNullOrWhiteSpace(RecordTitle) && !string.IsNullOrWhiteSpace(CardNumber) && !string.IsNullOrWhiteSpace(ExpiryDate) && !string.IsNullOrWhiteSpace(Cvv);
        }
    }
}
