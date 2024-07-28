using AsyncAwaitBestPractices;
using Caliburn.Micro;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TagsagNyilvantarto.Extensions;
using TagsagNyilvantarto.Models;

namespace TagsagNyilvantarto.ViewModels;

internal sealed class TagokEmailViewModel : PropertyChangedBase
{
    private ObservableCollection<TagokEmail> _tagokEmails;
    private ObservableCollection<TagokEmail> _selectedTags;
    private TagokEmail _selectedTag;
    private TagokEmail _selectedTag2;
    private readonly DataAccess _dataAccess;
    private readonly MsgBoxService _msgBoxService;

    public TagokEmailViewModel(DataAccess dataAccess, MsgBoxService msgBoxService)
    {
        _dataAccess = dataAccess;
        _msgBoxService = msgBoxService;
        _selectedTags = new ObservableCollection<TagokEmail>();
        GetEmailAddresses().SafeFireAndForget();
    }

    public ObservableCollection<TagokEmail> TagokEmails { get => _tagokEmails; set => _ = Set(ref _tagokEmails, value); }
    public ObservableCollection<TagokEmail> SelectedTags { get => _selectedTags; set => _ = Set(ref _selectedTags, value); }
    public TagokEmail SelectedTag { get => _selectedTag; set => _ = Set(ref _selectedTag, value); }
    public TagokEmail SelectedTag2 { get => _selectedTag2; set => _ = Set(ref _selectedTag2, value); }

    private async Task GetEmailAddresses() =>
        TagokEmails = (await _dataAccess.GetAllEmailAddressesAsync().ConfigureAwait(true)).ToObeservableCollection();


    public void Add()
    {
        if (SelectedTag is null)
            return;

        _selectedTags.Add(SelectedTag);
        _tagokEmails.Remove(SelectedTag);
    }

    public void AddAll()
    {
        SelectedTags.AddRange(TagokEmails);
        TagokEmails.Clear();
    }
    private bool SelectedTagsCountEqZero()
    {
        if (_selectedTags.Count > 0)
            return false;

        _msgBoxService.ShowNotification("A kiválasztott tagok listája üres.");
        return true;
    }
    public void Remove()
    {
        if (SelectedTagsCountEqZero())
            return;

        try
        {
            TagokEmails.Add(SelectedTag2);
            SelectedTags.Remove(SelectedTag2);
        }
        catch (Exception ex)
        {
            _msgBoxService.ShowError("Kijelölt tag eltávolítása nem sikerült.{newLine}{message}", Environment.NewLine, ex.Message);
        }
    }
    public void RemoveAll()
    {
        if (SelectedTagsCountEqZero())
            return;

        try
        {
            TagokEmails.AddRange(SelectedTags);
            SelectedTags.Clear();
        }
        catch (Exception ex)
        {
            _msgBoxService.ShowError("Kijelölt tag eltávolítása nem sikerült.{newLine}{message}", Environment.NewLine, ex.Message);
        }
    }
    private string GetAppendedMailAdresses() => string.Join(";", _selectedTags.Select(tag => tag.Email));
    public void CreateMail()
    {
        if (_selectedTags.Count == 0)
        {
            _msgBoxService.ShowError("Nincs kiválasztott tag.");
            return;
        }

        try
        {
            Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
            Microsoft.Office.Interop.Outlook.MailItem mailItem = (Microsoft.Office.Interop.Outlook.MailItem)outlookApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
            //mailItem.HTMLBody = "";
            //mailItem.Subject = "";
            mailItem.To = GetAppendedMailAdresses();
            //mailItem.Display(true);
            mailItem.Display();
        }
        catch (Exception ex)
        {
            _msgBoxService.ShowError("E-mail létrehozása nem sikerült!{newLine}{message}", Environment.NewLine, ex.Message);
        }
    }
}
