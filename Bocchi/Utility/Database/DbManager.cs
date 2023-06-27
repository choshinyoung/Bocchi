using System.Linq.Expressions;
using MongoDB.Driver;

namespace Bocchi.Utility.Database;

public static class DbManager
{
    private const string DbName = "bocchiDb";

    private static readonly MongoClient Client =
        new(
            $"mongodb://{Config.Get("MONGODB_USERNAME")}:{Config.Get("MONGODB_PASSWORD")}@localhost?authSource={DbName}"
        );

    private static readonly MongoDatabaseBase Db = (MongoDatabaseBase)Client.GetDatabase(DbName);

    private static readonly IMongoCollection<User> Users = Db.GetCollection<User>("Users");

    public static User GetUser(ulong id)
    {
        var searchResult = Users.Find(u => u.UserId == id);

        if (searchResult.Any())
        {
            return searchResult.Single();
        }

        User user = new(id);
        Users.InsertOne(user);

        return user;
    }

    public static void SetUser(ulong id, Expression<Func<User, object>> field, object value)
    {
        Users.UpdateOne(u => u.UserId == id, Builders<User>.Update.Set(field, value));
    }
}