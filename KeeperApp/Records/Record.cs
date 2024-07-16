using CommunityToolkit.Mvvm.ComponentModel;
using KeeperApp.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KeeperApp.Records;

public abstract class Record : ObservableObject
{
    private string title;

    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public bool IsDeleted { get; set; }
    public string OwnerUsernameHash { get; set; }
    public abstract string IconPath { get; set; }

    public abstract string Subtitle { get; }

    [EncryptProperty]
    public string Title
    {
        get => title;
        set
        {
            title = value;
            OnPropertyChanged();
        }
    }
}
