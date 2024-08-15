using CommunityToolkit.Mvvm.ComponentModel;
using KeeperApp.Records.ViewAttributes;
using KeeperApp.Security;
using System;
using System.Text.Json.Serialization;

namespace KeeperApp.Records;

[JsonDerivedType(typeof(Folder), typeDiscriminator:"folder")]
[JsonDerivedType(typeof(LoginRecord), typeDiscriminator:"login")]
[JsonDerivedType(typeof(CardCredentialsRecord), typeDiscriminator:"card")]
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
    [Hidden]
    public int? ParentId { get; set; }
    [Hidden]
    public Folder? Parent { get; set; }

    [EncryptProperty, RequiredProperty]
    public string Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }
}
