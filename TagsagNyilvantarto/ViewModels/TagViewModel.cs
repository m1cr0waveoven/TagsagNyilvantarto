using AsyncAwaitBestPractices;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TagsagNyilvantarto.Models;

namespace TagsagNyilvantarto.ViewModels;

internal sealed class TagViewModel : Screen
{
    private readonly DataAccess _dataAccess;
    private readonly IMsgBoxService _msgBoxService;
    private Tag _tag;
    private IList<TagsagAllapot> _tagsagAllapotok;
    private IList<TagsagAdattipus> _tagsagAdattipusok;
    private IList<string> _tagdijFizetesDatumok;
    private DateTime _fizetesDatuma = DateTime.Now;
    public TagViewModel(DataAccess dataAccess, IMsgBoxService msgBoxService, Tag tag)
    {
        _dataAccess = dataAccess;
        _msgBoxService = msgBoxService;
        Tag = tag;
        RefreshLists(dataAccess).SafeFireAndForget();
    }

    public Tag Tag { get => _tag; set => _ = Set(ref _tag, value); }
    public IList<TagsagAllapot> TagsagAllapotok { get => _tagsagAllapotok; set => _ = Set(ref _tagsagAllapotok, value); }
    public IList<TagsagAdattipus> TagsagAdattipusok { get => _tagsagAdattipusok; set => _ = Set(ref _tagsagAdattipusok, value); }
    public IList<string> TagdijFizetesDatumok { get => _tagdijFizetesDatumok; set => _ = Set(ref _tagdijFizetesDatumok, value); }
    public DateTime FizetesDatuma { get => _fizetesDatuma; set => _ = Set(ref _fizetesDatuma, value); }

    public async Task RefreshLists(DataAccess dataAccess)
    {
        TagsagAllapotok = (await dataAccess.GetAllTagsagAllapotAsync().ConfigureAwait(true)).ToList();
        TagsagAdattipusok = (await dataAccess.GetAllTagsagAdattipusAsync().ConfigureAwait(true)).ToList();
        TagdijFizetesDatumok = (await dataAccess.GetTagdijFizetesekAsync(Tag.TagId).ConfigureAwait(true)).ToList();
    }
    public async Task Save()
    {

        if (!CheckTag(Tag))
            return;

        if (_tag.TagId == default)
        {
            _tag.TagId = await _dataAccess.InsertTag(_tag).ConfigureAwait(true);
            if (_tag.TagId > 0)
            {
                _msgBoxService.ShowNotification("Új tag sikeresen felvéve.");

                //Tag beillesztése után nem sikerült a tag id-jét lekérdezni
                //string date = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                //int res = await _dataAccess.TagdijFizetve(_tag.Tag_id, date);
                //if (res == 1)
                //    _msgBoxService.ShowNotification("Új taghoz tagdíj fizetés rögzítve.");
                //Tag = new Tag();
                await TryCloseAsync().ConfigureAwait(true);
                return;
            }

            _msgBoxService.ShowError("Új tag felvétele nem sikerült");
            return;
        }

        int res = await _dataAccess.UpdateTag(_tag).ConfigureAwait(true);
        if (res == 1)
        {
            _msgBoxService.ShowNotification("Tag módosítása sikerült.");
            await TryCloseAsync().ConfigureAwait(true);
            return;
        }

        _msgBoxService.ShowError("Tag módosítása nem sikerült.");

    }
    private bool CheckTag(Tag tag)
    {
        if (String.IsNullOrEmpty(tag.Nev))
        {
            _msgBoxService.ShowError("Név megadása kötelező!");
            return false;
        }

        if (tag.SzuletesiDatum == default)
        {
            _msgBoxService.ShowError("Születési dátum nincs megadva.");
            return false;
        }

        if (String.IsNullOrEmpty(tag.Email))
        {
            _msgBoxService.ShowError("Email cím nincs megadva.");
            return false;
        }

        if (String.IsNullOrEmpty(tag.Telefon))
        {
            _msgBoxService.ShowError("Telefonszám nincs megadva.");
            return false;
        }

        if (String.IsNullOrEmpty(tag.Tisztseg))
        {
            Tag.Tisztseg = "-";
        }

        if (tag is { TagsagAllapot: null })
        {
            _msgBoxService.ShowError("Tagság állapot megadása kötelező!");
            return false;
        }

        if (tag is { AdatokTipusa: null })
        {
            _msgBoxService.ShowError("Tagsági adat típusának megadása kötelező megadása kötelező!");
            return false;
        }

        return true;


    }
    public async Task TagdijFizeteve()
    {
        if (Tag is { TagId: 0 })
        {
            _msgBoxService.ShowError("Tagdíj fizetés rögzítése előtt a tagot regisztrálni kell.\nKattintsa a mentés gombra, majd próbáld újra.");
            return;
        }

        if (_fizetesDatuma == default)
        {
            _msgBoxService.ShowError("Nincs dátum kiválasztva!");
            return;
        }

        string datum = $"{FizetesDatuma.Year}-{FizetesDatuma.Month}-{FizetesDatuma.Day}";
        int res = await _dataAccess.InsertTagdijFizetesAsync(Tag.TagId, datum).ConfigureAwait(false);

        if (res == 1)
            _msgBoxService.ShowNotification("Tagdíjfeieztés sikeresen rögzítve.");
        else
            _msgBoxService.ShowError("Tagdíjfizetés rögzítése nem sikerült.");

        TagdijFizetesDatumok = (await _dataAccess.GetTagdijFizetesekAsync(Tag.TagId).ConfigureAwait(true)).ToList();
    }

    public async Task Delete()
    {
        bool confirm = _msgBoxService.AskForConfirmation("Biztosan törli a tagot?");
        int res = 0;
        if (confirm)
            res = await _dataAccess.DeleteTagAsync(Tag.TagId).ConfigureAwait(false);

        if (res == 1)
            _msgBoxService.ShowNotification("Tag sikeresen törölve.");

        await TryCloseAsync().ConfigureAwait(true);
    }
    public void SendMail()
    {
        try
        {
            Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
            Microsoft.Office.Interop.Outlook.MailItem mailItem = (Microsoft.Office.Interop.Outlook.MailItem)outlookApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
            //mailItem.HTMLBody = "";
            //mailItem.Subject = "";
            mailItem.To = Tag.Email;
            //mailItem.Display(true);
            mailItem.Display();
        }
        catch (COMException comEx)
        {
            IoC.Get<IMsgBoxService>().ShowError("Hiba az emal létrehozása során! {newLine}{Error COM exception: {exMessage]}", Environment.NewLine, comEx.Message);
        }
        catch (Exception ex)
        {
            IoC.Get<IMsgBoxService>().ShowError("Hiba az emal létrehozása során! {newLine}{Error: {exMessage]}", Environment.NewLine, ex.Message);
        }
    }
}
