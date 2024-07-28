using Caliburn.Micro;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagsagNyilvantarto.Extensions;
using TagsagNyilvantarto.Models;

namespace TagsagNyilvantarto;

internal sealed class DataAccess : PropertyChangedBase
{
    private readonly string _connectionString;
    #region Fields
    private IEnumerable<string> _idk;
    private IEnumerable<string> _nevek;
    private IEnumerable<string> _szuletesek;
    private IEnumerable<string> _emailek;
    private IEnumerable<string> _telefonok;
    private IEnumerable<string> _tisztsegek;
    private IEnumerable<string> _tagsagkezdetek;
    private IEnumerable<string> _tagsagjogallasok;
    private IEnumerable<string> _adattipusok;
    private string[] _kepviselo;
    private string[] _admin;
    #endregion

    #region Properties
    public IEnumerable<string> Idk { get => _idk; set => _ = Set(ref _idk, value); }
    public IEnumerable<string> Nevek { get => _nevek; set => _ = Set(ref _nevek, value); }
    public IEnumerable<string> Szuletesek { get => _szuletesek; set => _ = Set(ref _szuletesek, value); }
    public IEnumerable<string> Emailek { get => _emailek; set => _ = Set(ref _emailek, value); }
    public IEnumerable<string> Telefonok { get => _telefonok; set => _ = Set(ref _telefonok, value); }
    public IEnumerable<string> Tisztsegek { get => _tisztsegek; set => _ = Set(ref _tisztsegek, value); }
    public IEnumerable<string> Tagsagkezdetek { get => _tagsagkezdetek; set => _ = Set(ref _tagsagkezdetek, value); }
    public IEnumerable<string> Tagsagjogallasok { get => _tagsagjogallasok; set => _ = Set(ref _tagsagjogallasok, value); }
    public IEnumerable<string> Adattipusok { get => _adattipusok; set => _ = Set(ref _adattipusok, value); }
    public string[] Kepviselo { get => _kepviselo; set => _ = Set(ref _kepviselo, value); }
    public string[] Admin { get => _admin; set => _ = Set(ref _admin, value); }
    #endregion
    public DataAccess()
    {
        _connectionString = $"Server=localhost; database=tagsag; UID=root; convert zero datetime=True; CharSet=utf8";
    }

    private Task<int> ExecuteCommandAsync(CommandDefinition command)
    {
        using IDbConnection connection = CreateConnection();

        try
        {
            return connection.ExecuteAsync(command);
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            throw;
        }
    }

    private MySqlConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    private int ExecuteCommand(CommandDefinition command)
    {
        using IDbConnection connection = CreateConnection();

        try
        {
            return connection.Execute(command);
        }
        catch (System.Data.OleDb.OleDbException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            throw;
        }
    }

    private T GetById<T>(int id) where T : class
    {
        if (id < 1)
            return null;

        using IDbConnection connection = CreateConnection();

        return connection.Get<T>(id);

    }

