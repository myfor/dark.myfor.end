/*  RELEASE NOTE
 *  Copyright (C) 2018 BIRENCHENS
 *  All right reserved
 *
 *  Filename:       Encrypt.cs
 *  Desctiption:    
 *
 *  CreateBy:       BIRENCHENS
 *  CreateDate:     2019-08-06 12:00:06
 *
 *  Version:        V1.0.0
 ***********************************************/

using System;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
	/// <summary>
	/// 加密解密
	/// </summary>
	public class Encrypt
	{
		/// <summary>
		/// SHA256 加密
		/// </summary>
		/// <param name="original">要加密的数据</param>
		/// <returns>加密后的字符</returns>
		public static string SHA256Encrypt(string original)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(original);
            using var en = SHA256.Create();

            byte[] hash = en.ComputeHash(bytes);

			StringBuilder builder = new StringBuilder(hash.Length);
			foreach (byte item in hash)
			{
				builder.Append(item.ToString("X2"));
			}

			return builder.ToString();
		}


		/// <summary>
		/// md5加密方式
		/// </summary>
		/// <param name="str">原字符串</param>
		/// <returns>加密后的字符串</returns>
		public static string MD5(string str)
		{
            using MD5 md5 = System.Security.Cryptography.MD5.Create();
            str = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", null);
			return str;
		}
	}
}
