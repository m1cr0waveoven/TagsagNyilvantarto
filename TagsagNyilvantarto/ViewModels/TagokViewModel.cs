using Caliburn.Micro;
using System.Data;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using TagsagNyilvantarto.Models;
using System.Diagnostics.CodeAnalysis;

namespace TagsagNyilvantarto.ViewModels;

internal sealed class TagokViewModel : Screen
{
    private readonly DataAccess _dataAccess;
    private readonly IWindowManager _windowManager;
    private readonly IMsgBoxService _msgBoxService;
    private DataView _tagok;
    private DataRowView _selectedRow;
    public FilterValues SelectedFilterValues { get; set; }
    public DataView Tagok { get => _tagok; set => _ = Set(ref _tagok, value); }
    public DataAccess DataAccess { get => _dataAccess; }
    public DataRowView SelectedRow { get => _selectedRow; set => _ = Set(ref _selectedRow, value); }

    public TagokViewModel(IWindowManager windowManager, IMsgBoxService msgBoxService, DataAccess dataAccess)
    {
        _dataAccess = dataAccess;
        _windowManager = windowManager;
        _msgBoxService = msgBoxService;
        SelectedFilterValues = new FilterValues(); // Szűrési értékek inicializálása üresen
    }

    public void Filter(DataView dataView)
    {
        if (dataView is null)
            return;

        StringBuilder filterExpression = new StringBuilder();
        string logicOperation = " AND ";
        if (!string.IsNullOrEmpty(SelectedFilterValues.Id))
        {
            filterExpression.Append($"Id = {SelectedFilterValues.Id}");
        }

        if (!string.IsNullOrEmpty(SelectedFilterValues.Nev))
        {
            if (filterExpression.Length != 0)
                filterExpression.Append(logicOperation);
            filterExpression.Append($"Név = '{SelectedFilterValues.Nev}'");
        }

        if (!string.IsNullOrEmpty(SelectedFilterValues.Szuletes))
        {
            if (filterExpression.Length != 0)
                filterExpression.Append(logicOperation);
            filterExpression.Append($"Születés = '{SelectedFilterValues.Szuletes}'");
        }

        if (!string.IsNullOrEmpty(SelectedFilterValues.Email))
        {
            if (filterExpression.Length != 0)
                filterExpression.Append(logicOperation);
            filterExpression.Append($"Email = '{SelectedFilterValues.Email}'");
        }

        if (!string.IsNullOrEmpty(SelectedFilterValues.Telefon))
        {
            if (filterExpression.Length != 0)
                filterExpression.Append(logicOperation);
            filterExpression.Append($"Telefon = '{SelectedFilterValues.Telefon}'");
        }

        if (!string.IsNullOrEmpty(SelectedFilterValues.Tisztseg))
        {
            if (filterExpression.Length != 0)
                filterExpression.Append(logicOperation);
            filterExpression.Append($"Tisztség = '{SelectedFilterValues.Tisztseg}'");
        }

        if (!string.IsNullOrEmpty(SelectedFilterValues.TagsagKezdete))
        {
            if (filterExpression.Length != 0)
                filterExpression.Append(logicOperation);
            filterExpression.Append($"TagságKezdete= '{SelectedFilterValues.TagsagKezdete}'");
        }

        if (!string.IsNullOrEmpty(SelectedFilterValues.Jogallas))
        {
            if (filterExpression.Length != 0)
                filterExpression.Append(logicOperation);
            filterExpression.Append($"Jogállás = '{SelectedFilterValues.Jogallas}'");
        }

        if (!string.IsNullOrEmpty(SelectedFilterValues.AdatokTipusa))
        {
            if (filterExpression.Length != 0)
                filterExpression.Append(logicOperation);
            filterExpression.Append($"AdatokTípusa = '{SelectedFilterValues.AdatokTipusa}'");
        }

        if (!string.IsNullOrEmpty(SelectedFilterValues.Kepviselo))
        {
            if (filterExpression.Length != 0)
                filterExpression.Append(logicOperation);
            if (SelectedFilterValues.Kepviselo == "Képviselő")
                filterExpression.Append($"Képviselő = True");
            else if (SelectedFilterValues.Kepviselo == "Nem képviselő")
                filterExpression.Append($"Képviselő = False");
        }

        if (!string.IsNullOrEmpty(SelectedFilterValues.AdatokTipusa))
        {
            if (filterExpression.Length != 0)
                filterExpression.Append(logicOperation);
            filterExpression.Append($"AdatokTípusa = '{SelectedFilterValues.AdatokTipusa}'");
        }

        if (!string.IsNullOrEmpty(SelectedFilterValues.Admin))
        {
            if (filterExpression.Length != 0)
                filterExpression.Append(logicOperation);
            if (SelectedFilterValues.Admin == "Admin")
                filterExpression.Append($"Admin = True");
            else if (SelectedFilterValues.Admin == "Nem admin")
                filterExpression.Append($"Képviselő = False");
        }

        dataView.RowFilter = filterExpression.ToString();
    }

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Will cause the DataView to lose its content. DataView does not create copy of the data.")]
    public async Task UpdateFromSource()
    {
        _tagok?.Dispose();
        DataTable tagokDT = await _dataAccess.FillTagokDataTableAsync().ConfigureAwait(true);
        Tagok = new DataView(tagokDT);

    }

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Will cause the DataView to lose its content. DataView does not create copy of the data.")]
    public async Task TagsagLejar()
    {
        _tagok?.Dispose();
        DataTable tagokDT = await _dataAccess.GetLejaroTagsagokAsync(false).ConfigureAwait(true); // false => hónapban lejáró tagságok
        Tagok = new DataView(tagokDT);
    }

    public async Task Open()
    {
        if (_selectedRow is null)
            return;

        int selectedId = _selectedRow.Row.Field<int>("Id");
        Tag tag = await _dataAccess.GetTagByIdAsync(selectedId).ConfigureAwait(true);

        dynamic settings = new ExpandoObject();
        settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
        settings.Width = 600;
        settings.Title = tag.Nev;
        await _windowManager.ShowWindowAsync(new TagViewModel(_dataAccess, IoC.Get<IMsgBoxService>(), tag), null, settings);
    }

    public async Task New()
    {
        dynamic settings = new ExpandoObject();
        settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
        settings.Width = 600;
        //settings.MinWidth = 450;
        settings.Title = "Új Tag";
        await _windowManager.ShowWindowAsync(new TagViewModel(_dataAccess, IoC.Get<IMsgBoxService>(), new Tag()), null, settings);
    }

    public async Task Delete()
    {
        if (_selectedRow is null)
            return;

        int selectedId = _selectedRow.Row.Field<int>("Id");
        bool confirm = _msgBoxService.AskForConfirmation("Biztosan törli a tagot?");
        int res = 0;
        if (confirm)
            res = await _dataAccess.DeleteTagAsync(selectedId).ConfigureAwait(false);

        if (res == 1)
            _msgBoxService.ShowNotification("Tag sikeresen törölve.");
    }

    public async Task SendMail()
    {
        dynamic settings = new ExpandoObject();
        settings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
        //settings.Width = 600;
        //settings.MinWidth = 450;
        settings.Title = "Köremail küldés";
        await _windowManager.ShowWindowAsync(new TagokEmailViewModel(_dataAccess, IoC.Get<MsgBoxService>()), null, settings);
    }
}
