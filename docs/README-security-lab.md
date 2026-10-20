# WebApplicationTestWeb — GitHub Security & Quality 实验项目

> ⚠️ **本仓库故意包含大量有漏洞的代码、假密钥和过期依赖，专用于测试 GitHub 的各类安全/质量扫描功能。**
> 仅供测试，切勿将其中任何代码、配置或"密钥"复制到真实项目。

本项目覆盖 GitHub **Security and quality** 面板下的全部 Findings 入口，每类都能产生可查看的数据：

| 面板条目 | 对应文件 | 说明 |
| --- | --- | --- |
| **Code quality** | `WebApplicationTestWeb/SecurityLab/CodeQualitySamples.cs` | 空引用解引用、死代码、空 catch、无限递归、竞态、浮点相等、async 无 await |
| **Dependabot · Malware / Vulnerabilities** | `Directory.Packages.props` + `WebApplicationTestWeb.csproj` + `SecurityLab/package.json` | 5 个含已知 GHSA 漏洞的 NuGet 包 + 5 个含已知漏洞的 npm 包 + 1 个被 GitHub 标记为 **malware** 的 npm 包（`@an1mshnpmb9c84920/gat-scope-target-125ec19a`，仅声明、不安装，`SecurityLab/.npmrc` 已禁用安装脚本） |
| **Secret scanning** | `SecurityLab/fake-secrets.txt` | AWS / GitHub PAT / Slack / Stripe / Google API 假密钥 |
| **Code scanning** | `.github/workflows/codeql.yml` + `WebApplicationTestWeb/SecurityLab/VulnerableEndpoints.cs` | CodeQL 周期扫描 SQL 注入、路径遍历、弱加密、SSRF、XXE、反序列化、硬编码凭据等 |
| **Security policy** | `.github/SECURITY.md` | 漏洞报告政策（面板里 Security policy 会从 Disabled 变 Enabled） |

## 使用步骤

1. 把仓库推送到 GitHub（私有仓库需开启 GitHub Advanced Security 试用，你截图里的试用已满足条件）。
2. 在仓库 **Settings → Advanced Security** 里逐一启用：
   - Dependabot alerts（Vulnerabilities / Malware 数据源）
   - Code scanning（Default setup 或走本仓库的 codeql.yml 工作流）
   - Secret scanning + Push protection
   - Code quality（Qodana）
3. 推送后等待几轮 Action 运行，然后到 **Security and quality → Findings** 各个面板查看数据。

## ⚠️ Secret scanning + Push protection 特别说明

`SecurityLab/fake-secrets.txt` 里的假密钥会被 GitHub 识别为真实凭据格式。
**如果启用了 Push protection，包含该文件的提交会被直接拒绝推送** —— 这正是功能生效的证明。

如果想让文件成功推上去（从而在 Secret scanning 面板里出现 alert），两种做法：

- 方案 A（推荐体验完整流程）：先关掉 push protection 推送，之后在 alert 列表里点 "Revoke" / "Close as revoked"。
- 方案 B：推送时在报错链接里点 "It's used in tests" 之类 allow 选项，允许该 secret 入库。

## 常见漏洞清单（供 CodeQL 验证）

| CWE | 位置 | 说明 |
| --- | --- | --- |
| CWE-89 | `VulnerableEndpoints.BuildLoginSql` | SQL 注入（字符串拼接 SQL） |
| CWE-22 | `VulnerableEndpoints.ReadUserProfile` | 路径遍历 |
| CWE-327 | `VulnerableEndpoints.HashPasswordWithMd5` | 弱哈希（MD5 存密码） |
| CWE-798 | `VulnerableEndpoints.DbConnectionString` / `ThirdPartyApiKey` | 硬编码凭据 |
| CWE-330 | `VulnerableEndpoints.GenerateSessionToken` | Random 生成安全令牌 |
| CWE-918 | `VulnerableEndpoints.IsInternalUrl` | SSRF 内网地址判断 |
| CWE-611 | `VulnerableEndpoints.ParseUntrustedXml` | XXE（XmlUrlResolver + DTD） |
| CWE-502 | `VulnerableEndpoints.DeserializeUntrusted` | 不安全反序列化（TypeNameHandling.All） |
| CWE-404 | `VulnerableEndpoints.ReadConfigLazy` | StreamReader 未 Dispose |
| CWE-209 | `VulnerableEndpoints.DumpException` | 异常堆栈外泄 |
