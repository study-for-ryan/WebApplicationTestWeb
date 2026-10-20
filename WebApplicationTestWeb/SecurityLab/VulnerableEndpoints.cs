using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplicationTestWeb.SecurityLab;

/// <summary>
/// ⚠️ 安全实验室: 本文件故意包含多种常见 Web 漏洞, 用于验证 GitHub Code Scanning / CodeQL 的检出能力。
/// 全部端点前缀: /SecurityLab/...
/// 生产代码严禁模仿。所有端点不操作真实数据, 仅返回演示内容。
/// </summary>
public class VulnerableEndpoints
{
    // [SEV-CRITICAL] CWE-89 SQL 注入: 用户输入直接拼接到 SQL 语句
    public static string BuildLoginSql(string username, string password)
    {
        return "SELECT * FROM Users WHERE Name = '" + username + "' AND Pwd = '" + password + "'";
    }

    // [SEV-CRITICAL] CWE-22 路径遍历: 用户输入直接拼接到文件路径
    public static string ReadUserProfile(string fileName)
    {
        var path = "/var/data/profiles/" + fileName;
        return File.Exists(path) ? File.ReadAllText(path) : "not found: " + path;
    }

    // [SEV-HIGH] CWE-327 弱哈希: 使用 MD5 处理口令
    public static string HashPasswordWithMd5(string password)
    {
        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    // [SEV-HIGH] CWE-798 硬编码凭据: 连接串与 API Key 写死在源码里
    public const string DbConnectionString =
        "Server=prod-db.internal;Database=AppDb;User Id=sa;Password=P@ssw0rd2026!;";

    public const string ThirdPartyApiKey = "sk-live-9f8e7d6c5b4a3210fedcba9876543210";

    // [SEV-MEDIUM] CWE-330 弱随机: 使用 Random 生成安全令牌
    public static string GenerateSessionToken()
    {
        var random = new Random();
        var token = new byte[16];
        random.NextBytes(token);
        return Convert.ToHexString(token);
    }

    // [SEV-CRITICAL] CWE-918 SSRF: 用户可控 URL 直接发起服务端请求
    public static bool IsInternalUrl(string url)
    {
        return url.StartsWith("http://169.254.169.254") || url.Contains("localhost") || url.Contains("internal");
    }

    // [SEV-MEDIUM] CWE-611 XXE: 允许 DTD 的 XML 解析
    public static string ParseUntrustedXml(string xml)
    {
        var doc = new System.Xml.XmlDocument { XmlResolver = new System.Xml.XmlUrlResolver() };
        doc.LoadXml(xml);
        return doc.DocumentElement?.InnerText ?? string.Empty;
    }

    // [SEV-HIGH] CWE-502 不安全反序列化: 对不可信数据启用 TypeNameHandling
    public static object DeserializeUntrusted(string json)
    {
        var settings = new Newtonsoft.Json.JsonSerializerSettings
        {
            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
        };
        return Newtonsoft.Json.JsonConvert.DeserializeObject(json, settings)!;
    }

    // [SEV-LOW] CWE-404 资源未释放: StreamReader 未 using/Dispose
    public static string ReadConfigLazy(string path)
    {
        var reader = new StreamReader(path);
        return reader.ReadToEnd();
    }

    // [SEV-MEDIUM] CWE-209 错误信息泄露: 原样返回异常堆栈
    public static string DumpException(Exception ex)
    {
        return ex.ToString();
    }
}
