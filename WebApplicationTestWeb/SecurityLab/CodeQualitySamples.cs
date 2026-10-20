// ⚠️ 安全实验室: 本文件专门用来触发"代码质量"类 Findings (Qodana / CodeQL quality 规则)。
// 包含: 空引用解引用、死代码、空 catch、无限递归、竞态、浮点相等比较、async 无 await 等。
// 这些代码可以编译通过, 但质量扫描会给出提示。生产代码严禁模仿。

namespace WebApplicationTestWeb.SecurityLab;

public static class CodeQualitySamples
{
    private static int _callCount;

    // 质量: 嵌套过深 + 魔法数字
    public static string LongMessyMethod(int score, string grade, bool isVip, bool isEmployee, int years)
    {
        var result = string.Empty;
        if (score > 90)
        {
            if (grade == "A")
            {
                if (isVip)
                {
                    result = "Platinum" + (years * 365 * 24 * 60 * 60).ToString();
                }
                else if (isEmployee)
                {
                    result = "Gold" + 3600;
                }
                else
                {
                    result = "Silver" + 42;
                }
            }
            else if (grade == "B")
            {
                result = "Bronze";
            }
            else
            {
                result = "Unknown";
            }
        }
        else if (score > 60)
        {
            result = isVip ? "Pass-Vip" : "Pass";
        }
        else if (score > 30)
        {
            result = "Retry";
        }
        else
        {
            result = "Fail";
        }
        return result;
    }

    // 质量: 空 catch 吞掉所有异常 (CodeQL: catching all exceptions)
    public static int SafeDivide(int a, int b)
    {
        try
        {
            return a / b;
        }
        catch
        {
            return 0;
        }
    }

    // 质量: 未使用变量 + 不可达代码 (return 后的代码)
    public static int DeadCode()
    {
        var unused = "this variable is never used";
        _ = unused;
        return _callCount++;
#pragma warning disable CS0162
        return -1; // 不可达
#pragma warning restore CS0162
    }

    // 质量: 可能的空引用解引用 (CodeQL: dereferencing a possibly-null reference)
    public static int NameLength(string? name)
    {
        return name.Length;
    }

    // 质量: 循环内字符串拼接 (性能反模式)
    public static string BuildCsv(int rows)
    {
        var csv = string.Empty;
        for (var i = 0; i < rows; i++)
        {
            csv += "row," + i + "," + DateTime.Now.ToString() + "\n";
        }
        return csv;
    }

    // 质量: 递归无终止条件 (CodeQL: unbounded recursion 风险)
    public static long Factorial(int n)
    {
        return n * Factorial(n - 1);
    }

    // 质量: 竞态条件: 非线程安全的静态计数器自增
    public static int NextCallCount()
    {
        _callCount = _callCount + 1;
        return _callCount;
    }

    // 质量: 浮点数 == 比较 (CodeQL: float equality)
    public static bool SamePrice(double a, double b)
    {
        return a == b;
    }
}
