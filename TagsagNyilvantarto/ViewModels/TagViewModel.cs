using AsyncAwaitBestPractices;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TagsagNyilvantarto.Models;

namespace TagsagNyilvantarto.ViewModels
{
    class TagViewModel : Screen
    {
        DataAccess _dataAccess;
        IMsgBoxService _msgBoxService;
        Tag _tag;
        IList<TagsagAllapot> _tagsagAllapotok;
        IList<TagsagAdattipus> _tagsagAdattipusok;
        IList<string> _tagdijFizetesDatumok;
        DateTime _fizetesDatuma = DateTime.Now;
        public TagViewModel(DataAccess dataAccess, IMsgBoxService msgBoxService, Tag tag)
        {
            _dataAccess = dataAccess;
            _msgBoxService = msgBoxService;
            Tag = tag;
            RefreshLists(dataAccess).SafeFireAndForget();
        }

        public Tag Tag { get => _tag; set => _tag = value; }
        public IList<TagsagAllapot> TagsagAllapotok { get => _tagsagAllapotok; set => _ = Set(ref _tagsagAllapotok, value); }
        public IList<TagsagAdattipus> TagsagAdattipusok { get => _tagsagAdattipusok; set => _ = Set(ref _tagsagAdattipusok, value); }
        public IList<string> TagdijFizetesDatumok { get => _tagdijFizetesDatumok; set => _ = Set(ref _tagdijFizetesDatumok, value); }
        public DateTime FizetesDatuma { get => _fizetesDatuma; set => _ = Set(ref _fizetesDatuma, value); }

        public async Task RefreshLists(DataAccess dataAccess)
        {
            TagsagAllapotok = (await dataAccess.GetAllTagsagAllapot().ConfigureAwait(false)).ToList();
            TagsagAdattipusok = (await dataAccess.GetAllTagsagAdattipus().ConfigureAwait(false)).ToList();
            TagdijFizetesDatumok = await dataAccess.TagdijFizetesek(Tag.Tag_id).ConfigureAwait(false);
        }
        public async Task Save()
        {

            //TODO: Check
            if (_tag.Tag_id == default)
            {
                _tag.Tag_id = await _dataAccess.InsertTag(_tag);
                if (_tag.Tag_id > 0)
                {
                    _msgBoxService.ShowNotification("Új tag sikeresen felvéve.");

                    //Tag beillesztése után nem sikerült a tag id-jét lekérdezni
                    //string date = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                    //int res = await _dataAccess.TagdijFizetve(_tag.Tag_id, date);
                    //if(res==1)
                    //    _msgBoxService.ShowNotification("Új taghoz tagdíj fizetés rögzítve.");
                    //Tag = new Tag();
                    this.TryClose();
                }
                else
                {
                    _msgBoxService.ShowError("Új tag felvétele nem sikerült");
                }
            }
            else
            {
                int res = await _dataAccess.UpdateTag(_tag);
                if (res == 1)
                {
                    _msgBoxService.ShowNotification("Tag módosítása sikerült.");
                    this.TryClose();
                }
                else
                {
                    _msgBoxService.ShowError("Tag módosítása nem sikerült.");
                }
            }
        }
        private bool Check(Tag tag)
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
            if (tag.TagsagAllapot == null)
            {
                _msgBoxService.ShowError("Tagság állapot megadása kötelező!");
                return false;
            }

            if (tag.AdatokTipusa == null)
            {
                _msgBoxService.ShowError("Tagsági adat típusának megadása kötelező megadása kötelező!");
                return false;
            }

            return true;


        }
        public async Task TagdijFizeteve()
        {
            if (Tag.Tag_id == default)
            {
                _msgBoxService.ShowError("Tagdíj fizetés rögzítése előtt a tagot regisztrálni kell.\nKattintsa a mentés gombra, majd próbáld újra.");
                return;
            }
            if (_fizetesDatuma == default)
            {
                _msgBoxService.ShowError("Nincs dátum kiválasztva!");
                return;
            }
            string datum = FizetesDatuma.Year + "-" + FizetesDatuma.Month + "-" + FizetesDatuma.Day;
            int res = await _dataAccess.TagdijFizetve(Tag.Tag_id, datum);

            if (res == 1)
                _msgBoxService.ShowNotification("Tagdíjfeieztés sikeresen rögzítve.");
            else
                _msgBoxService.ShowError("Tagdíjfizetés rögzítése nem sikerült.");

            TagdijFizetesDatumok = await _dataAccess.TagdijFizetesek(Tag.Tag_id);
        }

        public async Task Delete()
        {
            bool confirm = _msgBoxService.AskForConfirmation("Biztosan törli a tagot?");
            int res = 0;
            if (confirm)
                res = await _dataAccess.DeleteTag(Tag.Tag_id);

            if (res == 1)
                _msgBoxService.ShowNotification("Tag sikeresen törölve.");

            this.TryClose();
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
