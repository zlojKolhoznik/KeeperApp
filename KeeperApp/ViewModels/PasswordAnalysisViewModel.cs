using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Easy_Password_Validator;
using KeeperApp.Database;
using KeeperApp.Models;
using KeeperApp.Records;
using KeeperApp.Security.Authentication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace KeeperApp.ViewModels
{
    public class PasswordAnalysisViewModel : ObservableObject
    {
        private readonly PasswordValidatorService validator;
        private readonly IEnumerable<LoginRecord> records;
        private readonly ResourceLoader resourceLoader;

        public PasswordAnalysisViewModel(KeeperDbContext context, SignInManager signInManager, PasswordValidatorService validator)
        {
            this.validator = validator;
            records = context.GetRecordsForUser(signInManager.CurrentUserName).OfType<LoginRecord>();
            Results.CollectionChanged += (s, e) => OnPropertyChanged(nameof(AverageScore));
            resourceLoader = new ResourceLoader();
        }

        public string Title => resourceLoader.GetString("PasswordAnalysisTitle");
        public string AverageScoreLabel => resourceLoader.GetString("AverageScoreLabel");
        public string AnalyzePasswordsLabel => resourceLoader.GetString("AnalyzePasswordsLabel");


        public ObservableCollection<PasswordAnalysisResult> Results { get; } = [];
        public double AverageScore => Results.Count > 0 ? Math.Round(Results.Average(r => r.Score), 2) : 0;

        public RelayCommand AnalyzePasswordsCommand => new(AnalyzePasswords);

        private void AnalyzePasswords()
        {
            Results.Clear();
            foreach (var record in records)
            {
                var pass = validator.TestAndScore(record.Password);
                var analysisResult = new PasswordAnalysisResult
                {
                    Record = record,
                    Pass = pass,
                    Score = validator.Score,
                    FailureMessages = validator.FailureMessages.ToArray()
                };
                Results.Add(analysisResult);
            }
        }
    }
}
