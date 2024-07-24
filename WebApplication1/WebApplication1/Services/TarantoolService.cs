using Microsoft.Extensions.Options;

using ProGaudi.MsgPack.Light;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using ProGaudi.Tarantool.Client.Model.Enums;

using WebApplication1.Model;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class TarantoolService : ITarantoolService
{
    private readonly string connStr = "tarantool:tarantool@tarantool:3301";
    private readonly string spaceName = "users";
    private readonly string spacePrimaryIndex = "primary";
    private readonly string spaceEmailIndex = "email_index";
    private readonly ILogger<TarantoolService> _logger;
    private readonly Box _box;
    private readonly ISpace space;
    private readonly IIndex primaryIndex;
    private readonly IIndex secondaryIndex;

    public TarantoolService(ILogger<TarantoolService> logger)
    {
        _logger = logger;
        var msgPackContext = new MsgPackContext();
        msgPackContext.GenerateAndRegisterArrayConverter<UserTar>();

        var clientOptions = new ClientOptions(connStr, context: msgPackContext);
        _box = new Box(clientOptions);
        _box.Connect().ConfigureAwait(false).GetAwaiter().GetResult();
        space = _box.Schema[spaceName];
        primaryIndex = space[spacePrimaryIndex];
        secondaryIndex = space[spaceEmailIndex];

    }

    public async Task<int> Init()
    {
        var res = await _box.Call<int>("reload_single");

        return res.Data[0];
    }

    public async Task<UserTar?> GetById(long id)
    {
        var res = await primaryIndex.Select<TarantoolTuple<long>, UserTar>(TarantoolTuple.Create(id), new SelectOptions { Iterator = Iterator.Eq }); ;

        return res.Data[0];
    }

    public async Task<UserTar?> GetByEmail(string email)
    {
        var res = await secondaryIndex.Select<TarantoolTuple<string>, UserTar>(TarantoolTuple.Create(email), new SelectOptions { Iterator = Iterator.Eq }); ;

        return res.Data[0];
    }

    public async Task Insert(ICollection<UserTar> items)
    {
        foreach (var item in items)
        {
           await space.Insert(item);
        }

    }
}
