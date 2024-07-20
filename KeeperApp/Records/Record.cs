using CommunityToolkit.Mvvm.ComponentModel;
using KeeperApp.Records.ViewAttributes;
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

    [Hidden]
    public int Id { get; set; }
    [Hidden]
    public DateTime Created { get; set; }
    [Hidden]
    public DateTime Modified { get; set; }
    [Hidden]
    public bool IsDeleted { get; set; }
    [Hidden]
    public string OwnerUsernameHash { get; set; }
    [Hidden]
    public abstract string IconPath { get; set; }
    [Hidden]
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