    private Task<T> GetByIdAsync<T>(int id) where T : class
    {
        if (id < 1)
            return null;

        using IDbConnection connection = CreateConnection();

        return connection.GetAsync<T>(id);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Method will pass on the reference of the DataTable.")]
    public async Task<DataTable> FillTagokDataTableAsync()
    {
        using IDbConnection connection = CreateConnection();

        var queryBuilder = new StringBuilder()
           .Append("SELECT tagok.tag_id As Id, nev As Név, DATE_FORMAT(szuletes_datuma, '%Y.%m.%d.') As Születés, email As Email, telefon As Telefon, tisztseg As Tisztség, ")
           .Append("DATE_FORMAT(tagsag_kezdete, '%Y.%m.%d.') As TagságKezdete, DATE_FORMAT(Tagdijfizetesek.Fizetve, '%Y.%m.%d.') As Fizetve, ")
           .Append("tagsag_allapotok.allapot As Jogállás, tagsagi_adattipusok.tipus As AdatokTípusa, kepviselo As Képviselő, ")
           .Append("admin As Admin FROM tagok INNER JOIN tagsag_allapotok ON tagok.tagsag_allapot=tagsag_allapotok.id INNER JOIN tagsagi_adattipusok ON tagok.adatok_tipusa = tagsagi_adattipusok.id ")
           .Append("LEFT JOIN (SELECT tag_id, MAX(fizetve) As Fizetve FROM tagdij_fizetesek GROUP BY tag_id) As Tagdijfizetesek ON tagok.tag_id=Tagdijfizetesek.tag_id;");

        using MySqlDataAdapter mySqlDataAdapter = new(queryBuilder.ToString(), (MySqlConnection)connection);

        var tagokDT = new DataTable();
        _ = await mySqlDataAdapter.FillAsync(tagokDT).ConfigureAwait(false);
        UpdateFilterLists(tagokDT);
        return tagokDT;


    }
    private void UpdateFilterLists(DataTable tagokDT)
    {
        Idk = tagokDT.GetDistinctValuesFromColumn<int>("Id").Select(i => i.ToString());
        Nevek = tagokDT.GetDistinctValuesFromColumn<string>("Név");
        Szuletesek = tagokDT.GetDistinctValuesFromColumn<string>("Születés");
        Emailek = tagokDT.GetDistinctValuesFromColumn<string>("Email");
        Telefonok = tagokDT.GetDistinctValuesFromColumn<string>("Telefon");
        Tisztsegek = tagokDT.GetDistinctValuesFromColumn<string>("Tisztség");
        Tagsagkezdetek = tagokDT.GetDistinctValuesFromColumn<string>("TagságKezdete");
        Tagsagjogallasok = tagokDT.GetDistinctValuesFromColumn<string>("Jogállás");
        Adattipusok = tagokDT.GetDistinctValuesFromColumn<string>("AdatokTípusa");
        Kepviselo = ["Képviselő", "Nem képviselő"];
        Admin = ["Admin", "Nem admin"];
    }

    public async Task<Tag> GetTagByIdAsync(int Id)
    {
        Tag tag;
        using IDbConnection connection = CreateConnection();

        string sql = "SELECT tag_id As TagId, nev As Nev, DATE_FORMAT(szuletes_datuma, '%Y.%m.%d.') As SzuletesiDatum, email As Email, telefon As Telefon, tisztseg As Tisztseg, DATE_FORMAT(tagsag_kezdete, '%Y.%m.%d.') As TagsagKezdete, kepviselo As Kepviselo, admin As Admin FROM tagok WHERE tag_id=@Id;";
        tag = await connection.QueryFirstAsync<Tag>(sql, new { Id }).ConfigureAwait(false);
        sql = "SELECT id As Id, allapot As Allapot FROM tagsag_allapotok WHERE id=(SELECT tagsag_allapot FROM tagok WHERE tag_id=@Id);";
        tag.TagsagAllapot = await connection.QueryFirstAsync<TagsagAllapot>(sql, new { Id }).ConfigureAwait(false);
        sql = "SELECT id, tipus FROM tagsagi_adattipusok WHERE id=(SELECT adatok_tipusa FROM tagok WHERE tag_id=@Id);";
        tag.AdatokTipusa = await connection.QueryFirstAsync<TagsagAdattipus>(sql, new { Id }).ConfigureAwait(false);
        return tag;
    }

    public Task<IEnumerable<TagsagAllapot>> GetAllTagsagAllapotAsync()
    {
        using IDbConnection connection = CreateConnection();
        return connection.GetAllAsync<TagsagAllapot>();
    }

    public Task<IEnumerable<TagsagAdattipus>> GetAllTagsagAdattipusAsync()
    {
        using IDbConnection connection = CreateConnection();

        return connection.GetAllAsync<TagsagAdattipus>();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Query accepts no user input.")]
    public async Task<DataTable> GetLejaroTagsagokAsync(bool lejart)
    {
        using IDbConnection connection = CreateConnection();

        int kulonbseg = lejart ? 10000 : 9973; //true : false

        var queryBuilder = new StringBuilder()
           .Append("SELECT tagok.tag_id As Id, nev As Név, DATE_FORMAT(szuletes_datuma, '%Y.%m.%d.') As Születés, email As Email, telefon As Telefon, tisztseg As Tisztség, ")
           .Append("DATE_FORMAT(tagsag_kezdete, '%Y.%m.%d.') As TagságKezdete, tagsag_allapotok.allapot As Jogállás, adatok_tipusa As AdatokTípusa, kepviselo As Képviselő, admin As Admin ")
           .Append("FROM tagok INNER JOIN tagsag_allapotok ON tagok.tagsag_allapot=tagsag_allapotok.id ")
           .Append("INNER JOIN (SELECT id, tag_id, MAX(fizetve) As utolso_fizetes FROM tagdij_fizetesek GROUP BY tag_id) As utolso_fizetesek ")
           .Append("ON tagok.tag_id = utolso_fizetesek.tag_id ")
           .Append("WHERE date(now())-utolso_fizetes >= ")
           .Append(kulonbseg); // Február miatt 9973. 31 napos hónap esetén 9970 a két dátum közti különbség. 10000 vagy nagyobb, ha lejárt a tagság

        using MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(queryBuilder.ToString(), (MySqlConnection)connection);

        var tagokDT = new DataTable();
        _ = await mySqlDataAdapter.FillAsync(tagokDT).ConfigureAwait(false);
        return tagokDT;
    }

    public Task<int> InsertTag(Tag tag)
    {
        using IDbConnection connection = CreateConnection();

        string sql = "INSERT INTO tagok (nev, szuletes_datuma, email, telefon, tisztseg, tagsag_kezdete, adatok_tipusa, kepviselo, admin, tagsag_allapot) " +
            "VALUES (@Nev, @SzuletesiDatum, @Email, @Telefon, @Tisztseg, @TagsagKezdete, @AdatokTipusa, @Kepviselo, @Admin, @TagsagAllapot);";
        object parameters = new
        {
            tag.Nev,
            tag.SzuletesiDatum,
            tag.Email,
            tag.Telefon,
            tag.Tisztseg,
            tag.TagsagKezdete,
            AdatokTipusa = tag.AdatokTipusa.Id,
            tag.Kepviselo,
            tag.Admin,
            TagsagAllapot = tag.TagsagAllapot.Id
        };
        CommandDefinition command = new(sql, parameters);
        return connection.ExecuteAsync(command);
        //if (res == 1) // Ha sikerült beilleszteni a tagot az adatbázisba
        //{
        //    sql = "SELECT tag_id FROM tagok WHERE nev='@Nev' ORDER BY tag_id DESC LIMIT 1;";
        //    dynamic a = (await connection.QueryAsync<int>(sql, new { tag.Nev }, commandType: CommandType.Text)).SingleOrDefault();//Nem működik, adatbázisabn triggerrel helyettesítve
        //}

        //MySQL tirgger
        //CREATE TRIGGER after_tag_insert AFTER INSERT
        //ON tagok
        //FOR EACH ROW
        //INSERT INTO tagdij_fizetesek(tag_id, fizetve) VALUES(NEW.tag_id, NOW());
    }

    public Task<int> UpdateTag(Tag tag)
    {
        using IDbConnection connection = CreateConnection();
        string sql = "UPDATE tagok SET nev=@Nev, szuletes_datuma=@SzuletesiDatum, email=@Email, telefon=@Telefon, tisztseg=@Tisztseg, tagsag_kezdete=@TagsagKezdete, " +
            "adatok_tipusa=@AdatokTipusa, kepviselo=@Kepviselo, admin=@Admin, tagsag_allapot=@TagsagAllapota WHERE tag_id=@Id;";
        object parameteres = new
        {
            tag.Nev,
            tag.SzuletesiDatum,
            tag.Email,
            tag.Telefon,
            tag.Tisztseg,
            tag.TagsagKezdete,
            AdatokTipusa = tag.AdatokTipusa.Id,
            tag.Kepviselo,
            tag.Admin,
            TagsagAllapota = tag.TagsagAllapot.Id,
            Id = tag.TagId
        };
        CommandDefinition command = new(sql, parameteres, commandType: CommandType.Text);
        return connection.ExecuteAsync(command);
    }

    public Task<IEnumerable<string>> GetTagdijFizetesekAsync(int tagid)
    {
        using IDbConnection connection = CreateConnection();

        string sql = "SELECT DATE_FORMAT(fizetve, '%Y.%m.%d.') FROM tagdij_fizetesek WHERE tag_id=@tagid ORDER BY fizetve;";
        return connection.QueryAsync<string>(sql, new { tagid });
    }

    public Task<int> InsertTagdijFizetesAsync(int tagId, string fizetesDatuma)
    {
        using IDbConnection connection = CreateConnection();

        string sql = "INSERT INTO tagdij_fizetesek (tag_id, fizetve) VALUES (@TagId, @Datum);";
        object parameteres = new
        {
            TagId = tagId,
            Datum = fizetesDatuma
        };
        CommandDefinition command = new(sql, parameteres, commandType: CommandType.Text);
        return connection.ExecuteAsync(command);
    }

    public Task<int> DeleteTagAsync(int tag_id)
    {
        // tag törlésével tagdij_fizetesek táblából is törlődnek a hozzátartozó sorok
        using IDbConnection connection = CreateConnection();

        string sql = "DELETE FROM tagok WHERE tag_id=@tag_id;";
        return connection.ExecuteAsync(sql, new
        {
            tag_id
        });
    }

    public Task<IEnumerable<TagokEmail>> GetAllEmailAddressesAsync()
    {
        using IDbConnection connection = CreateConnection();

        string sql = "SELECT nev As Nev, email As Email FROM tagok WHERE email <> \"\";";
        return connection.QueryAsync<TagokEmail>(sql);
    }
}
