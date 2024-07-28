using Caliburn.Micro;
using System.Data;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using TagsagNyilvantarto.Models;

namespace TagsagNyilvantarto.ViewModels
{
    internal class TagokViewModel : Screen
    {
        private FilterValues _selectedFilterValues;
        private DataAccess _dataAccess;
        private IWindowManager _windowManager;
        private IMsgBoxService _msgBoxService;
        private DataView _tagok;
        private DataRowView _selectedRow;
        public FilterValues SelectedFilterValues { get => _selectedFilterValues; set => _selectedFilterValues = value; }
        public DataView Tagok { get => _tagok; set => _ = Set(ref _tagok, value); }
        public DataAccess DataAccess { get => _dataAccess; private set => _dataAccess = value; }
        public DataRowView SelectedRow { get => _selectedRow; set => _ = Set(ref _selectedRow, value); }

        public TagokViewModel(IWindowManager windowManager, IMsgBoxService msgBoxService, DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _windowManager = windowManager;
            _msgBoxService = msgBoxService;
            _selectedFilterValues = new FilterValues();//Szűrési értékek inicializálása üresen
        }

        public void Filter(DataView dataView)
        {
            if (dataView == null)
                return;

            StringBuilder filterExpression = new StringBuilder();
            string logicOperation = " AND ";
            if (!string.IsNullOrEmpty(_selectedFilterValues.Id))
            {
                filterExpression.Append($"Id = {_selectedFilterValues.Id}");
            }

            if (!string.IsNullOrEmpty(_selectedFilterValues.Nev))
            {
                if (filterExpression.Length != 0)
                    filterExpression.Append(logicOperation);
                filterExpression.Append($"Név = '{_selectedFilterValues.Nev}'");
            }

            if (!string.IsNullOrEmpty(_selectedFilterValues.Szuletes))
            {
                if (filterExpression.Length != 0)
                    filterExpression.Append(logicOperation);
                filterExpression.Append($"Születés = '{_selectedFilterValues.Szuletes}'");
            }

            if (!string.IsNullOrEmpty(_selectedFilterValues.Email))
            {
                if (filterExpression.Length != 0)
                    filterExpression.Append(logicOperation);
                filterExpression.Append($"Email = '{_selectedFilterValues.Email}'");
            }

            if (!string.IsNullOrEmpty(_selectedFilterValues.Telefon))
            {
                if (filterExpression.Length != 0)
                    filterExpression.Append(logicOperation);
                filterExpression.Append($"Telefon = '{_selectedFilterValues.Telefon}'");
            }

            if (!string.IsNullOrEmpty(_selectedFilterValues.Tisztseg))
            {
                if (filterExpression.Length != 0)
                    filterExpression.Append(logicOperation);
                filterExpression.Append($"Tisztség = '{_selectedFilterValues.Tisztseg}'");
            }

            if (!string.IsNullOrEmpty(_selectedFilterValues.TagsagKezdete))
            {
                if (filterExpression.Length != 0)
                    filterExpression.Append(logicOperation);
                filterExpression.Append($"TagságKezdete= '{_selectedFilterValues.TagsagKezdete}'");
            }

            if (!string.IsNullOrEmpty(_selectedFilterValues.Jogallas))
            {
                if (filterExpression.Length != 0)
                    filterExpression.Append(logicOperation);
                filterExpression.Append($"Jogállás = '{_selectedFilterValues.Jogallas}'");
            }

            if (!string.IsNullOrEmpty(_selectedFilterValues.AdatokTipusa))
            {
                if (filterExpression.Length != 0)
                    filterExpression.Append(logicOperation);
                filterExpression.Append($"AdatokTípusa = '{_selectedFilterValues.AdatokTipusa}'");
            }

            if (!string.IsNullOrEmpty(_selectedFilterValues.Kepviselo))
            {
                if (filterExpression.Length != 0)
                    filterExpression.Append(logicOperation);
                if (_selectedFilterValues.Kepviselo == "Képviselő")
                    filterExpression.Append($"Képviselő = True");
                else if (_selectedFilterValues.Kepviselo == "Nem képviselő")
                    filterExpression.Append($"Képviselő = False");
            }

            if (!string.IsNullOrEmpty(_selectedFilterValues.AdatokTipusa))
            {
                if (filterExpression.Length != 0)
                    filterExpression.Append(logicOperation);
                filterExpression.Append($"AdatokTípusa = '{_selectedFilterValues.AdatokTipusa}'");
            }

            if (!string.IsNullOrEmpty(_selectedFilterValues.Admin))
            {
                if (filterExpression.Length != 0)
                    filterExpression.Append(logicOperation);
                if (_selectedFilterValues.Admin == "Admin")
                    filterExpression.Append($"Admin = True");
                else if (_selectedFilterValues.Admin == "Nem admin")
                    filterExpression.Append($"Képviselő = False");
            }

            dataView.RowFilter = filterExpression.ToString();
        }

        public async Task UpdateFromSource()
        {
            _tagok?.Dispose();
            DataTable tagokDT = await _dataAccess.FillTagokDTAsync();
            Tagok = new DataView(tagokDT);

        }

        public async Task TagsagLejar()
        {
            _tagok?.Dispose();
            DataTable tagokDT = await _dataAccess.LejaroTagsagok(false);//false=> hónapőban lejáró tagságok
            Tagok = new DataView(tagokDT);
        }

        public async Task Open()
        {
            if (_selectedRow == null)
                return;

            int selectedId = _selectedRow.Row.Field<int>("Id");
            Tag tag = await _dataAccess.GetTag(selectedId);

            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
            settings.Width = 600;
            settings.Title = tag.Nev;
            _windowManager.ShowWindow(new TagViewModel(_dataAccess, IoC.Get<IMsgBoxService>(), tag), null, settings);
        }

        public void New()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
            settings.Width = 600;
            //settings.MinWidth = 450;
            settings.Title = "Új igény";
            _windowManager.ShowWindow(new TagViewModel(_dataAccess, IoC.Get<IMsgBoxService>(), new Tag()), null, settings);
        }

        public async Task Delete()
        {
            if (_selectedRow == null)
                return;

            int selectedId = _selectedRow.Row.Field<int>("Id");
            bool confirm = _msgBoxService.AskForConfirmation("Biztosan törli a tagot?");
            int res = 0;
            if (confirm)
                res = await _dataAccess.DeleteTag(selectedId);

            if (res == 1)
                _msgBoxService.ShowNotification("Tag sikeresen törölve.");
        }

        public void SendMail()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
            //settings.Width = 600;
            //settings.MinWidth = 450;
            settings.Title = "Köremail küldés";
            _windowManager.ShowWindow(new TagokEmailViewModel(_dataAccess, IoC.Get<MsgBoxService>()), null, settings);
        }
    }
}
