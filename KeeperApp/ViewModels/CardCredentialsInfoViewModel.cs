using KeeperApp.Database;
using KeeperApp.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.ViewModels
{
    public class CardCredentialsInfoViewModel : RecordInfoViewModel
    {
        public CardCredentialsInfoViewModel(KeeperDbContext dbContext) : base(dbContext)
        {
        }

        public new CardCredentialsRecord Record
        {
            get => (CardCredentialsRecord)record;
            set => SetProperty(ref record, value, nameof(Record));
        }

        public string CardNumberLabel => resourceLoader.GetString("CardNumber");
        public string ExpirationDateLabel => resourceLoader.GetString("ExpirationDate");
        public string CvvLabel => resourceLoader.GetString("Cvv");

        public override void SetRecordById(int recordId)
        {
            Record = dbContext.CardCredentials.Find(recordId);
        }
    }
}
