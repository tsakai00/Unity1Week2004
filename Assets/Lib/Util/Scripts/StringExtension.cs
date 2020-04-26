using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Lib.Util
{
    /// <summary>
    /// 文字列に関する拡張メソッド
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// スネークケースに変換
        /// </summary>
        public static string ToSnake(this string self)
        {
            string str = self;
            str = str.Replace(" ", "_");                    // 半角スペースはアンダーバーに変更
            str = Regex.Replace(str, "[^_a-zA-Z0-9]", "");  // 英数字アンダーバー以外は削除
            str = Regex.Replace(str, "[a-z][A-Z]", m => m.Groups[0].Value[0] + "_" + m.Groups[0].Value[1]).ToLower();
            return str;
        }

        /// <summary>
        /// キャメルケースに変換
        /// </summary>
        public static string ToCamel(this string self)
        {
            string str = self;
            str = ToSnake(str);
            str = Regex.Replace(str, "_[a-z]", m => "" + char.ToUpper(m.Groups[0].Value[1]));
            return str;
        }

        /// <summary>
        /// パスカルケースに変換
        /// </summary>
        public static string ToPascal(this string self)
        {
            string str = self;
            str = ToCamel(str);
            var charArray = str.ToCharArray();
            charArray[0] = char.ToUpper(charArray[0]);
            return new string(charArray);
        }

        /// <summary>
        /// アッパースネークケースに変換
        /// </summary>
        public static string ToUpperSnake(this string self)
        {
            string str = self;
            str = ToSnake(str);
            str = str.ToUpper();
            return str;
        }
        
        /// <summary>
        /// インデントしてAppendLine()
        /// </summary>
        public static void AppendIndentLine(this StringBuilder self, string str, int depth)
        {
            for(int i = 0; i < depth; i++)
            {
                self.Append("    ");
            }
            self.AppendLine(str);
        }

        /// <summary>
        /// インデントしてWriteLine()
        /// </summary>
        public static void WriteIndentLine(this StreamWriter self, string str, int depth)
        {
            string indent = "";
            for(int i = 0; i < depth; i++)
            {
                indent += "    ";
            }
            
            self.Write(indent);
            self.WriteLine(str);
        }
    }
}
