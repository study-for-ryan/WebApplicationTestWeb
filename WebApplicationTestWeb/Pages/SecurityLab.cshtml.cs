using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplicationTestWeb.Pages;

/// <summary>
/// ⚠️ 安全实验室端点页: 把 VulnerableEndpoints 中的漏洞方法暴露为 HTTP 可访问, 便于手动验证。
/// 仅供测试 GitHub Code Scanning, 生产代码严禁模仿。
/// </summary>
[BindProperties]
public class LabModel : PageModel
{
    public string Result { get; set; } = string.Empty;

    public void OnGetSql(string u, string p)
        => Result = SecurityLab.VulnerableEndpoints.BuildLoginSql(u ?? "admin", p ?? "123456");

    public void OnGetHash(string pwd)
        => Result = SecurityLab.VulnerableEndpoints.HashPasswordWithMd5(pwd ?? "secret");

    public void OnGetToken()
        => Result = SecurityLab.VulnerableEndpoints.GenerateSessionToken();

    public void OnGetXxe(string xml)
        => Result = SecurityLab.VulnerableEndpoints.ParseUntrustedXml(xml ?? "<?xml version='1.0'?><root>hi</root>");

    public void OnGetDeserialize(string json)
        => Result = SecurityLab.VulnerableEndpoints.DeserializeUntrusted(json ?? "{}").ToString() ?? string.Empty;
}
