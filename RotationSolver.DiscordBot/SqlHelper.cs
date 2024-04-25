using Npgsql;

namespace RotationSolver.DiscordBot;

internal static class SqlHelper
{
    private static NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(Config.PostgreSQL);
    }

    public static void UpdateSupporterData(ulong id, int github, string hash, string name)
    {
        SetValues($"CALL public.upsert_supporter({id}, {github}, {hash.GetValue()}, {name.GetValue()})");
    }

    private static string GetValue(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return "null";
        }
        else
        {
            return $"'{str}'";
        }
    }

    public static void UpdateRotationDevChannel(ulong id, ulong? channelId)
    {
        SetValues($"CALL public.upsert_rotation_dev({id}, {(channelId.HasValue ? channelId.Value : "null")})");
    }

    public static bool GetChannelId(ulong id, out ulong[] data)
    {
        return GetObjects($"SELECT \"RotationDev\".\"ChannelID\" FROM public.\"RotationDev\" WHERE \"DiscordID\" = {id}", out data);
    }

    public static void IsvalidSupporter(ulong id, bool valid)
    {
        SetValues($"CALL public.isvalid_supporter({id}, {valid})");
    }

    public static bool GetIDFromGithub(int github, out ulong[] data)
    {
        return GetObjects($"SELECT \"Supporter\".\"DiscordID\" FROM public.\"Supporter\" WHERE \"Github\" = {github}", out data);
    }

    public static bool GetHash(ulong id, out string[] data)
    {
        return GetObjects($"SELECT unnest(\"Supporter\".\"Hashes\") FROM public.\"Supporter\" WHERE \"DiscordID\" = {id}", out data);
    }

    public static bool GetName(ulong id, out string[] data)
    {
        return GetObjects($"SELECT \"Supporter\".\"Name\" FROM public.\"Supporter\" WHERE \"DiscordID\" = {id}", out data);
    }

    public static bool GetGithub(ulong id, out string[] data)
    {
        return GetObjects($"SELECT \"Supporter\".\"Github\" FROM public.\"Supporter\" WHERE \"DiscordID\" = {id}", out data);
    }

    public static string[] GetNames()
    {
        GetObjects<string>("SELECT * FROM public.\"GetSupportersName\"", out var result);
        return result;
    }

    public static string[] GetHashes()
    {
        GetObjects<string>("SELECT * FROM public.\"GetSupportersHash\"", out var result);
        return result;
    }

    private static void SetValues(string cmd)
    {
        using var conenction = CreateConnection();

        conenction.Open();
        using var command = new NpgsqlCommand(cmd, conenction);

        command.ExecuteNonQuery();
    }

    private static bool GetObjects<T>(string cmd, out T[] array)
    {
        using var conenction = CreateConnection();

        conenction.Open();
        using var command = new NpgsqlCommand(cmd, conenction);

        using var reader = command.ExecuteReader();

        var find = false;
        List<T> result = [];
        while (reader.Read())
        {
            find = true;
            try
            {
                result.Add((T)Convert.ChangeType(reader[0], typeof(T)));
            }
            catch
            {

            }
        }
        array = [.. result];
        return find;
    }
}
